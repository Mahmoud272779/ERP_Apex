using App.Application.Helpers.Service_helper.History;
using App.Application.Services.Process.FinancialAccounts;
using App.Domain.Entities.Process.Store;
using App.Domain.Models.Response.Store;
using App.Domain.Models.Response.Store.Reports.Store;
using App.Domain.Models.Security.Authentication.Response.Store;
using Microsoft.EntityFrameworkCore.Query;
using Org.BouncyCastle.Ocsp;
using System.Linq;
using static App.Domain.Models.Security.Authentication.Request.SharedRequestDTOs;

namespace App.Application.Services.Process.Sales_Man
{

    public class SalesManService : BaseClass, ISalesManService
    {
        private readonly IRepositoryQuery<InvSalesMan> SalesManQuery;
        private readonly IRepositoryQuery<InvPersons> _InvPersonsQuery;
        private readonly IRepositoryQuery<InvoiceMaster> _InvoiceMasterQuery;
        private readonly IRepositoryQuery<OfferPriceMaster> _OfferPriceMasterQuery;
        private readonly IRepositoryCommand<InvSalesMan> SalesManCommand;
        private readonly IHistory<InvSalesManHistory> history;
        private readonly IRepositoryQuery<GLBranch> branchesQuery;
        private readonly IRepositoryQuery<GLFinancialAccount> _gLFinancialAccountQuery;
        private readonly iUserInformation _userinformation;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IFinancialAccountBusiness _financialAccountBusiness;
        private readonly IRepositoryQuery<GLGeneralSetting> _gLGeneralSettingQuery;
        private readonly iGLFinancialAccountRelation _iGLFinancialAccountRelation;
        private readonly IRepositoryCommand<InvSalesMan_Branches> salesManBranchesCommand;
        private readonly IRepositoryQuery<InvSalesMan_Branches> salesManBranchesQuery;
        private readonly iUserInformation _iUserInformation;
        private readonly IHttpContextAccessor httpContext;
        private readonly IGeneralPrint _iGeneralPrint;
        private readonly IRepositoryQuery<GLIntegrationSettings> _GLIntegrationSettingsQuery;

        public SalesManService(IRepositoryQuery<InvSalesMan> _SalesManQuery
                                 , IRepositoryCommand<InvSalesMan> _SalesManCommand
                                 , IHistory<InvSalesManHistory> history
                                 , IRepositoryQuery<GLBranch> branchesQuery
                                 , IRepositoryQuery<GLFinancialAccount> GLFinancialAccountQuery
                                 , iUserInformation Userinformation
                                 , ISystemHistoryLogsService systemHistoryLogsService
                                 , IFinancialAccountBusiness financialAccountBusiness
                                 , IRepositoryQuery<GLGeneralSetting> GLGeneralSettingQuery
                                 , iGLFinancialAccountRelation iGLFinancialAccountRelation
                                 , IRepositoryCommand<InvSalesMan_Branches> salesManBranchesCommand
                                 , IRepositoryQuery<InvSalesMan_Branches> salesManBranchesQuery
                                 , iUserInformation iUserInformation
                                 , IHttpContextAccessor _httpContext,
IGeneralPrint iGeneralPrint,
IRepositoryQuery<InvoiceMaster> invoiceMasterQuery,
IRepositoryQuery<OfferPriceMaster> offerPriceMasterQuery,
IRepositoryQuery<GLIntegrationSettings> gLIntegrationSettingsQuery,
IRepositoryQuery<InvPersons> invPersonsQuery)
                                      : base(_httpContext)
        {
            SalesManQuery = _SalesManQuery;
            SalesManCommand = _SalesManCommand;
            httpContext = _httpContext;
            this.history = history;
            this.branchesQuery = branchesQuery;
            _gLFinancialAccountQuery = GLFinancialAccountQuery;
            _userinformation = Userinformation;
            _systemHistoryLogsService = systemHistoryLogsService;
            _financialAccountBusiness = financialAccountBusiness;
            _gLGeneralSettingQuery = GLGeneralSettingQuery;
            _iGLFinancialAccountRelation = iGLFinancialAccountRelation;
            this.salesManBranchesCommand = salesManBranchesCommand;
            this.salesManBranchesQuery = salesManBranchesQuery;
            _iUserInformation = iUserInformation;
            _iGeneralPrint = iGeneralPrint;
            _InvoiceMasterQuery = invoiceMasterQuery;
            _OfferPriceMasterQuery = offerPriceMasterQuery;
            _GLIntegrationSettingsQuery = gLIntegrationSettingsQuery;
            _InvPersonsQuery = invPersonsQuery;
        }

