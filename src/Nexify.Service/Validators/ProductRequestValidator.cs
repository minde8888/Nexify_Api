using FluentValidation;
using Microsoft.AspNetCore.Http;
using Nexify.Service.Dtos;

namespace Nexify.Service.Validators
{
    public class ProductRequestValidator : AbstractValidator<ProductRequest>
    {
        public ProductRequestValidator()
        {
            RuleFor(request => request.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(255).WithMessage("Title cannot be longer than 255 characters");

            RuleFor(request => request.Content)
                .NotEmpty()
                .MaximumLength(10000).WithMessage("Description cannot be longer than 10000 characters");

            RuleFor(request => request.Price)
                .NotEmpty().WithMessage("Price is required")
                .Matches(@"^\d+(\.\d{1,2})?$").WithMessage("Price must be a valid amount with up to 2 decimal places");

            RuleFor(request => request.Discount)
                .NotEmpty()
                .Matches(@"^\d+(\.\d{1,2})?$").WithMessage("Discount must be a valid amount with up to 2 decimal places");

            RuleFor(request => request.Stock)
                .NotEmpty().WithMessage("Stock is required")
                .Matches(@"^\d+$").WithMessage("Stock must be a valid integer");

            RuleFor(request => request.Images)
                .Cascade(CascadeMode.Stop)
                .Must(images => images == null || images.Any(image => image != null && image.Length > 0))
                .WithMessage("Invalid image file");

            RuleFor(request => request.ImagesNames)
                .Must(names => names == null || names.All(name => !string.IsNullOrWhiteSpace(name)))
                .When(request => request.ImagesNames != null)
                .WithMessage("All ImageNames must not be empty");

            RuleFor(request => request.CategoriesIds)
                .Must(categoryIds => categoryIds == null || categoryIds.All(id => id != Guid.Empty))
                .WithMessage("All CategoryId entries must be valid GUIDs or null");

            RuleFor(request => request.SubcategoriesIds)
                .Must(subcategoryIds => subcategoryIds == null || subcategoryIds.All(id => id != Guid.Empty))
                .WithMessage("All SubcategoryId entries must be valid GUIDs or null");
        }
    }
}