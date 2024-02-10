using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Entities.Products;
using Nexify.Service.Dtos;
using Nexify.Service.Services;

namespace Nexify.Api.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

      
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddNewProductAsync([FromForm] ProductRequest product)
        {
            await _productService.AddProductAsync(product);
            return Ok();
        }


        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductsResponse>> GetAllAsync([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var imageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var response = await _productService.GetAllProductsAsync(filter, imageSrc, route);
            return Ok(response);
        }
          
        [HttpPut("Update")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync([FromForm] ProductUpdate product)
        {
            await _productService.UpdateProductAsync(_hostEnvironment.ContentRootPath, product);
            return Ok();
        }

        [HttpDelete("id")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteProductAsync(string id)
        {
            await _productService.RemoveProductsAsync(id);
            return Ok();
        }
     }
}
