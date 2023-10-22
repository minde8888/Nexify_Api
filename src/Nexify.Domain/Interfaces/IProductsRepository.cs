using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Entities.Products;

namespace Nexify.Domain.Interfaces
{
    public interface IProductsRepository
    {
        public Task AddProductsAsync(Product products);
        public Task<PagedResult<Product>> GetAllAsync(PaginationFilter validFilter);
        public Task UpdateAsync(Product product);
        public Task<Product> GetAsync(Guid id);
        public Task RemoveAsync(Guid id);
    }
}
