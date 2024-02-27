using App.Application.Basic_Process;
using App.Infrastructure;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.Net.Http.Headers;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts
{
    public class UpdateFinancialAccountHandler : BusinessBase<GLFinancialAccount>, IRequestHandler<UpdateFinancialAccountRequest, IRepositoryActionResult>
    {
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GLPurchasesAndSalesSettings> _gLPurchasesAndSalesSettingsQuery;
       // private readonly IRepositoryQuery<GLJournalEntryDetailsAccounts> _gLJournalEntryDetailsAccountsQuery;
        private readonly IRepositoryQuery<GLGeneralSetting> generalSettingRepositoryQuery;
        private readonly IRepositoryQuery<GLFinancialCost> financialCostRepositoryQuery;
        private readonly IRepositoryCommand<GLFinancialCost> financialCostRepositoryCommand;
        private readonly IHttpContextAccessor httpContext;
        private readonly IRepositoryCommand<GLFinancialAccount> financialAccountRepositoryCommand;
        private readonly IRepositoryQuery<GLFinancialBranch> financialBranchRepositoryQuery;
        private readonly IRepositoryCommand<GLFinancialBranch> financialBranchRepositoryCommand;
        private readonly IRepositoryCommand<GLFinancialAccountHistory> financialAccountHistoryRepositoryCommand;
        private readonly iUserInformation _iUserInformation;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;

        public UpdateFinancialAccountHandler(IRepositoryActionResult repositoryActionResult, IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery, IRepositoryQuery<GLPurchasesAndSalesSettings> gLPurchasesAndSalesSettingsQuery, IRepositoryQuery<GLGeneralSetting> generalSettingRepositoryQuery, IRepositoryQuery<GLFinancialCost> financialCostRepositoryQuery, IRepositoryCommand<GLFinancialCost> financialCostRepositoryCommand, IHttpContextAccessor httpContext, IRepositoryCommand<GLFinancialAccount> financialAccountRepositoryCommand, IRepositoryQuery<GLFinancialBranch> financialBranchRepositoryQuery, IRepositoryCommand<GLFinancialBranch> financialBranchRepositoryCommand, IRepositoryCommand<GLFinancialAccountHistory> financialAccountHistoryRepositoryCommand, iUserInformation iUserInformation, ISystemHistoryLogsService systemHistoryLogsService) : base(repositoryActionResult)
        {
            this.financialAccountRepositoryQuery = financialAccountRepositoryQuery;
            _gLPurchasesAndSalesSettingsQuery = gLPurchasesAndSalesSettingsQuery;
            //_gLJournalEntryDetailsAccountsQuery = gLJournalEntryDetailsAccountsQuery;
            this.generalSettingRepositoryQuery = generalSettingRepositoryQuery;
            this.financialCostRepositoryQuery = financialCostRepositoryQuery;
            this.financialCostRepositoryCommand = financialCostRepositoryCommand;
            this.httpContext = httpContext;
            this.financialAccountRepositoryCommand = financialAccountRepositoryCommand;
            this.financialBranchRepositoryQuery = financialBranchRepositoryQuery;
            this.financialBranchRepositoryCommand = financialBranchRepositoryCommand;
            this.financialAccountHistoryRepositoryCommand = financialAccountHistoryRepositoryCommand;
            _iUserInformation = iUserInformation;
            _systemHistoryLogsService = systemHistoryLogsService;
        }
        public async Task<IRepositoryActionResult> Handle(UpdateFinancialAccountRequest parameter, CancellationToken cancellationToken)
        {
            try
            {
                
                var financialAccount = await financialAccountRepositoryQuery.SingleOrDefault(x => x.Id == parameter.Id && x.IsBlock == false, includes: role1 => role1.financialAccounts);
                var rolesChilderenId = financialAccountRepositoryQuery.FindQueryable(x => x.autoCoding.StartsWith(financialAccount.autoCoding) && x.Id != financialAccount.Id);
                var relationsSettingTable = _gLPurchasesAndSalesSettingsQuery.TableNoTracking.Where(x => x.FinancialAccountId == parameter.Id);
                if (parameter.IsMain != financialAccount.IsMain)
                {
                    if (rolesChilderenId.Any())
                        return repositoryActionResult.GetRepositoryActionResult(status: RepositoryActionStatus.ExistedBefore, message: "This Account have Childern you cant make it sub account");
                    //if (_gLJournalEntryDetailsAccountsQuery.TableNoTracking.Where(x => x.FinancialAccountId == parameter.Id).Any())
                    //    return repositoryActionResult.GetRepositoryActionResult(status: RepositoryActionStatus.ExistedBefore, message: "This Account have Have Operations");
                    if (relationsSettingTable.Any())
                        return repositoryActionResult.GetRepositoryActionResult(status: RepositoryActionStatus.BadRequest, message: "This Account have Have Operations");

                }

                var oldAutoCoding = financialAccount.autoCoding;
                var oldPirantID = financialAccount.ParentId;

                parameter.ArabicName = parameter.ArabicName.Trim();
                parameter.LatinName = parameter.LatinName.Trim();
                if (string.IsNullOrEmpty(parameter.LatinName))
                    parameter.LatinName = parameter.ArabicName;

                var table = Mapping.Mapper.Map<UpdateFinancialAccountParameter, GLFinancialAccount>(parameter, financialAccount);
                var setting = generalSettingRepositoryQuery.TableNoTracking.Include(x => x.subCodeLevels).FirstOrDefault();
                if (parameter.CostCenterId != null)
                {
                    table.HasCostCenter = 1;
                }
                else
                {
                    table.HasCostCenter = 2;
                }
                var financialCost = financialCostRepositoryQuery.FindQueryable(q => q.FinancialAccountId == financialAccount.Id).ToList();

                //Here we delete the related financial costs centers that relates to the financial account
                financialCostRepositoryCommand.RemoveRange(financialCost);
                if (parameter.CostCenterId != null)
                {
                    if (!table.IsMain)
                    {
                        foreach (var item in parameter.CostCenterId)
                        {
                            var costCenter = new GLFinancialCost();
                            costCenter.CostCenterId = item;
                            costCenter.FinancialAccountId = table.Id;
                            financialCostRepositoryCommand.Add(costCenter);
                        }
                    }
                }

                financialAccount.BrowserName = contextHelper.GetBrowserName(httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString());
                await financialAccountRepositoryCommand.UpdateAsyn(financialAccount);



                bool isChildrenChanged = false;
                if (parameter.Status != financialAccount.Status && parameter.Status == 2)
                {
                    rolesChilderenId.ToList().ForEach(x => x.Status = 2);
                    isChildrenChanged = true;
                }
                if (parameter.FinalAccount != financialAccount.FinalAccount)
                {
                    rolesChilderenId.ToList().ForEach(x => x.FinalAccount = financialAccount.FinalAccount);
                    isChildrenChanged = true;
                }
                if (isChildrenChanged)
                    await financialAccountRepositoryCommand.UpdateAsyn(rolesChilderenId);
                var financialBranch = financialBranchRepositoryQuery.FindAll(q => q.FinancialId == table.Id);
                financialBranchRepositoryCommand.RemoveRange(financialBranch);
                if (parameter.BranchesId != null)
                {
                    foreach (var item in parameter.BranchesId)
                    {
                        var bankBranche = new GLFinancialBranch();
                        bankBranche.BranchId = item;
                        bankBranche.FinancialId = table.Id;
                        financialBranchRepositoryCommand.AddWithoutSaveChanges(bankBranche);
                    }
                }

                await financialBranchRepositoryCommand.SaveAsync();
                financialAccountsHelper.HistoryFinancialAccount(financialAccountHistoryRepositoryCommand, _iUserInformation, financialAccount.CurrencyId, financialAccount.AccountCode, financialAccount.Status, financialAccount.FA_Nature, financialAccount.FinalAccount,
                    financialAccount.Credit, financialAccount.Debit, financialAccount.Notes, financialAccount.ParentId, financialAccount.HasCostCenter,
                    financialAccount.BrowserName, "U", financialAccount.LastTransactionUser);
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editCalculationGuide);
                return repositoryActionResult.GetRepositoryActionResult(financialAccount.Id, RepositoryActionStatus.Updated, message: "Updated Successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);
            }
        }
    }
}
