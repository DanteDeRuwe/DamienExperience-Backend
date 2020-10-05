using DamianTourBackend.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Abstractions;

namespace DamianTourBackend.Tests.UnitTests.Application.Validators
{
    public class RegisterValidatorTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly RegisterValidator _sut;

        public RegisterValidatorTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _sut = new RegisterValidator();
        }

        [Fact]
        public void Validate_GoodUser_ShouldPass()
        {
            // Arrange
            var registerDTO = DummyData.RegisterDTOFaker.Generate();

            // Act
            var result = _sut.TestValidate(registerDTO);

            // Assert          
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("plainaddress")]
        [InlineData("#@%^%#$@#$@#.com")]
        [InlineData("@example.com")]
        [InlineData("Joe Smith <email@example.com>")]
        [InlineData("email.example.com")]
        [InlineData("email@example@example.com")]
        [InlineData(".email@example.com")]
        [InlineData("email.@example.com")]
        [InlineData("あいうえお@example.com")]
        [InlineData("email@example.com (Joe Smith)")]
        [InlineData("email@example")]
        [InlineData("email@-example.com")]
        [InlineData("email@example..com")]
        public void Validate_BadEmail_ShouldFail(string email)
        {
            // Arrange
            var registerDTO = DummyData.RegisterDTOFaker.Generate();
            registerDTO.Email = email;

            // Act
            var result = _sut.TestValidate(registerDTO);

            // Assert          
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

    }
}
