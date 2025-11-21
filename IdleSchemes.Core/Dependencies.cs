using IdleSchemes.Core.Services;
using IdleSchemes.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdleSchemes.Core {
    public static class Dependencies {

        public static void Add(IServiceCollection serviceCollection) {
            serviceCollection.AddSingleton<IdService>();
            serviceCollection.AddSingleton<TimeService>();

            serviceCollection.AddDbContext<IdleDbContext>((sp, o) => o.UseNpgsql(sp.GetRequiredService<IConfiguration>().GetConnectionString("Database")));

            serviceCollection.AddScoped<EventService>();
            serviceCollection.AddScoped<SetupService>();
            serviceCollection.AddScoped<UserService>();
            serviceCollection.AddScoped<UserSessionService>();
        }

    }
}
