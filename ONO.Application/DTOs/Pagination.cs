using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.DTOs
{
    public class Pagination<T> where T : class
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public IEnumerable<T> Data { get; set; }

        public Pagination(int pageNumber, int pageSize, int count, IEnumerable<T> data) => (PageNumber, PageSize, Count, Data) = (pageNumber, pageSize, count, data);
    }
}
