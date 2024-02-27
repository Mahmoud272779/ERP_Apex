using App.Application.Helpers.Service_helper.History;
using App.Application.Services.Process.GeneralServices.DeletedRecords;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Domain.Entities.Process.General_Ledger;
using App.Domain.Entities.Process.Store;
using App.Domain.Models.Request.General;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Org.BouncyCastle.Utilities.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aliases = App.Application.Helpers.Aliases;

namespace App.Application.Services.Process.GLServices.Prnters
{
    public class PrinterService : IPrinterService
    {
        private readonly IRepositoryQuery<GLPrinter> printerRepositoryQuery;
        private readonly IRepositoryCommand<GLPrinter> printerRepositoryCommand;
        private readonly IHttpContextAccessor httpContext;
        private readonly IHistory<GLPrinterHistory> history;
        private readonly ISystemHistoryLogsService systemHistoryLogsService;
        private readonly IDeletedRecordsServices deletedRecords;
        private readonly IGeneralAPIsService generalAPIsService;

        public PrinterService(IRepositoryQuery<GLPrinter> printerRepositoryQuery,
                              IRepositoryCommand<GLPrinter> printerRepositoryCommand,
                              IHttpContextAccessor httpContext,
                              IHistory<GLPrinterHistory> History,
                              ISystemHistoryLogsService systemHistoryLogsService,
                              IDeletedRecordsServices deletedRecords,
                              IGeneralAPIsService generalAPIsService) {
            this.printerRepositoryQuery = printerRepositoryQuery;
            this.printerRepositoryCommand = printerRepositoryCommand;
            this.httpContext = httpContext;
            history = History;
            this.systemHistoryLogsService = systemHistoryLogsService;
            this.deletedRecords = deletedRecords;
            this.generalAPIsService = generalAPIsService;
        }


        public async Task<ResponseResult> AddPrinter(PrinterRequestsDTOs.Add parameter)
        {
            try
            {
                parameter.LatinName = Helpers.Helpers.IsNullString(parameter.LatinName);
                parameter.ArabicName = Helpers.Helpers.IsNullString(parameter.ArabicName);

                if (string.IsNullOrEmpty(parameter.LatinName))
                    parameter.LatinName = parameter.ArabicName;
                if (string.IsNullOrEmpty(parameter.ArabicName))
                    return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };

                var PrinterExist = await printerRepositoryQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName && a.BranchId == parameter.BranchId && !string.IsNullOrEmpty(parameter.ArabicName));
                if (PrinterExist != null)
                    return new ResponseResult { Result = Result.Exist, Note = Actions.ArabicNameExist };
                PrinterExist = await printerRepositoryQuery.GetByAsync(a => a.LatinName == parameter.LatinName && a.BranchId == parameter.BranchId && !string.IsNullOrEmpty(parameter.LatinName));
                if (PrinterExist != null)
                    return new ResponseResult { Result = Result.Exist, Note = Actions.EnglishNameExist };
                
                string pattern = @"^(?!0{1,3}\.)(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
                Regex regex = new Regex(pattern);

                if (regex.IsMatch(parameter.IP))
                {
                    var IP = await printerRepositoryQuery.GetByAsync(a => a.IP == parameter.IP && !string.IsNullOrEmpty(parameter.IP) && a.BranchId == parameter.BranchId);

                    if (IP != null)
                        return new ResponseResult() { Data = null, Id = IP.Id, Result = Result.Exist, Note = Actions.IPExist };
                }
                else
                    return new ResponseResult() { Data = null, Result = Result.Failed, Note = Actions.invalidType };



                var table = Mapping.Mapper.Map<PrinterRequestsDTOs.Add, GLPrinter>(parameter);
                table.ArabicName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();

                table.Code = printerRepositoryQuery.GetMaxCode(a => a.Code) + 1;
                table.ArabicName = parameter.ArabicName;
                table.LatinName = parameter.LatinName;

                if (table.Id == 1)
                    table.Status = (int)Status.Active;
                else
                    table.Status = parameter.Status;

                table.BranchId = parameter.BranchId;
                table.Notes = parameter.Notes;
                table.IP = parameter.IP;
                table.CanDelete = true;

                table.UTime = DateTime.Now;

                var saved = printerRepositoryCommand.Add(table);

