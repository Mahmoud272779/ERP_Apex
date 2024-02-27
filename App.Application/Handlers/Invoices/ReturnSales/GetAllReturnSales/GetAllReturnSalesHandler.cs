using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.ReturnSales.GetAllReturnSales
{
    public class GetAllReturnSalesHandler : IRequestHandler<GetAllReturnSalesRequest, ResponseResult>
    {
        private readonly IGetAllSalesServices getAllSalessService;

        public GetAllReturnSalesHandler(IGetAllSalesServices getAllSalessService)
        {
            this.getAllSalessService = getAllSalessService;
        }

        public async Task<ResponseResult> Handle(GetAllReturnSalesRequest request, CancellationToken cancellationToken)
        {
            return await getAllSalessService.GetAllSales(request);
        }
    }
}
