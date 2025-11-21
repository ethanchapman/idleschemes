using IdleSchemes.Core.Models.Input;
using IdleSchemes.Core.Services;
using IdleSchemes.Data;
using IdleSchemes.Data.Models.Events;
using IdleSchemes.Data.Models.Organizations;

namespace IdleSchemes.Tests.Helpers {
    public class TestEventHelper {

        private readonly IdleDbContext _dbContext;
        private readonly IdService _idService;
        private readonly TimeService _timeService;

        public TestEventHelper(IdleDbContext dbContext, IdService idService, TimeService timeService) {
            _dbContext = dbContext;
            _idService = idService;
            _timeService = timeService;
        }

        public EventInstance CreateEventInstance(string name,
            Organization organization,
            IEnumerable<Associate> associates,
            IEnumerable<(DateTime, DateTime)> sessions,
            List<TicketClassCreationOptions> ticketClasses) {

            var now = _timeService.GetNow();

            var id = _idService.GenerateId();
            var ev = _dbContext.EventInstances.Add(new EventInstance {
                Id = id,
                Template = new EventTemplate { 
                    Id = id,
                    Organization = organization
                },
                Info = new EventInfo {
                    Name = name
                },
                Created = now
            }).Entity;

            ev.Hosts = associates
                .Select(a => new Host {
                    Instance = ev,
                    Associate = a,
                }).ToList();

            ev.Sessions = sessions
                .Select(s => new EventSession {
                    Instance = ev,
                    StartTime = s.Item1,
                    EndTime = s.Item2
                })
                .ToList();
            ev.FirstSessionStarts = ev.Sessions.OrderBy(s => s.StartTime).First().StartTime;
            ev.LastSessionEnds = ev.Sessions.OrderBy(s => s.StartTime).Last().EndTime;

            _dbContext.TicketClasses.AddRange(ticketClasses.Select((tc, i) => new TicketClass {
                Id = _idService.GenerateId(),
                Instance = ev,
                Name = tc.Name,
                OrderSeq = i,
                TotalCount = tc.Available,
                BasePrice = tc.BasePrice,
                CanRegister = tc.CanRegister,
            }));

            return ev;
        }
    }
}
