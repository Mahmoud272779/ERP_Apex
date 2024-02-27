using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.PersonBalanceForPaymentMethod
{
    public  class GetPersonBalanceForPaymentMethodRequest : IRequest<GetPersonBalanceForPaymentMethodResponse>
    {
     
        public int invoiceTypeId { get; set; }
        public int personId { get; set; }
        public int BranchId { get; set; }
        public int? invoiceId { get; set; } = 0;
    }
}
