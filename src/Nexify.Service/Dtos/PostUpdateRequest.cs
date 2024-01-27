using Microsoft.AspNetCore.Http;

namespace Nexify.Service.Dtos
{
    public class PostUpdateRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<IFormFile> Images { get; set; }
        public List<string> ImageNames { get; set; }
        public List<Guid> CategoriesIds { get; set; } = new List<Guid>();
    }
}
