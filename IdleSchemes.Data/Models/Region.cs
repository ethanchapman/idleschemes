using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Data.Models {
    [PrimaryKey(nameof(Id))]
    public class Region {
        public required string Id { get; init; }
        public required string Name { get; set; }
        public required string TimeZone { get; set; }
        public bool IsPublic { get; set; }
    }
}
