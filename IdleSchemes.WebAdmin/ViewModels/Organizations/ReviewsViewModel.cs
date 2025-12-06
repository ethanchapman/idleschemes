using IdleSchemes.Core.Helpers;
using IdleSchemes.Data;
using IdleSchemes.Data.Models.Events;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.WebAdmin.ViewModels.Organizations {
    public class ReviewsViewModel : ListViewModel<ReviewsViewModel.ReviewRow> {

        private readonly IdleDbContext _dbContext;

        public ReviewsViewModel(IdleDbContext dbContext) {
            _dbContext = dbContext;
        }

        public string Title { get; } = "Reviews";

        protected override Task<int> CountAllAsync() {
            return _dbContext.Reviews
                .Where(r => r.Ticket.Registration!.Instance.Template.Organization == CurrentSession!.ActiveAssociation!.Organization)
                .CountAsync();
        }

        protected override async Task<List<ReviewRow>> FetchPageAsync(int skip) {
            var dbResults = await _dbContext.Reviews
                .Include(r => r.Ticket)
                .ThenInclude(t => t.Registration)
                .ThenInclude(r => r!.Instance)
                .ThenInclude(i => i.Template)
                .ThenInclude(t => t.Organization)
                .Where(r => r.Ticket.Registration!.Instance.Template.Organization == CurrentSession!.ActiveAssociation!.Organization)
                .OrderBy(r => r.Submitted)
                .Skip(skip)
                .ToListAsync();
            return dbResults
                .Select(r => new ReviewRow(r, this))
                .ToList();
        }

        public class ReviewRow {
            private readonly ReviewsViewModel _viewModel;
            public ReviewRow(Review review, ReviewsViewModel viewModel) {
                Instance = review;
                _viewModel = viewModel;
            }
            public Review Instance { get; }
            public string Submitted => TimeHelper.GetDateTimeString(Instance.Submitted, _viewModel.TimeZoneId);
        }
    }
}
