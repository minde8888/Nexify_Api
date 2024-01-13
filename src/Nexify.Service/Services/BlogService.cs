using AutoMapper;
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
    public class BlogService
    {
        private readonly IMapper _mapper;
        private readonly IBlogRepository _blogRepository;
        private readonly IImagesService _imagesService;
        private readonly IItemCategoryRepository _postCategoriesRepository;
        private readonly IUriService _uriService;

        public BlogService(
            IBlogRepository postRepository,
                IImagesService imagesService,
                    IMapper mapper,
                        IItemCategoryRepository itemCategoriesRepository,
                            IUriService uriService)
        {
            _blogRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _imagesService = imagesService ?? throw new ArgumentNullException(nameof(imagesService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _postCategoriesRepository = itemCategoriesRepository ?? throw new ArgumentNullException(nameof(itemCategoriesRepository));
            _uriService = uriService;
        }

        public async Task AddPostAsync(PostRequest post)
        {
            var validationResult = await new PostRequestValidator().ValidateAsync(post);
            ValidationExceptionHelper.ThrowIfInvalid<PostValidationException>(validationResult);

            var result = await _imagesService.MapAndSaveImages<PostRequest, Post>(post, post.Images);
            await _blogRepository.AddAsync(result);

            if (post.Id != null)
            {
                await _postCategoriesRepository.AddItemCategoriesAsync(post.Id, result.Id);
            }
        }

        public async Task AddPostCategoriesAsync(PostCategories postCategories)
        {
            var validationResult = await new PostCategoriesValidator().ValidateAsync(postCategories);
            ValidationExceptionHelper.ThrowIfInvalid<PostCategoriesValidationException>(validationResult);

            await _postCategoriesRepository.AddItemCategoriesAsync(new Guid(postCategories.CategoryId), new Guid(postCategories.PostId));
        }

        public async Task<PostsResponse> GetAllAsync(
            PaginationFilter filter,
                string imageSrc,
                    string route)
        {

            var validationResult = await new PaginationFilterValidator().ValidateAsync(filter);
            ValidationExceptionHelper.ThrowIfInvalid<PostCategoriesValidationException>(validationResult);

            var validFilter = filter ?? new PaginationFilter();
            var result = await _blogRepository.RetrieveAllAsync(validFilter);

            var pageParams = new PagedParams<Post>(
                result.Items,
                validFilter,
                result.TotalCount,
                _uriService,
                route);

            var pagedProducts = PaginationService.CreatePagedResponse(pageParams);

            var postDtos = MapPagedPost(pageParams, imageSrc);

            return new PostsResponse
            {
                PageNumber = pagedProducts.PageNumber,
                PageSize = pagedProducts.PageSize,
                TotalPages = pagedProducts.TotalPages,
                TotalRecords = pagedProducts.TotalRecords,
                NextPage = pagedProducts.NextPage,
                PreviousPage = pagedProducts.PreviousPage,
                Post = postDtos
            };
        }

        public async Task<PostDto> GetPostAsync(string id, string imageSrc)
        {
            if (string.IsNullOrEmpty(id))
                throw new ProductException("Post id can't by null");

            var post = await _blogRepository.GetByIdAsync(Guid.Parse(id));

            return MapPost(post, imageSrc);
        }

        public async Task UpdatePostAsync(string contentRootPath, PostRequest post)
        {
            var validationResult = await new PostRequestValidator().ValidateAsync(post);
            ValidationExceptionHelper.ThrowIfInvalid<ProductValidationException>(validationResult);

            var processedPost = await _imagesService.MapAndProcessObjectAsync<PostRequest, Post>(
                    post,
                    obj => obj.Images,
                    obj => obj.ImageName,
                    (obj, imageName) => Path.Combine("Images", imageName),
                    contentRootPath
                );

            await _blogRepository.ModifyAsync(processedPost);
        }

        public async Task RemovePostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new PostException("Post id can't by null");

            await _blogRepository.DeleteAsync(Guid.Parse(id));
        }

        public async Task RemovePostCategoriesAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new PostException("Post id can't by null");

            await _postCategoriesRepository.DeleteCategoriesItemAsync(new Guid(id));
        }

        private PostDto MapPost(Post post, string imageSrc)
        {
            if (post.ImageName == null)
                throw new PostException("Popst image name can't be null");

            var imageNames = post.ImageName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (imageNames.Length == 0)
                throw new PostException("There are no images for the post.");

            var imageUrls = imageNames.Select(imageName => $"{imageSrc}/Images/{imageName}").ToList();
            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                DateCreated = post.DateCreated,
                ImageSrc = imageUrls,
            };
        }

        private List<PostDto> MapPagedPost(PagedParams<Post> pageParams, string imageSrc)
        {
            var pagedPosts = PaginationService.CreatePagedResponse(pageParams);

            var result = pagedPosts.Data.Select(post =>
            {
                var postDto = _mapper.Map<PostDto>(post);

                if (!string.IsNullOrEmpty(post.ImageName))
                {
                    var imageNames = post.ImageName.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    var imageSrcs = imageNames.Select(name => $"{imageSrc}/Images/{name.Trim()}");
                    postDto.ImageSrc = imageSrcs.ToList();
                }

                return postDto;
            }).ToList();

            return result;
        }
    }
}
