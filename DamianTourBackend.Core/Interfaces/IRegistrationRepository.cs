﻿using DamianTourBackend.Core.Entities;
using System;
using System.Collections.Generic;

namespace DamianTourBackend.Core.Interfaces
{
    public interface IRegistrationRepository
    {
        Registration GetBy(Guid id, string email);
        void Add(Registration registration, string email);
        void Delete(Registration registration, string email);
        IEnumerable<Registration> GetAllFromUser(string email);
        IEnumerable<Registration> GetAll();
        void Update(Registration registration, string email);
    }
}