﻿using Nexify.Domain.Entities.Products;

namespace Nexify.Domain.Entities.Categories
{
    public class CategoriesProducts
    {
        public Guid CategoriesId { get; set; }
        public Category Categories { get; set; }
        public Guid ProductsId { get; set; }
        public Product Products { get; set; }
    }
}
