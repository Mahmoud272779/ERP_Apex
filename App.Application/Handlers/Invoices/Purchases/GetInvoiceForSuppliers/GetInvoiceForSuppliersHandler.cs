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
    public class GetInvoiceForSuppliersHandler : IRequestHandler<GetInvoiceForSuppliersRequest, ResponseResult>
    {
        private readonly ISendEmailPurchases SendEmailPurchases;

        public GetInvoiceForSuppliersHandler(ISendEmailPurchases sendEmailPurchases)
        {
            SendEmailPurchases = sendEmailPurchases;
        }

        public async Task<ResponseResult> Handle(GetInvoiceForSuppliersRequest request, CancellationToken cancellationToken)
        {
            return await SendEmailPurchases.GetInvoiceForSuppliers(request.InvoiceId);
        }
    }
}
