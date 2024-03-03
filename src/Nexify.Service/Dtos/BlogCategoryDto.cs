using Microsoft.AspNetCore.Http;

namespace Nexify.Service.Dtos
{
    public class BlogCategoryDto
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public List<IFormFile> Images { get; set; }
        public string ImageName { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
