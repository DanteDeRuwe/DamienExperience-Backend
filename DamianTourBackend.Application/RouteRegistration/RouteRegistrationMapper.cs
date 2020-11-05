using DamianTourBackend.Core.Entities;
using System;

namespace DamianTourBackend.Application.RouteRegistration
{
    public static class RouteRegistrationMapper
    {
        public static Registration MapToRegistration(this RouteRegistrationDTO model, User user, Route route)
        {
            ShirtSize shirtSizeModel = ShirtSize.GEEN;
            foreach (ShirtSize size in Enum.GetValues(typeof(ShirtSize)))
                if (model.ShirtSize.ToLower().Equals(size.ToString().ToLower()))
                    shirtSizeModel = size;

            return new Registration(
                timeStamp: DateTime.Now,
                route: route,
                user: user,
                orderedShirt: model.OrderedShirt,
                shirtSize: shirtSizeModel
            );
        }

    }
}