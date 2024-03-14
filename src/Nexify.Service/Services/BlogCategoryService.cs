using AutoMapper;
using Nexify.Data.Helpers;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Dtos.Blog;
using Nexify.Service.Validators;

namespace Nexify.Service.Services
{
    public class BlogCategoryService
    {
        private readonly IMapper _mapper;
        private readonly IBlogCategoryRepository _categoryRepository;
        private readonly IImagesService _imagesService;

        public BlogCategoryService(
            IMapper mapper,
                IBlogCategoryRepository categoryRepository,
                        IImagesService imagesService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _imagesService = imagesService ?? throw new ArgumentNullException(nameof(imagesService));
        }
        public async Task AddCategoryAsync(List<BlogCategoryDto> categories)
        {
            foreach (var categoryDto in categories)
            {
                var validationResult = await new BlogCategoryDtoValidator().ValidateAsync(categoryDto);
                ValidationExceptionHelper.ThrowIfInvalid<CategoryValidationException>(validationResult);

                var category = await _imagesService.MapAndSaveImages<BlogCategoryDto, BlogCategory>(categoryDto, categoryDto.Images, "ImageName");

                await _categoryRepository.AddAsync(category);
            }
        }

        public async Task<List<BlogCategoryResponse>> GetAllCategoriesAsync(string imageSrc)
        {
            var categories = await _categoryRepository.GetAllAsync();

            foreach (var category in categories)
            {
                category.ImageName = _imagesService.ProcessImages(category, imageSrc, "ImageName");
            }

            var mappedCategories = _mapper.Map<List<BlogCategoryResponse>>(categories);

            return mappedCategories;
        }

        public async Task UpdateCategory(BlogCategoryDto categoryDto, string contentRootPath)
        {
            var validationResult = await new BlogCategoryDtoValidator().ValidateAsync(categoryDto);
            ValidationExceptionHelper.ThrowIfInvalid<CategoryValidationException>(validationResult);

            var processedCategory = await _imagesService.MapAndProcessObjectAsync<BlogCategoryDto, BlogCategory>(
                  categoryDto,
                  obj => obj.Images,
                  obj => obj.ImageName,
                  (obj, imageName) => Path.Combine("Images", imageName),
                  contentRootPath,
                  "ImageName"
              );

            await _categoryRepository.UpdateAsync(processedCategory);
        }

        public async Task RemoveCategoryAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new CategoryException($"Invalid GUID: {id}");

            await _categoryRepository.RemoveAsync(new Guid(id));
        }
    }
}
