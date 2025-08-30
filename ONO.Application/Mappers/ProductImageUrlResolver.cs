using AutoMapper;
using AutoMapper.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ONO.Application.DTOs.ProductsDTOs;
using ONO.Application.DTOs.UsersCartDTOs;
using ONO.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.Mappers
{
    public class ProductImageUrlResolver : IValueResolver<Product, ProductDto, string>
    {
        readonly IHttpContextAccessor _httpContextAccessor;

        public ProductImageUrlResolver(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;


        public string Resolve(Product source, ProductDto destination, string destMember, ResolutionContext context)
        {
            var request = _httpContextAccessor.HttpContext?.Request;

            if (!string.IsNullOrEmpty(source.ImageUrl) || request is { })
            {
                var baseUrl = $"{request.Scheme}://{request.Host}";

                return $"{baseUrl}/Images/{source.ImageUrl}";
            }

            return string.Empty;
        }
    }
}
