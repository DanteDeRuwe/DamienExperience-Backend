using AspNetCore.Identity.Mongo.Model;
using DamianTourBackend.Api.Controllers;
using DamianTourBackend.Application;
using DamianTourBackend.Application.UpdateProfile;
using DamianTourBackend.Core.Interfaces;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
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
        private readonly RoleManager<MongoRole> _rm;
        private readonly IRegistrationRepository _registrationRepository;

        public ProfileControllerTest()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _updateProfileValidator = Substitute.For<IValidator<UpdateProfileDTO>>();
            _um = Substitute.For<FakeUserManager>();
            _rm = Substitute.For<FakeRoleManager>();
            _registrationRepository =  Substitute.For<IRegistrationRepository>();

            _sut = new ProfileController(_userRepository, _updateProfileValidator, _um, _rm, _registrationRepository);
        }

        [Fact]
        public void GetProfile_UserLoggedIn_ReceivesUser()
        {
            // Arrange 
            var user = DummyData.UserFaker.Generate();
            _sut.ControllerContext = FakeControllerContext.For(user);
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
            _sut.ControllerContext = FakeControllerContext.NotLoggedIn;

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
            _sut.ControllerContext = FakeControllerContext.For(user);
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
            var idUser = FakeAppUser.For(user);

            _um.FindByNameAsync(idUser.Email).Returns(idUser);
            _um.DeleteAsync(idUser).Returns(IdentityResult.Success);
            _sut.ControllerContext = FakeControllerContext.For(user);
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
            _sut.ControllerContext = FakeControllerContext.For(user);
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
            _sut.ControllerContext = FakeControllerContext.NotLoggedIn;
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
            var appUser = FakeAppUser.For(user);

            _updateProfileValidator.SetupPass();
            _um.FindByNameAsync(appUser.Email).Returns(appUser);
            _um.UpdateAsync(appUser).Returns(IdentityResult.Success);

            _sut.ControllerContext = FakeControllerContext.For(user);
            _userRepository.GetBy(user.Email).Returns(user);

            // Act 
            var result = await _sut.Update(updateProfileDTO);

            // Assert 
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(
                    updateProfileDTO, 
                    options => options.Excluding(u=>u.Friends)
                );
            _userRepository.Received().Update(user);
        }

        [Fact]
        public async Task UpdateProfile_ValidationFailed_ShouldReturnBadRequest()
        {
            // Arrange 
            var updateProfileDTO = DummyData.UpdateProfileDTOFaker.Generate();
            var user = DummyData.UserFaker.Generate();
            _updateProfileValidator.SetupFail();

            _sut.ControllerContext = FakeControllerContext.For(user);

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
            var appUser = FakeAppUser.For(user);
            
            _um.FindByNameAsync(appUser.Email).Returns(appUser);
            _updateProfileValidator.SetupPass();
            _sut.ControllerContext = FakeControllerContext.For(user);

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
            var updateProfileDTO = DummyData.UpdateProfileDTOFaker.Generate();
            _updateProfileValidator.SetupPass();
            _sut.ControllerContext = FakeControllerContext.NotLoggedIn;

            // Act 
            var meResult = await _sut.Update(updateProfileDTO);
            // Assert 
            meResult.Should().BeOfType<UnauthorizedResult>();
        }
    }
}