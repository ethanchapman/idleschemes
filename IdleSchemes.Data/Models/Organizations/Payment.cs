using System.ComponentModel.DataAnnotations.Schema;
using IdleSchemes.Data.Models.Events;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Data.Models.Organizations {
    [PrimaryKey(nameof(Id))]
    public class Payment {
        public required string Id { get; init; }
        [Column("OrganizationId")]
        public required Organization Organization { get; init; }
        [Column("AssociateId")]
        public required Associate? Associate { get; init; }
        [Column("RegistrationId")]
        public Registration? Registration { get; init; }
        public required string Memo { get; init; }
        public required int Amount { get; init; }
        public required DateTime Time { get; init; }
        public required DateTime Confirmed { get; set; }
        public DateTime? Paid { get; set; }
        public DateTime? Cancelled { get; set; }
    }
}
