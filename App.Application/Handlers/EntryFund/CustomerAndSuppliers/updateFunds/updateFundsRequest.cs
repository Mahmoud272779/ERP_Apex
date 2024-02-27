using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.EntryFund.CustomerAndSuppliers.updateFunds
{
    public class updateFundsRequest : FundsCustomerandSupplierParameter,IRequest<ResponseResult>
    {
        public bool isCustomer { get; set; }
    }
}
