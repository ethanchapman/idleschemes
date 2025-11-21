using IdleSchemes.Api.Models;
using IdleSchemes.Core.Models;
using IdleSchemes.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Api.Controllers {
    [ApiController]
    [Route("regions")]
    public class RegionController : ControllerBase {

        private readonly ILogger<RegionController> _logger;
        private readonly IdleDbContext _dbContext;

        public RegionController(ILogger<RegionController> logger, IdleDbContext dbContext) {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet(Name = "GetRegions")]
        public async Task<IEnumerable<RegionModel>> GetRegions() {
            var regions = await _dbContext.Regions
                .Where(r => r.IsPublic == true)
                .ToListAsync();
            return regions
                .Select(r => new RegionModel(r));
        }

    }
}
