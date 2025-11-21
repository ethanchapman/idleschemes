namespace IdleSchemes.Core.Models.Input {
    public class TicketClassCreationOptions {
        public string Name { get; set; } = "";
        public int? Available { get; set; } = null;
        public int BasePrice { get; set; } = 0;
        public bool CanRegister { get; set; } = true;
    }
}
