using DamianTourBackend.Application.Login;

namespace DamianTourBackend.Application.Register
{
    public class RegisterDTO : LoginDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordConfirmation { get; set; }
    }
}
