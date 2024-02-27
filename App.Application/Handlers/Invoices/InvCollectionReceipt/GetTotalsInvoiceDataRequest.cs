using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.InvCollectionReceipt
{
    public  class GetTotalsInvoiceDataRequest : IRequest<InvoiceData>
    {
        public int invoiceId { get; set; }
    }

    public class InvoiceData
    {
        public double Net;
        public double Remain;
        public double Paid;
    }
}
