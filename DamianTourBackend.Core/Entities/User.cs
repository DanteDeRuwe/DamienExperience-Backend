﻿using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace DamianTourBackend.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        //public ICollection<Registration> Registrations { get; set; }
        //public ICollection<Walk> Walks { get; set; }

        public User() { }


        public User(string lastName, string firstName, string email, string phoneNumber)
        {
            LastName = lastName;
            FirstName = firstName;
            Email = email;
            PhoneNumber = phoneNumber;
            //Registrations = new List<Registration>();
            //Walks = new List<Walk>();
        }
    }
}
