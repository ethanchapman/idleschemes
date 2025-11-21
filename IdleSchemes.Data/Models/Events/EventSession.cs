using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Data.Models.Events {
    [PrimaryKey("InstanceId", nameof(StartTime))]
    public class EventSession {
        [Column("InstanceId")]
        public required EventInstance Instance { get; init; }
        public required DateTime StartTime { get; init; }
        public required DateTime EndTime { get; init; }
    }
}
