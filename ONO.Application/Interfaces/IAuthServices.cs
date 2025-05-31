using CurrencyExchange_Practice.Core.AuthDtos;
using ONO.Application.DTOs.AuthDTOs;
using ONO.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.Interfaces
{
    public interface IAuthServices
    {
        Task<AuthServiceResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthServiceResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthServiceResponseDto> MakeAdminAsynce(UpdatePermissionDto updatePermissionDto);
        Task<string> CreateAccessToken(User user);
        Task<string> CreateRefreshToken(User user);
        Task<AuthServiceResponseDto> MakeOwnerAsynce(UpdatePermissionDto updatePermissionDto);
        Task<AuthServiceResponseDto> ValidateRefreshToken(string token);
    }
}
