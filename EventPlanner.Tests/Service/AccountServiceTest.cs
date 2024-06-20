using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EventPlanner.Interfaces;
using EventPlanner.Models;
using EventPlanner.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace EventPlanner.Tests.Service
{
	public class AccountServiceTest
	{
		private readonly IDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly IAccountService _accountService;
		private readonly ILoginHistoryService _loginHistoryService;
		private readonly IImageService _imageService;

		public AccountServiceTest()
		{
			// Dependencies
			_context = Substitute.For<IDbContext>();
			var userStore = Substitute.For<IUserStore<AppUser>>();
			_userManager = Substitute.For<UserManager<AppUser>>(userStore, null, null, null, null, null, null, null, null);
			_signInManager = Substitute.For<SignInManager<AppUser>>(
			_userManager, Substitute.For<IHttpContextAccessor>(), Substitute.For<IUserClaimsPrincipalFactory<AppUser>>(), null, null, null, null);
			_loginHistoryService = Substitute.For<ILoginHistoryService>();
			_imageService = Substitute.For<IImageService>();

			// SUT
			_accountService = new AccountService(_userManager, _context, _signInManager, _imageService, _loginHistoryService);
		}

		[Fact]
		public async Task CreateNewUser_ReturnsTuple()
		{
			// Arrange
			var inputModel = new InputUserModel
			{
				UserName = "testuser",
				Email = "test@example.com",
				FirstName = "Test",
				LastName = "User",
				Password = "Password123!"
			};
			var user = new AppUser { UserName = inputModel.UserName, Email = inputModel.Email };
			_userManager.CreateAsync(Arg.Any<AppUser>(), Arg.Any<string>()).Returns(IdentityResult.Success);

			// Act
			var (result, createdUser) = await _accountService.CreateNewUser(inputModel);

			// Assert
			result.Succeeded.Should().BeTrue();
			createdUser.UserName.Should().Be(inputModel.UserName);
			createdUser.Email.Should().Be(inputModel.Email);
		}

		[Fact]
		public async Task Logout_ShouldSignOutUser()
		{
			// Act
			await _accountService.Logout();

			// Assert
			await _signInManager.Received(1).SignOutAsync();
		}

		[Fact]
		public async Task AddToRoleAsync_ShouldReturnSuccessResult()
		{
			// Arrange
			var user = new AppUser { UserName = "testuser" };
			_userManager.AddToRoleAsync(user, "Member").Returns(IdentityResult.Success);

			// Act
			var result = await _accountService.AddToRoleAsync(user, "Member");

			// Assert
			result.Succeeded.Should().BeTrue();
		}

		[Fact]
		public async Task EditUserInfo_ShouldUpdateUserAndSaveChanges()
		{
			// Arrange
			var inputModel = new InputEditUserModel
			{
				UserName = "newusername",
				Email = "newemail@example.com",
				FirstName = "NewFirstName",
				LastName = "NewLastName"
			};
			var user = new AppUser { UserName = "oldusername", Email = "oldemail@example.com" };

			// Act
			var result = await _accountService.EditUserInfo(inputModel, user);

			// Assert
			user.UserName.Should().Be(inputModel.UserName);
			user.Email.Should().Be(inputModel.Email);
			user.FirstName.Should().Be(inputModel.FirstName);
			user.LastName.Should().Be(inputModel.LastName);
			await _context.Received(1).SaveChangesAsync();
		}

		[Fact]
		public async Task LoginWithLog_ShouldLoginUser()
		{
			// Arrange
			string userName = "User";
			string password = "Password";
			string IpAddress = "127.0.0.1";

			var foundUser = new AppUser()
			{
				Id = 1,
				UserName = userName
			};

			_userManager.FindByNameAsync(userName).Returns(foundUser);
            _signInManager.PasswordSignInAsync(userName, password, Arg.Any<bool>(), Arg.Any<bool>()).Returns(SignInResult.Success);
			_loginHistoryService.AddLoginRecord(Arg.Any<int>(), true, IpAddress).Returns(Task.CompletedTask);

            // Act
            var result = await _accountService.LoginWithLog(userName, password, IpAddress);

			// Assert
			result.Should().BeTrue();
			await _signInManager.Received(1).PasswordSignInAsync(userName, password, Arg.Any<bool>(), Arg.Any<bool>());
		}

        [Fact]
        public async Task LoginWithLog_ShouldLogUserLogin_OnLoginSuccess()
        {
            // Arrange
            string userName = "User";
            string password = "Password";
            string IpAddress = "127.0.0.1";

            var foundUser = new AppUser()
            {
                Id = 1,
                UserName = userName
            };

            _userManager.FindByNameAsync(userName).Returns(foundUser);
            _signInManager.PasswordSignInAsync(userName, password, Arg.Any<bool>(), Arg.Any<bool>()).Returns(SignInResult.Success);
            _loginHistoryService.AddLoginRecord(Arg.Any<int>(), true, IpAddress).Returns(Task.CompletedTask);

            // Act
            var result = await _accountService.LoginWithLog(userName, password, IpAddress);

            // Assert
            await _loginHistoryService.Received(1).AddLoginRecord(Arg.Any<int>(), true, Arg.Any<string>());
        }

        [Fact]
        public async Task LoginWithLog_ShouldLogUserLogin_OnIncorrectUsername()
        {
            // Arrange
            string userName = "User";
            string password = "Password";
            string IpAddress = "127.0.0.1";

            _userManager.FindByNameAsync(userName).ReturnsNull();
            _signInManager.PasswordSignInAsync(userName, password, Arg.Any<bool>(), Arg.Any<bool>()).Returns(SignInResult.Success);
            _loginHistoryService.AddLoginRecord(Arg.Any<int>(), true, IpAddress).Returns(Task.CompletedTask);

            // Act
            var result = await _accountService.LoginWithLog(userName, password, IpAddress);

			// Assert
			result.Should().Be(false);
            await _loginHistoryService.Received(0).AddLoginRecord(Arg.Any<int>(), Arg.Any<bool>(), Arg.Any<string>());
            await _signInManager.Received(0).PasswordSignInAsync(userName, password, Arg.Any<bool>(), Arg.Any<bool>());
        }
    }
}