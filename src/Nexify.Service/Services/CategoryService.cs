using AutoMapper;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Dtos;
using Nexify.Service.Validators;
using Nexify.Data.Helpers;
using Nexify.Service.Interfaces;

namespace Nexify.Service.Services
{
    public class CategoryService
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IImagesService _imagesService;
        private readonly ISubcategoryService _subcategoryService;

        public CategoryService(
            IMapper mapper,
                ICategoryRepository categoryRepository,
                    IImagesService imagesService,
                        ISubcategoryService subcategoryService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _imagesService = imagesService ?? throw new ArgumentNullException(nameof(imagesService));
            _subcategoryService = subcategoryService ?? throw new ArgumentNullException(nameof(subcategoryService));
        }
        public async Task AddCategoryAsync(List<AddCategories> categories)
        {
            foreach (var category in categories)
            {
                await ValidateAddCategory(category);

                var result = _mapper.Map<Category>(category);
                await _categoryRepository.AddAsync(result);
            }
        }

        private async Task ValidateAddCategory(AddCategories category)
        {
            var validationResult = await new AddCategoriesValidator().ValidateAsync(category);
            ValidationExceptionHelper.ThrowIfInvalid<CategoryValidationException>(validationResult);
        }

        public async Task<List<CategoryResponse>> GetAllCategoriesAsync(string imageSrc)
        {
            var categories = await _categoryRepository.GetAllAsync();

            ProcessImagesForCategories(categories, imageSrc, "ImageName");

            var mappedCategories = _mapper.Map<List<CategoryResponse>>(categories);

            return mappedCategories;
        }

        private void ProcessImagesForCategories(IEnumerable<Category> categories, string imageSrc, string propertyName)
        {
            foreach (var category in categories)
            {
                category.ImageName = _imagesService.ProcessImages(category, imageSrc, propertyName);

                foreach (var subcategory in category.Subcategories)
                {
                    subcategory.ImageName = _imagesService.ProcessImages(subcategory, imageSrc, propertyName);
                }
            }
        }

        public async Task UpdateCategory(CategoryDto categoryDto, string contentRootPath)
        {
            await ValidateCategoryDto(categoryDto);

             var processedCategory = await _imagesService.MapAndProcessObjectAsync<CategoryDto, Category>(
                categoryDto,
                obj => obj.Images,
                obj => obj.ImageName,
                (obj, imageName) => Path.Combine("Images", imageName),
                contentRootPath,
                "ImageName"
            );

            await _categoryRepository.UpdateAsync(processedCategory);
        }

        private async Task ValidateCategoryDto(CategoryDto categoryDto)
        {
            var validationResult = await new CategoryValidator().ValidateAsync(categoryDto);
            ValidationExceptionHelper.ThrowIfInvalid<CategoryValidationException>(validationResult);
        }

        public async Task RemoveCategoryAsync(string id)
        {
            if (!Guid.TryParse(id, out Guid newId))
            {
                throw new CategoryException($"Invalid GUID: {id}");
            }

            var category = await _categoryRepository.GetAsync(newId)
                ?? throw new CategoryException($"Category not found for ID: {id}");

            if (category.Subcategories != null)
            {
                foreach (var subcategory in category.Subcategories)
                {
                    await _subcategoryService.DeleteSubCategoryAsync(subcategory.SubcategoryId.ToString());
                }
            }

            await _categoryRepository.RemoveAsync(newId);
        }
    }
}
