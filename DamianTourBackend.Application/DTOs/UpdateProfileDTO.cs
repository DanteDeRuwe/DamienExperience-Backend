using System;
using System.Collections.Generic;
using System.Text;

namespace DamianTourBackend.Application.DTOs
{
    public class UpdateProfileDTO
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public string Wachtwoord { get; set; }        Wachtwoord kan momenteel nog niet veranderd worden (nog te bespreken)
        //public string Adres { get; set; }
    }
}
