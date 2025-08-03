using ONO.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.DTOs.InventoryTransactionDTOs
{
    public class InventoryTransactionDto
    {
        public inventoryEnum TransactionType { get; set; }
        public decimal Quantity { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
    }
}
