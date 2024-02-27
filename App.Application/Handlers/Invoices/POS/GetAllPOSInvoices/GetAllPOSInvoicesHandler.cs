using App.Domain.Models.Request.POS;
using MediatR;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.GetAllPOSInvoices
{
    public class GetAllPOSInvoicesHandler : IRequestHandler<GetAllPOSInvoicesRequest, ResponseResult>
    {
        private readonly IGetPOSInvoicesService GetPOSInvoiceService;

        public GetAllPOSInvoicesHandler(IGetPOSInvoicesService getPOSInvoiceService)
        {
            GetPOSInvoiceService = getPOSInvoiceService;
        }

        public async Task<ResponseResult> Handle(GetAllPOSInvoicesRequest parameter, CancellationToken cancellationToken)
        {
           return await GetPOSInvoiceService.GetAllPOSInvoices(parameter);
        }
    }
}
