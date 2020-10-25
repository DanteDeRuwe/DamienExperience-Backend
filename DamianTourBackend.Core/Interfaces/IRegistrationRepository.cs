using DamianTourBackend.Core.Entities;
using System;
using System.Collections.Generic;

namespace DamianTourBackend.Core.Interfaces
{
    public interface IRegistrationRepository
    {
        Registration GetBy(Guid id);
        void Add(Registration registration);
        void Delete(Route registration);
        IEnumerable<Registration> GetAll();
        void Update(Registration registration);
    }
}
