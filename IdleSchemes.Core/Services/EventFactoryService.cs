using IdleSchemes.Core.Helpers;
using IdleSchemes.Core.Models.Input;
using IdleSchemes.Data;
using IdleSchemes.Data.Models.Events;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Core.Services {
    public class EventFactoryService {

        private readonly IdleDbContext _dbContext;
        private readonly IdService _idService;
        private readonly TimeService _timeService;

        public EventFactoryService(IdleDbContext dbContext, IdService idService, TimeService timeService) {
            _dbContext = dbContext;
            _idService = idService;
            _timeService = timeService;
        }

        public async Task<EventTemplate> CreateEventTemplateAsync(EventTemplateCreationOptions options, bool save = false) {
            var template = await CreateTemplateAsync(options, false);
            if(save) {
                await _dbContext.SaveChangesAsync();
            }
            return template;
        }

        public async Task<EventInstance> CreateEventAsync(EventCreationOptions options, bool save = false) {
            EventTemplate? template;
            if (options.TemplateId is not null) {
                template = await _dbContext.EventTemplates
                    .SingleOrDefaultAsync(t => t.Id == options.TemplateId && t.Organization.Id == options.OrganizationId);
                if (template is null) {
                    throw new KeyNotFoundException($"Template {options.TemplateId} not found");
                }
            } else {
                template = await CreateTemplateAsync(options, true);
            }
            var instance = _dbContext.EventInstances.Add(new EventInstance {
                Id = _idService.GenerateId(),
                Template = template,
                Created = _timeService.GetNow(),
                RegistrationOpen = options.RegistrationOpen,
                RegistrationClose = options.RegistrationClose,
                CancellationDeadline = options.CancellationDeadline,
            }).Entity;
            FillInfo(instance.Info, options);
            instance.Sessions = options.Sessions
                .Select(s => new EventSession { 
                    Instance = instance,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                }).ToList();
            instance.FirstSessionStarts = instance.Sessions
                .OrderBy(s => s.StartTime)
                .First()
                .StartTime;
            instance.LastSessionEnds = instance.Sessions
                .OrderBy(s => s.EndTime)
                .Last()
                .EndTime;
            var hosts = await _dbContext.Associates
                .Where(a => options.HostAssociateIds.Contains(a.Id))
                .ToListAsync();
            instance.Hosts = hosts
                .Select(a => new Host { Associate =  a, Instance = instance })
                .ToList();
            int i = 0;
            foreach(var tc in options.TicketScheme.Classes) {
                var ticketClass = _dbContext.TicketClasses.Add(new TicketClass { 
                    Id = _idService.GenerateId(),
                    Instance = instance,
                    Name = tc.Name,
                    OrderSeq = i,
                    Limit = tc.Limit,
                    CanRegister = tc.CanRegister,
                    DiscreteTickets = tc.Tickets.Any() || (tc.Limit is not null && !tc.Tickets.Any()),
                    InviteCode = tc.RequiresCode ? _idService.GenerateSecret(8) : null
                }).Entity;
                if (tc.Tickets.Any()) {
                    ticketClass.DiscreteTickets = true;
                    _dbContext.Tickets.AddRange(tc.Tickets.Select(t => new Ticket {
                        Id = _idService.GenerateId(),
                        TicketClass = ticketClass,
                        Seat = t.Seat,
                        Price = ticketClass.Price + t.Price
                    }));
                } else if(tc.Limit is not null && !tc.Tickets.Any()) {
                    ticketClass.DiscreteTickets = true;
                    _dbContext.Tickets.AddRange(Enumerable.Range(0, tc.Limit.Value).Select(t => new Ticket {
                        Id = _idService.GenerateId(),
                        TicketClass = ticketClass,
                        Price = ticketClass.Price
                    }));
                }
                i++;
            }
            if (save) {
                await _dbContext.SaveChangesAsync();
            }
            return instance;
        }

        private async Task<EventTemplate> CreateTemplateAsync(EventCreationOptionsBase options, bool isUnique) {
            var orgainization = await _dbContext.Organizations
                .SingleOrDefaultAsync(o => o.Id == options.OrganizationId);
            if (orgainization is null) {
                throw new KeyNotFoundException($"Organization {options.OrganizationId} not found");
            }
            var template = _dbContext.EventTemplates.Add(new EventTemplate {
                Id = _idService.GenerateId(),
                IsUnique = isUnique,
                Organization = orgainization
            }).Entity;
            FillInfo(template.Info, options);
            return template;
        }

        private void FillInfo(EventInfo info, EventCreationOptionsBase options) {
            info.Name = options.Name;
            info.ShortDescription = options.ShortDescription;
            info.LongDescription = options.LongDescription;
            info.Images = SerializationHelper.Serialize(options.Images);
            info.IndividualTicketLimit = options.IndividualTicketLimit;
        }

    }
}
