using AutoMapper;
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
    public class UserCartImageResolver : IValueResolver<UsersCart, UsersCartDTO, string>
    {
        readonly IHttpContextAccessor _httpContextAccessor;     
        public UserCartImageResolver(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

        public string Resolve(UsersCart source, UsersCartDTO destination, string destMember, ResolutionContext context)
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