                history.AddHistory(table.Id, table.LatinName, table.ArabicName, Helpers.Aliases.HistoryActions.Add, Helpers.Aliases.TemporaryRequiredData.UserName);
                await systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addPrinter);

                return new ResponseResult() { Id = table.Id, Result = Result.Success, Note = Actions.SavedSuccessfully };
            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex, Note = Actions.ExceptionOccurred };
            }
        }
        public async Task<ResponseResult> GetPrinterById(int Id)
        {
            try
            {
                var data = await printerRepositoryQuery.GetAsync(Id);
                if (data != null)
                {
                    return new ResponseResult() { Data = data, Result = Result.Success, Note = Actions.Success };
                }
                return new ResponseResult() { Data = null, Result = Result.NotFound, Note = Actions.NotFound };
            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex, Note = Actions.ExceptionOccurred };

            }
        }
        public async Task<ResponseResult> GetAllPrinterByBranchDataDropDown(int BranchId)
        {
            ResponseResult responseResult = new ResponseResult();
            await Task.Run(() =>
            {
                var data = printerRepositoryQuery.TableNoTracking
                .Where(p => p.BranchId == BranchId && p.Status == (int)Status.Active);

                responseResult.Data = data;
                responseResult.Result = data.Any() ? Result.Success : Result.Failed;
            });
            return responseResult;
        }
        public async Task<ResponseResult> UpdatePrinter(PrinterRequestsDTOs.Update parameter)
        {
            try
            {
                if (parameter.Id == 0)
                    return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

                parameter.LatinName = Helpers.Helpers.IsNullString(parameter.LatinName);
                parameter.ArabicName = Helpers.Helpers.IsNullString(parameter.ArabicName);

                if (string.IsNullOrEmpty(parameter.LatinName))
                    parameter.LatinName = parameter.ArabicName;
                if (string.IsNullOrEmpty(parameter.ArabicName))
                    return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };

                var PrinterExist = await printerRepositoryQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName && a.BranchId == parameter.BranchId && !string.IsNullOrEmpty(parameter.ArabicName) && a.Id != parameter.Id );
                if (PrinterExist != null)
                    return new ResponseResult { Result = Result.Exist, Note = Actions.ArabicNameExist };
                PrinterExist = await printerRepositoryQuery.GetByAsync(a => a.LatinName == parameter.LatinName && a.BranchId == parameter.BranchId && !string.IsNullOrEmpty(parameter.LatinName) && a.Id != parameter.Id);
                if (PrinterExist != null)
                    return new ResponseResult { Result = Result.Exist, Note = Actions.EnglishNameExist };

                string pattern = @"^(?!0{1,3}\.)(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
                Regex regex = new Regex(pattern);

                if (regex.IsMatch(parameter.IP))
                {
                    var IP = await printerRepositoryQuery.GetByAsync(a => a.IP == parameter.IP && !string.IsNullOrEmpty(parameter.IP) && a.BranchId == parameter.BranchId &&  a.Id != parameter.Id);

                    if (IP != null)
                        return new ResponseResult() { Data = null, Id = IP.Id, Result = Result.Exist, Note = Actions.IPExist };
                }
                else
                    return new ResponseResult() { Data = null, Result = Result.Failed, Note = Actions.invalidType };



                var updateData = await printerRepositoryQuery.GetByAsync(q => q.Id == parameter.Id);

                var table = Mapping.Mapper.Map<PrinterRequestsDTOs.Update, GLPrinter>(parameter, updateData);

                table.ArabicName = parameter.ArabicName;
                table.LatinName = parameter.LatinName;

                if (table.Id == 1)
                    table.Status = (int)Status.Active;
                else
                    table.Status = parameter.Status;
                table.BranchId = parameter.BranchId;
                table.Notes = parameter.Notes;
                table.IP = parameter.IP;
                table.CanDelete = true;
                table.UTime = DateTime.Now;
                await printerRepositoryCommand.UpdateAsyn(table);

                history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);
                await systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editBranch);
                return new ResponseResult() { Id = table.Id, Result = Result.Success, Note = Actions.UpdatedSuccessfully };
            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex, Note = Actions.ExceptionOccurred };
            }
        }
        public async Task<ResponseResult> DeletePrinter(SharedRequestDTOs.Delete parameter)
        {
            try
            {
                //var lis = new List<ListOfBranches>();

                var printers = printerRepositoryQuery.FindAll(e => parameter.Ids.Contains(e.Id)).ToHashSet();

                printerRepositoryCommand.RemoveRange(printers);
                
                await printerRepositoryCommand.SaveAsync();

                if (printers.Count() == 0)
                    return new ResponseResult() { Data = null, Id = null, Result = Result.CanNotBeDeleted, Note = Actions.CanNotBeDeleted };

                //Fill The DeletedRecordTable
                var Ids = printers.Select(a => a.Id);
                deletedRecords.SetDeletedRecord(Ids.ToList(), 12);

                await systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deletePrinter);
                return new ResponseResult() { Data = null, Id = null, Result = Result.Success, Note = Actions.DeletedSuccessfully };

            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex, Result = Result.Success, Note = Actions.ExceptionOccurred };
            }
        }
        public async Task<ResponseResult> GetAllPrinterHistory(int PrinterId)
        {
            return await history.GetHistory(a => a.EntityId == PrinterId);
        }
        public async Task<ResponseResult> GetPrinterByDate(DateTime date, int PageNumber, int PageSize)
        {
            try
            {
                var resData = await printerRepositoryQuery.TableNoTracking.Where(q => q.UTime >= date).ToListAsync();

                return await generalAPIsService.Pagination(resData, PageNumber, PageSize);

            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.NotFound };


            }
        }

        public async Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameter)
        {
            if (parameter.Id.Count() == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            if (parameter.Status < (int)Status.Active || parameter.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }
            var categories = printerRepositoryQuery.TableNoTracking.Where(e => parameter.Id.Contains(e.Id));
            var CategoryList = categories.ToList();

            CategoryList.Select(e => { e.Status = parameter.Status; return e; }).ToList();
            if (parameter.Id.Contains(1))
                CategoryList.Where(q => q.Id == 1).Select(e => { e.Status = (int)Status.Active; return e; }).ToList();


            var result = await printerRepositoryCommand.UpdateAsyn(CategoryList);

            foreach (var Category in CategoryList)
            {
                history.AddHistory(Category.Id, Category.LatinName, Category.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);

            }
            await systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editItemCategories);
            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };
        }

        public async Task<ResponseResult> GetAllPrinterData(PrinterRequestsDTOs.Search parameters , bool isPrint = false)
        {
            try
            {
                //var userInfo = await _userinformation.GetUserInformation();
                var res = Enumerable.Empty<GLPrinter>().AsQueryable();

                if (parameters.IsSearchData)
                {
                    res = printerRepositoryQuery.TableNoTracking
                        .Where(a => parameters.PrinterId > 0 ? (a.Id == parameters.PrinterId) : true)
                        .Where(a => !string.IsNullOrEmpty(parameters.SearchCriteria) ? (a.Code.ToString().Contains(parameters.SearchCriteria) || a.ArabicName.Contains(parameters.SearchCriteria) || a.LatinName.Contains(parameters.SearchCriteria)) : true)
                        .Where(a => parameters.Status != 0 ? a.Status == parameters.Status : true);

                }
                else
                {
                    if (parameters.Ids != null)
                    {
                        string[] ids = parameters.Ids.Split(",");
                        foreach (string id in ids)
                        {
                            var item = printerRepositoryQuery.TableNoTracking
                                .Where(a => (Convert.ToInt32(id) > 0 ? a.Id == Convert.ToInt32(id) : false)).FirstOrDefault();
                            res = res.Append(item);
                        }

                    }
                    else
                        return new ResponseResult { Result = Result.ReleatedData };
                }
                res = (string.IsNullOrEmpty(parameters.SearchCriteria) ? res.OrderByDescending(q => q.Code) : res.OrderBy(a => (a.Code.ToString().Contains(parameters.SearchCriteria)) ? 0 : 1));
                var result = res.ToList();
                var count = result.Count();


                result = isPrint ? res.ToList() : res.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToList();

                //Paggination

                if (parameters.PageNumber <= 0 && parameters.PageNumber <= 0 && isPrint == false)
                {
                    return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };
                }
                result.Where(a => a.Id != 1).Select(a => { a.CanDelete = true; return a; }).ToList();

                return new ResponseResult() { Data = result, Result = (count > 0 ? Result.Success : Result.NoDataFound), DataCount = count, Note = "Ok" };
            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex, Note = Actions.ExceptionOccurred };
            }
            throw new NotImplementedException();


        }


    }
}
