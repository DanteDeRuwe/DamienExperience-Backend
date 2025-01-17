﻿using System;

namespace DamianTourBackend.Core.Entities
{
    public class Registration
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool Paid { get; set; }
        public Guid RouteId { get; set; }
        public bool OrderedShirt { get; set; }
        public ShirtSize ShirtSize { get; set; }
        public Privacy Privacy { get; set; }

        public Registration() { }

        public Registration(DateTime timeStamp, Route route, User user, bool orderedShirt, ShirtSize shirtSize, Privacy privacy, bool paid = false)
        {
            Id = Guid.NewGuid();
            TimeStamp = timeStamp;
            RouteId = route.Id;
            OrderedShirt = orderedShirt;
            ShirtSize = shirtSize;
            Privacy = privacy;
            user.Registrations.Add(this);
            Paid = paid;
        }
    }
}
