using IdleSchemes.Core.Helpers;
using IdleSchemes.Core.Services;
using IdleSchemes.Data;
using IdleSchemes.Data.Models.Events;
using IdleSchemes.Data.Models.Organizations;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.WebAdmin.ViewModels.Organizations {
    public class PaymentsViewModel : ListViewModel<Payment> {

        private readonly IdleDbContext _dbContext;
        private readonly DateTime _now;

        public PaymentsViewModel(IdleDbContext dbContext, TimeService timeService) {
            _dbContext = dbContext;
            _now = timeService.GetNow();
        }

        public string Title { get; } = "Payments";
        public int Sum { get; private set; } = 0;
        public FinanceFilterOption FinanceFilter { get; private set; } = FinanceFilterOption.Pending;

        public async Task SetFinanceFilterAsync(FinanceFilterOption financeFilterOption) {
            FinanceFilter = financeFilterOption;
            await ResetAsync();
        }

        protected override async Task<int> CountAllAsync() {
            Sum = await GetQuery()
                .SumAsync(r => r.Amount);
            return await GetQuery()
                .CountAsync();
        }

        protected override Task<List<Payment>> FetchPageAsync(int skip) {
            return GetQuery()
                .Include(p => p.Registration)
                .OrderByDescending(r => r.Confirmed)
                .Skip(skip)
                .ToListAsync();
        }

        private IQueryable<Payment> GetQuery() {
            var organization = CurrentSession!.ActiveAssociation!.Organization;
            if (FinanceFilter == FinanceFilterOption.Cancelled) {
                return _dbContext.Payments
                    .Where(p => p.Organization == CurrentOrganization && p.Cancelled != null);
            } else if (FinanceFilter == FinanceFilterOption.Paid) {
                return _dbContext.Payments
                    .Where(p => p.Organization == CurrentOrganization && p.Paid != null);
            } else if (FinanceFilter == FinanceFilterOption.Pending) {
                return _dbContext.Payments
                    .Where(p => p.Organization == CurrentOrganization && p.Cancelled == null && p.Paid == null);
            } else {
                return _dbContext.Payments
                    .Where(p => p.Organization == CurrentOrganization);
            }
        }

        public enum FinanceFilterOption {
            All,
            Pending,
            Paid,
            Cancelled
        }

        public class RegistrationInfo {
            private readonly PaymentsViewModel _viewModel;
            public RegistrationInfo(Registration registration, PaymentsViewModel viewModel) {
                Registration = registration;
                _viewModel = viewModel;
            }

            public Registration Registration { get; }
            public string Confirmed => TimeHelper.GetDateTimeString(Registration.Confirmed!.Value, _viewModel.TimeZoneId);
        }
    }
}
