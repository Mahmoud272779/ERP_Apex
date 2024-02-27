using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.PersonBalanceForPaymentMethod
{
    public  class GetPersonBalanceForPaymentMethodResponse
    {
        public double balance { get; set; }
        public double creditor { get; set; }
        public double debitor { get; set; }
        public int CreditOrDebit { get; set; }
    }
}
