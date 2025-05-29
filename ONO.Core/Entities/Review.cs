using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Core.Entities
{
    public class Review : BaseEntity
    {
        public int Rating { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string? Comment { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }

        public User User { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
