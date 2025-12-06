using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Data.Models.Events {
    [PrimaryKey(nameof(Id))]
    public class Ticket {
        public required string Id { get; init; }
        [Column("TicketClassId")]
        public required TicketClass TicketClass { get; set; }
        public string? Seat { get; set; }
        public int Price { get; set; }

        [Column("RegistrationId")]
        public Registration? Registration { get; set; }
    }
}
