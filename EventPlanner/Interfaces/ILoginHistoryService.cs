using UAParser;

namespace EventPlanner.Interfaces
{
	public interface ILoginHistoryService
	{
		Task AddLoginRecord(int userId, bool isSuccess, string ipAddress, string failureReason = null);

		public string GetDeviceInfo(ClientInfo clientInfo);

		public string GetBrowserInfo(ClientInfo clientInfo);

    }
}
