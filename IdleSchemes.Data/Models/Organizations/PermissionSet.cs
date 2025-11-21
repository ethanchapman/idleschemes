using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Data.Models.Organizations {
    public enum OwnAny {
        None,
        Own,
        Any,
    }
    [PrimaryKey(nameof(Id))]
    public class PermissionSet {
        public required string Id { get; init; }
        [Column("OrganizationId")]
        public required Organization? Organization { get; init; }
        public string Name { get; set; } = "";

        public OwnAny CanScheduleEvents { get; set; }
        public string? CanScheduleEventTemplates { get; set; }
        public OwnAny CanViewEvents { get; set; }
        public OwnAny CanPublishEvents { get; set; }
        public OwnAny CanCancelEvents { get; set; }
        public OwnAny CanViewPatrons { get; set; }
        public bool CanManagePatrons { get; set; }
        public bool CanManageAssociates { get; set; }
        public OwnAny CanViewFinances { get; set; }
        public bool CanManageOrganizationDetails { get; set; }
    }
}
