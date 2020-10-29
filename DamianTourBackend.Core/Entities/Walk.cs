using System;

namespace DamianTourBackend.Core.Entities
{
    public class Walk
    {
        public Guid Id { get; set; }
        public Guid UserID { get; set; }
        public Guid RouteID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Path WalkedPath { get; set; }

        public Walk() { }

        public Walk(DateTime startTime, Route route, User user)
        {
            StartTime = startTime;
            WalkedPath = new Path();
            RouteID = route.Id;
            UserID = user.Id;
        }
    }
}
