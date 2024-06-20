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

		public string GetDeviceInfo(ClientInfo clientInfo)
		{
			return $"{clientInfo.Device.Family} {clientInfo.Device.Brand} {clientInfo.Device.Model}";
        }

		public string GetBrowserInfo(ClientInfo clientInfo)
		{
			return $"{clientInfo.UA.Family} {clientInfo.UA.Major}.{clientInfo.UA.Minor}";
        }

		public async Task AddLoginRecord(int userId, bool isSuccess, string ipAddress, string failureReason = "")
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

			var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
			var clientInfo = uaParser.Parse(userAgent);
			var deviceInfo = GetDeviceInfo(clientInfo);
			var browserInfo = GetBrowserInfo(clientInfo); 

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