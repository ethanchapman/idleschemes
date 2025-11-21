using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace IdleSchemes.Core.Helpers {
    public static class DependencyInjectionHelpers {
    
        public static void AddAll(this IServiceCollection serviceCollection, Assembly assembly, Type type, ServiceLifetime serviceLifetime) {
            var types = assembly.GetTypes()
                .Where(t => type.IsAssignableFrom(t) && !t.IsAbstract && !t.IsGenericType && !t.IsInterface);
            foreach (var t in types) {
                serviceCollection.Add(new ServiceDescriptor(t, t, serviceLifetime));
            }
        }

    }
}
