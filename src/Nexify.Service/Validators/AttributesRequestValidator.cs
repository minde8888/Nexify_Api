
using FluentValidation;
using Nexify.Service.Dtos;

namespace Nexify.Service.Validators
{
    public class AttributesRequestValidator : AbstractValidator<AttributesRequest>
    {
        private const int MaxContentLength = 1000;

        public AttributesRequestValidator()
        {
            RuleFor(request => request.ImagesNames)
                .ForEach(name => name.MaximumLength(MaxContentLength).WithMessage($"Content cannot be longer than {MaxContentLength} characters"));

            RuleFor(request => request.ImageDescription)
                .ForEach(description => description.MaximumLength(MaxContentLength).WithMessage($"Content cannot be longer than {MaxContentLength} characters"));

            RuleFor(request => request.Images)
                .Cascade(CascadeMode.Stop)
                .Must(images => images == null || images.Any(image => image != null && image.Length > 0))
                .WithMessage("Invalid image file");
        }
    }
}
