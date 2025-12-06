using IdleSchemes.Data;
using IdleSchemes.Data.Models.Organizations;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.WebAdmin.ViewModels.Organizations {
    public class AssociatesViewModel : ListViewModel<Associate> {

        private readonly IdleDbContext _dbContext;

        public AssociatesViewModel(IdleDbContext dbContext) {
            _dbContext = dbContext;
        }

        public string Title { get; } = "Associates";

        protected override Task<int> CountAllAsync() {
            return _dbContext.Associates
                .Where(a => a.Organization == CurrentSession!.ActiveAssociation!.Organization)
                .CountAsync();
        }

        protected override Task<List<Associate>> FetchPageAsync(int skip) {
            return _dbContext.Associates
                .Include(a => a.User)
                .Include(a => a.PermissionSet)
                .Where(a => a.Organization == CurrentSession!.ActiveAssociation!.Organization)
                .OrderBy(a => a.PublicName)
                .Skip(skip)
                .ToListAsync();
        }
    }
}
