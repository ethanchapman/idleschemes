using IdleSchemes.Core.Helpers;
using IdleSchemes.Core.Models;
using IdleSchemes.Data;
using IdleSchemes.Data.Models.Events;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Core.Services {
    public class EventService {

        private static readonly TimeSpan REGISTRATION_TIMEOUT = TimeSpan.FromMinutes(10);

        private readonly IdleDbContext _dbContext;
        private readonly TimeService _timeService;

        public EventService(IdleDbContext dbContext, TimeService timeService) {
            _dbContext = dbContext;
            _timeService = timeService;
        }

        public IQueryable<EventInstance> QueryEvents() {
            return _dbContext.EventInstances
                .Include(e => e.Template)
                .ThenInclude(t => t.Organization);
        }

        public IQueryable<Ticket> QueryTickets() {
            return _dbContext.Tickets
                .Include(t => t.Registration)
                .Include(t => t.TicketClass);
        }

        public async Task<List<TicketClassModel>> GetTicketClassesWithAvailabilityAsync(EventInstance eventInstance, List<string>? onlyClassNames = null, DateTime? now = null) {
            now = now ?? _timeService.GetNow();
            if(eventInstance.TicketClasses.Count == 0) {
                await _dbContext.Entry(eventInstance)
                    .Reference(i => i.TicketClasses)
                    .LoadAsync();
            }
            var timeoutLimit = now - REGISTRATION_TIMEOUT;
            List<IGrouping<TicketClass, Ticket>> ticketsByClass;
            if (onlyClassNames is not null) {
                ticketsByClass = await QueryTickets()
                    .Where(t => t.Registration.Instance == eventInstance && onlyClassNames.Contains(t.TicketClass.Name)
                        && t.Registration.TicketsClaimed != null && t.Registration.TicketsClaimed > timeoutLimit
                        && t.Registration.Cancelled == null)
                    .GroupBy(t => t.TicketClass)
                    .ToListAsync();
            } else {
                ticketsByClass = await QueryTickets()
                    .Where(t => t.Registration.Instance == eventInstance
                        && t.Registration.TicketsClaimed != null && t.Registration.TicketsClaimed > timeoutLimit
                        && t.Registration.Cancelled == null)
                    .GroupBy(t => t.TicketClass)
                    .ToListAsync();
            }
            List<TicketClassModel> results = new List<TicketClassModel>();
            foreach(var ticketClass in eventInstance.TicketClasses.OrderBy(tc => tc.OrderSeq)) {
                var availability = new TicketClassAvailabilityModel {
                    TotalCount = ticketClass.TotalCount,
                    ConfirmedCount = 0,
                    PendingCount = 0,
                };
                var ticketsInClass = ticketsByClass
                    .FirstOrDefault(tbc => tbc.Key.Id == ticketClass.Id);
                if (ticketsInClass is not null) {
                    CalculateAvailability(ticketClass, availability, ticketsInClass);
                } else if(ticketClass.SeatOptions is not null) {
                    availability.RemainingSeats = SerializationHelper.Deserialize<List<string>>(ticketClass.SeatOptions);
                }
                results.Add(new TicketClassModel(ticketClass) {
                    Availability = availability
                });
            }
            return results;
        }

        private void CalculateAvailability(TicketClass ticketClass, TicketClassAvailabilityModel availability, IEnumerable<Ticket> ticketsInClass) {
            var confirmedTicketsInClass = ticketsInClass
                .Where(t => t.Registration.Confirmed is not null)
                .ToList();
            var pendingTicketsInClass = ticketsInClass
                .Where(t => t.Registration.Confirmed is null)
                .ToList();
            availability.ConfirmedCount = confirmedTicketsInClass.Count;
            availability.PendingCount = pendingTicketsInClass.Count;
            if (!string.IsNullOrEmpty(ticketClass.SeatOptions)) {
                GetAvailableAndPendingTickets(ticketClass.SeatOptions, availability, confirmedTicketsInClass, pendingTicketsInClass);
            }
        }

        private void GetAvailableAndPendingTickets(string seatOptions, TicketClassAvailabilityModel availability, List<Ticket> confirmedTicketsInClass, List<Ticket> pendingTicketsInClass) {
            var allSeats = SerializationHelper.Deserialize<List<string>>(seatOptions);
            var confirmedSeats = confirmedTicketsInClass
                .Where(t => !string.IsNullOrEmpty(t.Seat))
                .Select(t => t.Seat!)
                .ToList();
            availability.PendingSeats = pendingTicketsInClass
                .Where(t => !string.IsNullOrEmpty(t.Seat))
                .Select(t => t.Seat!)
                .ToList();
            availability.RemainingSeats = allSeats
                .Except(availability.PendingSeats.Union(confirmedSeats))
                .ToList();
        }

    }
}
