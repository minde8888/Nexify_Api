using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Entities.Pagination;

namespace Nexify.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        public Task AddAsync(Category category);
        public Task<List<Category>> GetAllAsync();
        public Task<Category> GetAsync(Guid id);
        public Task RemoveAsync(Guid id);
        public Task RemoveSubcategoryAsync(Guid id);
        public Task UpdateAsync(Category category);
    }
}
