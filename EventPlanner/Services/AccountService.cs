using EventPlanner.Data;
using EventPlanner.Interfaces;
using EventPlanner.Models;
using EventPlanner.Models.Profile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

using EventPlanner.Exceptions;

using Microsoft.AspNetCore.Authorization;
using EventPlanner.Models.User;

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

		public async Task MapInputUserInfo(InputEditUserModel inputModel, AppUser user)
		{
			string? newImgUrl = await UpdateUserProfileImage(user, inputModel.ImageFile);

			user.UserName = inputModel.UserName;
			user.Email = inputModel.Email;
			user.FirstName = inputModel.FirstName;
			user.LastName = inputModel.LastName;
			user.ProfileImageUrl = newImgUrl ?? user.ProfileImageUrl;
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

		public async Task<IdentityResult> DeleteUserAsync(AppUser user)
		{
            return await _userManager.DeleteAsync(user);
        }

		public async Task<int> EditUserInfo(InputEditUserModel inputModel, AppUser user)
		{
			await MapInputUserInfo(inputModel, user);
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
            AppUser user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
				return false;
            }

            var result = await Login(userName, password);
            if (result.Succeeded)
            {
                await _loginHistoryService.AddLoginRecord(user.Id, true, ipAddress);
				return true;
            }
            else
            {
                string failureReason = result.IsLockedOut ? "Account locked out" : "Invalid credentials";

                // User exists but an incorrect password has been provided
                await _loginHistoryService.AddLoginRecord(user.Id, false, ipAddress, failureReason);

				return false;
            }
        }

        public async Task<string?> UpdateUserProfileImage(AppUser user, IFormFile newImageFile)
        {
            if (user == null || newImageFile == null) return null;

            // Delete the old image if a new one is uploaded
            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                _imageService.DeleteImage(user.ProfileImageUrl);
            }

            // Upload the new image and update the user's image URL
            var imageUrl = await _imageService.UploadImage(newImageFile);

            return imageUrl;
        }

        public async Task Logout()
		{
			await _signInManager.SignOutAsync();
		}
	}
}