using FluentValidation;
using Nexify.Service.Dtos;

namespace Nexify.Service.Validators
{
    public class PostRequestValidator : AbstractValidator<PostRequest>
    {
        private const int MaxTitleLength = 255;
        private const int MaxContentLength = 10000;

        public PostRequestValidator()
        {
            RuleFor(request => request.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(MaxTitleLength).WithMessage($"Title cannot be longer than {MaxTitleLength} characters");

            RuleFor(request => request.Content)
                .MaximumLength(MaxContentLength).WithMessage($"Content cannot be longer than {MaxContentLength} characters");

            RuleFor(request => request.Images)
                .Cascade(CascadeMode.Stop)
                .Must(images => images == null || images.Any(image => image != null && image.Length > 0))
                .WithMessage("Invalid image file");

            RuleFor(request => request.ImageNames)
                .Must(names => names == null || names.All(name => !string.IsNullOrWhiteSpace(name)))
                .When(request => request.ImageNames != null)
                .WithMessage("All ImageNames must not be empty");

            RuleFor(request => request.CategoriesIds)
                .Must(categoryIds => categoryIds == null || categoryIds.All(id => id != Guid.Empty))
                .WithMessage("All CategoryId entries must be valid GUIDs or null");
        }
    }

}
