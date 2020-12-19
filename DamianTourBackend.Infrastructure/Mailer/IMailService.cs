using DamianTourBackend.Application.RouteRegistration;
using DamianTourBackend.Application.StopWalk;
using DamianTourBackend.Core.Entities;

namespace DamianTourBackend.Infrastructure.Mailer
{
    public interface IMailService
    {
        void SendCertificate(CertificateDTO dto);
        void SendRegistrationConfirmation(RegistrationMailDTO dto);
    }
}
