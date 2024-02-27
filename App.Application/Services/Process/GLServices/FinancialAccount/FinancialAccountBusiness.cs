using App.Application.Basic_Process;
using App.Application.Handlers.GeneralLedger.FinancialAccounts;
using App.Domain.Models.Common;
using App.Infrastructure;
using Attendleave.Erp.Core.APIUtilities;
using Attendleave.Erp.ServiceLayer.Abstraction;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace App.Application.Services.Process.FinancialAccounts
{
    public class FinancialAccountBusiness : BusinessBase<GLFinancialAccount>, IFinancialAccountBusiness
    {
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GLGeneralSetting> generalSettingRepositoryQuery;
        private readonly IRepositoryQuery<GLCurrency> currencyRepositoryQuery;
        private readonly IRepositoryQuery<GLJournalEntryDetails> journalEntryRepositoryQuery;
        private readonly IRepositoryCommand<GLFinancialAccount> financialAccountRepositoryCommand;
        private readonly IRepositoryCommand<GLFinancialCost> financialCostRepositoryCommand;
        private readonly IRepositoryCommand<GLFinancialBranch> financialBranchRepositoryCommand;
        private readonly IRepositoryQuery<GLFinancialBranch> financialBranchRepositoryQuery;
        private readonly IRepositoryQuery<GLFinancialCost> financialCostRepositoryQuery;
        private readonly IRepositoryQuery<GLBranch> branchRepositoryQuery;
        private readonly IRepositoryQuery<GLPurchasesAndSalesSettings> _gLPurchasesAndSalesSettingsQuery;
        private readonly IRepositoryQuery<GLCostCenter> costCenterRepositoryQuery;
        private readonly IRepositoryQuery<InvPersons> _invPersonsQuery;
        private readonly IRepositoryQuery<InvSalesMan> _invSalesManQuery;
        private readonly IRepositoryQuery<InvEmployees> _invEmployeesQuery;
        private readonly IRepositoryCommand<GLFinancialAccountForOpeningBalance> financialAccountForOpeningBalanceRepositoryCommand;
        private readonly IRepositoryQuery<GLFinancialAccountForOpeningBalance> financialAccountForOpeningBalanceRepositoryQuery;
        private readonly IPagedList<FinancialAccountDto, FinancialAccountDto> pagedListFinancialAccountDto;
        private readonly IPagedList<GLFinancialAccount, GLFinancialAccount> glFinancialAccountPagedList;
        private readonly IHttpContextAccessor httpContext;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;   
        private readonly IRepositoryQuery<GLBank> _gLBanKQuery;
        private readonly IRepositoryQuery<GLFinancialAccountBranch> _gLFinancialAccountBranchQuery;
        private readonly IRepositoryQuery<GLFinancialSetting> _gLFinancialSettingQuery;
       // private readonly IRepositoryQuery<GLJournalEntryDetailsAccounts> _gLJournalEntryDetailsAccountsQuery;
        private readonly IRepositoryQuery<GLSafe> _gLSafeQuery;
        private readonly IRepositoryQuery<GlReciepts> _gLRecieptQuery;
        private readonly IRepositoryQuery<GLOtherAuthorities> _gLOtherAuthoritiesQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> _invGeneralSettingsQuery;
        private readonly IHelperService _helperService;
        private readonly IRepositoryCommand<GLFinancialAccountHistory> financialAccountHistoryRepositoryCommand;
        private readonly IRepositoryQuery<GLFinancialAccountHistory> financialAccountHistoryRepositoryQuery;
        private readonly iUserInformation _iUserInformation;

        public FinancialAccountBusiness(
            IRepositoryQuery<GLFinancialAccount> FinancialAccountRepositoryQuery,
            IRepositoryCommand<GLFinancialAccount> FinancialAccountRepositoryCommand,
            IRepositoryQuery<GLGeneralSetting> GeneralSettingRepositoryQuery,
            IRepositoryQuery<GLCurrency> CurrencyRepositoryQuery,
            IRepositoryQuery<GLFinancialAccountHistory> FinancialAccountHistoryRepositoryQuery,
            IRepositoryCommand<GLFinancialAccountHistory> FinancialAccountHistoryRepositoryCommand,
            IRepositoryCommand<GLFinancialBranch> FinancialBranchRepositoryCommand,
            IRepositoryCommand<GLFinancialCost> FinancialCostRepositoryCommand,
            IRepositoryQuery<GLFinancialCost> FinancialCostRepositoryQuery,
            IRepositoryQuery<GLFinancialBranch> FinancialBranchRepositoryQuery,
            IRepositoryQuery<GLCostCenter> CostCenterRepositoryQuery,
            IRepositoryQuery<InvPersons> InvPersonsQuery,
            IRepositoryQuery<InvSalesMan> InvSalesManQuery,
            IRepositoryQuery<InvEmployees> InvEmployeesQuery,
            IRepositoryQuery<GLFinancialAccountForOpeningBalance> FinancialAccountForOpeningBalanceRepositoryQuery,
            IRepositoryQuery<GLJournalEntryDetails> JournalEntryRepositoryQuery,
            IRepositoryQuery<GLBranch> BranchRepositoryQuery,
            IRepositoryQuery<GLPurchasesAndSalesSettings> GLPurchasesAndSalesSettingsQuery,
            IRepositoryCommand<GLFinancialAccountForOpeningBalance> FinancialAccountForOpeningBalanceRepositoryCommand,
            IPagedList<FinancialAccountDto, FinancialAccountDto> PagedListFinancialAccountDto,
            IPagedList<GLFinancialAccount, GLFinancialAccount> _glFinancialAccountPagedList,
            IHttpContextAccessor HttpContext,
            ISystemHistoryLogsService systemHistoryLogsService,
            IRepositoryQuery<GLBank> GLBanKQuery,
            IRepositoryQuery<GLFinancialAccountBranch> GLFinancialAccountBranchQuery,
            IRepositoryQuery<GLFinancialSetting> GLFinancialSettingQuery,
           // IRepositoryQuery<GLJournalEntryDetailsAccounts> GLJournalEntryDetailsAccountsQuery,
            IRepositoryQuery<GLSafe> GLSafeQuery,
            IRepositoryQuery<GlReciepts> GLRecieptQuery,
            IRepositoryQuery<GLOtherAuthorities> GLOtherAuthoritiesQuery,
            IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsQuery,
            IHelperService helperService,
            IRepositoryActionResult repositoryActionResult,
            iUserInformation iUserInformation) : base(repositoryActionResult)
        {
            financialAccountRepositoryQuery = FinancialAccountRepositoryQuery;
            financialAccountRepositoryCommand = FinancialAccountRepositoryCommand;
            generalSettingRepositoryQuery = GeneralSettingRepositoryQuery;
            financialAccountForOpeningBalanceRepositoryQuery = FinancialAccountForOpeningBalanceRepositoryQuery;
            journalEntryRepositoryQuery = JournalEntryRepositoryQuery;
            httpContext = HttpContext;
            _systemHistoryLogsService = systemHistoryLogsService;
            _gLBanKQuery = GLBanKQuery;
            _gLFinancialAccountBranchQuery = GLFinancialAccountBranchQuery;
            _gLFinancialSettingQuery = GLFinancialSettingQuery;
            //_gLJournalEntryDetailsAccountsQuery = GLJournalEntryDetailsAccountsQuery;
            _gLSafeQuery = GLSafeQuery;
            _gLRecieptQuery = GLRecieptQuery;
            _gLOtherAuthoritiesQuery = GLOtherAuthoritiesQuery;
            _invGeneralSettingsQuery = InvGeneralSettingsQuery;
            _helperService = helperService;
            currencyRepositoryQuery = CurrencyRepositoryQuery;
            costCenterRepositoryQuery = CostCenterRepositoryQuery;
            _invPersonsQuery = InvPersonsQuery;
            _invSalesManQuery = InvSalesManQuery;
            _invEmployeesQuery = InvEmployeesQuery;
            financialCostRepositoryCommand = FinancialCostRepositoryCommand;
            financialBranchRepositoryQuery = FinancialBranchRepositoryQuery;
            financialAccountForOpeningBalanceRepositoryCommand = FinancialAccountForOpeningBalanceRepositoryCommand;
            pagedListFinancialAccountDto = PagedListFinancialAccountDto;
            glFinancialAccountPagedList = _glFinancialAccountPagedList;
            financialAccountHistoryRepositoryCommand = FinancialAccountHistoryRepositoryCommand;
            financialAccountHistoryRepositoryQuery = FinancialAccountHistoryRepositoryQuery;
            financialBranchRepositoryCommand = FinancialBranchRepositoryCommand;
            branchRepositoryQuery = BranchRepositoryQuery;
            _gLPurchasesAndSalesSettingsQuery = GLPurchasesAndSalesSettingsQuery;
            financialCostRepositoryQuery = FinancialCostRepositoryQuery;
            _iUserInformation = iUserInformation;
        }
        public async Task<string> AddAutomaticCode()
        {
            var code = financialAccountRepositoryQuery.FindQueryable(q => q.Id > 0);
            if (code.Count() > 0)
            {
                var code2 = financialAccountRepositoryQuery.FindQueryable(q => q.Id > 0).ToList().Last();
                var coo = code2.AccountCode;
                var codee = long.Parse(coo.ToString());
                if (codee == 0)
                {
                }
                var NewCode = codee + 1;
                return NewCode.ToString();
            }
            else
            {
                var NewCode = 1;
                return NewCode.ToString();
            }
        }
        public async Task<IRepositoryActionResult> AddFinancialAccount(FinancialAccountParameter parameter)
        {
            try
            {
                parameter.ArabicName = parameter.ArabicName.Trim();
                parameter.LatinName = parameter.LatinName.Trim();
                if (string.IsNullOrEmpty(parameter.LatinName))
                    parameter.LatinName = parameter.ArabicName;
                var table = Mapping.Mapper.Map<FinancialAccountParameter, GLFinancialAccount>(parameter);

                var setting = generalSettingRepositoryQuery.TableNoTracking.Include(x => x.subCodeLevels).FirstOrDefault();

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
                    table.AccountCode = await AddAutomaticCode();
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

                    if (parameter.withOutSaving)
                    {
                        return repositoryActionResult.GetRepositoryActionResult(table, RepositoryActionStatus.Created, message: "Saved Successfully");
                    }
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
                    table.Credit, table.Debit, table.Notes, table.ParentId, table.HasCostCenter, table.BrowserName, table.LastTransactionAction, table.LastTransactionUser);
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addCalculationGuide);
                return repositoryActionResult.GetRepositoryActionResult(table.Id, RepositoryActionStatus.Created, message: "Saved Successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex, status: RepositoryActionStatus.BadRequest, message: "Exception");

            }
        }
        public async Task<IRepositoryActionResult> UpdateFinancialAccount(UpdateFinancialAccountParameter parameter)
        {
            try
            {
                //var _financialAccount = financialAccountRepositoryQuery.TableNoTracking.Where(q => q.Id == parameter.Id).FirstOrDefault();
                //var children = financialAccountRepositoryQuery.TableNoTracking.Where(x => x.autoCoding.StartsWith(_financialAccount.autoCoding));
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
                    financialAccount.BrowserName, financialAccount.LastTransactionAction, financialAccount.LastTransactionUser);
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editCalculationGuide);
                return repositoryActionResult.GetRepositoryActionResult(financialAccount.Id, RepositoryActionStatus.Updated, message: "Updated Successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);
            }
        }
        public async Task<IRepositoryActionResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameter)
        {
            try
            {
                if (parameter.Id.Count() == 0)
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.UpdateFileWithErrors, message: "Error", status: RepositoryActionStatus.BadRequest);
                var accounts = financialAccountRepositoryQuery.TableNoTracking;
                var isFinancialAccountsExist = accounts.Where(x => parameter.Id.Contains(x.Id));
                List<GLFinancialAccount> _findAllAccountsWithChild = new List<GLFinancialAccount>();
                foreach (var account in isFinancialAccountsExist)
                {
                    var children = accounts.Where(x => x.autoCoding.StartsWith(account.autoCoding)).ToList();
                    foreach (var child in children)
                    {
                        if (!_findAllAccountsWithChild.Where(x => x.Id == child.Id).Any())
                        {
                            child.Status = parameter.Status;
                            _findAllAccountsWithChild.Add(child);
                        }
                    }
                }
                var updated = await financialAccountRepositoryCommand.UpdateAsyn(_findAllAccountsWithChild);
                if (updated)
                    await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editCalculationGuide);
                return repositoryActionResult.GetRepositoryActionResult(updated ? RepositoryActionStatus.Updated : RepositoryActionStatus.UpdateFileWithErrors, message: updated ? "Updated Successfully" : "Error", status: updated ? RepositoryActionStatus.Ok : RepositoryActionStatus.BadRequest);
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);
            }
        }
        /// <summary>
        /// stopped here 
        /// </summary>
        public async Task<IRepositoryActionResult> DeleteFinancialAccountAsync(SharedRequestDTOs.Delete parameter)
        {
            try
            {
                parameter.Ids = parameter.Ids.Where(x => x > 0).ToArray();

                if (parameter.Ids.Count() == 0)
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.CanNotDelete, message: "Error", status: RepositoryActionStatus.BadRequest);
                var accounts = financialAccountRepositoryQuery.TableNoTracking;
                var isFinancialAccountsExist = accounts.Where(x => parameter.Ids.Contains(x.Id));
                var parentId = isFinancialAccountsExist.Select(x => x.ParentId).FirstOrDefault();
                List<GLFinancialAccount> _findAllAccountsWithChild = new List<GLFinancialAccount>();
                foreach (var account in isFinancialAccountsExist)
                {
                    var children = accounts.Where(x => x.autoCoding.StartsWith(account.autoCoding)).ToList();
                    foreach (var child in children)
                    {
                        if (!_findAllAccountsWithChild.Where(x => x.Id == child.Id).Any())
                            _findAllAccountsWithChild.Add(child);
                    }
                }


                var findAllAccountsWithChild = _findAllAccountsWithChild.Select(x => x.Id).ToArray();

                //bank
                var CheckBanks = _gLBanKQuery.TableNoTracking.Where(x => findAllAccountsWithChild.Contains((int)x.FinancialAccountId));
                if (CheckBanks.Any())
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant delete those account becuase some of them used in GLBanks Table");

                //var check_GLJournalEntryDetailsAccounts = _gLJournalEntryDetailsAccountsQuery.TableNoTracking.Where(x => findAllAccountsWithChild.Contains(x.FinancialAccountId));
                //if (check_GLJournalEntryDetailsAccounts.Any())
                //    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant delete those account becuase some of them used in GLJournalEntryDetailsAccounts Table");
                //safes
                var check_GLSafe = _gLSafeQuery.TableNoTracking.Where(x => findAllAccountsWithChild.Contains((int)x.FinancialAccountId));
                if (check_GLSafe.Any())
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant delete those account becuase some of them used in GLSafe Table");
                //recs
                var check_gLReciept = _gLRecieptQuery.TableNoTracking.Where(x => findAllAccountsWithChild.Contains(x.FinancialAccountId.Value));
                if (check_gLReciept.Any())
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant delete those account becuase some of them used in gLReciept Table");
                //oter authorities
                var check_gLOtherAuthorities = _gLOtherAuthoritiesQuery.TableNoTracking.Where(x => findAllAccountsWithChild.Contains((int)x.FinancialAccountId));
                if (check_gLOtherAuthorities.Any())
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant delete those account becuase some of them used in gLOtherAuthorities Table");
                //Employees
                var checkEmps = _invEmployeesQuery.TableNoTracking.Where(x => findAllAccountsWithChild.Contains((int)x.FinancialAccountId));
                if (checkEmps.Any())
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant delete those account becuase some of them used in Employees Table");
                //persons
                var checkPersons = _invPersonsQuery.TableNoTracking.Where(x => findAllAccountsWithChild.Contains((int)x.FinancialAccountId));
                if (checkPersons.Any())
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant delete those account becuase some of them used in Persons Table");
                //salesMan
                var checkSalesMan = _invSalesManQuery.TableNoTracking.Where(x => findAllAccountsWithChild.Contains((int)x.FinancialAccountId));
                if (checkSalesMan.Any())
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant delete those account becuase some of them used in salesman Table");
                //Relationship
                var checkRelation = _gLPurchasesAndSalesSettingsQuery.TableNoTracking.Where(x => findAllAccountsWithChild.Contains((int)x.FinancialAccountId));
                if (checkRelation.Any())
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant delete those account becuase some of them used in gLPurchasesAndSalesSettings Table");
                var checkRelation2 = generalSettingRepositoryQuery.TableNoTracking.Where(x => findAllAccountsWithChild.Contains((int)x.FinancialAccountIdBank) ||
                                                                                              findAllAccountsWithChild.Contains((int)x.FinancialAccountIdCustomer) ||
                                                                                              findAllAccountsWithChild.Contains((int)x.FinancialAccountIdEmployee) ||
                                                                                              findAllAccountsWithChild.Contains((int)x.FinancialAccountIdOtherAuthorities) ||
                                                                                              findAllAccountsWithChild.Contains((int)x.FinancialAccountIdSafe) ||
                                                                                              findAllAccountsWithChild.Contains((int)x.FinancialAccountIdSalesMan) ||
                                                                                              findAllAccountsWithChild.Contains((int)x.FinancialAccountIdSupplier));
                if (checkRelation.Any())
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.BadRequest, message: "you cant delete those account becuase some of them used in generalSetting Table");

                financialAccountRepositoryCommand.RemoveRange(_findAllAccountsWithChild.OrderByDescending(x => x.Id));
                await financialAccountRepositoryCommand.SaveAsync();
                var dataRes = new financialAccountsHelper.DeleteResponse()
                {
                    parentId = parentId
                };
                int count = parameter.Ids.Count();
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteCalculationGuide,count);
                return repositoryActionResult.GetRepositoryActionResult(status: RepositoryActionStatus.Deleted, message: "Deleted Successfully", result: dataRes);
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex.Message);
            }
        }
        public async Task<IRepositoryActionResult> GetFinancialAccountById(int Id)
        {
            try
            {
                if (Id == 0)
                    return repositoryActionResult.GetRepositoryActionResult(null, RepositoryActionStatus.BadRequest, message: "Select cost center");
                var allAccounts = financialAccountsHelper.getAllAccounts(financialAccountRepositoryQuery);
                var allBanks = financialAccountsHelper.getAllBanks(_gLBanKQuery);
                var allJournalEntryDetails = financialAccountsHelper.getAllGLJournalEntryDetails(journalEntryRepositoryQuery);
                var allSafes = financialAccountsHelper.getAllGLSafes(_gLSafeQuery);
                var allReciptc = financialAccountsHelper.getAllGLReciept(_gLRecieptQuery);
                var allOtherAuthorities = financialAccountsHelper.getAllGLOtherAuthorities(_gLOtherAuthoritiesQuery);


                var costs = financialCostRepositoryQuery.FindQueryable(q => q.FinancialAccountId == Id).Select(x => x.CostCenterId).ToArray();
                var financialBranch = financialBranchRepositoryQuery.FindQueryable(q => q.FinancialId == Id).Select(x => x.BranchId).ToArray();

                var _account = allAccounts
                    .Include(x => x.Currency)
                    .Where(x => x.Id == Id);



                var account = _account.ToList()
                    .Select(x => new
                    {
                        nodeId = x.Id,
                        Code = x.AccountCode.Replace(".", string.Empty),
                        FA_Nature = x.FA_Nature,
                        FinalAccount = x.FinalAccount,
                        x.Status,
                        CurrencyId = x.CurrencyId,
                        CurrencyName = x.Currency.ArabicName,
                        x.ArabicName,
                        x.LatinName,
                        x.IsMain,
                        x.Notes,
                       // isHaveOperation = _gLJournalEntryDetailsAccountsQuery.TableNoTracking.Where(x => x.FinancialAccountId == Id).Any(),
                        isHaveChildren = financialAccountRepositoryQuery.TableNoTracking.Where(x => x.ParentId == Id).Any(),
                        x.ParentId,
                        ParentName = financialAccountRepositoryQuery.TableNoTracking.Where(q => q.Id == x.ParentId).Select(x => x.ArabicName).FirstOrDefault(),
                        costCenters = costCenterRepositoryQuery.TableNoTracking.Where(q => costs.Contains(q.Id)).Select(x => new { x.Id, x.ArabicName, x.LatinName }).ToList(),
                        financialBranchList = branchRepositoryQuery.TableNoTracking.Where(q => financialBranch.Contains(q.Id)).Select(x => new { x.Id, x.ArabicName, x.LatinName }).ToList(),
                        canDelete = financialAccountsHelper.CanDelete(x.autoCoding, allAccounts, allBanks, allJournalEntryDetails, allSafes, allReciptc, allOtherAuthorities).Result
                    }).FirstOrDefault();

                return repositoryActionResult.GetRepositoryActionResult(account, RepositoryActionStatus.Ok, message: "Ok");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);

            }
        }
        public async Task<IRepositoryActionResult> GetAllFinancialAccount(FA_Search paramters, int? id, int? pageNumber, int? pageSize)
        {
            pageNumber = pageNumber ?? 1;
            pageSize = pageSize ?? 20;
            var allAccounts = financialAccountsHelper.getAllAccounts(financialAccountRepositoryQuery);
            var allBanks = financialAccountsHelper.getAllBanks(_gLBanKQuery);
            var allJournalEntryDetails = financialAccountsHelper.getAllGLJournalEntryDetails(journalEntryRepositoryQuery);
            var allSafes = financialAccountsHelper.getAllGLSafes(_gLSafeQuery);
            var allReciptc = financialAccountsHelper.getAllGLReciept(_gLRecieptQuery);
            var allOtherAuthorities = financialAccountsHelper.getAllGLOtherAuthorities(_gLOtherAuthoritiesQuery);
            try
            {


                var searchCretiera = paramters.SearchCriteria;
                var parentDatasQuey = allAccounts.OrderBy(x => x.AccountCode).Where(x => (string.IsNullOrEmpty(paramters.SearchCriteria) ? x.ParentId == id : true) && x.IsBlock == false);
                var C_D = journalEntryRepositoryQuery.TableNoTracking
                            .Include(x => x.GLFinancialAccount)
                            .Include(x => x.journalEntry);

                if (!string.IsNullOrEmpty(paramters.SearchCriteria))
                    parentDatasQuey = parentDatasQuey.Where(x =>
                    searchCretiera == string.Empty ||
                    searchCretiera == null ||
                    x.ArabicName.ToLower().Contains(searchCretiera) ||
                    x.LatinName.ToLower().Contains(searchCretiera) ||
                    x.AccountCode.Replace(".", string.Empty) == searchCretiera
                    );


                var pagin = id != null ? parentDatasQuey.Skip(((pageNumber ?? 1) - 1) * 20).Take(20) : parentDatasQuey;
                var result = pagin.ToList();
                pageNumber = pageNumber == null ? 1 : pageNumber;
                double MaxPageNumber = parentDatasQuey.Count() / Convert.ToDouble(pageSize);
                var countofFilter = Math.Ceiling(MaxPageNumber);
                //Note Id numbe 0 is taken by load more property
                if (pageNumber != countofFilter)
                {
                    GLFinancialAccount gLFinancialAccount = new GLFinancialAccount()
                    {
                        Id = 0,
                        ArabicName = "...عرض المزيد",
                        LatinName = "...Load More",
                        nextPageNumber = pageNumber + 1,
                        ParentId = id,
                        FA_Nature = 3,
                        IsMain = true
                    };
                    result.Add(gLFinancialAccount);
                }

                #region new selector
                //var _Accounts = Mapping.Mapper.Map<List<GLFinancialAccount>, List<FinancialAccountDto>>(parentDatasQuey.ToList());
                var accounts = result.Select(x => new
                {
                    nodeId = x.Id,
                    ArabicName = x.ArabicName,
                    LatinName = x.LatinName,
                    ParentId = x.ParentId,
                    Status = x.Status,
                    FA_Nature = x.FA_Nature,
                    CurrencyId = x.CurrencyId,
                    IsMain = x.IsMain,
                    Code = x.AccountCode.Replace(".", string.Empty),
                    Debit = x.Debit,
                    Credit = x.Credit,
                    FinalAccount = x.FinalAccount,
                    HasCostCenter = x.HasCostCenter,
                    total = _helperService.GetFinanicalAccountTotalAmount(x.Id, x.autoCoding, C_D).Result,
                    hasChildren = financialAccountsHelper.checkIfHasChildren(x.Id, allAccounts).Result,
                    canDelete = financialAccountsHelper.CanDelete(x.autoCoding, allAccounts, allBanks, allJournalEntryDetails, allSafes, allReciptc, allOtherAuthorities).Result,
                    x.nextPageNumber
                });
                #endregion



                return repositoryActionResult.GetRepositoryActionResult(accounts, RepositoryActionStatus.Ok, message: "Ok");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);
            }
        }
        public async Task<IRepositoryActionResult> GetAllFinancialAccountForOpeningBalance()
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
        public async Task<IRepositoryActionResult> index()
        {
            var organos = financialAccountRepositoryQuery.FindAll(s => s.Id > 0);
            financialAccountsHelper.PopulateChildren(organos.Single(x => x.ParentId == 3), organos.ToList());
            return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.Ok, message: "Ok");
        }
        public async Task<IRepositoryActionResult> GetAllFinancialAccountDropdown()
        {
            try
            {
                var parentData = financialAccountRepositoryQuery.GetAll().Where(q => q.IsBlock == false).Select(x => new
                {
                    x.Id,
                    x.ArabicName,
                    x.LatinName,
                    x.Credit,
                    x.Debit,
                    x.AccountCode
                }).ToList();
                var list = new List<CostCenterDto>();
                if (parentData != null)
                {
                    foreach (var item in parentData)
                    {
                        var CostCenterDto = new CostCenterDto();
                        CostCenterDto.Id = item.Id;
                        CostCenterDto.ArabicName = item.ArabicName;
                        CostCenterDto.LatinName = item.LatinName;
                        CostCenterDto.Code = item.AccountCode;
                        CostCenterDto.Credit = item.Credit;
                        CostCenterDto.Debit = item.Debit;
                        list.Add(CostCenterDto);
                    }
                }

                // var result = pagedListFinancialAccountDto.GetGenericPagination(list, paramters.PageNumber, paramters.PageSize, Mapper);
                return repositoryActionResult.GetRepositoryActionResult(list, RepositoryActionStatus.Ok, message: "Ok");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);

            }
        }
        public async Task<ResponseResult> GetAllFinancialAccountHistory(int id)
        {
            var accountCode = financialAccountRepositoryQuery.TableNoTracking.Where(x => x.Id == id).Select(x => x.AccountCode).FirstOrDefault();
            var parentDatasQuey = financialAccountHistoryRepositoryQuery.TableNoTracking.Where(s => s.AccountCode == accountCode).Include(a => a.employees).ToList();
            var historyList = new List<HistoryResponceDto>();
            foreach (var item in parentDatasQuey.ToList())
            {
                var historyDto = new HistoryResponceDto();
                historyDto.Date = item.LastDate;

                HistoryActionsNames actionName = HistoryActionsAliasNames.HistoryName[item.LastAction];
                historyDto.TransactionTypeAr = actionName.ArabicName;
                historyDto.TransactionTypeEn = actionName.LatinName;
                historyDto.LatinName = item.employees.LatinName;
                historyDto.ArabicName = item.employees.ArabicName;
                historyDto.BrowserName = item.BrowserName;

                if (item.BrowserName.Contains("Chrome"))
                {
                    historyDto.BrowserName = "Chrome";
                }
                if (item.BrowserName.Contains("Firefox"))
                {
                    historyDto.BrowserName = "Firefox";
                }
                if (item.BrowserName.Contains("Opera"))
                {
                    historyDto.BrowserName = "Opera";
                }
                if (item.BrowserName.Contains("InternetExplorer"))
                {
                    historyDto.BrowserName = "InternetExplorer";
                }
                if (item.BrowserName.Contains("Microsoft Edge"))
                {
                    historyDto.BrowserName = "Microsoft Edge";
                }
                historyList.Add(historyDto);
            }
            return new ResponseResult { Data = historyList, Result = Result.Success, Note = Aliases.Actions.Success };
        }
        public async Task<IRepositoryActionResult> GetAllFinancialAccountDropDown()
        {
            var AllAccounts = financialAccountRepositoryQuery.FindQueryable(s => s.ParentId > 0).OrderBy(q => q.Id);
            var empCodes = financialAccountRepositoryQuery.FindSelectorQueryable<int>(AllAccounts, q => q.ParentId.Value);
            var List = empCodes.ToList();
            var parentDatas = financialAccountRepositoryQuery.FindAll(q => !List.Contains(q.Id)).ToList().OrderBy(q => q.ParentId);
            var list = new List<FinancialAccountDropDownReceipts>();

            if (parentDatas != null)
            {
                foreach (var item in parentDatas)
                {
                    var Dto = new FinancialAccountDropDownReceipts();
                    Dto.Id = item.Id;
                    Dto.ArabicName = item.ArabicName;
                    Dto.LatinName = item.LatinName;
                    Dto.Code = item.AccountCode;
                    Dto.AutoCode = item.AccountCode.Replace(".", String.Empty);
                    list.Add(Dto);
                }
            }

            return repositoryActionResult.GetRepositoryActionResult(list, RepositoryActionStatus.Ok, message: "Ok");


        }
        public async Task<IRepositoryActionResult> MoveFinancialAccount(MoveFinancial parameter)
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
        public async Task<ResponseResult> GetFinancialAccountDropDown(DropDownRequestForGL request)
        {
            // By Alaa
            // جعفور طلب يوم 24/3/2022 ان الليست لاضافة حساب جديد لازم تكون رئيسية وفي القيود فرعيه

            var parentDatas = financialAccountRepositoryQuery.FindAll(a => (request.isMain == null ? true : a.IsMain == request.isMain)
                         && (request.SearchCriteria != null ? (a.ArabicName.Contains(request.SearchCriteria) ||
                            a.LatinName.Contains(request.SearchCriteria) || a.AccountCode.Replace(".", string.Empty).StartsWith(request.SearchCriteria)) : true)).OrderBy(q => q.autoCoding).ToList();
            int dataCount = parentDatas.Count();
            double MaxPageNumber = parentDatas.Count() / Convert.ToDouble(request.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);

            if (request.PageSize > 0 && request.PageNumber > 0)
            {
                parentDatas = parentDatas.ToList().Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            }


            var res = parentDatas.Select(a => new FinancialAccountDropDown
            {
                Id = a.Id,
                Code = a.AccountCode.Replace(".", string.Empty),
                ArabicName = a.ArabicName,
                LatinName = a.LatinName
            });
            return new ResponseResult() { Data = res, DataCount = dataCount, Note = countofFilter == request.PageNumber ? Actions.EndOfData : null, Result = Result.Success };

        }
        public async Task<IRepositoryActionResult> RecodingFinancialAccount()
        {
            var accounts = financialAccountRepositoryQuery.GetAll().Where(x => x.autoCoding != null).ToList();
            var setting = await generalSettingRepositoryQuery.GetByAsync(a => true);
            financialAccountsHelper.recodingAccounts(accounts, setting);
            var saved = await financialAccountRepositoryCommand.UpdateAsyn(accounts);


            return repositoryActionResult.GetRepositoryActionResult();
        }
        public async Task<ResponseResult> GetAccountInformation(int id)
        {
            if (id == null)
                return new ResponseResult()
                {
                    ErrorMessageAr = ErrorMessagesAr.DataRequired,
                    ErrorMessageEn = ErrorMessagesEn.DataRequired,
                };
            var account = financialAccountRepositoryQuery.TableNoTracking.Where(x => x.Id == id).Select(x => new
            {
                x.Id,
                accountCode = x.AccountCode.Replace(".", string.Empty),
                MainAccountNameAr = financialAccountRepositoryQuery.TableNoTracking.Where(x => x.Id == id).Select(x => x.ArabicName).FirstOrDefault(),
                CurrancyNameAr = currencyRepositoryQuery.TableNoTracking.Where(c => c.Id == x.CurrencyId).Select(x => x.ArabicName).FirstOrDefault(),
                x.FA_Nature,
                CostCenterNameAr = costCenterRepositoryQuery.TableNoTracking.Where(c => c.Id == x.HasCostCenter).Select(x => x.ArabicName).FirstOrDefault(),
                x.Notes,
                accountNameAr = x.ArabicName,
                x.FinalAccount,
                branchNameAr = branchRepositoryQuery.TableNoTracking.Where(c => c.Id == x.BranchId).Select(x => x.ArabicName).FirstOrDefault(),
                x.Status,
                x.IsMain
            });
            return new ResponseResult()
            {
                Data = account.Any() ? account.FirstOrDefault() : null,
                Result = Result.Success,
                Id = account.Any() ? account.FirstOrDefault().Id : null
            };
        }
    }
}