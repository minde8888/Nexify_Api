using Microsoft.AspNetCore.Http;

namespace Nexify.Service.Dtos
{
    public class ProductRequest
    {
        public Guid ProductsId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Price { get; set; }
        public string Discount { get; set; }
        public string Stock { get; set; }
        public List<IFormFile> Images { get; set; }
        public List<string> ImagesNames { get; set; }
        public List<IFormFile> ItemsImages { get; set; }
        public List<string> ItemsNames { get; set; }
        public List<Guid> CategoriesIds { get; set; }
        public List<Guid> SubcategoriesIds { get; set; }
    }
}
