﻿using EventPlanner.Configuration;
using Microsoft.Extensions.Options;
using EventPlanner.Interfaces;
using System.Runtime.CompilerServices;

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

		public async Task<string?> UploadImage(IFormFile file)
		{
			string? localFilePath = null;
			if (file != null && file.Length > 0)
			{
				var fileName = Path.GetFileName(file.FileName);
				localFilePath = Path.Combine(_fileStorageSettings.ProfileImagesPath, fileName);
				var filePath = Path.Combine(_env.WebRootPath, localFilePath);
				

				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await file.CopyToAsync(fileStream);
				}

				// Make the path relative
				localFilePath = Path.Combine("/", localFilePath);
			}

			return localFilePath;
		}
	}
}