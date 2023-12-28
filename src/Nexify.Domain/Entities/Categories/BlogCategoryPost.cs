
using Nexify.Domain.Entities.Posts;

namespace Nexify.Domain.Entities.Categories
{
    public class BlogCategoryPost
    {
        public Guid CategoriesId { get; set; }
        public BlogCategory Categories { get; set; }
        public Guid PostId { get; set; }
        public Post Posts { get; set; }
    }
}
