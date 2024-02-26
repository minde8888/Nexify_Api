﻿using Microsoft.AspNetCore.Mvc;
using Nexify.Domain.Entities.Attributes;
using Nexify.Service.Dtos;
using Nexify.Service.Services;

namespace Nexify.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AttributeController : Controller
    {
        private readonly AttributesServices _attributesService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AttributeController(AttributesServices attributesService, IWebHostEnvironment hostEnvironment)
        {
            _attributesService = attributesService ?? throw new ArgumentNullException(nameof(attributesService));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddNewAttributesAsync([FromForm] List<AttributesRequest> attributes)
        {
            await _attributesService.AddAttributesAsync(attributes);

            return Ok();
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Attributes>>> GetAll()
        {
            var imageSrc = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var attributes = await _attributesService.GetAllAddAttributesAsync(imageSrc);
            return Ok(attributes);
        }
    }
}
