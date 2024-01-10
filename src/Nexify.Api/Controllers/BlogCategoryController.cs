using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexify.Domain.Entities.Pagination;
using Nexify.Service.Dtos;
using Nexify.Service.Services;

namespace Nexify.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BlogCategoryController : Controller
    {
        private readonly BlogCategoryService _categoryService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public BlogCategoryController(BlogCategoryService categoryService, IWebHostEnvironment hostEnvironment)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }


        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddNewCategory([FromForm] List<BlogCategoryDto> categories)
        {
            await _categoryService.AddCategoryAsync(categories);
            return Ok();
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<BlogCategoryDto>>> GetAll()
        {
            var imageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var productsInCategory = await _categoryService.GetAllCategoriesAsync(imageSrc);
            return Ok(productsInCategory);
        }

        [HttpGet("id")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryResponse>> Get([FromQuery] PaginationFilter filter, string id)
        {
            var route = Request.Path.Value;
            var imageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var productsInCategory = await _categoryService.GetCategoryAsync(filter, id, route, imageSrc);
            return Ok(productsInCategory);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult> Update([FromForm] BlogCategoryDto category)
        {
            await _categoryService.UpdateCategory(category, _hostEnvironment.ContentRootPath);
            return Ok();
        }

        [HttpDelete("id")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(string id)
        {
            await _categoryService.RemoveCategoryAsync(id);
            return Ok();
        }
    }
}
