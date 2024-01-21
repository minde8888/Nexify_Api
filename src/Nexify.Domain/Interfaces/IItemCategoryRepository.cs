
namespace Nexify.Domain.Interfaces
{
    public interface IItemCategoryRepository
    {
        public Task AddPostCategoriesAsync(Guid? categoryId, Guid productId);
        public Task DeleteCategoriesItemAsync(Guid id);
    }
}
