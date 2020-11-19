﻿using DamianTourBackend.Core.Entities;

namespace DamianTourBackend.Application.UpdateRoute
{
    public static class RouteMapper
    {
        public static void UpdateRoute(this RouteDTO model, ref Route route)
        {
            route.TourName = model.TourName;
            route.Date = model.Date;
            route.DistanceInMeters = model.DistanceInMeters;
            route.Path = new Path { LineColor = model.LineColor, Coordinates = model.Coordinates };
            route.Info = model.Info;
        }

        public static Route MapToRoute(this RouteDTO model) =>
            new Route
            {
                TourName = model.TourName,
                Date = model.Date,
                DistanceInMeters = model.DistanceInMeters,
                Path = new Path { LineColor = model.LineColor, Coordinates = model.Coordinates },
                Info = model.Info
            };

        public static RouteDTO MapToRouteDTO(this Route route) =>
            new RouteDTO
            {
                //TourId = route.Id,
                TourName = route.TourName,
                Date = route.Date,
                DistanceInMeters = route.DistanceInMeters,
                LineColor = route.Path.LineColor,
                Coordinates = route.Path.Coordinates,
                Info = route.Info
            };
    }
}
