using FluentValidation;
using Microsoft.AspNetCore.Http;
using Nexify.Service.Dtos;

namespace Nexify.Service.Validators
{
    public class AttributesUpdateValidator : AbstractValidator<AttributesUpdate>
    {
        public AttributesUpdateValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID is required.");

            RuleFor(x => x.AttributeName)
                .NotEmpty().WithMessage("Attribute name is required.")
                .Length(1, 50).WithMessage("Attribute name must be between 2 and 50 characters.");

            RuleFor(x => x.ImageName)
                .Length(2, 100).WithMessage("Image name must be between 2 and 100 characters.");

            RuleFor(x => x.Image)
                .Must(BeAValidImage).WithMessage("Invalid image format. Allowed formats are .jpg, .jpeg, .png");
        }

        private bool BeAValidImage(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            if (file != null)
            {
                var fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();
                return allowedExtensions.Contains(fileExtension);
            }
            return true;
        }
    }
}
