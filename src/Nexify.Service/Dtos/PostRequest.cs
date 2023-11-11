
using Microsoft.AspNetCore.Http;

namespace Nexify.Service.Dtos
{
    public class PostRequest
    {
        public Guid PostId { get; set; }

        public string Title { get; set; }

        public string Context { get; set; }

        public List<IFormFile> Images { get; set; }

        public string ImageName { get; set; } = string.Empty;
    }
}
