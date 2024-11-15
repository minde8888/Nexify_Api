﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexify.Service.Dtos.Blog;
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

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddNewCategory([FromForm] List<BlogCategoryDto> categories)
        {
            await _categoryService.AddCategoryAsync(categories);
            return Ok();
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<BlogCategoryResponse>>> GetAll()
        {
            var imageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var productsInCategory = await _categoryService.GetAllCategoriesAsync(imageSrc);
            return Ok(productsInCategory);
        }
     
        [HttpPut("update")]
        //[Authorize(Roles = "Admin")]
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
