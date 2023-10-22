﻿using FluentValidation;
using Nexify.Domain.Entities.Auth;

namespace Nexify.Service.Validators
{
    public class RequestTokenValidator : AbstractValidator<RequestToken>
    {
        public RequestTokenValidator()
        {
            RuleFor(x => x.Token)
                .NotNull().WithMessage("Token cannot be null")
                .NotEmpty().WithMessage("Token cannot be empty");

            RuleFor(x => x.RefreshToken)
                .NotNull().WithMessage("RefreshToken cannot be null")
                .NotEmpty().WithMessage("RefreshToken cannot be empty");
        }
    }
}
