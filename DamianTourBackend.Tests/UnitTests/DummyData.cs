using Bogus;
using DamianTourBackend.Application.DTOs;
using DamianTourBackend.Core.Entities;

namespace DamianTourBackend.Tests.UnitTests
{
    public class DummyData
    {
        public static readonly Faker<User> UserFaker = new Faker<User>()
            .RuleFor(u => u.FirstName, f => f.Person.FirstName)
            .RuleFor(u => u.LastName, f => f.Person.LastName)
            .RuleFor(u => u.Email, f => f.Person.Email)
            .RuleFor(u => u.PhoneNumber, f => f.Person.Phone);

        public static readonly Faker<RegisterDTO> RegisterDTOFaker = new Faker<RegisterDTO>()
            .RuleFor(r => r.FirstName, f => f.Person.FirstName)
            .RuleFor(r => r.LastName, f => f.Person.LastName)
            .RuleFor(r => r.Email, f => f.Person.Email)
            .RuleFor(r => r.Password, f => f.Internet.Password(8))
            .RuleFor(r => r.PasswordConfirmation, (f, r) => r.Password);

    }
}
