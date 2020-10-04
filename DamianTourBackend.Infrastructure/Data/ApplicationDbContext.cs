using DamianTourBackend.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DamianTourBackend.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<User> Users { get; set; }
        //public DbSet<Route> Routes { get; set; }
        //public DbSet<Walk> Walks { get; set; }
        //public DbSet<Registration> Registrations { get; set; }
        //public DbSet<Path> Paths { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // finds all classes that implement IEntityTypeConfiguration and applies their configurations
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUser>().ToTable("IdentityUsers");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        }
    }
}
