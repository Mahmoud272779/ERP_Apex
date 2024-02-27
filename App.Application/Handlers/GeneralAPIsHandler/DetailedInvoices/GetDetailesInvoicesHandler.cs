using App.Application.Handlers.GeneralAPIsHandler.GeneralLedgerHomeData;
using App.Domain.Models.Response.GeneralLedger;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.DetailedInvoices
{
    public class GetDetailesInvoicesHandler : IRequestHandler<GetDetailesInvoicesRequest, List<GetDetailesInvoicesResponse>>
    {

        private readonly IRepositoryQuery<InvoiceMaster> _invoicesmasterQuery;
       
        private readonly IRoundNumbers _roundNumbers;
        public GetDetailesInvoicesHandler(IRoundNumbers roundNumbers, IRepositoryQuery<InvoiceMaster> invoicesmasterQuery)
        {

            _roundNumbers = roundNumbers;
            _invoicesmasterQuery = invoicesmasterQuery;
        }

        public async Task<List<GetDetailesInvoicesResponse>> Handle(GetDetailesInvoicesRequest request, CancellationToken cancellationToken)
        {
            var invoies = _invoicesmasterQuery.TableNoTracking.Where(a => request.InvoicesIds.Contains(a.InvoiceId)).
                Select(a => new GetDetailesInvoicesResponse 
                {
                    Net= a.Net,
                    Remain= a.Remain, 
                    Paid = a.Paid, 
                    InvoiceId = a.InvoiceId ,
                    invoiceType = a.InvoiceType,
                    personId = a.PersonId
                }).ToList();


            return invoies;
        }
    }
}
