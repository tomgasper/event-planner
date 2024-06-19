using EventPlanner.Models;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EventPlanner.Interfaces;
using EventPlanner.Services;

using FluentAssertions;

namespace EventPlanner.Tests.Service
{
    public class EventsServiceTest
    {
        private readonly IDbContext _context;
        private readonly IEventsService _eventsService;
        public EventsServiceTest() {
            _context = Substitute.For<IDbContext>();

            _eventsService = new EventsService(_context);
        }

        [Fact]
        public async Task GetEventsForPageAsync_ReturnsEventList()
        {
            // Arrange
            var eventList = new List<Event>
            {
                new Event { Id = 1,Name = "Event1" },
                new Event { Id = 2,Name = "Event2" },
                new Event { Id = 3,Name = "Event3" },
        };

            var mockDbSet = eventList.AsQueryable().BuildMockDbSet();
            _context.Event.Returns(mockDbSet);

            // Act
            var result = await _eventsService.GetEventsRangeAsync();

            // Assert
            result.Should().BeOfType<List<Event>>();
            result.Should().HaveCount(c => c == 3);
        }
    }
}
