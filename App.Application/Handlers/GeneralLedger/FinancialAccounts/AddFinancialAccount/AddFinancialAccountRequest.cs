using Attendleave.Erp.Core.APIUtilities;
using MediatR;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts
{
    public class AddFinancialAccountRequest : FinancialAccountParameter,IRequest<IRepositoryActionResult>
    {
    }
}
