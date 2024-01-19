using Nexify.Domain.Entities.Subcategories;

namespace Nexify.Domain.Interfaces
{
    public interface ISubcategoryRepository
    {
        public Task AddAsync(Subcategory category);
        public Task RemoveAsync(Guid id);
        public Task UpdateAsync(Subcategory category);
    }
}
