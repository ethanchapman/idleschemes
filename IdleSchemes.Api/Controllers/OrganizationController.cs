using IdleSchemes.Api.Models;
using IdleSchemes.Core.Models;
using IdleSchemes.Data;
using IdleSchemes.Data.Models.Organizations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Api.Controllers {
    [ApiController]
    [Route("organizations")]
    public class OrganizationController : ControllerBase {

        private const int LIST_LIMIT = 40;

        private readonly ILogger<OrganizationController> _logger;
        private readonly IdleDbContext _dbContext;

        public OrganizationController(ILogger<OrganizationController> logger, IdleDbContext dbContext) {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet("all/{organizationId}", Name = "GetOranization")]
        public async Task<ActionResult<OrganizationModel>> GetOranization(string organizationId) {
            var organization = await QueryOrganizations()
                .Where(o => o.Id == organizationId)
                .SingleOrDefaultAsync();
            if (organization is null) {
                return NotFound();
            }
            return Ok(new OrganizationModel(organization));
        }

        [HttpGet("all/{organizationId}/associates", Name = "GetAssociatesInOrganization")]
        public async Task<ActionResult<IEnumerable<AssociateModel>>> GetAssociatesInOrganization(string organizationId) {
            var organization = await QueryOrganizations()
                .Where(o => o.Id == organizationId)
                .SingleOrDefaultAsync();
            if (organization is null) {
                return NotFound();
            }
            var associates = await _dbContext.Associates
                .Where(a => a.Organization == organization && a.IsPublic == true)
                .ToListAsync();
            return Ok(associates.Select(a => new AssociateModel(a)));
        }

        [HttpGet("byregion/{regionId}", Name = "GetOranizationsInRegion")]
        public async Task<IEnumerable<OrganizationModel>> GetOranizationsInRegion(string regionId, [FromQuery] int start = 0, [FromQuery] int limit = 20) {
            var organizations = await QueryOrganizations()
                .Where(o => o.Region!.Id == regionId && o.RegionApproved == true)
                .OrderByDescending(o => o.RegionScore)
                .Skip(start)
                .Take(Math.Min(limit, LIST_LIMIT))
                .ToListAsync();
            return organizations
                .Select(o => new OrganizationModel(o));
        }

        private IQueryable<Organization> QueryOrganizations() {
            return _dbContext.Organizations
                .Include(o => o.Region);
        }

    }
}
