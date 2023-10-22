using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using System.Web;

namespace Nexify.Service.Services
{
    public class UriService : IUriService
    {
        private readonly string _baseUri;
        public UriService(string baseUri)
        {
            _baseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
        }
        public Uri GetPageUri(PaginationFilter filter, string route)
        {
            if (filter == null)
            {
                throw new PaginationException("Pagination filter cannot be null.");
            }

            if (string.IsNullOrEmpty(route))
            {
                throw new PaginationException("Route cannot be null or empty.");
            }

            var uriBuilder = new UriBuilder(_baseUri + route);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["pageNumber"] = filter.PageNumber.ToString();
            query["pageSize"] = filter.PageSize.ToString();
            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri;
        }
    }
}
