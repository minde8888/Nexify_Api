using Nexify.Domain.Entities.Products;

namespace Nexify.Domain.Entities.Subcategories
{
    public class SubcategoriesProducts
    {
        public Guid SubcategoriesId { get; set; }
        public Subcategory Subcategories { get; set; }
        public Guid ProductsId { get; set; }
        public Product Products { get; set; }
    }
}
