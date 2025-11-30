using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Data.Models.Organizations {
    [PrimaryKey(nameof(Id))]
    [Index("RegionId", nameof(RegionScore))]
    public class Organization {
        public required string Id { get; init; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public string Images { get; set; } = "[]";
        public required string TimeZone { get; set; }
        public required DateTime Created { get; init; }

        [Column("RegionId")]
        public Region? Region { get; set; }
        public bool RegionApproved { get; set; }
        public int? RegionScore { get; set; }
        public decimal RegistrationFee { get; set; } = 0.1m;
        public DateTime LastPayout { get; set; } = DateTime.MinValue;
    }
}
