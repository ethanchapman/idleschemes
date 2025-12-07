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

        public async Task<TicketCollection> GetAllTicketsAsync(EventInstance eventInstance, DateTime? now = null) {
            now = now ?? _timeService.GetNow();
            var timeoutLimit = now.Value - REGISTRATION_TIMEOUT;
            var ticketClasses = await _dbContext.TicketClasses
                .Include(tc => tc.Tickets)
                .ThenInclude(t => t.Registration)
                .Where(tc => tc.Instance == eventInstance)
                .ToListAsync();
            return new TicketCollection(ticketClasses, timeoutLimit);
        }

    }
}
