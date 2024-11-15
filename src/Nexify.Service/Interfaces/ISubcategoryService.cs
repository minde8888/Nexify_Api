﻿using Nexify.Service.Dtos.Category;

namespace Nexify.Service.Interfaces
{
    public interface ISubcategoryService
    {
         Task AddSubCategoryAsync(List<AddSubcategory> subcategories);
        Task UpdateSubCategoryAsync(SubcategoryDto subcategoryDto, string rootPath);
        Task DeleteSubCategoryAsync(string id);
    }
}
