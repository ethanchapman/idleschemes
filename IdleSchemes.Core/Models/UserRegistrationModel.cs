using IdleSchemes.Data.Models.Events;

namespace IdleSchemes.Core.Models {
    public class UserRegistrationModel {

        public UserRegistrationModel() {
        }

        public UserRegistrationModel(Registration registration) {
            Id = registration.Id;
            Event = new EventInstanceModel(registration.Instance);
            Tickets = registration.Tickets
                .Select(t => new RegistrationTicket(t))
                .ToList();
        }

        public string Id { get; set; } = "";
        public EventInstanceModel Event { get; set; } = new EventInstanceModel();
        public List<RegistrationTicket> Tickets { get; set; } = new List<RegistrationTicket>();

        public class RegistrationTicket {
            public RegistrationTicket() {
            }

            public RegistrationTicket(Ticket ticket) {
                Id = ticket.Id;
                Class = ticket.TicketClass.Name;
                Seat = ticket.Seat;
                Name = ticket.HolderName ?? ticket.User?.Name;
                Email = ticket.User?.Email;
                Phone = ticket.User?.Phone;
            }

            public string Id { get; set; } = "";
            public string Class { get; set; } = "";
            public string? Seat { get; set; }
            public string? Name { get; set; } = "";
            public string? Email { get; set; }
            public string? Phone { get; set; }

        }
    }
}
