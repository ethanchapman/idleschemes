using System.ComponentModel.DataAnnotations.Schema;

namespace IdleSchemes.Data.Models.Events {
    [ComplexType]
    public class EventInfo {
        public string Name { get; set; } = "";
        public string ShortDescription { get; set; } = "";
        public string LongDescription { get; set; } = "";
        public string Images { get; set; } = "[]";
        public int? IndividualTicketLimit { get; set; } = null;
    }
}
