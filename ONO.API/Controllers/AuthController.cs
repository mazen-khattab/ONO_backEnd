using CurrencyExchange_Practice.Core.AuthDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using ONO.Application.DTOs.AuthDTOs;
using ONO.Application.Interfaces;
using ONO.Application.Services;
using ONO.Core.Entities;
using ONO.Core.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace ONO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly IAuthServices _authService;
        readonly RoleManager<Role> _roleManager;
        readonly UserManager<User> _userManager;
        readonly IServices<RefreshToken> _refreshService;
        readonly IConfiguration _config;

        public AuthController(IAuthServices authService, RoleManager<Role> roleManager, UserManager<User> userManager, IServices<RefreshToken> refreshService, IConfiguration config)
            => (_authService, _roleManager, _userManager, _refreshService, _config) = (authService, roleManager, userManager, refreshService, config);

        
        private async Task GenerateCookies(AuthServiceResponseDto authServiceResponseDto)
        {
            var accessTokenCookie = new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpDate"]!))
            };

            var refreshTokenCookie = new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(double.Parse(_config["Jwt:ExpDate"]!))
            };

            Response.Cookies.Append("accessToken", authServiceResponseDto.AccessToken, accessTokenCookie);
            Response.Cookies.Append("refreshToken", authServiceResponseDto.RefreshToken, refreshTokenCookie);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"refresh cookies: {Request.Cookies["refreshToken"]}");
            Console.WriteLine($"access cookies: {Request.Cookies["accessToken"]}");
            Console.ResetColor();

            await Task.CompletedTask;
        }


        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterDto register)
        {
            var result = await _authService.RegisterAsync(register);

            if (!result.IsSucceed) { return BadRequest(result.Message); }

            await GenerateCookies(result);

            var user = await _userManager.FindByEmailAsync(register.Email);
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                userName = user.UserName,
                userId = user.Id,
                userRoles = roles
            });
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var result = await _authService.LoginAsync(login);

            if (!result.IsSucceed) { return Unauthorized(result.Message); }

            await GenerateCookies(result);

            var user = await _userManager.FindByEmailAsync(login.Email);
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                userName = user.UserName,
                userId = user.Id,
                userRoles = roles
            });
        }


        [HttpPost]
        [Route("Refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken is null) { return Unauthorized("invalid refresh token!"); }

            var result = await _authService.ValidateRefreshToken(refreshToken);
            if (!result.IsSucceed) { return Unauthorized(result.Message); }

            await GenerateCookies(result);

            return Ok(new { message = "Tokens refreshed" });
        }


        [HttpPost]
        [Route("make-admin")]
        public async Task<IActionResult> MakeAdmin([FromBody] UpdatePermissionDto updatePermissionDto)
        {
            var makeAdmin = await _authService.MakeAdminAsynce(updatePermissionDto);

            return Ok(makeAdmin);
        }


        [HttpPost]
        [Route("make-owner")]
        public async Task<IActionResult> MakeOwner([FromBody] UpdatePermissionDto updatePermissionDto)
        {
            var makeOwner = await _authService.MakeOwnerAsynce(updatePermissionDto);

            return Ok(makeOwner);
        }


        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("accessToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            return Ok(new { message = "Logged out successfully" });
        }


        [HttpGet]
        [Route("Get-Profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken is null) { return Unauthorized(); }

            var token = await _refreshService.GetAsync(rt => rt.Token == refreshToken, includes: rt => rt.User);
            if (token is null || token.ExpDate < DateTime.UtcNow) { return Unauthorized(); }

            var user = token.User;
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                userName = user.UserName,
                userId = user.Id,
                userRole = roles
            });
        }
    }
}
