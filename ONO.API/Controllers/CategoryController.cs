using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ONO.Core.Entities;
using ONO.Core.Interfaces;

namespace ONO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        readonly IServices<Category> _service;
        readonly IMapper _mapper;

        public CategoryController(IServices<Category> services, IMapper mapper)
        {
            _service = services;
            _mapper = mapper;
        }


        [HttpGet]
        [Route("GetCategories")]
        public async Task<IActionResult> GetAllCategory()
        {
            var (categoires, count) = await _service.GetAllAsync();

            return Ok(categoires);
        }
    }
}
