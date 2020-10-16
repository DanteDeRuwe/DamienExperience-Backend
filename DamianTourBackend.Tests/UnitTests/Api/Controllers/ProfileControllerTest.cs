using DamianTourBackend.Api.Controllers;
using DamianTourBackend.Application;
using DamianTourBackend.Application.UpdateProfile;
using DamianTourBackend.Core.Interfaces;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Xunit;

namespace DamianTourBackend.Tests.UnitTests.Api.Controllers
{
    public class ProfileControllerTest
    {
        private readonly IUserRepository _userRepository;
        private readonly ProfileController _sut;
        private readonly UserManager<AppUser> _um;
        private readonly IValidator<UpdateProfileDTO> _updateProfileValidator;

        public ProfileControllerTest()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _updateProfileValidator = Substitute.For<IValidator<UpdateProfileDTO>>();
            _um = Substitute.For<FakeUserManager>();

            _sut = new ProfileController(_userRepository, _updateProfileValidator, _um);
        }

        [Fact]
        public void GetProfile_UserLoggedIn_ReceivesUser()
        {
            // Arrange 
            var user = DummyData.UserFaker.Generate();
            //// mocking identity
            var AppUser = new GenericIdentity(user.Email);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(AppUser)
                }
            };
            _sut.ControllerContext = context;
            ////
            _userRepository.GetBy(user.Email).Returns(user);

            // Act 
            var meResult = _sut.Get();

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
            var meResult = _sut.Get();
            // Assert 
            meResult.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public void GetProfile_UserLoggedIn_FailsAndReturnsBadRequestResult()
        {
            // Arrange 
            var user = DummyData.UserFaker.Generate();
            //// mocking identity
            var AppUser = new GenericIdentity(user.Email);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(AppUser)
                }
            };
            _sut.ControllerContext = context;
            ////
            _userRepository.GetBy(user.Email).ReturnsNull();
            // Act 
            var meResult = _sut.Get();

            // Assert 
            meResult.Should().BeOfType<BadRequestResult>();
        }


        [Fact]
        public async Task DeleteProfile_ProfileDeleted_Succes()
        {
            // Arrange 
            var user = DummyData.UserFaker.Generate();
            var idUser = new AppUser() { UserName = user.Email, Email = user.Email };


            _um.FindByNameAsync(idUser.Email).Returns(idUser);
            _um.DeleteAsync(idUser).Returns(IdentityResult.Success);
            //// mocking identity
            var AppUser = new GenericIdentity(user.Email);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(AppUser)
                }
            };
            _sut.ControllerContext = context;
            ////
            _userRepository.GetBy(user.Email).Returns(user);


            // Act 
            var result = await _sut.Delete();
            // Assert 
            result.Should().BeOfType<OkResult>();
            _userRepository.Received().Delete(user);
        }

        [Fact]
        public async Task DeleteProfile_UserNotFound_FailsAndReturnsBadRequestResult()
        {
            // Arrange 
            var user = DummyData.UserFaker.Generate();
            //// mocking identity
            var AppUser = new GenericIdentity(user.Email);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(AppUser)
                }
            };
            _sut.ControllerContext = context;
            ////
            _userRepository.GetBy(user.Email).ReturnsNull();


            // Act 
            var result = await _sut.Delete();
            // Assert 
            result.Should().BeOfType<BadRequestResult>();
        }


        [Fact]
        public async Task DeleteProfile_UserNotLoggedIn_FailsAndReturnsUnauthorizedResult()
        {
            // Arrange 
            var user = DummyData.UserFaker.Generate();
            //// mocking identity
            var AppUser = new GenericIdentity(user.Email);

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
            var result = await _sut.Delete();
            // Assert 
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task UpdateProfile_ProfileUpdated_Success()
        {
            // Arrange 
            var updateProfileDTO = DummyData.UpdateProfileDTOFaker.Generate();
            var user = DummyData.UserFaker.Generate();
            var idUser = new AppUser() { UserName = user.Email, Email = user.Email };

            _updateProfileValidator.SetupPass();
            _um.FindByNameAsync(idUser.Email).Returns(idUser);
            _um.UpdateAsync(idUser).Returns(IdentityResult.Success);

            //// mocking identity
            var AppUser = new GenericIdentity(user.Email);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(AppUser)
                }
            };
            _sut.ControllerContext = context;
            ////
            _userRepository.GetBy(user.Email).Returns(user);


            // Act 
            var result = await _sut.Update(updateProfileDTO);
            // Assert 
            result.Should().BeOfType<OkResult>();
            _userRepository.Received().Update(user);
        }

        [Fact]
        public async Task UpdateProfile_ValidationFailed_ShouldReturnBadRequest()
        {
            // Arrange 
            var updateProfileDTO = DummyData.UpdateProfileDTOFaker.Generate();
            var user = DummyData.UserFaker.Generate();
            _updateProfileValidator.SetupFail();

            //// mocking identity
            var AppUser = new GenericIdentity(user.Email);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(AppUser)
                }
            };
            _sut.ControllerContext = context;
            ////

            // Act 
            var result = await _sut.Update(updateProfileDTO);
            // Assert 
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateProfile_UserNotFound_ShouldReturnBadRequest()
        {
            // Arrange 
            var updateProfileDTO = DummyData.UpdateProfileDTOFaker.Generate();
            var user = DummyData.UserFaker.Generate();
            var idUser = new AppUser() { UserName = user.Email, Email = user.Email };
            _um.FindByNameAsync(idUser.Email).Returns(idUser);
            _updateProfileValidator.SetupPass();

            //// mocking identity
            var AppUser = new GenericIdentity(user.Email);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(AppUser)
                }
            };
            _sut.ControllerContext = context;
            ////

            _userRepository.GetBy(user.Email).ReturnsNull();

            // Act 
            var result = await _sut.Update(updateProfileDTO);
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
            var meResult = await _sut.Update(updateProfileDTO);
            // Assert 
            meResult.Should().BeOfType<UnauthorizedResult>();
        }
    }
}