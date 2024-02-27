using App.Application.Services.Reports.StoreReports.Sales;
using FastReport.Utils;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Reports.Purchases.PurchasesTransaction
{
    public class puchasesTransactionHandler : IRequestHandler<puchasesTransactionRequest, ResponseResult>
    {
        private readonly iRPT_Sales _iRPT_Sales;
        public puchasesTransactionHandler(iRPT_Sales iRPT_Sales)
        {
            _iRPT_Sales = iRPT_Sales;
        }
        public async Task<ResponseResult> Handle(puchasesTransactionRequest request, CancellationToken cancellationToken)
        {
            var res = await _iRPT_Sales.GetsalesTransaction(request);
            return new ResponseResult()
            {
                Data = res.data,
                TotalCount = res.TotalCount,
                Note = res.notes,
                Result = res.Result,
                DataCount = res.dataCount
            };
        }
    }
}
