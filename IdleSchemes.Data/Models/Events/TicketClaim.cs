using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Data.Models.Events {
    [PrimaryKey("RegistrationId", "TicketId")]
    public class TicketClaim {
        [Column("RegistrationId")]
        public required Registration Registration { get; set; }
        [Column("TicketId")]
        public required Ticket Ticket { get; set; }
        [Column("UserId")]
        public User? User { get; set; }
        public string? ForName { get; set; }
    }
}
