using FluentValidation;
using Nexify.Domain.Entities.Products;
namespace Nexify.Service.Validators
{
    public class ProductCategoriesValidator : AbstractValidator<ProductCategories>
    {
        public ProductCategoriesValidator()
        {
            RuleFor(pc => pc.ProductId)
                .NotEmpty().WithMessage("ProductId is required")
                .MaximumLength(36).WithMessage("ProductId cannot be longer than 36 characters");

            RuleFor(pc => pc.CategoryId)
                .NotEmpty().WithMessage("CategoryId is required")
                .MaximumLength(36).WithMessage("CategoryId cannot be longer than 36 characters");
        }
    }
}
