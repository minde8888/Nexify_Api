using AutoMapper;
using Nexify.Domain.Entities.Categories;
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
        private readonly IProductsCategoriesRepository _productCategoriesRepository;
        private readonly IUriService _uriService;

        public ProductsService(
            IImagesService imagesService,
                IProductsRepository productsRepository,
                    IProductsCategoriesRepository productCategoriesRepository,
                        IMapper mapper,
            IUriService uriService)
        {
            _imagesService = imagesService ?? throw new ArgumentNullException(nameof(imagesService));
            _productsRepository = productsRepository ?? throw new ArgumentNullException(nameof(productsRepository));
            _productCategoriesRepository = productCategoriesRepository ?? throw new ArgumentNullException(nameof(productCategoriesRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _uriService = uriService ?? throw new ArgumentNullException(nameof(uriService));
        }

        public async Task AddProductAsync(ProductRequest product)
        {
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
            await _productCategoriesRepository.AddProductCategoriesAsync(new Guid(productCategories.CategoryId), new Guid(productCategories.ProductId) );
        }

        public async Task<ProductsResponse> GetAllProductsAsync(
            PaginationFilter filter,
            string imageSrc,
            string route)
        {

            var validationResult = await new PaginationFilterValidator().ValidateAsync(filter);

            if (!validationResult.IsValid)
            {
                throw new PaginationValidationException(validationResult.Errors.ToString());
            }

            var validFilter = filter ?? new PaginationFilter();
            var result = await _productsRepository.GetAllAsync(validFilter);

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
            var product = await _productsRepository.GetAsync(Guid.Parse(id));

            return MapProduct(product, imageSrc);
        }

        public async Task UpdateProductAsync(string contentRootPath, ProductUpdate product)
        {
            if (product == null)
                throw new ProductException("Product can't by null");

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
            await _productsRepository.UpdateAsync(result);
        }

        public async Task RemoveProductsAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ProductException("Product id can't by null");
            Guid newId = new(id);

            await _productsRepository.RemoveAsync(newId);
        }

        public async Task RemoveProductCategoriesAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ProductException("Product id can't by null");
            Guid newId = new(id);

            await _productCategoriesRepository.DeleteCategoriesProductAsync(newId);
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
                Description = product.Description,
                Price = product.Price,
                ImageSrc = imageUrls
            };
        }

        private List<ProductDto> MapPagedProducts(PagedParams<Product> pageParams, string imageSrc)
        {
            var pagedProducts = PaginationService.CreatePagedResponse(pageParams);

            var result = pagedProducts.Data.Select(product =>
            {
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
