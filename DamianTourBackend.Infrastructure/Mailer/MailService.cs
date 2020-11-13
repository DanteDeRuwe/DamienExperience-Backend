
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
            //{BaseAddress = new Uri("http://localhost:3000/")};
        }

        public void SendCertificate(CertificateDTO dto)
        {
            /*var stringContent = new JsonContent(dto);
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri("http://localhost:3000/mailcertificate"));
            message.Content = stringContent;
            _client.SendAsync(message);*/

            //https://stackoverflow.com/questions/39134985/how-to-properly-perform-http-post-of-a-json-object-using-c-sharp-httpclient

            /*var JSON = JsonConvert.SerializeObject(dto);
            var stringContent = new StringContent(JSON, Encoding.UTF8, "application/x-www-form-urlencoded");*/

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

            /////*
           /* var httpWebRequest = (HttpWebRequest) WebRequest.Create("http://localhost:3000/mailcertificate");
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(dto);

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }*/

        }

        public async Task SendCertificateAsync(CertificateDTO dto) 
        {
            throw new NotImplementedException();
        }
    }


    public class JsonContent : StringContent
    {
        public JsonContent(object obj) : base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/x-www-form-urlencoded") { }
    }
}