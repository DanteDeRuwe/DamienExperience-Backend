using DamianTourBackend.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DamianTourBackend.Core.Interfaces
{
    public interface IWalkRepository
    {
        Walk GetBy(Guid id);
        void Add(string mail, Walk walk);
        void Update(string mail, Walk walk);
        void Delete(string mail, Walk walk);
        IEnumerable<Walk> GetAllWalksUser(Guid userId);
        IEnumerable<Walk> GetAllWalksForRoute(Guid routeId);
        Walk GetByUserAndRoute(Guid userId, Guid routeId);
    }
}
