using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace DamianTourBackend.Infrastructure.Data.Repositories
{
    class RegistrationRepository : IRegistrationRepository
    {
        public readonly IMongoCollection<Registration> _routes;

        public RegistrationRepository(IMongoDatabase db)
        {

        }

        public void Add(Registration registration)
        {
            throw new NotImplementedException();
        }

        public void Delete(Route registration)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Registration> GetAll()
        {
            throw new NotImplementedException();
        }

        public Registration GetBy(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Update(Registration registration)
        {
            throw new NotImplementedException();
        }
    }
}
