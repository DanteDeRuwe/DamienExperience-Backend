using AspNetCore.Identity.Mongo.Model;
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace DamianTourBackend.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        public readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<MongoUser> _mongoUsers;

        public UserRepository(IMongoDatabase db)
        {
            _users = db.GetCollection<User>("Users");
        }

        public void Add(User user)
        {
            _users.InsertOne(user);
        }

        public void Delete(User user)
        {
            _users.FindOneAndDelete(x => x.Id.Equals(user.Id));
        }

        public IEnumerable<User> GetAll()
        {
            return _users.Find(x => true).ToList();
        }

        public User GetBy(string email)
        {
            return _users.Find(user => user.Email.Equals(email)).FirstOrDefault();
        }

        public User GetBy(Guid id)
        {
            return _users.Find(user => user.Id.Equals(id)).FirstOrDefault();
        }

        public void Update(User user)
        {
            _users.ReplaceOne(u => u.Email.Equals(user.Email), user);
        }
    }
}
