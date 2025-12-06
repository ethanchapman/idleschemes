using IdleSchemes.Core.Services;
using IdleSchemes.Data;
using IdleSchemes.Data.Models.Organizations;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.WebAdmin.ViewModels.Organizations {
    public class MyPaymentsViewModel : ListViewModel<Payment> {

        private readonly IdleDbContext _dbContext;
        private readonly DateTime _now;

        public MyPaymentsViewModel(IdleDbContext dbContext, TimeService timeService) {
            _dbContext = dbContext;
            _now = timeService.GetNow();
        }

        public string Title { get; } = "My Payments";
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
            if (FinanceFilter == FinanceFilterOption.Cancelled) {
                return _dbContext.Payments
                    .Where(p => p.Associate!.User.Id == CurrentSession!.User!.Id && p.Cancelled != null);
            } else if (FinanceFilter == FinanceFilterOption.Paid) {
                return _dbContext.Payments
                    .Where(p => p.Associate!.User.Id == CurrentSession!.User!.Id && p.Paid != null);
            } else if (FinanceFilter == FinanceFilterOption.Pending) {
                return _dbContext.Payments
                    .Where(p => p.Associate!.User.Id == CurrentSession!.User!.Id && p.Cancelled == null && p.Paid == null);
            } else {
                return _dbContext.Payments
                    .Where(p => p.Associate!.User.Id == CurrentSession!.User!.Id);
            }
        }

        public enum FinanceFilterOption {
            All,
            Pending,
            Paid,
            Cancelled
        }

    }
}
