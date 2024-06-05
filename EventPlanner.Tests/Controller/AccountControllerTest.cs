using Microsoft.AspNetCore.Identity;
using EventPlanner.Models;
using EventPlanner.Interfaces;
using EventPlanner.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Tests.Controller
{
	public class AccountControllerTest
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IAccountService _accountService;
		private readonly AccountController _accountController;

		public AccountControllerTest()
		{
			// Dependencies
			var userStore = Substitute.For<IUserStore<AppUser>>();
			_userManager = Substitute.For<UserManager<AppUser>>(userStore, null, null, null, null, null, null, null, null);
			_accountService = Substitute.For<IAccountService>();

			// SUT
			_accountController = new AccountController(_userManager, _accountService);
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
			var result = await _accountController.Index();

			// Assert
			result.Should().BeOfType<ViewResult>()
				.Which.Model.Should().BeAssignableTo<InputEditUserModel>();
		}

		[Fact]
		public async Task Index_Post_RedirectsToIndex()
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
			var result = await _accountController.Index(inputModel);

			// Assert
			result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
		}

		[Fact]
		public async Task Login_Post_RedirectsToHome_WhenSuccess()
		{
			// Arrange
			var inputModel = new InputLoginModel
			{
				UserName = "Username1",
				Password = "password1"
			};
			_accountService.Login(inputModel).Returns(Microsoft.AspNetCore.Identity.SignInResult.Success);

			// Act
			var result = await _accountController.Login(inputModel);

			// Assert
			result.Should().BeOfType<LocalRedirectResult>().Which.Url.Should().Be("/");
		}

		[Fact]
		public async Task Login_Post_RedirectsToHome_WhenFail()
		{
			// Arrange
			var inputModel = new InputLoginModel
			{
				UserName = "Username1",
				Password = "password1"
			};
			_accountService.Login(inputModel).Returns(Microsoft.AspNetCore.Identity.SignInResult.Failed);

			// Act
			var result = await _accountController.Login(inputModel);

			// Assert
			result.Should().BeOfType<ViewResult>();
		}
	}
}