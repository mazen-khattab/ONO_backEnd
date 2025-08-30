using AutoMapper;
using ONO.Application.DTOs.GuestProductsDTOs;
using ONO.Application.DTOs.OrderDto;
using ONO.Application.DTOs.ProductsDTOs;
using ONO.Application.DTOs.UserDTOs;
using ONO.Application.DTOs.UsersCartDTOs;
using ONO.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.Mappers
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.cateName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom<ProductImageUrlResolver>());

            CreateMap<OrderDetails, OrderHistoryItemsDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Product.Description))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom<OrderHistoryImageResolver>())
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.StockUnit, opt => opt.MapFrom(src => src.Product.StockUnit))
                .ForMember(dest => dest.AgeRange, opt => opt.MapFrom(src => src.Product.AgeRange))
                .ForMember(dest => dest.ProductAmount, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Reserved, opt => opt.MapFrom(src => src.Product.Reserved));

            CreateMap<UsersCart, UsersCartDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ageRange, opt => opt.MapFrom(src => src.Product.AgeRange))
                .ForMember(dest => dest.cateName, opt => opt.MapFrom(src => src.Product.Category.Name))
                .ForMember(dest => dest.description, opt => opt.MapFrom(src => src.Product.Description))
                .ForMember(dest => dest.imageUrl, opt => opt.MapFrom<UserCartImageResolver>())
                .ForMember(dest => dest.price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.StockUnit, opt => opt.MapFrom(src => src.Product.StockUnit))
                .ForMember(dest => dest.Reserved, opt => opt.MapFrom(src => src.Product.Reserved));

            CreateMap<GuestCart, GuestCartDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ageRange, opt => opt.MapFrom(src => src.Product.AgeRange))
                .ForMember(dest => dest.cateName, opt => opt.MapFrom(src => src.Product.Category.Name))
                .ForMember(dest => dest.description, opt => opt.MapFrom(src => src.Product.Description))
                .ForMember(dest => dest.imageUrl, opt => opt.MapFrom<GuestCartImageResolver>())
                .ForMember(dest => dest.price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.StockUnit, opt => opt.MapFrom(src => src.Product.StockUnit))
                .ForMember(dest => dest.Reserved, opt => opt.MapFrom(src => src.Product.Reserved));

            CreateMap<UserAddress, UserAddressDto>().ReverseMap();

            CreateMap<UpdateUserDto, User>().ReverseMap()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Addresses));
        }
    }
}
