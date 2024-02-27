using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.PriceLists
{
    public  class salePriceListsRequest :IRequest<int>
    {
        public int branchId { get; set; }
        public int personId { get; set; }
        public int? salesManId { get; set; } = 0;
        public int employeeId { get; set; }
        public int? PriceListId { get; set; } = 0;
        public int invoiceTypeId { get; set; }
        public int? oldSalePriceId { get; set; }
        public InvGeneralSettings setting { get; set; }
    }
}
