using IdleSchemes.Core.Helpers;
using IdleSchemes.Data.Models.Events;

namespace IdleSchemes.Core.Models {
    public class EventInstanceModel {
        public EventInstanceModel() {
        }

        public EventInstanceModel(EventInstance instance) {
            Id = instance.Id;
            Name = instance.Info.Name;
            ShortDescription = instance.Info.ShortDescription;
            Images = SerializationHelper.Deserialize<List<ImageInfoModel>>(instance.Info.Images);
        }

        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string ShortDescription { get; set; } = "";
        public List<ImageInfoModel> Images { get; set; } = new List<ImageInfoModel>();
    }
}
