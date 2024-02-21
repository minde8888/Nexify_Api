using Moq;
using Nexify.Service.Services;
using Nexify.Domain.Exceptions;
using AutoMapper;

namespace Nexify.Service.UnitTests.Services
{
    public class ImagesServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly ImagesService _imagesService;

        public ImagesServiceTests()
        {
            _mapperMock = new Mock<IMapper>();
            _imagesService = new ImagesService(_mapperMock.Object);
        }

        [Fact]
        public async Task SaveImages_NullImageFiles_ThrowsException()
        {
            // Arrange & Act & Assert
            await Assert.ThrowsAsync<FileException>(() => _imagesService.SaveImagesAsync(null));
        }
        [Fact]
        public async Task DeleteImageAsync_FileExists_DeletesFile()
        {
            // Arrange
            var fileName = Path.GetRandomFileName();
            var path = Path.Combine(Path.GetTempPath(), fileName);
            await File.WriteAllTextAsync(path, "dummy content");

            // Act
            await _imagesService.DeleteImageAsync(path);

            // Assert
            Assert.False(File.Exists(path));
        }

        [Fact]
        public async Task DeleteImageAsync_FileDoesNotExist_DoesNotThrowException()
        {
            // Arrange
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            // Act & Assert
            await _imagesService.DeleteImageAsync(path);
        }


    }
}
