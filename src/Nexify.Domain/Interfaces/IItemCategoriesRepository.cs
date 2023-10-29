namespace Nexify.Domain.Interfaces
{
    public interface IItemCategoriesRepository
    {
        public Task AddItemCategoriesAsync(Guid categoryId, Guid productId);
        public Task DeleteCategoriesItemAsync(Guid id);
    }
}
