using DamianTourBackend.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DamianTourBackend.Core.Interfaces
{
    public interface IRouteRepository
    {
        Route GetBy(Guid id);
        void Add(Route route);
        void Delete(Route route);
        IEnumerable<Route> GetAll();
        void Update(Route route);
    }
}
