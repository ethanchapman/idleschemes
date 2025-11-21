using System.ComponentModel.DataAnnotations.Schema;
using IdleSchemes.Data.Models.Organizations;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Data.Models.Events {
    [PrimaryKey("InstanceId", "AssociateId")]
    public class Host {
        [Column("InstanceId")]
        public required EventInstance Instance { get; init; }
        [Column("AssociateId")]
        public required Associate Associate { get; init; }
    }
}
