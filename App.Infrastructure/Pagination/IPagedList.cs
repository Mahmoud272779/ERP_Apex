using Attendleave.Erp.ServiceLayer.BaseDto;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Attendleave.Erp.ServiceLayer.Abstraction
{
    public interface IPagedList<T, TDto> where TDto : class where T : class
    {
        Task<ListWithCountDto<TDto>> GetGenericPaginationAsync(IQueryable<T> source, int pageIndex, int pageSize, IMapper _mapper);
        Task<ListWithCountDto<TDto>> GetGenericPaginationAsync(IQueryable<T> source);
        ListWithCountDto<TDto> GetGenericPagination(IList<T> source, int pageIndex, int pageSize, IMapper _mapper);
    }
}
