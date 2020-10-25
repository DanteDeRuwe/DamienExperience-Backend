using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace DamianTourBackend.Infrastructure.Data.Repositories
{
    public class RouteRepository : IRouteRepository
    {
        public readonly IMongoCollection<Route> _routes;

        public RouteRepository(IMongoDatabase db)
        {
            //_routes = db.GetCollection<Route>("Routes");
        }

        public void Add(Route route)
        {
            throw new NotImplementedException();
        }

        public void Delete(Route route)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Route> GetAll()
        {
            throw new NotImplementedException();
        }

        public Route GetBy(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Update(Route route)
        {
            throw new NotImplementedException();
        }
    }
}
