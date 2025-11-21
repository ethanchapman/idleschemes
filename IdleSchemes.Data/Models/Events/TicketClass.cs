using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Data.Models.Events {
    public enum SeatSelection {
        Deny,
        Allow,
        Require
    }
    [PrimaryKey(nameof(Id))]
    [Index("InstanceId", nameof(Name), IsUnique = true)]
    public class TicketClass {
        public required string Id { get; init; }
        [Column("InstanceId")]
        public required EventInstance Instance { get; init; }
        public required string Name { get; init; }
        public required int OrderSeq { get; set; }
        public bool CanRegister { get; set; } = true;
        public string? InviteCode { get; set; }
        public int BasePrice { get; set; } = 0;

        public int? TotalCount { get; set; }
        public string? SeatOptions { get; set; }
        public SeatSelection SeatSelection { get; set; } = SeatSelection.Deny;
    }
}
