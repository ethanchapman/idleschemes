using System.ComponentModel.DataAnnotations.Schema;
using IdleSchemes.Data.Models.Organizations;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Data.Models.Events {
    [PrimaryKey(nameof(Id))]
    public class EventTemplate {
        public required string Id { get; init; }
        [Column("OrganizationId")]
        public required Organization Organization { get; init; }
        public EventInfo Info { get; init; } = new EventInfo();
        public bool IsUnique { get; set; } = true;
    }
}
