using Microsoft.EntityFrameworkCore;
using Nexify.Data.Context;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Interfaces;


namespace Nexify.Data.Repositories
{
    public class BlogCategoryRepository : IBlogCategoryRepository
    {
        private readonly AppDbContext _context;

        public BlogCategoryRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(BlogCategory category)
        {
            category.DateCreated = DateTime.UtcNow;
            _context.BlogCategory.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task<List<BlogCategory>> GetAllAsync() => 
            await _context.BlogCategory.OrderByDescending(p => p.DateCreated).ToListAsync();

        public async Task RemoveAsync(Guid id)
        {
            var category = await _context.BlogCategory.FirstOrDefaultAsync(x => x.Id == id);

            var relatedPosts = await _context.BlogCategoryPost
                .FirstOrDefaultAsync(x => x.CategoriesId == category.Id);

            _context.BlogCategoryPost.Remove(relatedPosts);

            category.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BlogCategory category)
        {
            var categorySave = await _context.BlogCategory
                .FirstOrDefaultAsync(x => x.Id == category.Id);
            categorySave.CategoryName = category.CategoryName;
            categorySave.Description = category.Description;
            categorySave.ImageName = category.ImageName;
            categorySave.DateUpdated = DateTime.UtcNow;

            _context.Entry(categorySave).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
