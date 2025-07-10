using CurrencyExchange_Practice.Core.AuthDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ONO.Application.DTOs.AuthDTOs;
using ONO.Application.Interfaces;
using ONO.Core.AnotherObjects;
using ONO.Core.Entities;
using ONO.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.Services
{
    public class AuthServices : IAuthServices
    {
        readonly UserManager<User> _userManager;
        readonly RoleManager<Role> _roleManager;
        readonly SignInManager<User> _signInManager;
        readonly IServices<RefreshToken> _services;
        readonly IConfiguration _config;

        public AuthServices(UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager, IConfiguration config, IServices<RefreshToken> services)
            => (_userManager, _roleManager, _signInManager, _config, _services) = (userManager, roleManager, signInManager, config, services);

        public async Task<AuthServiceResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user is null)
            {
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Invalid Cedentials"
                };
            }

            var isPasswordCorrect = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!isPasswordCorrect.Succeeded)
            {
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Invalid Cedentials"
                };
            }

            return new AuthServiceResponseDto()
            {
                IsSucceed = true,
                Message = "Login successful",
                AccessToken = await CreateAccessToken(user),
                RefreshToken = await CreateRefreshToken(user)
            };
        }

        public async Task<AuthServiceResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var user = await _userManager.FindByEmailAsync(registerDto.Email);

            if (user is { })
            {
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Email is already taken."
                };
            }

            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Passwords do not match"
                };
            }

            User newUser = new()
            {
                Fname = registerDto.FirstName,
                Lname = registerDto.LastName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.phoneNumber,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            int number = GenerateRandomNumbers();
            newUser.UserName = $"{newUser.Fname[0..1]}{newUser.Lname[0..1]}_{number}".ToUpper();

            var createdUser = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!createdUser.Succeeded)
            {
                string errorMessage = "User Creation Failed Because:\n";

                foreach (var error in createdUser.Errors)
                {
                    errorMessage += $"# {error.Description}";
                }

                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = errorMessage
                };
            }

            await _userManager.AddToRoleAsync(newUser, StaticUserRoles.USER);

            return new AuthServiceResponseDto()
            {
                IsSucceed = true,
                Message = "Register successful",
                AccessToken = await CreateAccessToken(newUser),
                RefreshToken = await CreateRefreshToken(newUser)
            };
        }

        public async Task<string> CreateAccessToken(User user)
        {
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, $"{user.Fname}_{user.Lname}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("jwtId", Guid.NewGuid().ToString()),
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                userClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:key"]));

            var tokenObject = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpDate"]!)),
                claims: userClaims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
                );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

            return token;
        }

        public async Task<string> CreateRefreshToken(User user)
        {
            var randomNumbers = new byte[64];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumbers);
            }

            string token = Convert.ToBase64String(randomNumbers);

            RefreshToken refreshToken = new()
            {
                Token = token,
                ExpDate = DateTime.UtcNow.AddDays(double.Parse(_config["Jwt:ExpDate"]!)),
                UserId = user.Id
            };

            var currentRefreshToken = await _services.GetAsync(rt => rt.UserId == user.Id);

            if (currentRefreshToken is { })
            {
                currentRefreshToken.ExpDate = DateTime.UtcNow.AddDays(double.Parse(_config["Jwt:ExpDate"]!));
                currentRefreshToken.Token = token;

                await _services.UpdateAsync(currentRefreshToken);
            }
            else
            {
                await _services.AddAsync(refreshToken);
            }

            await _services.SaveChangesAsync();
            return token;
        }

        public async Task<AuthServiceResponseDto> ValidateRefreshToken(string token)
        {
            var refreshToken = await _services.GetAsync(rt => rt.Token == token, includes: rt => rt.User);

            if (refreshToken is null || refreshToken.ExpDate < DateTime.UtcNow)
            {
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Invalid refresh token!"
                };
            }

            return new AuthServiceResponseDto()
            {
                IsSucceed = true,
                Message = "secceeded",
                AccessToken = await CreateAccessToken(refreshToken.User),
                RefreshToken = await CreateRefreshToken(refreshToken.User)
            };
        }

        public async Task<AuthServiceResponseDto> MakeAdminAsynce(UpdatePermissionDto updatePermissionDto)
        {
            var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);

            if (user is null)
            {
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Invalid User Name"
                };
            }

            await _userManager.AddToRoleAsync(user, StaticUserRoles.ADMIN);

            return new AuthServiceResponseDto()
            {
                IsSucceed = true,
                Message = "User is now an Admin"
            };
        }

        public async Task<AuthServiceResponseDto> MakeOwnerAsynce(UpdatePermissionDto updatePermissionDto)
        {
            var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);

            if (user is null)
            {
                return new AuthServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Invalid User Name"
                };
            }

            await _userManager.AddToRoleAsync(user, StaticUserRoles.OWNER);

            return new AuthServiceResponseDto()
            {
                IsSucceed = true,
                Message = "User is now an Owner"
            };
        }

        public int GenerateRandomNumbers()
        {
            Random rn = new();
            int randomNumber = 0;

            for (int i = 1; i <= 5; i++)
            {
                randomNumber += rn.Next(0, 50);
            }

            return randomNumber;
        }
    }
}
