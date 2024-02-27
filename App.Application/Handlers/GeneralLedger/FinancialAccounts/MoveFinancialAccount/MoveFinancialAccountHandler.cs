
using App.Application.Basic_Process;
using App.Infrastructure;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts
{
    public class MoveFinancialAccountHandler : BusinessBase<GLFinancialAccount>,IRequestHandler<MoveFinancialAccountRequest, IRepositoryActionResult>
    {
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GLGeneralSetting> generalSettingRepositoryQuery;
        private readonly IRepositoryCommand<GLFinancialAccount> financialAccountRepositoryCommand;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;

        public MoveFinancialAccountHandler(IRepositoryActionResult repositoryActionResult, IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery, IRepositoryQuery<GLGeneralSetting> generalSettingRepositoryQuery, IRepositoryCommand<GLFinancialAccount> financialAccountRepositoryCommand, ISystemHistoryLogsService systemHistoryLogsService) : base(repositoryActionResult)
        {
            this.financialAccountRepositoryQuery = financialAccountRepositoryQuery;
            this.generalSettingRepositoryQuery = generalSettingRepositoryQuery;
            this.financialAccountRepositoryCommand = financialAccountRepositoryCommand;
            _systemHistoryLogsService = systemHistoryLogsService;
        }
        public async Task<IRepositoryActionResult> Handle(MoveFinancialAccountRequest parameter, CancellationToken cancellationToken)
        {
            try
            {
                if (!parameter.IsMainMoveTo)
                {
                    GLFinancialAccount financialTo = new GLFinancialAccount();
                    var allFinancialList = new List<GLFinancialAccount>();
                    if (parameter.FinancialIdMovedTo != 0)
                    {
                        financialTo = await financialAccountRepositoryQuery.GetByAsync(q => q.Id == parameter.FinancialIdMovedTo);
                        if (!financialTo.IsMain)
                            return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: ErrorMessagesEn.AccountIsNotMain);
                    }
                    var financial = await financialAccountRepositoryQuery.GetByAsync(q => q.Id == parameter.FinancialIdWillMove);
                    var oldAutoCoding = financial.autoCoding;
                    var oldAccountCoding = financial.AccountCode;
                    var oldPirantID = financial.ParentId;
                    var setting = generalSettingRepositoryQuery.TableNoTracking.Include(x => x.subCodeLevels).FirstOrDefault();
                    var firstchild = await financialAccountRepositoryQuery.SingleOrDefault(s => s.Id == parameter.FinancialIdWillMove && s.IsBlock == false, includes: role1 => role1.financialAccounts);
                    var rolesChilderenId = financialAccountsHelper.GetAllChildren(firstchild, financialAccountRepositoryQuery).OrderBy(a => a.AccountCode).ToList();

                    if (setting != null)
                    {

                        var oldMovedAccountCode = financial.AccountCode;
                        if (parameter.FinancialIdMovedTo == 0)
                        {
                            financialAccountsHelper.ParentCodes(financial, setting, financialAccountRepositoryQuery);
                        }
                        else
                        {
                            financialAccountsHelper.ChildCodes(financialAccountRepositoryQuery, financial, setting, parameter.FinancialIdMovedTo, true, null, true);
                            if (financial.AccountCode == null)
                                return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: ErrorMessagesEn.codingError);
                        }
                        if (parameter.FinancialIdMovedTo != 0)
                            financial.ParentId = parameter.FinancialIdMovedTo;
                        else
                            financial.ParentId = null;

                        financial.FinalAccount = financialTo.FinalAccount;
                        await financialAccountRepositoryCommand.UpdateAsyn(financial);

                        //var allChilds = financialAccountRepositoryQuery.GetAll().Where(x => x.autoCoding.StartsWith(oldAutoCoding) && x.Id != financial.Id).ToList();
                        if (rolesChilderenId.Any())
                        {
                            int numOfSub;
                            var codeLevel = financial.AccountCode.Split('.').Length + 1;
                            if (setting.subCodeLevels.Select(x => x.value).ToArray().Length >= codeLevel)
                                numOfSub = setting.subCodeLevels.Skip(codeLevel - 1).Select(x => x.value).First();
                            else
                                numOfSub = setting.subCodeLevels.Select(x => x.value).Last();
                            var NewMovedAccountCode = financial.AccountCode;
                            for (int i = 0; i < rolesChilderenId.Count; i++)
                            {
                                var pID = rolesChilderenId.ElementAt(i).ParentId;
                                var Parentfinancial = rolesChilderenId.Where(q => q.Id == rolesChilderenId.ElementAt(i).ParentId).FirstOrDefault();
                                if (pID == financial.Id)
                                    Parentfinancial = financial;
                                if (Parentfinancial == null)
                                    continue;
                                rolesChilderenId.ElementAt(i).AccountCode = Parentfinancial.AccountCode + '.' + (allFinancialList.Where(x => x.AccountCode.StartsWith(Parentfinancial.AccountCode) && x.ParentId == Parentfinancial.Id).Count() + 1).ToString().PadLeft(numOfSub, '0');
                                rolesChilderenId.ElementAt(i).autoCoding = Parentfinancial.autoCoding + '.' + (allFinancialList.Where(x => x.AccountCode.StartsWith(Parentfinancial.AccountCode) && x.ParentId == Parentfinancial.Id).Count() + 1).ToString();
                                if (financialTo.Status == 2)
                                    rolesChilderenId.ElementAt(i).Status = 2;
                                rolesChilderenId.ElementAt(i).FinalAccount = financialTo.FinalAccount;
                                allFinancialList.Add(rolesChilderenId.ElementAt(i));
                            }
                            await financialAccountRepositoryCommand.UpdateAsyn(allFinancialList);
                        }



                    }
                    await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editCalculationGuide);
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.Updated, message: "Updated Successfully", status: RepositoryActionStatus.Ok);

                }
                else
                {
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.NothingModified, message: "Can't move to sub");

                }
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);
            }
        }
    }
}
