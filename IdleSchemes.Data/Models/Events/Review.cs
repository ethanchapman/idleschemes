using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Data.Models.Events {
    [PrimaryKey("TicketId")]
    public class Review {
        [Column("TicketId")]
        public required Ticket Ticket { get; init; }
        public DateTime Submitted { get; init; }
        public decimal Rating { get; set; } = 5;
        public string Comment { get; set; } = "";
    }
}
