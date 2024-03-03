using IntegrasjonTests.Setup;
using Microsoft.Extensions.Configuration;
using Nexify.Service.Dtos;
using System.Net.Http.Headers;
using System.Net;

namespace IntegrasjonTests.ControllerTests
{
    public class SubcategoryControllerTests
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string _baseUrl;
        private readonly JwtToken _jwtToken;

        public SubcategoryControllerTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>().Build();

            _factory = new CustomWebApplicationFactory<Program>(configuration);
            _client = _factory.CreateClient();
            _baseUrl = "/api/v1/Subcategory";

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
                { new StringContent("Test Title"), nameof(AddSubcategory.CategoryName) }
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
        public async Task Update_ReturnsOkResult()
        {
            // Arrange
            var token = _jwtToken.GenerateJwtToken("admin@example.com", "Admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var guid = Guid.NewGuid().ToString();

            var formData = new MultipartFormDataContent
            {
                { new StringContent(guid), nameof(SubcategoryDto.Id) },
                { new StringContent("Updated Title"), nameof(SubcategoryDto.CategoryName) },
                { new StringContent("This is updated content."), nameof(SubcategoryDto.Description) }
            };

            var requestUri = $"{_baseUrl}/update";

            // Act
            var response = await _client.PutAsync(requestUri, formData);

            // Assert
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
