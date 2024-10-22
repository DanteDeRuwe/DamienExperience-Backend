﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DamianTourBackend.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string DateOfBirth { get; set; }
        public ICollection<Walk> Walks { get; set; }
        public ICollection<Registration> Registrations { get; set; }
        public ICollection<string> Friends { get; set; }
        public Privacy Privacy { get; set; }

        public User()
        {
            Registrations = new List<Registration>();
            Walks = new List<Walk>();
            Friends = new List<string>();
            Privacy = Privacy.PRIVATE;
        }

        public User(string lastName, string firstName, string email, string phoneNumber)
        {
            LastName = lastName;
            FirstName = firstName;
            Email = email;
            PhoneNumber = phoneNumber;
            Registrations = new List<Registration>();
            Walks = new List<Walk>();
            Friends = new List<string>();
            Privacy = Privacy.PRIVATE;
        }

        public bool IsFriend(string email)
        {
            return Friends.Any(friend => friend.ToLower().Equals(email.ToLower()));
        }
    }
}
