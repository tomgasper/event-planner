using EventPlanner.Data;
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
using NSubstitute;
using EventPlanner.Models.Events;
using EventPlanner.Models.Location;

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
		public async Task GetOrCreateLocationAsync_ReturnsExistingLocation()
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
			var countryList = new List<Country> { new Country { Id = 1, Name = "Country1" } };
			var cityList = new List<City> { new City { Id = 1, Name = "City1", Country = countryList[0] } };
			var streetList = new List<Street> { new Street { Id = 1, Name = "Street1", City = cityList[0] }, };
			var locationList = new List<Location>{ new Location { Id = 1, Street = streetList[0], PostalCode = "12345", BuildingNumber = "10" } };

			var mockCountryDbSet = countryList.AsQueryable().BuildMockDbSet();
			var mockCityDbSet = cityList.AsQueryable().BuildMockDbSet();
			var mockStreetDbSet = streetList.AsQueryable().BuildMockDbSet();
			var mockLocationDbSet = locationList.AsQueryable().BuildMockDbSet();

			_context.Country.Returns(mockCountryDbSet);
			_context.City.Returns(mockCityDbSet);
			_context.Street.Returns(mockStreetDbSet);
			_context.Location.Returns(mockLocationDbSet);

			//Act
			var result = await _eventService.GetOrCreateLocationAsync(inputModel);

			//Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(1);
		}

		[Fact]
		public async Task GetOrCreateLocationAsync_ReturnsNewLocation()
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
			var countryList = new List<Country> { new Country { Id = 1, Name = "CountryOther" } };
			var cityList = new List<City> { new City { Id = 1, Name = "CityOther", Country = countryList[0] } };
			var streetList = new List<Street> { new Street { Id = 1, Name = "StreetOther", City = cityList[0] }, };
			var locationList = new List<Location> { new Location { Id = 1, Street = streetList[0], PostalCode = "12345", BuildingNumber = "10" } };

			var mockCountryDbSet = countryList.AsQueryable().BuildMockDbSet();
			var mockCityDbSet = cityList.AsQueryable().BuildMockDbSet();
			var mockStreetDbSet = streetList.AsQueryable().BuildMockDbSet();
			var mockLocationDbSet = locationList.AsQueryable().BuildMockDbSet();

			_context.Country.Returns(mockCountryDbSet);
			_context.City.Returns(mockCityDbSet);
			_context.Street.Returns(mockStreetDbSet);
			_context.Location.Returns(mockLocationDbSet);

			//Act
			var result = await _eventService.GetOrCreateLocationAsync(inputModel);

			//Assert
			result.Should().NotBeNull();
			result.Id.Should().NotBe(1);
			_context.Received(1).Add(Arg.Any<Location>());
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
			await _eventService.AddEventAsync(newEvent);

			//Assert
			mockDbSet.Received(1).Add(newEvent);
			await _context.Received(1).SaveChangesAsync();
		}
	}
}
