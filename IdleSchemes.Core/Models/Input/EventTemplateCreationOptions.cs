namespace IdleSchemes.Core.Models.Input {
    public class EventTemplateCreationOptions : EventCreationOptionsBase {
        public List<TicketScheme> TicketSchemes { get; set; } = new List<TicketScheme>();
    }
}
