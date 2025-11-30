using IdleSchemes.Core.Helpers;
using IdleSchemes.Data;
using IdleSchemes.Data.Models.Events;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.WebAdmin.ViewModels {
    public class RegistrationsViewModel : ListViewModel<RegistrationsViewModel.RegistrationInfo> {

        private readonly IdleDbContext _dbContext;

        public RegistrationsViewModel(IdleDbContext dbContext) {
            _dbContext = dbContext;
        }

        public string Title { get; } = "Registrations";

        protected override Task<int> CountAllAsync() {
            return _dbContext.Registrations
                .Where(r => r.Confirmed != null && r.Instance.Template.Organization == CurrentSession!.ActiveAssociation!.Organization)
                .CountAsync();
        }

        protected override async Task<List<RegistrationInfo>> FetchPageAsync(int skip) {
            var dbResults = await _dbContext.Registrations
                .Where(r => r.Confirmed != null && r.Instance.Template.Organization == CurrentSession!.ActiveAssociation!.Organization)
                .Include(r => r.Instance)
                .Include(r => r.Tickets)
                .Include(r => r.User)
                .OrderBy(r => r.Confirmed)
                .Skip(skip)
                .ToListAsync();
            return dbResults
                .Select(r => new RegistrationInfo(r, this))
                .ToList();
        }

        public class RegistrationInfo {
            private readonly RegistrationsViewModel _viewModel;
            public RegistrationInfo(Registration registration, RegistrationsViewModel viewModel) {
                Registration = registration;
                _viewModel = viewModel;
            }

            public Registration Registration { get; }
            public string Confirmed => TimeHelper.GetDateTimeString(Registration.Confirmed!.Value, _viewModel.TimeZoneId);
        }
    }
}
