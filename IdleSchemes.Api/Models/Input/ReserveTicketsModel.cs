using Microsoft.AspNetCore.Mvc.Localization;

namespace IdleSchemes.Api.Models.Input {
    public class ReserveTicketsModel {
        public string RegistrationId { get; set; } = "";
        public string RegistrationSecret { get; set; } = "";
        public List<TicketReservation> TicketReservations { get; set; } = new List<TicketReservation>();

        public class TicketReservation {
            public string Class { get; set; } = "";
            public string? Code { get; set; }
            public int? Count { get; set; } = 1;
            public List<SeatAssignment>? Seats { get; set; }
            public string? HolderName { get; set; }
        }

        public class SeatAssignment {
            public string? Seat { get; set; }
            public string? ForName { get; set; }
        }
    }
}
