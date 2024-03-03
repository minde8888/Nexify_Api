using FluentValidation;
using Microsoft.AspNetCore.Http;
using Nexify.Service.Dtos;

namespace Nexify.Service.Validators
{
    public class BlogCategoryDtoValidator : AbstractValidator<BlogCategoryDto>
    {
        public BlogCategoryDtoValidator()
        {
            RuleFor(dto => dto.CategoryName)
                .NotEmpty().WithMessage("CategoryName is required.")
                .MaximumLength(255).WithMessage("CategoryName cannot be longer than 255 characters.");

            RuleFor(dto => dto.Description)
                .MaximumLength(10000).WithMessage("Description cannot be longer than 10000 characters.");

            RuleFor(dto => dto.Images)
                .Must(HaveAtLeastOneImageOrNull).WithMessage("Image must be a valid image file");
        }

        private bool HaveAtLeastOneImageOrNull(List<IFormFile> images)
        {
            return images == null || (images.Count > 0 && images.All(image => image != null));
        }
    }
}

