using CurrencyExchange_Practice.Core.AuthDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ONO.Application.DTOs.AuthDTOs;
using ONO.Application.Interfaces;
using ONO.Core.AnotherObjects;
using ONO.Core.Entities;
using ONO.Core.Interfaces;
using Serilog;

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
        readonly ILogger<AuthController> _logger;

        public AuthController(IAuthServices authService, RoleManager<Role> roleManager, UserManager<User> userManager, IServices<RefreshToken> refreshService, IConfiguration config, ILogger<AuthController> logger)
            => (_authService, _roleManager, _userManager, _refreshService, _config, _logger) = (authService, roleManager, userManager, refreshService, config, logger);


        private void GenerateCookies(AuthServiceResponseDto authServiceResponseDto)
        {
            _logger.LogInformation("Generating cookies in 'GenerateCookies' method");

            try
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

                _logger.LogInformation("Cookies generated successfully\n");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Some thing went wrong while creating the cookies, {errer}", ex);
            }
        }


        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterDto register)
        {
            _logger.LogInformation("register ...");

            try
            {
                var result = await _authService.RegisterAsync(register);

                if (!result.IsSucceed) { return BadRequest(result.Message); }

                GenerateCookies(result);

                var user = await _userManager.FindByEmailAsync(register.Email);
                var roles = await _userManager.GetRolesAsync(user);

                _logger.LogInformation("the user has been register successfully ✅");

                return Ok(new
                {
                    userName = user.UserName,
                    userId = user.Id,
                    userRoles = roles
                });
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Some thing went wrong while register process, {errer}", ex);

                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            _logger.LogInformation("\n\nuser login...");

            try
            {
                var result = await _authService.LoginAsync(login);

                if (!result.IsSucceed) { return Unauthorized(result.Message); }

                GenerateCookies(result);

                var user = await _userManager.FindByEmailAsync(login.Email);
                var roles = await _userManager.GetRolesAsync(user);

                _logger.LogInformation("the user has been login successfully\n\n");

                return Ok(new
                {
                    userName = user.UserName,
                    userId = user.Id,
                    userRoles = roles
                });
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Some thing went wrong while login, {errer}", ex);

                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }


        [HttpPost]
        [Route("Refresh")]
        public async Task<IActionResult> Refresh()
        {
            _logger.LogInformation("Trying to refresh the tokens in 'Refresh' method");

            try
            {
                var refreshToken = Request.Cookies["refreshToken"];

                if (refreshToken is null)
                {
                    _logger.LogInformation("There is no any refresh token in cookies");

                    return NoContent();
                }

                _logger.LogInformation("getting the refresh token successfully from cookies");

                var result = await _authService.ValidateRefreshToken(refreshToken);

                if (!result.IsSucceed)
                {
                    _logger.LogInformation("Invalid refresh token!\n");
                    return NoContent();
                }

                GenerateCookies(result);

                _logger.LogInformation("Tokens refreshed successfully\n");

                return Ok(new { message = "Tokens refreshed" });
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Some thing went wrong while refreshing the tokens, {errer}", ex);

                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }


        [HttpPost]
        [Authorize(Roles = $"{StaticUserRoles.ADMIN}")]
        [Route("make-admin")]
        public async Task<IActionResult> MakeAdmin([FromBody] UpdatePermissionDto updatePermissionDto)
        {
            var makeAdmin = await _authService.MakeAdminAsynce(updatePermissionDto);

            return Ok(makeAdmin);
        }


        [HttpPost]
        [Authorize(Roles = $"{StaticUserRoles.ADMIN}, {StaticUserRoles.OWNER}")]
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
            _logger.LogInformation("logout ...");

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

            _logger.LogInformation("logout successfully");

            return Ok("Logged out successfully");
        }


        [HttpGet]
        [Route("Get-userRoles")]
        public async Task<IActionResult> GetUserRoles()
        {
            _logger.LogInformation("get user roles in 'GetUserRoles' method");

            try
            {
                var refreshToken = Request.Cookies["refreshToken"];

                var token = await _refreshService.GetAsync(rt => rt.Token == refreshToken, includes: rt => rt.User);
                if (token is null || token.ExpDate < DateTime.UtcNow)
                {
                    _logger.LogInformation("Invalid refresh token!\n");
                    return Unauthorized();
                }

                var user = token.User;
                var roles = await _userManager.GetRolesAsync(user);

                _logger.LogInformation("user roles returned successfully\n");

                return Ok(new
                {
                    userName = user.UserName,
                    userId = user.Id,
                    userRole = roles
                });
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Some thing went wrong while refreshing the tokens, {errer}", ex);

                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }
    }
}
