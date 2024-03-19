using Nexify.Domain.Interfaces;

namespace Nexify.Service.Services
{
    public class ProductAttributeUpdater : IProductRelationUpdater
    {
        private readonly IProductCategoryRepository _categoriesRepository;

        public ProductAttributeUpdater(IProductCategoryRepository categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
        }
        public async Task DeleteRangeAsync(Guid productId)
        {
            await _categoriesRepository.DeleteRangeProductAttribute(productId);
        }

        public async Task AddRelationAsync(Guid categoryId, Guid productId)
        {
            await _categoriesRepository.AddProductAttributes(categoryId, productId);
        }
    }
}
