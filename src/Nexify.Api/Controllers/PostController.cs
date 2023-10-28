using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult> AddNewProductAsync([FromForm] PostRequest post)
        {
            await _postService.AddProductAsync(post);
            return Ok();
        }

    }
}
