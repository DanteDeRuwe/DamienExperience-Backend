using DamianTourBackend.Core.Entities;
using DamianTourBackend.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using SautinSoft.Document;
using SendGrid;
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
    public class MailService
    {
        private readonly IConfiguration _configuration;

        public SmtpClient Client { get; set; }

        public MailService()
        {
        }

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
            string apiKey = _configuration["SendGrid:Key"];
            Client = new SmtpClient("smtp.sendgrid.net", 587)
            {
                Credentials = new System.Net.NetworkCredential("apikey", apiKey)
            };

        }

        public void CreateTestMessage()
        {
            string to = "jordy.vankerkvoorde@student.hogent.be";
            string from = "jordy.vankerkvoorde@student.hogent.be";
            MailMessage message = new MailMessage(from, to);
            message.Subject = "Using the new SMTP client.";
            message.Body = @"Using this new feature, you can send an email message from an application very easily.";
            
            try
            {
                Client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTestMessage,",
                    ex.ToString());
            }
        }




        public async Task CreateMail()
        {

            DocumentCore dc = new DocumentCore();
            dc.Content.End.Insert("Hello", new CharacterFormat()
            {
                FontName = "Verdana",
                Size = 65.5f,
                FontColor = Color.Black
            });

            dc.Save("certificate.pdf");
            
            Attachment certificate = new Attachment("certificate.pdf", MediaTypeNames.Application.Octet);
            string to = "jordy.vankerkvoorde@student.hogent.be";
            string from = "jordy.vankerkvoorde@student.hogent.be";
            MailMessage message = new MailMessage(from, to);
            message.Subject = "Using the new SMTP client.";
            message.Body = @"Using this new feature, you can send an email message from an application very easily.";
            message.Attachments.Add(certificate);

            try
            {
                await Task.Run(() => Client.Send(message));
               
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTestMessage,",
                    ex.ToString());
            }
            certificate.Dispose();
            if (File.Exists("certificate.pdf"))
            {
                File.Delete("certificate.pdf");
            }
        }


    }
}
