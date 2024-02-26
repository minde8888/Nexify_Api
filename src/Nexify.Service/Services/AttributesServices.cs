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

                await _attributesReposutory.AddAsync(_mapper.Map<Attributes>(item));
            }

        }

        public async Task<List<Attributes>> GetAllAddAttributesAsync(string imageSrc)
        {
            var attributess = await _attributesReposutory.GetAllAsync();

            ProcessImagesForCategories(attributess, imageSrc, "ImagesNames");

            var mappedAttributes = _mapper.Map<List<Attributes>>(attributess);

            return mappedAttributes;
        }

        private void ProcessImagesForCategories(IEnumerable<Attributes> attributes, string imageSrc, string propertyName)
        {
            foreach (var attribute in attributes)
            {
                if (propertyName == "ImageNames")
                {
                    var processedImageName = _imagesService.ProcessImages(attribute, imageSrc, propertyName);
                  attribute.ImagesNames.Add(processedImageName);

                }
            }
        }
    }
}
