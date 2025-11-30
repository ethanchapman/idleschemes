using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Data.Models.Events {
    [PrimaryKey(nameof(Id))]
    public class Ticket {
        public required string Id { get; init; }

        [Column("RegistrationId")]
        public required Registration Registration { get; set; }
        [Column("TicketClassId")]
        public required TicketClass TicketClass { get; set; }
        public string? Seat { get; set; }

        [Column("UserId")]
        public User? User { get; set; }
        public string? HolderName { get; set; }

    }
}
