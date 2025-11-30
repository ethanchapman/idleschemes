using IdleSchemes.Core.Helpers;
using IdleSchemes.Core.Services;
using IdleSchemes.Data;
using IdleSchemes.Data.Models.Events;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.WebAdmin.ViewModels {
    public class FinancesViewModel : ListViewModel<FinancesViewModel.RegistrationInfo> {

        private readonly IdleDbContext _dbContext;
        private readonly DateTime _now;

        public FinancesViewModel(IdleDbContext dbContext, TimeService timeService) {
            _dbContext = dbContext;
            _now = timeService.GetNow();
        }

        public string Title { get; } = "Finances";
        public int Sum { get; private set; } = 0;
        public FinanceFilterOption FinanceFilter { get; private set; } = FinanceFilterOption.Payable;

        public async Task SetFinanceFilterAsync(FinanceFilterOption financeFilterOption) {
            FinanceFilter = financeFilterOption;
            await ResetAsync();
        }

        protected override async Task<int> CountAllAsync() {
            Sum = await GetQuery()
                .SumAsync(r => r.Cost);
            return await GetQuery()
                .CountAsync();
        }

        protected override async Task<List<RegistrationInfo>> FetchPageAsync(int skip) {
            var dbResults = await GetQuery()
                .Include(r => r.Instance)
                .Include(r => r.Tickets)
                .Include(r => r.User)
                .OrderByDescending(r => r.Confirmed)
                .Skip(skip)
                .ToListAsync();
            return dbResults
                .Select(r => new RegistrationInfo(r, this))
                .ToList();
        }

        private IQueryable<Registration> GetQuery() {
            if (FinanceFilter == FinanceFilterOption.Payable) {
                return _dbContext.Registrations.Where(r => r.Instance.Template.Organization == CurrentSession!.ActiveAssociation!.Organization
                    && r.Confirmed > CurrentSession!.ActiveAssociation!.Organization.LastPayout && r.Cost > 0 && r.Instance.CancellationDeadline < _now);
            } else if (FinanceFilter == FinanceFilterOption.Pending) {
                return _dbContext.Registrations.Where(r => r.Instance.Template.Organization == CurrentSession!.ActiveAssociation!.Organization
                    && r.Confirmed > CurrentSession!.ActiveAssociation!.Organization.LastPayout && r.Cost > 0 && r.Instance.CancellationDeadline > _now);
            } else {
                return _dbContext.Registrations.Where(r => r.Instance.Template.Organization == CurrentSession!.ActiveAssociation!.Organization
                    && r.Confirmed != null && r.Cost > 0);
            }
        }

        public enum FinanceFilterOption {
            All,
            Pending,
            Payable
        }

        public class RegistrationInfo {
            private readonly FinancesViewModel _viewModel;
            public RegistrationInfo(Registration registration, FinancesViewModel viewModel) {
                Registration = registration;
                _viewModel = viewModel;
            }

            public Registration Registration { get; }
            public string Confirmed => TimeHelper.GetDateTimeString(Registration.Confirmed!.Value, _viewModel.TimeZoneId);
        }
    }
}
