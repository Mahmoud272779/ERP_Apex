using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.ReCreateCustomersAndSuppliersFA.CustomersFA
{
    public class ReCreateSupplierCustomerFARequest : IRequest<ResponseResult>
    {
        public int newParentId { get; set; }
        public int OldAccountId { get; set; }
        public PersonTypes Type { get; set; } // 1 is Customer 
    }
}
