using Microsoft.AspNetCore.Http;

namespace Nexify.Service.Dtos.Blog
{
    public class BlogCategoryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<IFormFile> Images { get; set; }
        public string ImageName { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
