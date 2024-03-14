using AutoMapper;
using Nexify.Data.Helpers;
using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Entities.Posts;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Dtos.Post;
using Nexify.Service.Interfaces;
using Nexify.Service.Validators;

namespace Nexify.Service.Services
{
    public class BlogService
    {
        private readonly IMapper _mapper;
        private readonly IBlogRepository _blogRepository;
        private readonly IImagesService _imagesService;
        private readonly IPostCategoryRepository _postCategoriesRepository;
        private readonly IUriService _uriService;

        public BlogService(
            IBlogRepository postRepository,
                IImagesService imagesService,
                    IMapper mapper,
                        IPostCategoryRepository itemCategoriesRepository,
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

            var result = await _imagesService.MapAndSaveImages<PostRequest, Post>(post, post.Images, "ImageNames");
            await _blogRepository.AddAsync(result);

            if (post.CategoriesIds != null && post.CategoriesIds.Any())
            {
                foreach (var categoryId in post.CategoriesIds)
                {
                    await _postCategoriesRepository.AddPostCategoriesAsync(categoryId, result.Id);
                }
            }
        }

        public async Task AddPostCategoriesAsync(PostCategories postCategories)
        {
            var validationResult = await new PostCategoriesValidator().ValidateAsync(postCategories);
            ValidationExceptionHelper.ThrowIfInvalid<PostCategoriesValidationException>(validationResult);

            await _postCategoriesRepository.AddPostCategoriesAsync(new Guid(postCategories.CategoryId), new Guid(postCategories.PostId));
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

        public async Task UpdatePostAsync(string contentRootPath, PostUpdateRequest post)
        {
            var validationResult = await new PostUpdateRequestValidator().ValidateAsync(post);
            ValidationExceptionHelper.ThrowIfInvalid<ProductValidationException>(validationResult);

            var processedPost = await _imagesService.MapAndProcessObjectListAsync<PostUpdateRequest, Post>(
                post,
                contentRootPath,
                "ImageNames"
            );

            await _blogRepository.ModifyAsync(processedPost);            

            if (post.CategoriesIds != null && post.CategoriesIds.Any())
            {
                await _postCategoriesRepository.DeleteRangePostCategories(processedPost.Id);
                
                foreach (var categoryId in post.CategoriesIds)
                {
                    await _postCategoriesRepository.AddPostCategoriesAsync(categoryId, processedPost.Id);
                }
            }
        }

        public async Task RemovePostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new PostException("Post id can't by null");

            await _blogRepository.DeleteAsync(Guid.Parse(id));
        }

        private List<PostDto> MapPagedPost(PagedParams<Post> pageParams, string imageSrc)
        {
            var pagedPosts = PaginationService.CreatePagedResponse(pageParams);

            var result = pagedPosts.Data.Select(post =>
            {
                var postDto = _mapper.Map<PostDto>(post);

                if (postDto.ImageNames.Count != 0)
                {
                    foreach (var imageName in postDto.ImageNames)
                    {
                        var imageNames = post.ImageNames;
                        var imageSrcs = imageNames.Select(name => $"{imageSrc}/Images/{name.Trim()}");
                        postDto.ImageSrc = imageSrcs.ToList();
                    }
                }

                return postDto;
            }).ToList();

            return result;
        }
    }
}
