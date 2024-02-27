using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.EntryFund.CustomerAndSuppliers.updateFunds.updateFundsGLRelation
{
    public class updateFundsGLRelationRequest : IRequest<bool>
    {
        public List<supAndCustUpdateFund> supAndCustUpdateFunds { get; set; }
        public bool isCustomer { get; set; }
        public DateTime date { get; set; }
        public bool isUpdate { get; set; } = false;
        public int branchId { get; set; } = 0;
    }
}
