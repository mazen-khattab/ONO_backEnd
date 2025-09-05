using AutoMapper;
using AutoMapper.Execution;
using Microsoft.AspNetCore.Http;
using ONO.Application.DTOs.ProductImagesDTOs;
using ONO.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.Mappers
{
    public class GallariesUrlResolver : IValueResolver<ProductImage, ProductImagesDto, string>
    {
        readonly IHttpContextAccessor _httpContextAccessor;
        public GallariesUrlResolver(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

        public string Resolve(ProductImage source, ProductImagesDto destination, string destMember, ResolutionContext context)
        {
            var request = _httpContextAccessor.HttpContext?.Request;

            if (!string.IsNullOrEmpty(source.ImageUrl) || request is { })
            {
                var baseUrl = $"{request.Scheme}://{request.Host}";

                return $"{baseUrl}/Gallary/{source.ImageUrl}";
            }

            return string.Empty;
        }
    }
}
