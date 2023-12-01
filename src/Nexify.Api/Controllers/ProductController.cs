using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Entities.Products;
using Nexify.Service.Dtos;
using Nexify.Service.Services;

namespace Nexify.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductController : Controller
    {
        private readonly ProductsService _productService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(ProductsService productService, IWebHostEnvironment hostEnvironment)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> AddNewProductAsync([FromForm] ProductRequest product)
        {
            await _productService.AddProductAsync(product);
            return Ok();
        }

        [HttpPost("category")]
        [AllowAnonymous]
        public async Task<ActionResult> ProductCategoriesAsync([FromForm] ProductCategories productCategories)
        {
            await _productService.AddProductCategoriesAsync(productCategories);
            return Ok();
        }

        [HttpPost("subcategory")]
        [AllowAnonymous]
        public async Task<ActionResult> ProductSubcategoriesAsync(string productId, string subcategoryId)
        {
            await _productService.AddProductSubcategoriesByIdAsync(productId, subcategoryId);
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ProductsResponse>> GetAllAsync([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var imageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var response = await _productService.GetAllProductsAsync(filter, imageSrc, route);
            return Ok(response);
        }

        [HttpGet("id")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDto>> GetAsync(string id)
        {
            var imageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var product = await _productService.GetProductAsync(id, imageSrc);
            return Ok(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAsync([FromForm] ProductUpdate product)
        {
            await _productService.UpdateProductAsync(_hostEnvironment.ContentRootPath, product);
            return Ok();
        }

        [HttpDelete("delete/{id}")]
        [AllowAnonymous]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteProductAsync(string id)
        {
            await _productService.RemoveProductsAsync(id);
            return Ok();
        }

        [HttpDelete("delete/category/{id}")] 
        [AllowAnonymous]
        public async Task<ActionResult> DeleteProductCategoriesAsync(string id)
        {
            await _productService.RemoveProductCategoriesAsync(id);
            return Ok();
        }

        [HttpDelete("delete/subcategory/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> DeleteProductSubcategoriesAsync(string productId, string subcategoryId)
        {
            await _productService.RemoveProductSubcategoriesAsync(productId, subcategoryId);
            return Ok();
        }
    }
}
