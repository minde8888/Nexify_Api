
namespace Nexify.Domain.Entities.Pagination
{
    public class PagedEntityResult<T>
    {
        public T Items { get; set; }
        public int TotalCount { get; set; }
    }
}
