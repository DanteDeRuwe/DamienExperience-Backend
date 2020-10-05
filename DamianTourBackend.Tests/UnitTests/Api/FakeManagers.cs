using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Threading.Tasks;

namespace DamianTourBackend.Tests.UnitTests.Api
{
    public class FakeSignInManager : SignInManager<IdentityUser>
    {
        public FakeSignInManager()
            : base(
                new FakeUserManager(),
                Substitute.For<IHttpContextAccessor>(),
                Substitute.For<IUserClaimsPrincipalFactory<IdentityUser>>(),
                Substitute.For<IOptions<IdentityOptions>>(),
                Substitute.For<ILogger<SignInManager<IdentityUser>>>(),
                Substitute.For<IAuthenticationSchemeProvider>(),
                Substitute.For<IUserConfirmation<IdentityUser>>()
            )
        { }
        public override Task<SignInResult> CheckPasswordSignInAsync(IdentityUser user, string password, bool lockoutOnFailure)
        {
            return Task.FromResult(SignInResult.Success);
        }
    }

    public class FakeUserManager : UserManager<IdentityUser>
    {
        public FakeUserManager()
            : base(Substitute.For<IUserStore<IdentityUser>>(),
                Substitute.For<IOptions<IdentityOptions>>(),
                Substitute.For<IPasswordHasher<IdentityUser>>(),
                new IUserValidator<IdentityUser>[0],
                new IPasswordValidator<IdentityUser>[0],
                Substitute.For<ILookupNormalizer>(),
                Substitute.For<IdentityErrorDescriber>(),
                Substitute.For<IServiceProvider>(),
                Substitute.For<ILogger<UserManager<IdentityUser>>>())
        { }

        public override Task<IdentityResult> CreateAsync(IdentityUser user, string password)
        {

            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<IdentityResult> AddToRoleAsync(IdentityUser user, string role)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<string> GenerateEmailConfirmationTokenAsync(IdentityUser user)
        {
            return Task.FromResult(Guid.NewGuid().ToString());
        }


    }
}