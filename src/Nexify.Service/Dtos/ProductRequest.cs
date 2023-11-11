using Microsoft.AspNetCore.Http;

namespace Nexify.Service.Dtos
{
    public class ProductRequest
    {
        public Guid ProductsId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Discount { get; set; }
        public string Stok { get; set; }
        public List<IFormFile> Images { get; set; }
        public string ImageName { get; set; } = string.Empty;

    }
}
