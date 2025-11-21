using IdleSchemes.Data;
using IdleSchemes.Data.Models.Organizations;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.WebAdmin.ViewModels {
    public class OrganizationsViewModel : ListViewModel<Organization> {

        private readonly IdleDbContext _dbContext;

        public OrganizationsViewModel(IdleDbContext dbContext) {
            _dbContext = dbContext;
        }

        public string Title { get; } = "Organizations";

        protected override Task<int> CountAllAsync() {
            return _dbContext.Organizations.CountAsync();
        }

        protected override Task<List<Organization>> FetchPageAsync(int skip) {
            return _dbContext.Organizations
                .Include(o => o.Region)
                .OrderBy(r => r.Id)
                .Skip(skip)
                .ToListAsync();
        }
    }
}
