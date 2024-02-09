using Moq;
using AutoMapper;
using Nexify.Domain.Entities.Subcategories;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Dtos;
using Nexify.Service.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Nexify.Service.UnitTests.Services
{
    public class SubcategoryServiceTests
    {
        private readonly Mock<ISubcategoryRepository> _subcategoryRepositoryMock;
        private readonly Mock<IImagesService> _imagesServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly SubcategoryService _service;

        public SubcategoryServiceTests()
        {  
            _subcategoryRepositoryMock = new Mock<ISubcategoryRepository>();
            _imagesServiceMock = new Mock<IImagesService>();
            _mapperMock = new Mock<IMapper>();
            _service = new SubcategoryService(_subcategoryRepositoryMock.Object, _imagesServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task AddSubCategoryAsync_ValidData_AddsSubcategoryToRepository()
        {
            // Arrange
            var subcategoryToAdd = new AddSubcategory
            {
                CategoryName = "TestCategory",
                CategoryId = Guid.NewGuid(), 
            };
            var subcategories = new List<AddSubcategory> { subcategoryToAdd };

            _mapperMock.Setup(m => m.Map<Subcategory>(It.IsAny<AddSubcategory>())).Returns(new Subcategory());

            // Act
            await _service.AddSubCategoryAsync(subcategories);

            // Assert
            _subcategoryRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Subcategory>()), Times.Once);
        }


        [Fact]
        public async Task AddSubCategoryAsync_InvalidData_ThrowsException()
        {
            // Arrange
            var subcategories = new List<AddSubcategory> { new AddSubcategory() };
            _mapperMock
                .Setup(m => m.Map<Subcategory>(It.IsAny<AddSubcategory>()))
                .Returns(new Subcategory());

            // Act & Assert
            await Assert.ThrowsAsync<SubcategoryValidationException>(() => _service.AddSubCategoryAsync(subcategories));
        }

 
        [Fact]
        public async Task UpdateSubCategoryAsync_ValidDto_CallsRepositoryUpdate()
        {
            // Arrange
            var subcategoryDto = new SubcategoryDto
            {
                Id = Guid.NewGuid(),
                CategoryName = "TestCategory",
                Description = "Test description",
                Images = new List<IFormFile>
            {
                new FormFile(new MemoryStream(new byte[1]), 0, 1, "image1", "image1.jpg"),
                new FormFile(new MemoryStream(new byte[1]), 0, 1, "image2", "image2.jpg")
            },
                ImageName = "testimage.jpg",
                CategoryId = "testCategoryId"
            };

            _imagesServiceMock
                .Setup(s => s.MapAndProcessObjectAsync<SubcategoryDto, Subcategory>(
                    subcategoryDto,
                    It.IsAny<Func<SubcategoryDto, List<IFormFile>>>(),
                    It.IsAny<Func<SubcategoryDto, string>>(),
                    It.IsAny<Func<SubcategoryDto, string, string>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new Subcategory());

            // Act
            await _service.UpdateSubCategoryAsync(subcategoryDto, "rootPath");

            // Assert
            _subcategoryRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Subcategory>()), Times.Once);
        }


        [Fact]
        public async Task DeleteSubCategoryAsync_NullId_ThrowsSubcategoryException()
        {
            // Arrange
            string? id = null;

            // Act & Assert
            await Assert.ThrowsAsync<SubcategoryException>(() => _service.DeleteSubCategoryAsync(id));
        }

        [Fact]
        public async Task DeleteSubCategoryAsync_EmptyId_ThrowsSubcategoryException()
        {
            // Arrange
            string id = "";

            // Act & Assert
            await Assert.ThrowsAsync<SubcategoryException>(() => _service.DeleteSubCategoryAsync(id));
        }

        [Fact]
        public async Task DeleteSubCategoryAsync_ValidId_CallsRepositoryRemoveAsync()
        {
            // Arrange
            var id = "7cfade2e-0554-4b7f-b89b-4819c7202b1a"; // Example valid GUID string
            var expectedGuid = Guid.Parse(id);

            // Act
            await _service.DeleteSubCategoryAsync(id);

            // Assert
            _subcategoryRepositoryMock.Verify(r => r.RemoveAsync(expectedGuid), Times.Once);
        }

    }
}
