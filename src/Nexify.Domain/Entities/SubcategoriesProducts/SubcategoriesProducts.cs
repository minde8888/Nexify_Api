using Nexify.Domain.Entities.Products;
using Nexify.Domain.Entities.Subcategories;

namespace Nexify.Domain.Entities.SubcategoriesProducts
{
    public class SubcategoriesProducts
    {
        public Guid SubcategoriesId { get; set; }
        public Subcategory Subcategories { get; set; }
        public Guid ProductsId { get; set; }
        public Product Products { get; set; }
    }
}
