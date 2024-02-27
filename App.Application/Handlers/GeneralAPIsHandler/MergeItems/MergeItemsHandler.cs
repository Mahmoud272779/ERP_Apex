using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.MergeItems
{
    public class MergeItemsHandler : IRequestHandler<MergeItemsRequest, ResponseResult>
    {
        public async Task<ResponseResult> Handle(MergeItemsRequest request, CancellationToken cancellationToken)
        {
            var listResult = await MergeItemsMethod.MergeItems(request.list, request.invoiceType);
            return new ResponseResult() { Data = listResult, Id = null, Result = Result.Success };
        }
    }
}
