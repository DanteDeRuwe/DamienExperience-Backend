using System;
using System.Collections.Generic;
using System.Text;

namespace DamianTourBackend.Application.UpdateWaypoint
{
    public class WaypointDTO
    {
        public Guid Id { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public Dictionary<string, Dictionary<string, string>> LanguagesText { get; set; }
    }
}
