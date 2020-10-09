using DamianTourBackend.Application.DTOs;
using DamianTourBackend.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace DamianTourBackend.Application.Mappers
{
    public static class UpdateProfileMapper
    {
        public static void UpdateUser(this UpdateProfileDTO model, ref User user)
        {
            user.Email = model.Email ?? user.Email;
            user.FirstName = model.FirstName ?? model.FirstName;
            user.LastName = model.LastName ?? user.LastName;
        }

        public static void UpdateIdentityUser(this UpdateProfileDTO model, ref IdentityUser identityUser)
        {
            identityUser.Email = model.Email ?? identityUser.Email;
            identityUser.UserName = model.Email ?? identityUser.UserName;
        }
    }
}