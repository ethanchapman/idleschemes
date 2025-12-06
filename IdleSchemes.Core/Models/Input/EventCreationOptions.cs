namespace IdleSchemes.Core.Models.Input {
    public class EventCreationOptions : EventCreationOptionsBase {
        public string? TemplateId { get; set; }
        public List<string> HostAssociateIds { get; set; } = new List<string>();
        public List<Session> Sessions { get; set; } = new List<Session>();
        public DateTime? RegistrationOpen { get; set; }
        public DateTime? RegistrationClose { get; set; }
        public DateTime? CancellationDeadline { get; set; }

        public class Session {
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }

    }
}
