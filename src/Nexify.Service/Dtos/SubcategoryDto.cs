using Microsoft.AspNetCore.Http;

namespace Nexify.Service.Dtos
{
    public class SubcategoryDto
    {
        public Guid SubcategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public string ImageName { get; set; } = string.Empty;
        public Guid ProductsId { get; set; }
    }
}
