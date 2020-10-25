using System;
using System.Collections.Generic;
using System.Text;

namespace DamianTourBackend.Application.RouteRegistration
{
    public class RouteRegistrationDTO
    {
        public Guid RouteId { get; set; }
        public bool OrderedShirt { get; set; }
        public string Size { get; set; }
    }
}
