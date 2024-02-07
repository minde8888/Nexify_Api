using Moq;
using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Interfaces;
using Nexify.Service.Interfaces;
using Nexify.Service.Services;

namespace Tests.Services
{
    public class PaginationServiceTests
    {
        [Fact]
        public void CreatePagedResponse_ValidPageParams_ReturnsPageResponse()
        {
            // Arrange
            var pagedData = new List<int> { 1, 2, 3 };
            var validFilter = new PaginationFilter(1, 10);
            var totalRecords = 3;
            var uriServiceMock = new Mock<IUriService>();
            uriServiceMock.Setup(x => x.GetPageUri(It.IsAny<PaginationFilter>(), It.IsAny<string>()))
                          .Returns(new Uri("http://localhost/page/1"));
            var uriService = uriServiceMock.Object;
            var route = "products";

            var pageParams = new PagedParams<int>(pagedData, validFilter, totalRecords, uriService, route);

            // Act
            var result = PaginationService.CreatePagedResponse(pageParams);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pagedData, result.Data);
            Assert.True(result.Succeeded);
            Assert.Equal(10, result.PageSize);
            Assert.Equal("http://localhost/page/1", result.FirstPage.AbsoluteUri);
            Assert.Equal("http://localhost/page/1", result.LastPage.AbsoluteUri);
            Assert.Null(result.PreviousPage);
            Assert.Null(result.NextPage);
        }
    }
}
