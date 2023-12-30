using FluentValidation;
using Microsoft.AspNetCore.Http;
using Nexify.Service.Dtos;

public class BlogCategoryDtoValidator : AbstractValidator<BlogCategoryDto>
{
    public BlogCategoryDtoValidator()
    {
        RuleFor(dto => dto.CategoryName)
            .NotEmpty().WithMessage("CategoryName is required.")
            .MaximumLength(255).WithMessage("CategoryName cannot be longer than 255 characters.");

        RuleFor(dto => dto.Description)
            .MaximumLength(1000).WithMessage("Description cannot be longer than 1000 characters.");

        RuleFor(dto => dto.Image)
            .Must(HaveAtLeastOneImage).WithMessage("Image is required and must be a valid image file")
            .When(dto => dto.Image != null);
    }

    private bool HaveAtLeastOneImage(List<IFormFile> images)
    {
        return images != null && images.Count > 0;
    }
}
