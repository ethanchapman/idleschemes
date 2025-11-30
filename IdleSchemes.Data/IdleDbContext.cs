using System.Data;
using IdleSchemes.Data.Models;
using IdleSchemes.Data.Models.Events;
using IdleSchemes.Data.Models.Organizations;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Data {
    public class IdleDbContext : DbContext {

        public IdleDbContext(DbContextOptions<IdleDbContext> options)
            : base(options) { 
        }

        public DbSet<Region> Regions { get; init; }
        public DbSet<User> Users { get; init; }
        public DbSet<UserSession> UserSessions { get; init; }
        public DbSet<Organization> Organizations { get; init; }
        public DbSet<Associate> Associates { get; init; }
        public DbSet<Patron> Patrons { get; init; }
        public DbSet<PermissionSet> PermissionSets { get; init; }

        public DbSet<EventTemplate> EventTemplates { get; init; }
        public DbSet<Venue> Venues { get; init; }
        public DbSet<EventInstance> EventInstances { get; init; }
        public DbSet<TicketClass> TicketClasses { get; init; }
        public DbSet<Ticket> Tickets { get; init; }
        public DbSet<Host> Hosts { get; init; }
        public DbSet<Registration> Registrations { get; init; }
        public DbSet<Review> Reviews { get; init; }

    }
}