        public async Task<ResponseResult> AddSalesMan(SalesManRequest parameter)
        {
            parameter.LatinName = Helpers.Helpers.IsNullString(parameter.LatinName);
            parameter.ArabicName = Helpers.Helpers.IsNullString(parameter.ArabicName);
            if (string.IsNullOrEmpty(parameter.LatinName))
                parameter.LatinName = parameter.ArabicName;
            var checkBranch = await branchesHelper.CheckIsBranchExist(parameter.Branches, branchesQuery);
            if (checkBranch != null)
                return checkBranch;
            if (string.IsNullOrEmpty(parameter.ArabicName))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };



            var ArabicsalesManExist = await SalesManQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName);
            if (ArabicsalesManExist != null)
                return new ResponseResult() { Data = null, Id = ArabicsalesManExist.Id, Result = Result.Exist, Note = Actions.ArabicNameExist };

            var LatinsalesManExist = await SalesManQuery.GetByAsync(a => a.LatinName == parameter.LatinName);
            if (LatinsalesManExist != null)
                return new ResponseResult() { Data = null, Id = LatinsalesManExist.Id, Result = Result.Exist, Note = Actions.EnglishNameExist };


            var PhoneExist = await SalesManQuery.GetByAsync(a => a.Phone == parameter.Phone && !string.IsNullOrEmpty(parameter.Phone));
            if (PhoneExist != null)
                return new ResponseResult() { Data = null, Id = PhoneExist.Id, Result = Result.Exist, Note = Actions.PhoneExist };

            var EmailExist = await SalesManQuery.GetByAsync(a => a.Email == parameter.Email && !string.IsNullOrEmpty(parameter.Email));
            if (EmailExist != null)
                return new ResponseResult() { Data = null, Id = EmailExist.Id, Result = Result.Exist, Note = Actions.EmailExist };

            if (parameter.Branches.Count() == 0)
                return new ResponseResult() { Result = Result.RequiredData, Note = Actions.BranchIsRequired };

            int NextCode = SalesManQuery.GetMaxCode(e => e.Code) + 1;

            var SalesManData = new InvSalesMan();
            SalesManData.ArabicName = parameter.ArabicName;
            SalesManData.LatinName = parameter.LatinName;
            SalesManData.Phone = parameter.Phone;
            SalesManData.Email = parameter.Email;
            SalesManData.ApplyToCommissions = parameter.ApplyToCommissions;
            if (parameter.ApplyToCommissions)
                SalesManData.CommissionListId = parameter.CommissionListId;
            else
                SalesManData.CommissionListId = null;
            //  SalesManData.BranchId = parameter.BranchId;
            SalesManData.Code = NextCode;
            SalesManData.Notes = parameter.Notes;
            SalesManData.SalesPriceId = parameter.SalesPriceId;
            var finanicalAccount = await _iGLFinancialAccountRelation.GLRelation(GLFinancialAccountRelation.salesman, parameter.FinancialAccountId ?? 0, parameter.Branches, SalesManData.ArabicName, SalesManData.LatinName);
            if (finanicalAccount.Result != Result.Success)
                return finanicalAccount;
            SalesManData.FinancialAccountId = finanicalAccount.Id;
            SalesManCommand.Add(SalesManData);

