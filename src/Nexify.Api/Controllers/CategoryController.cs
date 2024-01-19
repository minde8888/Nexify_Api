using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexify.Service.Dtos;
using Nexify.Service.Services;

namespace Nexify.Api.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CategoryController(CategoryService categoryService, IWebHostEnvironment hostEnvironment)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddNewCategory([FromForm] List<AddCategories> categories)
        {
            await _categoryService.AddCategoryAsync(categories);
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<CategoryResponse>>> GetAll()
        {
            var imageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var categories = await _categoryService.GetAllCategoriesAsync(imageSrc);
            return Ok(categories);
        }

        [HttpPut("update")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update([FromForm] CategoryDto category)
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
