using System.Reflection;
using IdleSchemes.WebAdmin.Components;
using IdleSchemes.WebAdmin.ViewModels;
using IdleSchemes.Core;
using IdleSchemes.Core.Helpers;

namespace IdleSchemes.WebAdmin {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            Dependencies.Add(builder.Services);
            builder.Services.AddAll(Assembly.GetExecutingAssembly(), typeof(ViewModelBase), ServiceLifetime.Scoped);

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddControllers(); 
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment()) {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAntiforgery();
            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();
            app.MapControllers();

            app.Run();
        }
    }
}
