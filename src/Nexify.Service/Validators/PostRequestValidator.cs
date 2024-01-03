using FluentValidation;
using Microsoft.AspNetCore.Http;
using Nexify.Service.Dtos;

namespace Nexify.Service.Validators
{
    public class PostRequestValidator : AbstractValidator<PostRequest>
    {
        public PostRequestValidator()
        {
            RuleFor(request => request.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(255).WithMessage("Title cannot be longer than 255 characters");

            RuleFor(request => request.Content)
                .MaximumLength(10000).WithMessage("Content cannot be longer than 10000 characters");

            RuleFor(request => request.Images)
                .Must(HaveAtLeastOneImage).When(request => request.Images != null).WithMessage("At least one image is required");

            RuleFor(request => request.ImageName)
                .MaximumLength(255).WithMessage("ImageName cannot be longer than 255 characters");

            RuleFor(request => request.CategoryId)
                .Must(id => id == null || id != Guid.Empty).WithMessage("CategoryId must not be empty.")
                .When(request => request.CategoryId.HasValue);
        }

        private bool HaveAtLeastOneImage(List<IFormFile> images)
        {
            return images != null && images.Count > 0;
        }
    }
}
