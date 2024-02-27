using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.ReturnSales.GetReturnSales
{
    public class GetReturnSalesHandler : IRequestHandler<GetReturnSalesRequest, ResponseResult>
    {
        private readonly IGetSalesByCodeForReturn GetSalesByCodeServiceForReturn;

        public GetReturnSalesHandler(IGetSalesByCodeForReturn getSalesByCodeServiceForReturn)
        {
            GetSalesByCodeServiceForReturn = getSalesByCodeServiceForReturn;
        }

        public async Task<ResponseResult> Handle(GetReturnSalesRequest request, CancellationToken cancellationToken)
        {
            return await GetSalesByCodeServiceForReturn.GetReturnSalesByCode(request.InvoiceCode);
        }
    }
}
