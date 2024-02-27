using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.ERPTool.ReCreateCustomersAndSuppliersFA.CustomersHelper
{
    public class CustomerSupplierFAHelperRequest : IRequest<bool>
    {
        public int newParentId { get; set; }
        public int OldAccountId { get; set; }
        public InvPersons person { get; set; }
        public GLFinancialAccount FinancialAccount { get; set; }
        public PersonTypes Type { get; set; } // 1 is Customer 
    }
}
