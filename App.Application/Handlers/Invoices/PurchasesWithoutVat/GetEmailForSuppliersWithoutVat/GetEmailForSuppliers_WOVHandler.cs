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
    public class GetEmailForSuppliers_WOVHandler : IRequestHandler<GetEmailForSuppliers_WOVRequest,ResponseResult>
    {
        private readonly ISendEmailPurchases SendEmailPurchases;

        public GetEmailForSuppliers_WOVHandler(ISendEmailPurchases sendEmailPurchases)
        {
            SendEmailPurchases = sendEmailPurchases;
        }

        public async Task<ResponseResult> Handle(GetEmailForSuppliers_WOVRequest request, CancellationToken cancellationToken)
        {
            return await SendEmailPurchases.GetEmailForSuppliers(request.InvoiceId);
        }
    }
}
