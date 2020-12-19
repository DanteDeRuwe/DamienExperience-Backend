using DamianTourBackend.Core.Entities;
using System.Collections.Generic;
using System.Net.Http;
using DamianTourBackend.Application.RouteRegistration;
using DamianTourBackend.Application.StopWalk;

namespace DamianTourBackend.Infrastructure.Mailer
{
    //mss internal?
    public class MailService : IMailService
    {
        public readonly HttpClient _client;

        public MailService()
        {
            _client = new HttpClient();
        }

        public void SendCertificate(CertificateDTO dto)
        {
            HttpContent content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"id", dto.Id},
                    {"firstname", dto.FirstName},
                    {"lastname", dto.LastName},
                    {"email", dto.Email},
                    {"distance", dto.Distance},
                    {"date", dto.Date}
                }
            );

            _client.PostAsync("http://localhost:3000/mailcertificate", content);
        }

        public void SendRegistrationConfirmation(RegistrationMailDTO dto) {
            //email, firstname, lastname, tourname, distance, date
            HttpContent content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"email", dto.Email},
                    {"firstname", dto.FirstName},
                    {"lastname", dto.LastName},
                    {"tourname", dto.Tourname },
                    {"distance", dto.Distance},
                    {"date", dto.Date}
                }
           );

            _client.PostAsync("http://localhost:3000/mailregistration", content);
        }
    }
}