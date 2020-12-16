using DamianTourBackend.Core.Entities;

namespace DamianTourBackend.Application.UpdateProfile
{
    public static class UpdateProfileMapper
    {
        public static void UpdateUser(this UpdateProfileDTO model, ref User user)
        {
            var privacy = (Privacy) model.Privacy;

            user.Email = model.Email ?? user.Email;
            user.FirstName = model.FirstName ?? model.FirstName;
            user.LastName = model.LastName ?? user.LastName;
            user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;
            user.DateOfBirth = model.DateOfBirth != null ? DateParser.Parse(model.DateOfBirth) : user.DateOfBirth;
            user.Friends = model.Friends ?? user.Friends;
            user.Privacy = privacy != user.Privacy ? privacy : user.Privacy;
        }

        public static void UpdateIdentityUser(this UpdateProfileDTO model, ref AppUser appUser)
        {
            appUser.Email = model.Email ?? appUser.Email;
            appUser.UserName = model.Email ?? appUser.UserName;
        }

        public static User MapToUser(this UpdateProfileDTO model)
        {
            var privacy = (Privacy) model.Privacy;
            return new User
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = DateParser.Parse(model.DateOfBirth),
                Friends = model.Friends,
                Privacy = privacy
            };
        }
    }
}