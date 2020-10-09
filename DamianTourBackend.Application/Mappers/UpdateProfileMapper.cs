using DamianTourBackend.Application.DTOs;
using DamianTourBackend.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace DamianTourBackend.Application.Mappers
{
    public static class UpdateProfileMapper
    {
        public static void UpdateUser(this UpdateProfileDTO model, ref User user)
        {
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
        }

        public static void UpdateIdentityUser(this UpdateProfileDTO model, ref IdentityUser identityUser)
        {
            identityUser.Email = model.Email;
            identityUser.UserName = model.Email;
        }
    }
}