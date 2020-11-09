using DamianTourBackend.Core.Entities;
using FluentValidation;
using System;
using System.Linq;

namespace DamianTourBackend.Application.RouteRegistration
{
    public class RouteRegistrationValidator : AbstractValidator<RouteRegistrationDTO>
    {
        //private readonly string[] _sizes = new string[] { "s", "m", "l", "xl", "xxl" };
        public RouteRegistrationValidator()
        {
            RuleFor(x => x.RouteId)
                .NotEmpty().WithMessage("Id cannot be null");

            //RuleFor(x => x.OrderedShirt)
            //    .NotEmpty().WithMessage("Must choose if you want a shirt");

            RuleFor(x => x.ShirtSize)
                //.NotEmpty().WithMessage("Size of shirt cannot be empty")
                .Must(CheckSize)
                .MaximumLength(5).WithMessage("Size of shirt must be valid");
        }

        private bool CheckSize(string shirtSize)
        {
            foreach (ShirtSize size in Enum.GetValues(typeof(ShirtSize)))
                if (shirtSize.ToLower().Equals(size.ToString().ToLower()))
                    return true;
            return false;
        }
    }
}
