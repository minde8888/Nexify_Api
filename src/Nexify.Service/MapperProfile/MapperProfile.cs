using AutoMapper;
using Nexify.Domain.Entities.Attributes;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Entities.Posts;
using Nexify.Domain.Entities.Products;
using Nexify.Domain.Entities.Subcategories;
using Nexify.Service.Dtos.Attributes;
using Nexify.Service.Dtos.Blog;
using Nexify.Service.Dtos.Category;
using Nexify.Service.Dtos.Post;
using Nexify.Service.Dtos.Product;

namespace Nexify.Service.MapperProfile
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, ProductRequest>().ReverseMap();
            CreateMap<Product, ProductUpdate>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();
            CreateMap<Product, CategoryProducts>().ReverseMap();

            CreateMap<Subcategory, SubcategoryDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SubcategoryId))
                .ReverseMap();
            CreateMap<Subcategory, AddSubcategory>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Title))
                .ReverseMap();
            CreateMap<Subcategory, SubcategoryResponse>()
                .ForMember(dest => dest.ImageSrc, opt => opt.MapFrom(src => src.ImageName))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SubcategoryId))
                .ReverseMap();
            CreateMap<Subcategory, SubcategoriesId>()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SubcategoryId))
                .ReverseMap();

            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Title))
                .ReverseMap();
            CreateMap<Category, CategoryResponse>()
                .ForMember(dest => dest.ImageSrc, opt => opt.MapFrom(src => src.ImageName))
                .ForMember(dest => dest.Subcategories, opt => opt.MapFrom(src => src.Subcategories))
                .ReverseMap();
            CreateMap<Category, CategoriesResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ImageSrc, opt => opt.MapFrom(src => src.ImageName))
                .ReverseMap();
            CreateMap<Category, AddCategories>()
                   .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Title))
                .ReverseMap();
            CreateMap<Category, CategoriesId>().ReverseMap();

            CreateMap<BlogCategory, BlogCategoryDto>().ReverseMap();
            CreateMap<BlogCategory, BlogCategoryResponse>()
                .ForMember(dest => dest.ImageSrc, opt => opt.MapFrom(src => src.ImageName))
                .ReverseMap();

            CreateMap<PostUpdateRequest, Post>().ReverseMap();
            CreateMap<PostDto, Post>().ReverseMap();
            CreateMap<PostRequest, Post>().ReverseMap();
            CreateMap<PostUpdateRequest, Post>().ReverseMap();

            CreateMap<ItemsAttributes, AttributesUpdate>().ReverseMap();
            CreateMap<ItemsAttributes, AttributesRequest>().ReverseMap();
            CreateMap<ItemsAttributes, ItemsAttributesDto>().ReverseMap();
            CreateMap<ItemsAttributes, AttributesId>().ReverseMap();
        }
    }
}
