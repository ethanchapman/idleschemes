using System.Collections.Immutable;
using IdleSchemes.Data.Models.Events;

namespace IdleSchemes.Core.Models {
    public class PublicTicketCollection {
        public PublicTicketCollection(TicketCollection ticketCollection) {
            Classes = ticketCollection.Classes
                .OrderBy(c => c.Class.OrderSeq)
                .Select(c => new PublicTicketClassCollection(c))
                .ToImmutableList();
            Confirmed = Classes.Sum(c => c.Confirmed);
            Pending = Classes.Sum(c => c.Pending);
            Available = Classes
                .SelectMany(c => c.Available)
                .ToImmutableList();
        }

        public ImmutableList<PublicTicketClassCollection> Classes { get; }
        public int Confirmed { get; }
        public int Pending { get; }
        public ImmutableList<PublicAvailableTicket> Available { get; }

    }

    public class PublicTicketClassCollection {

        public PublicTicketClassCollection(TicketClassCollection ticketClassCollection) {
            Class = new PublicTicketClass( ticketClassCollection.Class);
            Available = ticketClassCollection.Available
                .Select(t => new PublicAvailableTicket(t))
                .ToImmutableList();
            ActualAvailableCount = ticketClassCollection.ActualAvailableCount;
            if (ticketClassCollection.Class.DiscreteTickets) {
                All = ticketClassCollection.Class.Limit ?? ticketClassCollection.All.Count;
            } else {
                All = ticketClassCollection.Class.Limit;
            }
            Confirmed = ticketClassCollection.Confirmed.Count;
            Pending = ticketClassCollection.Pending.Count;
        }

        public PublicTicketClass Class { get; }

        public int? All { get; }
        public int Confirmed { get; }
        public int Pending { get; }
        public ImmutableList<PublicAvailableTicket> Available { get; }
        public int? ActualAvailableCount { get; }

    }

    public class PublicTicketClass {

        public PublicTicketClass(TicketClass ticketClass) {
            Id = ticketClass.Id;
            Name = ticketClass.Name;
            Limit = ticketClass.Limit;
            CanRegister = ticketClass.CanRegister;
            DiscreteTickets = ticketClass.DiscreteTickets;
            Price = ticketClass.Price;
            SeatSelection = ticketClass.SeatSelection;
        }

        public string Id { get; }
        public string Name { get; }
        public int? Limit { get; }
        public bool CanRegister { get; } 
        public bool DiscreteTickets { get; } 
        public string? InviteCode { get; }
        public int Price { get; } 
        public SeatSelection SeatSelection { get; }
    }

    public class PublicAvailableTicket {
        public PublicAvailableTicket(Ticket ticket) { 
            Id = ticket.Id;
            Class = ticket.TicketClass.Name;
            Seat = ticket.Seat!;
            Price = ticket.Price;
        }
        public string Id { get; }
        public string Class { get; }
        public string Seat { get; }
        public int Price { get; }
    }
}
