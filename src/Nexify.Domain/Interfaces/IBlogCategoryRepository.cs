using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Entities.Pagination;

namespace Nexify.Domain.Interfaces
{
    public interface IBlogCategoryRepository
    {
        public Task AddAsync(BlogCategory category);
        public Task<List<BlogCategory>> GetAllAsync();
        public Task RemoveAsync(Guid id);
        public Task UpdateAsync(BlogCategory category);
    }
}
