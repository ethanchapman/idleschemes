using IdleSchemes.Data.Models.Events;

namespace IdleSchemes.Core.Models {
    public class UserRegistrationModel {

        public UserRegistrationModel() {
        }

        public UserRegistrationModel(Registration registration) {
            Id = registration.Id;
            Event = new EventInstanceModel(registration.Instance);
            Tickets = registration.Claims
                .Select(t => new RegistrationTicket(t))
                .ToList();
        }

        public string Id { get; set; } = "";
        public EventInstanceModel Event { get; set; } = new EventInstanceModel();
        public List<RegistrationTicket> Tickets { get; set; } = new List<RegistrationTicket>();

        public class RegistrationTicket {
            public RegistrationTicket() {
            }

            public RegistrationTicket(TicketClaim ticketClaim) {
                Id = ticketClaim.Ticket.Id;
                Class = ticketClaim.Ticket.TicketClass.Name;
                Seat = ticketClaim.Ticket.Seat;
                Name = ticketClaim.ForName ?? ticketClaim.User?.Name;
                Email = ticketClaim.User?.Email;
                Phone = ticketClaim.User?.Phone;
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
