using System;
using System.Threading.Tasks;
using DamianTourBackend.Api;
using DamianTourBackend.Application.Login;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using DamianTourBackend.Tests.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Xunit;

namespace DamianTourBackend.Tests.IntegrationTests
{
    public class PersonTests : IClassFixture<CustomAppFactory<Startup>>, IDisposable
    {
        private readonly CustomAppFactory<Startup> _factory;

        public PersonTests(CustomAppFactory<Startup> fixture)
        {
            _factory = fixture;
        }

        [Fact]
        public async Task Login()
        {
            // Arrange
            var client = _factory.CreateClient();
            var loginDTO = new LoginDTO() {Email = "string@string.string", Password = "stringstring"};
            var loginContent = new JsonContent(loginDTO);

            // Act
            var response = await client.PostAsync("/api/login", loginContent);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            content.Should().MatchRegex("^[A-Za-z0-9-_=]+\\.[A-Za-z0-9-_=]+\\.?[A-Za-z0-9-_.+/=]*$");
        }

        public void Dispose() => _factory?.Dispose();
    }
}
