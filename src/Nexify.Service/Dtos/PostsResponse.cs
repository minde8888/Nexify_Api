
namespace Nexify.Service.Dtos
{
    public class PostsResponse
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public Uri NextPage { get; set; }
        public Uri PreviousPage { get; set; }
        public List<PostDto> Post { get; set; }
    }
}
