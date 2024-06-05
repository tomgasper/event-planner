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

namespace EventPlanner.Tests.Service
{
	public class AccountServiceTest
	{
		private readonly IDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly IAccountService _accountService;

		public AccountServiceTest()
		{
			// Dependencies
			_context = Substitute.For<IDbContext>();
			var userStore = Substitute.For<IUserStore<AppUser>>();
			_userManager = Substitute.For<UserManager<AppUser>>(userStore, null, null, null, null, null, null, null, null);
			_signInManager = Substitute.For<SignInManager<AppUser>>(
				_userManager, Substitute.For<IHttpContextAccessor>(), Substitute.For<IUserClaimsPrincipalFactory<AppUser>>(), null, null, null, null);

			// SUT
			_accountService = new AccountService(_userManager, _context, _signInManager);
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
	}
}