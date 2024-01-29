using FluentValidation;
using Nexify.Service.Dtos;

namespace Nexify.Service.Validators
{
    public class BlogCategoryResponseValidator : AbstractValidator<BlogCategoryResponse>
    {
        public BlogCategoryResponseValidator()
        {
            RuleFor(category => category.CategoryName)
                .NotEmpty().WithMessage("Category name is required.")
                .Length(2, 100).WithMessage("Category name must be between 2 and 100 characters.");

            RuleFor(category => category.Description)
                .MaximumLength(10000).WithMessage("Description can't be more than 10000 characters.");

            RuleFor(category => category.ImageSrc)
                .Must(BeAValidUrl).WithMessage("Image source must be a valid URL.")
                .When(category => !string.IsNullOrEmpty(category.ImageSrc)); // Apply this rule only if ImageSrc is not null or empty

            RuleFor(category => category.DateCreated)
                .Must(BeAPastDate).WithMessage("Date created must be a past date.");
        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private bool BeAPastDate(DateTime date)
        {
            return date < DateTime.Now;
        }
    }
}
