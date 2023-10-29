using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexify.Domain.Entities.Posts;
using Nexify.Service.Dtos;
using Nexify.Service.Services;

namespace Nexify.Api.Controllers
{
    public class PostController: Controller
    {
        private readonly PostService _postService;

        public PostController(PostService postService)
        {
            _postService = postService ?? throw new ArgumentNullException(nameof(postService));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddNewPosttAsync([FromForm] PostRequest post)
        {
            await _postService.AddPostAsync(post);
            return Ok();
        }

        [HttpPost("category")]
        [AllowAnonymous]
        public async Task<ActionResult> PostCategoriesAsync([FromForm] PostCategories postCategories)
        {
            await _postService.AddPostCategoriesAsync(postCategories);
            return Ok();
        }

    }
}
