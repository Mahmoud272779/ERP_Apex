using App.Application.Basic_Process;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts.RecodingFinancialAccount
{
    public class RecodingFinancialAccountHandler : BusinessBase<GLFinancialAccount>,IRequestHandler<RecodingFinancialAccountRequest, IRepositoryActionResult>
    {
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GLGeneralSetting> generalSettingRepositoryQuery;
        private readonly IRepositoryCommand<GLFinancialAccount> financialAccountRepositoryCommand;

        public RecodingFinancialAccountHandler(IRepositoryActionResult repositoryActionResult, IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery, IRepositoryQuery<GLGeneralSetting> generalSettingRepositoryQuery, IRepositoryCommand<GLFinancialAccount> financialAccountRepositoryCommand) : base(repositoryActionResult)
        {
            this.financialAccountRepositoryQuery = financialAccountRepositoryQuery;
            this.generalSettingRepositoryQuery = generalSettingRepositoryQuery;
            this.financialAccountRepositoryCommand = financialAccountRepositoryCommand;
        }
        public async Task<IRepositoryActionResult> Handle(RecodingFinancialAccountRequest request, CancellationToken cancellationToken)
        {
            var accounts = financialAccountRepositoryQuery.GetAll().Where(x => x.autoCoding != null).ToList();
            var setting = await generalSettingRepositoryQuery.GetByAsync(a => true);
            financialAccountsHelper.recodingAccounts(accounts, setting);
            var saved = await financialAccountRepositoryCommand.UpdateAsyn(accounts);


            return repositoryActionResult.GetRepositoryActionResult();
        }
    }
}
