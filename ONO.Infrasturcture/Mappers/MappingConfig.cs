using AutoMapper;
using ONO.Application.DTOs.ProductsDTOs;
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
            CreateMap<Product, ProductDto>().ForMember(dest => dest.cateName, opt => opt.MapFrom(src => src.Category.Name));
        }
    }
}
