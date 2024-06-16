using EventPlanner.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using UAParser;

namespace EventPlanner.Interfaces
{
	public class LoginHistoryService : ILoginHistoryService
	{
		private readonly IDbContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly Parser uaParser;

		public LoginHistoryService(IDbContext context, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
			uaParser = Parser.GetDefault();
		}
		public async Task AddLoginRecord(int userId, bool isSuccess, string ipAddress, string failureReason = "")
		{
			var user = _context.Users.FirstOrDefault(u => u.Id == userId);

			var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
			var clientInfo = uaParser.Parse(userAgent);

			var deviceInfo = $"{clientInfo.Device.Family} {clientInfo.Device.Brand} {clientInfo.Device.Model}";
			var browserInfo = $"{clientInfo.UA.Family} {clientInfo.UA.Major}.{clientInfo.UA.Minor}";

			var loginHistory = new LoginHistory
			{
				User = user,
				LoginTime = DateTime.UtcNow,
				IPAddress = ipAddress,
				DeviceInformation = deviceInfo,
				BrowserInformation = browserInfo,
				LoginSuccess = isSuccess,
				FailureReason = failureReason
			};

			_context.LoginHistory.Add(loginHistory);
			await _context.SaveChangesAsync();
		}
	}
}