using DamianTourBackend.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DamianTourBackend.Core.Interfaces
{
    public interface IMailService
    {
        void MailCertificate(User user);
        void CreateTestMessage();
    }
}
