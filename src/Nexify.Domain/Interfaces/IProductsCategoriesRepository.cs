namespace Nexify.Domain.Interfaces
{
    public interface IProductsCategoriesRepository
    {
        public Task AddProductCategoriesAsync(Guid categoryId, Guid productId);
        public Task DeleteCategoriesProductAsync(Guid id);
    }
}
