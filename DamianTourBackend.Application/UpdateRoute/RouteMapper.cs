using DamianTourBackend.Core.Entities;

namespace DamianTourBackend.Application.UpdateRoute
{
    public static class RouteMapper
    {
        public static void UpdateRoute(this RouteDTO model, ref Route route)
        {
            route.TourName = model.TourName;
            route.DistanceInMeters = model.DistanceInMeters;
            route.Path = new Path { LineColor = model.LineColor, Coordinates = model.Coordinates };
        }

        public static Route MapToRoute(this RouteDTO model) =>
            new Route
            {
                TourName = model.TourName,
                DistanceInMeters = model.DistanceInMeters,
                Path = new Path { LineColor = model.LineColor, Coordinates = model.Coordinates }
            };

        public static RouteDTO MapToRouteDTO(this Route route) =>
            new RouteDTO
            {
                TourName = route.TourName,
                DistanceInMeters = route.DistanceInMeters,
                LineColor = route.Path.LineColor,
                Coordinates = route.Path.Coordinates
            };
    }
}
