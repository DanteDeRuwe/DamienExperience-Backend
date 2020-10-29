using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

using System.Text;

namespace DamianTourBackend.Infrastructure.Data.Repositories
{
    class RouteRepository : IRouteRepository
    {
        public readonly IMongoCollection<Route> _routes;

        public RouteRepository(IMongoDatabase db) 
        {
            _routes = db.GetCollection<Route>("Routes");

        }

        public void Add(Route route)
        {
            _routes.InsertOne(route);
        }

        public void Delete(Route route)
        {
            _routes.FindOneAndDelete(r => r.Id.Equals(route.Id));
        }

        public IEnumerable<Route> GetAll()
        {
            return _routes.Find(r => true).ToList();
        }

        public Route GetById(Guid id)
        {
            return _routes.Find(r => r.Id.Equals(id)).FirstOrDefault();
        }

        public Route GetByName(string tourname)
        {
            return _routes.Find(r => r.TourName.Equals(tourname)).FirstOrDefault();
        }

        public void Update(Route route)
        {
            _routes.ReplaceOne(r => r.Id.Equals(route.Id), route);
        }
    }
}
