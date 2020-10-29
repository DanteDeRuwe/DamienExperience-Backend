using DamianTourBackend.Core.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DamianTourBackend.Application.UpdateRoute
{
    public static class RouteMapper
    {
        public static void UpdateRoute(this RouteDTO model, ref Route route) 
        {
            route.TourName = model.TourName;
            route.DistanceInMeters = model.DistanceInMeters;
            route.Path = new Path { Coordinates = model.Coordinates};
        }

        public static Route MapToRoute(this RouteDTO model) =>
            new Route
            {
                TourName = model.TourName,
                DistanceInMeters= model.DistanceInMeters,
                Path = new Path { Coordinates = model.Coordinates }
            };
    }
}
