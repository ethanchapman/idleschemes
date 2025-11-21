using IdleSchemes.Api.Models;
using IdleSchemes.Api.Models.Input;
using IdleSchemes.Core.Services;
using IdleSchemes.Data;
using IdleSchemes.Data.Models.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Api.Controllers {
    [ApiController]
    [Route("registrations")]
    public class RegistrationController : ControllerBase {

        public TimeSpan REGISTRATION_GRACE_PERIOD = TimeSpan.FromMinutes(10);

        private readonly ILogger<RegistrationController> _logger;
        private readonly IdleDbContext _dbContext;
        private readonly UserSessionService _userSessionService;
        private readonly IdService _idService;
        private readonly TimeService _timeService;
        private readonly EventService _eventService;

        public RegistrationController(ILogger<RegistrationController> logger,
            IdleDbContext dbContext, 
            UserSessionService userSessionService, 
            IdService idService,
            TimeService timeService,
            EventService eventService) {
            _logger = logger;
            _dbContext = dbContext;
            _userSessionService = userSessionService;
            _idService = idService;
            _timeService = timeService;
            _eventService = eventService;
        }

        [HttpPost("start", Name = "StartRegistration")]
        public async Task<ActionResult<RegistrationInfoModel>> StartRegistration([FromBody] StartRegistrationModel model) {
            if (_userSessionService.CurrentUser is null) {
                return Unauthorized();
            }

            var instance = await _dbContext.EventInstances.SingleOrDefaultAsync(ei => ei.Id == model.InstanceId);
            if(instance is null) {
                return NotFound();
            }
            if(instance.Cancelled is not null || instance.Filled is not null) {
                return NotFound();
            }

            var registration = _dbContext.Registrations.Add(new Registration {
                Id = _idService.GenerateId(),
                Secret = _idService.GenerateSecret(32),
                Started = _timeService.GetNow(),
                Instance = instance,
                User = _userSessionService.CurrentUser
            }).Entity;

            await _dbContext.SaveChangesAsync();

            return Ok(new RegistrationInfoModel {
                Id = registration.Id,
                Secret = registration.Secret,
                Started = registration.Started
            });
        }

        [HttpPost("reserve", Name = "ReserveTickets")]
        public async Task<ActionResult<ReservationInfoModel>> ReserveTickets([FromBody] ReserveTicketsModel model) {
            var now = _timeService.GetNow();
            var registration = await _dbContext.Registrations
                .Include(r => r.Instance)
                .ThenInclude(i => i.TicketClasses)
                .SingleOrDefaultAsync(r => r.Id == model.RegistrationId && r.Secret == r.Secret);
            if(registration is null) {
                return NotFound();
            }

            if (registration.Cancelled is not null) {
                _logger.LogInformation("Registration was cancelled" +
                    " (Registration {RegistrationId} Event {EventId})",
                    registration.Id, registration.Instance.Id);
                return BadRequest();
            }

            if (registration.TicketsClaimed is not null) {
                _logger.LogInformation("Tickets already claimed for registration" +
                    " (Registration {RegistrationId} Event {EventId})",
                    registration.Id, registration.Instance.Id);
                return BadRequest();
            }

            if (registration.Instance.Cancelled is not null || registration.Instance.Filled is not null) {
                _logger.LogInformation("Event is full or cancelled" +
                    " (Registration {RegistrationId} Event {EventId})",
                    registration.Id, registration.Instance.Id);
                return BadRequest();
            }

            if (registration.Instance.RegistrationClose is not null && registration.Instance.RegistrationClose < now) {
                _logger.LogInformation("Registration is closed for event" +
                    " (Registration {RegistrationId} Event {EventId})",
                    registration.Id, registration.Instance.Id);
                return BadRequest();
            }

            var ticketLimit = registration.Instance.Info.IndividualTicketLimit;
            var totalTicketCount = model.TicketReservations.Sum(r => r.Count);
            if (ticketLimit is not null && model.TicketReservations.Sum(r => r.Count) > ticketLimit) {
                _logger.LogInformation("Too many tickets in reservation" +
                    " ({totalTicketCount}/{ticketLimit} Registration {RegistrationId} Event {EventId})",
                    totalTicketCount, ticketLimit, registration.Id, registration.Instance.Id);
                return BadRequest();
            }

            var ticketClassesWithAvailability = await _eventService.GetTicketClassesWithAvailabilityAsync(registration.Instance,
                model.TicketReservations.Select(t => t.Class).ToList(),
                now);
            var newTickets = new List<Ticket>();
            foreach(var reservation in model.TicketReservations) {
                var ticketClass = registration.Instance.TicketClasses
                    .FirstOrDefault(tc => tc.Name == reservation.Class);
                if (ticketClass is null) {
                    _logger.LogInformation("Ticket class that does not exist" +
                        " ({Class} Registration {RegistrationId} Event {EventId})",
                        reservation.Class, registration.Id, registration.Instance.Id);
                    return BadRequest();
                }

                if (!ticketClass.CanRegister) {
                    _logger.LogInformation("Ticket class is unavailable" +
                        " ({Class} Registration {RegistrationId} Event {EventId})",
                        ticketClass, registration.Id, registration.Instance.Id);
                    return BadRequest();
                }

                if(!string.IsNullOrEmpty(ticketClass.InviteCode) && reservation.Code != ticketClass.InviteCode) {
                    _logger.LogInformation("Invalid invite code" +
                        " ({Class} Registration {RegistrationId} Event {EventId})",
                        ticketClass, registration.Id, registration.Instance.Id);
                    return BadRequest();
                }

                var availabilityInfo = ticketClassesWithAvailability
                    .FirstOrDefault(tcwa => tcwa.Id == ticketClass.Id)?.Availability;
                if (availabilityInfo is null) {
                    _logger.LogInformation("Availability information not found for ticket class" +
                        " ({Class} Registration {RegistrationId} Event {EventId})",
                        reservation.Class, registration.Id, registration.Instance.Id);
                    return BadRequest();
                }

                if (availabilityInfo.RemainingCount is not null && availabilityInfo.RemainingCount < reservation.Count) {
                    _logger.LogInformation("Not enough tickets available for class" +
                        " ({Count}/{Remaining} Class {Class} Registration {RegistrationId} Event {EventId})",
                        reservation.Count, availabilityInfo.RemainingCount, reservation.Class, registration.Id, registration.Instance.Id);
                    return BadRequest();
                }
                if (string.IsNullOrEmpty(ticketClass.SeatOptions) && reservation.Count is not null) {
                    // No seats, reserve via simple count
                    newTickets.AddRange(Enumerable.Range(0, reservation.Count.Value).Select(i => new Ticket {
                        Id = _idService.GenerateId(),
                        Registration = registration,
                        TicketClass = ticketClass
                    }));
                    registration.Cost += ticketClass.BasePrice * reservation.Count.Value;
                } else if (!string.IsNullOrEmpty(ticketClass.SeatOptions) && reservation.Count is not null && reservation.Seats is null
                        && (ticketClass.SeatSelection == SeatSelection.Allow || ticketClass.SeatSelection == SeatSelection.Deny)) {
                    // Auto-assign seats
                    newTickets.AddRange(availabilityInfo.RemainingSeats.Take(reservation.Count.Value).Select(t => new Ticket {
                        Id = _idService.GenerateId(),
                        Registration = registration,
                        TicketClass = ticketClass,
                        Seat = t
                    }));
                } else if (!string.IsNullOrEmpty(ticketClass.SeatOptions) && reservation.Count is null && reservation.Seats is not null
                        && (ticketClass.SeatSelection == SeatSelection.Allow || ticketClass.SeatSelection == SeatSelection.Require)) {
                    // Specific seats
                    var unavailableSeats = reservation.Seats
                        .Where(s => !availabilityInfo.RemainingSeats.Contains(s.Seat ?? ""));
                    if (unavailableSeats.Any()) {
                        var unavailableStr = string.Join(",", unavailableSeats);
                        _logger.LogInformation("Reservation included unavailable seats" +
                            " ({unavailableStr} Class {Class} Registration {RegistrationId} Event {EventId})",
                            unavailableStr, reservation.Class, registration.Id, registration.Instance.Id);
                        return BadRequest();
                    }
                    newTickets.AddRange(reservation.Seats.Select(t => new Ticket {
                        Id = _idService.GenerateId(),
                        Registration = registration,
                        TicketClass = ticketClass,
                        Seat = t.Seat,
                        HolderName = t.HolderName
                    }));
                } else {
                    _logger.LogInformation("Invalid ticket reservation" +
                        " (Class {Class} Registration {RegistrationId} Event {EventId})",
                        reservation.Class, registration.Id, registration.Instance.Id);
                    return BadRequest();
                }

            }

            _dbContext.Tickets.AddRange(newTickets);
            registration.TicketsClaimed = now;
            await _dbContext.SaveChangesAsync();

            return Ok(new ReservationInfoModel {
                Id = registration.Id,
                Tickets = newTickets.Select(t => new ReservationInfoModel.Ticket { 
                    Id = t.Id,
                    Class = t.TicketClass.Name, 
                    Seat = t.Seat 
                }).ToList()
            });
        }

        [HttpPost("confirm", Name = "ConfirmPayment")]
        public async Task<ActionResult> ConfirmTickets([FromBody] ConfirmRegistrationModel model) {
            var registration = await _dbContext.Registrations
                .SingleOrDefaultAsync(r => r.Id == model.RegistrationId && r.Secret == r.Secret);
            if (registration is null) {
                return NotFound();
            }
            if (registration.Cancelled is not null) {
                return NotFound();
            }
            registration.Confirmed = _timeService.GetNow();
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

    }
}
