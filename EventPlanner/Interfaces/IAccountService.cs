using EventPlanner.Models;
using Microsoft.AspNetCore.Identity;

namespace EventPlanner.Interfaces
{
	public interface IAccountService
	{
		InputEditUserModel PassInputUserInfo(AppUser user);
		public void PassInputUserInfo(InputEditUserModel inputModel, ref AppUser user);
		public Task<(IdentityResult, AppUser)> CreateNewUser(InputUserModel inputModel);
		public Task<IdentityResult> AddToRoleAsync(AppUser user, string nameRole);
		public Task<int> EditUserInfo(InputEditUserModel inputModel, AppUser user);
		public Task<SignInResult> Login(InputLoginModel inputModel);
		public Task Logout();
	}
}
