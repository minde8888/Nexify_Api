using Nexify.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexify.Domain.Entities.Pagination
{
    public class PagedResponse<T> : Response<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? NePage { get; set; }
        public int? PrevPage { get; set; }
        public Uri FirstPage { get; set; }
        public Uri LastPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public Uri NextPage { get; set; }
        public Uri PreviousPage { get; set; }
        public PagedResponse(T data, int pageNumber, int pageSize, int totalPages)
        {
            if (pageNumber <= 0)
                throw new PaginationException("Page number must be greater than zero.");

            if (pageSize <= 0)
                throw new PaginationException("Page number must be greater than zero.");

            if (totalPages <= 0)
                throw new PaginationException("Total page number must be greater than zero.");

            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.Data = data;
            this.Message = null;
            this.Succeeded = true;
            this.Errors = null;
        }
    }
}
