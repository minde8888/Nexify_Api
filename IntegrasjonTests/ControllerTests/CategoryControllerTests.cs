using IntegrasjonTests.Setup;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Nexify.Service.Dtos.Category;
using Nexify.Service.Dtos.Post;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Nexify.Domain.Entities.Categories;

namespace IntegrasjonTests.ControllerTests
{
    public class CategoryControllerTests
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string _baseUrl;
        private readonly JwtToken _jwtToken;

        public CategoryControllerTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>().Build();

            _factory = new CustomWebApplicationFactory<Program>(configuration);
            _client = _factory.CreateClient();
            _baseUrl = "/api/v1/blog";

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
                { new StringContent("Test Title"), nameof(AddCategories.CategoryName) }
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
        public async Task GetAll_ReturnsOkResultWithCategories()
        {
            // Arrange
            var token = _jwtToken.GenerateJwtToken("admin@example.com", "Admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var categories = new List<Category>
            {
                new Category
                {
                    Id = Guid.NewGuid(),
                    Title = "Test Category"
                }
            };

            _factory.CategoryRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);

            // Act
            var response = await _client.GetAsync($"{_baseUrl}");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Assuming the controller action converts entities to DTOs
            var deserializedCategories = JsonConvert.DeserializeObject<List<CategoryResponse>>(responseContent);

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

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Title = "Test Category"
            };

            _factory.CategoryRepositoryMock.Setup(x => x.UpdateAsync(category));

            var guid = Guid.NewGuid().ToString();

            var formData = new MultipartFormDataContent
            {
                { new StringContent(guid), nameof(category.Id) },
                { new StringContent("Updated Title"), nameof(category.Title) }
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
