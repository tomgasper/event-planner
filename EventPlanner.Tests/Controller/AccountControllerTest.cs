using Microsoft.AspNetCore.Identity;
using EventPlanner.Models;
using EventPlanner.Interfaces;
using EventPlanner.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.Extensions.Logging;
using EventPlanner.Models.User;

namespace EventPlanner.Tests.Controller
{
    public class AccountControllerTest
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IAccountService _accountService;
		private readonly AccountController _accountController;
		private readonly ILoginHistoryService _loginHistoryService;
		private readonly ILogger<AccountController> _logger;

		public AccountControllerTest()
		{
			// Dependencies
			var userStore = Substitute.For<IUserStore<AppUser>>();
			_userManager = Substitute.For<UserManager<AppUser>>(userStore, null, null, null, null, null, null, null, null);
			_accountService = Substitute.For<IAccountService>();
			_loginHistoryService = Substitute.For<ILoginHistoryService>();
			_logger = Substitute.For<ILogger<AccountController>>();

			// SUT
			_accountController = new AccountController(_userManager, _accountService, _loginHistoryService, _logger);
			var httpContext = new DefaultHttpContext();
			_accountController.ControllerContext = new ControllerContext()
			{
				HttpContext = httpContext
			};
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
			_accountService.LoginWithLog(inputModel.UserName, inputModel.Password, Arg.Any<string>() ).Returns(true);

			// Act
			var result = await _accountController.Login(inputModel);

			// Assert
			result.Should().BeOfType<LocalRedirectResult>().Which.Url.Should().Be("/");
		}

		[Fact]
		public async Task Login_Post_RedirectsToLogin_WhenFail()
		{
			// Arrange
			var inputModel = new InputLoginModel
			{
				UserName = "Username1",
				Password = "password1"
			};
			_accountService.LoginWithLog(inputModel.UserName, inputModel.Password, Arg.Any<string>()).Returns(false);

			// Act
			var result = await _accountController.Login(inputModel);

			// Assert
			result.Should().BeOfType<ViewResult>();
		}
	}
}