﻿using FluentValidation;
using Microsoft.AspNetCore.Http;
using Nexify.Service.Dtos.Category;

namespace Nexify.Service.Validators
{
    public class SubcategoryValidator : AbstractValidator<SubcategoryDto>
    {                                                     
        public SubcategoryValidator()
        {
            RuleFor(dto => dto.CategoryName)
                .NotEmpty().WithMessage("SubCategoryName is required")
                .MaximumLength(255).WithMessage("SubCategoryName cannot be longer than 255 characters");

            RuleFor(dto => dto.Description)
                .MaximumLength(10000).WithMessage("Description cannot be longer than 10000 characters");

            RuleFor(dto => dto.Images)
                .Must(HaveAtLeastOneImageOrNull).WithMessage("Image must be a valid image file");

            RuleFor(dto => dto.ImageName)
                .MaximumLength(255).WithMessage("ImageName cannot be longer than 255 characters");
        }

        private bool HaveAtLeastOneImageOrNull(List<IFormFile> images)
        {
            return images == null || (images.Count > 0 && images.All(image => image != null));
        }
    }
}
