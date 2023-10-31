using Nexify.Domain.Entities.Categories;

namespace Nexify.Service.Dtos
{
    public class PostDto
    {
        public Guid PostId { get; set; }
        public string Title { get; set; }
        public string Context { get; set; }
        public ICollection<string> ImageSrc { get; set; }
        public string ImageName { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}
