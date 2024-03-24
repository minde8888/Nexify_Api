
using AutoMapper;
using FluentAssertions;
using Moq;
using Nexify.Domain.Entities.Attributes;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Dtos.Attributes;
using Nexify.Service.Dtos.Product;
using Nexify.Service.Services;

namespace Tests.Services
{
    public class AttributesServicesTests
    {
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<IImagesService> _imagesServiceMock = new Mock<IImagesService>();
        private readonly Mock<IAttributesRepository> _attributesRepositoryMock = new Mock<IAttributesRepository>();
        private readonly AttributesServices _attributesServices;

        public AttributesServicesTests()
        {
            _attributesServices = new AttributesServices(_imagesServiceMock.Object, _attributesRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task AddAttributesAsync_ValidAttributes_AddsSuccessfully()
        {
            // Arrange
            var attributesRequestList = new List<AttributesRequest>
            {
                new AttributesRequest {
                    AttributeName = "AttributeName"
                }
            };

            // Act
            await _attributesServices.AddAttributesAsync(attributesRequestList);

            // Assert
            _attributesRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ItemsAttributes>()), Times.Exactly(attributesRequestList.Count));
        }

        [Fact]
        public async Task GetAllAddAttributesAsync_ReturnsAttributes()
        {
            // Arrange
            var imageSrc = "http://example.com/image/";
            var itemsAttributesList = new List<ItemsAttributes> {
                new ItemsAttributes()
                {
                      AttributeName = "AttributeName"
                }
            };

            _attributesRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(itemsAttributesList);

            _mapperMock.Setup(mapper => mapper.Map<List<ItemsAttributes>>(It.IsAny<List<ItemsAttributes>>()))
                .Returns(itemsAttributesList);

            // Act
            var result = await _attributesServices.GetAllAddAttributesAsync(imageSrc);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(itemsAttributesList.Count);
        }

        [Fact]
        public async Task UpdateAttributesAsync_WhenCalled_UpdatesAttributeSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            var attributeUpdate = new AttributesUpdate
            {
                Id = id,
                AttributeName = "Updated Attribute Name"
            };

            var mappedAttribute = new ItemsAttributes
            {
                Id = id,
                AttributeName = attributeUpdate.AttributeName
            };

            _mapperMock.Setup(m => m.Map<ItemsAttributes>(It.IsAny<AttributesUpdate>()))
                       .Returns(mappedAttribute);

            // Act
            await _attributesServices.UpdateAttributesAsync(attributeUpdate);

            // Assert
            _attributesRepositoryMock.Verify(x => x.ModifyAsync(It.Is<ItemsAttributes>(a =>
                a.Id == id && a.AttributeName == attributeUpdate.AttributeName)),
                Times.Once, "The attribute should be updated exactly once with the expected values.");
        }

        [Fact]
        public async Task RemovePostAsync_ValidId_RemovesSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();

            // Act
            await _attributesServices.RemovePostAsync(id);

            // Assert
            _attributesRepositoryMock.Verify(x => x.RemoveAsync(It.Is<Guid>(g => g.ToString() == id)), Times.Once);
        }

        [Fact]
        public async Task RemovePostAsync_InvalidId_ThrowsException()
        {
            // Arrange
            string? id = null;

            // Act & Assert
            await Assert.ThrowsAsync<AttributesException>(() => _attributesServices.RemovePostAsync(id));
        }

    }
}
