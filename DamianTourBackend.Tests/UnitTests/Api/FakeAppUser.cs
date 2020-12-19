using System.Collections.Generic;
using System.Linq;
using DamianTourBackend.Application;
using DamianTourBackend.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace DamianTourBackend.Tests.UnitTests.Api
{
    public static class FakeAppUser
    {
        public static AppUser For(User user)
        {
            return new AppUser() {UserName = user.Email, Email = user.Email};
        }
        
        public static AppUser WithClaims(this AppUser appUser, params string[] claimvalues)
        {
            var claims = new List<IdentityUserClaim<string>>();
            claims.AddRange(claimvalues.Select(c=>new IdentityUserClaim<string>(){ClaimValue = c}));
            appUser.Claims = claims;
            return appUser;
        }
    }
}