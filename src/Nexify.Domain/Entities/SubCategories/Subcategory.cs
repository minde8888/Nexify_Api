using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Entities.Products;

namespace Nexify.Domain.Entities.Subcategories
{
    public class Subcategory
    {
        public Guid SubcategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public List<Product> Products { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; }
    }
}