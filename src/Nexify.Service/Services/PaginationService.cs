using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Exceptions;
using Nexify.Service.Validators;
using Nexify.Service.Interfaces;
using Nexify.Data.Helpers;

namespace Nexify.Service.Services
{
    public static class PaginationService
    {
        public static PagedResponse<List<T>> CreatePagedResponse<T>(PagedParams<T> pageParams)
        {
            var validator = new PagedParamsValidator<T>();
            var validationResult = validator.Validate(pageParams);

            if (!validationResult.IsValid &&
                !(pageParams.PagedData == null || !pageParams.PagedData.Any()) &&
                pageParams.TotalRecords > 0)
            {
                ValidationExceptionHelper.ThrowIfInvalid<PagedResponseException>(validationResult);
            }

            var validFilter = pageParams.ValidFilter;
            var pagedData = pageParams.PagedData;
            var totalRecords = pageParams.TotalRecords;
            var uriService = pageParams.UriService;
            var route = pageParams.Route;

            var totalPages = (int)Math.Ceiling((double)totalRecords / validFilter.PageSize);

            var response = new PagedResponse<List<T>>(pagedData, validFilter.PageNumber, validFilter.PageSize, totalPages)
            {
                FirstPage = uriService.GetPageUri(new PaginationFilter(1, validFilter.PageSize), route),
                LastPage = uriService.GetPageUri(new PaginationFilter(totalPages, validFilter.PageSize), route)
            };

            if (validFilter.PageNumber > 1)
            {
                response.PreviousPage = uriService.GetPageUri(new PaginationFilter(validFilter.PageNumber - 1, validFilter.PageSize), route);
            }

            if (validFilter.PageNumber < totalPages)
            {
                response.NextPage = uriService.GetPageUri(new PaginationFilter(validFilter.PageNumber + 1, validFilter.PageSize), route);
            }
            response.TotalPages = totalPages;
            response.TotalRecords = totalRecords;

            return response;
        }
    }
}
