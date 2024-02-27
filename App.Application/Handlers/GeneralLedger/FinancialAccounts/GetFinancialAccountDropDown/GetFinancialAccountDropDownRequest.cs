using App.Domain.Models.Common;
using MediatR;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts
{
    public class GetFinancialAccountDropDownRequest : DropDownRequestForGL,IRequest<ResponseResult>
    {
    }
}
