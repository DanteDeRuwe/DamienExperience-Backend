using System;
using System.Collections;
using System.Collections.Generic;

namespace DamianTourBackend.Core.Entities
{
    public class Route
    {
        public Guid Id { get; set; }
        public string TourName { get; set; }
        public DateTime Date { get; set; }
        public int DistanceInMeters { get; set; }
        public Path Path { get; set; }
        public Dictionary<string, string> Info { get; set; }
        public ICollection<Waypoint> Waypoints { get; set; } 

        public Route() { }

        public Route(string tourName, DateTime date, int distanceInMeters, Path path, ICollection<Waypoint> waypoints = null)
        {
            TourName = tourName;
            Date = date;
            DistanceInMeters = distanceInMeters;
            Path = path;
            Info = new Dictionary<string, string>();
            Waypoints = waypoints;
        }
    }
}
