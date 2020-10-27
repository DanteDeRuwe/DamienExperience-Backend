using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;

namespace DamianTourBackend.Infrastructure.Data.Repositories
{
    class RegistrationRepository : IRegistrationRepository
    {
        public readonly IMongoCollection<User> _users;

        public RegistrationRepository(IMongoDatabase db)
        {
            _users = db.GetCollection<User>("Users");

        }

        public void Add(Registration registration, string email)
        {
            _users.FindOneAndUpdateAsync(Builders<User>.Filter.Eq(x => x.Email, email),
                Builders<User>.Update.Push(x => x.Registrations, registration));
        }

        public void Delete(Registration registration, string email)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Email, email);
            var update = Builders<User>.Update.PullFilter(x => x.Registrations,
                reg => reg.Id == registration.Id);
            _users.UpdateOne(filter, update);

        }

        public IEnumerable<Registration> GetAll()
        {
            return _users.AsQueryable().Where(u => u.Registrations.Count >= 1)
                .SelectMany(p => p.Registrations);
        }

        public IEnumerable<Registration> GetAllFromUser(string email)
        {
            return _users.AsQueryable().Where(u => u.Email.Equals(email))
                .SelectMany(p => p.Registrations);
        }

        public Registration GetBy(Guid id, string email)
        {
            return _users.AsQueryable().Where(u => u.Email.Equals(email))
                .SelectMany(p => p.Registrations).Where(r => r.Id.Equals(id)).FirstOrDefault();
        }

        public void Update(Registration registration, string email)
        {
            //mabye rework this later if needed
            //propebly not realy needed
            Delete(registration, email);
            Add(registration, email);
        }
    }
}
