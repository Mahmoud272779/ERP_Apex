using MediatR;
using System.Threading;

namespace App.Application.Handlers.Invoices.sales.SalesPrintWithSave
{
    public class SalesPrintWithSaveHandler : IRequestHandler<SalesPrintWithSaveRequest, bool>
    {
        private readonly IRepositoryQuery<InvGeneralSettings> settingService;

        public SalesPrintWithSaveHandler(IRepositoryQuery<InvGeneralSettings> settingService)
        {
            this.settingService = settingService;
        }

        public async Task<bool> Handle(SalesPrintWithSaveRequest request, CancellationToken cancellationToken)
        {
            var print = await settingService.GetByIdAsync(1);
            return print.Sales_PrintWithSave;
        }
    }
}
