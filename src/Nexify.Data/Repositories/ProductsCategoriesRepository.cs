﻿using Microsoft.EntityFrameworkCore;
using Nexify.Data.Context;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Interfaces;

namespace Nexify.Data.Repositories
{
    public class ProductsCategoriesRepository : IProductsCategoriesRepository
    {
        private readonly AppDbContext _context;
        public ProductsCategoriesRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddProductCategoriesAsync(Guid categoryId, Guid productId)
        {
            var categoriesProducts = new CategoriesProducts { ProductsId = productId, CategoriesId = categoryId };
            _context.CategoriesProducts.Add(categoriesProducts);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoriesProductAsync(Guid id)
        {
            var existingCategories = await _context.CategoriesProducts
                .Where(cp => cp.ProductsId == id)
                .ToListAsync();

            _context.CategoriesProducts.RemoveRange(existingCategories);
        }
    }
}