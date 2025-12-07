using System.Threading.Tasks;
using IdleSchemes.Api;
using IdleSchemes.Core;
using IdleSchemes.Core.Services;
using IdleSchemes.Data;
using IdleSchemes.Data.Models;
using IdleSchemes.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdleSchemes.Tests {
    public class TestBase {

        private ServiceProvider _serviceProvider;

        [SetUp]
        public async Task SetUp() {

            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> {
                { "ConnectionStrings:Database", "Host=localhost;Port=5432;Database=idle_events;Username=e;Password=e;" }
            }).Build();

            ServiceCollection serviceCollection = new ServiceCollection();
            Dependencies.Add(serviceCollection);
            serviceCollection.AddScoped<INavigationManager, MockNavigationManager>();
            serviceCollection.AddLogging(b => b.AddDebug());
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddScoped<TestSetupService>();

            var apiAssembly = typeof(Program).Assembly;
            var apiTypes = apiAssembly.GetTypes();
            var idleSchemesApiControllerTypes = apiTypes
                .Where(t => (t.FullName?.StartsWith("IdleSchemes.Api") ?? false) && typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract);
            foreach (var type in idleSchemesApiControllerTypes) {
                serviceCollection.AddScoped(type);
            }

            _serviceProvider = serviceCollection.BuildServiceProvider();

            using (var scope = _serviceProvider.CreateScope()) {
                await scope.ServiceProvider.GetRequiredService<TestSetupService>().DestroyAsync();
            }

            using (var scope = _serviceProvider.CreateScope()) {
                await scope.ServiceProvider.GetRequiredService<TestSetupService>().SetupAsync();
            }

        }

        [TearDown]
        public void TearDown() {
            _serviceProvider.Dispose();
        }

        protected async Task<TResult> ExecuteAction<TController, TResult>(Func<TController, TResult> func, string? email = null) where TController : notnull, ControllerBase {
            using (var scope = _serviceProvider.CreateScope()) {
                await LoginAsAsync(scope, email);
                return func(scope.ServiceProvider.GetRequiredService<TController>());
            }
        }

        protected async Task<TResult> ExecuteAction<TController, TResult>(Func<TController, ActionResult<TResult>> func, string? email = null) where TController : notnull, ControllerBase {
            using (var scope = _serviceProvider.CreateScope()) {
                await LoginAsAsync(scope, email);
                var actionResult = func(scope.ServiceProvider.GetRequiredService<TController>());
                return GetOkResult(actionResult);
            }
        }

        protected async Task<TResult> ExecuteAsyncAction<TController, TResult>(Func<TController, Task<TResult>> func, string? email = null) where TController : notnull, ControllerBase {
            using (var scope = _serviceProvider.CreateScope()) {
                await LoginAsAsync(scope, email);
                return await func(scope.ServiceProvider.GetRequiredService<TController>());
            }
        }

        protected async Task<TResult> ExecuteAsyncAction<TController, TResult>(Func<TController, Task<ActionResult<TResult>>> func, string? email = null) where TController : notnull, ControllerBase {
            using (var scope = _serviceProvider.CreateScope()) {
                await LoginAsAsync(scope, email);
                var actionResult = await func(scope.ServiceProvider.GetRequiredService<TController>());
                return GetOkResult(actionResult);
            }
        }

        private TResult GetOkResult<TResult>(ActionResult<TResult> actionResult) {
            var innerResult = actionResult.Result;
            if (innerResult is OkObjectResult okResult) {
                return (TResult)okResult.Value!;
            } else {
                throw new Exception($"Result was not {typeof(OkObjectResult).Name}, instead was {actionResult.GetType().Name}");
            }
        }

        protected IServiceScope CreateServiceScope() {
            return _serviceProvider.CreateScope();
        }

        protected async Task UpdateDatabaseAsync(Action<IdleDbContext> action) {
            using (var scope = CreateServiceScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IdleDbContext>();
                action(dbContext);
                await dbContext.SaveChangesAsync();
            }
        }

        protected async Task LoginAsAsync(IServiceScope scope, string? email) {
            if (email is null) {
                return;
            }
            User? user = await scope.ServiceProvider.GetRequiredService<UserService>().TryGetUserAsync(email);
            if (user is null) {
                throw new Exception($"User email not found: {email}");
            }
            await scope.ServiceProvider.GetRequiredService<UserSessionService>().CreateSessionForUserAsync(user);
        }
    }
}
