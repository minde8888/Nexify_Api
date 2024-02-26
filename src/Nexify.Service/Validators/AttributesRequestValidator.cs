
using FluentValidation;
using Nexify.Service.Dtos;

namespace Nexify.Service.Validators
{
    public class AttributesRequestValidator : AbstractValidator<AttributesRequest>
    {
        public AttributesRequestValidator()
        {
            RuleFor(request => request.AttributeName)
                .NotEmpty().WithMessage("AttributeName is required")
                .MaximumLength(255).WithMessage("AttributeName cannot be longer than 255 characters");
        }
    }
}
