using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Reports.Store.Store.ItemsBalanceInStores
{
    public class ItemsBalanceInStores : IRequestHandler<_ItemsBalanceInStoresRequest, ResponseResult>
    {
        private readonly IMediator _mediatR;

        public ItemsBalanceInStores(IMediator mediatR)
        {
            _mediatR = mediatR;
        }
        public async Task<ResponseResult> Handle(_ItemsBalanceInStoresRequest request, CancellationToken cancellationToken)
        {
            var res = await _mediatR.Send(new ItemsBalanceInStoresRequest
            {
                isPrint = false,
                catId = request.catId,
                itemId = request.itemId,
                itemTypes = request.itemTypes,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                storeId = request.storeId
            });
            return new ResponseResult()
            {
                Data = res.data,
                DataCount = res.dataCount,
                TotalCount = res.totalCount,
                Result = res.Result,
                Note = res.notes
            };
        }
    }
}