            var salesManBranchesList = new List<InvSalesMan_Branches>();
            foreach (var item in parameter.Branches)
            {
                salesManBranchesList.Add(new InvSalesMan_Branches() { BranchId = item, SalesManId = SalesManData.Id });

            }
            salesManBranchesCommand.AddRange(salesManBranchesList);
            await salesManBranchesCommand.SaveAsync();
            history.AddHistory(SalesManData.Id, SalesManData.LatinName, SalesManData.ArabicName, Aliases.HistoryActions.Add, Aliases.TemporaryRequiredData.UserName);
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addSalesmen);

            return new ResponseResult() { Data = null, Id = SalesManData.Id, Result = Result.Success };


        }


        private List<SalesManDto> GetBranchesData(List<InvSalesMan> salesManData)
        {

            var result = Mapping.Mapper.Map<List<InvSalesMan>, List<SalesManDto>>(salesManData.ToList());

            foreach (var item in result)
            {
                var Branches = branchesQuery.GetAll(e => item.Branches.Contains(e.Id))
                                    .Select(e => new { ArabicName = e.ArabicName, LatinName = e.LatinName }).ToList();
                item.BranchNameAr = string.Join(',', Branches.Select(e => e.ArabicName).ToArray());
                item.BranchNameEn = string.Join(',', Branches.Select(e => e.LatinName).ToArray());
            }
            return result;
        }
        public async Task<listOfSalesmanResponse> ListOfSalesMan(SalesManSearch parameters, string ids, bool isSearchData = true, bool isPrint = false)
        {
            int[] salesManIds = null;
            var branches = parameters.BranchList.Split(',').Select(x => int.Parse(x)).ToArray();
            if (!string.IsNullOrEmpty(ids))
                salesManIds = ids.Split(",").Select(c => int.Parse(c)).ToArray();

            var Branches = branchesQuery.TableNoTracking.Select(e => new {e.Id, ArabicName = e.ArabicName, LatinName = e.LatinName }).ToList();
            var invoices = _InvoiceMasterQuery.TableNoTracking;
            var offerPrices = _OfferPriceMasterQuery.TableNoTracking;
            var FinancialAccount = _gLFinancialAccountQuery.TableNoTracking;

            var salesMan = SalesManQuery.TableNoTracking.Include(c=> c.SalesManBranch).Include(c=> c.CommissionList).OrderByDescending(c=> c.Id);
            var persons = _InvPersonsQuery.TableNoTracking;
            var totalCount = salesMan.Count();


            var salesManFiltered = salesMan
                .Where(c => !string.IsNullOrEmpty(parameters.Name) ? (c.ArabicName.Contains(parameters.Name) || c.LatinName.Contains(parameters.Name) || c.Code.ToString().Contains(parameters.Name) || c.Phone.Contains(parameters.Name)) : true)
                .Where(c => c.SalesManBranch.Any(e => branches.First() != 0 ? branches.Contains(e.BranchId) : true))
                .Where(c=> salesManIds != null ? salesManIds.Contains(c.Id) : true);
            var dataCount = salesManFiltered.Count();

            var paginationData = !isPrint ? salesManFiltered.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToList() : salesManFiltered.ToList();


            var res = paginationData.Select(x => new getListOfSalesmanResponse
            {
                ApplyToCommissions = x.ApplyToCommissions,
                ArabicName = x.ArabicName,
                BranchNameAr = string.Join(',', Branches.Where(c=> x.SalesManBranch.Select(v=> v.BranchId).Contains(c.Id)).Select(e => e.ArabicName).ToArray()),
                BranchNameEn = string.Join(',', Branches.Where(c => x.SalesManBranch.Select(v => v.BranchId).Contains(c.Id)).Select(e => e.LatinName).ToArray()),
                Branches = x.SalesManBranch.Select(c=> c.BranchId).ToArray(),
                CanDelete = getCanDelete(invoices,offerPrices,persons,x.Id),
                Code = x.Code,
                CommissionListId = x.CommissionListId != null? x.CommissionListId : null,
                CommissionListNameAr = x.CommissionListId !=null ?  x.CommissionList.ArabicName : null,
                CommissionListNameEn = x.CommissionListId !=null ?  x.CommissionList.LatinName : null,
                Email = x.Email,
                Id = x.Id,
                LatinName = x.LatinName,
                Notes = x.Notes,
                Phone = x.Phone,
                SalesPriceId = x.SalesPriceId,
                FinancialAccountId = FinancialAccount.Where(d => d.Id == x.FinancialAccountId)
                                                     .Select(d => new FinancialAccount
                                                     {
                                                         Id = d.Id,
                                                         ArabicName = d.ArabicName,
                                                         LatinName = d.LatinName
                                                     }).FirstOrDefault()
            })
            .ToList();

            return new listOfSalesmanResponse
            {
                getListOfSalesmanResponses = res,
                DataCount = dataCount,
                TotalCount = totalCount,
                isDataExist = salesManFiltered.Any()
            };
        }
        private bool getCanDelete(IQueryable<InvoiceMaster> invoices,IQueryable<OfferPriceMaster> offerPrice, IQueryable<InvPersons> persons,int salesManId)
        {
            var canDelete = !invoices.Where(c => c.SalesManId == salesManId).Any() && !offerPrice.Where(c => c.SalesManId == salesManId).Any() && !persons.Where(c => c.SalesManId == salesManId).Any(); ;
            return canDelete;
        }
        public async Task<listOfSalesmanResponse> _ListOfSalesMan(SalesManSearch parameters, string ids, bool isSearchData = true, bool isPrint = false)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var branches = parameters.BranchList.Split(',').Select(x => int.Parse(x)).ToArray();
            var totalcount = SalesManQuery.TableNoTracking.Where(x => x.SalesManBranch.Select(d => d.BranchId).Any(c => userInfo.employeeBranches.Contains(c))).Count();
            var invoices = _InvoiceMasterQuery.TableNoTracking;
            var offerPrices = _OfferPriceMasterQuery.TableNoTracking;
            //var x =((IQueryable) totalcount).ToQueryString();
            IList<InvSalesMan> resData = new List<InvSalesMan>();
            if (!isSearchData)
            {
                var saleManObject = new InvSalesMan();

                string[] salesManIds = ids.Split(",");
                foreach (var id in salesManIds)
                {
                    saleManObject = SalesManQuery.TableNoTracking.Include(a => a.persons).Include(a => a.SalesManBranch).Where(a => a.Id == Convert.ToInt32(id)).FirstOrDefault();
                    resData.Add(saleManObject);
                }
            }
            else
            {
                resData = await SalesManQuery.GetAllIncludingAsync(0, 0,
                                a => ((a.Code.ToString().Contains(parameters.Name) || (string.IsNullOrEmpty(parameters.Name)
                                || a.ArabicName.Contains(parameters.Name) || a.LatinName.Contains(parameters.Name)) || a.Phone.Contains(parameters.Name)))
                                , e => (string.IsNullOrEmpty(parameters.Name) ? e.OrderByDescending(q => q.Code) : e.OrderBy(a => (a.Code.ToString().Contains(parameters.Name)) ? 0 : 1))
                                , w => w.CommissionList, s => s.SalesManBranch, a => a.persons, a => a.FinancialAccount);

            }

            var FinancialAccount = _gLFinancialAccountQuery.TableNoTracking;
            resData = resData.Where(x => x.SalesManBranch.Select(d => d.BranchId).Any(c => userInfo.employeeBranches.Contains(c))).ToList();
            if (parameters.BranchList != null && branches.Count() > 0)
                resData = resData.Where(a => a.SalesManBranch.Any(e => branches.First() != 0 ? branches.Contains(e.BranchId) : true)).ToList();

            var count = resData.Count();

            if (parameters.PageSize > 0 && parameters.PageNumber > 0)
            {
                resData = !isPrint ? resData.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToList() : resData;
            }
            else
            {
                return null;

            }
            resData.ToList().ForEach(x =>
            {
                x.CanDelete = invoices.Where(c=> c.SalesManId == x.Id).Any() || offerPrices.Where(c=> c.SalesManId == x.Id).Any() || x.persons.Any() ? false : true;
            });
            // resData.Where(a => a.Id != 1 && a.persons.Count == 0).Select(a => { a.CanDelete = true; return a; }).ToList();
            var result = GetBranchesData(resData.ToList());
            var response = result.Select(x => new getListOfSalesmanResponse
            {
                ApplyToCommissions = x.ApplyToCommissions,
                ArabicName = x.ArabicName,
                BranchNameAr = x.BranchNameAr,
                BranchNameEn = x.BranchNameEn,
                Branches = x.Branches,
                CanDelete = x.CanDelete,
                Code = x.Code,
                CommissionListId = x.CommissionListId,
                CommissionListNameAr = x.CommissionListNameAr,
                CommissionListNameEn = x.CommissionListNameEn,
                Email = x.Email,
                Id = x.Id,
                LatinName = x.LatinName,
                Notes = x.Notes,
                Phone = x.Phone,
                SalesPriceId = x.SalesPriceId,
                FinancialAccountId = FinancialAccount.Where(d => d.Id == x.FinancialAccountId)
                                                     .Select(d => new FinancialAccount
                                                     {
                                                         Id = d.Id,
                                                         ArabicName = d.ArabicName,
                                                         LatinName = d.LatinName
                                                     }).FirstOrDefault()
            }).ToList();
            return new listOfSalesmanResponse
            {
                getListOfSalesmanResponses = response,
                DataCount = count,
                TotalCount = totalcount,
                isDataExist = resData.Any()
            };
        }
        public async Task<ResponseResult> GetListOfSalesMan(SalesManSearch parameters)
        {
            var res = await ListOfSalesMan(parameters, null);
            return new ResponseResult() { Data = res.getListOfSalesmanResponses, DataCount = res.DataCount, TotalCount = res.TotalCount, Id = null, Result = res.isDataExist ? Result.Success : Result.NoDataFound };

        }
        public async Task<WebReport> SalesManReport(SalesManSearch parameters, string ids, bool isSearchData, exportType exportType, bool isArabic,int fileId=0)
        {
            var data = new listOfSalesmanResponse();
            // var mainData = (IEnumerable<itemBalanceInStoresResponse>)data.;

            data = await ListOfSalesMan(parameters, ids, isSearchData, true);



            var userInfo = await _iUserInformation.GetUserInformation();



            var otherdata = new ReportOtherData()
            {

                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),

                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")

            };

            var tablesNames = new TablesNames()
            {

                FirstListName = "SalesMan",

            };




            var report = await _iGeneralPrint.PrintReport<object, getListOfSalesmanResponse, object>(null, data.getListOfSalesmanResponses, null, tablesNames, otherdata
             , (int)SubFormsIds.Salesmen_Sales, exportType, isArabic,fileId);
            return report;
        }

        public async Task<ResponseResult> UpdateSalesMan(UpdateSalesManRequest parameters)
        {
            if (parameters.Id == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };
            var checkBranch = await branchesHelper.CheckIsBranchExist(parameters.Branches, branchesQuery);
            if (checkBranch != null)
                return checkBranch;
            parameters.LatinName = Helpers.Helpers.IsNullString(parameters.LatinName);
            parameters.ArabicName = Helpers.Helpers.IsNullString(parameters.ArabicName);
            if (string.IsNullOrEmpty(parameters.LatinName))
                parameters.LatinName = parameters.ArabicName;

            if (string.IsNullOrEmpty(parameters.ArabicName))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };


            var ArabicSalesManExist = await SalesManQuery.GetByAsync(a => a.ArabicName == parameters.ArabicName && a.Id != parameters.Id);
            if (ArabicSalesManExist != null)
                return new ResponseResult() { Data = null, Id = ArabicSalesManExist.Id, Result = Result.Exist, Note = Actions.ArabicNameExist };


            var EnglishSalesManExist = await SalesManQuery.GetByAsync(a => a.LatinName == parameters.LatinName && a.Id != parameters.Id);
            if (EnglishSalesManExist != null)
                return new ResponseResult() { Data = null, Id = EnglishSalesManExist.Id, Result = Result.Exist, Note = Actions.EnglishNameExist };


            var data = await SalesManQuery.GetByAsync(a => a.Id == parameters.Id);
            if (data == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.NoDataFound };

            var PhoneExist = await SalesManQuery.GetByAsync(a => a.Phone == parameters.Phone && a.Id != parameters.Id && !string.IsNullOrEmpty(parameters.Phone));
            if (PhoneExist != null)
                return new ResponseResult() { Data = null, Id = PhoneExist.Id, Result = Result.Exist, Note = Actions.PhoneExist };


            var EmailExist = await SalesManQuery.GetByAsync(a => a.Email == parameters.Email && a.Id != parameters.Id && !string.IsNullOrEmpty(parameters.Email));
            if (EmailExist != null)
                return new ResponseResult() { Data = null, Id = EmailExist.Id, Result = Result.Exist, Note = Actions.EmailExist };
            var GLSettings = _gLGeneralSettingQuery.TableNoTracking.FirstOrDefault();
            //if (GLSettings.DefultAccSafe != 1)
            //{
            //    parameters.FinancialAccountId = data.FinancialAccountId;
            //}
            var table = Mapping.Mapper.Map<UpdateSalesManRequest, InvSalesMan>(parameters, data);

            var userInfo = await _iUserInformation.GetUserInformation();
            var linkingMethod = _GLIntegrationSettingsQuery
            .TableNoTracking
                   .Where(c => c.GLBranchId == userInfo.CurrentbranchId && c.screenId == (int)SubFormsIds.Salesmen_Sales)
                   .FirstOrDefault().linkingMethodId;
            if (linkingMethod == 1 && table.FinancialAccountId != parameters.FinancialAccountId)
                table.FinancialAccountId = parameters.FinancialAccountId;
            //if (GLSettings.DefultAccSalesMan == 1 && table.FinancialAccountId != data.FinancialAccountId)
            //{
            //    var finanicalAccount = await _iGLFinancialAccountRelation.GLRelation(GLFinancialAccountRelation.salesman, (int)parameters.FinancialAccountId, parameters.Branches, table.ArabicName, table.LatinName);
            //    if (finanicalAccount.Result != Result.Success)
            //        return finanicalAccount;
            //    table.FinancialAccountId = finanicalAccount.Id;
            //}
            await SalesManCommand.UpdateAsyn(table);

            if (table.Id != 1)
            {
                await salesManBranchesCommand.DeleteAsync(e => e.SalesManId == data.Id);
                List<InvSalesMan_Branches> salesManBranchesList = new List<InvSalesMan_Branches>();
                foreach (var item in parameters.Branches)
                {
                    salesManBranchesList.Add(new InvSalesMan_Branches() { BranchId = item, SalesManId = data.Id });
                }
                salesManBranchesCommand.AddRange(salesManBranchesList);
                await salesManBranchesCommand.SaveAsync();
            }
            history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editSalesmen);
            return new ResponseResult() { Data = null, Id = data.Id, Result = data == null ? Result.Failed : Result.Success };

        }

        public async Task<ResponseResult> DeleteSalesMan(SharedRequestDTOs.Delete ListCode)
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











                var invoices = _InvoiceMasterQuery.TableNoTracking.Select(x => x.SalesManId).GroupBy(x=> x).Select(x=> x.FirstOrDefault());
                var offerPrices = _OfferPriceMasterQuery.TableNoTracking.Select(x=> x.SalesManId).GroupBy(x => x).Select(x => x.FirstOrDefault());


                var salesManUsed = SalesManQuery.TableNoTracking
                    .Include(a => a.persons)
                    .Where(x=> ListCode.Ids.Contains(x.Id))
                    .Where(a => 
                    a.persons.Count() > 0 || 
                    invoices.Contains(a.Id) || offerPrices.Contains(a.Id)
                    
                    )
                    .Select(a => a.Id)
                    .ToList();


                var deletedBranches = await salesManBranchesCommand.DeleteAsync(a => ListCode.Ids.Contains(a.SalesManId) && (salesManUsed != null ? !salesManUsed.Contains(a.SalesManId) : true));
                salesManBranchesCommand.SaveChanges();


                var SalesMan = SalesManQuery.TableNoTracking.Where(e => ListCode.Ids.Contains(e.Id)
                    && e.Id != 1 && (salesManUsed != null ? !salesManUsed.Contains(e.Id) : true)).ToList();

                SalesManCommand.RemoveRange(SalesMan);
                await SalesManCommand.SaveAsync();
                var FA_Deleted = await _financialAccountBusiness.DeleteFinancialAccountAsync(new SharedRequestDTOs.Delete()
                {
                    Ids = SalesMan.Select(x => x.FinancialAccountId ?? 0).ToArray(),
                    userId = userInfo.userId

                });
                if (SalesMan.Any())
                    await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteSalesmen);
                return new ResponseResult() { Data = null, Id = null, Result = (SalesMan.Count() == 0 ? Result.CanNotBeDeleted : Result.Success) };
            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = ex };
            }

        }



        public async Task<ResponseResult> GetSalesManHistory(int SalesManId)
        {
            return await history.GetHistory(a => a.EntityId == SalesManId);

        }

        public async Task<ResponseResult> GetSalesManDropDown(getDropDownlist param)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var SalesManList = SalesManQuery.TableNoTracking
                                .Include(x => x.SalesManBranch)
                                .Where(x => param.isPerson ? true : x.SalesManBranch.Select(c => c.BranchId).Contains(userInfo.CurrentbranchId))
                                .Where(a => param.code != 0 ? a.Code == param.code : true)
                                .Where(a => !string.IsNullOrEmpty(param.SearchCriteria) ? a.ArabicName.ToLower().Contains(param.SearchCriteria) || a.LatinName.ToLower().Contains(param.SearchCriteria) : true)
                                .Select(a => new { a.Id, a.Code, a.ArabicName, a.LatinName });



            double MaxPageNumber = SalesManList.Count() / Convert.ToDouble(param.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            var EndOfData = (countofFilter == param.PageNumber ? Actions.EndOfData : "");
            SalesManList = SalesManList.Skip((param.PageNumber - 1) * param.PageSize).Take(param.PageSize);





            return new ResponseResult() { Note = EndOfData, Data = SalesManList, Id = null, Result = SalesManList.Any() ? Result.Success : Result.Failed };

        }

    }
}
