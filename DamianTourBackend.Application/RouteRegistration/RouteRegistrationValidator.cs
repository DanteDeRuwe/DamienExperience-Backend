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

            RuleFor(x => x.OrderedShirt)
                .NotEmpty().WithMessage("Must choose if you want a shirt");

            RuleFor(x => x.SizeShirt)
                //.NotEmpty().WithMessage("Size of shirt cannot be empty")
                .Must(CheckSize)
                .MaximumLength(5).WithMessage("Size of shirt must be valid");
        }

        private bool CheckSize(string sizeShirt)
        {
            foreach (string size in Enum.GetValues(typeof(ShirtSize)))
                if (sizeShirt.Equals(size))
                    return true;
            return false;
        }
    }
}
