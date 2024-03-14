using Nexify.Domain.Entities.Categories;
using Nexify.Service.Dtos.Category;

namespace Nexify.Service.Dtos.Blog
{
    public class BlogPostCategoryResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageSrc { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public Uri NextPage { get; set; }
        public Uri PreviousPage { get; set; }
        public List<CategoryPosts> Posts { get; set; }
    }
}
