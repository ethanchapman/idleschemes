using System.Collections.Immutable;
using IdleSchemes.Data.Models.Events;

namespace IdleSchemes.Core.Models {
    public class TicketCollection {
        public TicketCollection(IEnumerable<TicketClass> ticketClasses, DateTime timeoutLimit) {
            Classes = ticketClasses
                .Select(tc => new TicketClassCollection(tc, timeoutLimit))
                .ToImmutableList();
            All = Classes.SelectMany(c => c.All).ToImmutableList();
            Confirmed = Classes.SelectMany(c => c.Confirmed).ToImmutableList();
            Pending = Classes.SelectMany(c => c.Pending).ToImmutableList();
            Available = Classes
                .SelectMany(c => c.Available)
                .ToImmutableList();
        }

        public ImmutableList<TicketClassCollection> Classes { get; }
        public ImmutableList<Ticket> All { get; }
        public ImmutableList<Ticket> Confirmed { get; }
        public ImmutableList<Ticket> Pending { get; }
        public ImmutableList<Ticket> Available { get; }

    }

    public class TicketClassCollection {

        private readonly DateTime _timeoutLimit;

        public TicketClassCollection(TicketClass ticketClass, DateTime timeoutLimit) {
            Class = ticketClass;
            _timeoutLimit = timeoutLimit;

            All = ticketClass.Tickets
                .ToImmutableList();
            Confirmed = ticketClass.Tickets
                .Where(t => t.Registration?.Confirmed is not null
                    && t.Registration?.Cancelled is null)
                .ToImmutableList();
            Pending = ticketClass.Tickets
                .Where(t => t.Registration?.Confirmed is null
                    && t.Registration?.TicketsClaimed is not null 
                    && t.Registration.TicketsClaimed > _timeoutLimit
                    && t.Registration?.Cancelled is null)
                .ToImmutableList();
            Cancelled = ticketClass.Tickets
                .Where(t => t.Registration?.Cancelled is not null)
                .ToImmutableList();
            Available = ticketClass.Tickets
                .Where(t => !Confirmed.Contains(t) && !Pending.Contains(t))
                .ToImmutableList();

        }

        public TicketClass Class { get; }

        public int? ActualAvailableCount {
            get {
                if (Class.DiscreteTickets) {
                    return Math.Min(Available.Count, Class.Limit ?? int.MaxValue);
                }
                return Class.Limit;
            }
        }

        public ImmutableList<Ticket> All { get; } = ImmutableList<Ticket>.Empty;
        public ImmutableList<Ticket> Confirmed { get; } = ImmutableList<Ticket>.Empty;
        public ImmutableList<Ticket> Pending { get; } = ImmutableList<Ticket>.Empty;
        public ImmutableList<Ticket> Cancelled { get; } = ImmutableList<Ticket>.Empty;
        public ImmutableList<Ticket> Available { get; } = ImmutableList<Ticket>.Empty;

    }
}
