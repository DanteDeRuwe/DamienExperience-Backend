using DamianTourBackend.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DamianTourBackend.Core.Interfaces
{
    public interface IMailService
    {
        Task SendMailWithCertificate(User user);
        void SendGridMailer(User user);
    }
}
