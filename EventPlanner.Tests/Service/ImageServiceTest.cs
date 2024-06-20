using EventPlanner.Services;
using EventPlanner.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;
using FluentAssertions;
using System.IO;
using System.Threading.Tasks;

namespace EventPlanner.Tests.Service
{
    public class ImageServiceTest
    {
        private readonly ImageService _imageService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IOptions<FileStorageSettings> _fileStorageSettings;

        public ImageServiceTest()
        {
            // Mock IWebHostEnvironment
            _webHostEnvironment = Substitute.For<IWebHostEnvironment>();
            _webHostEnvironment.WebRootPath.Returns(Path.GetTempPath()); // Use temporary path for testing

            // Setup FileStorageSettings with IOptions
            _fileStorageSettings = Options.Create(new FileStorageSettings
            {
                ProfileImagesPath = "images/profiles"
            });

            // Create instance of ImageService with mocked and real dependencies
            _imageService = new ImageService(_webHostEnvironment, _fileStorageSettings);
        }

        [Fact]
        public async Task UploadImage_ShouldSaveFile_AndReturnRelativePath()
        {
            // Arrange
            var fileName = "testimage.jpg";
            var fileMock = Substitute.For<IFormFile>();
            fileMock.FileName.Returns(fileName);
            fileMock.Length.Returns(1024);
            var expectedPath = Path.Combine("/", _fileStorageSettings.Value.ProfileImagesPath, fileName);

            // Setup MemoryStream to simulate file upload
            using var memoryStream = new MemoryStream();
            fileMock.OpenReadStream().Returns(memoryStream);
            fileMock.When(x => x.CopyToAsync(Arg.Any<Stream>(), Arg.Any<System.Threading.CancellationToken>()))
                    .Do(x => {
                        var stream = x.Arg<Stream>();
                        memoryStream.CopyTo(stream);
                    });

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, _fileStorageSettings.Value.ProfileImagesPath, fileName);
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            // Act
            var result = await _imageService.UploadImage(fileMock);

            // Assert
            result.Should().Be(expectedPath);
            File.Exists(filePath).Should().BeTrue();

            // Cleanup
            if (File.Exists(filePath))
            {
                File.Delete(filePath); // Ensure the file is deleted after test
            }
        }
    }
}