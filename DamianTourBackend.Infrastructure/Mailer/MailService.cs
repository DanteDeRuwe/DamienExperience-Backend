using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using SautinSoft.Document;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace DamianTourBackend.Infrastructure.Mailer
{
    //mss internal?
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;

        public SmtpClient Client { get; set; }

        public MailService()
        {
        }

        public MailService(IConfiguration configuration)
        {
            //Gets mail Sendgrid apiKey
            _configuration = configuration;
            string apiKey = _configuration["SendGrid:Key"];
            Client = new SmtpClient("smtp.sendgrid.net", 587)
            {
                Credentials = new System.Net.NetworkCredential("apikey", apiKey)
            };

        }




        public async Task SendMailWithCertificate(User user)
        {

            //Makes pdf
            DocumentCore dc = new DocumentCore();
            dc.Content.End.Insert("Hello", new CharacterFormat()
            {
                FontName = "Verdana",
                Size = 65.5f,
                FontColor = Color.Black
            });

            dc.Save("certificate.pdf");

            //Makes mail and attaches pdf
            System.Net.Mail.Attachment certificate = new System.Net.Mail.Attachment("certificate.pdf", MediaTypeNames.Application.Octet);
            string to = user.Email;
            string from = "jordy.vankerkvoorde@student.hogent.be";
            MailMessage message = new MailMessage(from, to);
            message.Subject = "Certificaat Damien Experience";
            message.Body = Text(user);
            message.Attachments.Add(certificate);

            //tries to send mail async
            try
            {
                await Task.Run(() => Client.Send(message));

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTestMessage,",
                    ex.ToString());
            }
            //delete mail
            certificate.Dispose();
            if (File.Exists("certificate.pdf"))
            {
                File.Delete("certificate.pdf");
            }
        }

        public string Text(User user) 
        {
            return @$"<!DOCTYPE html>
                        <html>
                        <head></head>
                            <body>
                                <h1>MyName is {user.FirstName}</h1>
                            </body>
                        </html>";
/*
            System.IO.StreamWriter file = new System.IO.StreamWriter("");
            file.WriteLine(lines);

            file.Close();

            return File.ReadAllText("");*/
        }


        public void SendGridMailer(User user) 
        {
            var sendGridClient = new SendGridClient(_configuration["SendGrid:Key"]);

            var sendGridMessage = new SendGridMessage();
            sendGridMessage.SetFrom("jordy.vankerkvoorde@student.hogent.be", "Damien Experience");
            sendGridMessage.AddTo("jordy.vankerkvoorde@student.hogent.be", "FirstName LastName");
            /*sendGridMessage.Subject = "success sendgrid";
            sendGridMessage.HtmlContent = "<strong>and easy to do anywhere, even with C#</strong>";*/

            sendGridMessage.SetTemplateId("d-cb844c22ba3e444fb0be079a8196acff");
            sendGridMessage.SetTemplateData(user);

            var response = sendGridClient.SendEmailAsync(sendGridMessage).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                Console.WriteLine("Email sent");
            }
        }
    }
}
