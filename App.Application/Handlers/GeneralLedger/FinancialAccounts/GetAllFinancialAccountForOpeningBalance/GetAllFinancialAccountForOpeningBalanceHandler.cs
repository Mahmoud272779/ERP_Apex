using App.Application.Basic_Process;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts
{
    public class GetAllFinancialAccountForOpeningBalanceHandler : BusinessBase<GLFinancialAccount>,IRequestHandler<GetAllFinancialAccountForOpeningBalanceRequest, IRepositoryActionResult>
    {
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GLFinancialAccountForOpeningBalance> financialAccountForOpeningBalanceRepositoryQuery;
        private readonly IRepositoryCommand<GLFinancialAccountForOpeningBalance> financialAccountForOpeningBalanceRepositoryCommand;

        public GetAllFinancialAccountForOpeningBalanceHandler(IRepositoryActionResult repositoryActionResult, IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery, IRepositoryQuery<GLFinancialAccountForOpeningBalance> financialAccountForOpeningBalanceRepositoryQuery, IRepositoryCommand<GLFinancialAccountForOpeningBalance> financialAccountForOpeningBalanceRepositoryCommand) : base(repositoryActionResult)
        {
            this.financialAccountRepositoryQuery = financialAccountRepositoryQuery;
            this.financialAccountForOpeningBalanceRepositoryQuery = financialAccountForOpeningBalanceRepositoryQuery;
            this.financialAccountForOpeningBalanceRepositoryCommand = financialAccountForOpeningBalanceRepositoryCommand;
        }
        public async Task<IRepositoryActionResult> Handle(GetAllFinancialAccountForOpeningBalanceRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var AllAccounts = financialAccountRepositoryQuery.FindQueryable(s => s.ParentId > 0 && s.IsBlock == false);
                var empCodes = financialAccountRepositoryQuery.FindSelectorQueryable<int>(AllAccounts, q => q.ParentId.Value);
                var List = empCodes.ToList();
                var parentDatas = financialAccountRepositoryQuery.FindAll(q => !List.Contains(q.Id));
                var existedparentDatasCodess = parentDatas.Select(q => q.Id);
                var list = new List<FinancialAccountForOpeningBalanceDto>();
                if (parentDatas != null)
                {
                    foreach (var parentData in parentDatas)
                    {
                        if (parentData == null)
                        {
                            return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.NotFound, message: "Not Found");
                        }
                        else
                        {
                            var CostCenterParents = new FinancialAccountForOpeningBalanceDto()
                            {
                                Id = parentData.Id,
                                ArabicName = parentData.ArabicName,
                                LatinName = parentData.LatinName,
                                Credit = parentData.OpenningCredit,
                                Debit = parentData.OpenningDebit,
                                Notes = parentData.Notes,
                                Code = parentData.AccountCode
                            };
                            var financialAccountForOpeningBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == parentData.AccountCode);
                            if (financialAccountForOpeningBalance == null)
                            {
                                var finanicalOpenningBalance = new GLFinancialAccountForOpeningBalance()
                                {
                                    ArabicName = CostCenterParents.ArabicName,
                                    LatinName = CostCenterParents.LatinName,
                                    Notes = CostCenterParents.Notes,
                                    Debit = CostCenterParents.Debit,
                                    Credit = CostCenterParents.Credit,
                                    AccountCode = CostCenterParents.Code
                                };
                                financialAccountForOpeningBalanceRepositoryCommand.Add(finanicalOpenningBalance);
                            }
                            list.Add(CostCenterParents);

                        }
                    }
                }

                return repositoryActionResult.GetRepositoryActionResult(list, RepositoryActionStatus.Ok, message: "Ok");

            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);

            }
        }
    }
}
