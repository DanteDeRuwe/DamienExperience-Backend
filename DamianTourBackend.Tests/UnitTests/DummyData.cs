using Bogus;
using DamianTourBackend.Application.Login;
using DamianTourBackend.Application.Register;
using DamianTourBackend.Application.RouteRegistration;
using DamianTourBackend.Application.UpdateProfile;
using DamianTourBackend.Core.Entities;
using System;

namespace DamianTourBackend.Tests.UnitTests
{
    public static class DummyData
    {
        private static readonly string[] _sizes = new string[] { "s", "m", "l", "xl", "xxl" };

        public static readonly Faker<User> UserFaker = new Faker<User>()
            .RuleFor(u => u.FirstName, f => f.Person.FirstName)
            .RuleFor(u => u.LastName, f => f.Person.LastName)
            .RuleFor(u => u.Email, f => f.Person.Email)
            .RuleFor(u => u.PhoneNumber, f => f.Person.Phone);

        public static readonly Faker<RegisterDTO> RegisterDTOFaker = new Faker<RegisterDTO>()
            .RuleFor(r => r.FirstName, f => f.Person.FirstName)
            .RuleFor(r => r.LastName, f => f.Person.LastName)
            .RuleFor(r => r.Email, f => f.Person.Email)
            .RuleFor(r => r.PhoneNumber, f => f.Phone.PhoneNumber("+## ### ### ###"))
            .RuleFor(r => r.DateOfBirth, f => f.Person.DateOfBirth.ToString("dd-MM-yyyy"))
            .RuleFor(r => r.Password, f => f.Internet.Password(8))
            .RuleFor(r => r.PasswordConfirmation, (f, r) => r.Password);

        public static readonly Faker<LoginDTO> LoginDTOFaker = new Faker<LoginDTO>()
            .RuleFor(r => r.Email, f => f.Person.Email)
            .RuleFor(r => r.Password, f => f.Internet.Password(8));

        public static readonly Faker<UpdateProfileDTO> UpdateProfileDTOFaker = new Faker<UpdateProfileDTO>()
            .RuleFor(r => r.FirstName, f => f.Person.FirstName)
            .RuleFor(r => r.LastName, f => f.Person.LastName)
            .RuleFor(r => r.Email, f => f.Person.Email)
            .RuleFor(r => r.PhoneNumber, f => f.Phone.PhoneNumber("+## ### ### ###"))
            .RuleFor(r => r.DateOfBirth, f => f.Person.DateOfBirth.ToString("dd-MM-yyyy"));

        public static readonly Faker<Route> RouteFaker = new Faker<Route>()
            .RuleFor(r => r.TourName, f => f.Random.Word())
            .RuleFor(r => r.DistanceInMeters, f => f.Random.Int(1, 100))
            .RuleFor(r => r.Id, f => Guid.NewGuid());

        public static readonly Faker<RouteRegistrationDTO> RouteRegistrationDTOFaker = new Faker<RouteRegistrationDTO>()
            .RuleFor(r => r.ShirtSize, f => f.PickRandom(Enum.GetValues(ShirtSize))
            .RuleFor(r => r.OrderedShirt, f => f.Random.Bool())
            .RuleFor(r => r.RouteId, f => Guid.NewGuid());
    }
}
