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

        public async Task<PagedResult<Product>> FetchAllAsync(PaginationFilter validFilter)
        {
            var pagedData = await _context.Product
                .Include(p => p.Categories)
                .ThenInclude(p => p.Subcategories)
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToListAsync();

            var totalCount = await _context.Product.CountAsync();

            return new PagedResult<Product> { Items = pagedData, TotalCount = totalCount };
        }

        public async Task ModifyAsync(Product product)
        {
            var currentProduct = await _context.Product
                .FirstOrDefaultAsync(p => p.Id == product.Id);

            _context.Entry(currentProduct).CurrentValues.SetValues(product);
            currentProduct.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Guid id)
        {
            var product = await _context.Product.
                Where(x => x.Id == id).FirstOrDefaultAsync();

            product.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }
}
