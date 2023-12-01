using Microsoft.AspNetCore.Http;

namespace Nexify.Service.Dtos
{
    public class CategoryDto
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public string ImageName { get; set; } = string.Empty;
        public Guid ProductsId { get; set; }
        public List<SubcategoryDto> Subcategories { get; set; }
    }
}
