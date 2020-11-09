using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DamianTourBackend.Infrastructure.Data.Repositories
{
    public class WalkRepository : IWalkRepository
    {
        //public readonly IMongoCollection<Walk> _walks;
        public readonly IMongoCollection<User> _users;

        public WalkRepository(IMongoDatabase db)
        {
            //_walks = db.GetCollection<Walk>("Walks");
            _users = db.GetCollection<User>("Users");
        }

        public void Add(string mail, Walk walk)
        {
            _users.FindOneAndUpdateAsync(Builders<User>.Filter.Eq(u => u.Email, mail),
                Builders<User>.Update.Push(u => u.Walks, walk));
        }

        public void Delete(string mail, Walk walk)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Email, mail);
            var update = Builders<User>.Update.PullFilter(x => x.Walks,
                wlk => wlk.Id == walk.Id);
            _users.UpdateOne(filter, update);
        }

        public IEnumerable<Walk> GetAllWalksForRoute(Guid routeId)
        {
            return _users.AsQueryable().Where(u => u.Walks.Count >= 1)
               .SelectMany(p => p.Walks).Where(u => u.RouteID.Equals(routeId));
        }

        public IEnumerable<Walk> GetAllWalksUser(Guid userId)
        {
            return _users.AsQueryable().Where(u=>u.Id.Equals(userId)).Where(u => u.Walks.Count >= 1)
               .SelectMany(p => p.Walks);
        }

        public Walk GetByUserAndRoute(Guid userId, Guid routeId)
        {
            return _users.AsQueryable().Where(w => w.Id.Equals(userId)).SelectMany(u=>u.Walks).Where(w=>w.RouteID.Equals(routeId))
                .SingleOrDefault();
        }

        public Walk GetBy(Guid id)
        {
            return _users.AsQueryable().Where(u => u.Walks.Count >= 1).SelectMany(u=>u.Walks).Where(w=>w.Id.Equals(id)).SingleOrDefault();
        }

        public void Update(string mail ,Walk walk)
        {
            Delete(mail,walk);
            Add(mail, walk);
        }
    }
}
