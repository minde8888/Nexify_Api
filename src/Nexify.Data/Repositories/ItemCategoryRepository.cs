using Microsoft.EntityFrameworkCore;
using Nexify.Data.Context;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Interfaces;

namespace Nexify.Data.Repositories
{
    public class ItemCategoryRepository : IItemCategoryRepository
    {
        private readonly AppDbContext _context;
        public ItemCategoryRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddItemCategoriesAsync(Guid? categoryId, Guid postId)
        {
            var categoriesProducts = new BlogCategoryPost { PostId = postId, CategoriesId = (Guid)categoryId };
            _context.BlogCategoryPost.Add(categoriesProducts);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoriesItemAsync(Guid id)
        {
            var existingCategories = await _context.CategoriesProducts
                .Where(cp => cp.ProductsId == id)
                .ToListAsync();

            _context.CategoriesProducts.RemoveRange(existingCategories);
        }
    }
}
