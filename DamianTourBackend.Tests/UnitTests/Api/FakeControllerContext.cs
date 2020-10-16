using DamianTourBackend.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;

namespace DamianTourBackend.Tests.UnitTests.Api
{
    public static class FakeControllerContext
    {
        public static readonly ControllerContext NotLoggedIn =
            new ControllerContext { HttpContext = new DefaultHttpContext { User = null } };

        public static ControllerContext For(User user)
        {
            var identity = new GenericIdentity(user.Email);
            var context = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
            return new ControllerContext { HttpContext = context };
        }
    }
}
