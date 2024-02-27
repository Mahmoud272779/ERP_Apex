using App.Application.Basic_Process;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts.index
{
    public class indexHandler : BusinessBase<GLFinancialAccount>,IRequestHandler<indexRequest, IRepositoryActionResult>
    {
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;

        public indexHandler(IRepositoryActionResult repositoryActionResult, IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery) : base(repositoryActionResult)
        {
            this.financialAccountRepositoryQuery = financialAccountRepositoryQuery;
        }
        public async Task<IRepositoryActionResult> Handle(indexRequest request, CancellationToken cancellationToken)
        {
            var organos = financialAccountRepositoryQuery.FindAll(s => s.Id > 0);
            financialAccountsHelper.PopulateChildren(organos.Single(x => x.ParentId == 3), organos.ToList());
            return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.Ok, message: "Ok");
        }
    }
}
