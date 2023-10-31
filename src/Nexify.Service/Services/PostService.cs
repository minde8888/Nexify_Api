using AutoMapper;
using Nexify.Data.Repositories;
using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Entities.Posts;
using Nexify.Domain.Entities.Products;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Dtos;
using Nexify.Service.Interfaces;
using Nexify.Service.Validators;

namespace Nexify.Service.Services
{
    public class PostService
    {
        private readonly IMapper _mapper;
        private readonly IPostRepository _postRepository;
        private readonly IImagesService _imagesService;
        private readonly IItemCategoryRepository _postCategoriesRepository;
        private readonly IUriService _uriService;

        public PostService(
            IPostRepository postRepository,
                IImagesService imagesService,
                    IMapper mapper,
                        IItemCategoryRepository itemCategoriesRepository,
                            IUriService uriService)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _imagesService = imagesService ?? throw new ArgumentNullException(nameof(imagesService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _postCategoriesRepository = itemCategoriesRepository ?? throw new ArgumentNullException(nameof(itemCategoriesRepository));
            _uriService = uriService;
        }

        public async Task AddPostAsync(PostRequest post)
        {
            var validationResult = await new PostRequestValidator().ValidateAsync(post);

            if (!validationResult.IsValid)
                throw new PostValidationException(validationResult.Errors.ToString());

            var result = _mapper.Map<Post>(post);
            result.PostId = Guid.NewGuid();

            if (post.Images?.Any() == true)
            {
                result.ImageName = string.
                    Join(",", await _imagesService.
                    SaveImages(post.Images));
            }

            await _postRepository.AddAsync(result);
        }

        public async Task AddPostCategoriesAsync(PostCategories postCategories)
        {
            var validationResult = await new PostCategoriesValidator().ValidateAsync(postCategories);

            if (!validationResult.IsValid)
                throw new PostCategoriesValidationException(validationResult.Errors.ToString());

            await _postCategoriesRepository.AddItemCategoriesAsync(new Guid(postCategories.CategoryId), new Guid(postCategories.PostId));
        }

        public async Task<PostsResponse> GetAllAsync(
            PaginationFilter filter,
                string imageSrc,
                    string route)
        {

            var validationResult = await new PaginationFilterValidator().ValidateAsync(filter);

            if (!validationResult.IsValid)
                throw new PaginationValidationException(validationResult.Errors.ToString());

            var validFilter = filter ?? new PaginationFilter();
            var result = await _postRepository.RetrieveAllAsync(validFilter);

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

            var post = await _postRepository.GetByIdAsync(Guid.Parse(id));

            return MapPost(post, imageSrc);
        }

        public async Task UpdatePostAsync(string contentRootPath, PostRequest post)
        {
            var validationResult = await new PostRequestValidator().ValidateAsync(post);
            if (!validationResult.IsValid)
            {
                throw new ProductValidationException(validationResult.Errors.ToString());
            }

            if (post.Images != null)
            {
                var imagesNames = post.ImageName.Split(',');
                foreach (var imageName in imagesNames)
                {
                    var imagePath = Path.Combine(contentRootPath, "Images", imageName);
                    await _imagesService.DeleteImageAsync(imagePath);
                }

                post.ImageName = await _imagesService.SaveImages(post.Images);
            }

            var result = _mapper.Map<Post>(post);
            await _postRepository.ModifyAsync(result);
        }

        public async Task RemovePostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new PostException("Post id can't by null");

            await _postRepository.DeleteAsync(Guid.Parse(id));
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
                PostId = post.PostId,
                Title = post.Title,
                Context = post.Context,
                DateCreated = post.DateCreated,
                ImageSrc = imageUrls,
            };
        }

        private List<PostDto> MapPagedPost(PagedParams<Post> pageParams, string imageSrc)
        {
            var pagedPosts = PaginationService.CreatePagedResponse(pageParams);

            var result = pagedPosts.Data.Select(post =>
            {
                var productDto = _mapper.Map<PostDto>(post);

                if (!string.IsNullOrEmpty(post.ImageName))
                {
                    var imageNames = post.ImageName.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    var imageSrcs = imageNames.Select(name => $"{imageSrc}/Images/{name.Trim()}");
                    productDto.ImageSrc = imageSrcs.ToList();
                }

                return productDto;
            }).ToList();

            return result;
        }
    }
}
