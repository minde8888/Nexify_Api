
using Microsoft.AspNetCore.Http;

namespace Nexify.Domain.Entities.Products
{
    public class ProductUpdate
    {
        public Guid ProductId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Price { get; set; }
        public string Discount { get; set; }
        public string Size { get; set; }
        public string Stock { get; set; }
        public string Location { get; set; }
        public List<IFormFile> Images { get; set; }
        public List<string> ImagesNames { get; set; }
        public List<IFormFile> ItemsImages { get; set; }
        public List<string> ItemsNames { get; set; }
        public List<Guid> CategoriesIds { get; set; } = new List<Guid>();
        public List<Guid> SubcategoriesIds { get; set; } = new List<Guid>();
    }
}
