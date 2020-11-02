using System;
using System.Collections.Generic;

namespace DamianTourBackend.Core.Entities
{
    public class Walk
    {
        public Guid Id { get; set; }
        //public Guid UserID { get; set; }
        public Guid RouteID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Path WalkedPath { get; set; }

        public Walk() { }

        public Walk(DateTime startTime, Route route)
        {
            Id = Guid.NewGuid();
            StartTime = startTime;
            WalkedPath = new Path();
            RouteID = route.Id;
            //UserID = user.Id;
        }

        public void SetCoords(List<double[]> coords) {
            WalkedPath.Coordinates = coords;
        }

        public void AddCoords(List<double[]> coords) {
            WalkedPath.Coordinates.AddRange(coords);
        }

    }
}
