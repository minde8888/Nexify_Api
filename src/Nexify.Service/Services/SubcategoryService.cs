using AutoMapper;
using Microsoft.AspNetCore.Http;
using Nexify.Data.Repositories;
using Nexify.Domain.Entities.Products;
using Nexify.Domain.Entities.Subcategories;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Dtos;
using Nexify.Service.Validators;

namespace Nexify.Service.Services
{
    public class SubcategoryService
    {
        private readonly IMapper _mapper;
        private readonly ISubcategoryRepository _subcategoryRepository;
        private readonly IImagesService _imagesService;

        public SubcategoryService(
                       IMapper mapper,
                                  ISubcategoryRepository subcategoryRepository,
                                                    IImagesService imagesService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _subcategoryRepository = subcategoryRepository ?? throw new ArgumentNullException(nameof(subcategoryRepository));
            _imagesService = imagesService ?? throw new ArgumentNullException(nameof(imagesService));
        }

        public async Task AddSubCategoryAsync(SubcategoryDto subcategoryDto)
        {
            var varlidationResult = await new SubcategoryDtoValidator().ValidateAsync(subcategoryDto);
            if (!varlidationResult.IsValid)
                throw new SubcategoryValidationException(varlidationResult.Errors.ToString());

            var subcategory = _mapper.Map<Subcategory>(subcategoryDto);

            if (subcategoryDto.Image != null)
            {
                subcategory.ImageName = await _imagesService.SaveImages(new List<IFormFile> { subcategoryDto.Image });
            }

            await _subcategoryRepository.AddAsync(subcategory, subcategoryDto.ProductsId);
        }

        public async Task<SubcategoryResponse> GetSubCategoryAsync(string id, string imageSrc)
        {
            if (string.IsNullOrEmpty(id))
                throw new SubcategoryException("Subcategory id can't by null");

            var subcategory = await _subcategoryRepository.GetAsync(Guid.Parse(id));

            return MapProduct(subcategory, imageSrc);
        }

        public async Task UpdateSubCategoryAsync(SubcategoryDto subcategoryDto, string rootPath)
        {
            var varlidationResult = await new SubcategoryDtoValidator().ValidateAsync(subcategoryDto);
            if (!varlidationResult.IsValid)
                throw new SubcategoryValidationException(varlidationResult.Errors.ToString());

            var subcategory = _mapper.Map<Subcategory>(subcategoryDto);

            if (subcategoryDto.Image != null)
            {
                subcategory.ImageName = await _imagesService.SaveImages(new List<IFormFile> { subcategoryDto.Image });
            }

            await _subcategoryRepository.UpdateAsync(subcategory);
        } 
        
        public async Task DeleteSubCategoryAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new SubcategoryException("Subcategory id can't by null");

            await _subcategoryRepository.RemoveAsync(Guid.Parse(id));
        }

        private SubcategoryResponse MapProduct(Subcategory subcategory, string imageSrc)
        {
            if (subcategory.ImageName == null)
                throw new ProductException("Product image name can't be null");

            var imageNames = subcategory.ImageName;
            if (imageNames.Length == 0)
                throw new ProductException("There are no images for the product.");

            return new SubcategoryResponse
            {
                SubCategoryId = subcategory.SubcategoryId,
                SubCategoryName = subcategory.SubCategoryName,
                Description = subcategory.Description,
                ImageSrc = imageNames.Select(imageName => $"{imageSrc}/Images/{imageName}").ToString(),
                Products = _mapper.Map<List<ProductDto>>(subcategory.Products)
            };
        }
    }
}
