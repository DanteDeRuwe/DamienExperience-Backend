﻿using DamianTourBackend.Core.Interfaces;
using DamianTourBackend.Infrastructure.Data;
using DamianTourBackend.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DamianTourBackend.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DamianToursContext")));

            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}