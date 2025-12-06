namespace IdleSchemes.Core.Models.Input {
    public class EventCreationOptionsBase {
        public string OrganizationId { get; set; } = "";
        public string Name { get; set; } = "";
        public string ShortDescription { get; set; } = "";
        public string LongDescription { get; set; } = "";
        public List<ImageInfoModel> Images { get; set; } = new List<ImageInfoModel>();
        public int? IndividualTicketLimit { get; set; } = null;
    }
}
