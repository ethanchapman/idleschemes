using IdleSchemes.Api.Models;
using IdleSchemes.Core.Helpers;
using IdleSchemes.Core.Models;
using IdleSchemes.Core.Services;
using IdleSchemes.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Api.Controllers {
    [ApiController]
    [Route("user/events")]
    public class UserEventsController : ControllerBase {

        private const int LIST_LIMIT = 40;

        private readonly ILogger<UserEventsController> _logger;
        private readonly UserSessionService _userSessionService;
        private readonly IdleDbContext _dbContext;
        private readonly TimeService _timeService;

        public UserEventsController(ILogger<UserEventsController> logger,
            UserSessionService userSessionService, 
            IdleDbContext dbContext,
            TimeService timeService) {
            _logger = logger;
            _userSessionService = userSessionService;
            _dbContext = dbContext;
            _timeService = timeService;
        }

        [HttpGet("tickets", Name = "GetMyTickets")]
        public async Task<ActionResult<IEnumerable<UserTicketModel>>> GetMyTickets(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] int limit = 20) {
            if (_userSessionService.CurrentUser is null) {
                return Unauthorized();
            }
            TimeHelper.SetFromAndToIfNecessary(ref from, ref to, _timeService.GetNow(), out var order, TimeSpan.FromDays(32), TimeSpan.FromDays(365));
            var tickets = await _dbContext.TicketClaims
                .Include(t => t.User)
                .Include(t => t.Registration)
                .ThenInclude(r => r.Instance)
                .Include(t => t.Registration)
                .ThenInclude(r => r.User)
                .Where(t => t.User == _userSessionService.CurrentUser
                    && t.Registration.Confirmed != null
                    && t.Registration.Instance.FirstSessionStarts > from.Value && t.Registration.Instance.FirstSessionStarts < to.Value)
                .OrderByTime(t => t.Registration.Instance.FirstSessionStarts, order)
                .Take(Math.Min(limit, LIST_LIMIT))
                .ToListAsync();
            return Ok(tickets
                .Select(t => new UserTicketModel(t)));
        }

        [HttpGet("registrations", Name = "GetMyEventRegistrations")]
        public async Task<ActionResult<IEnumerable<UserRegistrationModel>>> GetMyEventRegistrations(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] int limit = 20) {
            if (_userSessionService.CurrentUser is null) {
                return Unauthorized();
            }
            TimeHelper.SetFromAndToIfNecessary(ref from, ref to, _timeService.GetNow(), out var order, TimeSpan.FromDays(32), TimeSpan.FromDays(365));
            var registrations = await _dbContext.Registrations
                .Include(r => r.Instance)
                .Include(r => r.Claims)
                .ThenInclude(t => t.User)
                .Include(r => r.Claims)
                .ThenInclude(t => t.Ticket)
                .ThenInclude(t => t.TicketClass)
                .Where(r => r.User == _userSessionService.CurrentUser
                    && r.Confirmed != null
                    && r.Instance.FirstSessionStarts > from.Value && r.Instance.FirstSessionStarts < to.Value)
                .OrderByTime(r => r.Instance.FirstSessionStarts, order)
                .Take(Math.Min(limit, LIST_LIMIT))
                .ToListAsync();
            return Ok(registrations
                .Select(r => new UserRegistrationModel(r)));
        }

        [HttpGet("registrations/{registrationId}", Name = "GetEventRegistration")]
        public async Task<ActionResult<UserRegistrationModel>> GetEventRegistration(string registrationId) {
            if (_userSessionService.CurrentUser is null) {
                return Unauthorized();
            }
            var registration = await _dbContext.Registrations
                .Include(r => r.Instance)
                .Include(r => r.Claims)
                .ThenInclude(t => t.User)
                .Where(r => r.Id == registrationId 
                    && r.User == _userSessionService.CurrentUser && r.Confirmed != null)
                .FirstOrDefaultAsync();
            if(registration is null) {
                return NotFound();
            }
            return Ok(new UserRegistrationModel(registration));
        }

        [HttpDelete("registrations/{registrationId}", Name = "CancelEventRegistration")]
        public async Task<ActionResult> CancelEventRegistration(string registrationId) {
            var now = _timeService.GetNow();
            if (_userSessionService.CurrentUser is null) {
                return Unauthorized();
            }
            var registration = await _dbContext.Registrations
                .Include(r => r.Instance)
                .SingleOrDefaultAsync(r => r.Id == registrationId);
            if(registration is null) {
                return NotFound();
            }
            var deadline = registration.Instance.CancellationDeadline ?? registration.Instance.FirstSessionStarts ?? DateTime.MinValue;
            if(now > deadline) {
                return BadRequest();
            }

            registration.Cancelled = now;
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

    }
}
