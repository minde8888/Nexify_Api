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
        public string ImageName { get; set; }
        public List<Guid> CategoriesIds { get; set; }

    }
}
