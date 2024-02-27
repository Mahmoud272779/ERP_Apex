using App.Domain.Models.Security.Authentication.Response.Store;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Persons.GetPersonBalance
{
    public  class GetReceiptBalanceForBenifitForInvoicesRequest:IRequest<ResponseResult>
    {
        public int AuthorityId { get; set; }
        public List<personsForBalanceDto> persons  { get; set; }
        public bool fromGetInvoice { get; set; }
        public int BranchId { get; set; }

    }
}
