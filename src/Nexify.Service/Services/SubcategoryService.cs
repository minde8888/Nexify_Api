using AutoMapper;
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
        private readonly IMapper _mapper;

        public SubcategoryService(
                            ISubcategoryRepository subcategoryRepository,
                                            IImagesService imagesService,
                                                            IMapper mapper)
        {
            _subcategoryRepository = subcategoryRepository ?? throw new ArgumentNullException(nameof(subcategoryRepository));
            _imagesService = imagesService ?? throw new ArgumentNullException(nameof(imagesService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task AddSubCategoryAsync(List<AddSubcategory> subcategories)
        {
            foreach (var subcategory in subcategories)
            {
                await AddSubcategoryValidator(subcategory);
                var result = _mapper.Map<Subcategory>(subcategory);

                await _subcategoryRepository.AddAsync(result);
            }
        }

        private async Task AddSubcategoryValidator(AddSubcategory subcategories)
        {
            var validationResult = await new AddSubcategoryValidator().ValidateAsync(subcategories);
            ValidationExceptionHelper.ThrowIfInvalid<SubcategoryValidationException>(validationResult);
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
                rootPath);
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
            };
        }
    }
}
