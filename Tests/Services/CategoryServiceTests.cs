using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Moq;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Entities.Products;
using Nexify.Domain.Entities.Subcategories;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Dtos;
using Nexify.Service.Interfaces;
using Nexify.Service.Services;

namespace Nexify.Service.UnitTests.Services
{

    public class CategoryServiceTests
    {
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new Mock<ICategoryRepository>();
        private readonly Mock<IImagesService> _imagesServiceMock = new Mock<IImagesService>();
        private readonly Mock<ISubcategoryService> _subcategoryServiceMock = new Mock<ISubcategoryService>();
        private readonly CategoryService _categoryService;
        private readonly InlineValidator<AddCategories> _categoryValidator;

        public CategoryServiceTests()
        {
            _categoryService = new CategoryService(
                _mapperMock.Object,
                _categoryRepositoryMock.Object,
                _imagesServiceMock.Object,
                _subcategoryServiceMock.Object);

            _categoryValidator = new InlineValidator<AddCategories>();
        }

        [Fact]
        public async Task AddCategoryAsync_ValidCategory_AddsSuccessfully()
        {
            _categoryValidator.RuleFor(x => x.Subcategories).
                Must(categories => categories != null && categories.Any()).
                WithMessage("At least one category must be selected.");
            // Arrange
            var addCategoriesList = new List<AddCategories>
            {
                new AddCategories
                {
                    CategoryName = "Test Category",
                    Subcategories = new List<AddSubcategory>
                    {
                        new AddSubcategory { CategoryName = "Test Subcategory" }
                    }
                }
            };

            var categoryList = new List<Category>
            {
                new Category
                {
                    CategoryName = "Test Category",
                    Subcategories = new List<Subcategory>
                    {
                        new Subcategory { SubCategoryName = "Test Subcategory" }
                    }
                }
            };

            _mapperMock.Setup(m => m.Map<Category>(It.IsAny<AddCategories>()))
                .Returns((AddCategories source) => new Category { CategoryName = source.CategoryName });

            _categoryRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Category>()))
                .Returns(Task.CompletedTask);

            // Act
            await _categoryService.AddCategoryAsync(addCategoriesList);

