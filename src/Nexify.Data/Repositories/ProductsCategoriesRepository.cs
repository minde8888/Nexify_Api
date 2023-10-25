
using Microsoft.EntityFrameworkCore;
using Nexify.Data.Context;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Entities.CategoriesProducts;
using Nexify.Domain.Interfaces;

namespace Nexify.Data.Repositories
{
    public class ProductsCategoriesRepository : IProductsCategoriesRepository
    {
        private readonly AppDbContext _context;
        public ProductsCategoriesRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddProductCategoriesAsync(IEnumerable<Category> categories, Guid productId)
        {
            foreach (var cat in categories)
            {
                var category = await _context.Category.SingleOrDefaultAsync(c => c.CategoryName == cat.CategoryName);
                if (category == null)
                {
                    category = new Category { CategoryName = cat.CategoryName };
                    _context.Category.Add(category);
                }

                var categoriesProducts = new CategoriesProducts { ProductsId = productId, CategoriesId = category.CategoryId };
                _context.CategoriesProducts.Add(categoriesProducts);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoriesProductAsync(Guid id, string[] categoryNames)
        {
            var existingCategories = await _context.CategoriesProducts
                .Where(cp => cp.ProductsId == id)
                .ToListAsync();

            _context.CategoriesProducts.RemoveRange(existingCategories);

            foreach (var categoryName in categoryNames)
            {
                var category = new Category
                {
                    CategoryName = categoryName.ToString(),
                };
                _context.Category.Add(category);
                await _context.SaveChangesAsync();

                var categoriesProduct = new CategoriesProducts
                {
                    CategoriesId = category.CategoryId,
                    ProductsId = id,
                };
                _context.CategoriesProducts.Add(categoriesProduct);
            }
        }
    }
}
