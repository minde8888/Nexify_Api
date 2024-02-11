using Microsoft.EntityFrameworkCore;
using Nexify.Data.Context;
using Nexify.Domain.Entities.Subcategories;
using Nexify.Domain.Interfaces;

namespace Nexify.Data.Repositories
{
    public class ProductSubcategoryRepository : IProductSubcategoryRepository
    {
        private readonly AppDbContext _context;
        public ProductSubcategoryRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddProductSubcategoriesAsync(Guid subcategoriesId, Guid postId)
        {
            var existingEntry = await _context.SubcategoriesProducts
                .FirstOrDefaultAsync(bcp => bcp.ProductsId == postId && bcp.SubcategoriesId == subcategoriesId);

            if (existingEntry == null)
            {
                var categoriesProducts = new SubcategoriesProducts { ProductsId = postId, SubcategoriesId = subcategoriesId };
                _context.SubcategoriesProducts.Add(categoriesProducts);

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteRangeProductSubcategories(Guid postId)
        {
            var entitiesToRemove = await _context.SubcategoriesProducts
                                                 .Where(bc => bc.ProductsId == postId)
                                                 .ToListAsync();

            _context.SubcategoriesProducts.RemoveRange(entitiesToRemove);

            await _context.SaveChangesAsync();
        }


        public async Task DeleteSubcategoriesProductAsync(Guid id)
        {
            var existingCategories = await _context.SubcategoriesProducts
                .Where(cp => cp.ProductsId == id)
                .ToListAsync();

            _context.SubcategoriesProducts.RemoveRange(existingCategories);
        }
    }
}
