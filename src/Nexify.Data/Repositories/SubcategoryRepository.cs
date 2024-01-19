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

        public async Task RemoveAsync(Guid id)
        {
            var reesult = await _context.Subcategory
            .FirstOrDefaultAsync(p => p.SubcategoryId == id);

            reesult.IsDeleted = true;
            _context.SaveChanges();
        }

        public async Task UpdateAsync(Subcategory subcategory)
        {
            var reesult = await _context.Subcategory
                .FirstOrDefaultAsync(p => p.SubcategoryId == subcategory.SubcategoryId);

            reesult.Description = subcategory.Description;
            reesult.SubCategoryName = subcategory.SubCategoryName;
            reesult.ImageName = subcategory.ImageName;
            reesult.DateUpdated = DateTime.UtcNow;

            _context.Entry(reesult).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
