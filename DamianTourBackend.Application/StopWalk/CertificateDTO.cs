using System;
using System.Collections.Generic;
using System.Text;

namespace DamianTourBackend.Core.Entities
{
    public class CertificateDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Distance { get; set; }
        public string Date { get; set; }

        public CertificateDTO()
        {

        }
    }
}
