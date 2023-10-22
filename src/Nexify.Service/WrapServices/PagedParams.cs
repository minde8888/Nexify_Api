using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Interfaces;

namespace Nexify.Service.Interfaces
{
    public class PagedParams<T>
    {
        public PagedParams(List<T> pagedData,
            PaginationFilter validFilter,
            int totalRecords,
            IUriService uriService,
            string route)
        {
            PagedData = pagedData;
            ValidFilter = validFilter;
            UriService = uriService;
            Route = route;
            TotalRecords = totalRecords;
        }

        public List<T> PagedData { get; private set; }
        public PaginationFilter ValidFilter { get; private set; }
        public int TotalRecords { get; private set; }
        public IUriService UriService { get; private set; }
        public string Route { get; private set; }
    }
}
