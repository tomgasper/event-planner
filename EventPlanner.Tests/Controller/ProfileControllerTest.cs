using EventPlanner.Controllers;
using EventPlanner.Interfaces;
using EventPlanner.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using FluentAssertions;

namespace EventPlanner.Tests.Controller
{
	public class ProfileControllerTest
	{
		private readonly IAccountService _accountService;
		private readonly UserManager<AppUser> _userManager;
		private readonly ProfileController _profileController;
		private readonly ILogger<ProfileController> _logger;
		private readonly IProfileService _profileService;

		public ProfileControllerTest()
		{
			// Dependencies
			var userStore = Substitute.For<IUserStore<AppUser>>();
			_userManager = Substitute.For<UserManager<AppUser>>(userStore, null, null, null, null, null, null, null, null);
			_accountService = Substitute.For<IAccountService>();
			_logger = Substitute.For<ILogger<ProfileController>>();
			_profileService = Substitute.For<IProfileService>();

			// SUT
			_profileController = new ProfileController(_userManager, _accountService, _logger, _profileService);
		}

		[Fact]
		public async Task Index_Get_ReturnsView()
		{
			// Arrange
			var inputModel = new InputEditUserModel
			{
				UserName = "Username1",
				Email = "email@email.com",
				FirstName = "Firstname",
				LastName = "Lastname"
			};
			_accountService.PassInputUserInfo(Arg.Any<AppUser>()).Returns(inputModel);

			// Act
			var result = await _profileController.Index();

			// Assert
			result.Should().BeOfType<ViewResult>()
				.Which.Model.Should().BeAssignableTo<InputEditUserModel>();
		}


		[Fact]
		public async Task UpdateProfile_Post_RedirectsToIndex()
		{
			// Arrange
			var inputModel = new InputEditUserModel
			{
				UserName = "Username1",
				Email = "email@email.com",
				FirstName = "Firstname",
				LastName = "Lastname"
			};

			// Act
			var result = await _profileController.UpdateProfile(inputModel);

			// Assert
			result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
		}
	}
}
