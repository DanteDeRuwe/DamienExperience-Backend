using DamianTourBackend.Api.Controllers;
using DamianTourBackend.Application.DTOs;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace DamianTourBackend.Tests.UnitTests.Api
{
    public class UsersControllerTest
    {
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<IdentityUser> _sim;
        private readonly UsersController _sut;
        private readonly UserManager<IdentityUser> _um;
        private readonly IConfiguration _config;

        public UsersControllerTest()
        {
            _userRepository = Substitute.For<IUserRepository>();

            _sim = new FakeSignInManager();
            _um = new FakeUserManager();
            _config = FakeConfiguration.Get();

            _sut = new UsersController(_userRepository, _sim, _um, _config);
        }

        [Fact]
        public async Task Register_NewUser_ShouldRegisterAsync()
        {
            // Arrange
            var registerDTO = new RegisterDTO()
            {
                LastName = "Doe",
                FirstName = "John",
                Email = "johndoe@email.be",
                Password = "test",
                PasswordConfirmation = "test"
            };
            var user = new User("Doe", "John", "johndoe@email.be", "+32494567800");

            // Act          
            var result = await _sut.Register(registerDTO);

            // Assert
            _userRepository.Received().Add(Arg.Any<User>());
            _userRepository.Received().SaveChanges();
            result.Should().BeOfType<CreatedResult>();
        }

        [Fact]
        public async Task Register_UserAlreadyRegistered_ShouldNotRegisterAndReturnsBadRequest()
        {
            // Arrange
            var registerDTO = new RegisterDTO()
            {
                LastName = "Doe",
                FirstName = "John",
                Email = "johndoe@email.be",
                Password = "test",
                PasswordConfirmation = "test"
            };

            var user = new User("Doe", "John", "johndoe@email.be", "+32494567800");
            _userRepository.GetBy(user.Email).Returns(user); //already registered

            // Act     
            var secondTimeRegister = await _sut.Register(registerDTO);

            // Assert
            _userRepository.DidNotReceive().Add(Arg.Any<User>());
            _userRepository.DidNotReceive().SaveChanges();
            secondTimeRegister.Should().BeOfType<BadRequestResult>();
        }
    }
}