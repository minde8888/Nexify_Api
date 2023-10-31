using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Entities.Posts;
using Nexify.Service.Dtos;
using Nexify.Service.Services;

namespace Nexify.Api.Controllers
{
    public class PostController: Controller
    {
        private readonly PostService _postService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PostController(PostService postService, IWebHostEnvironment hostEnvironment)
        {
            _postService = postService ?? throw new ArgumentNullException(nameof(postService));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PostsResponse>> GetAllPostsAsync([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var imageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var response = await _postService.GetAllAsync(filter, imageSrc, route);
            return Ok(response);
        }

        [HttpGet("id")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDto>> GetAsync(string id)
        {
            var imageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var product = await _postService.GetPostAsync(id, imageSrc);
            return Ok(product);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAsync([FromForm] PostRequest post)
        {
            await _postService.UpdatePostAsync(_hostEnvironment.ContentRootPath, post);
            return Ok();
        }

        [HttpDelete("delete/{id}")]
        [AllowAnonymous]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteProductAsync(string id)
        {
            await _postService.RemovePostAsync(id);
            return Ok();
        }

        [HttpDelete("delete/category/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> DeleteProductCategoriesAsync(string id)
        {
            await _postService.RemovePostCategoriesAsync(id);
            return Ok();
        }
    }
}
