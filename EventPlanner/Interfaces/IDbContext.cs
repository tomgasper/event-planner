using EventPlanner.Models.Events;
using EventPlanner.Models.Location;
using EventPlanner.Models.Profile;
using EventPlanner.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Data.Common;

namespace EventPlanner.Interfaces
{
    public interface IDbContext
	{
		public DbSet<Event> Event { get; set; }
		public DbSet<Category> Category { get; set; }
		public DbSet<Country> Country { get; set; }
		public DbSet<City> City { get; set; }
		public DbSet<Street> Street { get; set; }
		public DbSet<Location> Location { get; set; }
		public DbSet<AppUser> Users { get; set; }
		public DbSet<EventType> EventType { get; set; }
		public DbSet<LoginHistory> LoginHistory { get; set; }

		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
		public void Add<T>(T entity) where T : class;
		public void Update<T>(T entity) where T : class;
		public void Remove<T>(T entity) where T : class;

		public EntityEntry Entry<T>(T entity) where T : class;
	}
}
