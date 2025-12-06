using IdleSchemes.Core.Models;
using IdleSchemes.Core.Services;
using IdleSchemes.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Api.Controllers {
    [ApiController]
    [Route("events")]
    public class EventController : ControllerBase {

        private const int LIST_LIMIT = 40;

        private readonly ILogger<EventController> _logger;
        private readonly IdleDbContext _dbContext;
        private readonly TimeService _timeService;
        private readonly EventService _eventService;
        private readonly UserSessionService _userSessionService;

        public EventController(ILogger<EventController> logger, 
            IdleDbContext dbContext,
            TimeService timeService,
            EventService eventService,
            UserSessionService userSessionService) {
            _logger = logger;
            _dbContext = dbContext;
            _timeService = timeService;
            _eventService = eventService;
            _userSessionService = userSessionService;
        }

        [HttpGet("all/{eventId}", Name = "GetEvent")]
        public async Task<ActionResult<EventInstanceModel>> GetEvent(string eventId) {
            var instance = await _eventService.QueryEvents()
                .Where(e => e.Id == eventId)
                .SingleOrDefaultAsync();
            if (instance is null) {
                return NotFound();
            }
            return new EventInstanceModel(instance);
        }

        [HttpGet("all/{eventId}/tickets", Name = "GetAvailableTickets")]
        public async Task<ActionResult<PublicTicketCollection>> GetAvailableTickets(string eventId) {
            var instance = await _dbContext.EventInstances
                .Include(i => i.TicketClasses)
                .Where(e => e.Id == eventId)
                .SingleOrDefaultAsync();
            if (instance is null) {
                return NotFound();
            }
            var ticketCollection = await _eventService.GetAllTicketsAsync(instance);
            return Ok(new PublicTicketCollection(ticketCollection));
        }

        [HttpGet("regions/{regionId}", Name = "GetEventsInRegion")]
        public async Task<IEnumerable<EventInstanceModel>> GetEventsInRegion(string regionId, [FromQuery] DateTime? after = null, [FromQuery] int limit = 20) {
            var now = _timeService.GetNow();
            after = after ?? now;
            var instances = await _eventService.QueryEvents()
                .Where(i => i.Template.Organization.Region!.Id == regionId && i.Template.Organization.RegionApproved == true
                    && i.Published != null && i.Cancelled == null && i.ListInRegion == true 
                    && (i.RegistrationClose == null || i.RegistrationClose > now) && i.FirstSessionStarts > after)
                .OrderBy(i => i.FirstSessionStarts)
                .Take(Math.Min(limit, LIST_LIMIT))
                .ToListAsync();
            return instances
                .Select(i => new EventInstanceModel(i));
        }

        [HttpGet("orgs/{organizationId}", Name = "GetEventsByOrg")]
        public async Task<IEnumerable<EventInstanceModel>> GetEventsByOrg(string organizationId, [FromQuery] DateTime? after = null, [FromQuery] int limit = 20) {
            var now = _timeService.GetNow();
            after = after ?? now;
            var instances = await _eventService.QueryEvents()
                .Where(i => i.Template.Organization.Id == organizationId
                    && i.Published != null && i.Cancelled == null 
                    && (i.RegistrationClose == null || i.RegistrationClose > now) && i.FirstSessionStarts > after )
                .OrderBy(i => i.FirstSessionStarts)
                .Take(Math.Min(limit, LIST_LIMIT))
                .ToListAsync();
            return instances
                .Select(i => new EventInstanceModel(i));
        }

        [HttpGet("associates/{associateId}", Name = "GetEventsByAssociate")]
        public async Task<IEnumerable<EventInstanceModel>> GetEventsByAssociate(string associateId, [FromQuery] DateTime? after = null, [FromQuery] int limit = 20) {
            var now = _timeService.GetNow();
            after = after ?? now;
            var eventIds = await _dbContext.Hosts
                .Include(h => h.Instance)
                .Where(eh => eh.Associate.Id == associateId
                    && eh.Instance.Published != null && eh.Instance.Cancelled == null 
                    && (eh.Instance.RegistrationClose == null || eh.Instance.RegistrationClose > now) && eh.Instance.FirstSessionStarts > after)
                .OrderBy(eh => eh.Instance.FirstSessionStarts)
                .Select(eh => eh.Instance.Id)
                .Take(Math.Min(limit, LIST_LIMIT))
                .ToListAsync();
            var instances = await _eventService.QueryEvents()
                .Where(e => eventIds.Contains(e.Id))
                .ToListAsync();
            return instances
                .Select(i => new EventInstanceModel(i));
        }

    }
}
