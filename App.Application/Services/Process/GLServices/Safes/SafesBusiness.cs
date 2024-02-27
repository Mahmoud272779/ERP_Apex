using App.Application.Basic_Process;
using App.Application.Services.Process.FinancialAccounts;
using Attendleave.Erp.Core.APIUtilities;
using Attendleave.Erp.ServiceLayer.Abstraction;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.Net.Http.Headers;

namespace App.Application.Services.Process.Safes
{
    public class SafesBusiness : BusinessBase<GLSafe>, ISafesBusiness
    {
        private readonly IRepositoryQuery<GLSafe> treasuryRepositoryQuery;
        private readonly IRepositoryCommand<GLSafe> treasuryRepositoryCommand;
        private readonly IRepositoryQuery<GLSafeHistory> treasuryHistoryRepositoryQuery;
        private readonly IRepositoryCommand<GLSafeHistory> treasuryHistoryRepositoryCommand;
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GlReciepts> supportRepositoryQuery;
        private readonly IRepositoryQuery<GLBranch> branchRepositoryQuery;
        private readonly IRepositoryQuery<GLGeneralSetting> _gLGeneralSettingQuery;
        private readonly IRepositoryQuery<GLFinancialSetting> financialSettingRepositoryQuery;
        private readonly IPagedList<AllTreasuryDto, AllTreasuryDto> pagedListTreasury;
        private readonly IFinancialAccountBusiness _financialAccountBusiness;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly iDefultDataRelation _iDefultDataRelation;
        private readonly iGLFinancialAccountRelation _iGLFinancialAccountRelation;
        private readonly IHttpContextAccessor httpContext;
        private readonly iUserInformation _userinformation;
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<GLIntegrationSettings> _GLIntegrationSettingsQuery;


