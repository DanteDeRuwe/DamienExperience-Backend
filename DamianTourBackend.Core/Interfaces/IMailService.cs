using DamianTourBackend.Core.Entities;
using System.Threading.Tasks;

namespace DamianTourBackend.Core.Interfaces
{
    public interface IMailService
    {
        void SendCertificate(CertificateDTO dto);
    }
}
