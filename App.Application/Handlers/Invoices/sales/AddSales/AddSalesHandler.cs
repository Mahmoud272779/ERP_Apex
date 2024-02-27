using MediatR;
using System.Threading;

namespace App.Application.Handlers.Invoices.sales.AddSales
{
    public class AddSalesHandler : IRequestHandler<AddSalesRequest, ResponseResult>
    {
        private readonly IAddSalesService addSalesService;

        public AddSalesHandler(IAddSalesService addSalesService)
        {
            this.addSalesService = addSalesService;
        }

        public async Task<ResponseResult> Handle(AddSalesRequest request, CancellationToken cancellationToken)
        {
            return await addSalesService.AddSales(request); 
        }
    }
}
