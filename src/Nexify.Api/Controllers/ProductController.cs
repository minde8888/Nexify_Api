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
        private readonly ProductsService _productsService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(ProductsService productsService, IWebHostEnvironment hostEnvironment)
        {
            _productsService = productsService ?? throw new ArgumentNullException(nameof(productsService));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddNewProductAsync([FromForm] ProductRequest products)
        {
            await _productsService.AddProductAsync(products);
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ProductRequest>> GetAllAsync([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var imageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var response = await _productsService.GetAllProductsAsync(filter, imageSrc, route);
            return Ok(response);
        }

        [HttpGet("id")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDto>> Get(string id)
        {
            var imageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var product = await _productsService.GetProductAsync(id, imageSrc);
            return Ok(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAsync([FromForm] ProductUpdate product)
        {
            await _productsService.UpdateProductAsync(_hostEnvironment.ContentRootPath, product);
            return Ok();
        }

        [HttpDelete("Delete/{id}")]
        [AllowAnonymous]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteProductAsync(string id)
        {
            await _productsService.RemoveProductsAsync(id);
            return Ok();
        }
    }
}
