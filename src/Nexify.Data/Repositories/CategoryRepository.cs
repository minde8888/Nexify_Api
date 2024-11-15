﻿using Microsoft.EntityFrameworkCore;
using Nexify.Data.Context;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Entities.Products;
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

        public async Task AddAsync(Category category)
        {
            _context.Category.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Category>> GetAllAsync() => await _context.Category
            .OrderByDescending(c => c.DateCreated)
            .Include(c => c.Subcategories.OrderBy(s => s.DateCreated))
            .ToListAsync()
            .ConfigureAwait(false);


        public async Task<Category> GetAsync(Guid id) => await _context.Category
                .Include(c => c.Subcategories)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task RemoveAsync(Guid id)
        {
            var category = await _context.Category.FirstOrDefaultAsync(x => x.Id == id);

            category.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveSubcategoryAsync(Guid id)
        {
            var subcategory = await _context.Subcategory.FirstOrDefaultAsync(x => x.SubcategoryId == id);

            subcategory.CategoryId = Guid.Empty;
            subcategory.IsDeleted = true;
            _context.Entry(subcategory).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            var categorySave = await _context.Category
                .FirstOrDefaultAsync(x => x.Id == category.Id);

            _context.Entry(categorySave).CurrentValues.SetValues(category);
            categorySave.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
