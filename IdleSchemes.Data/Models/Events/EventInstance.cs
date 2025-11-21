using System.ComponentModel.DataAnnotations.Schema;
using IdleSchemes.Data.Models.Organizations;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Data.Models.Events {
    [PrimaryKey(nameof(Id))]
    [Index(nameof(FirstSessionStarts))]
    [Index(nameof(LastSessionEnds))]
    public class EventInstance {
        public required string Id { get; init; }

        [Column("TemplateId")]
        public required EventTemplate Template { get; init; }
        [InverseProperty(nameof(Host.Instance))]
        public List<Host> Hosts { get; set; } = new List<Host>();
        public required EventInfo Info { get; set; } = new EventInfo();
        public bool ListInRegion { get; set; } = false;

        public required DateTime Created { get; set; }
        public DateTime? Published { get; set; }
        public DateTime? RegistrationOpen { get; set; }
        public DateTime? RegistrationClose { get; set; }
        public DateTime? CancellationDeadline { get; set; }
        public DateTime? Filled { get; set; }
        public DateTime? Cancelled { get; set; }

        [InverseProperty(nameof(EventSession.Instance))]
        public List<EventSession> Sessions { get; set; } = new List<EventSession>();
        public DateTime? FirstSessionStarts { get; set; }
        public DateTime? LastSessionEnds { get; set; }

        [InverseProperty(nameof(TicketClass.Instance))]
        public List<TicketClass> TicketClasses { get; set; } = new List<TicketClass>();
    }
}
