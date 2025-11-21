using IdleSchemes.Api.Controllers;
using IdleSchemes.Core.Models;
using Microsoft.AspNetCore.Components.Forms;
using Shouldly;

namespace IdleSchemes.Tests.Api {
    public class OrganizationTests : TestBase {

        [Test]
        public async Task ListRegions() {
            var result = await ExecuteAsyncAction<RegionController, IEnumerable<RegionModel>>(c => c.GetRegions());
            result.Count().ShouldBe(2);
        }

        [Test]
        public async Task ListOrganizations() {
            var result = await ExecuteAsyncAction<OrganizationController, IEnumerable<OrganizationModel>>(c => c.GetOranizationsInRegion("den"));
            result.Count().ShouldBe(1);
            result.First().Name.ShouldBe("Brush and Needle");

            result = await ExecuteAsyncAction<OrganizationController, IEnumerable<OrganizationModel>>(c => c.GetOranizationsInRegion("atx"));
            result.Count().ShouldBe(1);
            result.First().Name.ShouldBe("Austin Creative Reuse");

            await UpdateDatabaseAsync(db => {
                var craft = db.Organizations.First(o => o.Name == "Craft");
                craft.RegionApproved = true;
            });

            result = await ExecuteAsyncAction<OrganizationController, IEnumerable<OrganizationModel>>(c => c.GetOranizationsInRegion("atx"));
            result.Count().ShouldBe(2, "Craft should appear in the ATX region after being approved");
        }

        [Test]
        public async Task GetSpecificOrganization() {
            var listResults = await ExecuteAsyncAction<OrganizationController, IEnumerable<OrganizationModel>>(c => c.GetOranizationsInRegion("den"));
            var id = listResults.First(o => o.Name == "Brush and Needle").Id;

            var result = await ExecuteAsyncAction<OrganizationController, OrganizationModel>(c => c.GetOranization(id));
            result.Id.ShouldBe(id);
            result.Name.ShouldBe("Brush and Needle");
        }

        [Test]
        public async Task GetOrganizationAssociates() {
            var listResults = await ExecuteAsyncAction<OrganizationController, IEnumerable<OrganizationModel>>(c => c.GetOranizationsInRegion("den"));
            var id = listResults.First(o => o.Name == "Brush and Needle").Id;

            var results = await ExecuteAsyncAction<OrganizationController, IEnumerable<AssociateModel>>(c => c.GetAssociatesInOrganization(id));
            results.Count().ShouldBe(2);
        }

    }
}
