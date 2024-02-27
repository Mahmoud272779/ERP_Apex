using App.Application.Services.Process.Invoices.Purchase.IPurchasesServices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Purchases.GetInvoiceForSuppliers
{
    public class GetInvoiceForSuppliers_WOVHandler : IRequestHandler<GetInvoiceForSuppliers_WOVRequest, ResponseResult>
    {
        private readonly ISendEmailPurchases SendEmailPurchases;

        public GetInvoiceForSuppliers_WOVHandler(ISendEmailPurchases sendEmailPurchases)
        {
            SendEmailPurchases = sendEmailPurchases;
        }

        public async Task<ResponseResult> Handle(GetInvoiceForSuppliers_WOVRequest request, CancellationToken cancellationToken)
        {
            return await SendEmailPurchases.GetInvoiceForSuppliers(request.InvoiceId);
        }
    }
}
