using Nexify.Domain.Entities.Pagination;

namespace Nexify.Domain.Interfaces
{
    public interface IUriService
    {
        public Uri GetPageUri(PaginationFilter filter, string route);
    }
}
