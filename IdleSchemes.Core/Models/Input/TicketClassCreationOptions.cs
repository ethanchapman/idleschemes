namespace IdleSchemes.Core.Models.Input {
    public class TicketClassCreationOptions {
        public string Name { get; set; } = "";
        public int? Count { get; set; } = null;
        public List<string>? Seats { get; set; }
        public int BasePrice { get; set; } = 0;
        public bool CanRegister { get; set; } = true;
    }
}
