using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using ONO.Application.DTOs.GuestProductsDTOs;
using ONO.Application.DTOs.ProductsDTOs;
using ONO.Application.DTOs.UsersCartDTOs;
using ONO.Application.Services;
using ONO.Core.AnotherObjects;
using ONO.Core.Entities;
using ONO.Core.Interfaces;
using System.Security.Claims;

namespace ONO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        readonly ICartService _cartService;
        readonly IServices<Product> _productServices;
        readonly IServices<GuestCart> _guestService;
        readonly IMapper _mapper;

        public CartController(ICartService cartService, IServices<GuestCart> guestService, IMapper mapper, IServices<Product> productServices)
        {
            _cartService = cartService;
            _guestService = guestService;
            _mapper = mapper;
            _productServices = productServices;
        }


        [HttpGet]
        [Route("GetUsersCart")]
        public async Task<IActionResult> GetUsersCart()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim is null) { return Unauthorized(); }

            int userId = int.Parse(claim.Value);
            var products = await _cartService.GetAllAsync(up => up.UserId == userId, includes: up => up.Product);

            if (products.Item2 == 0)
            {
                return Ok(new List<UsersCartDTO>());
            }

            var UsersCart = _mapper.Map<IEnumerable<UsersCartDTO>>(products.Item1);
            return Ok(UsersCart);
        }


        [HttpGet]
        [Route("GetGuestProducts")]
        public async Task<IActionResult> GetGuestProducts([FromHeader(Name = "GuestId")] string guestId)
        {
            var products = await _guestService.GetAllAsync(gp => gp.UserId == guestId, includes: gp => gp.Product);

            if (products.Item2 == 0)
            {
                return Ok(new List<UsersCartDTO>());
            }

            var guestProducts = _mapper.Map<IEnumerable<GuestCartDto>>(products.Item1);

            return Ok(guestProducts);
        }


        [HttpPost]
        [Route("AddToCart")]
        public async Task<IActionResult> AddToCart([FromQuery] int productID, [FromQuery] int amount)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim is null) { return Unauthorized(); }

            int userId = int.Parse(claim.Value);
            var response = await _cartService.AddToCart(userId, productID, amount);

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
        [Route("AddToGuestCart")]
        public async Task<IActionResult> AddToGuestCart([FromHeader(Name = "GuestId")] string guestId, [FromQuery] int productID, [FromQuery] int amount)
        {
            var response = await _cartService.AddToCartGuest(guestId, productID, amount);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }
            else
            {
                return Ok(response.Message);
            }
        }


        [HttpPut]
        [Route("Increase")]
        public async Task<IActionResult> IncreaseAmount([FromQuery] int productId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim is null) { return Unauthorized(); }
            int userId = int.Parse(claim.Value);

            var response = await _cartService.IncreaseAmount(userId, productId);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }
            else
            {
                return Ok(response.Message);
            }
        }

        [HttpPut]
        [Route("IncreaseGuest")]
        public async Task<IActionResult> IncreaseGuest([FromHeader(Name = "GuestId")] string guestId, [FromQuery] int productID)
        {
            var response = await _cartService.IncreaseAmountGuest(guestId, productID);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }
            else
            {
                return Ok(response.Message);
            }
        }


        [HttpPut]
        [Route("Decrease")]
        public async Task<IActionResult> DecreaseAmount([FromQuery] int productId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim is null) { return Unauthorized(); }
            int userId = int.Parse(claim.Value);

            var response = await _cartService.DecreaseAmount(userId, productId);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }
            else
            {
                return Ok(response.Message);
            }
        }


        [HttpPut]
        [Route("DecreaseGuest")]
        public async Task<IActionResult> DecreaseGuest([FromHeader(Name = "GuestId")] string guestId, [FromQuery] int productID)
        {
            var response = await _cartService.DecreaseAmountGuest(guestId, productID);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }
            else
            {
                return Ok(response.Message);
            }
        }


        [HttpDelete]
        [Route("DeleteItem")]
        public async Task<IActionResult> DeleteItem([FromQuery] int productId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim is null) { return Unauthorized(); }
            int userId = int.Parse(claim.Value);

            var response = await _cartService.Delete(userId, productId);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }
            else
            {
                return Ok(response.Message);
            }
        }


        [HttpDelete]
        [Route("DeleteAllItems")]
        public async Task<IActionResult> DeleteAllItems()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim is null) { return Unauthorized(); }
            int userId = int.Parse(claim.Value);

            var response = await _cartService.DeleteAll(userId);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }
            else
            {
                return Ok(response.Message);
            }
        }


        [HttpDelete]
        [Route("DeleteGuestItem")]
        public async Task<IActionResult> DeleteGuestItem([FromHeader(Name = "GuestId")] string guestId, [FromQuery] int productID)
        {
            var response = await _cartService.DeleteGuest(guestId, productID);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }
            else
            {
                return Ok(response.Message);
            }
        }


        [HttpDelete]
        [Route("DeleteAllGuestItems")]
        public async Task<IActionResult> DeleteAllGuestItems([FromHeader(Name = "GuestId")] string guestId)
        {
            var response = await _cartService.DeleteGuestAll(guestId);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }
            else
            {
                return Ok(response.Message);
            }
        }
    }
}
