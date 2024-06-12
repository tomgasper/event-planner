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
		private readonly IEventsService _eventsService;
		private readonly EventsController _eventsController;

		public EventsControllerTest()
		{
			// Dependencies
			_logger = Substitute.For<ILogger<EventsController>>();
			_context = Substitute.For<IDbContext>();
			var userStore = Substitute.For<IUserStore<AppUser>>();
			_userManager = Substitute.For<UserManager<AppUser>>(userStore, null, null, null, null, null, null, null, null);
			_eventsService = Substitute.For<IEventsService>();

			// SUT
			_eventsController = new EventsController(_logger, _context, _userManager, _eventsService);

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

			_eventsService.GetEventsForPageAsync(1,100).Returns(eventList);

			// Act
			var result = await _eventsController.Index("","","", null, null);

			// Assert
			result.Should().BeOfType<ViewResult>()
				.Which.Model.Should().BeAssignableTo<IEnumerable<Event>>().Which.Should().HaveCount(2);
		}
    }
}
