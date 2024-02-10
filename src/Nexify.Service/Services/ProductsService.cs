using AutoMapper;
using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Entities.Products;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Dtos;
using Nexify.Service.Validators;
using Nexify.Service.Interfaces;
using Nexify.Data.Helpers;
using Nexify.Domain.Entities.Categories;

namespace Nexify.Service.Services
{
    public class ProductsService
    {
        private readonly IMapper _mapper;
        private readonly IImagesService _imagesService;
        private readonly IProductsRepository _productsRepository;
        private readonly IProductCategoryRepository _categoriesRepository;
        private readonly IUriService _uriService;
        private readonly DiscountService _discountService;

        public ProductsService(
            IImagesService imagesService,
                IProductsRepository productsRepository,
                    IProductCategoryRepository categoriesRepository,
                        IMapper mapper,
                            IUriService uriService,
                                DiscountService discountService)
        {
            _imagesService = imagesService ?? throw new ArgumentNullException(nameof(imagesService));
            _productsRepository = productsRepository ?? throw new ArgumentNullException(nameof(productsRepository));
            _categoriesRepository = categoriesRepository ?? throw new ArgumentNullException(nameof(categoriesRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _uriService = uriService ?? throw new ArgumentNullException(nameof(uriService));
            _discountService = discountService ?? throw new ArgumentException(nameof(discountService));
        }

        public async Task AddProductAsync(ProductRequest product)
        {
            var validationResult = await new ProductRequestValidator().ValidateAsync(product);
            ValidationExceptionHelper.ThrowIfInvalid<ProductValidationException>(validationResult);

            var result = await _imagesService.MapAndSaveImages<ProductRequest, Product>(product, product.Images, "ImagesNames");
            await _productsRepository.AddAsync(result);

            if (product.CategoriesIds != null && product.CategoriesIds.Any())
            {
                foreach (var categoryId in product.CategoriesIds)
                {
                    await _categoriesRepository.AddProductCategoriesAsync(categoryId, result.Id);
                }
            }
        }

        public async Task AddProductSubcategoriesByIdAsync(CategoryItems productCategories)
        {
            var validationResult = await new CategoryItemsValidator().ValidateAsync(productCategories);
            ValidationExceptionHelper.ThrowIfInvalid<ProductCategoriesValidationException>(validationResult);

            await _categoriesRepository.AddProductCategoriesAsync(new Guid(productCategories.CategoryId), new Guid(productCategories.ProductId));
        }

        public async Task<ProductsResponse> GetAllProductsAsync(
            PaginationFilter filter,
                string imageSrc,
                    string route)
        {
            var validationResult = await new PaginationFilterValidator().ValidateAsync(filter);
            ValidationExceptionHelper.ThrowIfInvalid<PaginationValidationException>(validationResult);

            var validFilter = filter ?? new PaginationFilter();
            var result = await _productsRepository.FetchAllAsync(validFilter);

            var pageParams = new PagedParams<Product>(
                result.Items,
                validFilter,
                result.TotalCount,
                _uriService,
                route);

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

            var processedPost = await _imagesService.MapAndProcessObjectListAsync<ProductUpdate, Product>(
                product,
                obj => obj.Images,
                contentRootPath,
                "ImageNames"
            );

            await _productsRepository.ModifyAsync(processedPost);

            if (product.CategoriesIds != null && product.CategoriesIds.Any())
            {
                await _categoriesRepository.DeleteRangeProductCategories(processedPost.Id);

                foreach (var categoryId in product.CategoriesIds)
                {
                    await _categoriesRepository.AddProductCategoriesAsync(categoryId, processedPost.Id);
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

        private List<ProductDto> MapPagedProducts(PagedParams<Product> pageParams, string imageSrc)
        {
            var pagedProducts = PaginationService.CreatePagedResponse(pageParams);

            var result = pagedProducts.Data.Select(product =>
            {
                if (product.Discount != null)
                    product.Price = _discountService.DiscountCounter(product.Discount, product.Price);

                var productDto = _mapper.Map<ProductDto>(product);

                if (productDto.ImageNames.Count != 0)
                {
                    var imageNames = product.ImagesNames;
                    var imageSrcs = imageNames.Select(name => $"{imageSrc}/Images/{name.Trim()}");
                    productDto.ImageSrc = imageSrcs.ToList();
                }

                return productDto;
            }).ToList();

            return result;
        }
    }
}
