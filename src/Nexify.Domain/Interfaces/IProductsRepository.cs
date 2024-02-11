using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Entities.Products;

namespace Nexify.Domain.Interfaces
{
    public interface IProductsRepository
    {
        public Task AddAsync(Product products);
        public Task<PagedResult<Product>> FetchAllAsync(PaginationFilter validFilter);
        public Task ModifyAsync(Product product);
        public Task RemoveAsync(Guid id);
    }
}
