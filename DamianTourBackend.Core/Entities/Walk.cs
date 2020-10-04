using System;
using System.Collections.Generic;
using System.Text;

namespace DamianTourBackend.Core.Entities {
    public class Walk {

        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Path WalkedPath { get; set; }
        public Route Route { get; set; }

        public Walk() {

        }

        public Walk(DateTime startTime, Route route) {
            StartTime = startTime;
            WalkedPath = new Path();
            Route = route;
        }
    }
}
