using App.Application.Basic_Process;
using App.Infrastructure;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts
{
    public class AddFinancialAccountHandler : BusinessBase<GLFinancialAccount>, IRequestHandler<AddFinancialAccountRequest, IRepositoryActionResult>
    {
        private readonly IRepositoryQuery<GLGeneralSetting> generalSettingRepositoryQuery;
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor httpContext;
        private readonly IRepositoryCommand<GLFinancialAccount> financialAccountRepositoryCommand;
        private readonly IRepositoryCommand<GLFinancialCost> financialCostRepositoryCommand;
        private readonly IRepositoryCommand<GLFinancialBranch> financialBranchRepositoryCommand;
        private readonly IRepositoryCommand<GLFinancialAccountHistory> financialAccountHistoryRepositoryCommand;
        private readonly iUserInformation _iUserInformation;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;


        public AddFinancialAccountHandler(IRepositoryQuery<GLGeneralSetting> generalSettingRepositoryQuery,
                                          IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery,
                                          IRepositoryActionResult repositoryActionResult,
                                          IHttpContextAccessor httpContext,
                                          IRepositoryCommand<GLFinancialAccount> financialAccountRepositoryCommand,
                                          IRepositoryCommand<GLFinancialCost> financialCostRepositoryCommand,
                                          IRepositoryCommand<GLFinancialBranch> financialBranchRepositoryCommand,
                                          IRepositoryCommand<GLFinancialAccountHistory> financialAccountHistoryRepositoryCommand,
                                          iUserInformation iUserInformation,
                                          ISystemHistoryLogsService systemHistoryLogsService) : base(repositoryActionResult)
        {
            this.generalSettingRepositoryQuery = generalSettingRepositoryQuery;
            this.financialAccountRepositoryQuery = financialAccountRepositoryQuery;
            this.httpContext = httpContext;
            this.financialAccountRepositoryCommand = financialAccountRepositoryCommand;
            this.financialCostRepositoryCommand = financialCostRepositoryCommand;
            this.financialBranchRepositoryCommand = financialBranchRepositoryCommand;
            this.financialAccountHistoryRepositoryCommand = financialAccountHistoryRepositoryCommand;
            _iUserInformation = iUserInformation;
            _systemHistoryLogsService = systemHistoryLogsService;
        }


        public async Task<IRepositoryActionResult> Handle(AddFinancialAccountRequest parameter, CancellationToken cancellationToken)
        {
            try
            {
                parameter.ArabicName = parameter.ArabicName.Trim();
                parameter.LatinName = parameter.LatinName.Trim();
                if (string.IsNullOrEmpty(parameter.LatinName))
                    parameter.LatinName = parameter.ArabicName;
                var table = Mapping.Mapper.Map<FinancialAccountParameter, GLFinancialAccount>(parameter);

                // can't map between int branch and int[]brances 
                // By Alaa
                var setting = generalSettingRepositoryQuery.TableNoTracking.Include(x => x.subCodeLevels).FirstOrDefault();
                // var setting = await generalSettingRepositoryQuery.GetByAsync(a=>a.BranchId==(parameter.BranchesId != null? parameter.BranchesId[0]:0));

                if (setting != null)
                {
                    if (setting.AutomaticCoding)
                    {
                        if (table.ParentId == null)
                        {
                            financialAccountsHelper.ParentCodes(table, setting, financialAccountRepositoryQuery);
                        }
                        else
                        {
                            financialAccountsHelper.ChildCodes(financialAccountRepositoryQuery, table, setting, parameter.ParentId);
                            if (table.AccountCode == null)
                                return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: ErrorMessagesEn.codingError);

                        }
                    }
                    else
                    {
                        // By Alaa
                        if (string.IsNullOrEmpty(parameter.AccountCode))
                            return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "Account code is required");
                        else
                            if (!Regex.IsMatch(parameter.AccountCode, "^(0|[1-9][0-9]*)$"))
                            return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "Account code Allows only Numbers");
                        var getParent = financialAccountRepositoryQuery.Find(x => x.Id == parameter.ParentId);
                        var lastElement = financialAccountRepositoryQuery.GetAll().Where(x => x.autoCoding.StartsWith(getParent.autoCoding)).OrderBy(x => x.Id).Last();
                        if (lastElement != null)
                        {
                            var autoCodeSplited = lastElement.autoCoding.Split('.');
                            var nextCode = int.Parse(autoCodeSplited.Last()) + 1;
                            string nextAutoCode = "";
                            for (int i = 0; i < autoCodeSplited.Length - 1; i++)
                            {
                                nextAutoCode = nextAutoCode + autoCodeSplited.ElementAt(i) + ".";
                            }
                            nextAutoCode = nextAutoCode + nextCode;
                            table.autoCoding = nextAutoCode;
                        }
                        table.AccountCode = table.AccountCode;
                        var checkCode = await financialAccountsHelper.CheckIsValidCode(table.AccountCode, financialAccountRepositoryQuery);
                        if (checkCode == true)
                        {
                            return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "This code Existing before ");
                        }
                    }
                }
                else
                {
                    table.AccountCode = await _mediator.Send(new AddAutomaticCodeRequest());
                    var checkCode = await financialAccountsHelper.CheckIsValidCode(table.AccountCode, financialAccountRepositoryQuery);
                    if (checkCode == true)
                    {
                        return repositoryActionResult.GetRepositoryActionResult(status: RepositoryActionStatus.ExistedBefore, message: "This code Existing before ");
                    }
                }
                if (parameter.CostCenterId.Count() == 0)
                {
                    table.HasCostCenter = 2;
                    table.BrowserName = contextHelper.GetBrowserName(httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString());
                    financialAccountRepositoryCommand.Add(table);
                }
                else
                {
                    table.HasCostCenter = 2;

                    table.BrowserName = contextHelper.GetBrowserName(httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString());

                    financialAccountRepositoryCommand.Add(table);
                }

                if (parameter.CostCenterId != null)
                {
                    foreach (var item in parameter.CostCenterId)
                    {
                        var costCenter = new GLFinancialCost();
                        costCenter.CostCenterId = item;
                        costCenter.FinancialAccountId = table.Id;
                        financialCostRepositoryCommand.Add(costCenter);

                    }
                }

                if (parameter.BranchesId != null)
                {
                    var ListbankBranch = new List<GLFinancialBranch>();

                    foreach (var item in parameter.BranchesId)
                    {
                        var bankBranch = new GLFinancialBranch();
                        bankBranch.BranchId = item;
                        bankBranch.FinancialId = table.Id;
                        ListbankBranch.Add(bankBranch);
                    }
                    financialBranchRepositoryCommand.AddRange(ListbankBranch);
                }
                await financialBranchRepositoryCommand.SaveAsync();
                financialAccountsHelper.HistoryFinancialAccount(financialAccountHistoryRepositoryCommand, _iUserInformation, table.CurrencyId, table.AccountCode, table.Status, table.FA_Nature, table.FinalAccount,
                    table.Credit, table.Debit, table.Notes, table.ParentId, table.HasCostCenter, table.BrowserName, "A", table.LastTransactionUser);
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addCalculationGuide);
                return repositoryActionResult.GetRepositoryActionResult(table.Id, RepositoryActionStatus.Created, message: "Saved Successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex, status: RepositoryActionStatus.BadRequest, message: "Exception");

            }
        }
    }
}
