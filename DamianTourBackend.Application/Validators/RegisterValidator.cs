using DamianTourBackend.Application.DTOs;
using FluentValidation;

namespace DamianTourBackend.Application.Validators
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

            RuleFor(x => x.PasswordConfirmation)
                .NotEmpty().WithMessage("Password confirmation is required")
                .Matches("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$").WithMessage("Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")
                .Equal(x => x.Password).WithMessage("Passwords should match.");
        }
    }
}
