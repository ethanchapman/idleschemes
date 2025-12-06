using System.Collections.Immutable;
using IdleSchemes.Data;
using IdleSchemes.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.WebAdmin.ViewModels.Global {
    public class RegionsViewModel : ListViewModel<Region> {

        private readonly IdleDbContext _dbContext;

        public RegionsViewModel(IdleDbContext dbContext) {
            _dbContext = dbContext;
        }

        public string Title { get; } = "Regions";

        public ImmutableList<string> TimeZones { get; } = TimeZoneInfo.GetSystemTimeZones()
            .Select(s => s.Id)
            .ToImmutableList();

        public async Task ToggleIsPublicAsync(Region region) {
            region.IsPublic = !region.IsPublic;
            await _dbContext.SaveChangesAsync();
        }

        public async Task SetTimeZoneAsync(Region region, string timeZone) {
            region.TimeZone = timeZone;
            await _dbContext.SaveChangesAsync();
        }

        protected override Task<int> CountAllAsync() {
            return _dbContext.Regions.CountAsync();
        }

        protected override Task<List<Region>> FetchPageAsync(int skip) {
            return _dbContext.Regions
                .OrderBy(r => r.Id)
                .Skip(skip)
                .ToListAsync();
        }

    }
}
