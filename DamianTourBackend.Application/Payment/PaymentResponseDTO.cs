namespace DamianTourBackend.Application.Payment
{
    public class PaymentResponseDTO
    {
        /*
            AAVADDRESS: "NO"
            ACCEPTANCE: "TEST"
            BRAND: "KBC Online"
            CARDNO: ""
            CN: ""
            ED: ""
            IP: "178.117.47.151"
            NCERROR: "0"
            PAYID: "3092731841"
            PM: "KBC Online"
            SHASIGN: "EE86434F942B07A27DB63F5068E1F8236DCA6DC5"
            STATUS: "9"
            TRXDATE: "12/10/20"
            amount: "65"
            currency: "EUR"
            orderID: "785a535b-9759-497e-b59c-c4e311a5da96"
        */
        public string Aavaddress { get; set; }
        public string Acceptance { get; set; }
        public string Amount { get; set; }

        public string Brand { get; set; }

        public string CardNo { get; set; }
        public string CN { get; set; }
        public string Currency { get; set; }

        public string ED { get; set; }

        public string IP { get; set; }

        public string NCError { get; set; }

        public string OrderID { get; set; }

        public string PayId { get; set; }
        public string PM { get; set; }

        public string ShaSign { get; set; }
        public string Status { get; set; }

        public string TRXDate { get; set; }


    }
}
