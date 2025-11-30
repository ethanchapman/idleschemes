using IdleSchemes.Core.Helpers;
using IdleSchemes.Data;
using IdleSchemes.Data.Models.Organizations;
using Microsoft.EntityFrameworkCore;
using static IdleSchemes.WebAdmin.ViewModels.PatronsViewModel;

namespace IdleSchemes.WebAdmin.ViewModels {
    public class PatronsViewModel : ListViewModel<PatronRow> {

        private readonly IdleDbContext _dbContext;

        public PatronsViewModel(IdleDbContext dbContext) {
            _dbContext = dbContext;
        }

        public string Title { get; } = "Patrons";

        protected override Task<int> CountAllAsync() {
            return _dbContext.Patrons
                .Where(p => p.Organization == CurrentSession!.ActiveAssociation!.Organization)
                .CountAsync();
        }

        protected override async Task<List<PatronRow>> FetchPageAsync(int skip) {
            var dbResults = await _dbContext.Patrons
                .Include(p => p.User)
                .Include(p => p.Organization)
                .Where(p => p.Organization == CurrentSession!.ActiveAssociation!.Organization)
                .OrderBy(p => p.User.Name)
                .Skip(skip)
                .ToListAsync();
            return dbResults
                .Select(p => new PatronRow(p, this))
                .ToList();
        }

        public class PatronRow {
            private readonly PatronsViewModel _viewModel;
            public PatronRow(Patron patron, PatronsViewModel viewModel) {
                Instance = patron;
                _viewModel = viewModel;
            }
            public Patron Instance { get; }
            public string Email => Instance.AllowEmail ? (Instance.User.Email ?? "-") : "-";
            public string Since => TimeHelper.GetRangeStartString(Instance.Since, Instance.Since, _viewModel.TimeZoneId);
        }
    }
}
