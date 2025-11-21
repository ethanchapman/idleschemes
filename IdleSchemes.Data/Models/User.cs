using System.ComponentModel.DataAnnotations.Schema;
using IdleSchemes.Data.Models.Organizations;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Data.Models {
    [PrimaryKey(nameof(Id))]
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Phone), IsUnique = true)]
    public class User {
        public required string Id { get; init; }
        public required string Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? TimeZone { get; set; }
        public string? Password { get; set; }
        [Column("RegionId")]
        public Region? Region { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsModerator { get; set; }
        public required DateTime Created { get; set; }
        public DateTime? EmailVerified { get; set; }
        public DateTime? PhoneVerified { get; set; }
        public DateTime? Removed { get; set; }

        [InverseProperty(nameof(Associate.User))]
        public List<Associate> Assoctations { get; set; } = new List<Associate>();
    }
}
