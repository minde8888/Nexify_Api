using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexify.Service.Dtos.Category;
using Nexify.Service.Interfaces;
using Nexify.Service.Services;

namespace Nexify.Api.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SubcategoryController : Controller
    {
        private readonly ISubcategoryService _subcategoryService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public SubcategoryController(ISubcategoryService subCategoryService, IWebHostEnvironment hostEnvironment)
        {
            _subcategoryService = subCategoryService ?? throw new ArgumentNullException(nameof(subCategoryService));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddNewCategory([FromForm] List<AddSubcategory> categories)
        {
            await _subcategoryService.AddSubCategoryAsync(categories);
            return Ok();
        }

        [HttpPut("update")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update([FromForm] SubcategoryDto subcategory)
        {
            await _subcategoryService.UpdateSubCategoryAsync(subcategory, _hostEnvironment.ContentRootPath);
            return Ok();
        }

        [HttpDelete("id")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete([FromQuery]  string id)
        {
            await _subcategoryService.DeleteSubCategoryAsync(id);
            return Ok();
        }
    }
}
