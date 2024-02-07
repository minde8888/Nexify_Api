using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Exceptions;
using Nexify.Service.Services;

namespace Tests.Services
{
    public class UriServiceTests
    {
        private readonly UriService _uriService;
        private const string? BaseUri = "http://example.com/";

        public UriServiceTests()
        {
            _uriService = new UriService(BaseUri);
        }

        [Fact]
        public void GetPageUri_Throws_PaginationNullException_When_Filter_Is_Null()
        {
            // Arrange
            PaginationFilter? filter = null;
            var route = "test-route";

            // Act and Assert
            var ex = Assert.Throws<PaginationException>(() => _uriService.GetPageUri(filter, route));
            Assert.Equal("Pagination filter cannot be null.", ex.Message);
        }

        [Fact]
        public void GetPageUri_Throws_PaginationNullException_When_Route_Is_Null_Or_Empty()
        {
            // Arrange
            var filter = new PaginationFilter();
            string? route = null;

            // Act and Assert
            var ex = Assert.Throws<PaginationException>(() => _uriService.GetPageUri(filter, route));
            Assert.Equal("Route cannot be null or empty.", ex.Message);
        }

        [Fact]
        public void GetPageUri_Returns_Correct_Uri()
        {
            // Arrange
            var filter = new PaginationFilter { PageNumber = 1, PageSize = 10 };
            var route = "test-route";

            // Act
            var uri = _uriService.GetPageUri(filter, route);

            // Assert
            var expectedUri = new Uri($"{BaseUri}{route}?pageNumber={filter.PageNumber}&pageSize={filter.PageSize}");
            Assert.Equal(expectedUri, uri);
        }
    }
}
