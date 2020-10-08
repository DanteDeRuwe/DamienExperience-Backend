using DamianTourBackend.Api.Controllers;
using DamianTourBackend.Application.DTOs;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

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
        private readonly IValidator<UpdateProfileDTO> _updateProfileValidator;

        public UsersControllerTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _userRepository = Substitute.For<IUserRepository>();

            _sim = new FakeSignInManager();
            //_um = new FakeUserManager();
            _um = Substitute.For<FakeUserManager>();
            _config = FakeConfiguration.Get();
            _registerValidator = Substitute.For<IValidator<RegisterDTO>>();
            _loginValidator = Substitute.For<IValidator<LoginDTO>>();
            _updateProfileValidator = Substitute.For<IValidator<UpdateProfileDTO>>();

            _sut = new UsersController(_userRepository, _sim, _um, _config, _loginValidator, _registerValidator, _updateProfileValidator);
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
        public async Task Login_ValidationSuccess_ShouldReturnToken()
        {
            // Arrange
            var loginDTO = DummyData.LoginDTOFaker.Generate();
            _loginValidator.SetupPass();
            _um.FindByNameAsync(loginDTO.Email).Returns(new IdentityUser() { UserName = loginDTO.Email, Email = loginDTO.Email });

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

        #region Profile Tests 

        #region GetProfile Tests
        [Fact]
        public void GetProfile_UserLoggedIn_ReceivesUser()
        {
            // Arrange 
            var user = DummyData.UserFaker.Generate();
            //// mocking identity
            var identityUser = new GenericIdentity(user.Email);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identityUser)
                }
            };
            _sut.ControllerContext = context;
            ////
            _userRepository.GetBy(user.Email).Returns(user);

            // Act 
            var meResult = _sut.GetProfile();

            // Assert 
            meResult.Should().BeOfType<OkObjectResult>();
            _userRepository.Received().GetBy(user.Email);
        }


        [Fact]
        public void GetProfile_UserNotLoggedIn_FailsAndReturnsBadRequestResult()
        {
            // Arrange 
            //// mocking identity

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = null
                }
            };
            _sut.ControllerContext = context;
            ////
            // Act 
            var meResult = _sut.GetProfile();
            // Assert 
            meResult.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void GetProfile_UserLoggedIn_FailsAndReturnsBadRequestResult()
        {
            // Arrange 
            var user = DummyData.UserFaker.Generate();
            //// mocking identity
            var identityUser = new GenericIdentity(user.Email);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identityUser)
                }
            };
            _sut.ControllerContext = context;
            ////
            _userRepository.GetBy(user.Email).ReturnsNull();
            // Act 
            var meResult = _sut.GetProfile();

            // Assert 
            meResult.Should().BeOfType<BadRequestResult>();
        }

        #endregion

        #region Delete Profile Tests 

        [Fact]
        public async Task DeleteProfile_ProfileDeleted_Succes()
        {
            // Arrange 
            var user = DummyData.UserFaker.Generate();
            var idUser = new IdentityUser() { UserName = user.Email, Email = user.Email };


            _um.FindByNameAsync(idUser.Email).Returns(idUser);
            _um.DeleteAsync(idUser).Returns(IdentityResult.Success);
            //// mocking identity
            var identityUser = new GenericIdentity(user.Email);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identityUser)
                }
            };
            _sut.ControllerContext = context;
            ////
            _userRepository.GetBy(user.Email).Returns(user);


            // Act 
            var result = await _sut.DeleteProfile();
            // Assert 
            result.Should().BeOfType<OkResult>();
            _userRepository.Received().Delete(user);
            _userRepository.Received().SaveChanges();
        }

        [Fact]
        public async Task DeleteProfile_UserNotFound_FailsAndReturnsBadRequestResult()
        {
            // Arrange 
            var user = DummyData.UserFaker.Generate();
            //// mocking identity
            var identityUser = new GenericIdentity(user.Email);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identityUser)
                }
            };
            _sut.ControllerContext = context;
            ////
            _userRepository.GetBy(user.Email).ReturnsNull();


            // Act 
            var result = await _sut.DeleteProfile();
            // Assert 
            result.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async Task DeleteProfile_UserNotLoggedIn_FailsAndReturnsUnauthorizedResult()
        {
            // Arrange 
            var user = DummyData.UserFaker.Generate();
            //// mocking identity
            var identityUser = new GenericIdentity(user.Email);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = null
                }
            };
            _sut.ControllerContext = context;
            ////
            _userRepository.GetBy(user.Email).ReturnsNull();


            // Act 
            var result = await _sut.DeleteProfile();
            // Assert 
            result.Should().BeOfType<UnauthorizedResult>();
        }

        #endregion

        #region Edit Profile Tests
        [Fact]
        public async Task UpdateProfile_ProfileUpdated_Success()
        {
            // Arrange 
            var updateProfileDTO = DummyData.UpdateProfileDTOFaker.Generate();
            var user = DummyData.UserFaker.Generate();
            var idUser = new IdentityUser() { UserName = user.Email, Email = user.Email };

            _updateProfileValidator.SetupPass();
            _um.FindByNameAsync(idUser.Email).Returns(idUser);
            _um.UpdateAsync(idUser).Returns(IdentityResult.Success);

            //// mocking identity
            var identityUser = new GenericIdentity(user.Email);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identityUser)
                }
            };
            _sut.ControllerContext = context;
            ////
            _userRepository.GetBy(user.Email).Returns(user);


            // Act 
            var result = await _sut.UpdateProfile(updateProfileDTO);
            // Assert 
            result.Should().BeOfType<OkResult>();
            _userRepository.Received().Update(user);
            _userRepository.Received().SaveChanges();
        }

        [Fact]
        public async Task UpdateProfile_ValidationFailed_ShouldReturnBadRequest()
        {
            // Arrange 
            var updateProfileDTO = DummyData.UpdateProfileDTOFaker.Generate();
            var user = DummyData.UserFaker.Generate();
            _updateProfileValidator.SetupFail();

            //// mocking identity
            var identityUser = new GenericIdentity(user.Email);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identityUser)
                }
            };
            _sut.ControllerContext = context;
            ////

            // Act 
            var result = await _sut.UpdateProfile(updateProfileDTO);
            // Assert 
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateProfile_UserNotFound_ShouldReturnBadRequest()
        {
            // Arrange 
            var updateProfileDTO = DummyData.UpdateProfileDTOFaker.Generate();
            var user = DummyData.UserFaker.Generate();
            var idUser = new IdentityUser() { UserName = user.Email, Email = user.Email };
            _um.FindByNameAsync(idUser.Email).Returns(idUser);
            _updateProfileValidator.SetupPass();

            //// mocking identity
            var identityUser = new GenericIdentity(user.Email);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identityUser)
                }
            };
            _sut.ControllerContext = context;
            ////

            _userRepository.GetBy(user.Email).ReturnsNull();

            // Act 
            var result = await _sut.UpdateProfile(updateProfileDTO);
            // Assert 

            result.Should().BeOfType<BadRequestResult>();
        }


        [Fact]
        public async Task UpdateProfile_UserNotLoggedIn_FailsAndReturnsBadRequestResult()
        {
            // Arrange 
            //// mocking identity
            var updateProfileDTO = DummyData.UpdateProfileDTOFaker.Generate();
            _updateProfileValidator.SetupPass();

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = null
                }
            };
            _sut.ControllerContext = context;
            ////
            // Act 
            var meResult = await _sut.UpdateProfile(updateProfileDTO);
            // Assert 
            meResult.Should().BeOfType<UnauthorizedResult>();
        }
        #endregion


        #endregion
    }
}