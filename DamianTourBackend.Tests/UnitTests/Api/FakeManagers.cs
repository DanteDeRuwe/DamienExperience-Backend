using AspNetCore.Identity.Mongo.Model;
using DamianTourBackend.Application;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DamianTourBackend.Tests.UnitTests.Api
{
    public class FakeSignInManager : SignInManager<AppUser>
    {
        public FakeSignInManager()
            : base(
                new FakeUserManager(),
                Substitute.For<IHttpContextAccessor>(),
                Substitute.For<IUserClaimsPrincipalFactory<AppUser>>(),
                Substitute.For<IOptions<IdentityOptions>>(),
                Substitute.For<ILogger<SignInManager<AppUser>>>(),
                Substitute.For<IAuthenticationSchemeProvider>(),
                Substitute.For<IUserConfirmation<AppUser>>()
            )
        { }
    }

    public class FakeUserManager : UserManager<AppUser>
    {
        public FakeUserManager()
            : base(Substitute.For<IUserStore<AppUser>>(),
                Substitute.For<IOptions<IdentityOptions>>(),
                Substitute.For<IPasswordHasher<AppUser>>(),
                new IUserValidator<AppUser>[0],
                new IPasswordValidator<AppUser>[0],
                Substitute.For<ILookupNormalizer>(),
                Substitute.For<IdentityErrorDescriber>(),
                Substitute.For<IServiceProvider>(),
                Substitute.For<ILogger<UserManager<AppUser>>>())
        { }
    }

    public class FakeRoleManager : RoleManager<MongoRole>
    {
        public FakeRoleManager()
            : base(Substitute.For<IRoleStore<MongoRole>>(),
                  new List<IRoleValidator<MongoRole>>(),
                  Substitute.For<ILookupNormalizer>(),
                  Substitute.For<IdentityErrorDescriber>(),
                  Substitute.For<ILogger<RoleManager<MongoRole>>>()
                  )
        { }
    }
}