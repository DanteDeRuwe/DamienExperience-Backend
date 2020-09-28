using System;

namespace DamianTourBackend.Core.Entities
{
    public class Route
    {
        public Guid Id { get; set; }
        public string TourName { get; set; }
        public int DistanceInMeters { get; set; }
        public Path Path { get; set; }

        public Route() { }

        public Route(string tourName, int distanceInMeters, Path path)
        {
            TourName = tourName;
            DistanceInMeters = distanceInMeters;
            Path = path;
        }
    }
}
