using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexify.Domain.Entities.Pagination;
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
        public async Task<IActionResult> AddNewCategory([FromForm] List<CategoryDto> categories)
        {
            await _categoryService.AddCategoryAsync(categories);
            return Ok();
        }

        [HttpPost("subcategory")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddNewSubcategory(string categoryId, string subcategoryId)
        {
            await _categoryService.AddSubcategoryToCategoryAsync(categoryId, subcategoryId);
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<CategoryResponse>>> GetAll()
        {
            var imageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var productsInCategory = await _categoryService.GetAllCategoriesAsync(imageSrc);
            return Ok(productsInCategory);
        }

        [HttpGet("id")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryResponse>> Get([FromQuery] PaginationFilter filter, string id)
        {
            var route = Request.Path.Value;
            var imageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var productsInCategory = await _categoryService.GetCategoryAsync(filter, id, route, imageSrc);
            return Ok(productsInCategory);
        }

        [HttpPut("update")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update([FromForm] CategoryDto category)
        {
            await _categoryService.UpdateCategory(category, _hostEnvironment.ContentRootPath);
            return Ok();
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(string id)
        {
            await _categoryService.RemoveCategoryAsync(id);
            return Ok();
        }

        [HttpDelete("delete/subcategory/{id}")]
        public async Task<ActionResult> DeleteSubcategory(string id)
        {
            await _categoryService.RemoveSubcategoryByIdAsync(id);
            return Ok();
        }
    }
}
