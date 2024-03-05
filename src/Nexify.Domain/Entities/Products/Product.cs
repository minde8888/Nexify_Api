using Nexify.Domain.Entities.Attributes;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Entities.Subcategories;

namespace Nexify.Domain.Entities.Products
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Price { get; set; }
        public string Discount { get; set; }
        public string Size { get; set; }
        public string Stock { get; set; }
        public string Location { get; set; }
        public List<string> ImagesNames { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<Subcategory> Subcategories { get; set; }
        public ICollection<ProductAttribute> ProductAttribute { get; set; }
    }
}
