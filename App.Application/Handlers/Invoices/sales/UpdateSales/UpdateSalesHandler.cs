using App.Application.Services.Process.StoreServices.Invoices.Sales.Sales.ISalesServices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.sales.UpdateSales
{
    public class UpdateSalesHandler : IRequestHandler<UpdateSalesRequest, ResponseResult>
    {
        private readonly IUpdateSalesService updateSalesService;

        public UpdateSalesHandler(IUpdateSalesService updateSalesService)
        {
            this.updateSalesService = updateSalesService;
        }

        public async Task<ResponseResult> Handle(UpdateSalesRequest request, CancellationToken cancellationToken)
        {
            return await updateSalesService.UpdateSales(request);
        }
    }
}
