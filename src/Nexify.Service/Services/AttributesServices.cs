using AutoMapper;
using Nexify.Data.Helpers;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Dtos;
using Nexify.Service.Validators;

namespace Nexify.Service.Services
{
    public class AttributesServices
    {
        private readonly IImagesService _imagesService;
        private readonly IAttributesReposutory _attributesReposutory;

        public AttributesServices(
            IImagesService imagesService,
                IAttributesReposutory attributesReposutory)
        {
            _imagesService = imagesService ?? throw new ArgumentNullException(nameof(imagesService));
            _attributesReposutory = attributesReposutory ?? throw new ArgumentNullException(nameof(attributesReposutory)); ;
        }

        public async Task AddAttributesAsync(AttributesRequest attributes)
        {
            var validationResult = await new AttributesRequestValidator().ValidateAsync(attributes);
            ValidationExceptionHelper.ThrowIfInvalid<AttributesValidationException>(validationResult);

            var result = await _imagesService.MapAndSaveImages<AttributesRequest, Domain.Entities.Attributes.Attribute>(attributes, attributes.Images, "ImagesNames");
            await _attributesReposutory.AddAsync(result);
        }
    }
}
