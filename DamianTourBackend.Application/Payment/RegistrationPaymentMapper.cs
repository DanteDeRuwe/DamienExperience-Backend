using DamianTourBackend.Application.Helpers;
using DamianTourBackend.Core.Entities;
using Microsoft.Extensions.Configuration;

namespace DamianTourBackend.Application.Payment
{
    public class RegistrationPaymentMapper
    {
        public static RegistrationPaymentDTO DTOFrom(User user, Route route, Registration registration, string language, IConfiguration config)
        {
            string amount = registration.OrderedShirt ? "6500" : "5000";
            string shasign = EncoderHelper.CalculateNewShaSign(config, amount, "EUR", user.Email, language, registration.Id.ToString(), "damiaanactie", user.Id.ToString());

            return new RegistrationPaymentDTO()
            {
                Amount = amount,
                Currency = "EUR",
                Email = user.Email,
                Language = language,
                OrderId = registration.Id.ToString(),
                PspId = "damiaanactie",
                UserId = user.Id.ToString(),
                ShaSign = shasign,
                RouteName = route.TourName
            };
        }
    }
}