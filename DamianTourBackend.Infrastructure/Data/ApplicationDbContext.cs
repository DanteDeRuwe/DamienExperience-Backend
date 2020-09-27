﻿using System;
using System.Collections.Generic;
using System.Text;
using DamianTourBackend.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DamianTourBackend.Infrastructure.Data{
    public class ApplicationDbContext : IdentityDbContext {
        public DbSet<User> Users { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Walk> Walks { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Path> MyProperty { get; set; }
        //public DbSet<CoordinateTuple> CoordinateTuples { get; set; } 

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);
        }
    }
}
