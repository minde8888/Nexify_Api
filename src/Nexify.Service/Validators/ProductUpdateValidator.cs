using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Nexify.Domain.Entities.Products
{
    public class ProductUpdateValidator : AbstractValidator<ProductUpdate>
    {
        public ProductUpdateValidator()
        {
            RuleFor(p => p.ProductId).NotEmpty().WithMessage("Product ID is required.");

            RuleFor(p => p.Title)
                .NotEmpty().WithMessage("Title is required.")
                .Length(1, 100).WithMessage("Title must be between 1 and 100 characters.");

            RuleFor(p => p.Content)
                .NotEmpty().WithMessage("Content is required.");

            RuleFor(p => p.Price)
                .NotEmpty().WithMessage("Price is required.")
                .Matches(@"^\d+(\.\d{1,2})?$").WithMessage("Price must be a valid decimal number with up to two decimal places.");

            RuleFor(p => p.Discount)
                .Matches(@"^\d+(\.\d{1,2})?$").When(p => !string.IsNullOrEmpty(p.Discount)).WithMessage("Discount must be a valid decimal number with up to two decimal places.")
                .Must((model, discount) => string.IsNullOrEmpty(discount) || (decimal.TryParse(discount, out var discountValue) && discountValue >= 0 && discountValue <= 100)).When(p => !string.IsNullOrEmpty(p.Discount)).WithMessage("Discount must be between 0 and 100 if provided.");

            RuleFor(p => p.Size)
                .Length(0, 50).When(p => !string.IsNullOrEmpty(p.Size)).WithMessage("Size must be between 0 and 50 characters if provided.");

            RuleFor(p => p.Stock)
                .NotEmpty().WithMessage("Stock is required.")
                .Matches("^[0-9]+$").WithMessage("Stock must be a valid integer.");

            RuleFor(p => p.Location)
                .Length(0, 100).When(p => !string.IsNullOrEmpty(p.Location)).WithMessage("Location must be between 0 and 100 characters if provided.");

            RuleFor(p => p.Images)
                .Must(images => images == null || images.Count <= 10).WithMessage("You can upload up to 10 images.");

            RuleForEach(p => p.Images).SetValidator(new FileValidator());

            RuleFor(p => p.CategoriesIds)
                .Must(ids => ids != null && ids.Count > 0).WithMessage("At least one category is required.");

            RuleFor(p => p.SubcategoriesIds)
                .Must(ids => ids != null && ids.Count >= 0).WithMessage("At least one subcategory is required.");
        }
    }

    public class FileValidator : AbstractValidator<IFormFile>
    {
        public FileValidator()
        {
            RuleFor(file => file.Length).LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("File size must be less than 10MB");
            RuleFor(file => file.ContentType).Must(type => type.StartsWith("image/")).WithMessage("Only images are allowed");
        }
    }
}
