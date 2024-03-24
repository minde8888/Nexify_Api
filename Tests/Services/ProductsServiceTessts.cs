using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Entities.Products;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Dtos.Product;
using Nexify.Service.Services;

namespace Tests.Services
{
    public class ProductsServiceTests
    {
        private readonly Mock<IImagesService> _mockImagesService = new Mock<IImagesService>();
        private readonly Mock<IProductsRepository> _mockProductsRepository = new Mock<IProductsRepository>();
        private readonly Mock<IProductCategoryRepository> _mockCategoryRepository = new Mock<IProductCategoryRepository>();
        private readonly Mock<IProductSubcategoryRepository> _mockSubcategoryRepository = new Mock<IProductSubcategoryRepository>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<IUriService> _mockUriService = new Mock<IUriService>();
        private readonly ProductsService _productsService;

        public ProductsServiceTests()
        {
            _productsService = new ProductsService(
               _mockImagesService.Object,
               _mockProductsRepository.Object,
               _mockCategoryRepository.Object,
               _mockSubcategoryRepository.Object,
               _mapperMock.Object,
               _mockUriService.Object
           );
        }

        [Fact]
        public async Task AddProductAsync_ValidRequest_ProcessesSuccessfully()
        {
            // Arrange
            var mockImage = new Mock<IFormFile>();
            mockImage.Setup(_ => _.FileName).Returns("valid_image.jpg");
            mockImage.Setup(_ => _.Length).Returns(1024);

            var productRequest = new ProductRequest
            {
                Title = "Test title",
                Price = "100",
                Stock = "1",
                Images = new List<IFormFile> { mockImage.Object },
                CategoriesIds = new List<Guid>() { Guid.NewGuid() }
            };
            var product = new Product();

            _mockImagesService.Setup(s => s.MapAndSaveImages<ProductRequest, Product>(
                It.IsAny<ProductRequest>(),
                It.IsAny<List<IFormFile>>(),
                It.IsAny<string>()))
                .ReturnsAsync(product);

            _mockProductsRepository.Setup(r => r.AddAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            _mockCategoryRepository.Setup(r => r.AddProductCategoriesAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            // Act
            await _productsService.AddProductAsync(productRequest);

            // Assert
            _mockImagesService.Verify(s => s.MapAndSaveImages<ProductRequest, Product>(
                It.IsAny<ProductRequest>(), It.IsAny<List<IFormFile>>(), It.IsAny<string>()), Times.Once);

            _mockProductsRepository.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);

            _mockCategoryRepository.Verify(r => r.AddProductCategoriesAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Exactly(productRequest.CategoriesIds.Count));
        }

        [Fact]
        public async Task GetAllProductsAsync_ValidFilter_ReturnsProductsResponse()
        {
            // Arrange
            var filter = new PaginationFilter(1, 5);
            var mockProducts = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Title = "Product 1" },
            };

            var pagedResult = new PagedResult<Product>
            {
                Items = mockProducts,
                TotalCount = mockProducts.Count
            };

            _mockProductsRepository.Setup(repo => repo.FetchAllAsync(It.IsAny<PaginationFilter>()))
                .ReturnsAsync(pagedResult);

            string imageSrc = "http://example.com/images/";
            string route = "http://example.com/products";

            _mapperMock.Setup(mapper => mapper.Map<List<ProductDto>>(It.IsAny<List<Product>>()))
                .Returns(mockProducts.Select(product => new ProductDto
                {
                    Id = product.Id,
                    Title = product.Title,
                }).ToList());

            // Act
            var response = await _productsService.GetAllProductsAsync(filter, imageSrc, route);

            // Assert
            response.Should().NotBeNull();
            response.PageNumber.Should().Be(filter.PageNumber);
            response.PageSize.Should().Be(filter.PageSize);
            response.TotalRecords.Should().Be(mockProducts.Count);
            response.Products.Should().NotBeNull();
            response.Products.Count.Should().Be(mockProducts.Count);
        }

        [Fact]
        public async Task UpdateProductAsync_WithValidData_UpdatesProductAndRelations()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productUpdate = new ProductUpdate
            {
                ProductId = productId,
                Title = "Updated Product",
                Price = "100",
                Stock = "1",
                CategoriesIds = new List<Guid> { Guid.NewGuid() },
                SubcategoriesIds = new List<Guid> { Guid.NewGuid() },
                AttributesIds = new List<Guid> { Guid.NewGuid() }
            };
            var processedProduct = new Product
            {
                Id = productId,
                Title = productUpdate.Title
            };

            _mockImagesService.Setup(service => service.MapAndProcessObjectListAsync<ProductUpdate, Product>(
                    It.IsAny<ProductUpdate>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<int>>()))
                .ReturnsAsync(processedProduct);

            _mockProductsRepository.Setup(repo => repo.ModifyAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            _mockCategoryRepository.Setup(repo => repo.DeleteRangeProductCategories(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            _mockCategoryRepository.Setup(repo => repo.AddProductCategoriesAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            // Act
            await _productsService.UpdateProductAsync("fakePath", productUpdate);

            // Assert
            _mockImagesService.Verify(service => service.MapAndProcessObjectListAsync<ProductUpdate, Product>(
                It.IsAny<ProductUpdate>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<int>>()), Times.Once);

            _mockProductsRepository.Verify(repo => repo.ModifyAsync(It.Is<Product>(p => p.Id == productId && p.Title == "Updated Product")), Times.Once);

            _mockCategoryRepository.Verify(repo => repo.DeleteRangeProductCategories(productId), Times.Once);
            _mockCategoryRepository.Verify(repo => repo.AddProductCategoriesAsync(It.IsAny<Guid>(), productId), Times.Exactly(productUpdate.CategoriesIds.Count));

        }

        [Fact]
        public async Task RemoveProductsAsync_ValidId_RemovesProduct()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            _mockProductsRepository.Setup(repo => repo.RemoveAsync(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            // Act
            await _productsService.RemoveProductsAsync(productId);

            // Assert
            _mockProductsRepository.Verify(repo => repo.RemoveAsync(It.Is<Guid>(id => id == Guid.Parse(productId))), Times.Once);
        }

        [Fact]
        public async Task RemoveProductsAsync_InvalidId_ThrowsProductException()
        {
            // Arrange
            string? productId = null;

            // Act & Assert
            await Assert.ThrowsAsync<ProductException>(() => _productsService.RemoveProductsAsync(productId));
        }
    }
}