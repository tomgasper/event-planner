using EventPlanner.Data;
using EventPlanner.Interfaces;
using EventPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EventPlanner.Services
{
	public class AccountService : IAccountService
	{
		private readonly IDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;

		public AccountService(UserManager<AppUser> userManager, IDbContext dbContext, SignInManager<AppUser> signInManager) {
			_context = dbContext;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public InputEditUserModel PassInputUserInfo(AppUser user)
		{
			InputEditUserModel inputModel = new()
			{
				UserName = user.UserName,
				Email = user.Email,
				FirstName = user.FirstName,
				LastName = user.LastName
			};

			return inputModel;
		}

		public void PassInputUserInfo(InputEditUserModel inputModel, ref AppUser user)
		{
			user.UserName = inputModel.UserName;
			user.Email = inputModel.Email;
			user.FirstName = inputModel.FirstName;
			user.LastName = inputModel.LastName;
		}

		public async Task<(IdentityResult, AppUser)> CreateNewUser(InputUserModel inputModel)
		{
			var user = new AppUser
			{
				UserName = inputModel.UserName,
				Email = inputModel.Email,
				FirstName = inputModel.FirstName,
				LastName = inputModel.LastName
			};
				
			var result = await _userManager.CreateAsync(user, inputModel.Password);
			return (result, user);
		}

		public async Task<IdentityResult> AddToRoleAsync(AppUser user, string nameRole)
		{
			return await _userManager.AddToRoleAsync(user, nameRole);
		}

		public async Task<int> EditUserInfo(InputEditUserModel inputModel, AppUser user)
		{
			PassInputUserInfo(inputModel, ref user);
			_context.Update(user);
			return await _context.SaveChangesAsync();
		}

		public async Task<SignInResult> Login(InputLoginModel inputModel)
		{
			return await _signInManager.PasswordSignInAsync(
				inputModel.UserName, inputModel.Password, false, false);
		}

		public async Task Logout()
		{
			await _signInManager.SignOutAsync();
		}
	}
}