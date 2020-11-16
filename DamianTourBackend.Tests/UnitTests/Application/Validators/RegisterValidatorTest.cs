using DamianTourBackend.Application.Register;
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
        [InlineData(null)]
        [InlineData("")]
        [InlineData("                                           ")]
        public void Validate_BadFirstName_ShouldFail(string name)
        {
            // Arrange
            var registerDTO = DummyData.RegisterDTOFaker.Generate();
            registerDTO.FirstName = name;

            // Act
            var result = _sut.TestValidate(registerDTO);

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
            var registerDTO = DummyData.RegisterDTOFaker.Generate();
            registerDTO.LastName = name;

            // Act
            var result = _sut.TestValidate(registerDTO);

            // Assert          
            result.ShouldHaveValidationErrorFor(x => x.LastName);
        }


        [Theory]
        [InlineData(null)] //should be optional
        [InlineData("")]
        [InlineData("       ")]

        [InlineData("0101234567")]
        [InlineData("010-1234567")]
        [InlineData("010 - 123 45 67")]
        [InlineData("010 1234 567")]
        [InlineData("06-12345678")]
        [InlineData("06 123 456 78")]
        [InlineData("0111-123456")]
        [InlineData("0111 123456")]
        [InlineData("+31101234567")]
        [InlineData("0031101234567")]
        [InlineData("+31(0) 10123 4567")]
        [InlineData("+3110-1234567")]
        [InlineData("003110 1234567")]
        [InlineData("+316 123 456 78")]
        [InlineData("+31(0)6 123 45678")]
        [InlineData("+31111-123456")]
        [InlineData("0031111-123456")]
        [InlineData("010 22 33 44")]
        [InlineData("0412 333 444 555")]
        public void Validate_GoodPhone_ShouldPass(string phone)
        {
            // Arrange
            var registerDTO = DummyData.RegisterDTOFaker.Generate();
            registerDTO.PhoneNumber = phone;

            // Act
            var result = _sut.TestValidate(registerDTO);

            // Assert          
            result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
        }

        [Theory]
        [InlineData("123")]
        [InlineData("010 11 22 3d")]
        [InlineData("0412 112 235 222 222 222 222")]
        public void Validate_BadPhone_ShouldFail(string phone)
        {
            // Arrange
            var registerDTO = DummyData.RegisterDTOFaker.Generate();
            registerDTO.PhoneNumber = phone;

            // Act
            var result = _sut.TestValidate(registerDTO);

            // Assert          
            result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        }


        [Theory]
        [InlineData(null)] //should be optional
        [InlineData("")]
        [InlineData("       ")]
        [InlineData("01012001")]
        [InlineData("01/01/2001")]
        [InlineData("01.01.2001")]
        [InlineData("01 01 2001")]
        [InlineData("29/02/2004")] //leap year
        [InlineData("29/02/2000")] //leap year
        public void Validate_GoodDoB_ShouldPass(string dob)
        {
            // Arrange
            var registerDTO = DummyData.RegisterDTOFaker.Generate();
            registerDTO.DateOfBirth = dob;

            // Act
            var result = _sut.TestValidate(registerDTO);

            // Assert          
            result.ShouldNotHaveValidationErrorFor(x => x.DateOfBirth);
        }


        [Theory]
        [InlineData("date: 01012001")]
        [InlineData("dob01012001")]
        [InlineData("###01012001")]
        [InlineData("32 01 2001")]
        [InlineData("30 02 2001")]
        [InlineData("31 04 2001")]
        [InlineData("3")]
        [InlineData("010101")]
        [InlineData("010120011")]
        [InlineData("29/02/2001")] //not leap year
        [InlineData("29/02/1900")] //not leap year
        public void Validate_BadDoB_ShouldFail(string dob)
        {
            // Arrange
            var registerDTO = DummyData.RegisterDTOFaker.Generate();
            registerDTO.DateOfBirth = dob;

            // Act
            var result = _sut.TestValidate(registerDTO);

            // Assert          
            result.ShouldHaveValidationErrorFor(x => x.DateOfBirth);
        }

        [Theory]
        [InlineData("TestPassword", "testPassword")]
        [InlineData("TestPassword", "Test Password")]
        [InlineData("Test-Password", "test-password")]
        [InlineData("TestPassword", "testpassword")]
        [InlineData("TestPassword", "Test0Password")]
        [InlineData("TestPassword", "TestPassw0rd")]
        public void Validate_NonMatchingPasswords_ShouldFail(string pass, string confirm)
        {
            // Arrange
            var registerDTO = DummyData.RegisterDTOFaker.Generate();
            registerDTO.Password = pass;
            registerDTO.PasswordConfirmation = confirm;

            // Act
            var result = _sut.TestValidate(registerDTO);

            // Assert          
            result.ShouldNotHaveValidationErrorFor(x => x.Password);
            result.ShouldHaveValidationErrorFor(x => x.PasswordConfirmation);
        }
    }
}
