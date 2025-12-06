using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Data.Models.Events {
    public enum SeatSelection {
        Deny,
        Allow,
        Require
    }
    [PrimaryKey(nameof(Id))]
    [Index("InstanceId", nameof(Name), IsUnique = true)]
    public class TicketClass {
        public required string Id { get; init; }
        [Column("InstanceId")]
        public required EventInstance Instance { get; init; }
        public required string Name { get; init; }
        public required int OrderSeq { get; set; }
        public int? Limit { get; set; }
        public bool CanRegister { get; set; } = true;
        public bool DiscreteTickets { get; set; } = false;
        public string? InviteCode { get; set; }
        public int Price { get; set; } = 0;
        public SeatSelection SeatSelection { get; set; } = SeatSelection.Deny;
        [InverseProperty(nameof(Ticket.TicketClass))]
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
