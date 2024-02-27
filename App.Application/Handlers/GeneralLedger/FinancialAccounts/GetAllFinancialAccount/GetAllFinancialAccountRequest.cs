using Attendleave.Erp.Core.APIUtilities;
using MediatR;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts
{
    public class GetAllFinancialAccountRequest : FA_Search, IRequest<IRepositoryActionResult>
    {
        public int? parentId { get; set; }
        public int? pageNumber { get; set; }
        public int? pageSize { get; set; }
    }
}
