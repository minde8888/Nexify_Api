using Nexify.Domain.Interfaces;

namespace Nexify.Service.Services
{
    public class ProductSubcategoryUpdater : IProductRelationUpdater
    {
        private readonly IProductSubcategoryRepository _subcategoryRepository;

        public ProductSubcategoryUpdater(IProductSubcategoryRepository subcategoryRepository)
        {
            _subcategoryRepository = subcategoryRepository;
        }

        public async Task DeleteRangeAsync(Guid productId)
        {
            await _subcategoryRepository.DeleteRangeProductSubcategories(productId);
        }

        public async Task AddRelationAsync(Guid subcategoryId, Guid productId)
        {
            await _subcategoryRepository.AddProductSubcategoriesAsync(subcategoryId, productId);
        }
    }
}
