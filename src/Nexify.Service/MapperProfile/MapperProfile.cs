using AutoMapper;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Entities.Products;
using Nexify.Domain.Entities.Subcategories;
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
            CreateMap<Subcategory, SubcategoryDto>().ReverseMap();

            CreateMap<Category, CategoryResponse>()
                .ForMember(dest => dest.ImageSrc, opt => opt.MapFrom(src => src.ImageName))
                .ForMember(dest => dest.Subcategories, opt => opt.MapFrom(src => src.Subcategories))
                .ReverseMap();

            CreateMap<Subcategory, SubcategoryResponse>().ReverseMap();

            CreateMap<Category, CategoriesResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.CategoryName))
                .ForMember(dest => dest.ImageSrc, opt => opt.MapFrom(src => src.ImageName))
                .ReverseMap();
            CreateMap<Product, CategoryProducts>().ReverseMap();
        }

    }
}
