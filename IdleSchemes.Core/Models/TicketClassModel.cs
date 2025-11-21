using IdleSchemes.Data.Models.Events;

namespace IdleSchemes.Core.Models {
    public class TicketClassModel {

        public TicketClassModel() {
        }

        public TicketClassModel(TicketClass ticketClass) {
            Id = ticketClass.Id;
            Name = ticketClass.Name;
            BasePrice = ((decimal)ticketClass.BasePrice) * 0.01m;
            SeatSelection = ticketClass.SeatSelection;
        }

        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public decimal BasePrice { get; set; }
        public TicketClassAvailabilityModel? Availability { get; set; }
        public SeatSelection SeatSelection { get; set; } = SeatSelection.Deny;

    }
}
