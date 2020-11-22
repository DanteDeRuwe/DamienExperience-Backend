using Bogus;
using DamianTourBackend.Application.Login;
using DamianTourBackend.Application.Register;
using DamianTourBackend.Application.RouteRegistration;
using DamianTourBackend.Application.UpdateProfile;
using DamianTourBackend.Application.UpdateRoute;
using DamianTourBackend.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DamianTourBackend.Tests.UnitTests
{
    public static class DummyData
    {
        public static readonly Faker<User> UserFaker = new Faker<User>()
            .RuleFor(u => u.FirstName, f => f.Person.FirstName)
            .RuleFor(u => u.LastName, f => f.Person.LastName)
            .RuleFor(u => u.Email, f => f.Person.Email)
            .RuleFor(u => u.PhoneNumber, f => f.Person.Phone)
            .RuleFor(u => u.Registrations, GenerateRegistrationsForUser);

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
            .RuleFor(r => r.Id, f => Guid.NewGuid())
            .RuleFor(r => r.Date, f => DateTime.Now)
            .RuleFor(r => r.Path, f => new Path() { LineColor = "Black", Coordinates = new List<double[]>() })
            .RuleFor(r => r.Info, f => new Dictionary<string, string>())
            ;

        public static readonly Faker<Route> PastRouteFaker = new Faker<Route>()
            .RuleFor(r => r.TourName, f => f.Random.Word())
            .RuleFor(r => r.DistanceInMeters, f => f.Random.Int(1, 100))
            .RuleFor(r => r.Id, f => Guid.NewGuid())
            .RuleFor(r => r.Date, f => DateTime.Now.AddYears(-1))
            .RuleFor(r => r.Path, f => new Path() { LineColor = "Black", Coordinates = new List<double[]>() })
            .RuleFor(r => r.Info, f => new Dictionary<string, string>())
            ;

        public static readonly Faker<RouteRegistrationDTO> RouteRegistrationDTOFaker = new Faker<RouteRegistrationDTO>()
            .RuleFor(r => r.ShirtSize, f => f.PickRandom<ShirtSize>().ToString())
            .RuleFor(r => r.OrderedShirt, f => f.Random.Bool())
            .RuleFor(r => r.RouteId, f => Guid.NewGuid());

        private static ICollection<Registration> GenerateRegistrationsForUser(Faker f, User u) =>
            RouteRegistrationDTOFaker
                .Generate(f.Random.Int(1, 5))
                .Select(r => r.MapToRegistration(u, RouteFaker.Generate()))
                .ToList();
    }
}