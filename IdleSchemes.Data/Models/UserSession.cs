using System.ComponentModel.DataAnnotations.Schema;
using IdleSchemes.Data.Models.Organizations;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Data.Models {
    [PrimaryKey(nameof(Id))]
    public class UserSession {
        public required string Id { get; init; }
        [Column("UserId")]
        public User? User { get; set; }
        public required DateTime Created { get; set; }
        public DateTime? Voided { get; set; }
        public Associate? ActiveAssociation { get; set; }
    }
}
