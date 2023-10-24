using Microsoft.EntityFrameworkCore;
using Nexify.Data.Context;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Entities.CategoriesProducts;
using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Interfaces;

namespace Nexify.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(Category category, Guid productId)
        {
            _context.Category.Add(category);
            await _context.SaveChangesAsync();

            var categoriesProducts = new CategoriesProducts
            {
                ProductsId = productId,
                CategoriesId = category.CategoryId
            };
            _context.CategoriesProducts.Add(categoriesProducts);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Category.ToListAsync();
        }

        public async Task<PagedEntityResult<Category>> GetAsync(Guid id, PaginationFilter validFilter)
        {
            var category = await _context.Category
                .Include(c => c.Products)
                .FirstOrDefaultAsync(x => x.CategoryId == id);

            if (category != null && category.Products != null)
            {
                var totalCount = category.Products.Count;

                var pagedProducts = category.Products
                    .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                    .Take(validFilter.PageSize);

                return new PagedEntityResult<Category>
                {
                    Items = category,
                    TotalCount = totalCount
                };
            }

            return new PagedEntityResult<Category> { Items = new Category(), TotalCount = 0 };
        }

        public async Task RemoveAsync(Guid id)
        {
            var category = await _context.Category.FirstOrDefaultAsync(x => x.CategoryId == id);

            var categoriesProducts = await _context.CategoriesProducts.FirstOrDefaultAsync(x => x.CategoriesId == id);
            _context.CategoriesProducts.Remove(categoriesProducts);

            category.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            var categorySave = await _context.Category
                .FirstOrDefaultAsync(x => x.CategoryId == category.CategoryId);
            categorySave.CategoryName = category.CategoryName;
            categorySave.Description = category.Description;
            categorySave.ImageName = category.ImageName;

            _context.Entry(categorySave).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
