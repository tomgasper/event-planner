using EventPlanner.Data;
using EventPlanner.Models;
using EventPlanner.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventPlanner.Tests.Helper;
using EventPlanner.Interfaces;
using MockQueryable;
using MockQueryable.NSubstitute;

namespace EventPlanner.Tests.Service
{
	public class EventServiceTest
	{
		private readonly IDbContext _context;
		private readonly IEventService _eventService;

		public EventServiceTest()
		{
			// Mock db context via interface
			_context = Substitute.For<IDbContext>();

			// SUT
			_eventService = new EventService(_context);
		}

		[Fact]
		public async Task GetAllEventsAsync_ReturnsEventList()
		{
			//Arrange
			var eventList = new List<Event>
			{
				new Event
				{
					Id = 1,
					Name = "Event1"
				},
				new Event
				{
					Id = 2,
					Name = "Event2"
				},
				new Event
				{
					Id = 3,
					Name = "Event3"
				}
			};

			var mockDbSet = eventList.AsQueryable().BuildMockDbSet();
			_context.Event.Returns(mockDbSet);

			//Act
			var result = await _eventService.GetAllEventsAsync();

			//Assert
			result.Should().BeOfType<List<Event>>();
			result.Should().HaveCount(c => c == 3);
		}

		[Fact]
		public async Task GetOrCreateLocationAsync_ReturnsLocation()
		{
			//Arrange
			var inputModel = new InputEventModel
			{
				CountryName = "Country1",
				CityName = "City1",
				StreetName = "Street1",
				PostalCode = "12345",
				BuildingNumber = "10"
			};

			//Act
			var result = await _eventService.GetOrCreateLocationAsync(inputModel);

			//Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(1);
		}

		[Fact]
		public async Task CreateEventAsync_Returns()
		{
			//Arrange
			var location = new Location { Id = 1, Street = new Street { Id = 1, Name = "Street1" }, PostalCode = "12345", BuildingNumber = "10" };
			var newEvent = new Event
			{
				Id = 1,
				Name = "Concert1",
				CategoryId = 1,
				DateTime = DateTime.Now,
				Location = location,
				MaxNumberParticipants = 100,
				ImageUrl = "http://example.com/image1.jpg"
			};
			var mockDbSet = Substitute.For<DbSet<Event>>();
			_context.Event.Returns(mockDbSet);


			//Act
			await _eventService.CreateEventAsync(newEvent);

			//Assert
			mockDbSet.Received(1).Add(newEvent);
			await _context.Received(1).SaveChangesAsync();
		}
	}
}
