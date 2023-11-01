using FluentValidation;
using Microsoft.AspNetCore.Http;
using Nexify.Service.Dtos;

namespace Nexify.Service.Validators
{
    public class SubcategoryValidator : AbstractValidator<SubcategoryDto>
    {
        public SubcategoryValidator()
        {
            RuleFor(dto => dto.SubCategoryName)
                .NotEmpty().WithMessage("SubCategoryName is required")
                .MaximumLength(255).WithMessage("SubCategoryName cannot be longer than 255 characters");

            RuleFor(dto => dto.Description)
                .MaximumLength(1000).WithMessage("Description cannot be longer than 1000 characters");

            RuleFor(dto => dto.Image)
                .Must(BeAValidImage).WithMessage("Image is required and must be a valid image file");

            RuleFor(dto => dto.ImageName)
                .MaximumLength(255).WithMessage("ImageName cannot be longer than 255 characters");

            RuleFor(dto => dto.ProductsId)
                .NotEmpty().WithMessage("ProductsId is required")
                .NotEqual(Guid.Empty).WithMessage("ProductsId cannot be empty");
        }

        private bool BeAValidImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return false;
            }

            return true;
        }
    }
}
