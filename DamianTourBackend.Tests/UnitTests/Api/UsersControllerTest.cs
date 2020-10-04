using DamianTourBackend.Api.Controllers;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using NSubstitute;
using Xunit;

namespace DamianTourBackend.Tests.UnitTests.Api
{
    public class UsersControllerTest
    {
        private readonly IUserRepository _userRepository;
        private readonly UsersController _sut;

        public UsersControllerTest()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _sut = new UsersController(_userRepository);
        }

        [Fact]
        public void Register_NewUser_ShouldRegister()
        {
            // Arrange
            var user = new User("Doe", "John", "johndoe@email.be", "+32 471 234 567");

            // Act          
            _sut.Register(user);

            // Assert
            _userRepository.Received().Add(Arg.Any<User>());
            _userRepository.Received().SaveChanges();
            _userRepository.GetBy("johndoe@email.be").Returns(user);
        }
    }


}
