using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Data.Models.Organizations {
    [PrimaryKey(nameof(Id), "UserId", "OrganizationId")]
    [Index("UserId", "OrganizationId", IsUnique = true)]
    public class Associate {
        public required string Id { get; init; }
        [Column("UserId")]
        public required User User { get; init; }
        [Column("OrganizationId")]
        public required Organization Organization { get; init; }
        [Column("PermissionSet")]
        public required PermissionSet PermissionSet { get; init; }
        public bool IsPublic { get; set; } = false;
        public string PublicName { get; set; } = "";
        public string PublicBio { get; set; } = "";
        public required DateTime Since { get; init; }
    }
}
