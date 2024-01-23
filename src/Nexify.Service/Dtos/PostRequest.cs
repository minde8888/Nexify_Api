using Microsoft.AspNetCore.Http;

namespace Nexify.Service.Dtos
{
    public class PostRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<IFormFile> Images { get; set; }
        public string ImageName { get; set; } = string.Empty;
        public List<Guid> CategoriesIds { get; set; } = new List<Guid>();
    }
}
