using Microsoft.AspNetCore.Mvc;
using Nexify.Service.Dtos;
using Nexify.Service.Services;

namespace Nexify.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AttributController : Controller
    {
        private readonly AttributesServices _attributesService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AttributController(AttributesServices attributesService, IWebHostEnvironment hostEnvironment)
        {
            _attributesService = attributesService ?? throw new ArgumentNullException(nameof(attributesService));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddNewAttributesAsync([FromForm] AttributesRequest attributes)
        {
            await _attributesService.AddAttributesAsync(attributes);
            return Ok();
        }
    }
}
