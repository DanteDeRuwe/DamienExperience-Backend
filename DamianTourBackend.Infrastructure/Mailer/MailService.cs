
using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SautinSoft.Document;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

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
    }
}