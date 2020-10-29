using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace DamianTourBackend.Infrastructure.Data.Repositories
{
    public class WalkRepository : IWalkRepository
    {
        public readonly IMongoCollection<Walk> _walks;

        public WalkRepository(MongoDatabaseBase db)
        {
            _walks = db.GetCollection<Walk>("Walks");
        }

        public void Add(Walk walk)
        {
            _walks.InsertOne(walk);
        }

        public void Delete(Walk walk)
        {
            _walks.FindOneAndDelete(w => w.Id.Equals(walk.Id));
        }

        public IEnumerable<Walk> GetAllWalksForRoute(Guid routeId)
        {
            return _walks.Find(w => true).ToList();
        }

        public IEnumerable<Walk> GetAllWalksUser(Guid userId)
        {
            return _walks.Find(w => w.UserID.Equals(userId)).ToList();
        }

        public Walk GetByUserAndRoute(Guid userId, Guid routeId)
        {
            return _walks.Find(w => w.UserID.Equals(userId) && w.RouteID.Equals(routeId)).SingleOrDefault();
        }

        public Walk GetBy(Guid id)
        {
            return _walks.Find(w => w.Id.Equals(id)).SingleOrDefault();
        }

        public void Update(Walk walk)
        {
            _walks.ReplaceOne(w => w.Id.Equals(walk.Id), walk);
        }
    }
}
