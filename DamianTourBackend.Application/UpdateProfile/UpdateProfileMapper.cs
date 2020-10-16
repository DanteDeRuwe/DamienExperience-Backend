using DamianTourBackend.Core.Entities;

namespace DamianTourBackend.Application.UpdateProfile
{
    public static class UpdateProfileMapper
    {
        public static void UpdateUser(this UpdateProfileDTO model, ref User user)
        {
            user.Email = model.Email ?? user.Email;
            user.FirstName = model.FirstName ?? model.FirstName;
            user.LastName = model.LastName ?? user.LastName;
        }

        public static void UpdateIdentityUser(this UpdateProfileDTO model, ref AppUser appUser)
        {
            appUser.Email = model.Email ?? appUser.Email;
            appUser.UserName = model.Email ?? appUser.UserName;
        }
    }
}