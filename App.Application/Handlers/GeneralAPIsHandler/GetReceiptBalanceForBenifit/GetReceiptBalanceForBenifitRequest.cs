using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.GetReceiptBalanceForBenifit
{
    public class GetReceiptBalanceForBenifitRequest : IRequest<ResponseResult>
    {
        public int AuthorityId { get; set; }
        public int BenefitID { get; set; }
    }
}
