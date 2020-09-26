using System;
using System.Collections.Generic;
using System.Text;

namespace DamianTourBackend.Core.Entities {
    public class User {
        public Guid Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public ICollection<Registration> Registration { get; set; }
        public ICollection<Walk> Walk { get; set; }

        public User() {

        }

        public User(string lastName, string firstName, string email, string phoneNumber) {
            LastName = lastName;
            FirstName = firstName;
            Email = email;
            PhoneNumber = phoneNumber;
            Registration = new List<Registration>();
            Walk = new List<Walk>();
        }
    }
}
