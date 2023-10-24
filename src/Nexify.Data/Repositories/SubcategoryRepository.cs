using Microsoft.EntityFrameworkCore;
using Nexify.Data.Context;
using Nexify.Domain.Entities.CategoriesProducts;
using Nexify.Domain.Entities.Products;
using Nexify.Domain.Entities.SubCategories;
using Nexify.Domain.Interfaces;

namespace Nexify.Data.Repositories
{
    public class SubcategoryRepository : ISubcategoryRepository
    {
        private readonly AppDbContext _context;

        public SubcategoryRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(Subcategory subcategory, Guid productId)
        {
            _context.Subcategory.Add(subcategory);
            await _context.SaveChangesAsync();

            var categoriesProducts = new CategoriesProducts
            {
                ProductsId = productId,
                CategoriesId = subcategory.SubCategoryId
            };
            _context.CategoriesProducts.Add(categoriesProducts);
            await _context.SaveChangesAsync();
        }

        public async Task<Subcategory> GetAsync(Guid id)
        {
            return await _context.Subcategory
               .Include(c => c.Products)
               .FirstOrDefaultAsync(x => x.SubCategoryId == id);
        }

        public async Task RemoveAsync(Guid id)
        {
            var currentProduct = await _context.Subcategory
            .FirstOrDefaultAsync(p => p.SubCategoryId == id);

            _context.Remove(_context.Subcategory.FirstOrDefaultAsync(p => p.SubCategoryId == id));
            _context.SaveChanges();
        }

        public async Task UpdateAsync(Subcategory subcategory)
        {
            var currentProduct = await _context.Subcategory
                .FirstOrDefaultAsync(p => p.SubCategoryId == subcategory.SubCategoryId);

            currentProduct.Description = subcategory.Description;
            currentProduct.SubCategoryName = subcategory.Description;
            currentProduct.ImageName = subcategory.ImageName;
            currentProduct.DateUpdated = DateTime.UtcNow;

            _context.Entry(currentProduct).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
