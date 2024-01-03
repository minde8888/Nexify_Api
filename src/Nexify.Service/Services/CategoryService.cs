using AutoMapper;
using Microsoft.AspNetCore.Http;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Entities.Products;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Dtos;
using Nexify.Service.Validators;
using Nexify.Service.Interfaces;
using Nexify.Data.Helpers;

namespace Nexify.Service.Services
{
    public class CategoryService
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUriService _uriService;
        private readonly IImagesService _imagesService;
        private readonly SubcategoryService _subcategoryService;

        public CategoryService(
            IMapper mapper,
                ICategoryRepository categoryRepository,
                    IUriService uriService,
                        IImagesService imagesService,
                           SubcategoryService subcategoryService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _uriService = uriService ?? throw new ArgumentNullException(nameof(uriService));
            _imagesService = imagesService ?? throw new ArgumentNullException(nameof(imagesService));
            _subcategoryService = subcategoryService ?? throw new ArgumentNullException(nameof(subcategoryService));
        }
        public async Task AddCategoryAsync(List<CategoryDto> categories)
        {
            foreach (var categoryDto in categories)
            {
                var validationResult = await new CategoryValidator().ValidateAsync(categoryDto);
                ValidationExceptionHelper.ThrowIfInvalid<CategoryValidationException>(validationResult);

                var category = _mapper.Map<Category>(categoryDto);

                if (categoryDto.Images != null)
                {
                    foreach (var image in categoryDto.Images)
                    {
                        category.ImageName = await _imagesService.SaveImages(new List<IFormFile> { image });
                    }
                }

                await _categoryRepository.AddAsync(category);

                if (categoryDto.Subcategories != null)
                {
                    await _subcategoryService.AddSubCategoryAsync(categoryDto.Subcategories, category.CategoryId);
                }
            }
        }

        public async Task<List<CategoryResponse>> GetAllCategoriesAsync(string imageSrc)
        {
            var categories = await _categoryRepository.GetAllAsync();

            foreach (var category in categories)
            {
                category.ImageName = !string.IsNullOrEmpty(category.ImageName) ?
                    $"{imageSrc}/Images/{category.ImageName}" :
                    null;

                foreach (var subcategory in category.Subcategories)
                {
                    subcategory.ImageName = !string.IsNullOrEmpty(subcategory.ImageName) ?
                        $"{imageSrc}/Images/{subcategory.ImageName}" :
                        null;
                }
            }

            var mappedCategories = _mapper.Map<List<CategoryResponse>>(categories);

            return mappedCategories;
        }

        public async Task<CategoriesResponse> GetCategoryAsync(
            PaginationFilter filter,
            string id,
            string route,
            string imageSrc)
        {
            if (!Guid.TryParse(id, out Guid guidId))
                throw new CategoryException($"Invalid GUID: {id}");

            var validationResult = await new PaginationFilterValidator().ValidateAsync(filter);
            ValidationExceptionHelper.ThrowIfInvalid<PaginationValidationException>(validationResult);

            var validFilter = filter ?? new PaginationFilter();
            var category = await _categoryRepository.GetAsync(guidId, validFilter);

            var pagedParams = new PagedParams<Category>(
                new List<Category> { category.Items },
                validFilter,
                category.TotalCount,
                _uriService,
                route);

            var pagedResponse = PaginationService.CreatePagedResponse(pagedParams);

            var productsWithImages = category.Items.Products != null ?
                ListImages(category.Items.Products, imageSrc) :
                null;

            var result = _mapper.Map<CategoriesResponse>(category.Items);
            result.Products = productsWithImages;
            result.PageSize = pagedResponse.PageSize;
            result.TotalPages = pagedResponse.TotalPages;
            result.TotalRecords = pagedResponse.TotalRecords;
            result.NextPage = pagedResponse.NextPage;
            result.PreviousPage = pagedResponse.PreviousPage;
            return result;
        }

        private List<CategoryProducts> ListImages(ICollection<Product> products, string imageSrc)
        {
            var catProducts = _mapper.Map<List<CategoryProducts>>(products);
            var imageUrls = new List<string>();

            foreach (var product in products)
            {
                if (product.ImageName == null)
                    throw new ProductException("Product image name can't be null");

                var imageNames = product.ImageName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (imageNames.Length == 0)
                    throw new ProductException("There are no images for the product.");

                var productImageUrls = imageNames.Select(imageName => $"{imageSrc}/Images/{imageName}").ToList();
                imageUrls.AddRange(productImageUrls);
            }

            foreach (var item in catProducts)
            {
                item.ImageSrc = imageUrls;
            }

            return catProducts;
        }

        public async Task UpdateCategory(CategoryDto categoryDto, string contentRootPath)
        {
            var validationResult = await new CategoryValidator().ValidateAsync(categoryDto);
            ValidationExceptionHelper.ThrowIfInvalid<CategoryValidationException>(validationResult);

            var mappedCategory = _mapper.Map<Category>(categoryDto);

            if (categoryDto.Images != null)
            {
                foreach (var image in categoryDto.Images)
                {
                    categoryDto.ImageName = await _imagesService.SaveImages(new List<IFormFile> { image });
                }
                var imagePath = Path.Combine(contentRootPath, "Images", categoryDto.ImageName);
                await _imagesService.DeleteImageAsync(imagePath);
            }

            await _categoryRepository.UpdateAsync(mappedCategory);
        }

        public async Task RemoveCategoryAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new CategoryException($"Invalid GUID: {id}");

            await _categoryRepository.RemoveAsync(new Guid(id));
        }

        public async Task RemoveSubcategoryByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new CategoryException($"Invalid GUID: {id}");

            await _categoryRepository.RemoveAsync(new Guid(id));
        }

    }
}