            // Assert
            _mapperMock.Verify(m => m.Map<Category>(It.IsAny<AddCategories>()), Times.Exactly(addCategoriesList.Count));
            _categoryRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Exactly(addCategoriesList.Count));
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ReturnsProcessedCategories()
        {
            // Arrange
            var categories = new List<Category>
        {
            new Category
            {
                CategoryName = "Electronics",
                ImageName = "electronics.jpg",
                Subcategories = new List<Subcategory>
                {
                    new Subcategory { SubCategoryName = "Smartphones", ImageName = "smartphones.jpg" }
                }
            }
        };
            var categoryResponses = new List<CategoryResponse>
        {
            new CategoryResponse
            {
                CategoryName = "Electronics",
                ImageSrc = "https://example.com/electronics.jpg"
            }
        };

            _categoryRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(categories);
            _imagesServiceMock.Setup(service => service.ProcessImages(It.IsAny<Category>(), It.IsAny<string>(), It.IsAny<string>()))
                              .Returns((Category cat, string src, string propName) => $"{src}/{cat.ImageName}");
            _mapperMock.Setup(mapper => mapper.Map<List<CategoryResponse>>(It.IsAny<List<Category>>()))
                       .Returns(categoryResponses);

            // Act
            var result = await _categoryService.GetAllCategoriesAsync("https://example.com");

            // Assert
            _categoryRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
            _imagesServiceMock.Verify(service => service.ProcessImages(It.IsAny<Category>(), "https://example.com", "ImageName"), Times.AtLeastOnce);
            _mapperMock.Verify(mapper => mapper.Map<List<CategoryResponse>>(It.IsAny<List<Category>>()), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(categoryResponses.Count, result.Count);
            Assert.Equal(categoryResponses[0].CategoryName, result[0].CategoryName);
            Assert.Equal("https://example.com/electronics.jpg", result[0].ImageSrc);
        }

        [Fact]
        public async Task UpdateCategory_ValidCategoryDto_UpdatesCategory()
        {
            // Arrange
            var categoryDto = new CategoryDto
            {
                CategoryName = "Test Category Name",
                Images = new List<IFormFile> { Mock.Of<IFormFile>() },
                ImageName = "test.jpg"
            };
            var category = new Category()
            {
                CategoryName = "Test Category Name",
            };


            _imagesServiceMock.Setup(service => service.MapAndProcessObjectAsync<CategoryDto, Category>(
                It.IsAny<CategoryDto>(),
                It.IsAny<Func<CategoryDto, IEnumerable<IFormFile>>>(),
                It.IsAny<Func<CategoryDto, string>>(),
                It.IsAny<Func<CategoryDto, string, string>>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            )).ReturnsAsync(category);

            _categoryRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Category>()))
                                   .Returns(Task.CompletedTask);

            // Act
            await _categoryService.UpdateCategory(categoryDto, "contentRootPath");

            // Assert
            _imagesServiceMock.Verify(service => service.MapAndProcessObjectAsync<CategoryDto, Category>(
                It.IsAny<CategoryDto>(),
                It.IsAny<Func<CategoryDto, IEnumerable<IFormFile>>>(),
                It.IsAny<Func<CategoryDto, string>>(),
                It.IsAny<Func<CategoryDto, string, string>>(),
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);

            _categoryRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public async Task RemoveCategoryAsync_WithValidGuidAndExistingCategory_RemovesCategory()
        {
            // Arrange
            var existingCategoryId = Guid.NewGuid();
            var existingCategory = new Category
            {
                Id = existingCategoryId,
                Subcategories = new List<Subcategory>()
            };

            _categoryRepositoryMock.Setup(repo => repo.GetAsync(existingCategoryId))
                .ReturnsAsync(existingCategory);

            _categoryRepositoryMock.Setup(repo => repo.RemoveAsync(existingCategoryId))
                .Returns(Task.CompletedTask);

            // Act
            await _categoryService.RemoveCategoryAsync(existingCategoryId.ToString());

            // Assert
            _categoryRepositoryMock.Verify(repo => repo.RemoveAsync(existingCategoryId), Times.Once);
        }

        [Fact]
        public async Task RemoveCategoryAsync_WithInvalidGuid_ThrowsCategoryException()
        {
            // Arrange
            var invalidGuid = "invalid-guid";

            // Act & Assert
            await Assert.ThrowsAsync<CategoryException>(() => _categoryService.RemoveCategoryAsync(invalidGuid));
        }

        [Fact]
        public async Task RemoveCategoryAsync_WithValidGuidAndNonExistingCategory_ThrowsCategoryException()
        {
            // Arrange
            var nonExistingCategoryId = Guid.NewGuid();

            _categoryRepositoryMock.Setup(repo => repo.GetAsync(nonExistingCategoryId))
                .ReturnsAsync((Category?)null);

            // Act & Assert
            await Assert.ThrowsAsync<CategoryException>(() => _categoryService.RemoveCategoryAsync(nonExistingCategoryId.ToString()));
        }

        [Fact]
        public async Task RemoveCategoryAsync_CategoryWithSubcategories_DeletesSubcategoriesFirst()
        {
            // Arrange
            var categoryIdWithSubcategories = Guid.NewGuid();
            var categoryWithSubcategories = new Category
            {
                Id = categoryIdWithSubcategories,
                Subcategories = new List<Subcategory>
            {
                new Subcategory { SubcategoryId = Guid.NewGuid() }
            }
            };

            _categoryRepositoryMock.Setup(repo => repo.GetAsync(categoryIdWithSubcategories))
                .ReturnsAsync(categoryWithSubcategories);

            _categoryRepositoryMock.Setup(repo => repo.RemoveAsync(categoryIdWithSubcategories))
                .Returns(Task.CompletedTask);

            _subcategoryServiceMock.Setup(service => service.DeleteSubCategoryAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _categoryService.RemoveCategoryAsync(categoryIdWithSubcategories.ToString());

            // Assert
            _subcategoryServiceMock.Verify(service => service.DeleteSubCategoryAsync(It.IsAny<string>()), Times.Exactly(categoryWithSubcategories.Subcategories.Count));
            _categoryRepositoryMock.Verify(repo => repo.RemoveAsync(categoryIdWithSubcategories), Times.Once);
        }
    }
}