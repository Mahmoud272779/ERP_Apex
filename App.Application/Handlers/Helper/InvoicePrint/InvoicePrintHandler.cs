using App.Application.Services.Printing.PrintResponse;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Handlers.Helper.InvoicePrint
{
    public class InvoicePrintHandler : IRequestHandler<InvoicePrintRequest, ReportsReponse>
    {
        private readonly IPrintResponseService iPrintResponseService;

        public InvoicePrintHandler(IPrintResponseService iPrintResponseService)
        {
            this.iPrintResponseService = iPrintResponseService;
        }

        public async Task<ReportsReponse> Handle(InvoicePrintRequest request, CancellationToken cancellationToken)
        {
            return await iPrintResponseService.Print(request.invoiceId, request.screenId, request.invoiceCode, request.exportType, request.employeeNameAr, request.employeeNameEn, request.isPOS,request.isPriceOffer, request.isArabic);
        }
    }
}
