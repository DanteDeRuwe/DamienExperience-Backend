using System;

namespace DamianTourBackend.Core.Entities
{
    public class Route
    {
        public Guid Id { get; set; }
        public string TourName { get; set; }
        public DateTime Date { get; set; }
        public int DistanceInMeters { get; set; }
        public Path Path { get; set; }

        public Route() { }

        public Route(string tourName, DateTime date, int distanceInMeters, Path path)
        {
            TourName = tourName;
            Date = date;
            DistanceInMeters = distanceInMeters;
            Path = path;
        }
    }
}
