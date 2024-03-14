using FluentValidation;
using Nexify.Service.Dtos.Category;

namespace Nexify.Service.Validators
{
    public class AddSubcategoryValidator : AbstractValidator<AddSubcategory>
    {
        public AddSubcategoryValidator()
        {
            RuleFor(dto => dto.CategoryName)
                    .NotEmpty().WithMessage("SubCategoryName is required")
                    .MaximumLength(255).WithMessage("SubCategoryName cannot be longer than 255 characters");
            RuleFor(dto => dto.CategoryId)
                    .NotEmpty().WithMessage("CategoryId is required");
        }
    }
}
