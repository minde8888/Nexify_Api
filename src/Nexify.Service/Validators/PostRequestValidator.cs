using FluentValidation;
using Nexify.Service.Dtos;

namespace Nexify.Service.Validators
{
    public class PostRequestValidator : AbstractValidator<PostRequest>
    {
        private const int MaxTitleLength = 255;
        private const int MaxContentLength = 10000;
        private const int MaxImageNameLength = 255;

        public PostRequestValidator()
        {
            RuleFor(request => request.Id)
            .NotEmpty().WithMessage("Id is required");

            RuleFor(request => request.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(MaxTitleLength).WithMessage($"Title cannot be longer than {MaxTitleLength} characters");

            RuleFor(request => request.Content)
                .MaximumLength(MaxContentLength).WithMessage($"Content cannot be longer than {MaxContentLength} characters");

            RuleFor(request => request.Images)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("At least one image is required")
                .Must(images => images?.Any() == true).WithMessage("At least one image is required")
                .ForEach(imageRule =>
                {
                    imageRule.Must(image => image != null && image.Length > 0).WithMessage("Invalid image file");
                });

            RuleFor(request => request.ImageName)
                .MaximumLength(MaxImageNameLength).WithMessage($"ImageName cannot be longer than {MaxImageNameLength} characters");

            RuleFor(request => request.CategoriesIds)
                .Must(categoryIds => categoryIds != null && categoryIds.All(id => id != Guid.Empty)).WithMessage("All CategoryId entries must be valid GUIDs");
        }
    }
}
