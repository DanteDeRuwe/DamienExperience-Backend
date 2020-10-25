using System;

namespace DamianTourBackend.Core.Entities
{
    public class Registration
    {

        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        // public bool Paid { get; set; } vragen aan klant
        public Route Route { get; set; }
        public User User { get; set; }
        public bool OrderedShirt { get; set; }

        public Registration() { }

        public Registration(DateTime timeStamp, Route route, User user, bool orderedShirt)
        {
            TimeStamp = timeStamp;
            Route = route;
            User = user;
            OrderedShirt = orderedShirt;
            user.Registrations.Add(this); // ?
        }
    }
}
