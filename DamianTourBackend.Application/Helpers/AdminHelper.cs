using System.Linq;

namespace DamianTourBackend.Application.Helpers
{
    public static class AdminHelper
    {
        public static bool IsAdmin(this AppUser user) => user.Claims.Any(c => c.ClaimValue.Equals("admin"));
    }
}
