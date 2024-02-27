using Attendleave.Erp.Core.APIUtilities;
using MediatR;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts
{
    public class UpdateFinancialAccountRequest : UpdateFinancialAccountParameter,IRequest<IRepositoryActionResult>
    {
    }
}
