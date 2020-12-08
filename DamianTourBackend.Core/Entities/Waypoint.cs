using System;
using System.Collections.Generic;
using System.Text;

namespace DamianTourBackend.Core.Entities
{
    public class Waypoint
    {
        public Guid Id { get; set; }
        //public string Color { get; set; }


        //public string Title_NL { get; set; }
        //public string Title_FR { get; set; }
        //public string Description_NL { get; set; }
        //public string Description_FR { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public Dictionary<string, Dictionary<string, string>> LanguagesText { get; set; }

        public Waypoint() {
            Id = Guid.NewGuid();
        }

        public Waypoint(
            //string title_NL, string title_FR, string desc_NL, string desc_FR
            Dictionary<string, Dictionary<string, string>> texts
            , double longitude, double latitude)
        {
            Id = Guid.NewGuid();
            LanguagesText = texts;
            //Title_NL = title_NL;
            //Title_FR = title_FR;
            //Description_NL = desc_NL;
            //Description_FR = desc_FR;
            Longitude = longitude;
            Latitude = latitude;
        }
    }
}
