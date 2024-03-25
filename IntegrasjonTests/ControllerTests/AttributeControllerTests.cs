using IntegrasjonTests.Setup;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using Nexify.Domain.Entities.Attributes;
using Nexify.Service.Dtos.Attributes;
using Nexify.Service.Dtos.Post;
using System.Net;
using System.Net.Http.Headers;

namespace IntegrasjonTests.ControllerTests
{
    public class AttributeControllerTests
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string _baseUrl;
        private readonly JwtToken _jwtToken;

        public AttributeControllerTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>().Build();

            _factory = new CustomWebApplicationFactory<Program>(configuration);
            _client = _factory.CreateClient();
            _baseUrl = "/api/v1/Attribute";

            _jwtToken = new JwtToken(configuration);
        }

        [Fact]
        public async Task AddNewAttributesAsync_ReturnsOkResult()
        {
            // Arrange
            var token = _jwtToken.GenerateJwtToken("admin@example.com", "Admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var formData = new MultipartFormDataContent
            {
                { new StringContent("Test Title"), nameof(AttributesRequest.AttributeName) }
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
        public async Task GetAll_ReturnsOkResultWithAttributes()
        {
            // Arrange
            var token = _jwtToken.GenerateJwtToken("admin@example.com", "Admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var attribute = new ItemsAttributes
            {
                Id = Guid.NewGuid(),
                AttributeName = "TestAttributeName"
            };

            var mockAttributes = new List<ItemsAttributes> { attribute };
            _factory.AttributesRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(mockAttributes);

            // Act
            var response = await _client.GetAsync($"{_baseUrl}");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var deserializedCategories = JsonConvert.DeserializeObject<List<ItemsAttributes>>(responseContent);

            // Assert
            Assert.NotNull(deserializedCategories);
            Assert.NotEmpty(deserializedCategories);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOkResult()
        {
            // Arrange
            var token = _jwtToken.GenerateJwtToken("admin@example.com", "Admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var attribute = new ItemsAttributes
            {
                Id = Guid.NewGuid(),
                AttributeName = "TestAttributeName"
            };

            _factory.AttributesRepositoryMock.Setup(x => x.ModifyAsync(attribute));

            var guid = Guid.NewGuid().ToString();

            var formData = new MultipartFormDataContent
            {
                { new StringContent(guid), nameof(ItemsAttributes.Id) },
                { new StringContent("Updated Title"), nameof(ItemsAttributes.AttributeName) }
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
