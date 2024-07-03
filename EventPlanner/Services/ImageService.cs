using EventPlanner.Configuration;
using Microsoft.Extensions.Options;
using EventPlanner.Interfaces;
using System.Runtime.CompilerServices;

using EventPlanner.Exceptions;

namespace EventPlanner.Services
{
	public class ImageService : IImageService
	{
		private readonly IWebHostEnvironment _env;
		private readonly FileStorageSettings _fileStorageSettings;

		public ImageService(IWebHostEnvironment env, IOptions<FileStorageSettings> fileStorageSettings){
			_env = env;
			_fileStorageSettings = fileStorageSettings.Value;
		}

		public string? GetDefaultUserAvatar()
		{
            var localFilePath = Path.Combine(_fileStorageSettings.ProfileImagesPath, _fileStorageSettings.DefaultProfilePictureName);
            var filePath = Path.Combine(_env.WebRootPath, localFilePath);

			if (File.Exists(filePath))
			{
				return Path.Combine("/", localFilePath);
			}
			else { return null; }
        }

		public async Task<string?> UploadImage(IFormFile file)
		{
			string? localFilePath = null;
			if (file != null && file.Length > 0)
			{
				var fileName = Path.GetFileName(file.FileName);
				localFilePath = Path.Combine(_fileStorageSettings.ProfileImagesPath, fileName);
				var filePath = Path.Combine(_env.WebRootPath, localFilePath);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await file.CopyToAsync(fileStream);
				}

				// Make the path relative
				localFilePath = Path.Combine("/", localFilePath);
			}

			return localFilePath;
		}

        public void DeleteImage(string imageUrl)
        {
			if (string.IsNullOrEmpty(imageUrl))
				throw new InvalidInputException("Trying to delete image with null path.");

            var filePath = Path.Combine(_env.WebRootPath, imageUrl.TrimStart('/'));
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
			else throw new NotFoundException($"Couldn't delete the image: {filePath}. The image doesn't exist.");
        }
    }
}