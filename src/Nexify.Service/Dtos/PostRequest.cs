using Microsoft.AspNetCore.Http;

namespace Nexify.Service.Dtos
{
    public class PostRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public List<IFormFile> Images { get; set; }
        public string ImageName { get; set; } = string.Empty;
        public List<Guid> CategoryId { get; set; } = new List<Guid>();
    }
}
