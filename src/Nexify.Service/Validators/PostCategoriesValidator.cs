using FluentValidation;
using Nexify.Domain.Entities.Posts;

namespace Nexify.Service.Validators
{
    public class PostCategoriesValidator : AbstractValidator<PostCategories>
    {
        public PostCategoriesValidator()
        {
            RuleFor(pc => pc.PostId)
                .NotEmpty().WithMessage("PostId is required")
                .MaximumLength(36).WithMessage("PostId cannot be longer than 36 characters");

            RuleFor(pc => pc.CategoryId)
                .NotEmpty().WithMessage("CategoryId is required")
                .MaximumLength(36).WithMessage("CategoryId cannot be longer than 36 characters");
        }
    }
}
