using DamianTourBackend.Application.UpdateProfile;
using FluentValidation.TestHelper;
using Xunit;

namespace DamianTourBackend.Tests.UnitTests.Application.Validators
{
    public class UpdateProfileValidatorTest
    {
        private readonly UpdateProfileValidator _sut;

        public UpdateProfileValidatorTest()
        {
            _sut = new UpdateProfileValidator();
        }

        [Fact]
        public void Validate_GoodUser_ShouldPass()
        {
            // Arrange
            var updateProfileDTO = DummyData.UpdateProfileDTOFaker.Generate();

            // Act
            var result = _sut.TestValidate(updateProfileDTO);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("           ")]
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
            var updateProfileDTO = DummyData.UpdateProfileDTOFaker.Generate();
            updateProfileDTO.Email = email;

            // Act
            var result = _sut.TestValidate(updateProfileDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("                                           ")]
        public void Validate_BadFirstName_ShouldFail(string name)
        {
            // Arrange
            var updateProfileDTO = DummyData.UpdateProfileDTOFaker.Generate();
            updateProfileDTO.FirstName = name;

            // Act
            var result = _sut.TestValidate(updateProfileDTO);

            // Assert          
            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("                                           ")]
        public void Validate_BadLastName_ShouldFail(string name)
        {
            // Arrange
            var updateProfileDTO = DummyData.UpdateProfileDTOFaker.Generate();
            updateProfileDTO.LastName = name;

            // Act
            var result = _sut.TestValidate(updateProfileDTO);

            // Assert          
            result.ShouldHaveValidationErrorFor(x => x.LastName);
        }
    }
}
