namespace DamianTourBackend.Application.Payment
{
    public class RegistrationPaymentDTO
    {
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string Email { get; set; }
        public string Language { get; set; }
        public string OrderId { get; set; }
        public string PspId { get; set; }
        public string UserId { get; set; }
        public string ShaSign { get; set; }
        public string RouteName { get; set; }
        
        public RegistrationPaymentDTO() { }
    }
}
