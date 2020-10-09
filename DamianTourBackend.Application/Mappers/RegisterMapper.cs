using DamianTourBackend.Application.DTOs;
using DamianTourBackend.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace DamianTourBackend.Application.Mappers
{
    public static class RegisterMapper
    {
        public static User MapToUser(this RegisterDTO model) =>
            new User
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

        public static IdentityUser MapToIdentityUser(this RegisterDTO model) =>
            new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };
    }
}
