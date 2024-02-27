using System.Collections.Generic;

namespace Attendleave.Erp.ServiceLayer.BaseDto
{
    public class ListWithCountDto<T> :  Interfaces.IBase.IListWithCountDto<T> where T : class
    {
        public int ListCount { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public IList<T> Data { get; set; }
    }
}
