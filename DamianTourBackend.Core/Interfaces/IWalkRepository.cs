using DamianTourBackend.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DamianTourBackend.Core.Interfaces
{
    public interface IWalkRepository
    {
        Walk GetBy(Guid id);
        Walk GetByName(string tourname);
        void Add(Walk walk);
        void Update(Walk walk);
        void Delete(Walk walk);
        IEnumerable<Walk> GetAllWalksUser(Guid userId);
        IEnumerable<Walk> GetAllWalksForTour(Guid routeId);
    }
}
