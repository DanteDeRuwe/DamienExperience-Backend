using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace DamianTourBackend.Tests.IntegrationTests.Helpers
{
    public static class DependencySwapExtensions
    {
        public static void Remove<TService>(this IServiceCollection services, ServiceLifetime lifetime)
        {
            if (services.Any(x => x.ServiceType == typeof(TService) && x.Lifetime == lifetime))
            {
                var serviceDescriptors =
                    services.Where(x => x.ServiceType == typeof(TService) && x.Lifetime == lifetime).ToList();
                foreach (var serviceDescriptor in serviceDescriptors)
                {
                    services.Remove(serviceDescriptor);
                }
            }
        }

        public static void Swap<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime) 
            where TImplementation : class, TService
        {
            services.Remove<TService>(lifetime);

            switch (lifetime)
            {
                case ServiceLifetime.Scoped:
                    services.AddScoped(typeof(TService), typeof(TImplementation));
                    break;
                case ServiceLifetime.Singleton:
                    services.AddSingleton(typeof(TService), typeof(TImplementation));
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(typeof(TService), typeof(TImplementation));
                    break;
                default: 
                    throw new ArgumentException("Please provide a lifetime");
            }
        }
    }
}