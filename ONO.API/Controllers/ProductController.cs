
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ONO.Application.DTOs;
using ONO.Application.DTOs.ProductsDTOs;
using ONO.Application.Interfaces;
using ONO.Core.Entities;
using ONO.Core.Interfaces;

namespace ONO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        readonly IProductServices _productService;
        readonly IMapper _mapper;
        readonly ILogger<ProductController> _logger;

        public ProductController(IProductServices services, IMapper mapper, ILogger<ProductController> logger)
        {
            _productService = services;
            _mapper = mapper;
            _logger = logger;
        }


        [HttpGet]
        [Route("GetProducts")]
        public async Task<ActionResult<Pagination<ProductDto>>> GetAllProducts([FromQuery] ProductRequestDto productDto)
        {
            var (products, productsCount) = await _productService.GetAllAsync(P => (P.StockUnit >= 1 && P.Reserved < P.StockUnit)
            && (string.IsNullOrEmpty(productDto.Search) || P.Name.Contains(productDto.Search))
            && (string.IsNullOrEmpty(productDto.CateName) || P.Category.Name.Contains(productDto.CateName))
            && P.AgeRange >= productDto.AgeRange,
            pageNumber: productDto.PageNumber,
            pageSize: productDto.PageSize,
            includes: [p => p.Category, p => p.Gallary]);

            var productMap = _mapper.Map<IEnumerable<ProductDto>>(products);

            return Ok(new Pagination<ProductDto>(productDto.PageNumber, productDto.PageSize, productsCount, productMap));
        }
    }
}

  