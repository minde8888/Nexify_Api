using FluentValidation;
using Nexify.Service.Dtos;

namespace Nexify.Service.Validators
{
    public class AddCategoriesValidator : AbstractValidator<AddCategories>
    {
        public AddCategoriesValidator()
        {
            RuleFor(dto => dto.CategoryName)
                .NotEmpty().WithMessage("SubCategoryName is required")
                .MaximumLength(255).WithMessage("SubCategoryName cannot be longer than 255 characters");
        }
    }
}
