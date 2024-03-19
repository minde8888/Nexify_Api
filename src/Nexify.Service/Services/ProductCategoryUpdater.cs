using Nexify.Domain.Interfaces;

namespace Nexify.Service.Services
{
    public class ProductCategoryUpdater : IProductRelationUpdater
    {
        private readonly IProductCategoryRepository _categoriesRepository;

        public ProductCategoryUpdater(IProductCategoryRepository categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
        }

        public async Task DeleteRangeAsync(Guid productId)
        {
            await _categoriesRepository.DeleteRangeProductCategories(productId);
        }

        public async Task AddRelationAsync(Guid categoryId, Guid productId)
        {
            await _categoriesRepository.AddProductCategoriesAsync(categoryId, productId);
        }
    }

}
