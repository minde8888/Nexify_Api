using Microsoft.EntityFrameworkCore;
using Nexify.Data.Context;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Interfaces;

namespace Nexify.Data.Repositories
{
    public class PostCategoryRepository : IPostCategoryRepository
    {
        private readonly AppDbContext _context;
        public PostCategoryRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddPostCategoriesAsync(Guid categoryId, Guid postId)
        {
            var existingEntry = await _context.BlogCategoryPost
                .FirstOrDefaultAsync(bcp => bcp.PostId == postId && bcp.CategoriesId == categoryId);

            if (existingEntry == null)
            {
                var categoriesProducts = new BlogCategoryPost { PostId = postId, CategoriesId = categoryId };
                _context.BlogCategoryPost.Add(categoriesProducts);

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteCategoriesPostAsync(Guid id)
        {
            var existingCategories = await _context.CategoriesProducts
                .Where(cp => cp.ProductsId == id)
                .ToListAsync();

            _context.CategoriesProducts.RemoveRange(existingCategories);
        }
    }
}
