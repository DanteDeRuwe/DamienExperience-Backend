namespace DamianTourBackend.Application.DTOs
{
    public class RegisterDTO : LoginDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordConfirmation { get; set; }
    }
}
