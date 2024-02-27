using App.Application.Services.Process.Invoices.Purchase.IPurchasesServices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Purchases.GetEmailForSuppliers
{
    public class GetEmailForSuppliersHandler : IRequestHandler<GetEmailForSuppliersRequest,ResponseResult>
    {
        private readonly ISendEmailPurchases SendEmailPurchases;

        public GetEmailForSuppliersHandler(ISendEmailPurchases sendEmailPurchases)
        {
            SendEmailPurchases = sendEmailPurchases;
        }

        public async Task<ResponseResult> Handle(GetEmailForSuppliersRequest request, CancellationToken cancellationToken)
        {
            return await SendEmailPurchases.GetEmailForSuppliers(request.InvoiceId);
        }
    }
}
