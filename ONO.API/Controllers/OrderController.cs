using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ONO.Application.DTOs.OrderDto;
using ONO.Application.Interfaces;
using ONO.Core.Entities;
using System.Security.Claims;

namespace ONO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        readonly IOrderServices _orderServices;
        readonly IMapper _mapper;

        public OrderController(IOrderServices orderServices, IMapper mapper)
        {
            _orderServices = orderServices;
            _mapper = mapper;
        }


        [HttpPost]
        [Route("CompleteOrder")]
        public async Task<IActionResult> CompteletOrder(OrderInfoDto orderInfo)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim is null) { return Unauthorized(); }

            int userId = int.Parse(claim.Value);

            var response = await _orderServices.CompleteOrder(orderInfo, userId);

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
