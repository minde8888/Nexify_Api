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
            .Must(HaveValidImages).WithMessage("Image must be a valid file.");

        RuleFor(dto => dto.PostId)
            .NotEmpty().WithMessage("PostId is required.")
            .NotEqual(Guid.Empty).WithMessage("Invalid PostId.");
    }

    private bool HaveValidImages(List<IFormFile> images)
    {
        return images != null && images.All(IsImage);
    }

    private bool IsImage(IFormFile file)
    {
        return file.ContentType.StartsWith("image/");
    }
}
