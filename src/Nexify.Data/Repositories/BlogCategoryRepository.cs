using Microsoft.EntityFrameworkCore;
using Nexify.Data.Context;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Entities.Pagination;
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
            _context.BlogCategory.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task<List<BlogCategory>> GetAllAsync()
        {
            return await _context.BlogCategory
                .ToListAsync();
        }
        public async Task<PagedEntityResult<BlogCategory>> GetAsync(Guid id, PaginationFilter validFilter)
        {
            var category = await _context.BlogCategory
                .Include(c => c.Posts)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (category != null && category.Posts != null)
            {
                var totalCount = category.Posts.Count;

                var pagedProducts = category.Posts
                    .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                    .Take(validFilter.PageSize);

                return new PagedEntityResult<BlogCategory>
                {
                    Items = category,
                    TotalCount = totalCount
                };
            }

            return new PagedEntityResult<BlogCategory> { Items = new BlogCategory(), TotalCount = 0 };
        }

        public async Task RemoveAsync(Guid id)
        {
            var category = await _context.BlogCategory.FirstOrDefaultAsync(x => x.Id == id);

            var categoriesProducts = await _context.CategoriesProducts.FirstOrDefaultAsync(x => x.CategoriesId == id);
            _context.CategoriesProducts.Remove(categoriesProducts);

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

            _context.Entry(categorySave).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
