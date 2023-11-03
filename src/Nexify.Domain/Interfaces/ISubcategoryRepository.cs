using Nexify.Domain.Entities.Subcategories;

namespace Nexify.Domain.Interfaces
{
    public interface ISubcategoryRepository
    {
        public Task AddAsync(Subcategory category, Guid productId);
        public Task<Subcategory> GetAsync(Guid id);
        public Task RemoveAsync(Guid id);
        public Task UpdateAsync(Subcategory category);
    }
}
