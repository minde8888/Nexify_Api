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
    public class SubcategoryController : Controller
    {
        private readonly SubcategoryService _subcategoryService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public SubcategoryController(SubcategoryService subCategoryService, IWebHostEnvironment hostEnvironment)
        {
            _subcategoryService = subCategoryService ?? throw new ArgumentNullException(nameof(subCategoryService));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }

        [HttpGet("id")]
        [AllowAnonymous]
        public async Task<ActionResult<SubcategoryResponse>> Get([FromQuery] string id)
        {
            var imageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var productsInCategory = await _subcategoryService.GetSubCategoryAsync(id, imageSrc);
            return Ok(productsInCategory);
        }

        [HttpPut("Update")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update([FromForm] SubcategoryDto subcategory)
        {
            await _subcategoryService.UpdateSubCategoryAsync(subcategory, _hostEnvironment.ContentRootPath);
            return Ok();
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(string id)
        {
            await _subcategoryService.DeleteSubCategoryAsync(id);
            return Ok();
        }
    }
}
