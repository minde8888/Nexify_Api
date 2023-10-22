using Nexify.Domain.Entities.Categories;

namespace Nexify.Domain.Interfaces
{
    public interface IProductsCategoriesRepository
    {
        public Task UpdateCategoriesProductAsync(Guid id, string[] categoryNames);
        public Task AddProductCategoriesAsync(IEnumerable<Category> categories, Guid productId);
    }
}
