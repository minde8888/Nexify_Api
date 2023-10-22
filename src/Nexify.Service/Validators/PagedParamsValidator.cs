using FluentValidation;
using Nexify.Service.Interfaces;

namespace Nexify.Service.Validators
{
    public class PagedParamsValidator<T> : AbstractValidator<PagedParams<T>>
    {
        public PagedParamsValidator()
        {
            RuleFor(p => p.PagedData).NotNull().NotEmpty();
            RuleFor(p => p.ValidFilter).NotNull();
            RuleFor(p => p.TotalRecords).GreaterThan(0);
            RuleFor(p => p.UriService).NotNull();
            RuleFor(p => p.Route).NotNull().NotEmpty();
        }
    }
}
