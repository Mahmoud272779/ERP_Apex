using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.POS.PosNavigation
{
    public class PosNavigationRequest : IRequest<ResponseResult>
    {
        public int invoiceTypeId { get; set; }
        public int invoiceId { get; set; }
        public int stepType { get; set; }
        public int branchId { get; set; }
    }
}
