using Nexify.Data.Helpers;
using Nexify.Domain.Entities.Subcategories;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Dtos;
using Nexify.Service.Validators;

namespace Nexify.Service.Services
{
    public class SubcategoryService
    {
        private readonly ISubcategoryRepository _subcategoryRepository;
        private readonly IImagesService _imagesService;

        public SubcategoryService(
                            ISubcategoryRepository subcategoryRepository,
                                            IImagesService imagesService)
        {
            _subcategoryRepository = subcategoryRepository ?? throw new ArgumentNullException(nameof(subcategoryRepository));
            _imagesService = imagesService ?? throw new ArgumentNullException(nameof(imagesService));
        }

        public async Task AddSubCategoryAsync(List<SubcategoryDto> subcategories, Guid categoryId)
        {
            foreach (var subcategoryDto in subcategories)
            {
                await ValidateSubcategoryDto(subcategoryDto);

                var subcategory = await _imagesService.MapAndSaveImages<SubcategoryDto, Subcategory>(subcategoryDto, subcategoryDto.Images);
                subcategory.CategoryId = categoryId;

                await _subcategoryRepository.AddAsync(subcategory);
            }
        }

        private async Task ValidateSubcategoryDto(SubcategoryDto subcategories)
        {
            var validationResult = await new SubcategoryValidator().ValidateAsync(subcategories);
            ValidationExceptionHelper.ThrowIfInvalid<SubcategoryValidationException>(validationResult);
        }

        public async Task<SubcategoryResponse> GetSubCategoryAsync(string id, string imageSrc)
        {
            if (string.IsNullOrEmpty(id))
                throw new SubcategoryException("Subcategory id can't by null");

            var subcategory = await _subcategoryRepository.GetAsync(Guid.Parse(id));

            return MapSubcategory(subcategory, imageSrc);
        }

        public async Task UpdateSubCategoryAsync(SubcategoryDto subcategoryDto, string rootPath)
        {
            var validationResult = await new SubcategoryValidator().ValidateAsync(subcategoryDto);
            ValidationExceptionHelper.ThrowIfInvalid<SubcategoryValidationException>(validationResult);

            var processedSubcategory = await _imagesService.MapAndProcessObjectAsync<SubcategoryDto, Subcategory>(
                subcategoryDto,
                obj => obj.Images,
                obj => obj.ImageName,
                (obj, imageName) => Path.Combine("Images", imageName),
                rootPath
);
            await _subcategoryRepository.UpdateAsync(processedSubcategory);
        }

        public async Task DeleteSubCategoryAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new SubcategoryException("Subcategory id can't by null");

            await _subcategoryRepository.RemoveAsync(Guid.Parse(id));
        }

        private SubcategoryResponse MapSubcategory(Subcategory subcategory, string imageSrc)
        {
            if (subcategory.ImageName == null)
                throw new ProductException("Product image name can't be null");

            var imageNames = subcategory.ImageName;
            if (imageNames.Length == 0)
                throw new ProductException("There are no images for the product.");

            return new SubcategoryResponse
            {
                Id = subcategory.SubcategoryId,
                CategoryName = subcategory.SubCategoryName,
                Description = subcategory.Description,
                ImageSrc = imageNames.Select(imageName => $"{imageSrc}/Images/{imageName}").ToString(),
                //Products = _mapper.Map<List<ProductDto>>(subcategory.Products)
            };
        }
    }
}
