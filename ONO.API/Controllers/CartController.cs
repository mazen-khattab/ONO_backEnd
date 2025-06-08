using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ONO.Core.Entities;
using ONO.Core.Interfaces;

namespace ONO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        readonly IServices<UserProducts> _cartService;
        readonly IMapper _mapper;

        public CartController(IServices<UserProducts> cartService, IMapper mapper)
        {
            _cartService = cartService;
            _mapper = mapper;
        }


        [HttpPatch]
        [Route("AddToCart")]
        public async Task<IActionResult> AddToCart([FromQuery]int userId, [FromQuery] int productID, [FromQuery] int amount)
        {
            var existProduct = await _cartService.GetAsync(p => p.ProductID == productID && p.UserId == userId);

            if (existProduct is { })
            {
                existProduct.ProudctAmount += amount;
                await _cartService.UpdateAsync(existProduct);
                return Ok("proeuct has been updated");
            }

            UserProducts product = new()
            {
                UserId = userId,
                ProductID = productID,
                ProudctAmount = amount,
            };

            _cartService.AddAsync(product);
            return Ok("product has been added");
        }
    }
}
