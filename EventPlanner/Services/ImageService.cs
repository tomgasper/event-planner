using EventPlanner.Configuration;
using Microsoft.Extensions.Options;
using
using System.Runtime.CompilerServices;

namespace EventPlanner.Services
{
	public class ImageService
	{
		private readonly IWebHostEnvironment _env;
		private readonly FileStorageSettings _fileStorageSettings;

		public ImageService(IWebHostEnvironment env, IOptions<FileStorageSettings> fileStorageSettings){
			_env = env;
			_fileStorageSettings = fileStorageSettings.Value;
		}

		public async Task<string?> UploadImage(IFormFile file)
		{
			string? filePath = null;
			if (file != null && file.Length > 0)
			{
				var fileName = Path.GetFileName(file.FileName);
				filePath = Path.Combine(_env.WebRootPath, _fileStorageSettings.ProfileImagesPath, fileName);

				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await file.CopyToAsync(fileStream);
				}
			}

			return filePath;
		}
	}
}