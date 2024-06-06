using EventPlanner.Models;
using EventPlanner.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Services
{
    public class EventService : IEventService
    {
        private readonly IDbContext _context;
        
        public EventService(IDbContext context)
        {
            _context = context;
        }
        public async Task<EventViewModel> GetEventForViewById(int id)
        {
            var result = await _context.Event
                .Include("Location").Include("Location.Street").Include("Location.Street.City").Include("Location.Street.City.Country")
                .FirstOrDefaultAsync(e => e.Id == id);
            if (result != null)
            {
                EventViewModel model = new();
                model.Id = result.Id;
                model.Name = result.Name;
                model.Category = null;

                var lookupCategory = await _context.Category.FirstOrDefaultAsync(c => c.Id == result.CategoryId);

                if (lookupCategory != null)
                {
                    model.Category = lookupCategory.Name;
                }

                // Address is deeply nested in relational db
                if (result.Location != null)
                {
                    model.PostalCode = result.Location.PostalCode;
                    model.BuidingNo = result.Location.BuildingNumber;
                    model.Street = null;
                    model.City = null;
                    model.Country = null;

                    if (result.Location.Street != null)
                    {
                        model.Street = result.Location.Street.Name;

                        if (result.Location.Street.City != null)
                        {
                            model.City = result.Location.Street.City.Name;

                            if (result.Location.Street.City.Country != null)
                            {
                                model.Country = result.Location.Street.City.Country.Name;
                            }
                        }
                    }
                }
                return model;
            }
            return null;
        }
    }
}
