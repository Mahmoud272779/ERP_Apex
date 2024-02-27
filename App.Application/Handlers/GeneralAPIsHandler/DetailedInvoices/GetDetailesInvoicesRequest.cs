using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.DetailedInvoices
{
    public class GetDetailesInvoicesRequest: IRequest<List<GetDetailesInvoicesResponse>>
    {
        public int[] InvoicesIds { get; set; }
    }

    public class GetDetailesInvoicesResponse
    { 
        public int InvoiceId { get; set; }
        public double Paid { get; set; }
        public string invoiceType { get; set; } 
        public double Remain { get; set; }
        public double Net { get;set; }
        public int? personId { get; set; }
    }

}
