using DamianTourBackend.Core.Entities;

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
        }

        public static Route MapToRoute(this RouteDTO model) =>
            new Route
            {
                TourName = model.TourName,
                Date = model.Date,
                DistanceInMeters = model.DistanceInMeters,
                Path = new Path { LineColor = model.LineColor, Coordinates = model.Coordinates }
            };

        public static RouteDTO MapToRouteDTO(this Route route) =>
            new RouteDTO
            {
                TourName = route.TourName,
                Date = route.Date,
                DistanceInMeters = route.DistanceInMeters,
                LineColor = route.Path.LineColor,
                Coordinates = route.Path.Coordinates
            };
    }
}
