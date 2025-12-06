using IdleSchemes.Data.Models.Events;

namespace IdleSchemes.Core.Models.Input {
    public class TicketScheme {
        public string Name { get; set; } = "";
        public List<TicketSchemeClass> Classes { get; set; } = new List<TicketSchemeClass>();
    }

    public class TicketSchemeClass {
        public string Name { get; set; } = "";
        public int Price { get; set; } = 0;
        public int? Limit { get; set; } = 0;
        public bool CanRegister { get; set; } = true;
        public bool RequiresCode { get; set; } = false;
        public SeatSelection SeatSelection { get; set; } = SeatSelection.Deny;
        public List<TicketSchemeTicket> Tickets { get; set; } = new List<TicketSchemeTicket>();
    }

    public class TicketSchemeTicket {
        public string Seat { get; set; } = "";
        public int Count { get; set; } = 1;
        public int Price { get; set; } = 0;
    }
}
