using DamianTourBackend.Application.Login;
using DamianTourBackend.Application.Register;
using DamianTourBackend.Application.UpdateProfile;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DamianTourBackend.Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IValidator<LoginDTO>, LoginValidator>();
            services.AddTransient<IValidator<RegisterDTO>, RegisterValidator>();
            services.AddTransient<IValidator<UpdateProfileDTO>, UpdateProfileValidator>();
        }
    }
}
