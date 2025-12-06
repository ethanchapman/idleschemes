using IdleSchemes.Data.Models.Events;

namespace IdleSchemes.Core.Models {
    public class UserTicketModel {

        public UserTicketModel() {
        }

        public UserTicketModel(TicketClaim ticketClaim) {
            Id = ticketClaim.Ticket.Id;
            RegistrationId = ticketClaim.Registration.Id;
            Class = ticketClaim.Ticket.TicketClass.Name;
            Seat = ticketClaim.Ticket.Seat;
            Event = new EventInstanceModel(ticketClaim.Registration.Instance);
            CanManage = ticketClaim.User == ticketClaim.Registration.User;
        }

        public string Id { get; set; } = "";
        public string RegistrationId { get; set; } = "";
        public string Class { get; set; } = "";
        public string? Seat { get; set; }
        public bool CanManage { get; set; }
        public EventInstanceModel Event { get; set; } = new EventInstanceModel();
    }
}
