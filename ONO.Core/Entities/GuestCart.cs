using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Core.Entities
{
    public class GuestCart : BaseEntity
    {
        public int ProductId { get; set; }
        public int ProductAmount { get; set; }
        public string UserId { get; set; }
        public DateTime ExpireAt { get; set; } = DateTime.Now.AddMinutes(15);

        public Product Product { get; set; }
    }
}
