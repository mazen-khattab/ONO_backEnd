using AutoMapper;
using AutoMapper.Execution;
using Microsoft.AspNetCore.Http;
using ONO.Application.DTOs.GuestProductsDTOs;
using ONO.Application.DTOs.UsersCartDTOs;
using ONO.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.Mappers
{
    public class GuestCartImageResolver : IValueResolver<GuestCart, GuestCartDto, string>
    {
        readonly IHttpContextAccessor _httpContextAccessor;

        public GuestCartImageResolver(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;


        public string Resolve(GuestCart source, GuestCartDto destination, string destMember, ResolutionContext context)
        {
            var request = _httpContextAccessor.HttpContext?.Request;

            if (!string.IsNullOrEmpty(source.Product.ImageUrl) || request is { })
            {
                var baseUrl = $"{request.Scheme}://{request.Host}";

                return $"{baseUrl}/Images/{source.Product.ImageUrl}";
            }

            return string.Empty;
        }
    }
}
