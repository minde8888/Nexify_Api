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

            _context.Entry(reesult).CurrentValues.SetValues(subcategory);
            reesult.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
