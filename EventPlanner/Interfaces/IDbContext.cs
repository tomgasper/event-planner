using EventPlanner.Models;
using Microsoft.EntityFrameworkCore;

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

		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
		int SaveChanges();
		public void Add<T>(T entity) where T : class;
		public void Update<T>(T entity) where T : class;
		public void Remove<T>(T entity) where T : class;
	}
}
