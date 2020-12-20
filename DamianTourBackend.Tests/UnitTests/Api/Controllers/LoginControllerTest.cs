using DamianTourBackend.Api.Controllers;
using DamianTourBackend.Application;
using DamianTourBackend.Application.Login;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System.Threading.Tasks;
using Xunit;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace DamianTourBackend.Tests.UnitTests.Api.Controllers
{
    public class LoginControllerTest
    {
        private readonly IValidator<LoginDTO> _validator;
        private readonly SignInManager<AppUser> _sim;
        private readonly UserManager<AppUser> _um;
        private readonly LoginController _sut;

        public LoginControllerTest()
        {
            _validator = Substitute.For<IValidator<LoginDTO>>();
            _sim = Substitute.For<FakeSignInManager>();
            _um = Substitute.For<FakeUserManager>();
            var config = FakeConfiguration.Get();

            _sut = new LoginController(_validator, _sim, _um, config);
        }

        [Fact]

        public async Task Login_ValidationSuccess_ShouldReturnToken()

        {
            // Arrange
            var loginDTO = DummyData.LoginDTOFaker.Generate();
            _validator.SetupPass();
            _um.FindByNameAsync(loginDTO.Email).Returns(new AppUser() { UserName = loginDTO.Email, Email = loginDTO.Email });

            _sim.CheckPasswordSignInAsync(Arg.Any<AppUser>(), Arg.Any<string>(), Arg.Any<bool>())
                .Returns(Task.FromResult(SignInResult.Success));


            //Act
            var login = await _sut.Login(loginDTO);

            //Assert
            login.Should().BeOfType<OkObjectResult>()
                .Which.Value.ToString().Should().MatchRegex("^[A-Za-z0-9-_=]+\\.[A-Za-z0-9-_=]+\\.?[A-Za-z0-9-_.+/=]*$");

        }

        [Fact]
        public async Task Login_UserNotFoundOrRegistered_ShouldReturnBadRequest()
        {
            // Arrange
            var loginDTO = DummyData.LoginDTOFaker.Generate();
            _validator.SetupPass();
            _um.FindByNameAsync(loginDTO.Email).ReturnsNull();

            //Act
            var login = await _sut.Login(loginDTO);

            //Assert
            login.Should().BeOfType<BadRequestObjectResult>();

        }

        [Fact]
        public async Task Login_ValidationFailed_ShouldNotLoginAndReturnBadRequest()
        {
            // Arrange
            _validator.SetupFail();

            // Act     
            var loginFail = await _sut.Login(new LoginDTO());

            // Assert
            loginFail.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}