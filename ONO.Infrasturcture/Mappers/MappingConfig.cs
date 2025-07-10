using AutoMapper;
using ONO.Application.DTOs.GuestProductsDTOs;
using ONO.Application.DTOs.ProductsDTOs;
using ONO.Application.DTOs.UserProductsDTOs;
using ONO.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Infrasturcture.Mappers
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.cateName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id));

            CreateMap<UserProducts, UserProductsDTOs>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ageRange, opt => opt.MapFrom(src => src.Product.AgeRange))
                .ForMember(dest => dest.cateName, opt => opt.MapFrom(src => src.Product.Category.Name))
                .ForMember(dest => dest.description, opt => opt.MapFrom(src => src.Product.Description))
                .ForMember(dest => dest.imageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.StockUnit, opt => opt.MapFrom(src => src.Product.StockUnit))
                .ForMember(dest => dest.Reserved, opt => opt.MapFrom(src => src.Product.Reserved));

            CreateMap<TemporaryReservation, GuestProductsDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ageRange, opt => opt.MapFrom(src => src.Product.AgeRange))
                .ForMember(dest => dest.cateName, opt => opt.MapFrom(src => src.Product.Category.Name))
                .ForMember(dest => dest.description, opt => opt.MapFrom(src => src.Product.Description))
                .ForMember(dest => dest.imageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.StockUnit, opt => opt.MapFrom(src => src.Product.StockUnit))
                .ForMember(dest => dest.Reserved, opt => opt.MapFrom(src => src.Product.Reserved));
        }
    }
}
