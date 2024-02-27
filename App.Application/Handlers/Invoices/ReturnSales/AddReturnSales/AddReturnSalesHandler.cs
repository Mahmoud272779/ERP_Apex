using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.ReturnSales.AddReturnSales
{
    public class AddReturnSalesHandler : IRequestHandler<AddReturnSalesRequest, ResponseResult>
    {
        private readonly IAddReturnSalesService AddReturnSalesService;

        public AddReturnSalesHandler(IAddReturnSalesService addReturnSalesService)
        {
            AddReturnSalesService = addReturnSalesService;
        }

        public async Task<ResponseResult> Handle(AddReturnSalesRequest request, CancellationToken cancellationToken)
        {
            return await AddReturnSalesService.AddReturnSales(request);
        }
    }
}
