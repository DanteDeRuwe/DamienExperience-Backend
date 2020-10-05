using Bogus;
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

    }
}
