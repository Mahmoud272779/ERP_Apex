using App.Domain.Entities.Process;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.ConvertOffline
{
    public  class addReceiptsForOfflinePosRequest : IRequest<bool>
    {
       
        public   List<InvoiceMaster> invoiceMasters { get; set; }
        public List<InvoicePaymentsMethods> invoicesPaymentMethods { get; set; }
    }
}
