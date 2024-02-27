using App.Application.Handlers.GeneralAPIsHandler.GetInvoiceById;
using App.Application.Services.Process.Invoices.Purchase;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.InvCollectionReceipt
{
    public class GetTotalsInvoiceDataHandler : IRequestHandler<GetTotalsInvoiceDataRequest, InvoiceData>
    {
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;

        public GetTotalsInvoiceDataHandler(IRepositoryQuery<InvoiceMaster> invoiceMasterRepositoryQuery)
        {
            InvoiceMasterRepositoryQuery = invoiceMasterRepositoryQuery;
        }

        public async Task<InvoiceData> Handle(GetTotalsInvoiceDataRequest request, CancellationToken cancellationToken)
        {
           var res = InvoiceMasterRepositoryQuery.TableNoTracking.Where(a => a.InvoiceId == request.invoiceId)
                .Select(a => new InvoiceData (){Net= a.Net,Paid= a.Paid,Remain= a.Remain });
       
            if(res.Count() > 0) { new InvoiceData(); };
            InvoiceData InvoiceData = new InvoiceData()
            {
                Net = res.First().Net,
                Paid = res.First().Paid,
                Remain = res.First().Remain
            };
            return InvoiceData;
        }
    }
}
