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

        public PostService(IPostRepository postRepository, IImagesService imagesService, IMapper mapper)
        {
            _postRepository = postRepository;
            _imagesService = imagesService;
            _mapper = mapper;
        }

        public async Task AddProductAsync(PostRequest post)
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
    }
}
