using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Helper
{
    public class PaginationFilter
    {
        public int PageStart { get; set; }
        public int PageSize { get; set; }
        public PaginationFilter()
        {
            this.PageStart = 0;
            this.PageSize = 5;
        }
        public PaginationFilter(int pageStart, int pageSize)
        {
            this.PageStart = pageStart < 0 ? 0 : pageStart;
            this.PageSize = pageSize > 100 ? 100 : pageSize;
        }
    }
}
