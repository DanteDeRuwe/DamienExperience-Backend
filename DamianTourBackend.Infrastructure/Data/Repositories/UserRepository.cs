using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DamianTourBackend.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        /*private readonly ApplicationDbContext _context;
        private readonly DbSet<User> _users;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
            _users = context.Users;
        }

        public void Add(User user)
        {
            _users.Add(user);
        }

        public IEnumerable<User> GetAll()
        {
            return _users//.Include(u => u.Registrations)
                         //.Include(u => u.Walks)
                         .ToList();
        }

        public User GetBy(string email)
        {
            return GetAll().Where(u => u.Email.Equals(email)).FirstOrDefault();
        }

        public User GetBy(Guid id)
        {
            return GetAll().Where(u => u.Id.Equals(id)).FirstOrDefault();
        }

        public void Update(User user)
        {
            _context.Update(user);
        }

        public void Delete(User user)
        {
            _users.Remove(user);
        }

        public void SaveChanges() => _context.SaveChanges();*/
        public void Add(User user)
        {
            throw new NotImplementedException();
        }

        public void Delete(User user)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public User GetBy(string email)
        {
            throw new NotImplementedException();
        }

        public User GetBy(Guid id)
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public void Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}
