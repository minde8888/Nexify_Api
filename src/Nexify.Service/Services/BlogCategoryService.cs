using AutoMapper;
using Microsoft.AspNetCore.Http;
using Nexify.Data.Helpers;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Entities.Posts;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Dtos;
using Nexify.Service.Interfaces;
using Nexify.Service.Validators;

namespace Nexify.Service.Services
{
    public class BlogCategoryService
    {
        private readonly IMapper _mapper;
        private readonly IBlogCategoryRepository _categoryRepository;
        private readonly IUriService _uriService;
        private readonly IImagesService _imagesService;

        public BlogCategoryService(
            IMapper mapper,
                IBlogCategoryRepository categoryRepository,
                    IUriService uriService,
                        IImagesService imagesService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _uriService = uriService ?? throw new ArgumentNullException(nameof(uriService));
            _imagesService = imagesService ?? throw new ArgumentNullException(nameof(imagesService));
        }
        public async Task AddCategoryAsync(List<BlogCategoryDto> categories)
        {
            foreach (var categoryDto in categories)
            {
                var validationResult = await new BlogCategoryDtoValidator().ValidateAsync(categoryDto);
                ValidationExceptionHelper.ThrowIfInvalid<CategoryValidationException>(validationResult);

                var category = await _imagesService.MapAndSaveImages<BlogCategoryDto, BlogCategory>(categoryDto, categoryDto.Images);

                await _categoryRepository.AddAsync(category);
            }
        }

        public async Task<List<BlogCategoryDto>> GetAllCategoriesAsync(string imageSrc)
        {
            var categories = await _categoryRepository.GetAllAsync();

            foreach (var category in categories)
            {
                category.ImageName = _imagesService.ProcessImages(category.ImageName, imageSrc);
            }

            var mappedCategories = _mapper.Map<List<BlogCategoryDto>>(categories);

            return mappedCategories;
        }

        public async Task<BlogPostCategoryResponse> GetCategoryAsync(
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

            var pagedParams = new PagedParams<BlogCategory>(
                new List<BlogCategory> { category.Items },
                validFilter,
                category.TotalCount,
                _uriService,
                route);

            var pagedResponse = PaginationService.CreatePagedResponse(pagedParams);

            var postsWithImages = category.Items.Posts != null ?
                ListImages(category.Items.Posts, imageSrc) :
                null;

            var result = _mapper.Map<BlogPostCategoryResponse>(category.Items);
            result.Posts = postsWithImages;
            result.PageSize = pagedResponse.PageSize;
            result.TotalPages = pagedResponse.TotalPages;
            result.TotalRecords = pagedResponse.TotalRecords;
            result.NextPage = pagedResponse.NextPage;
            result.PreviousPage = pagedResponse.PreviousPage;
            return result;
        }

        private List<CategoryPosts> ListImages(ICollection<Post> posts, string imageSrc)
        {
            var catPosts = _mapper.Map<List<CategoryPosts>>(posts);
            var imageUrls = new List<string>();

            foreach (var post in posts)
            {
                if (post.ImageName == null)
                    throw new PostException("Post image name can't be null");

                var imageNames = post.ImageName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (imageNames.Length == 0)
                    throw new PostException("There are no images for the product.");

                var productImageUrls = imageNames.Select(imageName => $"{imageSrc}/Images/{imageName}").ToList();
                imageUrls.AddRange(productImageUrls);
            }

            foreach (var item in catPosts)
            {
                item.ImageSrc = imageUrls;
            }

            return catPosts;
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
                  contentRootPath
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
