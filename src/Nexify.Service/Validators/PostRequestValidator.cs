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

            RuleFor(request => request.Text)
                .NotEmpty().WithMessage("Text is required")
                .MaximumLength(1000).WithMessage("Text cannot be longer than 1000 characters");

            RuleFor(request => request.Images)
                .Must(HaveAtLeastOneImage).WithMessage("At least one image is required");

            RuleFor(request => request.ImageName)
                .MaximumLength(255).WithMessage("ImageName cannot be longer than 255 characters");
        }

        private bool HaveAtLeastOneImage(List<IFormFile> images)
        {
            return images != null && images.Count > 0;
        }
    }
}
