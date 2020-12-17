using System;
using System.Threading.Tasks;
using DamianTourBackend.Api;
using DamianTourBackend.Application.Login;
using FluentAssertions;
using DamianTourBackend.Tests.IntegrationTests.Helpers;
using Xunit;

namespace DamianTourBackend.Tests.IntegrationTests
{
    public class LoginTest : IClassFixture<CustomAppFactory<Startup>>, IDisposable
    {
        private readonly CustomAppFactory<Startup> _factory;

        public LoginTest(CustomAppFactory<Startup> fixture)
        {
            _factory = fixture;
        }

        [Fact]
        public async Task Login_GoodCredentials_ShouldReturnToken()
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
        
        [Fact]
        public async Task Login_BadCredentials_ShouldNotReturnToken()
        {
            // Arrange
            var client = _factory.CreateClient();
            var loginDTO = new LoginDTO() {Email = "string@string.string", Password = "xxxxxxxx"};
            var loginContent = new JsonContent(loginDTO);

            // Act
            var response = await client.PostAsync("/api/login", loginContent);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.IsSuccessStatusCode.Should().BeFalse();
            content.Should().NotMatchRegex("^[A-Za-z0-9-_=]+\\.[A-Za-z0-9-_=]+\\.?[A-Za-z0-9-_.+/=]*$");
        }

        public void Dispose() => _factory?.Dispose();
    }
}
