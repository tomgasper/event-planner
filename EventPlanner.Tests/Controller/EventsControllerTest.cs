using System.Collections.Generic;
using System.Threading.Tasks;
using EventPlanner.Controllers;
using EventPlanner.Data;
using EventPlanner.Models;
using EventPlanner.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace EventPlanner.Tests.Controller
{
	public class EventsControllerTest
	{
		private readonly ILogger<EventsController> _logger;
		private readonly EventPlannerDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		private readonly IEventService _eventService;
		private readonly EventsController _eventsController;

		public EventsControllerTest()
		{
			// Dependencies
			_logger = Substitute.For<ILogger<EventsController>>();
			// In-Memory Db
			var options = new DbContextOptionsBuilder<EventPlannerDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;
			_context = new EventPlannerDbContext(options);
			var userStore = Substitute.For<IUserStore<AppUser>>();
			_userManager = Substitute.For<UserManager<AppUser>>(userStore, null, null, null, null, null, null, null, null);
			_eventService = Substitute.For<IEventService>();

			// SUT
			_eventsController = new EventsController(_logger, _context, _userManager, _eventService);

			// Seed the database
			_context.Category.AddRange(new List<Category>
			{
				new Category { Id = 1, Name = "Music" }
			});
			_context.SaveChanges();
		}

		[Fact]
		public async Task Index_ReturnsViewResult_WithListOfEvents()
		{
			// Arrange
			var events = new List<Event>
			{
				new Event { Name = "Event1" },
				new Event { Name = "Event2" }
			};
			_eventService.GetAllEventsAsync().Returns(events);

			// Act
			var result = await _eventsController.Index();

			// Assert
			result.Should().BeOfType<ViewResult>().Which.Model.Should().BeAssignableTo<IEnumerable<Event>>().Which.Should().HaveCount(2);
		}

		[Fact]
		public async Task Create_Get_ReturnsViewResult()
		{
			// Act
			var result = await _eventsController.Create();

			// Assert
			result.Should().BeOfType<ViewResult>();
		}

		[Fact]
		public async Task Create_Post_ReturnsRedirectToActionResult_WhenModelStateIsValid()
		{
			// Arrange
			var inputModel = new InputEventModel
			{
				Name = "Event1",
				CategoryId = 1,
				DateTime = DateTime.Now,
				CountryName = "Country1",
				CityName = "City1",
				StreetName = "Street1",
				PostalCode = "12345",
				BuildingNumber = "10",
				MaxNumberParticipants = 100,
				ImageUrl = "http://example.com/image.jpg"
			};

			var location = new Location { Id = 1, Street = new Street { Id = 1, Name = "Street1" }, PostalCode = "12345", BuildingNumber = "10" };
			_eventService.GetOrCreateLocationAsync(inputModel).Returns(location);

			// Act
			var result = await _eventsController.Create(inputModel);

			// Assert
			result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
			await _eventService.Received(1).CreateEventAsync(Arg.Is<Event>(e => e.Name == inputModel.Name));
		}

		[Fact]
		public async Task Create_Post_ReturnsViewResult_WhenModelStateIsInvalid()
		{
			// Arrange
			var inputModel = new InputEventModel();
			_eventsController.ModelState.AddModelError("Name", "Required");

			// Act
			var result = await _eventsController.Create(inputModel);

			// Assert
			result.Should().BeOfType<ViewResult>();
			_eventService.DidNotReceive().CreateEventAsync(Arg.Any<Event>());
		}

		[Fact]
		public async Task Create_Post_ReturnsViewResult_WhenEventAlreadyExists()
		{
			// Arrange
			var inputModel = new InputEventModel
			{
				Name = "Event1",
				CategoryId = 1,
				DateTime = DateTime.Now,
				CountryName = "Country1",
				CityName = "City1",
				StreetName = "Street1",
				PostalCode = "12345",
				BuildingNumber = "10",
				MaxNumberParticipants = 100,
				ImageUrl = "http://example.com/image.jpg"
			};

			var location = new Location { Id = 1, Street = new Street { Id = 1, Name = "Street1" }, PostalCode = "12345", BuildingNumber = "10" };
			_eventService.GetOrCreateLocationAsync(inputModel).Returns(location);

			var newEvent = new Event
			{
				Name = inputModel.Name,
				CategoryId = inputModel.CategoryId,
				DateTime = inputModel.DateTime,
				Location = location,
				MaxNumberParticipants = inputModel.MaxNumberParticipants,
				ImageUrl = inputModel.ImageUrl
			};

			_eventService.EventExistsAsync(Arg.Any<Event>()).Returns(true);

			// Act
			var result = await _eventsController.Create(inputModel);

			// Assert
			result.Should().BeOfType<ViewResult>();
			_eventService.DidNotReceive().CreateEventAsync(Arg.Any<Event>());
			// _eventsController.ModelState.Should().ContainKey(string.Empty).WhichValue.Errors.Should().ContainSingle(e => e.ErrorMessage == "An event with the same details already exists.");
		}
	}
}
