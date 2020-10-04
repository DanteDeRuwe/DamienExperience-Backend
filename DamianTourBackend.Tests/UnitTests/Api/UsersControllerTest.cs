using DamianTourBackend.Api.Controllers;
using DamianTourBackend.Core.DTOs;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NSubstitute;
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
            _config = Substitute.For<IConfiguration>();
            _sut = new UsersController(_userRepository, _sim, _um, _config);
        }

        [Fact]
        public void Register_NewUser_ShouldRegister()
        {
            // Arrange
            var registerDTO = new RegisterDTO() { LastName = "Doe", FirstName = "John", Email = "johndoe@email.be", Password = "test", PasswordConfirmation = "test" };
            var user = new User("Doe", "John", "johndoe@email.be", "+32494567800");

            // Act          
            var result = _sut.Register(registerDTO);

            // Assert
            _userRepository.Received().Add(Arg.Any<User>());
            _userRepository.Received().SaveChanges();
            _userRepository.GetBy("johndoe@email.be").Returns(user);
        }
    }


}