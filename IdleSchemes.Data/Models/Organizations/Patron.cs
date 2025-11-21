using System.ComponentModel.DataAnnotations.Schema;
using IdleSchemes.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Data.Models.Organizations {
    [PrimaryKey("UserId", "OrganizationId")]
    public class Patron {
        [Column("UserId")]
        public required User User { get; init; }
        [Column("OrganizationId")]
        public required Organization Organization { get; init; }
        public bool AllowMarketing { get; set; }
        public required DateTime Since { get; init; }
    }
}
