using Nexify.Domain.Entities.Categories;

namespace Nexify.Domain.Entities.Posts
{
    public class Post
    {
        public Guid PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}
