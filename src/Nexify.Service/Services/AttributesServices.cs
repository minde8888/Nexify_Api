using AutoMapper;
using Nexify.Data.Helpers;
using Nexify.Domain.Entities.Attributes;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Dtos;
using Nexify.Service.Validators;

namespace Nexify.Service.Services
{
    public class AttributesServices
    {
        private readonly IMapper _mapper;
        private readonly IImagesService _imagesService;
        private readonly IAttributesRepository _attributesReposutory;

        public AttributesServices(
            IImagesService imagesService,
                IAttributesRepository attributesReposutory,
                    IMapper mapper)
        {
            _imagesService = imagesService ?? throw new ArgumentNullException(nameof(imagesService));
            _attributesReposutory = attributesReposutory ?? throw new ArgumentNullException(nameof(attributesReposutory)); ;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task AddAttributesAsync(List<AttributesRequest> attributes)
        {
            foreach (var item in attributes)
            {
                var validationResult = await new AttributesRequestValidator().ValidateAsync(item);
                ValidationExceptionHelper.ThrowIfInvalid<AttributesValidationException>(validationResult);

                if (item.Images != null && item.Images.Any())
                {
                    var mappedAttributes = await _imagesService.MapAndSaveImages<AttributesRequest, Attributes>(item, item.Images, "ImagesNames");
                    await _attributesReposutory.AddAsync(mappedAttributes);
                }
                else
                {
                    await _attributesReposutory.AddAsync(_mapper.Map<Attributes>(item));
                }
            }
  
        }

    }
}
