using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexify.Domain.Entities.Pagination;
using Nexify.Service.Dtos;
using Nexify.Service.Services;

namespace Nexify.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BlogController : Controller
    {
        private readonly BlogService _blogService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public BlogController(BlogService postService, IWebHostEnvironment hostEnvironment)
        {
            _blogService = postService ?? throw new ArgumentNullException(nameof(postService));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddNewPostAsync([FromForm] PostRequest post)
        {
            await _blogService.AddPostAsync(post);
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PostsResponse>> GetAllPostsAsync([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var imageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var response = await _blogService.GetAllAsync(filter, imageSrc, route);
            return Ok(response);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAsync([FromForm] PostUpdateRequest post)
        {
            await _blogService.UpdatePostAsync(_hostEnvironment.ContentRootPath, post);
            return Ok();
        }

        [HttpDelete("id")]
        [AllowAnonymous]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteAsync([FromQuery] string id)
        {
            await _blogService.RemovePostAsync(id);
            return Ok();
        }
    }
}
