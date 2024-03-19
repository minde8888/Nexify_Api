using Nexify.Service.Dtos.Attributes;
using Nexify.Service.Dtos.Category;

namespace Nexify.Service.Dtos.Product
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Price { get; set; }
        public string Discount { get; set; }
        public string Stock { get; set; }
        public string Location { get; set; }
        public List<string> ImageSrc { get; set; } = new List<string>();
        public DateTime DateCreated { get; set; }
        public ICollection<CategoriesId> Categories { get; set; }
        public ICollection<SubcategoriesId> Subcategories { get; set; }
        public ICollection<AttributesId> Attributes { get; set; }
    }
}
