using DamianTourBackend.Core.Entities;

namespace DamianTourBackend.Application.RouteRegistration
{
    public static class RegistrationMailMapper
    {
        public static RegistrationMailDTO MapToDTO(this (User, Route) userRouteTuple)
        {
            var (user, route) = userRouteTuple;
            return new RegistrationMailDTO {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Tourname = route.TourName,
                Distance = (route.DistanceInMeters / 1000).ToString(),
                Date = route.Date.ToString()
            };
        }
    }
}