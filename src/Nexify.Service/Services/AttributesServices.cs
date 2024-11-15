﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Nexify.Data.Helpers;
using Nexify.Domain.Entities.Attributes;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Dtos.Attributes;
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

                await _attributesReposutory.AddAsync(_mapper.Map<ItemsAttributes>(item));
            }

        }

        public async Task<List<ItemsAttributes>> GetAllAddAttributesAsync(string imageSrc)
        {
            var attributes = await _attributesReposutory.GetAllAsync();

            ProcessImagesForCategories(attributes, imageSrc, "ImageName");

            var mappedAttributes = _mapper.Map<List<ItemsAttributes>>(attributes);

            return mappedAttributes;
        }

        private void ProcessImagesForCategories(IEnumerable<ItemsAttributes> attributes, string imageSrc, string propertyName)
        {
            foreach (var attribute in attributes)
            {
                if (propertyName == "ImageName")
                {
                    attribute.ImageName = _imagesService.ProcessImages(attribute, imageSrc, propertyName);
                }
            }
        }

        public async Task UpdateAttributesAsync(AttributesUpdate attribute)
        {
            var validationResult = await new AttributesUpdateValidator().ValidateAsync(attribute);
            ValidationExceptionHelper.ThrowIfInvalid<AttributesValidationException>(validationResult);

            var mapAttribute = _mapper.Map<ItemsAttributes>(attribute);

            if (attribute.Image != null)
            {
                mapAttribute.ImageName = await _imagesService.SaveImagesAsync(new List<IFormFile> { attribute.Image });
            }

            await _attributesReposutory.ModifyAsync(mapAttribute);
        }

        public async Task RemovePostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new AttributesException("Attributes id can't by null");

            await _attributesReposutory.RemoveAsync(Guid.Parse(id));
        }
    }
}
