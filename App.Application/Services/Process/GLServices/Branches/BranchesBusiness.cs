using App.Application.Basic_Process;
using App.Application.Helpers.Service_helper.History;
using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Application.Services.Process.GeneralServices.DeletedRecords;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Domain.Entities.Process.Store;
using Attendleave.Erp.Core.APIUtilities;
using Attendleave.Erp.ServiceLayer.Abstraction;
using Microsoft.Net.Http.Headers;
using Org.BouncyCastle.Ocsp;

namespace App.Application.Services.Process.Branches
{
    public class BranchesBusiness : BusinessBase<GLBranch>, IBranchesBusiness
    {
        private readonly IRepositoryQuery<GLBranch> branchRepositoryQuery;
        private readonly IRepositoryCommand<GLBranch> branchRepositoryCommand;
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GLBank> banksRepositoryQuery;
        private readonly IRepositoryQuery<InvSalesMan_Branches> _invSalesMan_BranchesQuery;
        private readonly IRepositoryQuery<InvPersons_Branches> _invPersons_BranchesQuery;
        private readonly IRepositoryQuery<GLBankBranch> bankBranchRepositoryQuery;
        private readonly IRepositoryQuery<InvStoreBranch> _invStoreBranchQuery;
        private readonly IRepositoryQuery<InvEmployeeBranch> _invEmployeeBranchQuery;
        private readonly IRepositoryQuery<GlReciepts> _glRecieptsQuery;
        private readonly IRepositoryCommand<InvSalesMan_Branches> _invSalesMan_BranchesCommand;
        private readonly IRepositoryCommand<InvPersons_Branches>  _invPersons_BranchesCommand;
        private readonly IRepositoryCommand<InvEmployeeBranch>    _invEmployeeBranchCommand;
        private readonly IDeletedRecordsServices _deletedRecords;
        private readonly IGeneralAPIsService generalAPIsService;
        private readonly IRepositoryQuery<InvStpStores> _invStpStoresQuery;
        private readonly IRepositoryQuery<GLSafe> treasuryRepositoryQuery;
        private readonly IRepositoryQuery<InvoiceMaster> _invoiceMasterQuery;
        private readonly IRepositoryQuery<InvEmployees> employeesRepositoryQuery;
        private readonly iDefultDataRelation _iDefultDataRelation;
        private readonly iUserInformation _iUserInformation;
        private readonly IPagedList<GLBranch, GLBranch> pagedListBranch;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly ISecurityIntegrationService _securityIntegrationService;
        private readonly IHttpContextAccessor httpContext;
        private readonly IHistory<GLBranchHistory> history;
        public BranchesBusiness(
        IRepositoryQuery<GLBranch> BranchRepositoryQuery,
        IRepositoryCommand<GLBranch> BranchRepositoryCommand,
        IRepositoryQuery<GLBank> BanksRepositoryQuery,
        IRepositoryQuery<GLSafe> TreasuryRepositoryQuery,
        IRepositoryQuery<InvoiceMaster> InvoiceMasterQuery,
        IRepositoryQuery<GLBankBranch> BankBranchRepositoryQuery,
        IRepositoryQuery<InvStoreBranch> InvStoreBranchQuery,
        IRepositoryQuery<InvStpStores> InvStpStoresQuery,
        IRepositoryQuery<GLFinancialAccount> FinancialAccountRepositoryQuery,
        IPagedList<GLBranch, GLBranch> PagedListBranch,
        ISystemHistoryLogsService systemHistoryLogsService,
        ISecurityIntegrationService securityIntegrationService,
        IHttpContextAccessor HttpContext, IRepositoryQuery<InvEmployees> employeesRepositoryQuery,
        iDefultDataRelation iDefultDataRelation,
        iUserInformation iUserInformation,
        IHistory<GLBranchHistory> History,
        //Relation
        IRepositoryQuery<InvSalesMan_Branches> InvSalesMan_BranchesQuery,
        IRepositoryQuery<InvPersons_Branches> InvPersons_BranchesQuery, 
        IRepositoryQuery<InvEmployeeBranch> InvEmployeeBranchQuery,
        IRepositoryQuery<GlReciepts> GlRecieptsQuery,
        IRepositoryCommand<InvSalesMan_Branches> InvSalesMan_BranchesCommand,
        IRepositoryCommand<InvPersons_Branches> InvPersons_BranchesCommand,
        IRepositoryCommand<InvEmployeeBranch> InvEmployeeBranchCommand,
        IDeletedRecordsServices deletedRecords,
        IGeneralAPIsService generalAPIsService,

        //Relation
        IRepositoryActionResult repositoryActionResult) : base(repositoryActionResult)
        {
            branchRepositoryQuery = BranchRepositoryQuery;
            branchRepositoryCommand = BranchRepositoryCommand;
            banksRepositoryQuery = BanksRepositoryQuery;
            _invSalesMan_BranchesQuery = InvSalesMan_BranchesQuery;
            _invPersons_BranchesQuery = InvPersons_BranchesQuery;
            treasuryRepositoryQuery = TreasuryRepositoryQuery;
            _invoiceMasterQuery = InvoiceMasterQuery;
            bankBranchRepositoryQuery = BankBranchRepositoryQuery;
            _invStoreBranchQuery = InvStoreBranchQuery;
            _invEmployeeBranchQuery = InvEmployeeBranchQuery;
            _glRecieptsQuery = GlRecieptsQuery;
            _invSalesMan_BranchesCommand = InvSalesMan_BranchesCommand;
            _invPersons_BranchesCommand = InvPersons_BranchesCommand;
            _invEmployeeBranchCommand = InvEmployeeBranchCommand;
            _deletedRecords = deletedRecords;
            this.generalAPIsService = generalAPIsService;
            _invStpStoresQuery = InvStpStoresQuery;
            financialAccountRepositoryQuery = FinancialAccountRepositoryQuery;
            pagedListBranch = PagedListBranch;
            _systemHistoryLogsService = systemHistoryLogsService;
            _securityIntegrationService = securityIntegrationService;
            this.employeesRepositoryQuery = employeesRepositoryQuery;
            _iDefultDataRelation = iDefultDataRelation;
            _iUserInformation = iUserInformation;
            httpContext = HttpContext;
            this.history = History;
        }
        public async Task<string> AddAutomaticCode()
        {
            var code = branchRepositoryQuery.FindQueryable(q => q.Id > 0);
            if (code.Count() > 0)
            {
                var code2 = branchRepositoryQuery.FindQueryable(q => q.Id > 0).ToList().Last();
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
        public async Task<GLBranch> CheckIsValidNameAr(string NameAr)
        {

            var branch
                = await branchRepositoryQuery.GetByAsync(
                   cust => cust.ArabicName == NameAr);
            return branch;

        }
        public async Task<GLBranch> CheckIsValidNameEn(string NameEn)
        {

            var branch
                = await branchRepositoryQuery.GetByAsync(
                   cust => cust.LatinName == NameEn);
            return branch;

        }
        public async Task<ResponseResult> AddBranch(BranchRequestsDTOs.Add parameter)
        {
            try
            {
                parameter.Status = 1;
                var security = await _securityIntegrationService.getCompanyInformation();
                if (!security.isInfinityNumbersOfBranchs)
                {
                    var branchsCount = branchRepositoryQuery.GetAll().Count();
                    if (branchsCount >= security.AllowedNumberOfBranchs)
                        return new ResponseResult()
                        {
                            Note = Actions.YouHaveTheMaxmumOfBranchs,
                            Result = Result.MaximumLength,
                            ErrorMessageAr = "تجاوزت الحد الاقصي من عدد الفروع",
                            ErrorMessageEn = "You Cant add a new branch because you have the maximum of branches for your bunlde"
                        };
                }
                parameter.LatinName = Helpers.Helpers.IsNullString(parameter.LatinName);
                parameter.ArabicName = Helpers.Helpers.IsNullString(parameter.ArabicName);
                if (string.IsNullOrEmpty(parameter.LatinName))
                    parameter.LatinName = parameter.ArabicName;
                if (string.IsNullOrEmpty(parameter.ArabicName))
                    return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };

                if (parameter.Status < (int)Status.Active || parameter.Status > (int)Status.Inactive)
                {
                    return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
                }
                var BranchExist = await branchRepositoryQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName && !string.IsNullOrEmpty(parameter.ArabicName));
                if (BranchExist != null)
                    return new ResponseResult { Result = Result.Exist, Note = Actions.ArabicNameExist };
                BranchExist = await branchRepositoryQuery.GetByAsync(a => a.LatinName == parameter.LatinName && !string.IsNullOrEmpty(parameter.LatinName));
                if (BranchExist != null)
                    return new ResponseResult { Result = Result.Exist, Note = Actions.EnglishNameExist };


                var PhoneExist = await branchRepositoryQuery.GetByAsync(a => a.Phone == parameter.Phone && !string.IsNullOrEmpty(parameter.Phone));
                if (PhoneExist != null)
                    return new ResponseResult() { Data = null, Id = PhoneExist.Id, Result = Result.Exist, Note = Actions.PhoneExist };



                var table = Mapping.Mapper.Map<BranchRequestsDTOs.Add, GLBranch>(parameter);
                table.BrowserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();


                table.Code = branchRepositoryQuery.GetMaxCode(a => a.Code) + 1;//await AddAutomaticCode();
                table.CanDelete = true;

                //  table.Code = await AddAutomaticCode();

                table.UTime = DateTime.Now;

                var saved = branchRepositoryCommand.Add(table);
                if (saved)
                {
                    await _iDefultDataRelation.BranchsRelation(table.Id);
                }


                history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Add,Aliases.TemporaryRequiredData.UserName);
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addBranch);
                 return new ResponseResult() { Id = table.Id, Result = Result.Success, Note = Actions.SavedSuccessfully };
            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex, Note = Actions.ExceptionOccurred };
            }
            //return  new Task<bool>(() => true);
        }
        public async Task<ResponseResult> UpdateBranch(BranchRequestsDTOs.Update parameter)
        {
            try
            {
                if (parameter.Id == 0)
                    return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };
                parameter.Status = 1;
                parameter.LatinName = Helpers.Helpers.IsNullString(parameter.LatinName);
                parameter.ArabicName = Helpers.Helpers.IsNullString(parameter.ArabicName);
                if (string.IsNullOrEmpty(parameter.LatinName))
                    parameter.LatinName = parameter.ArabicName;
                if (string.IsNullOrEmpty(parameter.ArabicName))
                    return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };

                if (parameter.Status < (int)Status.Active || parameter.Status > (int)Status.Inactive)
                {
                    return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
                }

                var branchExist = await branchRepositoryQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName && !string.IsNullOrEmpty(parameter.ArabicName) && a.Id != parameter.Id);
                if (branchExist != null)
                    return new ResponseResult { Result = Result.Exist, Note = Actions.ArabicNameExist };
                branchExist = await branchRepositoryQuery.GetByAsync(a => a.LatinName == parameter.LatinName && !string.IsNullOrEmpty(parameter.LatinName) && a.Id != parameter.Id);
                if (branchExist != null)
                    return new ResponseResult { Result = Result.Exist, Note = Actions.EnglishNameExist };

                var PhoneExist = await branchRepositoryQuery.GetByAsync(a => a.Phone == parameter.Phone && !string.IsNullOrEmpty(parameter.Phone) && a.Id != parameter.Id);
                if (PhoneExist != null)
                    return new ResponseResult() { Data = null, Id = PhoneExist.Id, Result = Result.Exist, Note = Actions.PhoneExist };

                var updateData = await branchRepositoryQuery.GetByAsync(q => q.Id == parameter.Id);


                var table = Mapping.Mapper.Map<BranchRequestsDTOs.Update, GLBranch>(parameter, updateData);
                table.BrowserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();
                table.UTime = DateTime.Now;
                await branchRepositoryCommand.UpdateAsyn(table);

                history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editBranch);
                return new ResponseResult() { Id = table.Id, Result = Result.Success, Note = Actions.UpdatedSuccessfully };
             }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex, Note = Actions.ExceptionOccurred };
            }
        }
        public async Task<ResponseResult> GetBranchById(int Id)
        {
            try
            {
                var treeData = await branchRepositoryQuery.GetAsync(Id);
                if (treeData != null)
                {
                    return new ResponseResult() { Data = treeData, Result = Result.Success, Note = Actions.Success };
                }
                return new ResponseResult() { Data = null, Result = Result.NotFound, Note = Actions.NotFound };
                // return repositoryActionResult.GetRepositoryActionResult(treeData, RepositoryActionStatus.Ok, message: "Ok");
            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex, Note = Actions.ExceptionOccurred };
                //return repositoryActionResult.GetRepositoryActionResult(ex);

            }
        }
        public async Task<ResponseResult> DeleteBranch(SharedRequestDTOs.Delete parameter)
        {
            try
            {
                var lis = new List<ListOfBranches>();

                var branches = branchRepositoryQuery.FindAll(e => parameter.Ids.Contains(e.Id)
                               && e.Id != 1 && !e.BankBranches.Select(a => a.BranchId).Contains(e.Id)
                               && !e.FinancialCosts.Select(a => a.BranchId).Contains(e.Id)
                               && !e.Treasuries.Select(a => a.Id).Contains(e.Id)).ToList();

                //delete _invSalesMan_BranchesCommand
                foreach (var item in branches)
                {
                    await _invSalesMan_BranchesCommand.DeleteAsync(x => x.SalesManId == 1 && x.BranchId == item.Id);
                    await _invPersons_BranchesCommand.DeleteAsync(x => x.PersonId == 1  && x.BranchId == item.Id);
                    await _invPersons_BranchesCommand.DeleteAsync(x => x.PersonId == 2 && x.BranchId == item.Id);
                    await _invEmployeeBranchCommand.DeleteAsync(x => x.EmployeeId == 1 && x.BranchId == item.Id);
                }
                branchRepositoryCommand.RemoveRange(branches);
                var x =  await branchRepositoryCommand.SaveAsync();


                if (branches.Count() == 0)
                    return new ResponseResult() { Data = null, Id = null, Result = Result.CanNotBeDeleted, Note = Actions.CanNotBeDeleted };

                //Fill The DeletedRecordTable
                var Ids = branches.Select(a => a.Id);
                int count = Ids.Count();
                _deletedRecords.SetDeletedRecord(Ids.ToList(), 8);

                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteBranch,count);
                return new ResponseResult() { Data = null, Id = null, Result = Result.Success, Note = Actions.DeletedSuccessfully };

                #region old
                /*  foreach (var item2 in parameter.Ids)
                  {
                      var financial = await financialAccountRepositoryQuery.GetByAsync(q => q.BranchId == item2);
                      if (financial != null)
                      {
                          var bra = await branchRepositoryQuery.GetByAsync(q => q.BranchId == financial.BranchId);
                          var list = new ListOfBranches();
                          list.Name = bra?.ArabicName;
                          lis.Add(list);
                      }
                      if (financial == null)
                      {
                          var bank = await bankBranchRepositoryQuery.GetByAsync(q => q.BranchId == item2);
                          if (bank != null)
                          {
                              var bra = await branchRepositoryQuery.GetByAsync(q => q.BranchId == bank.BranchId);
                              var list = new ListOfBranches();
                              list.Name = bra?.ArabicName;
                              lis.Add(list);
                          }
                          if (bank == null)
                          {
                              var treasury = await treasuryRepositoryQuery.GetByAsync(q => q.BranchId == item2);
                              if (treasury != null)
                              {
                                  var bra = await branchRepositoryQuery.GetByAsync(q => q.BranchId == treasury.BranchId);
                                  var list = new ListOfBranches();
                                  list.Name = bra?.ArabicName;
                                  lis.Add(list);
                              }
                          }
                      }



                  }
                  if (lis.Count() < parameter.Ids.Count())
                  {
                      var BranchDeletedCount = 0;
                      foreach (var item in parameter.Ids)
                      {
                          var BranchDeleted = await branchRepositoryQuery.GetByAsync(q => q.BranchId == item);
                          var financial = await financialAccountRepositoryQuery.GetByAsync(q => q.BranchId == item);
                          if (financial == null)
                          {
                              branchRepositoryCommand.Remove(BranchDeleted);
                              BranchDeletedCount++;
                          }
                      }
                      await branchRepositoryCommand.SaveAsync();
                      if (lis.Count() == 0 || BranchDeletedCount >0)
                      {
                          return new ResponseResult() { Result = Result.Success, Note = Actions.DeletedSuccessfully };
                      }
                      else
                      {
                          return new ResponseResult() { Data = lis, Result = Result.CanNotBeDeleted, Note = Actions.CanNotBeDeleted };
                      }
                  }
                  else
                  {
                      return new ResponseResult() { Data = lis, Result = Result.CanNotBeDeleted, Note = Actions.CanNotBeDeleted };
                  }*/
                // return new ResponseResult() { Result = Result.Success, Note = Actions.DeletedSuccessfully };
                #endregion
            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex, Result = Result.Success, Note = Actions.ExceptionOccurred };
            }
        }
        public async Task<ResponseResult> GetAllBranchData(BranchRequestsDTOs.Search paramters)
        {
            try
            {
                var user = await _iUserInformation.GetUserInformation();
                var treeData = branchRepositoryQuery.TableNoTracking.Where(x=> user.employeeBranches.Contains(x.Id));

                if (!string.IsNullOrEmpty(paramters.Name))
                {
                    treeData = treeData.Where(x => 
                    x.ArabicName.ToLower().Contains(paramters.Name) ||
                    x.LatinName.ToLower().Contains(paramters.Name) ||
                    x.Code.ToString().Contains(paramters.Name));
                }
                if (!string.IsNullOrEmpty(paramters.Name))
                {
                    treeData = treeData.OrderBy(x => x.Id);
                }
                else
                {
                    treeData = treeData.OrderByDescending(x => x.Id);

                }

                //(0, 0,
                //    x => ) ? 0 : 1))
                //                // a => a.Stores, 
                //                //a => a.EmployeeBranches,
                //                //a => a.Treasuries,
                //                //a => a.PersonBranch,
                //                //a => a.StoreBranches,
                //                //a => a.SalesManBranch,
                //                /*a => a.FinancialCosts,*/
                //                //a => a.BankBranches,
                //                /*, a => a.InvoiceMaster,*/ 
                //                //b => b.EmployeeBranches
                //                );


                var Stores = _invStpStoresQuery.TableNoTracking;
                var EmployeeBranches = _invEmployeeBranchQuery.TableNoTracking;
                var Treasuries = treasuryRepositoryQuery.TableNoTracking;
                var PersonBranch = _invPersons_BranchesQuery.TableNoTracking;
                var StoreBranches = _invStoreBranchQuery.TableNoTracking;
                var BankBranches = bankBranchRepositoryQuery.TableNoTracking;
                var invoices = _invoiceMasterQuery.TableNoTracking;
                var salesMan = _invSalesMan_BranchesQuery.TableNoTracking;
                var InvEmployeeBranch = _invEmployeeBranchQuery.TableNoTracking;
                var recs = _glRecieptsQuery.TableNoTracking;

                var response = new List<GLBranch>();
                response = treeData.ToList();
                if (paramters.Status != null && paramters.Status > 0)
                {
                    response = treeData.Where(q => q.Status == paramters.Status).ToList();
                }


                int count = treeData.Count();
                if (paramters.PageSize > 0 && paramters.PageNumber > 0)
                {
                    response = treeData.Skip((paramters.PageNumber - 1) * paramters.PageSize).Take(paramters.PageSize).ToList();
                }
                
                #region old
                //else
                //{
                //    return new ResponseResult() { Data = treeData, DataCount =count, Result = Result.Failed };
                //}

                //treeData.Where(a => a.Id != 1 && a.Stores.Count() == 0 && a.EmployeeBranches.Count() == 0 &&
                // a.PersonBranch.Count() == 0 && a.SalesManBranch.Count() == 0 && a.FinancialCosts.Count() == 0 &&
                //  a.Treasuries.Count() == 0 && a.BankBranches.Count() == 0
                //  && a.InvoiceMaster.Count() == 0 && a.EmployeeBranches.Count == 0)
                //      .Select(e => { e.CanDelete = true; return e; }).ToList();

                //treeData = treeData.Select(e =>
                //           {
                //               e.Stores = null; e.PersonBranch = null;
                //               e.SalesManBranch = null; e.FinancialCosts = null; e.Treasuries = null;
                //               e.BankBranches = null; e.InvoiceMaster = null; return e;
                //           }).ToList();
                #endregion

                var managerData = employeesRepositoryQuery.TableNoTracking.Select(a => new { a.Id, a.ArabicName, a.LatinName, a.Status }).Where(x => true);
                var listOfData = new List<GLBranch>();
                foreach (var item in response)
                {
                    var _salesMan         =    salesMan        .Where(x => x.BranchId == item.Id && x.SalesManId != 1);
                    var _PersonBranch     =    PersonBranch    .Where(d => d.BranchId == item.Id && d.PersonId != 1 && d.PersonId != 2);
                    var _EmployeeBranches =    EmployeeBranches.Where(d => d.BranchId == item.Id && d.EmployeeId != 1);
                    var _StoreBranches    =    StoreBranches   .Where(d => d.BranchId == item.Id);
                    var _BankBranches     =    BankBranches    .Where(d => d.BranchId == item.Id);
                    var _Treasuries       =    Treasuries      .Where(d => d.BranchId == item.Id);
                    var _invoices         =    invoices        .Where(x => x.BranchId == item.Id);
                    var _rec              =    recs            .Where(x => x.BranchId == item.Id);
                    item.CanDelete   =      _salesMan           .Any() ||
                                            _PersonBranch       .Any() ||
                                            _EmployeeBranches   .Any() ||
                                            _StoreBranches      .Any() || 
                                            _BankBranches       .Any() || 
                                            _Treasuries         .Any() ||
                                            _invoices           .Any() ||
                                            _rec                .Any() ||
                                            item.Id == 1
                                            ? false : true;
                    listOfData.Add(item);
                }


                var dta = response.Select(x => new
                {
                    x.Id,
                    x.Code,
                    x.LatinName,
                    x.ArabicName,
                    x.AddressEn,
                    x.AddressAr,
                    x.Fax,
                    x.Phone,
                    x.Status,
                    x.Notes,
                    ManagerId = managerData.Where(a => a.Id == x.ManagerId).Select(c => new { c.Id, c.ArabicName, c.LatinName, c.Status, phone = x.ManagerPhone }).FirstOrDefault(),
                    x.BrowserName,
                    x.CanDelete,
                    x.SalesPriceId
                });

                #region old
                //var resData = Mapping.Mapper.Map<List<GLBranch>, List<BranchResponsesDTOs.GetAll>>(treeData.ToList());

                //var response = new List<BranchResponsesDTOs.GetAll>();
                //foreach (var item in dta)
                //{
                //    var manager = managerData.Where(a => a.Id == item.Manager).ToList();
                //    item.ManagerId = manager.Select(x => new { x.Id, x.ArabicName, x.LatinName, x.Status, phone = item.ManagerPhone }).FirstOrDefault();
                //    response.Add(item);
                //}
                //resData.ToList().ForEach(branch =>
                //{

                //    if (branch.ManagerId > 0)
                //    {


                //        branch.manager = managerData.Select(x => new { x.Id, x.ArabicName, x.LatinName, x.Status,phone = branch.ManagerPhone});
                //        //branch.ManagerNameEn = managerData.First().LatinName;
                //        //branch.ManagerStatus = managerData.First().Status;
                //    }
                //});
                #endregion
                return new ResponseResult() { Data = dta, Result = (treeData.Count() > 0 ? Result.Success : Result.NoDataFound), DataCount = count, Note = "Ok" };
            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex, Note = Actions.ExceptionOccurred };
            }
        }
        public async Task<ResponseResult> GetAllBranchDataDropDown()
        {
            #region
            //try
            //{
            //    var treeData =
            //        branchRepositoryQuery.GetAll()
            //        .Select(a => new { Id = a.Id, ArabicName = a.ArabicName, LatinName = a.LatinName, IsMain = (a.ArabicName == "الفرع الرئيسي" || a.LatinName == "الفرع الرئيسي") ? true : false }).ToList();

            //    return new ResponseResult() { Data = treeData, Result = Result.Success, Note = "Ok" };
            //}
            //catch (Exception ex)
            //{
            //    return new ResponseResult() { Data = ex, Note = Actions.ExceptionOccurred };
            //}
            #endregion old
            var userInfo = await _iUserInformation.GetUserInformation();
            ResponseResult responseResult = new ResponseResult();
            await Task.Run(() =>
            {
                var data =
                branchRepositoryQuery.TableNoTracking
                .Where(e => userInfo.employeeBranches.Contains(e.Id) && e.Status == (int)Status.Active )
                .Select(a => new { Id=a.Id, ArabicName=a.ArabicName, LatinName=a.LatinName, a.Status, IsMain = a.Id == userInfo.CurrentbranchId ? true : false });

                responseResult.Data = data;
                responseResult.Result = data.Any() ? Result.Success : Result.Failed;
            });
            return responseResult;
        }
        public async Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameter)
        {
            return new ResponseResult() {Note = Actions.UpdatedSuccessfully };
            if (parameter.Status < (int)Status.Active || parameter.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }
            bool result = false;
            try
            {
                foreach (var item in parameter.Id)
                {
                    var branch = await branchRepositoryQuery.GetByAsync(q => q.Id == item);
                    if (branch.Id == 1)
                    {
                        branch.Status = (int)Status.Active;
                    }
                    else
                    {
                        branch.Status = parameter.Status;
                    }

                    result = await branchRepositoryCommand.UpdateAsyn(branch);
                }
                return new ResponseResult() { Result = result ? Result.Success : Result.Failed, Note = Actions.UpdatedSuccessfully };
            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex, Note = Actions.ExceptionOccurred };
            }
        }
         public async Task<ResponseResult> GetAllBranchHistory(int BranchId)
        {
          return  await history.GetHistory(a=>a.EntityId==BranchId);
        }

        public async Task<ResponseResult> GetAllBranchDataDropDownForPersons(bool isSuppler)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            bool showAllSettings = false;
            if (isSuppler)
                showAllSettings = userInfo.otherSettings.showAllBranchesInSuppliersInfo;
            else
                showAllSettings = userInfo.otherSettings.showAllBranchesInCustomerInfo;
            ResponseResult responseResult = new ResponseResult();
            await Task.Run(() =>
            {
                var data =
                branchRepositoryQuery.TableNoTracking
                .Where(e => !showAllSettings? userInfo.employeeBranches.Contains(e.Id) : true && e.Status == (int)Status.Active)
                .Select(a => new { Id = a.Id, ArabicName = a.ArabicName, LatinName = a.LatinName, a.Status, IsMain = (a.ArabicName == "الفرع الرئيسي" || a.LatinName == "الفرع الرئيسي") ? true : false });

                responseResult.Data = data;
                responseResult.Result = data.Any() ? Result.Success : Result.Failed;
            });
            return responseResult;
        }

        public async Task<ResponseResult> GetBranchesByDate(DateTime date, int PageNumber, int PageSize)
        {
            try
            {
                var resData = await branchRepositoryQuery.TableNoTracking.Where(q => q.UTime >= date).ToListAsync();

                return await generalAPIsService.Pagination(resData, PageNumber, PageSize);

            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.NotFound };


            }
        }

    }
}
