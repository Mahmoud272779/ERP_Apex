using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.EntryFund.CustomerAndSuppliers.GetCustomerAndSupplierFund
{
    public class GetCustomerAndSupplierFundRequest : FundsCustomerandSupplierSearch,IRequest<ResponseResult>
    {
        public bool isCustomer { get; set; }
        public bool isSupplier { get; set; }
    }
}
