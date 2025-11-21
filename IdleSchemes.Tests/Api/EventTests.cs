using IdleSchemes.Api.Controllers;
using IdleSchemes.Core.Models;
using Shouldly;

namespace IdleSchemes.Tests.Api {
    public class EventTests : TestBase {

        [Test]
        public async Task ListRegionEvents() {
            var result = await ExecuteAsyncAction<EventController, IEnumerable<EventInstanceModel>>(c => c.GetEventsInRegion("den"));
            result.Count().ShouldBe(2);
        }

        [Test]
        public async Task ListOrganizationEvents() {
            var listResults = await ExecuteAsyncAction<OrganizationController, IEnumerable<OrganizationModel>>(c => c.GetOranizationsInRegion("den"));
            var id = listResults.First().Id;

            var result = await ExecuteAsyncAction<EventController, IEnumerable<EventInstanceModel>>(c => c.GetEventsByOrg(id));
            result.Count().ShouldBe(3);
        }

        [Test]
        public async Task ListHostEvents() {
            var listResults = await ExecuteAsyncAction<OrganizationController, IEnumerable<OrganizationModel>>(c => c.GetOranizationsInRegion("den"));
            var id = listResults.First(o => o.Name == "Brush and Needle").Id;

            var assocaites = await ExecuteAsyncAction<OrganizationController, IEnumerable<AssociateModel>>(c => c.GetAssociatesInOrganization(id));

            var associate = assocaites.First(a => a.Name == "Ethan (B&N)");
            var results = await ExecuteAsyncAction<EventController, IEnumerable<EventInstanceModel>>(c => c.GetEventsByAssociate(associate.Id));
            results.Count().ShouldBe(2);

            associate = assocaites.First(a => a.Name == "Madeline (B&N)");
            results = await ExecuteAsyncAction<EventController, IEnumerable<EventInstanceModel>>(c => c.GetEventsByAssociate(associate.Id));
            results.Count().ShouldBe(1);
        }

    }
}
