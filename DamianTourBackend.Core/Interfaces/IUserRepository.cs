﻿using DamianTourBackend.Core.Entities;
using System;
using System.Collections.Generic;

namespace DamianTourBackend.Core.Interfaces
{
    public interface IUserRepository
    {
        User GetBy(string email);
        User GetById(Guid id);
        void Add(User user);
        void Delete(User user);
        IEnumerable<User> GetAll();
        void Update(User user);
    }
}
