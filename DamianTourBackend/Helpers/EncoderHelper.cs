using DamianTourBackend.Application.Payment;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;
using System.Text;

namespace DamianTourBackend.Api.Helpers
{
    public static class EncoderHelper
    {
        public static string CalculateNewShaSign(IConfiguration _config, string amount, string currency, string email, string language, string orderid, string pspid, string userid)
        {
            string hash;
            string key = _config["Payment:Key"];
            string input =
                "AMOUNT=" + amount + key +
                "CURRENCY=" + currency + key +
                "EMAIL=" + email + key +
                "LANGUAGE=" + language + key +
                "ORDERID=" + orderid + key +
                "PSPID=" + pspid + key +
                "USERID=" + userid + key;
            using (SHA1 sha1Hash = SHA1.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
            }
            return hash.ToLower();
        }

        public static bool ControlShaSign(IConfiguration _config, PaymentResponseDTO dto)
        {
            string hash;
            string key = _config["PaymentResponse:Key"];
            string input =
                //"AAVADDRESS=" + dto.Aavaddress + key +
                //"ACCEPTANCE=" + dto.Acceptance + key +
                "AMOUNT=" + dto.Amount + key +
                //"BRAND=" + dto.Brand + key +
                //"CARDNO=" + dto.CardNo + key +
                //"CN=" + dto.CN + key +
                "CURRENCY=" + dto.Currency + key +
                //"ED=" + dto.ED + key +
                //"IP=" + dto.IP + key +
                "NCERROR=" + dto.NCError + key +
                "ORDERID=" + dto.OrderID + key +
                "PAYID=" + dto.PayId + key +
                //"PM=" + dto.PM + key +
                "STATUS=" + dto.Status + key;//+
                                             //"TRXDATE=" + dto.TRXDate + key;

            using (SHA1 sha1Hash = SHA1.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty).ToLower();
            }

            return dto.ShaSign.ToLower().Equals(hash);
        }
    }
}
