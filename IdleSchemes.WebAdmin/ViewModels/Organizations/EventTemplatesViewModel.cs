using IdleSchemes.Data;
using IdleSchemes.Data.Models.Events;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.WebAdmin.ViewModels.Organizations {
    public class EventTemplatesViewModel : ListViewModel<EventTemplate> {

        private readonly IdleDbContext _dbContext;

        public EventTemplatesViewModel(IdleDbContext dbContext) {
            _dbContext = dbContext;
        }

        public string Title { get; } = "Event Templates";

        protected override Task<int> CountAllAsync() {
            return _dbContext.EventTemplates
                .Where(t => t.Organization == CurrentSession!.ActiveAssociation!.Organization && t.IsUnique == false)
                .CountAsync();
        }

        protected override Task<List<EventTemplate>> FetchPageAsync(int skip) {
            return _dbContext.EventTemplates
                .Where(t => t.Organization == CurrentSession!.ActiveAssociation!.Organization && t.IsUnique == false)
                .OrderBy(t => t.Info.Name)
                .Skip(skip)
                .ToListAsync();
        }
    }
}
