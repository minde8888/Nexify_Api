using IntegrasjonTests.Setup;
using Moq;
using Nexify.Service.Dtos.Product;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.Extensions.Configuration;
using Nexify.Domain.Entities.Pagination;
using Newtonsoft.Json;
using Nexify.Domain.Entities.Products;

namespace IntegrasjonTests.ControllerTests
{
    public class ProductControllerTests
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string _baseUrl;
        private readonly JwtToken _jwtToken;

        public ProductControllerTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>().Build();

            _factory = new CustomWebApplicationFactory<Program>(configuration);
            _client = _factory.CreateClient();
            _baseUrl = "/api/v1/Product";

            _jwtToken = new JwtToken(configuration);
        }

        [Fact]
        public async Task AddNewProductAsync_ReturnsOk()
        {
            var token = _jwtToken.GenerateJwtToken("admin@example.com", "Admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var formData = new MultipartFormDataContent
            {
                { new StringContent("Test Title"), nameof(ProductRequest.Title) },
                { new StringContent("99.99"), nameof(ProductRequest.Price) }, 
                { new StringContent("10"), nameof(ProductRequest.Stock) }
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
        public async Task GetAllAsync_ReturnsOkWithProductsResponse()
        {
            // Arrange
            var filter = new PaginationFilter { PageNumber = 1, PageSize = 10 };
            var mockProducts = new List<Product>
            {
                new Product
                {
                    Id = Guid.NewGuid(),
                    Title = "Sample Product",
                    Content = "Sample Description",
                    Price = "100",
                    Discount = "10%",
                    Stock = "20",
                    Location = "Sample Location",
                    DateCreated = DateTime.Now,
                }
            };

            var pagedResult = new PagedResult<Product>
            {
                Items = mockProducts,
                TotalCount = mockProducts.Count
            };

            _factory.ProductsRepositoryMock
                .Setup(repo => repo.FetchAllAsync(It.IsAny<PaginationFilter>()))
                .ReturnsAsync(pagedResult);

            // Act
            var response = await _client.GetAsync($"{_baseUrl}?pageNumber={filter.PageNumber}&pageSize={filter.PageSize}");
            var result = JsonConvert.DeserializeObject<ProductsResponse>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.Equal(filter.PageNumber, result.PageNumber);
            Assert.Equal(filter.PageSize, result.PageSize);
            Assert.Equal(1, result.TotalPages);
            Assert.Equal(mockProducts.Count, result.TotalRecords);
            Assert.Equal(mockProducts.Count, result.Products.Count);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOk()
        {
            // Arrange
            var token = _jwtToken.GenerateJwtToken("admin@example.com", "Admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var guid = Guid.NewGuid().ToString();

            var formData = new MultipartFormDataContent
            {
                { new StringContent(guid), nameof(ProductUpdate.ProductId) },
                { new StringContent("Test Title"), nameof(ProductRequest.Title) },
                { new StringContent("99.99"), nameof(ProductRequest.Price) },
                { new StringContent("10"), nameof(ProductRequest.Stock) },
                { new StringContent("This is updated content."), nameof(ProductUpdate.Content) }
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
        public async Task DeleteProductAsync_ReturnsOk()
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