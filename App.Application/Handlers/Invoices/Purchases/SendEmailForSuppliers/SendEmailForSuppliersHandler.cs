using App.Application.Services.Process.Invoices.Purchase.IPurchasesServices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Purchases.SendEmailForSuppliers
{
    public class SendEmailForSuppliersHandler : IRequestHandler<SendEmailForSuppliersRequest,ResponseResult>
    {
        private readonly ISendEmailPurchases SendEmailPurchases;

        public SendEmailForSuppliersHandler(ISendEmailPurchases sendEmailPurchases)
        {
            SendEmailPurchases = sendEmailPurchases;
        }

        public async Task<ResponseResult> Handle(SendEmailForSuppliersRequest request, CancellationToken cancellationToken)
        {
            return await SendEmailPurchases.SendEmailForSuppliers(request);
        }
    }
}
