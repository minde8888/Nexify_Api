﻿using Microsoft.EntityFrameworkCore;
using Nexify.Data.Context;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Interfaces;

namespace Nexify.Data.Repositories
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly AppDbContext _context;
        public ProductCategoryRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddProductCategoriesAsync(Guid categoryId, Guid postId)
        {
            var existingEntry = await _context.CategoriesProducts
                .FirstOrDefaultAsync(bcp => bcp.ProductsId == postId && bcp.CategoriesId == categoryId);

            if (existingEntry == null)
            {
                var categoriesProducts = new CategoriesProducts { ProductsId = postId, CategoriesId = categoryId };
                _context.CategoriesProducts.Add(categoriesProducts);

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteRangeProductCategories(Guid postId)
        {
            var entitiesToRemove = await _context.CategoriesProducts
                                                 .Where(bc => bc.ProductsId == postId)
                                                 .ToListAsync();

            _context.CategoriesProducts.RemoveRange(entitiesToRemove);

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
