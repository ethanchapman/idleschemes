using IdleSchemes.Data.Models.Organizations;

namespace IdleSchemes.Core.Models {
    public class AssociateModel {

        public AssociateModel() {
        }

        public AssociateModel(Associate associate) {
            Id = associate.Id;
            Name = associate.PublicName;
            PublicBio = associate.PublicBio;
        }

        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string PublicBio { get; set; } = "";

    }
}
