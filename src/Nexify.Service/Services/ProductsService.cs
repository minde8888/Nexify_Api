﻿using AutoMapper;
using Nexify.Domain.Entities.Pagination;
using Nexify.Domain.Entities.Products;
using Nexify.Domain.Exceptions;
using Nexify.Domain.Interfaces;
using Nexify.Service.Dtos;
using Nexify.Service.Validators;
using Nexify.Service.Interfaces;

namespace Nexify.Service.Services
{
    public class ProductsService
    {
        private readonly IMapper _mapper;
        private readonly IImagesService _imagesService;
        private readonly IProductsRepository _productsRepository;
        private readonly IItemCategoryRepository _productCategoriesRepository;
        private readonly IUriService _uriService;
        private readonly DiscountService _discountService;

        public ProductsService(
            IImagesService imagesService,
                IProductsRepository productsRepository,
                    IItemCategoryRepository productCategoriesRepository,
                        IMapper mapper,
                            IUriService uriService,
                                DiscountService discountService)
        {
            _imagesService = imagesService ?? throw new ArgumentNullException(nameof(imagesService));
            _productsRepository = productsRepository ?? throw new ArgumentNullException(nameof(productsRepository));
            _productCategoriesRepository = productCategoriesRepository ?? throw new ArgumentNullException(nameof(productCategoriesRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _uriService = uriService ?? throw new ArgumentNullException(nameof(uriService));
            _discountService = discountService ?? throw new ArgumentException(nameof(discountService));
        }

        public async Task AddProductAsync(ProductRequest product)
        {
            var validationResult = await new ProductRequestValidator().ValidateAsync(product);

            if (!validationResult.IsValid)
                throw new ProductValidationException(validationResult.Errors.ToString());

            var result = _mapper.Map<Product>(product);
            result.ProductId = Guid.NewGuid();

            if (product.Images?.Any() == true)
            {
                result.ImageName = string.
                    Join(",", await _imagesService.
                    SaveImages(product.Images));
            }

            await _productsRepository.AddAsync(result);
        }

        public async Task AddProductCategoriesAsync(ProductCategories productCategories)
        {
            var validationResult = await new ProductCategoriesValidator().ValidateAsync(productCategories);

            if (!validationResult.IsValid)
                throw new ProductCategoriesValidationException(validationResult.Errors.ToString());

            await _productCategoriesRepository.AddItemCategoriesAsync(new Guid(productCategories.CategoryId), new Guid(productCategories.ProductId));
        }

        public async Task AddProductSubcategoriesByIdAsync(string productId, string subcategoryId)
        {
            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(subcategoryId))
                throw new ProductException("Product id or subcategory id can't by null");

            await _productsRepository.AddProductSubcategoriesAsync(new Guid(productId), new Guid(subcategoryId));
        }

        public async Task<ProductsResponse> GetAllProductsAsync(
            PaginationFilter filter,
                string imageSrc,
                    string route)
        {

            var validationResult = await new PaginationFilterValidator().ValidateAsync(filter);

            if (!validationResult.IsValid)
                throw new PaginationValidationException(validationResult.Errors.ToString());

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

        public async Task<ProductDto> GetProductAsync(string id, string imageSrc)
        {
            if (string.IsNullOrEmpty(id))
                throw new ProductException("Product id can't by null");

            var product = await _productsRepository.RetrieveAsync(Guid.Parse(id));

            if (product.Discount != null)
                product.Price = _discountService.DiscountCounter(product.Discount, product.Price);

            return MapProduct(product, imageSrc);
        }

        public async Task UpdateProductAsync(string contentRootPath, ProductUpdate product)
        {
            var validationResult = await new ProductUpdateValidator().ValidateAsync(product);
            if (!validationResult.IsValid)
            {
                throw new ProductValidationException(validationResult.Errors.ToString());
            }

            if (product.Images != null)
            {
                var imagesNames = product.ImageName.Split(',');
                foreach (var imageName in imagesNames)
                {
                    var imagePath = Path.Combine(contentRootPath, "Images", imageName);
                    await _imagesService.DeleteImageAsync(imagePath);
                }

                product.ImageName = await _imagesService.SaveImages(product.Images);
            }

            var result = _mapper.Map<Product>(product);
            await _productsRepository.ModifyAsync(result);
        }

        public async Task RemoveProductsAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ProductException("Product id can't by null");

            Guid newId = new(id);

            await _productsRepository.RemoveAsync(newId);
        }

        public async Task RemoveProductCategoriesAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ProductException("Product id can't by null");

            Guid newId = new(id);

            await _productCategoriesRepository.DeleteCategoriesItemAsync(newId);
        }

        public async Task RemoveProductSubcategoriesAsync(string productId, string subcategoryId)
        {
            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(subcategoryId))
                throw new ProductException("Product id or subcategory id can't by null");

            await _productsRepository.DeleteSubcategoriesProductAsync(new Guid(productId), new Guid(subcategoryId));
        }

        private ProductDto MapProduct(Product product, string imageSrc)
        {
            if (product.ImageName == null)
                throw new ProductException("Product image name can't be null");

            var imageNames = product.ImageName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (imageNames.Length == 0)
                throw new ProductException("There are no images for the product.");

            var imageUrls = imageNames.Select(imageName => $"{imageSrc}/Images/{imageName}").ToList();
            return new ProductDto
            {
                Id = product.ProductId,
                Title = product.Title,
                Context = product.Context,
                Price = product.Price,
                ImageSrc = imageUrls
            };
        }

        private List<ProductDto> MapPagedProducts(PagedParams<Product> pageParams, string imageSrc)
        {
            var pagedProducts = PaginationService.CreatePagedResponse(pageParams);

            var result = pagedProducts.Data.Select(product =>
            {
                if (product.Discount != null)
                    product.Price = _discountService.DiscountCounter(product.Discount, product.Price);

                var productDto = _mapper.Map<ProductDto>(product);

                if (!string.IsNullOrEmpty(product.ImageName))
                {
                    var imageNames = product.ImageName.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    var imageSrcs = imageNames.Select(name => $"{imageSrc}/Images/{name.Trim()}");
                    productDto.ImageSrc = imageSrcs.ToList();
                }

                return productDto;
            }).ToList();

            return result;
        }
    }
}