        public SafesBusiness(
           IRepositoryQuery<GLSafe> TreasuryRepositoryQuery,
           IRepositoryCommand<GLSafe> TreasuryRepositoryCommand,
           IRepositoryQuery<GLSafeHistory> TreasuryHistoryRepositoryQuery,
           IRepositoryCommand<GLSafeHistory> TreasuryHistoryRepositoryCommand,
           IRepositoryQuery<GlReciepts> SupportRepositoryQuery,
           IRepositoryQuery<GLFinancialSetting> FinancialSettingRepositoryQuery,
           IRepositoryQuery<GLFinancialAccount> FinancialAccountRepositoryQuery,
           IRepositoryQuery<GLBranch> BranchRepositoryQuery,
           IRepositoryQuery<GLGeneralSetting> GLGeneralSettingQuery,
           IPagedList<AllTreasuryDto, AllTreasuryDto> PagedListTreasury,
           IFinancialAccountBusiness financialAccountBusiness,
           ISystemHistoryLogsService systemHistoryLogsService,
           iDefultDataRelation iDefultDataRelation,
           iGLFinancialAccountRelation iGLFinancialAccountRelation,
           IHttpContextAccessor HttpContext,
           iUserInformation Userinformation,
            IRepositoryActionResult repositoryActionResult, iUserInformation iUserInformation, IRepositoryQuery<GLIntegrationSettings> gLIntegrationSettingsQuery) : base(repositoryActionResult)
        {
            treasuryRepositoryQuery = TreasuryRepositoryQuery;
            treasuryRepositoryCommand = TreasuryRepositoryCommand;
            treasuryHistoryRepositoryQuery = TreasuryHistoryRepositoryQuery;
            treasuryHistoryRepositoryCommand = TreasuryHistoryRepositoryCommand;
            financialAccountRepositoryQuery = FinancialAccountRepositoryQuery;
            financialSettingRepositoryQuery = FinancialSettingRepositoryQuery;
            branchRepositoryQuery = BranchRepositoryQuery;
            _gLGeneralSettingQuery = GLGeneralSettingQuery;
            supportRepositoryQuery = SupportRepositoryQuery;
            pagedListTreasury = PagedListTreasury;
            _financialAccountBusiness = financialAccountBusiness;
            _systemHistoryLogsService = systemHistoryLogsService;
            _iDefultDataRelation = iDefultDataRelation;
            _iGLFinancialAccountRelation = iGLFinancialAccountRelation;
            httpContext = HttpContext;
            _userinformation = Userinformation;
            _iUserInformation = iUserInformation;
            _GLIntegrationSettingsQuery = gLIntegrationSettingsQuery;
        }
        public async Task<GLSafe> CheckIsValidNameAr(string NameAr)
        {
            var treasury
                = await treasuryRepositoryQuery.GetByAsync(
                   cust => cust.ArabicName == NameAr);
            return treasury;
        }
        public async Task<GLSafe> CheckIsValidNameEn(string NameEn)
        {
            var treasury
                = await treasuryRepositoryQuery.GetByAsync(
                   cust => cust.LatinName == NameEn);
            return treasury;
        }
        public async Task<bool> CheckIsValidNameArUpdate(string NameAr, int Id)
        {

            var treasury
                = await treasuryRepositoryQuery.GetByAsync(
                   cust => cust.ArabicName == NameAr && cust.Id == Id);
            return treasury == null ? false : true;

        }
        public async Task<bool> CheckIsValidNameEnUpdate(string NameEn, int Id)
        {

            var treasury
                = await treasuryRepositoryQuery.GetByAsync(
                   cust => cust.LatinName == NameEn && cust.Id == Id);
            return treasury == null ? false : true;

        }
        public async Task<string> AddAutomaticCode()
        {
            var code = treasuryRepositoryQuery.FindQueryable(q => q.BranchId > 0);
            if (code.Count() > 0)
            {
                var code2 = treasuryRepositoryQuery.FindQueryable(q => q.BranchId > 0).ToList().Last();
                int codee = (Convert.ToInt32(code2.Code));
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

        //public async Task<ResponseResult> GLRelation(bool isSafe,int FinancialAccountId, int branchId,string arabicName,string latinName)
        //{
        //    var glSettings = _gLGeneralSettingQuery.TableNoTracking.FirstOrDefault();
        //    var defultAcc = isSafe ? glSettings.DefultAccSafe : glSettings.DefultAccBank;
        //    if (defultAcc == 1)
        //    {
        //        if(FinancialAccountId <= 0)
        //            return new ResponseResult()
        //            {
        //                Note = Actions.IdIsRequired,
        //                Result = Result.Failed
        //            };
        //        var findAccount = await financialAccountRepositoryQuery.GetByIdAsync(FinancialAccountId);
        //        if(findAccount == null)
        //            return new ResponseResult()
        //            {
        //                Note = Actions.NotFound,
        //                Result = Result.NotFound
        //            };
        //        if(findAccount.IsMain)
        //            return new ResponseResult()
        //            {
        //                Note = "Financial Account can not be main",
        //                Result = Result.Failed
        //            };
        //        return new ResponseResult()
        //        {
        //            Result = Result.Success,
        //            Id = FinancialAccountId
        //        };
        //    }
        //    else if(defultAcc == 2)
        //    {
        //        int financailAccount = isSafe ? glSettings.FinancialAccountIdSafe : glSettings.FinancialAccountIdBank;
        //        int[] branchs = { branchId };
        //        int[] costCenters = { };
        //        var financaialAccount = await financialAccountRepositoryQuery.GetByIdAsync(financailAccount);
        //        var createFinancialAccount = await _financialAccountBusiness.AddFinancialAccount(new FinancialAccountParameter()
        //        {
        //            AccountCode = null,
        //            BranchesId = branchs,
        //            CostCenterId = costCenters,
        //            CurrencyId = financaialAccount.CurrencyId,
        //            FA_Nature = financaialAccount.FA_Nature,
        //            FinalAccount = financaialAccount.FinalAccount,
        //            IsMain = false,
        //            Notes = "",
        //            ParentId = financailAccount,
        //            Status = 1,
        //            ArabicName = arabicName,
        //            LatinName = latinName
        //        });
        //        return new ResponseResult()
        //        {
        //            Result = Result.Success,
        //            Id = (int)createFinancialAccount.Data
        //        };
        //    }
        //    else if(defultAcc == 3)
        //    {
        //        return new ResponseResult()
        //        {
        //            Result = Result.Success,
        //            Id = isSafe ? glSettings.FinancialAccountIdSafe : glSettings.FinancialAccountIdBank
        //        };
        //    }
        //    return new ResponseResult()
        //    {
        //        Result = Result.Failed
        //    };
        //}
        
        public async Task<ResponseResult> AddTreasury(TreasuryParameter parameter)
        {
            try
            {
                var checkBranch = await branchesHelper.CheckIsBranchExist(new int[] { parameter.BranchId }, branchRepositoryQuery);
                if (checkBranch != null)
                    return checkBranch;
                parameter.LatinName = Helpers.Helpers.IsNullString(parameter.LatinName);
                parameter.ArabicName = Helpers.Helpers.IsNullString(parameter.ArabicName);
                if (string.IsNullOrEmpty( parameter.LatinName))
                {
                    parameter.LatinName = parameter.ArabicName;
                }

                if (string.IsNullOrEmpty(parameter.ArabicName))
                {
                    return new ResponseResult { Result = Result.Failed, Note = Aliases.Actions.NameIsRequired };
                }
                if (parameter.Status < (int)Status.Active || parameter.Status > (int)Status.Inactive)
                {
                    return new ResponseResult { Result = Result.Failed, Note = Aliases.Actions.InvalidStatus };
                }

                var treasuryExist = await treasuryRepositoryQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName && !string.IsNullOrEmpty(parameter.ArabicName));
                if (treasuryExist != null)
                    return new ResponseResult { Result = Result.Exist, Note = Aliases.Actions.ArabicNameExist };
                treasuryExist = await treasuryRepositoryQuery.GetByAsync(a => a.LatinName == parameter.LatinName && !string.IsNullOrEmpty(parameter.LatinName));
                if (treasuryExist != null)
                    return new ResponseResult { Result = Result.Exist, Note = Aliases.Actions.EnglishNameExist };


                var table = Mapping.Mapper.Map<TreasuryParameter, GLSafe>(parameter);
                table.BrowserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();
              
                table.Code = treasuryRepositoryQuery.GetMaxCode(a => a.Code) + 1; //await AddAutomaticCode();
             
                
                var financialsetting = await financialSettingRepositoryQuery.GetByAsync(q => q.IsTreasuries == true && q.IsAssumption == true);
                if (financialsetting != null)
                {
                    table.FinancialAccountId = null;
                }
                int[] branch = { table.BranchId };
                var finanicalAccount = await _iGLFinancialAccountRelation.GLRelation(GLFinancialAccountRelation.safe, parameter.FinancialAccountId??0, branch, table.ArabicName, table.LatinName);
                if (finanicalAccount.Result != Result.Success)
                    return finanicalAccount;
                table.FinancialAccountId = finanicalAccount.Id;
                var saved = treasuryRepositoryCommand.Add(table);
                if (saved)
                {
                    await _iDefultDataRelation.AdministratorUserRelation(2,table.Id);
                }
                HistoryTreasury(table.BranchId, table.Status, table.ArabicName, table.LatinName, table.Notes, table.Id, table.FinancialAccountId,
                                          table.BrowserName, "A", "");
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addSafes);
                return new ResponseResult { Id = table.Id, Result = Result.Success, Note = "Saved Successfully" };
            }
            catch (Exception ex)
            {
                return new ResponseResult { Data = ex, Result = Result.Success, Note = Aliases.Actions.ExceptionOccurred };

            }
        }
        public async Task<ResponseResult> UpdateTreasury(UpdateTreasuryParameter parameter)
        {
            try
            {
                var checkBranch = await branchesHelper.CheckIsBranchExist(new int[] { parameter.BranchId }, branchRepositoryQuery);
                if (checkBranch != null)
                    return checkBranch;
                parameter.LatinName = Helpers.Helpers.IsNullString(parameter.LatinName);
                parameter.ArabicName = Helpers.Helpers.IsNullString(parameter.ArabicName);
                if (string.IsNullOrEmpty(parameter.LatinName))
                {
                    parameter.LatinName = parameter.ArabicName;
                }

                if (string.IsNullOrEmpty(parameter.ArabicName))
                {
                    return new ResponseResult { Result = Result.Failed, Note = Actions.NameIsRequired };
                }
                if (parameter.Status < (int)Status.Active || parameter.Status > (int)Status.Inactive)
                {
                    return new ResponseResult { Result = Result.Failed, Note =  Actions.InvalidStatus };
                }
                var treasuryExist = await treasuryRepositoryQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName && !string.IsNullOrEmpty(parameter.ArabicName) && a.Id != parameter.Id);
                if (treasuryExist != null)
                    return new ResponseResult { Result = Result.Exist, Note =  Actions.ArabicNameExist };
                

                treasuryExist = await treasuryRepositoryQuery.GetByAsync(a => a.LatinName == parameter.LatinName && !string.IsNullOrEmpty(parameter.LatinName) && a.Id != parameter.Id);
                if (treasuryExist != null)
                    return new ResponseResult { Result = Result.Exist, Note =  Actions.EnglishNameExist };
                var glSettings = _gLGeneralSettingQuery.TableNoTracking.FirstOrDefault();

                //var updateData = await  treasuryRepositoryQuery.GetByAsync(q => q.Id == parameter.Id);
                var updateData = treasuryRepositoryQuery.TableNoTracking.Include(a => a.financialAccount).Where(a => a.Id == parameter.Id).FirstOrDefault();
                var rec = supportRepositoryQuery.TableNoTracking.Where(x => x.SafeID == parameter.Id).Any();
                if (rec) { parameter.BranchId = updateData.BranchId; }
                //if (glSettings.DefultAccSafe != 1)
                //{
                //    parameter.FinancialAccountId = updateData.FinancialAccountId;
                //}
                var table = Mapping.Mapper.Map<UpdateTreasuryParameter, GLSafe>(parameter, updateData);
                table.BrowserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();

                var userInfo = await _iUserInformation.GetUserInformation();
                var linkingMethod = _GLIntegrationSettingsQuery
                .TableNoTracking
                       .Where(c => c.GLBranchId == userInfo.CurrentbranchId && c.screenId == (int) SubFormsIds.Safes_MainData)
                       .FirstOrDefault().linkingMethodId;


                if (linkingMethod == 1 && table.FinancialAccountId != parameter.FinancialAccountId)
                    table.FinancialAccountId = parameter.FinancialAccountId;

                else
                    table.FinancialAccountId = table.financialAccount.Id;

                var financialsetting = await financialSettingRepositoryQuery.GetByAsync(q => q.IsTreasuries == true && q.IsAssumption == true);
                if (financialsetting != null)
                {
                    table.FinancialAccountId = null;
                }
                //if(glSettings.DefultAccSafe == 1 && parameter.FinancialAccountId != updateData.FinancialAccountId)
                //{
                //    var finanicalAccount = await _iGLFinancialAccountRelation.GLRelation(GLFinancialAccountRelation.safe, parameter.FinancialAccountId ?? 0, table.BranchId.ToString().Select(x=> (int)x).ToArray(), table.ArabicName, table.LatinName);
                //    if (finanicalAccount.Result != Result.Success)
                //        return finanicalAccount;
                //    table.FinancialAccountId = finanicalAccount.Id;
                //}
                await treasuryRepositoryCommand.UpdateAsyn(table);

                HistoryTreasury(table.BranchId, table.Status, table.ArabicName, table.LatinName, table.Notes, table.Id, table.FinancialAccountId.Value,
                              table.BrowserName, "U", null);
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editSafes);
                return new ResponseResult { Id = table.Id, Result = Result.Success, Note = Actions.UpdatedSuccessfully };

            }
            catch (Exception ex)
            {
                return new ResponseResult { Data = ex };

            }
        }
        public async Task<ResponseResult> GetTreasuryById(int Id)
        {
            try
            {
                var retSafe = (await treasuryRepositoryQuery.GetAllIncludingAsync(0, 0,
                                                            a => a.Id == Id,
                                                           w => w.Branch, w => w.financialAccount)).FirstOrDefault();
                // var retSafe = safe.FirstOrDefault();
                if (retSafe != null)
                {

                    return new ResponseResult { Data = retSafe, Id = Id, Result = Result.Success, Note = Aliases.Actions.Success };
                }
                return new ResponseResult { Data = null, Result = Result.NotFound, Note = Aliases.Actions.NotFound };
            }
            catch (Exception ex)
            {
                return new ResponseResult { Data = ex, Note = Aliases.Actions.ExceptionOccurred };
            }
            #region Old code
            //try
            //{
            //    var treeData = await treasuryRepositoryQuery.GetAsync(Id);
            //    return new ResponseResult { Data = treeData, Result = Result.Success, Note = "OK" };
            //}
            //catch (Exception ex)
            //{
            //    return new ResponseResult { Data = ex };
            //} 
            #endregion
        }
        public async Task<ResponseResult> DeleteTreasuryAsync(SharedRequestDTOs.Delete parameter)
        {
            try
            {
                UserInformationModel userInfo = await _userinformation.GetUserInformation();
                if (userInfo == null)
                    return new ResponseResult()
                    {
                        Note = Actions.JWTError,
                        Result = Result.Failed
                    };
                var test = await treasuryRepositoryQuery.GetAllIncludingAsync(0, 0,
                                                            a => parameter.Ids.Contains(a.Id),
                                                            w => w.FundsBanksSafes, w => w.PaymentMethod, w => w.reciept);
                var elementsCanBeDeleted = test.Where(x => x.FundsBanksSafes.Count() == 0 && x.PaymentMethod.Count() == 0 && x.reciept.Count() == 0);
                int count = elementsCanBeDeleted.Count();
                List<int> deletedItems = new List<int>();

                
                treasuryRepositoryCommand.RemoveRange(elementsCanBeDeleted);
                var result = await treasuryRepositoryCommand.SaveAsync();
                if (result)
                {
                    deletedItems.AddRange(elementsCanBeDeleted.Select(x=> x.Id));
                }
                
                var FA_Deleted = await _financialAccountBusiness.DeleteFinancialAccountAsync(new SharedRequestDTOs.Delete()
                {
                    Ids = elementsCanBeDeleted.Select(x => x.FinancialAccountId ?? 0).ToArray(),
                    userId = userInfo.userId
                });
                if(deletedItems.Any())
                    await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteSafes,count);

                return new ResponseResult() { Data = deletedItems, Result = deletedItems.Any() ? Result.Success : Result.Failed };

            }
            catch (Exception ex)
            {

                return new ResponseResult() { Data = ex, Result = Result.Failed, Note = Aliases.Actions.ExceptionOccurred };
            }
            #region Old code
            //try
            //{
            //    var lis = new List<ListOfTreasury>();
            //    foreach (var item2 in parameter.Ids)
            //    {
            //        var reciept = await supportRepositoryQuery.GetByAsync(q => q.TreasuryId == item2);
            //        if (reciept != null)
            //        {
            //            var bra = await treasuryRepositoryQuery.GetByAsync(q => q.Id == reciept.TreasuryId);
            //            var list = new ListOfTreasury();
            //            list.Name = bra?.ArabicName;
            //            lis.Add(list);
            //        }
            //    }
            //    if (lis.Count() < parameter.Ids.Count())
            //    {
            //        foreach (var item in parameter.Ids)
            //        {
            //            var BranchDeleted = await treasuryRepositoryQuery.GetByAsync(q => q.Id == item);
            //            var support = await supportRepositoryQuery.GetByAsync(q => q.TreasuryId == item);

            //            if (support == null)
            //            {
            //                treasuryRepositoryCommand.Remove(BranchDeleted);
            //            }
            //        }
            //        await treasuryRepositoryCommand.SaveAsync();
            //        if (lis.Count() == 0)
            //        {
            //            return new ResponseResult { Result = Result.Success, Note = "Deleted Successfully" };
            //        }
            //        else
            //        {
            //            return new ResponseResult { Data = lis, Result = Result.Failed, Note = "Can not be deleted " };

            //        }
            //    }
            //    else
            //    {
            //        return new ResponseResult { Data = lis, Result = Result.Failed, Note = "Can not be deleted " };


            //    }
            //    return new ResponseResult { Result = Result.Success, Note = "Deleted Successfully" };

            //}
            //catch (Exception ex)
            //{
            //    return new ResponseResult { Data = ex };
            //} 
            #endregion
        }
        public async Task<ResponseResult> GetAllTreasuryData(PageTreasuryParameter parameters)
        {

            try
            {
                


                var userinfo = await _userinformation.GetUserInformation();

                var allFinanialAccounts = financialAccountRepositoryQuery.TableNoTracking;
                var allBranches = branchRepositoryQuery.TableNoTracking;
                var dta = treasuryRepositoryQuery.TableNoTracking
                                                        //.Include(x => x.reciept)
                                                        .Include(x => x.PaymentMethod)
                                                        .Include(x => x.FundsBanksSafes)
                                                        .Include(x => x.Branch)
                                                        .Where(x=> userinfo.employeeBranches.Contains(x.BranchId));
                var resData = dta.Where(x => true);
                if (!string.IsNullOrEmpty(parameters.Name))
                    resData = resData.Where(a => (a.Code.ToString().Contains(parameters.Name) || string.IsNullOrEmpty(parameters.Name)
                                                           || a.ArabicName.Contains(parameters.Name) || a.LatinName.Contains(parameters.Name)));
                if (parameters.Status == 1)
                    resData = resData.Where(x => x.Status == 1);
                else if (parameters.Status == 2)
                    resData = resData.Where(x => x.Status == 2);

                if (string.IsNullOrEmpty(parameters.Name))
                    resData = resData.OrderByDescending(q => q.Code);
                else
                    resData = resData.OrderBy(q => q.Code);
                var count = resData.Count();
                if (parameters.PageSize > 0 && parameters.PageNumber > 0)
                {
                    resData = resData.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize);
                }
                else
                {
                    return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };

                }
                var listOfData = new List<GLSafe>();
                var reciepts = supportRepositoryQuery.TableNoTracking.Where(x=> resData.Select(c=> c.Id).Contains(x.SafeID??0));

                foreach (var item in resData)
                {
                    item.CanDelete = reciepts.Where(x=> x.SafeID == item.Id).Any() || item.PaymentMethod.Any() || item.FundsBanksSafes.Any() || item.Id == 1 ? false : true;
                    listOfData.Add(item);
                }
                var response = listOfData.Select(x => new
                {
                    x.ArabicName,
                    x.BranchId,
                    branchNameAr = allBranches.Where(c=> c.Id == x.BranchId).Select(c=> c.ArabicName).FirstOrDefault(),
                    branchNameEn = allBranches.Where(c=> c.Id == x.BranchId).Select(c=> c.LatinName).FirstOrDefault(),
                    x.BrowserName,
                    canDelete = x.CanDelete,
                    x.Code,
                    FinancialAccountId = allFinanialAccounts.Where(c=> c.Id == x.FinancialAccountId).Select(c=> new {c.Id,c.ArabicName,c.LatinName}).FirstOrDefault(),
                    fundsBanksSafes = x.FundsBanksSafes,
                    x.Id,
                    x.LatinName,
                    x.Notes,
                    x.OtherSettingsSafes,
                    x.PaymentMethod,
                    x.reciept,
                    x.Status
                }).ToList();
                return new ResponseResult() { Data = response, DataCount = count, Id = null, Result = resData.Any() ? Result.Success : Result.Failed };

            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex, Result = Result.Failed, Note = Aliases.Actions.ExceptionOccurred };
            }
            #region OldCode
            //try
            //{
            //    var searchCretiera = paramters.SearchCriteria;
            //    var treeData = treasuryRepositoryQuery
            //      .FindQueryable(x =>
            //      (searchCretiera == string.Empty ||
            //            searchCretiera == null || x.Code.ToString().Contains(paramters.SearchCriteria) ||
            //            x.ArabicName.ToLower().Contains(searchCretiera) ||
            //            x.LatinName.ToLower().Contains(searchCretiera) ||
            //            x.Branch.ArabicName.ToLower().Contains(searchCretiera) ||
            //            x.Branch.LatinName.ToLower().Contains(searchCretiera))
            //     );
            //    if (paramters.branchSearches != null)
            //    {
            //        if (paramters.branchSearches.Status != 0)
            //            treeData = treeData.Where(q => q.Status == paramters.branchSearches.Status);
            //    }
            //    treeData = treeData.Include(q => q.financialAccount).Include(s => s.Branch);

            //    var treeDataList = (string.IsNullOrEmpty(paramters.SearchCriteria) ? treeData.OrderByDescending(q => q.Code) : treeData.OrderBy(a => (a.Code.ToString().Contains(paramters.SearchCriteria)) ? 0 : 1)).ToList();
            //    int count = treeData.Count();
            //    if (paramters.PageSize > 0 && paramters.PageNumber > 0)
            //    {
            //        treeDataList = treeDataList.Skip((paramters.PageNumber - 1) * paramters.PageSize).Take(paramters.PageSize).ToList();
            //    }
            //    else
            //    {
            //        return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };

            //    }
            //    //  var treeDataList = treeData.ToList();
            //    var list = Mapping.Mapper.Map<List<GLSafe>, List<AllTreasuryDto>>(treeData.OrderByDescending(e => e.Id).ToList());

            //    //   var resultData = pagedListTreasury.GetGenericPagination(list, paramters.PageNumber, paramters.PageSize, Mapper);
            //    return new ResponseResult { Data = treeDataList, Result = treeData.Count() > 0 ? Result.Success : Result.NoDataFound, DataCount = count, Note = Aliases.Actions.Success };
            //}
            //catch (Exception ex)
            //{
            //    return new ResponseResult { Data = ex };
            //} 
            #endregion
        }

        public async Task<ResponseResult> GetAllTreasuryDataDropDown()
        {
            try
            {
                var userinfo = await _userinformation.GetUserInformation();
                var treeData = treasuryRepositoryQuery.GetAll().Where(x=> x.BranchId == userinfo.CurrentbranchId &&  userinfo.userSafes.Contains(x.Id)).ToList();

                var list = Mapping.Mapper.Map<List<GLSafe>, List<TreasuryDto>>(treeData.ToList());

                return new ResponseResult { Data = list, Result = Result.Success, Note = "OK" };

            }
            catch (Exception ex)
            {
                return new ResponseResult { Data = ex };
            }
        }

        public async Task<ResponseResult> GetAllTreasuryByUserDropDown()
        {
            try
            {
                var userinfo = await _userinformation.GetUserInformation();
                var treeData = treasuryRepositoryQuery.GetAll().Where(x => userinfo.userSafes.Contains(x.Id)).ToList();

                var list = Mapping.Mapper.Map<List<GLSafe>, List<TreasuryDto>>(treeData.ToList());

                return new ResponseResult { Data = list, Result = Result.Success, Note = "OK" };

            }
            catch (Exception ex)
            {
                return new ResponseResult { Data = ex };
            }
        }
        public async Task<ResponseResult> GetAllTreasuryWithoutFilterDropDown()
        {
            try
            {
                var treeData = treasuryRepositoryQuery.GetAll().ToList();

                var list = Mapping.Mapper.Map<List<GLSafe>, List<TreasuryDto>>(treeData.ToList());

                return new ResponseResult { Data = list, Result = Result.Success, Note = "OK" };

            }
            catch (Exception ex)
            {
                return new ResponseResult { Data = ex };
            }
        }
        public async Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameter)
        {
            try
            {
                foreach (var item in parameter.Id)
                {
                    var treasury = await treasuryRepositoryQuery.GetByAsync(q => q.Id == item);
                    if (treasury.Id == 1)
                    {
                        treasury.Status = (int)Status.Active;
                    }
                    else
                    {
                        treasury.Status = parameter.Status;
                    }
                    await treasuryRepositoryCommand.UpdateAsyn(treasury);

                }
                return new ResponseResult { Result = Result.Success, Note = "Updated Successfully" };
            }
            catch (Exception ex)
            {
                return new ResponseResult { Data = ex };
            }
        }
        public async void HistoryTreasury(int branchId, int status, string nameAr, string nameEn, string notes, int treasuryId, int? FinancialAccountId,
                                          string browserName, string lastTransactionAction, string LastTransactionUser)
        {
            var userInfo = await _iUserInformation.GetUserInformation();

            var history = new GLSafeHistory()
            {
                employeesId=userInfo.employeeId,
                LastDate = DateTime.Now,
                LastAction = lastTransactionAction,
                BranchId = branchId,
                FinancialAccountId = FinancialAccountId,
                LastTransactionUser = userInfo.employeeNameEn.ToString(),
                LastTransactionUserAr= userInfo.employeeNameAr.ToString(),
                TreasuryId = treasuryId,
                Status = status,
                ArabicName = nameAr,
                LatinName = nameEn,
                Notes = notes,
                BrowserName = browserName,
            };
            treasuryHistoryRepositoryCommand.Add(history);
        }
        public async Task<ResponseResult> GetAllTreasuryHistory(int TreasuryId)
        {
            var parentDatasQuey = treasuryHistoryRepositoryQuery.FindQueryable(s => s.TreasuryId == TreasuryId).Include(a=>a.employees);
            var historyList = new List<HistoryResponceDto>();
            foreach (var item in parentDatasQuey.ToList())
            {
                var historyDto = new HistoryResponceDto();
                HistoryActionsNames actionName = HistoryActionsAliasNames.HistoryName[item.LastAction];
                historyDto.TransactionTypeEn = actionName.LatinName;
                historyDto.TransactionTypeAr = actionName.ArabicName;
                historyDto.LatinName = item.employees.LatinName;
                historyDto.ArabicName = item.employees.ArabicName;
                historyDto.Date = item.LastDate;
                if (item.LastAction == "U")
                {
                    historyDto.TransactionTypeAr = "تعديل";
                }
                if (item.LastAction == "A")
                {
                    historyDto.TransactionTypeAr = "اضافة";
                }
              //  historyDto.userNameAr = item.LastTransactionUser;

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
            return new ResponseResult { Data = historyList, Result = Result.Success, Note = "OK" };
        }
        public async Task<ResponseResult> GetAllTreasuryDataSetting()
        {
            try
            {
                var treeData = treasuryRepositoryQuery.TableNoTracking.Include(q => q.financialAccount).ToList();
                var list = Mapping.Mapper.Map<List<GLSafe>, List<TreasurySettingDto>>(treeData.ToList());

                return new ResponseResult { Data = list, Result = Result.Success, Note = "OK" };
            }
            catch (Exception ex)
            {
                return new ResponseResult { Data = ex };
            }
        }


    }
}
