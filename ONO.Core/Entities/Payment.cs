using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Core.Entities
{
    public class Payment : BaseEntity
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; } // the amount that paid
        public string Currency { get; set; }
        public string Provider { get; set; } // the name of payment gateway 
        public string TranascationId { get; set; } // the id that the payment gateway return it to the payment process
        public string Status { get; set; }
        public string HmacSignature { get; set; }
        public string Pan { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; }

        public Order Order { get; set; }
    }
}
