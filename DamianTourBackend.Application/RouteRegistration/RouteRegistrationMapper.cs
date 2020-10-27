using System;
using DamianTourBackend.Core.Entities;

namespace DamianTourBackend.Application.RouteRegistration
{
    public static class RouteRegistrationMapper
    {
        public static Registration MapToRegistration(this RouteRegistrationDTO model, User user, Route route) =>
            new Registration(
                timeStamp: DateTime.Now,
                route: route,
                user: user,
                orderedShirt: model.OrderedShirt,
                sizeShirt: model.Size
            );
    }
}