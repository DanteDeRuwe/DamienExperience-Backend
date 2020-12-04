using DamianTourBackend.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DamianTourBackend.Api.Helpers
{
    public static class AdminHelper
    {
        public static bool IsAdmin(this AppUser user) => user.Claims.Any(c => c.ClaimValue.Equals("admin"));
    }
}
