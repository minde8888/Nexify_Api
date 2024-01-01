
namespace Nexify.Domain.Interfaces
{
    public interface IItemCategoryRepository
    {
        public Task AddItemCategoriesAsync(Guid? categoryId, Guid productId);
        public Task DeleteCategoriesItemAsync(Guid id);
    }
}
