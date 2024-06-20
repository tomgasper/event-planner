using EventPlanner.Controllers;
using EventPlanner.Interfaces;
using EventPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MockQueryable.NSubstitute;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Tests.Controller
{
    public class EventControllerTest
    {
        IDbContext _context;
        UserManager<AppUser> _userManager;
        IEventService _eventService;
        EventController _eventController;


        public EventControllerTest()
        {
            // Dependencies
            _context = Substitute.For<IDbContext>();
            var userStore = Substitute.For<IUserStore<AppUser>>();
            _userManager = Substitute.For<UserManager<AppUser>>(userStore, null, null, null, null, null, null, null, null);
            _eventService = Substitute.For<IEventService>();

            // SUT
            _eventController = new EventController(_eventService,_context, _userManager);

            // Seed data - fill category select list
            var categoryList = new List<Category> { new Category { Id = 1, Name = "Category1" } };
            var mockCategoryDbSet = categoryList.AsQueryable().BuildMockDbSet();
            _context.Category.Returns(mockCategoryDbSet);

        }


        [Fact]
        public async Task Create_Get_ReturnsViewResult()
        {
            //Act
            var result = await _eventController.Create();

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

            var newEvent = new Event { Id = 1, Name="Event1" };
            _eventService.AddEventAsync(Arg.Any<Event>()).Returns(newEvent);

            // Act
            var result = await _eventController.Create(inputModel);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
            await _eventService.Received(1).AddEventAsync(Arg.Any<Event>());
        }


        [Fact]
        public async Task Create_Post_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            var inputModel = new InputEventModel();
            _eventController.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _eventController.Create(inputModel);

            // Assert
            result.Should().BeOfType<ViewResult>();
            _eventService.DidNotReceive().AddEventAsync(Arg.Any<Event>());
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
            var result = await _eventController.Create(inputModel);

            // Assert
            result.Should().BeOfType<ViewResult>();
            _eventService.DidNotReceive().AddEventAsync(Arg.Any<Event>());
            var modelState = _eventController.ModelState[string.Empty];
            modelState.Errors.Should().ContainSingle(e =>
            e.ErrorMessage == "An event with the same details already exists.");
        }
    }
}
