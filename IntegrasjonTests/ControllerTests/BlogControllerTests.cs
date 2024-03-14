using IntegrasjonTests.Setup;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Entities.Posts;
using Nexify.Domain.Entities.Products;
using Nexify.Service.Dtos.Post;
using System;
using System.Net;
using System.Net.Http.Headers;

namespace IntegrationTests.ControllerTests
{
    public class BlogControllerTests
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string _baseUrl;
        private readonly JwtToken _jwtToken;

        public BlogControllerTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>().Build();

            _factory = new CustomWebApplicationFactory<Program>(configuration);
            _client = _factory.CreateClient();
            _baseUrl = "/api/v1/blog";

            _jwtToken = new JwtToken(configuration);
        }

        [Fact]
        public async Task AddNewPostAsync_ReturnsOkResult()
        {
            // Arrange
            var token = _jwtToken.GenerateJwtToken("admin@example.com", "Admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var formData = new MultipartFormDataContent
            {
                { new StringContent("Test Title"), nameof(PostRequest.Title) },
                { new StringContent("This is a test post."), nameof(PostRequest.Content) }
            };

            // Act
            var response = await _client.PostAsync($"{_baseUrl}", formData);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Xunit.Sdk.XunitException($"Expected OK, got {response.StatusCode}. Response content: {responseContent}");
            }

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOk()
        {
            // Arrange
            var filter = new PaginationFilter { PageNumber = 1, PageSize = 10 };

            var expectedProducts = new PagedResult<Post>
            {
                Items = new List<Post>
            {
                new Post
                {
                    Id = Guid.NewGuid(),
                    Title = "Sample Product",
                    Content = "Sample Description",
                    IsDeleted = false,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Categories = new List<BlogCategory>()
                }
            },
                TotalCount = 1
            };

            _factory.BlogRepositoryMock
                .Setup(repo => repo.RetrieveAllAsync(It.IsAny<PaginationFilter>()))
                .ReturnsAsync(expectedProducts);

            // Act
            var response = await _client.GetAsync($"{_baseUrl}" +
                $"?pageNumber={filter.PageNumber}" +
                $"&pageSize={filter.PageSize}");

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PostsResponse>(content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.Equal(filter.PageNumber, result.PageNumber);
            Assert.Equal(filter.PageSize, result.PageSize);
            Assert.Equal(expectedProducts.Items.Count, result.Post.Count);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOkResult()
        {
            // Arrange
            var token = _jwtToken.GenerateJwtToken("admin@example.com", "Admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var guid = Guid.NewGuid().ToString();

            var formData = new MultipartFormDataContent
            {
                { new StringContent(guid), nameof(PostUpdateRequest.Id) },
                { new StringContent("Updated Title"), nameof(PostUpdateRequest.Title) },
                { new StringContent("This is updated content."), nameof(PostUpdateRequest.Content) }
            };

            var requestUri = $"{_baseUrl}/update";

            // Act
            var response = await _client.PutAsync(requestUri, formData);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Xunit.Sdk.XunitException($"Expected OK, got {response.StatusCode}. Response content: {responseContent}");
            }

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsOkResult()
        {
            // Arrange
            var guid = Guid.NewGuid().ToString();
            var requestUri = $"{_baseUrl}/id?id={guid}";

            // Act
            var response = await _client.DeleteAsync(requestUri);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Xunit.Sdk.XunitException($"Expected OK, got {response.StatusCode}. Response content: {responseContent}");
            }

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
