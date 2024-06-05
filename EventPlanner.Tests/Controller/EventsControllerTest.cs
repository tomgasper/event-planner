using EventPlanner.Controllers;
using EventPlanner.Data;
using EventPlanner.Models;
using EventPlanner.Interfaces;

using FluentAssertions;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;

namespace EventPlanner.Tests.Controller
{
    public class EventsControllerTest
	{
		private readonly ILogger<EventsController> _logger;
		private readonly IDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		private readonly IEventService _eventService;
		private readonly EventsController _eventsController;

		public EventsControllerTest()
		{
			// Dependencies
			_logger = Substitute.For<ILogger<EventsController>>();
			_context = Substitute.For<IDbContext>();
			var userStore = Substitute.For<IUserStore<AppUser>>();
			_userManager = Substitute.For<UserManager<AppUser>>(userStore, null, null, null, null, null, null, null, null);
			_eventService = Substitute.For<IEventService>();

			// SUT
			_eventsController = new EventsController(_logger, _context, _userManager, _eventService);

			// Seed data - fill category select list
			var categoryList = new List<Category> { new Category { Id = 1, Name = "Category1" } };
			var mockCategoryDbSet = categoryList.AsQueryable().BuildMockDbSet();
			_context.Category.Returns(mockCategoryDbSet);
		}

		[Fact]
		public async Task Index_ReturnsViewResult_WithListOfEvents()
		{
			// Arrange
			var eventList = new List<Event>
			{
				new Event { Name = "Event1" },
				new Event { Name = "Event2" }
			};

			_eventService.GetAllEventsAsync().Returns(eventList);

			// Act
			var result = await _eventsController.Index();

			// Assert
			result.Should().BeOfType<ViewResult>()
				.Which.Model.Should().BeAssignableTo<IEnumerable<Event>>().Which.Should().HaveCount(2);
		}

		[Fact]
		public async Task Create_Get_ReturnsViewResult()
		{
			//Act
			var result = await _eventsController.Create();

			//Assert
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
			};

			var location = new Location
			{
				Id = 1,
				Street = new Street { Id = 1, Name = "Street1" },
				PostalCode = "12345",
				BuildingNumber = "10"
			};

			_eventService.EventExistsAsync(Arg.Any<Event>()).Returns(true);
			_eventService.GetOrCreateLocationAsync(Arg.Any<InputEventModel>()).Returns(location);

			// Act
			var result = await _eventsController.Create(inputModel);

			// Assert
			result.Should().BeOfType<ViewResult>();
			_eventService.DidNotReceive().CreateEventAsync(Arg.Any<Event>());
			var modelState = _eventsController.ModelState[string.Empty];
			modelState.Errors.Should().ContainSingle(e =>
			e.ErrorMessage == "An event with the same details already exists.");
		}
	}
}
