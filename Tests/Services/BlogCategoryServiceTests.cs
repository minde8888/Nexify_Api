using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Interfaces;
using Nexify.Service.Dtos.Blog;
using Nexify.Service.Services;

namespace Tests.Services
{
    public class BlogCategoryServiceTests
    {
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<IBlogCategoryRepository> _categoryRepositoryMock = new Mock<IBlogCategoryRepository>();
        private readonly Mock<IImagesService> _imagesServiceMock = new Mock<IImagesService>();
        private readonly BlogCategoryService _blogCategoryService;

        public BlogCategoryServiceTests()
        {
            _blogCategoryService = new BlogCategoryService(_mapperMock.Object, _categoryRepositoryMock.Object, _imagesServiceMock.Object);
        }

        [Fact]
        public async Task AddCategoryAsync_ValidCategories_AddsCategoriesSuccessfully()
        {
            // Arrange
            var categoriesDto = new List<BlogCategoryDto>
            {
                new BlogCategoryDto { Title = "Test Category" }
            };

            _imagesServiceMock.Setup(
                x => x.MapAndSaveImages<BlogCategoryDto, BlogCategory>
                (It.IsAny<BlogCategoryDto>(),
                It.IsAny<List<IFormFile>>(), It.IsAny<string>()))
                              .ReturnsAsync(new BlogCategory());

            _categoryRepositoryMock.Setup(x => x.AddAsync(It.IsAny<BlogCategory>()))
                                   .Returns(Task.CompletedTask);

            // Act
            await _blogCategoryService.AddCategoryAsync(categoriesDto);

            // Assert
            _categoryRepositoryMock.Verify(x => x.AddAsync(It.IsAny<BlogCategory>()), Times.Exactly(categoriesDto.Count));
        }

        [Fact]
        public async Task GetAllCategoriesAsync_WhenCalled_ReturnsAllCategories()
        {
            // Arrange
            var categories = new List<BlogCategory>
            {
                new BlogCategory { Title = "Test Category" }
            };

            var categoryResponses = new List<BlogCategoryResponse>
            {
                new BlogCategoryResponse { Title = "Test Category", ImageSrc = "http://example.com/test.jpg" }
            };

            _categoryRepositoryMock.Setup(x => x.GetAllAsync())
                                   .ReturnsAsync(categories);

            _mapperMock.Setup(x => x.Map<List<BlogCategoryResponse>>(It.IsAny<List<BlogCategory>>()))
                       .Returns(categoryResponses); // Adjusted to return a non-empty list

            // Act
            var result = await _blogCategoryService.GetAllCategoriesAsync("imageSrc");

            // Assert
            Assert.NotEmpty(result); // This should now pass since we're returning a non-empty list
            _mapperMock.Verify(x => x.Map<List<BlogCategoryResponse>>(It.IsAny<List<BlogCategory>>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCategory_ValidCategory_UpdatesSuccessfully()
        {
            // Arrange
            var categoryDto = new BlogCategoryDto
            {
                Title = "Test Category"
            };

            _imagesServiceMock.Setup(x => x.MapAndProcessObjectAsync<BlogCategoryDto, BlogCategory>(
                It.IsAny<BlogCategoryDto>(),
                It.IsAny<Func<BlogCategoryDto, IEnumerable<IFormFile>>>(),
                It.IsAny<Func<BlogCategoryDto, string>>(),
                It.IsAny<Func<BlogCategoryDto, string, string>>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(new BlogCategory());

            _categoryRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<BlogCategory>()))
                                   .Returns(Task.CompletedTask);

            // Act
            await _blogCategoryService.UpdateCategory(categoryDto, "contentRootPath");

            // Assert
            _categoryRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<BlogCategory>()), Times.Once);
        }

        [Fact]
        public async Task RemoveCategoryAsync_ValidId_RemovesCategorySuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();

            _categoryRepositoryMock.Setup(x => x.RemoveAsync(It.IsAny<Guid>()))
                                   .Returns(Task.CompletedTask);

            // Act
            await _blogCategoryService.RemoveCategoryAsync(id);

            // Assert
            _categoryRepositoryMock.Verify(x => x.RemoveAsync(It.IsAny<Guid>()), Times.Once);
        }

    }
}
