﻿using FluentValidation;
using System.Text.RegularExpressions;

namespace DamianTourBackend.Application.UpdateProfile
{
    public class UpdateProfileValidator : AbstractValidator<UpdateProfileDTO>
    {
        public UpdateProfileValidator()
        {
            RuleFor(x => x.FirstName)
                    .NotEmpty().WithMessage("First name is required")
                    .MaximumLength(200).WithMessage("First name cannot be longer than 200 characters");

            RuleFor(x => x.LastName)
                    .NotEmpty().WithMessage("Last name is required")
                    .MaximumLength(200).WithMessage("Last name cannot be longer than 200 characters");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .Must(BeValidEmail).WithMessage("Invalid email format.");
        }
        private bool BeValidEmail(string email)
        {
            if (email == null) return false;
            string validEmailRegex = "^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-z])@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-z][-\\w]*[0-9a-z]*\\.)+[a-z0-9][\\-a-z0-9]{0,22}[a-z0-9]))$";
            return Regex.IsMatch(email?.ToLower(), validEmailRegex);
        }

    }
}
