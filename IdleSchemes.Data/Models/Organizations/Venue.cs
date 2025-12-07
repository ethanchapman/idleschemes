using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Data.Models.Organizations {
    [PrimaryKey(nameof(Id))]
    public class Venue {
        public required string Id { get; init; }
        [Column("OrganizationId")]
        public required Organization Organization { get; init; }
        [Column("ParentId")]
        public Venue? Parent { get; set; }
        public string Name { get; set; } = "";
        public string PublicLocation { get; set; } = "";
        public string PrivateLocation { get; set; } = "";
        public string? TicketSchemes { get; set; }
    }
}
