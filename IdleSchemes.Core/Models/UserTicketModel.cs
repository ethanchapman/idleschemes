using IdleSchemes.Data.Models.Events;

namespace IdleSchemes.Core.Models {
    public class UserTicketModel {

        public UserTicketModel() {
        }

        public UserTicketModel(Ticket ticket) {
            Id = ticket.Id;
            RegistrationId = ticket.Registration.Id;
            Class = ticket.TicketClass.Name;
            Seat = ticket.Seat;
            Event = new EventInstanceModel(ticket.Registration.Instance);
            CanManage = ticket.User == ticket.Registration.User;
        }

        public string Id { get; set; } = "";
        public string RegistrationId { get; set; } = "";
        public string Class { get; set; } = "";
        public string? Seat { get; set; }
        public bool CanManage { get; set; }
        public EventInstanceModel Event { get; set; } = new EventInstanceModel();
    }
}
