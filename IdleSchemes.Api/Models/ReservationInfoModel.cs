namespace IdleSchemes.Api.Models {
    public class ReservationInfoModel {

        public ReservationInfoModel() {
        }

        public string Id { get; set; } = "";
        public string Secret { get; set; } = "";
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();


        public class Ticket {
            public string Id { get; set; } = "";
            public string Class { get; set; } = "";
            public string? Seat { get; set; }
        }
    }
}
