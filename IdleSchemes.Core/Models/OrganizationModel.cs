using IdleSchemes.Data.Models.Organizations;

namespace IdleSchemes.Core.Models {
    public class OrganizationModel {

        public OrganizationModel() {
        }

        public OrganizationModel(Organization organization) {
            Id = organization.Id;
            Name = organization.Name;
        }

        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
    }
}
