using App.Application.Basic_Process;
using App.Application.Helpers;
using App.Application.Services.Process.GeneralServices.SystemHistoryLogsServices;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using App.Infrastructure.Pagination;
using Attendleave.Erp.Core.APIUtilities;
using Attendleave.Erp.ServiceLayer.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.CostCenters
{
    public class CostCentersBusiness : BusinessBase<GLCostCenter>, ICostCentersBusiness
    {
        private readonly IRepositoryQuery<GLCostCenter> costCenterRepositoryQuery;
        private readonly IRepositoryQuery<GLFinancialCost> financialCostRepositoryQuery;
        private readonly IRepositoryCommand<GLFinancialCost> financialCostRepositoryCommand;
        private readonly IRepositoryCommand<GLCostCenter> costCenterRepositoryCommand;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IRepositoryCommand<GLCostCenterHistory> costCenterHistoryRepositoryCommand;
        private readonly IRepositoryQuery<GLCostCenterHistory> costCenterHistoryRepositoryQuery;
        private readonly IPagedList<CostCenterDto, CostCenterDto> pagedListCostCenter;
        private readonly IRepositoryQuery<GLJournalEntryDetails> JournalEntryDetailsQuery;
        private readonly IRepositoryQuery<GLRecieptCostCenter> recieptCostCenterQuery;
        private readonly IRepositoryQuery<GLCurrency> _GLCurrencyQuery;

        private readonly IHttpContextAccessor httpContext;
        private readonly IPagedList<GenaricCostCenterDto, GenaricCostCenterDto> pagedListGenaricCostCenterDto;
        private readonly iUserInformation _iUserInformation;

        public CostCentersBusiness(
             IRepositoryCommand<GLFinancialCost> financialCostRepositoryCommand,
            IRepositoryQuery<GLCostCenter> CostCenterRepositoryQuery,
            IPagedList<CostCenterDto, CostCenterDto> PagedListCostCenter,
            IHttpContextAccessor HttpContext,
             IRepositoryQuery<GLFinancialCost> FinancialCostRepositoryQuery,
             IRepositoryQuery<GLCostCenterHistory> CostCenterHistoryRepositoryQuery,
             IRepositoryCommand<GLCostCenterHistory> CostCenterHistoryRepositoryCommand,
            IRepositoryCommand<GLCostCenter> CostCenterRepositoryCommand,
            ISystemHistoryLogsService systemHistoryLogsService,
           IPagedList<GenaricCostCenterDto, GenaricCostCenterDto> PagedListGenaricCostCenterDto,
           IRepositoryQuery<GLJournalEntryDetails> JournalEntryDetailsQuery,
           IRepositoryQuery<GLJournalEntryDetails> JournalEntryDetailsQueryForDeleteParent,
           IRepositoryQuery<GLCostCenter> costCenterRepositoryQueryNew,
        IRepositoryActionResult repositoryActionResult,
        IRepositoryQuery<GLRecieptCostCenter> recieptCostCenterQuery, iUserInformation iUserInformation, IRepositoryQuery<GLCurrency> gLCurrencyQuery) : base(repositoryActionResult)
        {
            this.financialCostRepositoryCommand = financialCostRepositoryCommand;
            costCenterRepositoryQuery = CostCenterRepositoryQuery;
            pagedListCostCenter = PagedListCostCenter;
            costCenterRepositoryCommand = CostCenterRepositoryCommand;
            _systemHistoryLogsService = systemHistoryLogsService;
            httpContext = HttpContext;
            costCenterHistoryRepositoryQuery = CostCenterHistoryRepositoryQuery;
            financialCostRepositoryQuery = FinancialCostRepositoryQuery;
            costCenterHistoryRepositoryCommand = CostCenterHistoryRepositoryCommand;
            pagedListGenaricCostCenterDto = PagedListGenaricCostCenterDto;
            this.JournalEntryDetailsQuery = JournalEntryDetailsQuery;
            this.recieptCostCenterQuery = recieptCostCenterQuery;
            _iUserInformation = iUserInformation;
            _GLCurrencyQuery = gLCurrencyQuery;
        }
        public async Task<string> AddAutomaticCode()
        {
            var code = costCenterRepositoryQuery.FindQueryable(q => q.Id > 0);
            if (code.Count() > 0)
            {
                var code2 = costCenterRepositoryQuery.FindQueryable(q => q.Id > 0).ToList().Last();
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
        public async Task<IRepositoryActionResult> AddCostCenter(CostCenterParameter parameter)
        {
            try
            {
                if (string.IsNullOrEmpty(parameter.LatinName))
                    parameter.LatinName = parameter.ArabicName;
                var table = Mapping.Mapper.Map<CostCenterParameter, GLCostCenter>(parameter);
                table.BrowserName = contextHelper.GetBrowserName(httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString());
                table.LastTransactionAction = "A";
                table.Code = await AddAutomaticCode();
                costCenterRepositoryCommand.Add(table);
                GenerateAutoCodeOfSerialOfChild(parameter.ParentId, table.Id);
                HistoryCostCenter(table.Notes, table.ParentId, table.BrowserName, table.LastTransactionAction, table.AddTransactionUser, table.Id
            , table.ArabicName, table.LatinName, table.BranchId, table.CC_Nature);
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addCostCenter);
                return repositoryActionResult.GetRepositoryActionResult(table.Id, RepositoryActionStatus.Created, message: "Saved Successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex, RepositoryActionStatus.Created, message: "Saved Successfully");
            }
        }

        public async Task GenerateAutoCodeOfSerialOfChild(int? parent, int id)
        {
            var data = costCenterRepositoryQuery.TableNoTracking.Where(a => a.Id == id).First();

            if (parent == null)
                data.AutoCode = id.ToString();
            else
            {
                var autoCodeOfParent = costCenterRepositoryQuery.TableNoTracking.Where(a => a.Id == parent)
                        .Select(a => a.AutoCode).First();

                data.AutoCode = autoCodeOfParent + "." + id.ToString();

            }

            costCenterRepositoryCommand.UpdateAsyn(data);
        }
        public async Task<IRepositoryActionResult> UpdateCostCenter(UpdateCostCenterParameter parameter)
        {
            try
            {
                // Ga3foor said : if cost center has a child make it main.
                // By Alaa



                if (string.IsNullOrEmpty(parameter.LatinName))
                    parameter.LatinName = parameter.ArabicName;
                if (parameter.ParentId == parameter.Id)
                    return repositoryActionResult.GetRepositoryActionResult(parameter.Id, RepositoryActionStatus.Error, message: "can't select the node as a parent ");
                if (parameter.InitialBalance == null)
                    parameter.InitialBalance = 0;
                var costCenters = costCenterRepositoryQuery.TableNoTracking;
                var updateData = costCenters.Where(a => a.Id == parameter.Id)
                    .Include(a => a.costCenters).ToList().First();

                // edit from sub to main check that coscenter not used before
                if (parameter.Type == (int)CostCenterType.Main && updateData.Type == (int)CostCenterType.Sub)
                {
                    var costCenterUsed = JournalEntryDetailsQuery.TableNoTracking.Where(a => a.CostCenterId == parameter.Id);
                    if (costCenterUsed.Count() > 0)
                        return repositoryActionResult.GetRepositoryActionResult(parameter.Id, RepositoryActionStatus.Error, message: "can't updated , cost center is used");

                }
                var ListOfChildren = costCenterRepositoryQuery.TableNoTracking
                    .Where(a => a.AutoCode.StartsWith(updateData.AutoCode) && a.Id != parameter.Id)
                           .OrderBy(a => a.Type).ThenByDescending(a => a.Id);

                //  var costCenterHasChilds = GetAllChildren(updateData);
                if (ListOfChildren.Count() > 0 && parameter.Type == (int)CostCenterType.Sub && updateData.Type == (int)CostCenterType.Main)
                    return repositoryActionResult.GetRepositoryActionResult(parameter.Id, RepositoryActionStatus.Error, message: "can't updated , cost center has children");

                var costCenterType = updateData.Type;
                var table = Mapping.Mapper.Map<UpdateCostCenterParameter, GLCostCenter>(parameter, updateData);
                if (costCenterType == 2)
                {
                    var isCostCenterHasChilders = costCenters.Where(x => x.ParentId == updateData.Id).Any();
                    if (isCostCenterHasChilders)
                        table.Type = 2;
                }
                if(costCenterType == 1) 
                {
                    var isCostCenterHasJournalEntries = JournalEntryDetailsQuery.TableNoTracking.Where(x => x.CostCenterId == table.Id).Any();
                    if(isCostCenterHasJournalEntries)
                        table.Type = 1;
                }

                table.BrowserName = contextHelper.GetBrowserName(httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString());
                await costCenterRepositoryCommand.UpdateAsyn(table);
                GenerateAutoCodeOfSerialOfChild(updateData.ParentId, table.Id);

                HistoryCostCenter(table.Notes, table.ParentId, table.BrowserName, table.LastTransactionAction, table.AddTransactionUser, table.Id
               , table.ArabicName, table.LatinName, table.BranchId, table.CC_Nature);
                costCenterRepositoryCommand.SaveChanges();
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editCostCenter);
                return repositoryActionResult.GetRepositoryActionResult(table.Id, RepositoryActionStatus.Updated, message: "Updated Successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);
            }
        }

        public async Task<Tuple<List<string>, List<GLCostCenter>>> DeleteCostCenter(int id_)
        {
            var parent = costCenterRepositoryQuery.TableNoTracking.Where(a => a.Id == id_).ToList();
            var costCenterUsed = new List<string>();
            var childWillDelete = new List<GLCostCenter>();
            if (parent.Count() == 0)
            {
                return Tuple.Create(costCenterUsed, childWillDelete);
            }
            IList<GLCostCenter> ListOfChildren = await costCenterRepositoryQuery.TableNoTracking.Where(a => a.AutoCode == parent.First().AutoCode
                              || a.AutoCode.StartsWith(parent.First().AutoCode))
                              .OrderByDescending(a => a.Id).ToListAsync();


            if (ListOfChildren.Count() > 0)
            {
                var IdsOfChildren = ListOfChildren.Where(a => a.Type == (int)CostCenterType.Sub).Select(a => a.Id).ToList();
                var costCenterUsedInJournalentry = JournalEntryDetailsQuery.TableNoTracking.Where(a => IdsOfChildren.Contains(a.CostCenterId.Value))
                      .Select(a => a.CostCenterId).ToList();

                var costCenterUsedInReciepts = recieptCostCenterQuery.TableNoTracking.Where(a => IdsOfChildren.Contains(a.CostCenterId))
                      .Select(a => a.CostCenterId).ToList();

                if (costCenterUsedInJournalentry.Count() > 0)
                {
                    costCenterUsed.AddRange(ListOfChildren.Where(a => costCenterUsedInJournalentry.Contains(a.Id)).Select(a => a.ArabicName).ToList());
                }
                else if (costCenterUsedInReciepts.Count() > 0)
                {
                    costCenterUsed.AddRange(ListOfChildren.Where(a => costCenterUsedInReciepts.Contains(a.Id)).Select(a => a.ArabicName).ToList());

                }
                else
                {
                    try
                    {
                        childWillDelete.AddRange(ListOfChildren);

                    }
                    catch (Exception e)
                    {

                        throw;
                    }
                }


            }

            return Tuple.Create(costCenterUsed, childWillDelete); ;
        }
        public async Task<ResponseResult> DeleteCostCenterAsync(SharedRequestDTOs.Delete parameter)
        {

            var costCenterListUsed = new List<string>();
            var costCenterWillDelete = new List<GLCostCenter>();
            var nameOfCostCenterUsed = "";
            foreach (var id in parameter.Ids)
            {
                var result = DeleteCostCenter(id);
                var costCenterUsed = result.Result.Item1;
                var childWillDelete = result.Result.Item2;
                if (costCenterUsed.Count() > 0)
                {
                    costCenterListUsed.AddRange(costCenterUsed);

                }


                costCenterWillDelete.AddRange(childWillDelete);

            }

            if (costCenterWillDelete.Count() > 0)
            {
                costCenterRepositoryCommand.RemoveRange(costCenterWillDelete);
                await costCenterRepositoryCommand.SaveAsync();
                var IdsOfChildren = costCenterWillDelete.Where(a => a.Type == (int)CostCenterType.Sub).Select(a => a.Id).ToList();
                financialCostRepositoryCommand.DeleteAsync(a => IdsOfChildren.Contains(a.CostCenterId));

            }
            if (costCenterListUsed.Count() > 0)
            {
                nameOfCostCenterUsed = String.Join(", ", costCenterListUsed);
                return new ResponseResult { Data = nameOfCostCenterUsed, Result = Result.CanNotBeDeleted, ErrorMessageAr = "لا يمكن الحذف بعض مراكز التكلفه مستخدمة", ErrorMessageEn = "Can not be deleted ,Some cost centers are used" };

            }
            else
            {
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteCostCenter);
                return new ResponseResult { Result = Result.Success, ErrorMessageAr = "تم الحذف بنجاح", ErrorMessageEn = "Deleted successfully" };
            }

        }


        public async Task<IRepositoryActionResult> GetCostCenterById(int Id)
        {
            try
            {
                var costCenterData = await costCenterRepositoryQuery.GetAsync(Id);
                return repositoryActionResult.GetRepositoryActionResult(costCenterData, RepositoryActionStatus.Ok, message: "Ok");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);

            }
        }
        public async Task<IRepositoryActionResult> GetAllCostCenterData(PageParameter paramters)
        {
            try
            {
                List<GenaricCostCenterDto> CostCenterParentsList = new List<GenaricCostCenterDto>();
                var parentDatas = costCenterRepositoryQuery.FindAll(s => s.ParentId == null && !string.IsNullOrEmpty(paramters.SearchCriteria) ? s.ArabicName.Contains(paramters.SearchCriteria) || s.LatinName.Contains(paramters.SearchCriteria) : true);
                if (parentDatas == null)
                {
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.NotFound, message: "Not Found");
                }
                else
                {
                    var defultCurrency = _GLCurrencyQuery.TableNoTracking.Where(c=> c.IsDefault).FirstOrDefault();
                    foreach (var parentData in parentDatas)
                    {
                        var CostCenterParents = new GenaricCostCenterDto()
                        {
                            Id = parentData.Id,
                            ArabicName = parentData.ArabicName,
                            LatinName = parentData.LatinName,
                            CC_Nature = parentData.CC_Nature,
                            Code = parentData.Code,
                            InitialBalance = parentData.InitialBalance,
                            Notes = parentData.Notes,
                            Type = parentData.Type,
                            ParentId = parentData.ParentId,
                            currencyAR = defultCurrency.AbbrAr,
                            currencyEn = defultCurrency.AbbrEn
                        };
                        var firestChileds = await costCenterRepositoryQuery.FindAllAsync(s => s.ParentId == parentData.Id);
                        var r = await GetUserChildRoles(firestChileds.ToList());
                        CostCenterParents.GenaricCostCenterDtos = r.Select(q => new GenaricCostCenterDto2
                        {
                            Id = q.Id,
                            ArabicName = q.ArabicName,
                            LatinName = q.LatinName,
                            CC_Nature = q.CC_Nature,
                            Code = q.Code,
                            InitialBalance = q.InitialBalance,
                            Notes = q.Notes,
                            Type = q.Type,
                            ParentId = q.ParentId
                        }).ToList();
                        CostCenterParentsList.Add(CostCenterParents);
                    }
                    var result = pagedListGenaricCostCenterDto.GetGenericPagination(CostCenterParentsList, paramters.PageNumber, paramters.PageSize, Mapper);
                    return repositoryActionResult.GetRepositoryActionResult(result, RepositoryActionStatus.Ok, message: "Ok");
                }
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);

            }
        }
        public async Task<IRepositoryActionResult> GetAllCostCenterDataWithOutPage()
        {
            try
            {
                List<GenaricCostCenterDto> CostCenterParentsList = new List<GenaricCostCenterDto>();

                var parentDatas = costCenterRepositoryQuery.FindAll(s => s.ParentId == null);

                if (parentDatas == null)
                {
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.NotFound, message: "Not Found");
                }
                else
                {
                    var allChildren = costCenterRepositoryQuery.FindAll(s => s.ParentId != null);

                    var costCenterInJournalEntry = JournalEntryDetailsQuery.TableNoTracking.Where(a => a.CostCenterId != null)
                        .Select(a => a.CostCenterId).ToList();
                    var allCostCenter = costCenterRepositoryQuery.TableNoTracking.ToList();
                    var costCenterUsed = allCostCenter.Where(a => costCenterInJournalEntry.Contains(a.Id))
                        .Select(a => a.AutoCode.Split('.')).ToList();

                    var costCenterUsed_ = new List<int>();
                    foreach (var costCenter in costCenterUsed)
                    {
                        int[] Ids = Array.ConvertAll(costCenter, s => int.Parse(s));
                        costCenterUsed_.AddRange(Ids);
                    }
                    var costCenterUsed_1 = costCenterUsed_.Distinct();

                    foreach (var parentData in parentDatas)
                    {
                        var childrenData = allChildren.Where(a => a.AutoCode.StartsWith(parentData.AutoCode)).ToList();
                        var parents = childrenData.Select(a => a.ParentId).ToList();
                        var defultCurrency = _GLCurrencyQuery.TableNoTracking.Where(c => c.IsDefault).FirstOrDefault();

                        var CostCenterParents = new GenaricCostCenterDto()
                        {
                            Id = parentData.Id,
                            ArabicName = parentData.ArabicName,
                            LatinName = parentData.LatinName,
                            CC_Nature = parentData.CC_Nature,
                            Code = parentData.Code,
                            InitialBalance = parentData.InitialBalance,
                            Notes = parentData.Notes,
                            Type = parentData.Type,
                            ParentId = parentData.ParentId,
                            CanEdit = !(costCenterUsed_1.Contains(parentData.Id) || parents.Contains(parentData.Id)),//!allCostCenter.Select(a => a.AutoCode).Any(s => parentData.AutoCode.StartsWith(s)),
                            CanDelete = !costCenterUsed_1.Contains(parentData.Id),
                            currencyAR = defultCurrency.AbbrAr,
                            currencyEn = defultCurrency.AbbrEn
                        };

                        CostCenterParents.GenaricCostCenterDtos = childrenData.Select(q => new GenaricCostCenterDto2
                        {
                            Id = q.Id,
                            ArabicName = q.ArabicName,
                            LatinName = q.LatinName,
                            CC_Nature = q.CC_Nature,
                            Code = q.Code,
                            InitialBalance = q.InitialBalance,
                            Notes = q.Notes,
                            Type = q.Type,
                            ParentId = q.ParentId,
                            CanEdit = !(costCenterUsed_1.Contains(q.Id) || parents.Contains(q.Id)), //!allCostCenter.Select(a => a.AutoCode).Any(s => q.AutoCode.StartsWith(s))),
                            CanDelete = !costCenterUsed_1.Contains(q.Id)
                        }).ToList();
                        CostCenterParentsList.Add(CostCenterParents);
                    }
                    //CostCenterParentsList.Select(a => { a.CanEdit = false; a.CanDelete = false; return a; })
                    //    .Where(a => costCenterUsed_1.Contains(a.Id)).ToList();
                    //CostCenterParentsList.Select(a=> { a.CanEdit = false; a.CanDelete = false; return a; })
                    //     .Where(a=> costCenterUsed_1.Contains(a.Id)).ToList();
                    return repositoryActionResult.GetRepositoryActionResult(CostCenterParentsList, RepositoryActionStatus.Ok, message: "Ok");
                }
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);

            }
        }
        public async Task<List<GLCostCenter>> GetUserChildRoles(List<GLCostCenter> firstchildIds)
        {
            List<GLCostCenter> userChildRoles = new List<GLCostCenter>();
            foreach (var item in firstchildIds)
            {
                var firstchild = await costCenterRepositoryQuery.SingleOrDefault(s => s.Id == item.Id
                         , includes: role1 => role1.costCenters
                        );
                //var ff = await costCenterRepositoryQuery.GetByAsync(s => s.Id == item);
                //var firstchild  =await costCenterRepositoryQuery.Get<CostCenter>( select:
                //  q=> new CostCenter()  , filter: s => s.Id == item
                //      // , includes: role1 => role1.costCenters
                //        );
                var rolesChilderenId = GetAllChildren(firstchild);
                userChildRoles.Add(firstchild);
                //// rolesChilderenId
                userChildRoles.AddRange(rolesChilderenId);
            }

            return userChildRoles;
        }
        public List<GLCostCenter> GetAllChildren(GLCostCenter parent)
        {
            List<GLCostCenter> children = new List<GLCostCenter>();
            PopulateChildren(parent, children);
            return children;
        }
        public void PopulateChildren(GLCostCenter parent, List<GLCostCenter> children)
        {
            List<GLCostCenter> myChildren;

            if (TryGetItschildren(parent, out myChildren))
            {
                children.AddRange(myChildren);
                foreach (var child in myChildren)
                {
                    var firstchild = costCenterRepositoryQuery.SingleOrDefault(s => s.Id == child.Id
                         , includes: role1 => role1.costCenters
                        ).Result;
                    PopulateChildren(firstchild, children);
                }
            }
        }
        public bool TryGetItschildren(GLCostCenter role, out List<GLCostCenter> list)
        {
            list = new List<GLCostCenter>();
            if (role.costCenters.Count() == 0)
            {
                return false;
            }
            else
            {
                foreach (var item in role.costCenters)
                {
                    list.Add(item);
                }

                return true;
            }
        }
        public async Task<IRepositoryActionResult> GetAllCostCenterDataDropDown(PageParameter paramters)
        {
            try
            {
                var parentData = costCenterRepositoryQuery.FindAll(a => a.Type == (int)CostCenterType.Sub).ToList();
                var list = Mapping.Mapper.Map<List<GLCostCenter>, List<CostCenterDto>>(parentData.ToList());

                var result = pagedListCostCenter.GetGenericPagination(list, paramters.PageNumber, paramters.PageSize, Mapper);
                return repositoryActionResult.GetRepositoryActionResult(result, RepositoryActionStatus.Ok, message: "Ok");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);
            }
        }
        public async void HistoryCostCenter(string notes, int? parentId, string browserName, string lastTransactionAction, string addTransactionUser, int costCenterId
            , string nameAr, string nameEn, int branchId, int cC_Nature)
        {
            var userInfo = await _iUserInformation.GetUserInformation();

            var history = new GLCostCenterHistory()
            {
                employeesId = userInfo.employeeId,
                LastDate = DateTime.Now,
                LastAction = lastTransactionAction,
                LastTransactionAction = lastTransactionAction,
                AddTransactionUser = addTransactionUser,
                LastTransactionUser = userInfo.employeeNameEn.ToString(),
                LastTransactionUserAr = userInfo.employeeNameAr.ToString(),
                CostCenterId = costCenterId,
                ArabicName = nameAr,
                LatinName = nameEn,
                BranchId = branchId,
                CC_Nature = cC_Nature,
                Notes = notes,
                ParentId = parentId,
                BrowserName = browserName,
            };
            costCenterHistoryRepositoryCommand.Add(history);
        }
        public async Task<ResponseResult> GetAllCostCenterHistory(int costCenterId)
        {
            var parentDatasQuey = costCenterHistoryRepositoryQuery.FindQueryable(s => s.CostCenterId == costCenterId).Include(a => a.employees);
            var historyList = new List<HistoryResponceDto>();
            foreach (var item in parentDatasQuey.ToList())
            {
                var historyDto = new HistoryResponceDto();
                HistoryActionsNames actionName = HistoryActionsAliasNames.HistoryName[item.LastAction];


                historyDto.Date = item.LastDate;

                historyDto.TransactionTypeAr = actionName.ArabicName;
                historyDto.TransactionTypeEn = actionName.LatinName;
                historyDto.LatinName = item.employees.LatinName;
                historyDto.ArabicName = item.employees.ArabicName;
                historyDto.BrowserName = item.BrowserName;
                historyList.Add(historyDto);
            }
            return new ResponseResult() { Data = historyList, Id = null, Result = Result.Success };

            // return repositoryActionResult.GetRepositoryActionResult(historyList, RepositoryActionStatus.Ok, message: "Ok");
        }
        //Not used
        public async Task<IRepositoryActionResult> GetAllCostCenterDataDropDown()
        {

            var AllAccounts = costCenterRepositoryQuery.FindQueryable(s => s.ParentId > 0).OrderBy(q => q.Id);
            var empCodes = costCenterRepositoryQuery.FindSelectorQueryable<int>(AllAccounts, q => q.ParentId.Value);
            var List = empCodes.ToList();
            var parentDatas = costCenterRepositoryQuery.FindAll(q => List.Contains(q.Id)).ToList().OrderBy(q => q.ParentId);

            var list = Mapping.Mapper.Map<List<GLCostCenter>, List<CostCenterDropDown>>(parentDatas.ToList());
            #region
            //var list = new List<FinancialAccountDropDown>();
            //foreach (var item in parentDatas)
            //{
            //    var Dto = new FinancialAccountDropDown();
            //    Dto.Id = item.Id;
            //    Dto.ArabicName = item.ArabicName;
            //    Dto.LatinName = item.LatinName;
            //    Dto.Code = item.Code;
            //    list.Add(Dto);
            //}

            #endregion
            return repositoryActionResult.GetRepositoryActionResult(list, RepositoryActionStatus.Ok, message: "Ok");

        }
        // By Alaa
        public async Task<IRepositoryActionResult> GetAllCostCenterDropDown(int type, int? finanncialAccountId)
        {
            // By Alaa
            // جعفور طلب يوم 17/3/2022 ان الليست لاضافة مركز تكلفه جديد لازم تكون رئيسية وفي القيود فرعيه

            //  var parentDatas = costCenterRepositoryQuery.FindAll(a => (type>0? a.Type == type: true)).ToList().OrderBy(q => q.ParentId);
            //  var list = Mapping.Mapper.Map<List<GLCostCenter>, List<CostCenterDropDown>>(parentDatas)

            // لو هستخدم ليست مراكز التكلفه مع الحسابات هنرجع كل المراكز المرتبطه بالحساب المحدد بس
            var hasFinancialAccont = financialCostRepositoryQuery.TableNoTracking
                .Where(e => (finanncialAccountId != null ? e.FinancialAccountId == finanncialAccountId : true));
            var costCenter = costCenterRepositoryQuery.FindAll(a => (type > 0 ? a.Type == type : true)
             //hamada select code with data in dropdown 
             && (finanncialAccountId != null ? hasFinancialAccont.Select(a => a.CostCenterId).Contains(a.Id) : true)).ToList().OrderBy(q => q.ParentId);
            var parentData = costCenter.Select(a => new { a.Id, a.ArabicName, a.LatinName, a.Type, a.Code, a.ParentId });
            return repositoryActionResult.GetRepositoryActionResult(parentData, RepositoryActionStatus.Ok, message: "Ok");

        }

        public bool GetJournalEntryByCostCenterId(int costCenterId)
        {
            var costCenterUsed = JournalEntryDetailsQuery.GetByAsync(a => a.CostCenterId == costCenterId);

            if (costCenterUsed != null)
                return true;
            else
                return false;


        }
    }
}
