using FluentValidation;
using Nexify.Domain.Entities.Categories;
namespace Nexify.Service.Validators
{
    public class CategoryItemsValidator : AbstractValidator<CategoryItems>
    {
        public CategoryItemsValidator()
        {
            RuleFor(pc => pc.ProductId)
                .NotEmpty().WithMessage("ProductsId is required")
                .MaximumLength(36).WithMessage("ProductsId cannot be longer than 36 characters");

            RuleFor(pc => pc.CategoryId)
                .NotEmpty().WithMessage("CategoriesId is required")
                .MaximumLength(36).WithMessage("CategoriesId cannot be longer than 36 characters");
        }
    }
}
