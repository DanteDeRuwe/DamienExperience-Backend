using DamianTourBackend.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DamianTourBackend.Core.Interfaces
{
    public interface IUserRepository
    {
        User GetBy(string email);
        User GetBy(Guid id);
        void Add(User parkUser);
        IEnumerable<User> GetAll();
        void Update(User user);
        void SaveChanges();
    }
}
