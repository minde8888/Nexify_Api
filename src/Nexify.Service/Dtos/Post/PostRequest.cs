using Microsoft.AspNetCore.Http;

namespace Nexify.Service.Dtos.Post
{
    public class PostRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<IFormFile> Images { get; set; }
        public List<string> ImageNames { get; set; }
        public List<Guid> CategoriesIds { get; set; }
    }

}
