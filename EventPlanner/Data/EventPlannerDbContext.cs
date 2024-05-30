using System;
using System.Reflection.Emit;
using EventPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Data
{
    public class EventPlannerDbContext : IdentityDbContext<AppUser, AppUserRole, int>
    {
        public EventPlannerDbContext(DbContextOptions<EventPlannerDbContext> options) : base(options)
        {

        }

        public DbSet<Event> Events { get; set; }

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
