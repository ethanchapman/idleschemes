namespace IdleSchemes.Core.Models {
    public class TicketClassAvailabilityModel {
        public int? TotalCount { get; set; }
        public int ConfirmedCount { get; set; }
        public int PendingCount { get; set; }
        public int ConfirmedAndPendingCount => ConfirmedCount + PendingCount;
        public int? RemainingCount => TotalCount is null ? null : (TotalCount - ConfirmedAndPendingCount);
        public List<string> RemainingSeats { get; set; } = new List<string>();
        public List<string> PendingSeats { get; set; } = new List<string>();
    }
}
