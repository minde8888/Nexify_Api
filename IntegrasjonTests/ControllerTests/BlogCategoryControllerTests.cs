using IntegrasjonTests.Setup;
using Microsoft.Extensions.Configuration;
using Moq;
using Nexify.Service.Dtos;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using Nexify.Domain.Entities.Categories;

namespace IntegrasjonTests.ControllerTests
{
    public class BlogCategoryControllerTests
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string _baseUrl;
        private readonly JwtToken _jwtToken;

        public BlogCategoryControllerTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>().Build();

            _factory = new CustomWebApplicationFactory<Program>(configuration);
            _client = _factory.CreateClient();
            _baseUrl = "/api/v1/blogCategory";

            _jwtToken = new JwtToken(configuration);
        }

        [Fact]
        public async Task AddNewCategory_ReturnsOkResult()
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
        public async Task GetAll_ReturnsOkObjectResult_WithCategories()
        {
            // Arrange
            var category = new BlogCategory
            {
                Id = Guid.NewGuid(),
                Title = "Test Category Name",
                Description = "Test Description"
            };

            var mockCategories = new List<BlogCategory> { category };
            _factory.BlogCategoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(mockCategories);

            // Act
            var response = await _client.GetAsync($"{_baseUrl}");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var deserializedCategories = JsonConvert.DeserializeObject<List<Category>>(responseContent);

            // Assert
            Assert.NotNull(deserializedCategories);
            Assert.NotEmpty(deserializedCategories);
        }

        [Fact]
        public async Task Update_ReturnsOkResult()
        {
            // Arrange
            var token = _jwtToken.GenerateJwtToken("admin@example.com", "Admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var category = new BlogCategory
            {
                Id = Guid.NewGuid(),
                Title = "Test Category Name",
                Description = "Test Description"
            };

            _factory.BlogCategoryRepositoryMock.Setup(x => x.UpdateAsync(category));

            var guid = Guid.NewGuid().ToString();

            var formData = new MultipartFormDataContent
            {
                { new StringContent(guid), nameof(BlogCategoryDto.Id) },
                { new StringContent("Updated Title"), nameof(BlogCategoryDto.CategoryName) },
                { new StringContent("This is updated content."), nameof(BlogCategoryDto.Description) }
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
        public async Task Delete_ReturnsOkResult()
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
