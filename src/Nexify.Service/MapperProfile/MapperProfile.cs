﻿using AutoMapper;
using Nexify.Domain.Entities.Categories;
using Nexify.Domain.Entities.Posts;
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
            CreateMap<Subcategory, SubcategoryDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.SubCategoryName))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SubcategoryId))
                .ReverseMap();

            CreateMap<Category, CategoryResponse>()
                .ForMember(dest => dest.ImageSrc, opt => opt.MapFrom(src => src.ImageName))
                .ForMember(dest => dest.Subcategories, opt => opt.MapFrom(src => src.Subcategories))
                .ReverseMap();

            CreateMap<Subcategory, SubcategoryResponse>()
                 .ForMember(dest => dest.ImageSrc, opt => opt.MapFrom(src => src.ImageName))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SubcategoryId))
                .ReverseMap();

            CreateMap<Category, CategoriesResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.CategoryName))
                .ForMember(dest => dest.ImageSrc, opt => opt.MapFrom(src => src.ImageName))
                .ReverseMap();
            CreateMap<Product, CategoryProducts>().ReverseMap();
            CreateMap<BlogCategory, BlogCategoryDto>().ReverseMap();
            CreateMap<PostRequest, Post>().ReverseMap();
            CreateMap<PostDto, Post>().ReverseMap();
        }

    }
}
