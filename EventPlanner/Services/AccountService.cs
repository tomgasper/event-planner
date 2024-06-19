using EventPlanner.Data;
using EventPlanner.Interfaces;
using EventPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

using Microsoft.AspNetCore.Authorization;

namespace EventPlanner.Services
{
	public class AccountService : IAccountService
	{
		private readonly IDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly IImageService _imageService;
		private readonly ILoginHistoryService _loginHistoryService;

		public AccountService(UserManager<AppUser> userManager, IDbContext dbContext, SignInManager<AppUser> signInManager, IImageService imageService, ILoginHistoryService loginHistoryService) {
			_context = dbContext;
			_userManager = userManager;
			_signInManager = signInManager;
			_imageService = imageService;
			_loginHistoryService = loginHistoryService;
		}

		public InputEditUserModel PassInputUserInfo(AppUser user)
		{
			InputEditUserModel inputModel = new()
			{
				ImageUrl = user.ProfileImageUrl,
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
			string? uploadedImage = await _imageService.UploadImage(inputModel.Image);

			if (uploadedImage == null) throw new ArgumentException("Error uploaing image!");

			var user = new AppUser
			{
				ProfileImageUrl = uploadedImage,
				UserName = inputModel.UserName,
				Email = inputModel.Email,
				FirstName = inputModel.FirstName,
				LastName = inputModel.LastName,
				AccountSettings = new AccountSettings()
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

		public async Task<SignInResult> Login(string userName, string password)
		{
			return await _signInManager.PasswordSignInAsync(
                userName, password, false, false);
		}

		public async Task<bool> LoginWithLog(string userName, string password, string ipAddress)
		{
            var result = await Login(userName, password);
            AppUser user = await _userManager.FindByNameAsync(userName);

            if (result.Succeeded)
            {
                await _loginHistoryService.AddLoginRecord(user.Id, true, ipAddress);
				return true;
            }
            else
            {
                string failureReason = result.IsLockedOut ? "Account locked out" : "Invalid credentials";

                // User exists but an incorrect password has been provided
                if (user != null)
                {
                    await _loginHistoryService.AddLoginRecord(user.Id, false, ipAddress, failureReason);
                }

				return false;
            }
        }

        public async Task Logout()
		{
			await _signInManager.SignOutAsync();
		}
	}
}