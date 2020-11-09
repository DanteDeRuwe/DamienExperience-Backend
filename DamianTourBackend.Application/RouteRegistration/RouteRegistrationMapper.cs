using DamianTourBackend.Core.Entities;
using System;

namespace DamianTourBackend.Application.RouteRegistration
{
    public static class RouteRegistrationMapper
    {
        public static Registration MapToRegistration(this RouteRegistrationDTO model, User user, Route route)
        {
            ShirtSize modelShirtSize = ShirtSize.GEEN;
            Enum.TryParse(model.ShirtSize, out modelShirtSize);

            return new Registration(
                timeStamp: DateTime.Now,
                route: route,
                user: user,
                orderedShirt: model.OrderedShirt,
                shirtSize: modelShirtSize
            );
        }

        public static RouteRegistrationDTO MapToRegistrationDTO(this Registration registration) =>
            new RouteRegistrationDTO {
                RouteId = registration.RouteId,
                OrderedShirt = registration.OrderedShirt,
                ShirtSize = registration.ShirtSize.ToString()
            };
    }
}