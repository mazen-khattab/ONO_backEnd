using Microsoft.AspNetCore.Hosting.Server;
using ONO.Application.DTOs.OrderDto;
using ONO.Core.AnotherObjects;
using ONO.Core.Entities;
using ONO.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.Interfaces
{
    public interface IOrderServices : IServices<Order>
    {
        Task<ResponseInfo> CompleteOrder(CheckoutOrderInfoDto orderInfo, int userId);
        Task<ICollection<OrderHistoryDto>> OrdersHistory(int userId);
    }
}
