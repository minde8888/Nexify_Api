using AutoMapper;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Entities.Products;
using Nexify.Service.Dtos;

namespace Nexify.Service.MapperProfile
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, ProductRequest>().ReverseMap();
            CreateMap<Product, ProductUpdate>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CategoryResponse, Category>().ReverseMap();

            CreateMap<Category, CategoriesResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.CategoryName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ImageSrc, opt => opt.MapFrom(src => src.ImageName))
                .ReverseMap();
            CreateMap<Product, CategoryProducts>().ReverseMap();
        }

    }
}
