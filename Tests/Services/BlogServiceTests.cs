using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using Nexify.Domain.Entities.Posts;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Dtos.Post;
using Nexify.Service.Services;
using Nexify.Service.Validators;

namespace Tests.Services
{
    public class BlogServiceTests
    {
        private readonly Mock<IBlogRepository> _blogRepositoryMock = new Mock<IBlogRepository>();
        private readonly Mock<IImagesService> _imagesServiceMock = new Mock<IImagesService>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<IPostCategoryRepository> _postCategoriesRepositoryMock = new Mock<IPostCategoryRepository>();
        private readonly Mock<IUriService> _uriServiceMock = new Mock<IUriService>();
        private readonly BlogService _blogService;

        public BlogServiceTests()
        {
            _blogService = new BlogService(
                 _blogRepositoryMock.Object,
                 _imagesServiceMock.Object,
                 _mapperMock.Object,
                 _postCategoriesRepositoryMock.Object,
                 _uriServiceMock.Object);
        }

        [Fact]
        public async Task AddPostAsync_ValidPost_AddsPostAndCategoriesSuccessfully()
        {
            // Arrange
            var mockImage = new Mock<IFormFile>();
            mockImage.Setup(_ => _.FileName).Returns("valid_image.jpg");
            mockImage.Setup(_ => _.Length).Returns(1024);

            var postRequest = new PostRequest
            {
                Id = Guid.NewGuid(),
                Title = "Test Title",
                Content = "Test Content",
                Images = new List<IFormFile> { mockImage.Object },
                ImageNames = new List<string>(),
                CategoriesIds = new List<Guid> { Guid.NewGuid() }
            };
            var post = new Post();

            _imagesServiceMock.Setup(
                s => s.MapAndSaveImages<PostRequest, Post>(It.IsAny<PostRequest>(),
                It.IsAny<List<IFormFile>>(), It.IsAny<string>()))
                .ReturnsAsync(post);

            _blogRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Post>()))
                .Returns(Task.CompletedTask);

            _postCategoriesRepositoryMock.Setup(r => r.AddPostCategoriesAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            // Act
            await _blogService.AddPostAsync(postRequest);

            // Assert
            _blogRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Post>()), Times.Once);
            _postCategoriesRepositoryMock.Verify(r => r.AddPostCategoriesAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Exactly(postRequest.CategoriesIds.Count));
        }

        [Fact]
        public async Task AddPostCategoriesAsync_WhenValid_AddsPostCategoriesSuccessfully()
        {
            // Arrange
            var postCategories = new PostCategories
            {
                CategoryId = Guid.NewGuid().ToString(),
                PostId = Guid.NewGuid().ToString()
            };
            var validationResult = new ValidationResult();

            _postCategoriesRepositoryMock.Setup(repo => repo.AddPostCategoriesAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                                         .Returns(Task.CompletedTask);

            // Act
            await _blogService.AddPostCategoriesAsync(postCategories);

            // Assert
            _postCategoriesRepositoryMock.Verify(repo => repo.AddPostCategoriesAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task AddPostCategoriesAsync_WhenInvalid_ThrowsPostCategoriesValidationException()
        {
            // Arrange
            var postCategories = new PostCategories
            {
                CategoryId = "",
                PostId = ""
            };

            var validator = new PostCategoriesValidator();

            var validationResult = await validator.ValidateAsync(postCategories);

            if (!validationResult.IsValid)
            {
                _postCategoriesRepositoryMock.Setup(repo => repo.AddPostCategoriesAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                                             .Throws(new PostCategoriesValidationException(validationResult.ToString()));
            }

            // Act & Assert
            await Assert.ThrowsAsync<PostCategoriesValidationException>(() => _blogService.AddPostCategoriesAsync(postCategories));
        }

    }
}
