using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Data.Models.Events {
    [PrimaryKey(nameof(Id))]
    public class Registration {
        public required string Id { get; init; }
        public required string Secret { get; init; }
        [Column("InstanceId")]
        public required EventInstance Instance { get; init; }
        [Column("UserId")]
        public required User User { get; init; }

        public int Cost { get; set; }

        public required DateTime Started { get; init; }
        public DateTime? TicketsClaimed { get; set; }
        public DateTime? Confirmed { get; set; }
        public DateTime? Cancelled { get; set; }

        [InverseProperty(nameof(Ticket.Registration))]
        public List<TicketClaim> Claims { get; set; } = new List<TicketClaim>();
    }
}
