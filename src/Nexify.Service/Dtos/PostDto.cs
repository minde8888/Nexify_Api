
namespace Nexify.Service.Dtos
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public ICollection<string> ImageSrc { get; set; }
        public List<string> ImageNames{ get; set; }
        public DateTime DateCreated { get; set; }
        public ICollection<BlogCategoryDto> Categories { get; set; }
    }
}
