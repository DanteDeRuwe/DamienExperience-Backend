using DamianTourBackend.Api.Controllers;
using DamianTourBackend.Application.DTOs;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace DamianTourBackend.Tests.UnitTests.Api
{
    public class UsersControllerTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<IdentityUser> _sim;
        private readonly UsersController _sut;
        private readonly UserManager<IdentityUser> _um;
        private readonly IConfiguration _config;
        private readonly IValidator<RegisterDTO> _registerValidator;
        private readonly IValidator<LoginDTO> _loginValidator;

        public UsersControllerTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _userRepository = Substitute.For<IUserRepository>();

            _sim = Substitute.For<FakeSignInManager>();
            _um = Substitute.For<FakeUserManager>();
            _config = FakeConfiguration.Get();
            _registerValidator = Substitute.For<IValidator<RegisterDTO>>();
            _loginValidator = Substitute.For<IValidator<LoginDTO>>();


            _sut = new UsersController(_userRepository, _sim, _um, _config, _loginValidator, _registerValidator);
        }
        #region Register Tests

        [Fact]
        public async Task Register_GoodUser_ShouldRegisterAndReturnToken()
        {
            // Arrange
            var registerDTO = DummyData.RegisterDTOFaker.Generate();
            _um.CreateAsync(Arg.Any<IdentityUser>(), Arg.Any<string>()).Returns(IdentityResult.Success);

            _registerValidator.SetupPass();

            // Act          
            var result = await _sut.Register(registerDTO);

            // Assert
            result.Should().BeOfType<CreatedResult>()
                .Which.Value.ToString().Should().MatchRegex("^[A-Za-z0-9-_=]+\\.[A-Za-z0-9-_=]+\\.?[A-Za-z0-9-_.+/=]*$");

            _userRepository.Received().Add(Arg.Any<User>());
            _userRepository.Received().SaveChanges();

        }

        [Fact]
        public async Task Register_EmailAlreadyRegistered_ShouldNotRegisterAndReturnsBadRequest()
        {
            // Arrange
            var user = DummyData.UserFaker.Generate();
            var registerDTO = DummyData.RegisterDTOFaker.Generate();
            registerDTO.Email = user.Email;

            _registerValidator.SetupPass();
            _userRepository.GetBy(user.Email).Returns(user); //already registered

            // Act     
            var secondTimeRegister = await _sut.Register(registerDTO);


            // Assert
            secondTimeRegister.Should().BeOfType<BadRequestResult>();
            _userRepository.DidNotReceive().Add(Arg.Any<User>());
            _userRepository.DidNotReceive().SaveChanges();
        }

        [Fact]
        public async Task Register_ValidationFailed_ShouldNotRegisterAndReturnsBadRequest()
        {
            // Arrange
            _registerValidator.SetupFail();

            // Act     
            var secondTimeRegister = await _sut.Register(new RegisterDTO());

            // Assert
            secondTimeRegister.Should().BeOfType<BadRequestObjectResult>();
            _userRepository.DidNotReceive().Add(Arg.Any<User>());
            _userRepository.DidNotReceive().SaveChanges();
        }
        #endregion

        #region Login Tests
        [Fact]
        public async Task Login_ValidationSucces_ShouldReturnToken()
        {
            // Arrange
            var loginDTO = DummyData.LoginDTOFaker.Generate();
            _loginValidator.SetupPass();
            _um.FindByNameAsync(loginDTO.Email).Returns(new IdentityUser() { UserName = loginDTO.Email, Email = loginDTO.Email });
            _sim.CheckPasswordSignInAsync(Arg.Any<IdentityUser>(), Arg.Any<string>(), Arg.Any<bool>())
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
            _loginValidator.SetupPass();
            _um.FindByNameAsync(loginDTO.Email).ReturnsNull();

            //Act
            var login = await _sut.Login(loginDTO);

            //Assert
            login.Should().BeOfType<BadRequestResult>();

        }

        [Fact]
        public async Task Login_ValidationFailed_ShouldNotLoginAndReturnBadRequest()
        {
            // Arrange
            _loginValidator.SetupFail();

            // Act     
            var loginFail = await _sut.Login(new LoginDTO());

            // Assert
            loginFail.Should().BeOfType<BadRequestObjectResult>();
        }

        #endregion
    }
}