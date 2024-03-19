using AutoMapper;
using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Entities.Products;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Validators;
using Nexify.Service.Interfaces;
using Nexify.Data.Helpers;
using Nexify.Service.Dtos.Product;
using Nexify.Data.Migrations;
using Nexify.Data.Repositories;

namespace Nexify.Service.Services
{
    public class ProductsService
    {
        private readonly IMapper _mapper;
        private readonly IImagesService _imagesService;
        private readonly IProductsRepository _productsRepository;
        private readonly IProductCategoryRepository _categoriesRepository;
        private readonly IProductSubcategoryRepository _subcategoryRepository;
        private readonly IUriService _uriService;

        public ProductsService(
            IImagesService imagesService,
                IProductsRepository productsRepository,
                    IProductCategoryRepository categoriesRepository,
                        IProductSubcategoryRepository subcategoryRepository,
                            IMapper mapper,
                                IUriService uriService)
        {
            _imagesService = imagesService ?? throw new ArgumentNullException(nameof(imagesService));
            _productsRepository = productsRepository ?? throw new ArgumentNullException(nameof(productsRepository));
            _categoriesRepository = categoriesRepository ?? throw new ArgumentNullException(nameof(categoriesRepository));
            _subcategoryRepository = subcategoryRepository ?? throw new ArgumentNullException(nameof(subcategoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _uriService = uriService ?? throw new ArgumentNullException(nameof(uriService));
        }

        public async Task AddProductAsync(ProductRequest product)
        {
            var validationResult = await new ProductRequestValidator().ValidateAsync(product);
            ValidationExceptionHelper.ThrowIfInvalid<ProductValidationException>(validationResult);

            var result = await _imagesService.MapAndSaveImages<ProductRequest, Product>(
                product,
                product.Images,
                "ImagesNames");

            await _productsRepository.AddAsync(result);

            if (product.CategoriesIds != null && product.CategoriesIds.Any())
            {
                foreach (var categoryId in product.CategoriesIds)
                {
                    await _categoriesRepository.AddProductCategoriesAsync(categoryId, result.Id);
                }
            }
        }

        public async Task<ProductsResponse> GetAllProductsAsync(
            PaginationFilter filter,
                string imageSrc,
                    string route)
        {
            var validationResult = await new PaginationFilterValidator().ValidateAsync(filter);
            ValidationExceptionHelper.ThrowIfInvalid<PaginationValidationException>(validationResult);

            var result = await _productsRepository.FetchAllAsync(filter ?? new PaginationFilter());

            var pageParams = new PagedParams<Product>(result.Items, filter, result.TotalCount, _uriService, route);

            var pagedProducts = PaginationService.CreatePagedResponse(pageParams);

            var productDtos = MapPagedProducts(pageParams, imageSrc);

            return new ProductsResponse
            {
                PageNumber = pagedProducts.PageNumber,
                PageSize = pagedProducts.PageSize,
                TotalPages = pagedProducts.TotalPages,
                TotalRecords = pagedProducts.TotalRecords,
                NextPage = pagedProducts.NextPage,
                PreviousPage = pagedProducts.PreviousPage,
                Products = productDtos
            };
        }

        public async Task UpdateProductAsync(string contentRootPath, ProductUpdate product)
        {
            await ValidateProductUpdate(product);

            var processedProduct = await _imagesService.MapAndProcessObjectListAsync<ProductUpdate, Product>(
                product,
                contentRootPath,
                "ImagesNames",
                product.ImagesNames
            );

            await _productsRepository.ModifyAsync(processedProduct);

            var categoryUpdater = new ProductCategoryUpdater(_categoriesRepository);
            var subcategoryUpdater = new ProductSubcategoryUpdater(_subcategoryRepository);
            var attributeUpdater = new ProductAttributeUpdater(_categoriesRepository);

            await ApplyUpdates(processedProduct.Id, product.CategoriesIds, categoryUpdater);
            await ApplyUpdates(processedProduct.Id, product.SubcategoriesIds, subcategoryUpdater);
            await ApplyUpdates(processedProduct.Id, product.AttributesIds, attributeUpdater);
        }

        private async Task ApplyUpdates(Guid productId, IEnumerable<Guid> relationIds, IProductRelationUpdater updater)
        {
            if (relationIds != null && relationIds.Any())
            {
                await updater.DeleteRangeAsync(productId);
                foreach (var id in relationIds)
                {
                    await updater.AddRelationAsync(id, productId);
                }
            }
        }

        private async Task ValidateProductUpdate(ProductUpdate product)
        {
            var validationResult = await new ProductUpdateValidator().ValidateAsync(product);
            ValidationExceptionHelper.ThrowIfInvalid<PaginationValidationException>(validationResult);
        }

        public async Task RemoveProductsAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ProductException("Product id can't by null");

            Guid newId = new(id);

            await _productsRepository.RemoveAsync(newId);
        }

        public List<ProductDto> MapPagedProducts(PagedParams<Product> pageParams, string imageSrc)
        {
            var pagedProducts = PaginationService.CreatePagedResponse(pageParams);
            var productDtos = _mapper.Map<List<ProductDto>>(pagedProducts.Data);

            productDtos.ForEach(dto =>
            {
                var correspondingProduct = pagedProducts.Data.FirstOrDefault(p => p.Id == dto.Id);
                if (correspondingProduct != null)
                {
                    SetImageSourcesForDto(dto, correspondingProduct, imageSrc);
                }
            });

            return productDtos;
        }

        private void SetImageSourcesForDto(ProductDto dto, Product product, string baseImageSrc)
        {
            Func<List<string>, List<string>> transformNamesToUrls = names => names
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct()
                .Select(name => $"{baseImageSrc}/Images/{name.Trim()}")
                .ToList();

            dto.ImageSrc = transformNamesToUrls(product.ImagesNames ?? new List<string>());
        }
    }
}
