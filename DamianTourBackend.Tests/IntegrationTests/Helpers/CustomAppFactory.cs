using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace DamianTourBackend.Tests.IntegrationTests.Helpers
{
    public class CustomAppFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.Remove<IMongoDatabase>(ServiceLifetime.Singleton);
                services.AddSingleton(x =>
                {
                    var client = new MongoClient();
                    return client.GetDatabase("DamienTourIntegrationTestDB");
                });
            });
        }
    }
}