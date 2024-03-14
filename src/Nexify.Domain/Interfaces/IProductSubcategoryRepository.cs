namespace Nexify.Domain.Interfaces
{
    public interface IProductSubcategoryRepository
    {
        Task AddProductSubcategoriesAsync(Guid subcategoriesId, Guid productsId);
        Task DeleteRangeProductSubcategories(Guid productsId);
        Task DeleteSubcategoriesProductAsync(Guid id);
    }
}
