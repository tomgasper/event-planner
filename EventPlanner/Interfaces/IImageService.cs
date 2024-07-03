namespace EventPlanner.Interfaces
{
	public interface IImageService
	{
		Task<string?> UploadImage(IFormFile file);
		void DeleteImage(string imageUrl);

		string? GetDefaultUserAvatar();
    }
}
