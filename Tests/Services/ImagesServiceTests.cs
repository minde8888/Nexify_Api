using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Nexify.Service.Services;
using Nexify.Domain.Exceptions;
using AutoMapper;
using Microsoft.AspNetCore.Http.Internal;
using System.Reflection;

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

        //[Fact]
        //public async Task SaveImages_ValidImageFiles_ReturnsImageNames()
        //{
        //    // Arrange
        //    var tempDirectory = Path.Combine("D:\\#C\\Nexify\\Nexify.Aip\\Images"); // Update this path to the correct one
        //    Directory.CreateDirectory(tempDirectory);

        //    var imageFiles = new List<IFormFile>
        //    {
        //        new FormFile(new MemoryStream(new byte[1]), 0, 1, "image1", "image1.jpg"),
        //        new FormFile(new MemoryStream(new byte[1]), 0, 1, "image2", "image2.jpg")
        //    };

        //    // Act
        //    var result = await _imagesService.SaveImages(imageFiles);

        //    // Assert
        //    Assert.NotEmpty(result);
        //    Assert.Equal(2, result.Split(',').Length);

        //    // Clean up
        //    Directory.Delete(tempDirectory, true);
        //}


        [Fact]
        public async Task SaveImages_NullImageFiles_ThrowsException()
        {
            // Arrange & Act & Assert
            await Assert.ThrowsAsync<FileException>(() => _imagesService.SaveImages(null));
        }

    
    }
}
