using System;
using System.Collections.Generic;

namespace DamianTourBackend.Core.Entities
{
    public class Waypoint
    {
        public Guid Id { get; set; }
        //public string Color { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public Dictionary<string, Dictionary<string, string>> LanguagesText { get; set; }

        public Waypoint() {
            Id = Guid.NewGuid();
        }

        public Waypoint(
            Dictionary<string, Dictionary<string, string>> texts
            , double longitude, double latitude)
        {
            Id = Guid.NewGuid();
            LanguagesText = texts;
            Longitude = longitude;
            Latitude = latitude;
        }
    }
}
