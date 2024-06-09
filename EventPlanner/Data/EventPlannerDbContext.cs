using System;
using System.Reflection.Emit;
using EventPlanner.Models;
using EventPlanner.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Data
{
    public class EventPlannerDbContext : IdentityDbContext<AppUser, AppUserRole, int>, IDbContext
    {
        public EventPlannerDbContext(DbContextOptions<EventPlannerDbContext> options) : base(options)
        {
            
        }

        // Adhering to IDbContext Interface

        public DbSet<Event> Event { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Street> Street { get; set; }
        public DbSet<Location> Location { get; set; }

		public int SaveChanges()
        {
            return base.SaveChanges();
        }

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        /*
        public Task<T> FirstOrDefaultAsync<T>(IQueryable<T> source)
        {
            return base.FirstOrDefaultAsync(source);
        }
        */


        public void Add<T>(T entity) where T : class
		{
			base.Add(entity);
		}

		public void Update<T>(T entity) where T : class
		{
			base.Update(entity);
		}

		public void Remove<T>(T entity) where T : class
		{
			base.Remove(entity);
		}

		protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>(b =>
            {
                b.ToTable("AppUsers");
            });

            /*
            Ignore some of the template columns in AppUser table
            builder.Entity<AppUser>(b =>
            {
                b.Ignore(c => c.AccessFailedCount);
                b.Ignore(c => c.LockoutEnabled);
                b.Ignore(c => c.TwoFactorEnabled);
            });
            */

            builder.Entity<AppUserRole>(b =>
            {
                b.ToTable("AppUsersRoles");
            });

            builder.Entity<IdentityUserClaim<int>>(b =>
            {
                b.ToTable("AppUserClaims");
            });
        }
    }
}
