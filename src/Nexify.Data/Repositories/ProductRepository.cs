using Microsoft.EntityFrameworkCore;
using Nexify.Data.Context;
using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Entities.Products;
using Nexify.Domain.Interfaces;

namespace Nexify.Data.Repositories
{
    public class ProductRepository : IProductsRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(Product products)
        {
            _context.Product.Add(products);
            await _context.SaveChangesAsync();
        }

        public async Task AddProductSubcategoriesAsync(Guid productId, Guid subcategoryId)
        {
            var product = await _context.Product
                .FirstOrDefaultAsync(p => p.ProductId == productId);
            product.SubcategoriesId = subcategoryId;

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<Product>> FetchAllAsync(PaginationFilter validFilter)
        {
            var pagedData = await _context.Product
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToListAsync();

            var totalCount = await _context.Product.CountAsync();

            return new PagedResult<Product> { Items = pagedData, TotalCount = totalCount };
        }

        public async Task<Product> RetrieveAsync(Guid id)
        {
            return await _context.Product.
                Include(c => c.Categories).
                Where(x => x.ProductId == id).FirstOrDefaultAsync();
        }

        public async Task ModifyAsync(Product product)
        {
            var currentProduct = await _context.Product
                .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

            currentProduct.Title = product.Title;
            currentProduct.Description = product.Description;
            currentProduct.ImageName = product.ImageName;
            currentProduct.Price = product.Price;
            currentProduct.DateUpdated = DateTime.UtcNow;

            _context.Entry(currentProduct).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Guid id)
        {
            var product = await _context.Product.
                Where(x => x.ProductId == id).FirstOrDefaultAsync();

            product.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSubcategoriesProductAsync(Guid productId, Guid subcategoryId)
        {
            var product = await _context.Product
              .FirstOrDefaultAsync(p => p.ProductId == productId);
            product.SubcategoriesId = Guid.Empty;

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
