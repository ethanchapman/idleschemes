using IdleSchemes.Core.Helpers;
using IdleSchemes.Core.Services;
using IdleSchemes.Data;
using IdleSchemes.Data.Models.Events;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.WebAdmin.ViewModels {
    public class EventsViewModel : ListViewModel<EventsViewModel.EventInfo> {

        private readonly IdleDbContext _dbContext;
        private readonly TimeService _timeService;

        public EventsViewModel(IdleDbContext dbContext, TimeService timeService) {
            _dbContext = dbContext;
            _timeService = timeService;
        }

        public string Title { get; } = "Events";
        public EventFilterOption EventFilter { get; private set; } = EventFilterOption.Upcoming;

        public async Task SetEventFilterAsync(EventFilterOption eventFilterOption) {
            EventFilter = eventFilterOption;
            await ResetAsync();
        }

        protected override Task<int> CountAllAsync() {
            return GetQuery().CountAsync();
        }


        protected override async Task<List<EventInfo>> FetchPageAsync(int skip) {
            var dbResults = await GetQuery()
                .OrderBy(e => e.FirstSessionStarts)
                .Skip(skip)
                .ToListAsync();
            return dbResults
                .Select(e => new EventInfo(e, this))
                .ToList();
        }

        private IQueryable<EventInstance> GetQuery() {
            var today = ConvertToUtc(_timeService.GetNow(TimeZoneId!).Date);
            if (EventFilter == EventFilterOption.Upcoming) {
                return _dbContext.EventInstances
                    .Where(e => e.Template.Organization == CurrentSession!.ActiveAssociation!.Organization && e.LastSessionEnds > today);
            } else {
                return _dbContext.EventInstances
                    .Where(e => e.Template.Organization == CurrentSession!.ActiveAssociation!.Organization && e.LastSessionEnds < today);
            }
        }

        public enum EventFilterOption {
            Upcoming,
            Past
        }

        public class EventInfo {
            private readonly EventsViewModel _viewModel;
            public EventInfo(EventInstance eventInstance, EventsViewModel viewModel) {
                Instance = eventInstance;
                _viewModel = viewModel;
            }

            public EventInstance Instance { get; }
            public DateTime Created => _viewModel.ConvertFromUtc(Instance.Created);
            public string Starts => TimeHelper.GetRangeStartString(Instance.FirstSessionStarts!.Value, Instance.LastSessionEnds!.Value, _viewModel.TimeZoneId);
            public string Ends => TimeHelper.GetRangeEndString(Instance.FirstSessionStarts!.Value, Instance.LastSessionEnds!.Value, _viewModel.TimeZoneId);
        }
    }
}
