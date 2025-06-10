using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using ONO.Application.DTOs;
using ONO.Application.DTOs.ProductsDTOs;
using ONO.Core.Entities;
using ONO.Core.Interfaces;

namespace ONO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        readonly IServices<UserProducts> _cartService;
        readonly IServices<Product> _productService;
        readonly IMapper _mapper;

        public CartController(IServices<UserProducts> cartService, IMapper mapper, IServices<Product> productService)
        {
            _cartService = cartService;
            _mapper = mapper;
            _productService = productService;
        }


        private async Task<UserProducts> GetProduct(int userId, int productId) => await _cartService.GetAsync(p => p.UserId == userId && p.ProductID == productId);


        [HttpGet]
        [Route("GetUserProducts")]
        public async Task<IActionResult> GetUserProducts([FromQuery] int userId)
        {
            var products = await _cartService.GetAllAsync(up => up.UserId == userId, includes: up => up.Product);
            if (products.Item2 == 0)
            {
                return Ok(new List<UserProductsDTOs>());
            }

            var userProducts = _mapper.Map<IEnumerable<UserProductsDTOs>>(products.Item1);

            return Ok(userProducts);
        }


        [HttpPost]
        [Route("AddToCart")]
        public async Task<IActionResult> AddToCart([FromQuery]int userId, [FromQuery] int productID, [FromQuery] int amount)
        {
            var product = await _productService.GetAsync(p => p.Id == productID, includes: p => p.Category);

            var existProduct = await GetProduct(userId, productID);

            if (existProduct is { })
            {
                existProduct.ProductAmount += amount;
                existProduct.Product = product;
                await _cartService.UpdateAsync(existProduct);

                var existProductsDto = _mapper.Map<UserProductsDTOs>(existProduct);
                return Ok(existProductsDto);
            }

            UserProducts userProduct = new()
            {
                UserId = userId,
                ProductID = productID,
                ProductAmount = amount,
                Product = product
            };

            await _cartService.AddAsync(userProduct);

            var ProductsDto = _mapper.Map<UserProductsDTOs>(userProduct);
            return Ok(ProductsDto);
        }


        [HttpPut]
        [Route("Increase")]
        public async Task<IActionResult> IncreaseAmount([FromQuery] int userId, [FromQuery] int productId)
        {
            var userProduct = await GetProduct(userId, productId);
            var product = await _productService.GetAsync(p => p.Id == productId);

            if (userProduct is null) { return BadRequest("the product does not exist"); }

            if (product.StockUnit > userProduct.ProductAmount)
            {
                userProduct.ProductAmount += 1;
                await _cartService.UpdateAsync(userProduct);
                return Ok("the amount has been updated");
            }

            return BadRequest("can not increase the amount");
        }


        [HttpPut]
        [Route("Decrease")]
        public async Task<IActionResult> DecreaseAmount([FromQuery] int userId, [FromQuery] int productId)
        {
            var userProduct = await GetProduct(userId, productId);
            var product = await _productService.GetAsync(p => p.Id == productId);

            if (userProduct is null) { return BadRequest("the product does not exist"); }

            if (userProduct.ProductAmount > 1)
            {
                userProduct.ProductAmount -= 1;
                await _cartService.UpdateAsync(userProduct);
                return Ok("the amount has been updated");
            }

            return BadRequest("can not decrease the amount");
        }


        [HttpDelete]
        [Route("DeleteItem")]
        public async Task<IActionResult> DeleteItem([FromQuery] int userId, [FromQuery] int productId)
        {
            var userProduct = await GetProduct(userId, productId);

            if (userProduct is null) { return BadRequest("the product does not exist"); }

            await _cartService.DeleteAsync(userProduct);

            return Ok("The product has been deleted");
        }
    }
}
