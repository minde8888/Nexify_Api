using FluentValidation;
using Microsoft.AspNetCore.Http;
using Nexify.Service.Dtos;

public class ProductRequestValidator : AbstractValidator<ProductRequest>
{
    public ProductRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(255).WithMessage("Title cannot be longer than 255 characters");

        RuleFor(request => request.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(1000).WithMessage("Description cannot be longer than 1000 characters");

        RuleFor(request => request.Price)
            .NotEmpty().WithMessage("Price is required")
            .Must(BeAValidPrice).WithMessage("Price must be a valid numerical value");

        RuleFor(request => request.Images)
            .Must(HaveAtLeastOneImage).WithMessage("At least one image is required");

        RuleFor(request => request.ImageNames)
            .ForEach(nameRule => nameRule.MaximumLength(255).WithMessage("ImageName cannot be longer than 255 characters"));
    }

    private bool BeAValidPrice(string price)
    {
        return decimal.TryParse(price, out _);
    }

    private bool HaveAtLeastOneImage(List<IFormFile> images)
    {
        return images != null && images.Count > 0;
    }
}
