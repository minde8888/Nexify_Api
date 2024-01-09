using Microsoft.EntityFrameworkCore;
using Nexify.Data.Context;
using Nexify.Domain.Entities.Subcategories;
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

        public async Task AddAsync(Subcategory subcategory)
        {
            _context.Subcategory.Add(subcategory);
            await _context.SaveChangesAsync();
        }

        public async Task<Subcategory> GetAsync(Guid id)
        {
            return await _context.Subcategory
               .Include(c => c.Products)
               .FirstOrDefaultAsync(x => x.SubcategoryId == id);
        }

        public async Task RemoveAsync(Guid id)
        {
            var currentProduct = await _context.Subcategory
            .FirstOrDefaultAsync(p => p.SubcategoryId == id);

            currentProduct.IsDeleted = true;
            _context.SaveChanges();
        }

        public async Task UpdateAsync(Subcategory subcategory)
        {
            var currentProduct = await _context.Subcategory
                .FirstOrDefaultAsync(p => p.SubcategoryId == subcategory.SubcategoryId);

            currentProduct.Description = subcategory.Description;
            currentProduct.SubCategoryName = subcategory.SubCategoryName;
            currentProduct.ImageName = subcategory.ImageName;
            currentProduct.DateUpdated = DateTime.UtcNow;

            _context.Entry(currentProduct).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
