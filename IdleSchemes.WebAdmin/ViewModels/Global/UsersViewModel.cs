using IdleSchemes.Data;
using IdleSchemes.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.WebAdmin.ViewModels.Global {
    public class UsersViewModel : ListViewModel<User> {

        private readonly IdleDbContext _dbContext;

        public UsersViewModel(IdleDbContext dbContext) {
            _dbContext = dbContext;
        }

        public string Title { get; } = "Users";

        protected override Task<int> CountAllAsync() {
            return _dbContext.Users.CountAsync();
        }

        protected override Task<List<User>> FetchPageAsync(int skip) {
            return _dbContext.Users
                .Include(o => o.Region)
                .OrderBy(r => r.Id)
                .Skip(skip)
                .ToListAsync();
        }
    }
}
