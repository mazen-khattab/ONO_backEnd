using ONO.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Core.Entities
{
    public class UserProducts : BaseEntity
    {
        public int ProductAmount { get; set; }
        public int UserId { get; set; }
        public int ProductID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public User User { get; set; }
        public Product Product { get; set; }
        public bool IsCompleted { get ; set ; }
    }
}
