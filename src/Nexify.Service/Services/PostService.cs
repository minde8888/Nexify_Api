using AutoMapper;
using Nexify.Domain.Entities.Posts;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Dtos;
using Nexify.Service.Validators;

namespace Nexify.Service.Services
{
    public class PostService
    {
        private readonly IMapper _mapper;
        private readonly IPostRepository _postRepository;
        private readonly IImagesService _imagesService;
        private readonly IItemCategoriesRepository _postCategoriesRepository;

        public PostService(
            IPostRepository postRepository,
                IImagesService imagesService,
                    IMapper mapper,
                        IItemCategoriesRepository itemCategoriesRepository)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _imagesService = imagesService ?? throw new ArgumentNullException(nameof(imagesService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _postCategoriesRepository = itemCategoriesRepository ?? throw new ArgumentNullException(nameof(itemCategoriesRepository));
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
    }
}
