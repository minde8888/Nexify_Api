using Microsoft.AspNetCore.Http;

namespace Nexify.Service.Dtos
{
    public class SubcategoryDto
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public List<IFormFile> Images { get; set; }
        public string ImageName { get; set; } = string.Empty;
        public Guid ProductsId { get; set; }
    }
}
