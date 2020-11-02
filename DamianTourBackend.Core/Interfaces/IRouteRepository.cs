using DamianTourBackend.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DamianTourBackend.Core.Interfaces
{
    public interface IRouteRepository
    {
        Route GetBy(Guid id);
        Route GetByName(string tourname);
        void Add(Route route);
        void Update(Route route);
        void Delete(Route route);
        IEnumerable<Route> GetAll();
    }
}
