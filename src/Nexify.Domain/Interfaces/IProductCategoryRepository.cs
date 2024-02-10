namespace Nexify.Domain.Interfaces
{
    public interface IProductCategoryRepository
    {
        Task AddProductCategoriesAsync(Guid categoryId, Guid postId);
        Task DeleteRangeProductCategories(Guid postId);
        Task DeleteCategoriesProductAsync(Guid id);
    }
}
