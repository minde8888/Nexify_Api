
namespace Nexify.Domain.Interfaces
{
    public interface IPostCategoryRepository
    {
        public Task AddPostCategoriesAsync(Guid categoryId, Guid productId);
        public Task DeleteCategoriesPostAsync(Guid id);
    }
}
