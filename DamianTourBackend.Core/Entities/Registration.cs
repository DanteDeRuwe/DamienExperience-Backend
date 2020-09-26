﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DamianTourBackend.Core.Entities {
    public class Registration {

        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        // public bool Paid { get; set; } vragen aan klant
        public Route Route { get; set; }
        public User User { get; set; }

        public Registration() {

        }

        public Registration(DateTime timeStamp, Route route, User user) {
            TimeStamp = timeStamp;
            Route = route;
            User = user;
        }
    }
}