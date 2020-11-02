using DamianTourBackend.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DamianTourBackend.Core.Interfaces
{
    public interface IWalkRepository
    {
        Walk GetBy(Guid id);
        void Add(Walk walk);
        void Update(Walk walk);
        void Delete(Walk walk);
        IEnumerable<Walk> GetAllWalksUser(Guid userId);
        IEnumerable<Walk> GetAllWalksForRoute(Guid routeId);
        Walk GetByUserAndRoute(Guid userId, Guid routeId);
    }
}
