using IdleSchemes.Core.Models;
using IdleSchemes.Data.Models;

namespace IdleSchemes.Core.Models {
    public class RegionModel {
        public RegionModel() {
        }

        public RegionModel(Region region) {
            Id = region.Id;
            Name = region.Name;
        }

        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
    }
}
