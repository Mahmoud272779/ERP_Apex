using System.Collections.Generic;

namespace Attendleave.Erp.ServiceLayer.Interfaces.IBase
{
    public interface IListWithCountDto<T> : IPageParameter where T : class
    {
        int ListCount { get; set; }
        IList<T> Data { get; set; }
    }
}
