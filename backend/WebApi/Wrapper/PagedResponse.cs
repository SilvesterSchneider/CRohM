using System;

namespace WebApi.Wrapper
{
    public class PagedResponse<T> : Response<T>
    {
        public int PageStart { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public PagedResponse(T data, int pageStart, int pageSize, int totalRecords)
        {
            this.PageStart = pageStart;
            this.PageSize = pageSize;
            this.Data = data;
            this.TotalRecords = totalRecords;
            this.Message = null;
            this.Succeeded = true;
            this.Errors = null;
        }
    }
}
