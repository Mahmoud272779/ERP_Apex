using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Application.Basic_Process;
using App.Application.Helpers;
using App.Application.Helpers.Service_helper.History;
using App.Application.Services.HelperService;
using App.Application.Services.Process.FinancialAccounts;
using App.Application.Services.Process.GeneralServices.SystemHistoryLogsServices;
using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using App.Infrastructure.UserManagementDB;
using Attendleave.Erp.Core.APIUtilities;
using Attendleave.Erp.ServiceLayer.Abstraction;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Net.Http.Headers;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Safes
{
    public class OtherAuthoritiesServices : BusinessBase<GLOtherAuthorities>, IOtherAuthoritiesServices
    {
        private readonly IRepositoryCommand<GLOtherAuthoritiesHistory> OtherAuthoritiesHistoryCommand;
        private readonly IRepositoryCommand<GLOtherAuthorities> otherAuthoritiesRepositoryCommand;
        private readonly IRepositoryQuery<GLOtherAuthoritiesHistory> OtherAuthoritiesHistoryQuery;
        private readonly IRepositoryQuery<GLOtherAuthorities> otherAuthoritiesRepositoryQuery;
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GLFinancialSetting> financialSettingRepositoryQuery;
        private readonly IRepositoryQuery<GLGeneralSetting> _gLGeneralSettingQuery;
        private readonly IRepositoryQuery<GlReciepts> _glRecieptsQuery;
        private readonly IRepositoryQuery<InvEmployees> EmployeeQuery;
        private readonly iUserInformation _userinformation;
        private readonly IFinancialAccountBusiness _financialAccountBusiness;
        private readonly iGLFinancialAccountRelation _iGLFinancialAccountRelation;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IHttpContextAccessor httpContext;
        private readonly IHelperService GeneralSetteingCashing;
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<GLIntegrationSettings> _GLIntegrationSettingsQuery;



        public OtherAuthoritiesServices(
           IRepositoryCommand<GLOtherAuthoritiesHistory> otherAuthoritiesHistoryCommand,
           IRepositoryCommand<GLOtherAuthorities> OtherAuthoritiesRepositoryCommand,
           IRepositoryQuery<GLOtherAuthoritiesHistory> otherAuthoritiesHistoryQuery,
           IRepositoryQuery<GLOtherAuthorities> OtherAuthoritiesRepositoryQuery,
           IRepositoryQuery<GLFinancialSetting> FinancialSettingRepositoryQuery,
           IRepositoryQuery<GLFinancialAccount> FinancialAccountRepositoryQuery,
           IRepositoryQuery<GLGeneralSetting> GLGeneralSettingQuery,
           IRepositoryQuery<GlReciepts> GlRecieptsQuery,
           IRepositoryQuery<InvEmployees> employeeQuery,
           iUserInformation Userinformation,
           IFinancialAccountBusiness financialAccountBusiness,
           iGLFinancialAccountRelation iGLFinancialAccountRelation,
           ISystemHistoryLogsService systemHistoryLogsService,
           IRepositoryActionResult repositoryActionResult,
           IHttpContextAccessor HttpContext, iUserInformation iUserInformation
, IRepositoryQuery<GLIntegrationSettings> gLIntegrationSettingsQuery) : base(repositoryActionResult)
        {
            otherAuthoritiesRepositoryCommand = OtherAuthoritiesRepositoryCommand;
            otherAuthoritiesRepositoryQuery = OtherAuthoritiesRepositoryQuery;
            financialAccountRepositoryQuery = FinancialAccountRepositoryQuery;
            financialSettingRepositoryQuery = FinancialSettingRepositoryQuery;
            OtherAuthoritiesHistoryCommand = otherAuthoritiesHistoryCommand;
            OtherAuthoritiesHistoryQuery = otherAuthoritiesHistoryQuery;
            _iGLFinancialAccountRelation = iGLFinancialAccountRelation;
            _systemHistoryLogsService = systemHistoryLogsService;
            _gLGeneralSettingQuery = GLGeneralSettingQuery;
            _glRecieptsQuery = GlRecieptsQuery;
            _userinformation = Userinformation;
            _financialAccountBusiness = financialAccountBusiness;
            EmployeeQuery = employeeQuery;
            httpContext = HttpContext;
            _iUserInformation = iUserInformation;
            _GLIntegrationSettingsQuery = gLIntegrationSettingsQuery;
        }


        public async Task<ResponseResult> AddOtherAuthorities(OtherAuthoritiesParameter parameter)
        {
            try
            {
                UserInformationModel userInfo = await _userinformation.GetUserInformation();
                var GLSettings = _gLGeneralSettingQuery.TableNoTracking.FirstOrDefault();
                //var setting = await GeneralSetteingCashing.GetAllGeneralSettings();

                parameter.LatinName = Helpers.Helpers.IsNullString(parameter.LatinName);
                parameter.ArabicName = Helpers.Helpers.IsNullString(parameter.ArabicName);

                if (string.IsNullOrEmpty(parameter.LatinName))
                {
                    parameter.LatinName = parameter.ArabicName;
                }

                //requrid name arabic
                if (string.IsNullOrEmpty(parameter.ArabicName))
                {
                    return new ResponseResult { Result = Result.Failed, Note = Aliases.Actions.NameIsRequired };
                }

                //check finiancal account 
                if (!financialAccountRepositoryQuery.TableNoTracking.Where(c => c.Id == parameter.FinancialAccountId).Any())
                    if (parameter.Status < (int)Status.Active || parameter.Status > (int)Status.Inactive)
                    {
                        return new ResponseResult { Result = Result.Failed, Note = "Financial Account Not Exist" };
                    }
                //we can change int to bool 
                if (parameter.Status < (int)Status.Active || parameter.Status > (int)Status.Inactive)
                {
                    return new ResponseResult { Result = Result.Failed, Note = Aliases.Actions.InvalidStatus };
                }

                //check if object is exist in dataBase or not
                var OtherAuthoritiesExist = await otherAuthoritiesRepositoryQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName && !string.IsNullOrEmpty(parameter.ArabicName));
                if (OtherAuthoritiesExist != null)
                    return new ResponseResult { Result = Result.Exist, Note = Aliases.Actions.ArabicNameExist };

                OtherAuthoritiesExist = await otherAuthoritiesRepositoryQuery.GetByAsync(a => a.LatinName == parameter.LatinName && !string.IsNullOrEmpty(parameter.LatinName));

                if (OtherAuthoritiesExist != null)
                    return new ResponseResult { Result = Result.Exist, Note = Aliases.Actions.EnglishNameExist };


                var table = Mapping.Mapper.Map<OtherAuthoritiesParameter, GLOtherAuthorities>(parameter);
                table.Code = otherAuthoritiesRepositoryQuery.GetMaxCode(a => a.Code) + 1; //await AddAutomaticCode();


                var finanicalAccount = await _iGLFinancialAccountRelation.GLRelation(GLFinancialAccountRelation.OtherAuthorities, parameter.FinancialAccountId ?? 0, new int[] { userInfo.CurrentbranchId }, table.ArabicName, table.LatinName);
                if (finanicalAccount.Result != Result.Success)
                    return finanicalAccount;

                table.FinancialAccountId = finanicalAccount.Id;

                var saved = otherAuthoritiesRepositoryCommand.Add(table);

                if (!saved)
                {
                    return new ResponseResult { Id = null, Result = Result.Failed, ErrorMessageAr = " لا يمكن الحفظ", ErrorMessageEn = "Data Not Saved" };
                }

                AddOtherAuthoritiesHistory(table, "A", parameter.userId);
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addOtherAuthorities);

                return new ResponseResult { Id = table.Id, Result = Result.Success, Note = "Saved Successfully" };
            }
            catch (Exception e)
            {


                return new ResponseResult { Id = null, Result = Result.Success, Note = e.Message };

            }

        }
        public async Task<ResponseResult> UpdateOtherAuthorities(UpdateOtherAuthoritiesParameter parameter)
        {
            if (parameter.Id == -1)
                return new ResponseResult()
                {
                    Note = Actions.DefultDataCanNotbeEdited,
                    Result = Result.Failed
                };
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
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }

            var OtherAuthoritiesExist = await otherAuthoritiesRepositoryQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName && !string.IsNullOrEmpty(parameter.ArabicName) && a.Id != parameter.Id);
            if (OtherAuthoritiesExist != null)
                return new ResponseResult { Result = Result.Exist, Note = Actions.ArabicNameExist };

            OtherAuthoritiesExist = await otherAuthoritiesRepositoryQuery.GetByAsync(a => a.LatinName == parameter.LatinName && !string.IsNullOrEmpty(parameter.LatinName) && a.Id != parameter.Id);

            if (OtherAuthoritiesExist != null)
                return new ResponseResult { Result = Result.Exist, Note = Actions.EnglishNameExist };

            var updateData = await otherAuthoritiesRepositoryQuery.GetByAsync(q => q.Id == parameter.Id);

            if (updateData == null)
                return new ResponseResult { Result = Result.Exist, Note = " Not found this to updat" };

            var GLSettings = _gLGeneralSettingQuery.TableNoTracking.FirstOrDefault();

            //if (GLSettings.DefultAccSafe != 1)
            //{
            //    parameter.FinancialAccountId = updateData.FinancialAccountId;
            //}
            var table = Mapping.Mapper.Map<UpdateOtherAuthoritiesParameter, GLOtherAuthorities>(parameter, updateData);

            // check financial account
            var userInfo = await _iUserInformation.GetUserInformation();
            var linkingMethod = _GLIntegrationSettingsQuery
            .TableNoTracking
                   .Where(c => c.GLBranchId == userInfo.CurrentbranchId && c.screenId == (int)SubFormsIds.OtherAuthorities_MainData)
                   .FirstOrDefault().linkingMethodId;
            if (linkingMethod == 1 && table.FinancialAccountId != parameter.FinancialAccountId)
                table.FinancialAccountId = parameter.FinancialAccountId;
            var saved = await otherAuthoritiesRepositoryCommand.UpdateAsyn(table);

            if (!saved)
            {
                return new ResponseResult { Id = null, Result = Result.Failed, ErrorMessageAr = " لا يمكن الحفظ", ErrorMessageEn = "Data Not Saved" };
            }
            AddOtherAuthoritiesHistory(table, "U", parameter.userId);

            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editOtherAuthorities);
            return new ResponseResult { Id = table.Id, Result = Result.Success, Note = Actions.UpdatedSuccessfully };

        }
        public async Task<ResponseResult> GetOtherAuthoritiesById(int Id)
        {
            var resOSA = (await otherAuthoritiesRepositoryQuery.GetAllIncludingAsync(0, 0,
                                                        a => a.Id == Id,
                                                        w => w.Branch, w => w.FinancialAccount)).FirstOrDefault();
            // var retSafe = safe.FirstOrDefault();
            if (resOSA != null)
            {

                return new ResponseResult { Data = resOSA, Id = Id, Result = Result.Success, Note = Aliases.Actions.Success };
            }
            return new ResponseResult { Data = null, Result = Result.NotFound, Note = Aliases.Actions.NotFound };

        }
        public async Task<ResponseResult> DeleteOtherAuthoritiesAsync(SharedRequestDTOs.Delete parameter)
        {


            UserInformationModel userInfo = await _userinformation.GetUserInformation();
            if (userInfo == null)
                return new ResponseResult()
                {
                    Note = Actions.JWTError,
                    Result = Result.Failed
                };
            var test = await otherAuthoritiesRepositoryQuery.GetAllIncludingAsync(0, 0,
                                                            a => parameter.Ids.Contains(a.Id),
                                                             w => w.reciept);
            var elementsCanBeDeleted = test.Where(x => x.reciept.Count() == 0);
            int count = elementsCanBeDeleted.Count();
            if (parameter.Ids.Contains(-1))
                return new ResponseResult()
                {
                    Note = Actions.DefultDataCanNotbeDeleted,
                    Result = Result.Failed
                };


            List<int> FA_Ids = new List<int>();
            //var test = await otherAuthoritiesRepositoryQuery.GetAllIncludingAsync(0, 0, a => parameter.Ids.Contains(a.Id));
            List<int> deletedItems = new List<int>();
            foreach (var item in elementsCanBeDeleted)
            {
                if (item.Id != -1)
                {
                    var result = await otherAuthoritiesRepositoryCommand.DeleteAsync(item.Id);
                    if (result)
                    {
                        FA_Ids.Add(item.FinancialAccountId ?? 0);
                        deletedItems.Add(item.Id);
                        AddOtherAuthoritiesHistory(item, "D", parameter.userId);
                    }
                }
            }
            var FA_Deleted = await _financialAccountBusiness.DeleteFinancialAccountAsync(new SharedRequestDTOs.Delete()
            {
                userId = userInfo.userId,
                Ids = FA_Ids.ToArray()
            });
            //return list of deleted ids
            if (deletedItems.Any())
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteOtherAuthorities,count);
            return new ResponseResult() { Note = deletedItems.Any() ? "" : Actions.DefultDataCanNotbeDeleted, Data = deletedItems, Result = deletedItems.Any() ? Result.Success : Result.Failed };

        }
        public async Task<ResponseResult> GetAllOtherAuthoritiesData(PageOtherAuthoritiesParameter parameters)
        {
            var recs = _glRecieptsQuery.TableNoTracking.Where(x => x.Authority == (int)AuthorityTypes.other);
            var resData = await otherAuthoritiesRepositoryQuery.GetAllIncludingAsync(
                        0, 0,
                        x => (parameters.Status == 0 ? 1 == 1 : x.Status == parameters.Status) && (parameters.SearchCriteria == string.Empty || parameters.SearchCriteria == null || x.Code.ToString().Contains(parameters.SearchCriteria) ||
                           x.ArabicName.ToLower().Contains(parameters.SearchCriteria) || x.LatinName.ToLower().Contains(parameters.SearchCriteria))
                           , e => (string.IsNullOrEmpty(parameters.Name) ?
                        e.OrderByDescending(q => q.Code) :
                        e.OrderBy(a => (a.Code.ToString().Contains(parameters.Name)) ? 0 : 1)));

            var count = resData.Count();
            int totalCount = resData.Count();
            var allFinancialAccouns = financialAccountRepositoryQuery.TableNoTracking;
            var finalResult = Pagenation<GLOtherAuthorities>.pagenationList(parameters.PageSize, parameters.PageNumber, resData.ToList());
            var list = new List<GLOtherAuthorities>();
            foreach (var item in finalResult)
            {
                if (item.Id == -1)
                    item.CanDelete = false;
                else
                {
                    if (recs.Where(x => x.BenefitId == item.Id).Any())
                        item.CanDelete = false;
                    else
                        item.CanDelete = true;
                }
                list.Add(item);
            }

            var response = list.Select(x => new
            {
                x.Id,
                x.ArabicName,
                x.LatinName,
                x.Code,
                x.Status,
                x.Notes,
                x.CanDelete,
                financialAccountId = allFinancialAccouns.Where(c => c.Id == x.FinancialAccountId).Select(c => new { c.Id, c.ArabicName, c.LatinName }).FirstOrDefault()
            });
            return new ResponseResult() { TotalCount = totalCount, Data = response, DataCount = count, Id = null, Result = resData.Any() ? Result.Success : Result.Failed };

        }
        public async Task<ResponseResult> GetAllOtherAuthoritiesDataDropDown(DropDownParameter parameter)
        {
            var userInfo = _iUserInformation.GetUserInformation();
            var Data = otherAuthoritiesRepositoryQuery.GetAll(a =>
                                                           a.Status == (int)Status.Active
                                                           && (parameter.code != 0 ? a.Code == parameter.code : 1 == 1)
                                                           || (parameter.SearchCriteria == string.Empty || parameter.SearchCriteria == null ||
                           a.ArabicName.ToLower().Contains(parameter.SearchCriteria) || a.LatinName.ToLower().Contains(parameter.SearchCriteria))).ToList();
            List<GLOtherAuthorities> pagenatData = Pagenation<GLOtherAuthorities>.pagenationList(parameter.pageSize, parameter.pageNumber, Data);

            var list = Mapping.Mapper.Map<List<GLOtherAuthorities>, List<TreasuryDto>>(Data.ToList());


            return new ResponseResult { Data = list, Result = Result.Success, Note = "OK" };

        }
        public async Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameter)
        {
            if (parameter.Id.Count() == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };
            if (parameter.Id.Contains(-1))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, ErrorMessageAr = " لا يمكن تعديل الحاله فى العنصر الافتراضى   ", ErrorMessageEn = " can not change status in defult value" };
            var Othor = otherAuthoritiesRepositoryQuery.TableNoTracking.Where(e => parameter.Id.Contains(e.Id));
            var OthorList = Othor.ToList();

            OthorList.Select(e => { e.Status = parameter.Status; return e; }).ToList();

            var rssult = await otherAuthoritiesRepositoryCommand.UpdateAsyn(OthorList);

            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };


        }
        public async Task<ResponseResult> AddOtherAuthoritiesHistory(GLOtherAuthorities history, string lastActoin, int userId)
        {

            var userInfo = _iUserInformation.GetUserInformation();
            var otherAuthorityHistory = new GLOtherAuthoritiesHistory()
            {
                ArabicName = history.LatinName,
                LatinName = history.LatinName,
                AuthorityId = history.Id,
                BranchId = history.BranchId,


                BrowserName = contextHelper.GetBrowserName(httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString()),
                UserId = userId,
                userName = userId.ToString(),
                FinancialAccountId = history.FinancialAccountId,
                LastAction = lastActoin,
                LastDate = DateTime.Now
            };
            OtherAuthoritiesHistoryCommand.Add(otherAuthorityHistory);
            await OtherAuthoritiesHistoryCommand.SaveAsync();

            return new ResponseResult() { Result = Result.Success };

        }

        public async Task<ResponseResult> GetOtherAuthoritiesHistory(int Id)
        {

            var Data = OtherAuthoritiesHistoryQuery.TableNoTracking.Where(s => s.AuthorityId == Id).ToList();

            var historyList = new List<HistoryResponceDto>();

            foreach (var item in Data)
            {
                var emp = EmployeeQuery.TableNoTracking.Where(h => h.Id == item.UserId).FirstOrDefault();
                var historyDto = new HistoryResponceDto();
                historyDto.Date = item.LastDate;
                HistoryActionsNames actionName = HistoryActionsAliasNames.HistoryName[item.LastAction];
                historyDto.TransactionTypeAr = actionName.ArabicName;
                historyDto.TransactionTypeEn = actionName.LatinName;
                historyDto.ArabicName = emp.ArabicName;//فى حالة لو اليوزر اتمسح هتعمل ايه 
                historyDto.LatinName = emp.LatinName;//فى حالة لو اليوزر اتمسح هتعمل ايه 
                historyDto.BrowserName = item.BrowserName;
                ///historyDto.LastTransactionAction = specifyHistoryAction(item.ReceiptsAction);
                historyList.Add(historyDto);
            }

            return new ResponseResult() { Data = historyList, Id = null, Result = Result.Success };
        }


    }


}
