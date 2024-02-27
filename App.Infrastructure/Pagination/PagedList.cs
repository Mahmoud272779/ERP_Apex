using App.Infrastructure.Mapping;
using Attendleave.Erp.ServiceLayer.BaseDto;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Attendleave.Erp.ServiceLayer.Abstraction
{
    [Serializable]
    public class PagedList<T, TDto> : IPagedList<T, TDto> where T : class where TDto : class
    {
        public async Task<ListWithCountDto<TDto>> GetGenericPaginationAsync(IQueryable<T> source, int pageIndex, int pageSize, IMapper _mapper)
        {
            var total = await source.CountAsync();
            var TotalPages = total / pageSize;

            if (total % pageSize > 0)
                TotalPages++;
            var Data = await source.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();

            var dataDto = _mapper.Map<IList<T>, IList<TDto>>(Data);
            var returnData = new ListWithCountDto<TDto>
            {
                ListCount = total,
                TotalPages = TotalPages,
                PageNumber = pageIndex,
                PageSize = pageSize,
                Data = dataDto
            };
            return returnData;
        }
        public async Task<ListWithCountDto<TDto>> GetGenericPaginationAsync(IQueryable<T> source)
        {
            var total = await source.CountAsync();
            var returnData = new ListWithCountDto<TDto>
            {
                ListCount = total
            };
            return returnData;
        }


        public ListWithCountDto<TDto> GetGenericPagination(IList<T> source, int pageIndex, int pageSize, IMapper _mapper)
        {
            var total = source.Count();
            var TotalPages = total / pageSize;

            if (total % pageSize > 0)
                TotalPages++;
            var Data = source;
            if (pageSize > 0 && pageIndex > 0)
            {
                Data = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            //else
            //{
            //    Data = source.Skip((pageIndex) * pageSize).Take(pageSize).ToList(); 
            //}

            //var dataDto = _mapper.Map<IList<T>, IList<TDto>>(Data);
            var dataDto = Mapping.Mapper.Map<IList<T>, IList<TDto>>(Data);
            var returnData = new ListWithCountDto<TDto>
            {
                ListCount = total,
                TotalPages = TotalPages,
                PageNumber = pageIndex,
                PageSize = pageSize,
                Data = dataDto
            };
            return returnData;
        }


    }
}
