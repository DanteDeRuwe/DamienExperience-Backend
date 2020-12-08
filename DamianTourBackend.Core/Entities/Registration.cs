using System;

namespace DamianTourBackend.Core.Entities
{
    public class Registration
    {

        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool Paid { get; set; }
        //public Route Route { get; set; }
        public Guid RouteId { get; set; }
        //public User User { get; set; }
        public bool OrderedShirt { get; set; }
        public ShirtSize ShirtSize { get; set; }
        public string SizeShirt { get; set; } //DELETE THIS after db reset

        public Registration() { }

        public Registration(DateTime timeStamp, Route route, User user, bool orderedShirt, ShirtSize shirtSize, bool paid = false)
        {
            Id = Guid.NewGuid();
            TimeStamp = timeStamp;
            //Route = route;
            RouteId = route.Id;
            //User = user;
            OrderedShirt = orderedShirt;
            ShirtSize = shirtSize;
            user.Registrations.Add(this);
            Paid = paid;
        }
    }
}
