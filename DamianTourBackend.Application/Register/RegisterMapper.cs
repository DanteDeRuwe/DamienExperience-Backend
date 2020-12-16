using DamianTourBackend.Application.Helpers;
using DamianTourBackend.Core.Entities;

namespace DamianTourBackend.Application.Register
{
    public static class RegisterMapper
    {
        public static User MapToUser(this RegisterDTO model) =>
            new User
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = DateParser.Parse(model.DateOfBirth)
            };

        public static AppUser MapToAppUser(this RegisterDTO model) =>
            new AppUser()
            {
                UserName = model.Email,
                Email = model.Email
            };
    }
}
