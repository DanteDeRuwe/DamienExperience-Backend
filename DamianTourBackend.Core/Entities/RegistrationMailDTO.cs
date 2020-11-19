using System;
using System.Collections.Generic;
using System.Text;

namespace DamianTourBackend.Core.Entities
{
    public class RegistrationMailDTO
    {
        //email, firstname, lastname, tourname, distance, date
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Tourname { get; set; }
        public string Distance { get; set; }
        public string Date { get; set; }

        public RegistrationMailDTO()
        {

        }
    }
}
