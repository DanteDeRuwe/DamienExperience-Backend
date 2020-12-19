using DamianTourBackend.Application.Login;
using FluentValidation;
using System;
using DamianTourBackend.Application.Helpers;

namespace DamianTourBackend.Application.Register
{
    public class RegisterValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterValidator()
        {
            Include(new LoginValidator());

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(200).WithMessage("First name cannot be longer than 200 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(200).WithMessage("Last name cannot be longer than 200 characters");

            RuleFor(x => x.PhoneNumber)
                //.NotEmpty().WithMessage("Phone is required")
                .Matches("^((?![A-z]).){5,}$").Unless(x => string.IsNullOrWhiteSpace(x.PhoneNumber)).WithMessage("Phone number has at least 5 non alphanumeric characters")
                .MaximumLength(25).WithMessage("Phone number can't be longer than 25 characters");

            RuleFor(x => x.DateOfBirth)
                //.NotEmpty().WithMessage("Date of birth is required")
                .Must(BeValidBirthDate).Unless(x => string.IsNullOrWhiteSpace(x.DateOfBirth)).WithMessage("Date of birth is invalid, it should be dd MM yyyy")
                .MaximumLength(10).WithMessage("Date of birth can't be longer than 10 characters");


            RuleFor(x => x.PasswordConfirmation)
                .NotEmpty().WithMessage("Password confirmation is required")
                .MinimumLength(8).WithMessage("Password should be a minimum of 8 characters long")
                .Equal(x => x.Password).WithMessage("Passwords should match.");
        }

        private bool BeValidBirthDate(string value)
        {
            DateTime date;
            bool isValidDate = DateParser.TryParse(value, out date);
            if (!isValidDate) return false;
            return (date < DateTime.Today);
        }
    }
}
