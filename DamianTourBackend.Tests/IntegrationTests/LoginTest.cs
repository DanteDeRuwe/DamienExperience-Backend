using System;
using System.Threading.Tasks;
using DamianTourBackend.Api;
using DamianTourBackend.Application.Login;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace DamianTourBackend.Tests.IntegrationTests
{
    public class LoginTest : IDisposable
    {
        private WebApplicationFactory<Startup> _factory;

        public LoginTest()
        {
            _factory = new WebApplicationFactory<Startup>();
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
            content.Should().MatchRegex("^[A-Za-z0-9-_=]+\\.[A-Za-z0-9-_=]+\\.?[A-Za-z0-9-_.+/=]*$");
        }

        public void Dispose() => _factory?.Dispose();
    }
}