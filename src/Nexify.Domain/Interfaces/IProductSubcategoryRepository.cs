namespace Nexify.Domain.Interfaces
{
    public interface IProductSubcategoryRepository
    {
        Task AddProductSubcategoriesAsync(Guid subcategoriesId, Guid postId);
        Task DeleteRangeProductSubcategories(Guid postId);
        Task DeleteSubcategoriesProductAsync(Guid id);
    }
}
