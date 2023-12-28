
namespace Nexify.Service.Dtos
{
    public class CategoryPosts
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> ImageSrc { get; set; }
    }
}
