using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ONO.Application.DTOs.UserDTOs;
using ONO.Application.Interfaces;
using ONO.Core.Entities;
using ONO.Core.Interfaces;
using System.Diagnostics;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ONO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        readonly IUserServices _userServices;
        readonly IMapper _mapper;

        public UserController(IUserServices userServices, IMapper mapper)
        {
            _userServices = userServices;
            _mapper = mapper;
        }


        [HttpGet]
        [Route("GetUserProfile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim is null) { return Unauthorized(); }

            int userId = int.Parse(claim.Value);

            var user = await _userServices.GetAsync(u => u.Id == userId, includes: u => u.Addresses);
            var userAddress = _mapper.Map<List<UserAddressDto>>(user.Addresses);

            return Ok(new
            {
                id = userId,
                firstName = user.Fname,
                lastName = user.Lname,
                email = user.Email,
                phone = user.PhoneNumber,
                address = userAddress.Count == 0 ?  null : userAddress[0]
            });
        }

        [HttpPost]
        [Route("updateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserDto newUserInfo)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim is null) { return Unauthorized(); }

            int userId = int.Parse(claim.Value);

            var response = await _userServices.UpdateUserProfile(newUserInfo, userId);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }
            else
            {
                return Ok(response.Message);
            }
        }

        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto passwordInfo)
        {
            if (passwordInfo.NewPassword != passwordInfo.ComfirmedPassword) { return BadRequest("Passwords do not match"); }

            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim is null) { return Unauthorized(); }

            int userId = int.Parse(claim.Value);

            var response = await _userServices.ChangePassword(passwordInfo.OldPassword, passwordInfo.NewPassword, userId);

            if (!response.IsSuccess) { return BadRequest(response.Message); }

            return Ok(response.Message);
        }

        [HttpPost]
        [Route("ContactUs")]
        public async Task<IActionResult> ContactUs([FromBody] ContactUsDto contactInfo)
        {
            await _userServices.ContactUs(contactInfo.Name, contactInfo.PhoneNumber);

            return Ok("email send successful");
        }

        [HttpPost]
        [Route("AddUserAddress")]
        public async Task<IActionResult> AddUserAddress([FromBody] UserAddressDto addressInfo)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim is null) { return Unauthorized(); }

            int userId = int.Parse(claim.Value);

            var response = await _userServices.AddUserAddress(addressInfo, userId);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Message);
        }
    }
}
