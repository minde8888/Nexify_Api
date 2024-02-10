using FluentValidation;
using Microsoft.AspNetCore.Http;
using Nexify.Domain.Entities.Products;

namespace Nexify.Service.Validators
{
    public class ProductUpdateValidator : AbstractValidator<ProductUpdate>
    {
        public ProductUpdateValidator()
        {
            RuleFor(update => update.ProductId)
                .NotEmpty().WithMessage("ProductsId is required")
                .NotEqual(Guid.Empty).WithMessage("ProductsId cannot be empty");

            RuleFor(update => update.Title)
                .MaximumLength(255).WithMessage("Title cannot be longer than 255 characters");

            RuleFor(update => update.Content)
                .MaximumLength(10000).WithMessage("Description cannot be longer than 10000 characters");

            RuleFor(update => update.Price)
                .Must(BeAValidPrice).WithMessage("Price must be a valid numerical value");

            RuleFor(update => update.Images)
                .Must(HaveAtLeastOneImage).WithMessage("At least one image is required");

            RuleFor(request => request.ImagesNames)
                .Must(names => names == null || names.All(name => !string.IsNullOrWhiteSpace(name)))
                .When(request => request.ImagesNames != null)
                .WithMessage("All ImageNames must not be empty");

            RuleFor(request => request.CategoriesIds)
                .Must(categoryIds => categoryIds == null || categoryIds.All(id => id != Guid.Empty))
                .WithMessage("All CategoryId entries must be valid GUIDs or null");
        }

        private bool BeAValidPrice(string price)
        {
            if (!string.IsNullOrWhiteSpace(price) && !decimal.TryParse(price, out _))
            {
                return false;
            }
            return true;
        }

        private bool HaveAtLeastOneImage(List<IFormFile> images)
        {
            return images != null && images.Count > 0;
        }
    }
}
