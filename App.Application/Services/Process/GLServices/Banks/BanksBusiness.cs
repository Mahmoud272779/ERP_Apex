using App.Application.Basic_Process;
using App.Application.Helpers;
using App.Application.Services.HelperService;
using App.Application.Services.Process.FinancialAccounts;
using App.Application.Services.Process.GeneralServices.SystemHistoryLogsServices;
using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using Attendleave.Erp.Core.APIUtilities;
using Attendleave.Erp.ServiceLayer.Abstraction;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Bank
{
    public class BanksBusiness : BusinessBase<GLBank>, iBanksService
    {
        private readonly IRepositoryCommand<GLBank> banksRepositoryCommand;
        private readonly IRepositoryCommand<GLBanksHistory> banksHistoryRepositoryCommand;
        private readonly IRepositoryCommand<GLBankBranch> bankBranchRepositoryCommand;

        private readonly IRepositoryQuery<GLBank> banksRepositoryQuery;
        private readonly IRepositoryQuery<GLFinancialAccount> _FinancialAccountQuery;
        private readonly IRepositoryQuery<GLBanksHistory> banksHistoryRepositoryQuery;
        private readonly IRepositoryQuery<GlReciepts> recieptsRepositoryQuery;
        private readonly IRepositoryQuery<GLBankBranch> bankBranchRepositoryQuery;
        private readonly IRepositoryQuery<GLFinancialSetting> financialSettingRepositoryQuery;
        private readonly IRepositoryQuery<GLGeneralSetting> _gLGeneralSettingQuery;
        private readonly iGLFinancialAccountRelation _iGLFinancialAccountRelation;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IFinancialAccountBusiness _financialAccountBusiness;
        private readonly IRepositoryQuery<GLBranch> branchRepositoryQuery;

        private readonly IHttpContextAccessor httpContext;
        private readonly iUserInformation _userinformation;
        private readonly iDefultDataRelation _iDefultDataRelation;
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<GLIntegrationSettings> _GLIntegrationSettingsQuery;

        public BanksBusiness(
           IRepositoryQuery<GLBank> BanksRepositoryQuery,
           IRepositoryQuery<GLFinancialAccount> FinancialAccountQuery,
           IRepositoryCommand<GLBank> BanksRepositoryCommand,
           IRepositoryQuery<GLBanksHistory> BanksHistoryRepositoryQuery,
           IRepositoryQuery<GLBankBranch> BankBranchRepositoryQuery,
           IRepositoryCommand<GLBanksHistory> BanksHistoryRepositoryCommand,
           IRepositoryQuery<GlReciepts> _recieptsRepositoryQuery,
           IRepositoryCommand<GLBankBranch> BankBranchRepositoryCommand,
           IRepositoryQuery<GLFinancialSetting> FinancialSettingRepositoryQuery,
           IRepositoryQuery<GLGeneralSetting> GLGeneralSettingQuery,
           iGLFinancialAccountRelation iGLFinancialAccountRelation,
           ISystemHistoryLogsService systemHistoryLogsService,
            IFinancialAccountBusiness financialAccountBusiness,
           IHttpContextAccessor HttpContext,
           iUserInformation Userinformation,
           iDefultDataRelation iDefultDataRelation,
           IRepositoryQuery<GLBranch> BranchRepositoryQuery,
           IRepositoryActionResult repositoryActionResult, iUserInformation iUserInformation, IRepositoryQuery<GLIntegrationSettings> gLIntegrationSettingsQuery) : base(repositoryActionResult)
        {
            banksRepositoryQuery = BanksRepositoryQuery;
            _FinancialAccountQuery = FinancialAccountQuery;
            banksRepositoryCommand = BanksRepositoryCommand;
            banksHistoryRepositoryQuery = BanksHistoryRepositoryQuery;
            banksHistoryRepositoryCommand = BanksHistoryRepositoryCommand;
            bankBranchRepositoryCommand = BankBranchRepositoryCommand;
            bankBranchRepositoryQuery = BankBranchRepositoryQuery;
            recieptsRepositoryQuery = _recieptsRepositoryQuery;
            financialSettingRepositoryQuery = FinancialSettingRepositoryQuery;
            _gLGeneralSettingQuery = GLGeneralSettingQuery;
            _iGLFinancialAccountRelation = iGLFinancialAccountRelation;
            _systemHistoryLogsService = systemHistoryLogsService;
            _financialAccountBusiness = financialAccountBusiness;
            branchRepositoryQuery = BranchRepositoryQuery;
            httpContext = HttpContext;
            _userinformation = Userinformation;
            _iDefultDataRelation = iDefultDataRelation;
            _iUserInformation = Userinformation;
            _GLIntegrationSettingsQuery = gLIntegrationSettingsQuery;
        }
        public async Task<GLBank> CheckIsValidNameAr(string NameAr)
        {
            var bank
                = await banksRepositoryQuery.GetByAsync(
                   cust => cust.ArabicName == NameAr);
            return bank;
        }
        public async Task<GLBank> CheckIsValidNameEn(string NameEn)
        {
            var bank
                = await banksRepositoryQuery.GetByAsync(
                   cust => cust.LatinName == NameEn);
            return bank;
        }
        public async Task<bool> CheckIsValidNameArUpdate(string NameAr, int Id)
        {

            var bank
                = await banksRepositoryQuery.GetByAsync(
                   cust => cust.ArabicName == NameAr && cust.Id == Id);
            return bank == null ? false : true;

        }
        public async Task<bool> CheckIsValidNameEnUpdate(string NameEn, int Id)
        {

            var bank
                = await banksRepositoryQuery.GetByAsync(
                   cust => cust.LatinName == NameEn && cust.Id == Id);
            return bank == null ? false : true;

        }
        public async Task<int> AddAutomaticCode()
        {
            var code = banksRepositoryQuery.FindQueryable(q => q.Id > 0);
            if (code.Count() > 0)
            {
                var code2 = banksRepositoryQuery.FindQueryable(q => q.Id > 0).ToList().Last();
                int codee = (Convert.ToInt32(code2.Code));
                if (codee == 0)
                {
                }
                var NewCode = codee + 1;
                return NewCode;
            }
            else
            {
                var NewCode = 1;
                return NewCode;

            }
        }
        public async Task<ResponseResult> AddBanks(BankRequestsDTOs.Add parameter)
        {
            try
            {
                int[] _branches = { 1 };
                parameter.BranchesId = _branches;//remove this code when you need to control branches in banks

                var checkBranch = await branchesHelper.CheckIsBranchExist(parameter.BranchesId, branchRepositoryQuery);
                if (checkBranch != null)
                    return checkBranch;
                parameter.ArabicName = Helpers.Helpers.IsNullString(parameter.ArabicName);
                parameter.LatinName = Helpers.Helpers.IsNullString(parameter.LatinName);
                if (string.IsNullOrEmpty(parameter.LatinName))
                {
                    parameter.LatinName = parameter.ArabicName;
                }
                if (string.IsNullOrEmpty(parameter.ArabicName.Trim()))
                    return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Aliases.Actions.NameIsRequired };

                if (parameter.Status < (int)Status.Active || parameter.Status > (int)Status.Inactive)
                {
                    return new ResponseResult { Result = Result.Failed, Note = Aliases.Actions.InvalidStatus };
                }
                var BankExist = await banksRepositoryQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName && !string.IsNullOrEmpty(parameter.ArabicName));
                if (BankExist != null)
                    return new ResponseResult { Result = Result.Exist, Note = Aliases.Actions.ArabicNameExist };
                BankExist = await banksRepositoryQuery.GetByAsync(a => a.LatinName == parameter.LatinName && !string.IsNullOrEmpty(parameter.LatinName));
                if (BankExist != null)
                    return new ResponseResult { Result = Result.Exist, Note = Aliases.Actions.EnglishNameExist };

                var PhoneExist = await banksRepositoryQuery.GetByAsync(a => a.Phone == parameter.Phone && !string.IsNullOrEmpty(parameter.Phone));
                if (PhoneExist != null)
                    return new ResponseResult() { Data = null, Id = PhoneExist.Id, Result = Result.Exist, Note = Actions.PhoneExist };

                var EmailExist = await banksRepositoryQuery.GetByAsync(a => a.Email == parameter.Email && !string.IsNullOrEmpty(parameter.Email));
                if (EmailExist != null)
                    return new ResponseResult() { Data = null, Id = EmailExist.Id, Result = Result.Exist, Note = Actions.EmailExist };
                var branches = branchRepositoryQuery.TableNoTracking.Select(c => c.Id).ToArray();
                parameter.BranchesId = branches;
                var table = new GLBank();
                table.BrowserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();

                table.ArabicAddress = parameter.AddressAr;
                table.LatinAddress = parameter.AddressEn;
                table.LatinName = parameter.LatinName.Trim();
                table.ArabicName = parameter.ArabicName.Trim();
                table.Code = banksRepositoryQuery.GetMaxCode(a => a.Code) + 1;// await AddAutomaticCode();
                table.AccountNumber = parameter.AccountNumber;
                table.Email = parameter.Email;
                table.FinancialAccountId = parameter.FinancialAccountId;
                table.Notes = parameter.Notes;
                table.Phone = parameter.Phone;
                table.Status = parameter.Status;
                table.Website = parameter.Website;
                //table.BranchId = parameter.BranchesId.ToList().First();

                var financialsetting = await financialSettingRepositoryQuery.GetByAsync(q => q.IsBanks == true && q.IsAssumption == true);
                if (financialsetting != null)
                {
                    table.FinancialAccountId = null;
                }
                
                var finanicalAccount = await _iGLFinancialAccountRelation.GLRelation(GLFinancialAccountRelation.bank, parameter.FinancialAccountId ?? 0, parameter.BranchesId, table.ArabicName, table.LatinName);
                if (finanicalAccount.Result != Result.Success)
                    return finanicalAccount;
                table.FinancialAccountId = finanicalAccount.Id;
                var saved = banksRepositoryCommand.Add(table);
                if (parameter.BranchesId.Count() > 0)
                {
                    foreach (var item in parameter.BranchesId)
                    {
                        var bankBranch = new GLBankBranch();
                        bankBranch.BranchId = item;
                        bankBranch.BankId = table.Id;
                        bankBranchRepositoryCommand.AddWithoutSaveChanges(bankBranch);
                    }
                    await bankBranchRepositoryCommand.SaveAsync();

                }
                if (saved)
                {
                    await _iDefultDataRelation.AdministratorUserRelation(1, table.Id);
                }
                HistoryBanks(table.Id, table.Status, table.ArabicName, table.LatinName, table.Notes, table.Id, table.FinancialAccountId,
                                     table.BrowserName, "A", "", table.ArabicAddress, table.LatinAddress, table.Email, table.Phone, table.AccountNumber);
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addBanks);
                return new ResponseResult { Id = table.Id, Result = Result.Success, Note = Aliases.Actions.SavedSuccessfully };
            }
            catch (Exception ex)
            {
                return new ResponseResult { Data = ex, Result = Result.Failed, Note = Aliases.Actions.SavedSuccessfully };

            }
        }
        public async Task<ResponseResult> UpdateBanks(BankRequestsDTOs.Update parameter)
        {
            try
            {
                int[] _branches = { 1 };
                parameter.BranchesId = _branches;//remove this code when you need to control branches in banks
                var checkBranch = await branchesHelper.CheckIsBranchExist(parameter.BranchesId, branchRepositoryQuery);
                if (checkBranch != null)
                    return checkBranch;
                parameter.ArabicName = Helpers.Helpers.IsNullString(parameter.ArabicName);
                parameter.LatinName = Helpers.Helpers.IsNullString(parameter.LatinName);
                if (string.IsNullOrEmpty(parameter.LatinName))
                {
                    parameter.LatinName = parameter.ArabicName;
                }
                if (string.IsNullOrEmpty(parameter.ArabicName.Trim()))
                    return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Aliases.Actions.NameIsRequired };

                if (parameter.Status < (int)Status.Active || parameter.Status > (int)Status.Inactive)
                {
                    return new ResponseResult { Result = Result.Failed, Note = Aliases.Actions.InvalidStatus };
                }
                var BankExist = await banksRepositoryQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName && !string.IsNullOrEmpty(parameter.ArabicName) && a.Id != parameter.Id);
                if (BankExist != null)
                    return new ResponseResult { Result = Result.Exist, Note = Aliases.Actions.ArabicNameExist };
                BankExist = await banksRepositoryQuery.GetByAsync(a => a.LatinName == parameter.LatinName && !string.IsNullOrEmpty(parameter.LatinName) && a.Id != parameter.Id);
                if (BankExist != null)
                    return new ResponseResult { Result = Result.Exist, Note = Aliases.Actions.EnglishNameExist };

                var PhoneExist = await banksRepositoryQuery.GetByAsync(a => a.Phone == parameter.Phone && !string.IsNullOrEmpty(parameter.Phone) && a.Id != parameter.Id);
                if (PhoneExist != null)
                    return new ResponseResult() { Data = null, Id = PhoneExist.Id, Result = Result.Exist, Note = Actions.PhoneExist };
                var EmailExist = await banksRepositoryQuery.GetByAsync(a => a.Email == parameter.Email && !string.IsNullOrEmpty(parameter.Email) && a.Id != parameter.Id);
                if (EmailExist != null)
                    return new ResponseResult() { Data = null, Id = EmailExist.Id, Result = Result.Exist, Note = Actions.EmailExist };
                var branches = branchRepositoryQuery.TableNoTracking.Select(c => c.Id).ToArray();
                parameter.BranchesId = branches;
                var gLSettigs = _gLGeneralSettingQuery.TableNoTracking.FirstOrDefault();
                var table = await banksRepositoryQuery.GetByAsync(q => q.Id == parameter.Id);
                table.ArabicAddress = parameter.AddressAr;
                table.LatinAddress = parameter.AddressEn;
                table.AccountNumber = parameter.AccountNumber;
                table.Email = parameter.Email;
                table.Code = table.Code;
                if (gLSettigs.DefultAccSafe == 1)
                {
                    table.FinancialAccountId = parameter.FinancialAccountId;
                }
                table.Notes = parameter.Notes;
                table.Phone = parameter.Phone;
                table.Status = parameter.Status;
                table.Website = parameter.Website;
                table.BrowserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();
                table.LatinName = parameter.LatinName.Trim();
                table.ArabicName = parameter.ArabicName.Trim();
                if (table.LatinName == "" || table.LatinName == null)
                {
                    table.LatinName = table.ArabicName;
                }

                var financialsetting = await financialSettingRepositoryQuery.GetByAsync(q => q.IsBanks == true && q.IsAssumption == true);
                if (financialsetting != null)
                {
                    table.FinancialAccountId = null;
                }
                var userInfo = await _iUserInformation.GetUserInformation();
                var linkingMethod = _GLIntegrationSettingsQuery
                .TableNoTracking
                       .Where(c => c.GLBranchId == userInfo.CurrentbranchId && c.screenId == (int)SubFormsIds.Banks_MainData)
                       .FirstOrDefault().linkingMethodId;
                if (linkingMethod == 1 && table.FinancialAccountId != parameter.FinancialAccountId)
                    table.FinancialAccountId = parameter.FinancialAccountId;

                await banksRepositoryCommand.UpdateAsyn(table);
                var bankBranch = bankBranchRepositoryQuery.FindAll(q => q.BankId == table.Id);
                var branchList = new List<GLBankBranch>();
                var branchListCanNotBeDeleted = new List<GLBankBranch>();
                var recs = recieptsRepositoryQuery.TableNoTracking.Where(x => x.BankId == table.Id);
                foreach (var item in bankBranch)
                {
                    var checkBranchUsed = recs.Where(x => x.BranchId == item.BranchId).Any();
                    if (!checkBranchUsed)
                        branchList.Add(item);
                    if (checkBranchUsed)
                        branchListCanNotBeDeleted.Add(item);

                }
                bankBranchRepositoryCommand.RemoveRange(branchList);
                var branchesList = parameter.BranchesId.Where(x => !branchListCanNotBeDeleted.Select(c=> c.BranchId).ToArray().Contains(x));
                foreach (var item in branchesList)
                {
                    var bankBranche = new GLBankBranch();
                    bankBranche.BranchId = item;
                    bankBranche.BankId = table.Id;
                    bankBranchRepositoryCommand.AddWithoutSaveChanges(bankBranche);
                }
                await bankBranchRepositoryCommand.SaveAsync();
                HistoryBanks(table.Id, table.Status, table.ArabicName, table.LatinName, table.Notes, table.Id, table.FinancialAccountId.Value,
                                     table.BrowserName, "U", "", table.ArabicAddress, table.LatinAddress, table.Email, table.Phone, table.AccountNumber);
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editBanks);
                return new ResponseResult { Id = table.Id, Result = Result.Success, Note = Aliases.Actions.UpdatedSuccessfully };

                //  return repositoryActionResult.GetRepositoryActionResult(table.Id, RepositoryActionStatus.Updated, message: "Updated Successfully");
            }
            catch (Exception ex)
            {
                return new ResponseResult { Data = ex };
                //   return repositoryActionResult.GetRepositoryActionResult(ex);
            }
        }
        public async Task<ResponseResult> GetBankById(int Id)
        {
            try
            {
                var banks = (await banksRepositoryQuery.GetAllIncludingAsync(0, 0, b => b.Id == Id,
                    bb => bb.BankBranches,
                    fa => fa.FinancialAccount)).FirstOrDefault();
                var response = new BankResponsesDTOs.GetAll();
                Mapping.Mapper.Map(banks, response);

                var Branches2 = branchRepositoryQuery.GetAll(e => (banks.BankBranches.Select(s => s.BranchId)).ToArray().Contains(e.Id))
                                    .Select(e => new { ArabicName = e.ArabicName, LatinName = e.LatinName }).ToList();
                response.BranchNameAr = string.Join(',', Branches2.Select(e => e.ArabicName).ToArray());
                response.BranchNameEn = string.Join(',', Branches2.Select(e => e.LatinName).ToArray());

                return new ResponseResult
                {
                    Data = response,
                    Result = response != null ? Result.Success : Result.NotFound,
                    Note = response != null ? Aliases.Actions.Success : Aliases.Actions.NotFound
                };
                #region OldCode

                //var treeData = await banksRepositoryQuery.SingleOrDefault(q => q.Id == Id,
                //    e => e.BankBranches.Where(Q => Q.BankId == Id));
                //var financial = await financialAccountRepositoryQuery.GetByAsync(q => q.Id == treeData.FinancialAccountId);
                //var Dto = new BankResponsesDTOs.GetAll()
                //{
                //    Id = treeData.Id,
                //    ArabicName = treeData.ArabicName,
                //    LatinName = treeData.LatinName,
                //    Code = treeData.Code,
                //    AccountNumber = treeData.AccountNumber,
                //    FinancialAccountId = treeData.FinancialAccountId,
                //    FinancialAccountCode = financial?.AccountCode,
                //    BranchesId = new List<int>(),
                //    Notes = treeData.Notes,
                //    Status = treeData.Status,
                //    FinancialName = financial?.ArabicName,
                //    AddressAr = treeData.ArabicAddress,
                //    AddressEn = treeData.LatinAddress,
                //    Email = treeData.Email,
                //    Phone = treeData.Phone,
                //    Website = treeData.Website
                //};
                //var branchbank = bankBranchRepositoryQuery.FindAll(q => q.BankId == treeData.Id).ToList();
                //for (int i = 0; i < branchbank.Count; i++)
                //{
                //    var branchs = await branchRepositoryQuery.GetByAsync(q => q.BranchId == branchbank[i].BranchId);
                //    Dto.BranchesId.Add(branchs.BranchId);
                //    Dto.BranchNameAr += branchs.ArabicName;
                //    if (i < branchbank.Count - 1)
                //    {
                //        Dto.BranchNameAr += ",";
                //    }
                //}

                //return new ResponseResult { Data = Dto, Result = Result.Success, Note = Aliases.Actions.Success };
                ////return repositoryActionResult.GetRepositoryActionResult(Dto, RepositoryActionStatus.Ok, message: "Ok"); 
                #endregion
            }
            catch (Exception ex)
            {
                // return repositoryActionResult.GetRepositoryActionResult(ex);
                return new ResponseResult { Data = ex };
            }
        }
        public async Task<ResponseResult> DeleteBank(SharedRequestDTOs.Delete parameter)
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

                var lis = new List<ListOfBanks>();
                foreach (var item2 in parameter.Ids)
                {
                    var reciept = await recieptsRepositoryQuery.GetByAsync(q => q.BankId == item2);
                    if (reciept != null)
                    {
                        var bra = await banksRepositoryQuery.GetByAsync(q => q.Id == reciept.BankId);
                        var list = new ListOfBanks();
                        list.Name = bra?.ArabicName;
                        lis.Add(list);
                    }
                }
               // var count = lis.Count();
                List<int> FA_Ids = new List<int>();
                if (lis.Count() < parameter.Ids.Count())
                {
                    var BanksDeletedCount = 0;
                    foreach (var item in parameter.Ids)
                    {
                        var BranchDeleted = await banksRepositoryQuery.GetByAsync(q => q.Id == item);
                        FA_Ids.Add(BranchDeleted.FinancialAccountId??0);
                        var bankBranch = bankBranchRepositoryQuery.FindAll(q => q.BankId == BranchDeleted.Id);
                        var support = await recieptsRepositoryQuery.GetByAsync(q => q.BankId == item);

                        if (support == null)
                        {
                            bankBranchRepositoryCommand.RemoveRange(bankBranch);
                            banksRepositoryCommand.Remove(BranchDeleted);
                            
                            BanksDeletedCount++;
                        }
                    }
                    await banksRepositoryCommand.SaveAsync();

                    var FADeleted = await _financialAccountBusiness.DeleteFinancialAccountAsync(new SharedRequestDTOs.Delete()
                    {
                        Ids = FA_Ids.ToArray(),
                        userId = userInfo.userId
                    });
                    if (lis.Count() == 0 || BanksDeletedCount > 0)
                    {
                        await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteBanks,parameter.Ids.Count());
                        return new ResponseResult { Result = Result.Success, Note = Aliases.Actions.DeletedSuccessfully };
                    }
                    else
                    {
                        return new ResponseResult { Data = lis, Result = Result.Failed, Note = Aliases.Actions.CanNotBeDeleted };
                    }
                }
                else
                {
                    return new ResponseResult { Data = lis, Result = Result.Failed, Note = Aliases.Actions.CanNotBeDeleted };
                }
            }
            catch (Exception ex)
            {
                return new ResponseResult { Data = ex };
            }
        }
        public async Task<ResponseResult> GetAllBanksData(BankRequestsDTOs.Search parameters)
        {
            try
            {
                var userinfo = await _userinformation.GetUserInformation();
                var treeData = banksRepositoryQuery.TableNoTracking
                           .Include(a => a.reciept)
                           .Include(a => a.BankBranches)
                           .Include(a => a.FundsBanksSafes)
                           .Include(a => a.FinancialAccount)
                           .Include(a => a.PaymentMethod)
                           .Where(x =>
                           (parameters.SearchCriteria == string.Empty || parameters.SearchCriteria == null || x.Code.ToString().Contains(parameters.SearchCriteria) ||
                           x.ArabicName.ToLower().Contains(parameters.SearchCriteria) || x.LatinName.ToLower().Contains(parameters.SearchCriteria)));


                treeData = treeData.Where( x => x.BankBranches.Select(d => d.BranchId).ToArray().Any(d => userinfo.employeeBranches.Contains(d)));
                int totalCount = treeData.Count();
                if (parameters.Status != 0)
                    treeData = treeData.Where(q => q.Status == parameters.Status);

                var treeDataList = (string.IsNullOrEmpty(parameters.SearchCriteria) ?
                    treeData.OrderByDescending(q => q.Code) :
                    treeData.OrderBy(a => (a.Code.ToString().Contains(parameters.SearchCriteria)) ? 0 : 1)).ToList();

                var data = new List<BankResponsesDTOs.GetAll>();
                Mapping.Mapper.Map(treeDataList, data);

                if (parameters.PageSize > 0 && parameters.PageNumber > 0)
                {
                    data = data.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToList();
                }
                var res =  new List<BankResponsesDTOs.GetAll>();
                foreach (var item in data)
                {
                    var Branches2 = branchRepositoryQuery.GetAll(e => item.BranchesId.Contains(e.Id))
                                    .Select(e => new { ArabicName = e.ArabicName, LatinName = e.LatinName }).ToList();
                    item.BranchNameAr = string.Join(',', Branches2.Select(e => e.ArabicName).ToArray());
                    item.BranchNameEn = string.Join(',', Branches2.Select(e => e.LatinName).ToArray());
                    if (item.Id == 1)
                    {
                        item.CanDelete = false;
                    }
                    else if (treeData.Where(x=> x.Id == item.Id).FirstOrDefault().reciept.Count == 0 && treeData.Where(x => x.Id == item.Id).FirstOrDefault().PaymentMethod.Count == 0 && treeData.Where(x => x.Id == item.Id).FirstOrDefault().FundsBanksSafes.Count == 0)
                    {
                        item.CanDelete = true;
                    }
                    
                    res.Add(item);
                }
                var allFinaincalAccounts = _FinancialAccountQuery.TableNoTracking;
                var allBranches = branchRepositoryQuery.TableNoTracking;
                var response = res.Select(x => new
                {
                    x.Id,
                    x.LatinName,
                    x.ArabicName,
                    x.Code,
                    x.Phone,
                    x.Email,
                    x.Status,
                    x.AddressAr,
                    x.AddressEn,
                    x.Website,
                    branchNameAr = string.Join(",", allBranches.Where(c => x.BranchesId.Contains(c.Id)).Select(c => c.ArabicName).ToList()),
                    branchNameEn = string.Join(",", allBranches.Where(c => x.BranchesId.Contains(c.Id)).Select(c => c.LatinName).ToList()),
                    BranchesId = allBranches.Where(c => x.BranchesId.Contains(c.Id)).Select(c => c.Id).ToList(),
                    x.AccountNumber,
                    x.Notes,
                    FinancialAccountId = allFinaincalAccounts.Where(c=> c.Id == x.FinancialAccountId).Select(c=> new {c.Id,c.ArabicName,c.LatinName}).FirstOrDefault(),
                    x.CanDelete
                });
                return new ResponseResult {TotalCount = totalCount, Data = response, DataCount = treeDataList.Count(), Result = Result.Success, Note = Aliases.Actions.Success };
            }
            catch (Exception ex)
            {
                return new ResponseResult { Data = ex };

            }
            #region Old Code
            //try
            //{
            //    var searchCretiera = paramters.SearchCriteria;
            //    var treeData = banksRepositoryQuery
            //   .TableNoTracking
            //   .Include(a => a.supports)
            //   .Include(a => a.BankBranches)
            //   .Include(a => a.FundsBanksSafes)
            //   .Where(x =>
            //   (searchCretiera == string.Empty || searchCretiera == null || x.Code.ToString().Contains(searchCretiera) ||
            //   x.ArabicName.ToLower().Contains(searchCretiera) || x.LatinName.ToLower().Contains(searchCretiera)));
            //    var list = new List<BankResponsesDTOs.GetAll>();

            //    if (paramters.Status != 0)
            //        treeData = treeData.Where(q => q.Status == paramters.Status);

            //    var treeDataList = (string.IsNullOrEmpty(paramters.SearchCriteria) ? treeData.OrderByDescending(q => q.Code) : treeData.OrderBy(a => (a.Code.ToString().Contains(paramters.SearchCriteria)) ? 0 : 1)).ToList();

            //    foreach (var item in treeDataList)
            //    {
            //        var financial = await financialAccountRepositoryQuery.GetByAsync(q => q.Id == item.FinancialAccountId);
            //        var branch = await branchRepositoryQuery.GetByAsync(q => q.BranchId == item.BranchId);
            //        var bankDTO = new BankResponsesDTOs.GetAll()
            //        {
            //            Id = item.Id,
            //            ArabicName = item.ArabicName,
            //            LatinName = item.LatinName,
            //            AccountNumber = item.AccountNumber,
            //            FinancialAccountId = item.FinancialAccountId,
            //            FinancialAccountCode = financial?.AccountCode,
            //            BranchesId = new List<int>(),
            //            Code = item.Code,
            //            Notes = item.Notes,
            //            Status = item.Status,
            //            FinancialName = financial?.ArabicName,
            //            AddressAr = item.ArabicAddress,
            //            AddressEn = item.LatinAddress,
            //            Email = item.Email,
            //            Phone = item.Phone,
            //            Website = item.Website

            //        };
            //        bankDTO.CanDelete = (item.Id == 1 || item.supports.Count() > 0
            //                              || item.BankBranches.Count() > 0 ||
            //                               item.FundsBanksSafes.Count() > 0
            //                                   ? false : true);

            //        var branchbank = bankBranchRepositoryQuery.FindAll(q => q.BankId == item.Id).ToList();
            //        for (int i = 0; i < branchbank.Count; i++)
            //        {
            //            var branchs = await branchRepositoryQuery.GetByAsync(q => q.BranchId == branchbank[i].BranchId);
            //            bankDTO.BranchesId.Add(branchs.BranchId);

            //            bankDTO.BranchNameAr += branchs.ArabicName;
            //            bankDTO.BranchNameEn += branchs.LatinName;
            //            if (i < branchbank.Count - 1)
            //            {
            //                bankDTO.BranchNameAr += ",";
            //                bankDTO.BranchNameEn += ",";
            //            }
            //        }
            //        list.Add(bankDTO);

            //    }
            //    list = (string.IsNullOrEmpty(paramters.SearchCriteria) ? list.OrderByDescending(q => q.Code) : list.OrderBy(a => (a.Code.ToString().Contains(paramters.SearchCriteria)) ? 0 : 1)).ToList();


            //    if (paramters.PageSize > 0 && paramters.PageNumber > 0)
            //    {
            //        list = list.Skip((paramters.PageNumber - 1) * paramters.PageSize).Take(paramters.PageSize).ToList();
            //    }
            //    else
            //    {
            //        return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };
            //    }

            //    return new ResponseResult { Data = list, DataCount = treeDataList.Count(), Result = Result.Success, Note = Aliases.Actions.Success };
            //}
            //catch (Exception ex)
            //{
            //    return new ResponseResult { Data = ex };

            //} 
            #endregion
        }
        public async Task<ResponseResult> GetAllBanksDataDropDown()
        {
            try
            {
                var userinfo = await _userinformation.GetUserInformation();
                var _dropDownData =  banksRepositoryQuery
                                    .TableNoTracking.Include(x=> x.BankBranches);
                var dropDownData = _dropDownData
                    .Where(x=> x.BankBranches.Select(d => d.BranchId).ToArray().Any(c=> userinfo.employeeBranches.Contains(c)) && userinfo.userBanks.Contains(x.Id))

                    .Select(e => new
                            {
                                e.Id,
                                e.ArabicName,
                                e.LatinName,
                                e.Status
                            }).ToList();
                                    
                return new ResponseResult { Data = dropDownData, Result = dropDownData.Any() ? Result.Success : Result.Failed, Note = Aliases.Actions.Success };
            }
            catch (Exception ex)
            {
                return new ResponseResult { Data = ex };
            }
        }
        public async Task<ResponseResult> GetAllBanksByUserDropDown()
        {
            try
            {
                var userinfo = await _userinformation.GetUserInformation();
                var treeData = banksRepositoryQuery.GetAll().Where(x=>  userinfo.userBanks.Contains(x.Id)).ToList();


                return new ResponseResult { Data = treeData, Result = treeData.Any() ? Result.Success : Result.Failed, Note = Aliases.Actions.Success };
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
                if (parameter.Status < (int)Status.Active || parameter.Status > (int)Status.Inactive)
                {
                    return new ResponseResult { Result = Result.Failed, Note = Aliases.Actions.InvalidStatus };
                }
                foreach (var item in parameter.Id)
                {
                    var bank = await banksRepositoryQuery.GetByAsync(q => q.Id == item);
                    if (bank.Id == 1)
                    {
                        bank.Status = (int)Status.Active;
                    }
                    else
                    {
                        bank.Status = parameter.Status;
                    }
                    await banksRepositoryCommand.UpdateAsyn(bank);

                }
                return new ResponseResult { Result = Result.Success, Note = Aliases.Actions.UpdatedSuccessfully };
                // return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.Updated, message: "Updated Successfully");
            }
            catch (Exception ex)
            {
                return new ResponseResult { Data = ex };
                //return repositoryActionResult.GetRepositoryActionResult(ex);
            }
        }
        public async void HistoryBanks(int branchId, int status, string nameAr, string nameEn, string notes, int bankId, int? FinancialAccountId,
                                       string browserName, string lastTransactionAction, string LastTransactionUser, string addressAr, string addressEn, string email, string phone, string accountNumber)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var history = new GLBanksHistory()
            {
                employeesId=userInfo.employeeId,
                LastDate = DateTime.Now,
                LastAction = lastTransactionAction,
                BranchId = branchId,
                AddressAr = addressAr,
                AddressEn = addressEn,
                Email = email,
                Phone = phone,
                AccountNumber = accountNumber,
                FinancialAccountId = FinancialAccountId,
                LastTransactionUser = userInfo.employeeNameEn.ToString(),
                LastTransactionUserAr = userInfo.employeeNameAr.ToString(),
                BankId = bankId,
                Status = status,
                ArabicName = nameAr,
                LatinName = nameEn,
                Notes = notes,
                BrowserName = browserName,
            };
            banksHistoryRepositoryCommand.Add(history);
        }
        public async Task<ResponseResult> GetAllBankHistory(int BankId)
        {
            var parentDatasQuey = banksHistoryRepositoryQuery.FindQueryable(s => s.BankId == BankId).Include(a=>a.employees);
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
            //return repositoryActionResult.GetRepositoryActionResult(historyList, RepositoryActionStatus.Ok, message: "Ok");
        }
        public async Task<ResponseResult> GetAllBanksSetting()
        {
            try
            {
                var treeData = banksRepositoryQuery.TableNoTracking.Include(q => q.FinancialAccount);
                var list = Mapping.Mapper.Map<List<GLBank>, List<BankResponsesDTOs.BankSettingDto>>(treeData.ToList());
                #region
                //var list = new List<BankSettingDto>();
                //foreach (var item in treeData)
                //{
                //    var bankDto = new BankSettingDto();
                //    bankDto.Id = item.Id;
                //    bankDto.NameAr = item.NameAr;
                //    bankDto.NameEn = item.NameEn;
                //    bankDto.FinancialAccountId = item.FinancialAccountId;
                //    var financial = await financialAccountRepositoryQuery.GetByAsync(q=>q.Id == item.FinancialAccountId);
                //    bankDto.FinancialAccountCode = financial.AccountCode;
                //    bankDto.FinancialAccountName = financial.NameAr;

                //    list.Add(bankDto);
                //}
                #endregion
                return new ResponseResult { Data = list, Result = Result.Success, Note = Aliases.Actions.Success };
                //return repositoryActionResult.GetRepositoryActionResult(list, RepositoryActionStatus.Ok, message: "Ok");
            }
            catch (Exception ex)
            {
                //   return repositoryActionResult.GetRepositoryActionResult(ex);
                return new ResponseResult { Data = ex };
            }
        }
    }
}
