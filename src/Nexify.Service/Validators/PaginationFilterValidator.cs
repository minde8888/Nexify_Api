using FluentValidation;
using Nexify.Domain.Entities.Pagination;

namespace Nexify.Service.Validators
{
    public class PaginationFilterValidator : AbstractValidator<PaginationFilter>
    {
        public PaginationFilterValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThan(0);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 10);
        }
    }
}
