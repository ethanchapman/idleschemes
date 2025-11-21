using IdleSchemes.Core.Services;
using IdleSchemes.Data;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.WebAdmin.ViewModels {
    public class HomeViewModel : ViewModelBase {

        private readonly IdleDbContext _dbContext;
        private readonly TimeService _timeService;

        public HomeViewModel(IdleDbContext dbContext, TimeService timeService) {
            _dbContext = dbContext;
            _timeService = timeService;
        }

        public string Title { get; } = "Home";

        public int EventsToday { get; private set; }
        public int RegistrationsToday { get; private set; }
        public string RevenueToday { get; private set; } = "";

        protected override async Task InitializeAsync() {
            var now = _timeService.GetNow();
            var today = now.Date;
            var tomorrow = today.AddDays(1);

            EventsToday = await _dbContext.EventInstances
                .SelectMany(ei => ei.Sessions)
                .Where(s => s.StartTime > today && s.StartTime < tomorrow)
                .CountAsync();

            RegistrationsToday = await _dbContext.Registrations
                .Where(r => r.Confirmed > today && r.Confirmed < tomorrow)
                .CountAsync();

            var revenueToday = await _dbContext.Registrations
                .Where(r => r.Confirmed > today && r.Confirmed < tomorrow)
                .SumAsync(r => r.Cost);
            RevenueToday = "$" + (revenueToday * 0.01m).ToString("F2");

        }
    }
}
