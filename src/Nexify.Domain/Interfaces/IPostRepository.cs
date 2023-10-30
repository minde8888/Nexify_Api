
using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Entities.Posts;
using Nexify.Domain.Entities.Products;

namespace Nexify.Domain.Interfaces
{
    public interface IPostRepository
    {
        public Task AddAsync(Post post);
        public Task DeleteAsync(Guid id);
        public Task UpdateAsync(Post post);
        public Task<Post> GetByIdAsync(Guid id);
        public Task<PagedResult<Post>> RetrieveAllAsync(PaginationFilter validFilter);
    }
}
