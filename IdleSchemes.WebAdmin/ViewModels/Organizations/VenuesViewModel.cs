using IdleSchemes.Data;
using IdleSchemes.Data.Models.Organizations;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.WebAdmin.ViewModels.Organizations {
    public class VenuesViewModel : ListViewModel<Venue> {

        private readonly IdleDbContext _dbContext;

        public VenuesViewModel(IdleDbContext dbContext) {
            _dbContext = dbContext;
        }

        public string Title { get; } = "Venues";

        protected override Task<int> CountAllAsync() {
            return _dbContext.Patrons
                .Where(p => p.Organization == CurrentSession!.ActiveAssociation!.Organization)
                .CountAsync();
        }

        protected override Task<List<Venue>> FetchPageAsync(int skip) {
            return _dbContext.Venues
                .Include(v => v.Organization)
                .Where(v => v.Organization == CurrentSession!.ActiveAssociation!.Organization)
                .OrderBy(v => v.Name)
                .Skip(skip)
                .ToListAsync();
        }

    }
}
