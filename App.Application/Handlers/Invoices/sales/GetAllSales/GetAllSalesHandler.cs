using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.sales.GetAllSales
{
    public class GetAllSalesHandler : IRequestHandler<GetAllSalesRequest, ResponseResult>
    {
        private readonly IGetAllSalesServices getAllSalesServices;

        public GetAllSalesHandler(IGetAllSalesServices getAllSalesServices)
        {
            this.getAllSalesServices = getAllSalesServices;
        }

        public async Task<ResponseResult> Handle(GetAllSalesRequest request, CancellationToken cancellationToken)
        {
            return await getAllSalesServices.GetAllSales(request);
        }
    }
}
