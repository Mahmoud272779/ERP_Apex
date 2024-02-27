using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Application.Basic_Process;
using App.Application.Helpers;
using App.Application.Services.Process.GeneralServices.SystemHistoryLogsServices;
using App.Domain.Entities.Process;
using App.Domain.Entities.Process.General_Ledger;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using Attendleave.Erp.Core.APIUtilities;
using Attendleave.Erp.ServiceLayer.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Currancy
{
    public class CurrencyBusiness : BusinessBase<GLCurrency>, ICurrencyBusiness
    {
        private readonly IRepositoryQuery<GLCurrency> currencyRepositoryQuery;
        private readonly IRepositoryCommand<GLCurrency> currencyRepositoryCommand;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IRepositoryQuery<GLCurrencyHistory> currencyHistoryRepositoryQuery;
        private readonly IRepositoryCommand<GLCurrencyHistory> currencyHistoryRepositoryCommand;
        private readonly IHttpContextAccessor httpContext;
        private readonly iUserInformation _iUserInformation;

        public CurrencyBusiness(
            IRepositoryQuery<GLCurrency> CurrencyRepositoryQuery,
            IRepositoryQuery<GLFinancialAccount> FinancialAccountRepositoryQuery,
            IRepositoryCommand<GLCurrency> CurrencyRepositoryCommand,
            IPagedList<GLCurrency, GLCurrency> PagedListCurrency,
            IRepositoryActionResult repositoryActionResult,
            ISystemHistoryLogsService systemHistoryLogsService,
             IRepositoryQuery<GLCurrencyHistory> currencyHistoryRepositoryQuery,
             IRepositoryCommand<GLCurrencyHistory> currencyHistoryRepositoryCommand,
             IHttpContextAccessor httpContext, iUserInformation iUserInformation
             ) : base(repositoryActionResult)
        {
            currencyRepositoryQuery = CurrencyRepositoryQuery;
            currencyRepositoryCommand = CurrencyRepositoryCommand;
            _systemHistoryLogsService = systemHistoryLogsService;
            this.currencyHistoryRepositoryQuery = currencyHistoryRepositoryQuery;
            this.currencyHistoryRepositoryCommand = currencyHistoryRepositoryCommand;
            this.httpContext = httpContext;
            _iUserInformation = iUserInformation;
        }
        public async Task<int> AddAutomaticCode()
        {
            var code = currencyRepositoryQuery.FindQueryable(q => q.Id > 0).ToList().Last();
            int codee = (Convert.ToInt32(code.Code));
            var NewCode = codee + 1;
            return NewCode;
        }
        public async Task<GLCurrency> CheckIsValidNameAr(string NameAr)
        {

            var Currency
                = await currencyRepositoryQuery.GetByAsync(
                   cust => cust.ArabicName == NameAr);
            return Currency;

        }
        public async Task<bool> CheckIsValidNameArUpdate(string NameAr, int Id)
        {

            var Currency
                = await currencyRepositoryQuery.GetByAsync(
                   cust => cust.ArabicName == NameAr && cust.Id == Id);
            return Currency == null ? false : true;

        }
        public async Task<bool> CheckIsValidNameEnUpdate(string NameEn, int Id)
        {

            var Currency
                = await currencyRepositoryQuery.GetByAsync(
                   cust => cust.LatinName == NameEn && cust.Id == Id);
            return Currency == null ? false : true;

        }
        public async Task<GLCurrency> CheckIsValidNameEn(string NameEn)
        {

            var Currency
                = await currencyRepositoryQuery.GetByAsync(
                   cust => cust.LatinName == NameEn && !string.IsNullOrEmpty(NameEn));
            return Currency;

        }
        public async Task<ResponseResult> AddCurrency(CurrencyParameter parameter)
        {
            try
            {
                
                parameter.LatinName = Helpers.Helpers.IsNullString(parameter.LatinName);
                parameter.ArabicName = Helpers.Helpers.IsNullString(parameter.ArabicName);
                parameter.CoinsEn = Helpers.Helpers.IsNullString(parameter.CoinsEn);
                parameter.CoinsAr = Helpers.Helpers.IsNullString(parameter.CoinsAr);
                parameter.AbbrEn = Helpers.Helpers.IsNullString(parameter.AbbrEn);
                parameter.AbbrAr = Helpers.Helpers.IsNullString(parameter.AbbrAr);
                parameter.BrowserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();

                //  parameter.LastUpdate = DateTime.Now.ToString();
                if (string.IsNullOrEmpty(parameter.LatinName))
                {
                    parameter.LatinName = parameter.ArabicName;
                }
                if (string.IsNullOrEmpty(parameter.CoinsEn))
                {
                    parameter.CoinsEn = parameter.CoinsAr;
                }
                if (string.IsNullOrEmpty(parameter.AbbrEn))
                {
                    parameter.AbbrEn = parameter.AbbrAr;
                }

                if (string.IsNullOrEmpty(parameter.ArabicName))
                {
                    return new ResponseResult { Result = Result.Failed, Note = Aliases.Actions.NameIsRequired };
                }
                if (parameter.Status < (int)Status.Active || parameter.Status > (int)Status.Inactive)
                {
                    return new ResponseResult { Result = Result.Failed, Note = Aliases.Actions.InvalidStatus };
                }

                var currencyExist = await currencyRepositoryQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName && !string.IsNullOrEmpty(parameter.ArabicName));
                if (currencyExist != null)
                    return new ResponseResult { Result = Result.Exist, Note = Aliases.Actions.ArabicNameExist };
                currencyExist = await currencyRepositoryQuery.GetByAsync(a => a.LatinName == parameter.LatinName && !string.IsNullOrEmpty(parameter.LatinName));
                if (currencyExist != null)
                    return new ResponseResult { Result = Result.Exist, Note = Aliases.Actions.EnglishNameExist };

                if (parameter.isDefault)
                {
                    await ChangeIsDefaultStatus();
                    parameter.Factor = 1;
                    parameter.Status = (int)Status.Active;
                }
                var table = Mapping.Mapper.Map<CurrencyParameter, GLCurrency>(parameter);
                table.Code = currencyRepositoryQuery.GetMaxCode(a => a.Code) + 1;  //  await AddAutomaticCode();

                currencyRepositoryCommand.Add(table);
                await currencyRepositoryCommand.SaveAsync();

                // لو تم تحديد العمله لافتراضي نعدل باقي العملات تبقى غير افتراضيه 
                if (parameter.isDefault)
                {
                    var defaultCurrency = currencyRepositoryQuery.TableNoTracking.ToList();
                    defaultCurrency.Select(a => { a.IsDefault = false; return a; }).ToList();
                    currencyRepositoryCommand.UpdateAsyn(defaultCurrency);
                }

                //string BrowserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();
                HistoryCurrency(0, table.Status, table.ArabicName, table.LatinName, table.Notes, table.Id, table.BrowserName, "A", null);

                //HistoryCurrency(table.BranchId, table.Status, table.ArabicName, table.LatinName, table.Notes, table.Id,
                //                       BrowserName, table.LastTransactionAction, table.LastTransactionUser);
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addCurrancy);
                return new ResponseResult() { Id = table.Id, Data = table, Result = Result.Success, Note = Aliases.Actions.SavedSuccessfully };
            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex, Result = Result.Failed, Note = Aliases.Actions.ExceptionOccurred };
            }
        }

        private async Task<bool> ChangeIsDefaultStatus()
        {
            bool result = false;
            var currencies = await currencyRepositoryQuery.GetAllAsyn();
            if (currencies != null)
            {
                currencies.ToList().ForEach(a => a.IsDefault = false);
                result = await currencyRepositoryCommand.UpdateAsyn(currencies);
            }
            return result;
        }

        public async Task<ResponseResult> UpdateCurrency(UpdateCurrencyParameter parameter)
        {
            try
            {
               
                parameter.LatinName = Helpers.Helpers.IsNullString(parameter.LatinName);
                parameter.ArabicName = Helpers.Helpers.IsNullString(parameter.ArabicName);
                parameter.CoinsEn = Helpers.Helpers.IsNullString(parameter.CoinsEn);
                parameter.CoinsAr = Helpers.Helpers.IsNullString(parameter.CoinsAr);
                parameter.AbbrEn = Helpers.Helpers.IsNullString(parameter.AbbrEn);
                parameter.AbbrAr = Helpers.Helpers.IsNullString(parameter.AbbrAr);
                parameter.BrowserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();

                //  table.LastUpdate = DateTime.Now.ToString();
                if (string.IsNullOrEmpty(parameter.LatinName))
                {
                    parameter.LatinName = parameter.ArabicName;
                }
                
                
                if (string.IsNullOrEmpty(parameter.CoinsEn))
                {
                    parameter.CoinsEn = parameter.CoinsAr;
                }
                if (string.IsNullOrEmpty(parameter.AbbrEn))
                {
                    parameter.AbbrEn = parameter.AbbrAr;
                }
                if (string.IsNullOrEmpty(parameter.ArabicName))
                {
                    return new ResponseResult { Result = Result.Failed, Note = Aliases.Actions.NameIsRequired };
                }
                if (parameter.Status < (int)Status.Active || parameter.Status > (int)Status.Inactive)
                {
                    return new ResponseResult { Result = Result.Failed, Note = Aliases.Actions.InvalidStatus };
                }
                var currencyExist = await currencyRepositoryQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName && !string.IsNullOrEmpty(parameter.ArabicName) && a.Id != parameter.Id);
                if (currencyExist != null)
                    return new ResponseResult { Result = Result.Exist, Note = Aliases.Actions.ArabicNameExist };
                currencyExist = await currencyRepositoryQuery.GetByAsync(a => a.LatinName == parameter.LatinName && !string.IsNullOrEmpty(parameter.LatinName) && a.Id != parameter.Id);
                if (currencyExist != null)
                    return new ResponseResult { Result = Result.Exist, Note = Aliases.Actions.EnglishNameExist };


                if (parameter.isDefault)
                {
                    parameter.Factor = 1;
                    parameter.Status = (int)Status.Active;
                    await ChangeIsDefaultStatus();
                }
             

                // لو تم تغير العمله لافتراضي نعدل باقي العملات تبقى غير افتراضيه 
                // لو تم تغير العمله لغير افتراضيه نعدل العمله الاولى تبقا هي الافتراضيه id=1
                var updateData = await currencyRepositoryQuery.GetByAsync(q => q.Id == parameter.Id);
                if (parameter.isDefault != updateData.IsDefault)
                {
                    var defaultCurrency = currencyRepositoryQuery.TableNoTracking.Where(a => a.Id != parameter.Id).ToList();

                    defaultCurrency.Select(a => { a.IsDefault = false; return a; }).ToList();
                    if (!parameter.isDefault)
                        defaultCurrency.Where(a => a.Id == 1).Select(a => { a.IsDefault = true; return a; }).ToList();

                     currencyRepositoryCommand.UpdateAsyn(defaultCurrency);
                    currencyRepositoryCommand.SaveChanges();
                }
                // updateData.LastUpdate = DateTime.Now;
                var table = Mapping.Mapper.Map<UpdateCurrencyParameter, GLCurrency>(parameter, updateData);

                 currencyRepositoryCommand.UpdateAsyn(table);
               // currencyRepositoryCommand.SaveChanges();

                HistoryCurrency(0, table.Status, table.ArabicName, table.LatinName, table.Notes, table.Id, table.BrowserName, "U", null);
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editCurrancy);
                return new ResponseResult() { Data = table, Id = table.Id, Result = Result.Success, Note = Aliases.Actions.UpdatedSuccessfully };
            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex, Note = Aliases.Actions.ExceptionOccurred };
            }
        }
        public async Task<ResponseResult> GetCurrencyById(int Id)
        {
            try
            {
                var currency = await currencyRepositoryQuery.GetAsync(Id);
                if (currency != null)
                {
                    return new ResponseResult() { Data = currency, Result = Result.Success, Note = Aliases.Actions.Success };
                }
                return new ResponseResult() { Data = currency, Result = Result.Success, Note = Aliases.Actions.NotFound };
            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex, Result = Result.Failed, Note = Aliases.Actions.ExceptionOccurred };

            }
        }
        public async Task<ResponseResult> DeleteCurrencyAsync(SharedRequestDTOs.Delete parameter)
        {
            try
            {
                var test = await currencyRepositoryQuery.GetAllIncludingAsync(0, 0,
                                                            a => parameter.Ids.Contains(a.Id),
                                                            w => w.financialAccounts);
                int count = test.Count();
                List<int> deletedItems = new List<int>();
                foreach (var item in test)
                {
                    if (item.Id != 1)
                    {
                        if (item.financialAccounts.Count == 0 && item.IsDefault == false)
                        {
                            var result = await currencyRepositoryCommand.DeleteAsync(item.Id);
                            if (result)
                            {
                                deletedItems.Add(item.Id);
                            }
                        }
                    }
                }
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteCurrancy,count);
                return new ResponseResult() { Data = deletedItems, Result = deletedItems.Any() ? Result.Success : Result.Failed };

            }
            catch (Exception ex)
            {

                return new ResponseResult() { Data = ex, Result = Result.Failed, Note = Aliases.Actions.ExceptionOccurred };

            }

            #region Old code
            //try
            //{
            //    var lis = new List<ListOfCurrency>();
            //    foreach (var item2 in parameter.Ids)
            //    {
            //        var support = await financialAccountRepositoryQuery.GetByAsync(q => q.CurrencyId == item2);
            //        if (support != null)
            //        {
            //            var currency = await currencyRepositoryQuery.GetByAsync(q => q.Id == support.CurrencyId);
            //            var list = new ListOfCurrency();
            //            list.Name = currency?.ArabicName;
            //            lis.Add(list);
            //        }
            //    }
            //    if (lis.Count() < parameter.Ids.Count())
            //    {
            //        foreach (var item in parameter.Ids)
            //        {
            //            var currencyDeleted = await currencyRepositoryQuery.GetByAsync(q => q.Id == item);
            //            var support = await financialAccountRepositoryQuery.GetByAsync(q => q.CurrencyId == item);

            //            if (support == null)
            //            {
            //                currencyRepositoryCommand.Remove(currencyDeleted);
            //            }
            //        }
            //        await currencyRepositoryCommand.SaveAsync();
            //        if (lis.Count() == 0)
            //        {
            //            return new ResponseResult() { Result = Result.Success, Note = Aliases.Actions.DeletedSuccessfully };

            //        }
            //        else
            //        {
            //            return new ResponseResult() { Data = lis, Result = Result.CanNotBeDeleted, Note = Aliases.Actions.CanNotBeDeleted };
            //        }
            //    }
            //    else
            //    {
            //        return new ResponseResult() { Data = lis, Result = Result.CanNotBeDeleted, Note = Aliases.Actions.CanNotBeDeleted };

            //    }

            //    return new ResponseResult() { Result = Result.Success, Note = Aliases.Actions.DeletedSuccessfully };
            //}
            //catch (Exception ex)
            //{
            //    return new ResponseResult() { Data = ex, Result = Result.Failed, Note = Aliases.Actions.ExceptionOccurred };

            //} 
            #endregion
        }
        public async Task<ResponseResult> GetAllCurrencyData(currencyRequest parameters)
        {
            try
            {
                var resData = await currencyRepositoryQuery.GetAllIncludingAsync(0, 0,
                a => ((a.Code.ToString().Contains(parameters.SearchCriteria) || string.IsNullOrEmpty(parameters.SearchCriteria)
                || a.ArabicName.Contains(parameters.SearchCriteria) || a.LatinName.Contains(parameters.SearchCriteria))
                && (parameters.Status == 0 || a.Status == parameters.Status))
                , e => (string.IsNullOrEmpty(parameters.SearchCriteria) ?
                e.OrderByDescending(q => q.Code) :
                e.OrderBy(a => (a.Code.ToString().Contains(parameters.SearchCriteria)) ? 0 : 1)),
                w => w.financialAccounts);

                //resData.ToList().ForEach(c =>
                //{
                //    if (c.Id == 1)
                //    {
                //        c.CanDelete = false;
                //    }
                //    else if (c.financialAccounts.Count == 0)
                //    {
                //        c.CanDelete = true;
                //    }
                //    c.financialAccounts = null;
                //});

                resData.Where(a=> a.Id != 1 && a.financialAccounts.Count() ==0 && !a.IsDefault ).Select(a => { a.CanDelete = true; return a; }).ToList();
                resData.Select(a => { a.financialAccounts = null; return a; }).ToList();
                var count = resData.Count();

                if (parameters.PageSize > 0 && parameters.PageNumber > 0)
                {
                    resData = resData.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToList();
                }
                else
                {
                    return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };

                }

                return new ResponseResult() { Data = resData, DataCount = count, Id = null, Result = resData.Any() ? Result.Success : Result.Failed };

            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex, Result = Result.Failed, Note = Aliases.Actions.ExceptionOccurred };
            }




            #region Old code
            //try
            //{
            //    var searchCretiera = parameters.SearchCriteria;

            //    var treeData = currencyRepositoryQuery
            //   .FindQueryable(x =>
            //   (searchCretiera == string.Empty || searchCretiera == null ||
            //   x.ArabicName.ToLower().Contains(searchCretiera) || x.LatinName.ToLower().Contains(searchCretiera) ||
            //   x.Code.ToString().Contains(searchCretiera))).ToList();

            //    if (parameters.Status > 0)
            //    {
            //        if (parameters.Status < (int)Status.Active || parameters.Status > (int)Status.Inactive)
            //            return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };

            //        treeData = treeData.Where(q => q.Status == parameters.Status).ToList();
            //    }

            //    treeData = (string.IsNullOrEmpty(parameters.SearchCriteria) ?
            //        treeData.OrderByDescending(q => q.Code) :
            //        treeData.OrderBy(a => (a.Code.ToString().Contains(parameters.SearchCriteria)) ? 0 : 1)).ToList();
            //    int count = treeData.Count();
            //    if (parameters.PageSize > 0 && parameters.PageNumber > 0)
            //    {
            //        treeData = treeData.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToList();
            //    }
            //    else
            //    {
            //        return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };

            //    }

            //    return new ResponseResult() { Data = treeData, Result = (treeData.Count() > 0 ? Result.Success : Result.NoDataFound),
            //        DataCount = count, Note = Aliases.Actions.Success };
            //}
            //catch (Exception ex)
            //{
            //    return new ResponseResult() { Data = ex, Result = Result.Failed, Note = Aliases.Actions.ExceptionOccurred };
            //} 
            #endregion
        }
        public async Task<ResponseResult> GetAllCurrencyDataDropDown()
        {
            try
            {
                var treeData = currencyRepositoryQuery.GetAll().ToList();
                var list = Mapping.Mapper.Map<List<GLCurrency>, List<CurrencyDto>>(treeData.ToList());
                list.Where(q => q.Status == 1).ToList().ForEach(q => q.IsDefault = true);
                return new ResponseResult() { Data = list, Result = Result.Success, Note = "Ok" };
            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex, Result = Result.Failed, Note = "Exception occurred" };

            }
        }
        public async Task<ResponseResult> UpdateCurrencyFactor(UpdateCurrencyFactorList parameter)
        {
            try
            {
                foreach (var item in parameter.updateCurrencyFactorParameters)
                {
                    var updateData = await currencyRepositoryQuery.GetByAsync(q => q.Id == item.Id);

                    updateData.Factor = item.Factor;
                    // updateData.LastUpdate = DateTime.Now.ToString();
                    await currencyRepositoryCommand.UpdateAsyn(updateData);
                }
                // }
                return new ResponseResult() { Result = Result.Success, Note = Aliases.Actions.UpdatedSuccessfully };
            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex, Result = Result.Failed, Note = Aliases.Actions.ExceptionOccurred };
            }
        }

        public async Task<ResponseResult> UpdateCurrencyStatus(SharedRequestDTOs.UpdateStatus parameter)
        {
            try
            {
                if (parameter.Status < (int)Status.Active || parameter.Status > (int)Status.Inactive)
                {
                    return new ResponseResult { Result = Result.Failed, Note = Aliases.Actions.InvalidStatus };
                }
                foreach (var item in parameter.Id)
                {
                    var currency = await currencyRepositoryQuery.GetByAsync(q => q.Id == item);
                    if (currency.Id == 1 || currency.IsDefault)
                    {
                        currency.Status = (int)Status.Active;
                    }
                    else
                    {
                        currency.Status = parameter.Status;
                    }
                    await currencyRepositoryCommand.UpdateAsyn(currency);
                }
                return new ResponseResult { Result = Result.Success, Note = Aliases.Actions.UpdatedSuccessfully };
            }
            catch (Exception ex)
            {
                return new ResponseResult { Data = ex, Note = Aliases.Actions.ExceptionOccurred };
            }

        }

        public async void HistoryCurrency(int branchId, int status, string nameAr, string nameEn, string notes, int currencyId,
                                         string browserName, string lastTransactionAction, string LastTransactionUser)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var history = new GLCurrencyHistory()
            {
                employeesId=userInfo.employeeId,
                LastDate = DateTime.Now,
                LastAction = lastTransactionAction,
                //LastAction = lastTransactionAction,
                BranchId = 0,
                LastTransactionUser = userInfo.employeeNameEn.ToString(),
                CuerrncyId = currencyId,
                Status = status,
                ArabicName = nameAr,
                LatinName = nameEn,
                Notes = notes,
                BrowserName = browserName,
                LastTransactionUserAr = userInfo.employeeNameAr.ToString()
            };
            currencyHistoryRepositoryCommand.Add(history);
           // currencyHistoryRepositoryCommand.SaveChanges();
        }
        public async Task<ResponseResult> GetAllCurrencyHistory(int CurrencyId)
        {
            var parentDatasQuey = currencyHistoryRepositoryQuery.FindQueryable(s => s.CuerrncyId == CurrencyId).Include(s=>s.employees);
            var historyList = new List<HistoryResponceDto>();
            foreach (var item in parentDatasQuey.ToList())
            {
                var historyDto = new HistoryResponceDto();
                historyDto.Date = item.LastDate;
                historyDto.ArabicName = item.employees.ArabicName;
                historyDto.LatinName = item.employees.LatinName;
                if (item.LastAction == "U")
                {
                    historyDto.TransactionTypeAr = "تعديل";
                    
                }
                if (item.LastAction == "A")
                {
                    historyDto.TransactionTypeAr = "اضافة";
                }
               // historyDto.userNameAr = item.LastTransactionUser;

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

    }
}
