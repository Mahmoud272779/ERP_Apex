using App.Application.Helpers.Service_helper.History;
using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Application.Services.Process.GeneralServices.DeletedRecords;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Domain.Entities.Process.Store;
using App.Domain.Entities.Setup;
using App.Domain.Models.Response.Store.Reports.Store;
using App.Infrastructure.UserManagementDB;
using DocumentFormat.OpenXml.ExtendedProperties;
using FastReport.Utils;
using Org.BouncyCastle.Ocsp;
using System.Linq;

namespace App.Application.Services.Process.Store
{

    public class StoresService : BaseClass, IStoresService
    {
        private readonly IRepositoryQuery<InvStpStores> StoresRepositoryQuery;
        private readonly IRepositoryQuery<OtherSettingsStores> _otherSettingsStoresQuery;
        private readonly IRepositoryQuery<InvoiceMaster> _invoiceMasterQuery;
        private readonly IRepositoryQuery<OfferPriceMaster> OfferPriceMasterQuery;
        private readonly IDeletedRecordsServices _deletedRecords;
        private readonly IRepositoryCommand<DeletedRecords> _deletedRecordCommand;
        private readonly IRepositoryCommand<InvStpStores> StoresRepositoryCommand;
        private readonly IRepositoryCommand<InvStoreBranch> StoreBranchCommand;
        private readonly IRepositoryQuery<InvStoreBranch> StoreBranchQuery;
        private readonly IGeneralAPIsService generalAPIsService;
        private readonly IRepositoryQuery<GLBranch> branchesRepositoryQuery;
        private readonly IRepositoryQuery<InvStpItemCardStores> _InvStpItemCardStoresQuery;

        private readonly IHttpContextAccessor httpContext;
        private readonly iDefultDataRelation _iDefultDataRelation;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly iUserInformation _iUserInformation;
        private readonly ISecurityIntegrationService _securityIntegrationService;
        private readonly IHistory<InvStoresHistory> history;

        public StoresService(
                               IRepositoryQuery<InvStpStores> _StoresRepositoryQuery,
                               IRepositoryQuery<OtherSettingsStores> OtherSettingsStoresQuery,
                               IRepositoryQuery<InvoiceMaster> InvoiceMasterQuery,
                               IRepositoryCommand<InvStpStores> _StoresRepositoryCommand,
                               IHistory<InvStoresHistory> history,
                               IRepositoryCommand<InvStoreBranch> StoreBranchCommand,
                               IHttpContextAccessor _httpContext,
                               iDefultDataRelation iDefultDataRelation,
                               ISystemHistoryLogsService systemHistoryLogsService,
                               iUserInformation iUserInformation,
                               ISecurityIntegrationService securityIntegrationService,
                               IRepositoryQuery<GLBranch> branchesRepositoryQuery,
                               IRepositoryQuery<InvStpItemCardStores> invStpItemCardStoresQuery,
                               IRepositoryQuery<OfferPriceMaster> offerPriceMasterQuery,
                               IDeletedRecordsServices deletedRecords,
                               IRepositoryQuery<InvStoreBranch> storeBranchQuery,
                               IGeneralAPIsService generalAPIsService) : base(_httpContext)
        {
            StoresRepositoryQuery = _StoresRepositoryQuery;
            _otherSettingsStoresQuery = OtherSettingsStoresQuery;
            _invoiceMasterQuery = InvoiceMasterQuery;
            StoresRepositoryCommand = _StoresRepositoryCommand;
            httpContext = _httpContext;
            _iDefultDataRelation = iDefultDataRelation;
            _systemHistoryLogsService = systemHistoryLogsService;
            _iUserInformation = iUserInformation;
            _securityIntegrationService = securityIntegrationService;
            this.history = history;
            this.StoreBranchCommand = StoreBranchCommand;
            this.branchesRepositoryQuery = branchesRepositoryQuery;
            _InvStpItemCardStoresQuery = invStpItemCardStoresQuery;
            this.OfferPriceMasterQuery = offerPriceMasterQuery;
            _deletedRecords = deletedRecords;
            StoreBranchQuery = storeBranchQuery;
            this.generalAPIsService = generalAPIsService;
        }
        public async Task<ResponseResult> AddStore(StoresParameter parameter)
        {
            var security = await _securityIntegrationService.getCompanyInformation();
            //if (parameter.branchId == 0)
            //    return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.BranchIdIsRequired };
            if (!security.isInfinityNumbersOfStores)
            {
                var storesCount = StoresRepositoryQuery.GetAll().Count();
                if (storesCount >= security.AllowedNumberOfStore)
                    return new ResponseResult()
                    {
                        Data = null,
                        Id = null,
                        Result = Result.MaximumLength,
                        Note = Actions.YouHaveTheMaxmumOfStores,
                        ErrorMessageAr = "تجاوزت الحد الاقصي من عدد المخازن",
                        ErrorMessageEn = "You Cant add a new store because you have the maximum of stores for your bunlde"
                    };
            }
            var checkBranch = await branchesHelper.CheckIsBranchExist(new int[] { parameter.branchId }, branchesRepositoryQuery);
            if (checkBranch != null)
                return checkBranch;

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



            var ArabicStoreExist = await StoresRepositoryQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName);
            if (ArabicStoreExist != null)
                return new ResponseResult() { Data = null, Id = ArabicStoreExist.Id, Result = Result.Exist, Note = Actions.ArabicNameExist };

            var LatinStoreExist = await StoresRepositoryQuery.GetByAsync(a => a.LatinName == parameter.LatinName);
            if (LatinStoreExist != null)
                return new ResponseResult() { Data = null, Id = LatinStoreExist.Id, Result = Result.Exist, Note = Actions.EnglishNameExist };

            if (!string.IsNullOrEmpty(parameter.Phone))
            {
                if (parameter.Phone.Length < 7 && parameter.Phone.Length > 0)
                    return new ResponseResult { Result = Result.RequiredData, Note = Actions.PhoneDigitLessThan7 };
            }
            var PhoneExist = await StoresRepositoryQuery.GetByAsync(a => a.Phone == parameter.Phone && !string.IsNullOrEmpty(parameter.Phone));
            if (PhoneExist != null)
                return new ResponseResult() { Data = null, Id = PhoneExist.Id, Result = Result.Exist, Note = Actions.PhoneExist };


            int NextCode = StoresRepositoryQuery.GetMaxCode(e => e.Code) + 1;

            var table = Mapping.Mapper.Map<StoresParameter, InvStpStores>(parameter);
            table.Code = NextCode;
            table.UTime = DateTime.Now; // Set Time
            var saved = StoresRepositoryCommand.Add(table);

            List<InvStoreBranch> storeBranchList = new List<InvStoreBranch>();
            storeBranchList.Add(new InvStoreBranch() { BranchId = parameter.branchId, StoreId = table.Id });

            //foreach (var i in parameter.Branches)
            //{
            //}
            if (saved)
            {
                await _iDefultDataRelation.AdministratorUserRelation(3, table.Id);
            }
            StoreBranchCommand.AddRange(storeBranchList);


            history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Add, Aliases.TemporaryRequiredData.UserName);
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addStores);
            return new ResponseResult() { Data = null, Id = table.Id, Result = Result.Success };

        }
        public async Task<ResponseResult> GetListOfStores(StoresSearch parameters)
        {
            var userinfo = await _iUserInformation.GetUserInformation();

            var resData = await StoresRepositoryQuery.GetAllIncludingAsync(0, 0,
                a => (a.Code.ToString().Contains(parameters.Name) || string.IsNullOrEmpty(parameters.Name) || a.ArabicName.Contains(parameters.Name) || a.LatinName.Contains(parameters.Name))
                && (parameters.Status == 0 || a.Status == parameters.Status),
                e => (string.IsNullOrEmpty(parameters.Name) ?
                e.OrderByDescending(q => q.Code) :
                e.OrderBy(a => (a.Code.ToString().Contains(parameters.Name)) ? 0 : 1)),
                a => a.StoreBranches/*, w => w.CardStores, s => s.InvoiceMaster,s=> s.InvoiceMasterTo*/);

            resData = resData.Where(x => userinfo.employeeBranches.Contains(x.StoreBranches.FirstOrDefault().BranchId)).ToList();
            if (parameters.Status > 0)
                resData = resData.Where(e => e.Status == parameters.Status).ToList();

            if (parameters.BranchList != null && parameters.BranchList.Count() > 0)
            {
                resData = resData.Where(x => x.StoreBranches.Any(y => parameters.BranchList.Contains(y.BranchId))).ToList();
            }

            //resData.Where(a => a.CardStores.Count == 0 && a.InvoiceMaster.Count == 0 && a.Code != 1).Select(a => { a.CanDelete = true; return a; }).ToList();
            var count = resData.Count();
            if (parameters.PageSize > 0 && parameters.PageNumber > 0)
            {
                resData = resData.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToList();
            }
            else
            {
                return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };

            }
            List<AllStoresDto> StoreList = new List<AllStoresDto>();
            Mapping.Mapper.Map(resData, StoreList);

            var branches = branchesRepositoryQuery.TableNoTracking;
            var ItemCardStores = _InvStpItemCardStoresQuery.TableNoTracking;
            var InvoiceMaster = _invoiceMasterQuery.TableNoTracking;
            var OfferPriceMaster = OfferPriceMasterQuery.TableNoTracking;
            var res = StoreList.ToList().Select(x => new
            {
                addressAr = x.AddressAr,
                addressEn = x.AddressEn,
                arabicName = x.ArabicName,
                branchNameAr = string.Join(',', branches.Where(c => x.Branches.Contains(c.Id)).Select(c => c.ArabicName)),
                branchNameEn = string.Join(',', branches.Where(c => x.Branches.Contains(c.Id)).Select(c => c.LatinName)),
                branchId = x.Branches.FirstOrDefault(),
                canDelete = canDeleteStore(x.Id, ItemCardStores, InvoiceMaster, OfferPriceMaster).Result,
                code = x.Code,
                fax = x.Fax,
                id = x.Id,
                latinName = x.LatinName,
                notes = x.Notes,
                phone = x.Phone,
                status = x.Status
            });
            return new ResponseResult() { Data = res, DataCount = count, Id = null, Result = StoreList.Any() ? Result.Success : Result.Failed };

        }
        private async Task<bool> canDeleteStore(int storeId, IQueryable<InvStpItemCardStores> ItemCardStores, IQueryable<InvoiceMaster> InvoiceMaster, IQueryable<OfferPriceMaster> OfferPriceMaster)
        {
            if (storeId == 1)
                return false;
            else if (ItemCardStores.Where(x => x.StoreId == storeId).Any())
                return false;
            else if (InvoiceMaster.Where(x => x.StoreId == storeId || x.StoreIdTo == storeId).Any())
                return false;
            else if (OfferPriceMaster.Where(x => x.StoreId == storeId).Any())
                return false;
            return true;
        }
        public async Task<ResponseResult> UpdateStores(UpdateStoresParameter parameters)
        {


            var checkBranch = await branchesHelper.CheckIsBranchExist(new int[] { parameters.branchId }, branchesRepositoryQuery);
            if (checkBranch != null && parameters.branchId != 0)
                return checkBranch;
            if (parameters.Id == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };
            parameters.LatinName = Helpers.Helpers.IsNullString(parameters.LatinName);
            parameters.ArabicName = Helpers.Helpers.IsNullString(parameters.ArabicName);
            if (string.IsNullOrEmpty(parameters.LatinName))
                parameters.LatinName = parameters.ArabicName;

            if (string.IsNullOrEmpty(parameters.ArabicName))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };

            if (parameters.Status < (int)Status.Active || parameters.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }
            var ArabicStoresExist = await StoresRepositoryQuery.GetByAsync(a => a.ArabicName == parameters.ArabicName && a.Id != parameters.Id);

            if (ArabicStoresExist != null)
                return new ResponseResult() { Data = null, Id = ArabicStoresExist.Id, Result = Result.Exist, Note = Actions.ArabicNameExist };

            var LatinStoresExist = await StoresRepositoryQuery.GetByAsync(a => a.LatinName == parameters.LatinName && a.Id != parameters.Id);
            if (LatinStoresExist != null)
                return new ResponseResult() { Data = null, Id = LatinStoresExist.Id, Result = Result.Exist, Note = Actions.EnglishNameExist };

            if (!string.IsNullOrEmpty(parameters.Phone))
            {
                if (parameters.Phone.Length < 7 && parameters.Phone.Length > 0)
                    return new ResponseResult { Result = Result.RequiredData, Note = Actions.PhoneDigitLessThan7 };
            }
            var PhoneExist = await StoresRepositoryQuery.GetByAsync(a => a.Phone == parameters.Phone
              && !string.IsNullOrEmpty(parameters.Phone) && a.Id != parameters.Id);
            if (PhoneExist != null)
                return new ResponseResult() { Data = null, Id = PhoneExist.Id, Result = Result.Exist, Note = Actions.PhoneExist };


            var data = await StoresRepositoryQuery.GetByAsync(a => a.Id == parameters.Id);
            if (data == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.NoDataFound };

            var table = Mapping.Mapper.Map<UpdateStoresParameter, InvStpStores>(parameters, data);
            if (table.Code == 1)
                table.Status = (int)Status.Active;

            table.UTime = DateTime.Now; // Set Time

            if (parameters.branchId != 0)
            {
                if (!_invoiceMasterQuery.TableNoTracking.Where(x => x.StoreId == parameters.Id).Any())
                {
                    await StoreBranchCommand.DeleteAsync(e => e.StoreId == data.Id);
                    List<InvStoreBranch> storeBranches = new List<InvStoreBranch>();
                    storeBranches.Add(new InvStoreBranch() { BranchId = parameters.branchId, StoreId = data.Id });
                    //foreach (var item in parameters.Branches)
                    //{
                    //}

                    await StoresRepositoryCommand.UpdateAsyn(table);
                    StoreBranchCommand.AddRange(storeBranches);
                }
            }


            history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editStores);
            return new ResponseResult() { Data = null, Id = data.Id, Result = data == null ? Result.Failed : Result.Success };

        }
        public async Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameters)
        {
            if (parameters.Id.Count() == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            if (parameters.Status < (int)Status.Active || parameters.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }
            var stores = StoresRepositoryQuery.TableNoTracking.Where(e => parameters.Id.Contains(e.Id) && e.Code != 1);
            var StoresList = stores.ToList();

            StoresList.Select(e => { e.Status = parameters.Status; return e; }).ToList();
            //if (parameters.Id.Contains(1))
            //    StoresList.Where(q => q.Code == 1).Select(e => { e.Status = (int)Status.Active; return e; }).ToList();

            var result = await StoresRepositoryCommand.UpdateAsyn(StoresList);

            foreach (var store in StoresList)
            {
                history.AddHistory(store.Id, store.LatinName, store.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);

            }
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editStores);
            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };

        }
        public async Task<ResponseResult> DeleteStores(SharedRequestDTOs.Delete ListCode)
        {
            if (ListCode.Ids.Contains(1))
            {
                ListCode.Ids = ListCode.Ids.Where(c => c != 1).ToArray();
            }
            var stores = await StoresRepositoryQuery
                .GetAllIncludingAsync(0, 0,
                e => ListCode.Ids.Contains(e.Id) && e.Code != 1, w => w.CardStores, x => x.InvoiceMaster, x => x.OfferPriceMaster);
            if (!stores.Any())
            {
                return new ResponseResult() { Result = Result.NoDataFound};

            }
            List<int> deletedStores = new List<int>();
            foreach (var store in stores)
            {
                if (store.CardStores.Count == 0 && store.InvoiceMaster.Count == 0 && store.OfferPriceMaster.Count == 0)
                {
                    deletedStores.Add(store.Id);
                    var result = await StoresRepositoryCommand.DeleteAsync(store.Id);
                    if (result)
                        await StoreBranchCommand.DeleteAsync(e => ListCode.Ids.Contains(e.StoreId));
                }
            }

            //Fill The DeletedRecordTable
            _deletedRecords.SetDeletedRecord(deletedStores, 4);

            /* var listRecords = new List<DeletedRecords>();
             foreach (var item in deletedStores)
             {
                 var deleted = new DeletedRecords
                 {
                     Type = 4,
                     DTime = DateTime.Now,
                     RecordID = item
                 };
                 listRecords.Add(deleted);
             }
             _deletedRecordCommand.AddRangeAsync(listRecords);*/
            int count = stores.Count;
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteStores,count);
            return new ResponseResult() { Data = deletedStores, Id = null, Result = deletedStores.Any() ? Result.Success : Result.Failed };
        }
        public async Task<ResponseResult> GetStoreHistory(int Code)
        {
            return await history.GetHistory(a => a.EntityId == Code);
        }
        public async Task<ResponseResult> GetActiveStoresDropDown()
        {
            var userinfo = await _iUserInformation.GetUserInformation();
            var StoresList = StoresRepositoryQuery
                            .TableNoTracking
                            .Include(x => x.StoreBranches)
                            .Where(x => userinfo.employeeId != 1 ? userinfo.userStors.Contains(x.Id) : true)
                            .Where(e => e.Status == (int)Status.Active)
                            .Select(a => new { a.Id, a.Code, a.ArabicName, a.LatinName,
                            });
            return new ResponseResult() { Data = StoresList, Id = null, Result = StoresList.Any() ? Result.Success : Result.Failed };

        }
        public async Task<ResponseResult> GetAllStoresDropDown(int? invoiceTypeId)
        {
            var userinfo = await _iUserInformation.GetUserInformation();
            var StoresList = StoresRepositoryQuery
                            .TableNoTracking
                            .Include(x => x.StoreBranches)
                           .Where(x =>(invoiceTypeId==(int)DocumentType.OutgoingTransfer?true: userinfo.employeeBranches.Contains(x.StoreBranches.FirstOrDefault().BranchId)))
                            .Select(a => new { a.Id, 
                             code=   a.Code,
                              ArName = a.ArabicName, 
                             EnName=   a.LatinName, 
                              status=  a.Status,
                             
                            });

            return new ResponseResult() { Data = StoresList, Id = null, Result = StoresList.Any() ? Result.Success : Result.Failed };

        }

        public async Task<ResponseResult> GetAllActiveStoresDropDownForInvoices(int? invoiceId, bool isTransfer = false, int invoiceTypeId = 0)
        {
            var userinfo = await _iUserInformation.GetUserInformation();
            var userStores = _otherSettingsStoresQuery.TableNoTracking.Where(x => x.otherSettingsId == userinfo.otherSettings.Id).Select(x => x.InvStpStoresId).ToList();

            int invoiceStoreId = 0;
            if (!string.IsNullOrEmpty(invoiceId.ToString()))
            {
                if (invoiceTypeId != 0 && invoiceTypeId != null)
                    invoiceStoreId = OfferPriceMasterQuery.GetByIdAsync(invoiceId.Value).Result.StoreId;
                else
                    invoiceStoreId = _invoiceMasterQuery.GetByIdAsync(invoiceId.Value).Result.StoreId;
            }

            var stores = StoresRepositoryQuery
                            .TableNoTracking
                            .Include(x => x.StoreBranches);


            var StoresList = stores.Where(e => e.Status == (int)Status.Active).ToList();

            if (invoiceStoreId != 0)
            {
                if (!StoresList.Where(x => x.Id == invoiceStoreId).Any())
                {
                    var invoiceStore = stores.Where(x => x.Id == invoiceStoreId).FirstOrDefault();
                    StoresList.Add(invoiceStore);
                }
            }

            if (userinfo.CurrentbranchId == 1 && userinfo.userId == 1)
            {
                userStores.Add(1);
            }


            var res = StoresList
                            .Where(x => !isTransfer ? userinfo.CurrentbranchId == x.StoreBranches.FirstOrDefault().BranchId : true)
                            .Select(a => new { a.Id, a.Code, a.ArabicName, a.LatinName, a.Status })
                            .ToList();
            if (userinfo.userId != 1)
                res = res.Where(x => userStores.Contains(x.Id)).ToList();


            return new ResponseResult() { Data = res, Id = null, Result = StoresList.Any() ? Result.Success : Result.Failed };

        }

        public async Task<ResponseResult> GetAllStoreChangesAfterDate(DateTime date, int PageNumber, int PageSize )
        {
            var resData = await StoresRepositoryQuery.TableNoTracking
                .Where(c => c.UTime >= date)
                .Include(s=>s.StoreBranches)
                .ToListAsync(); 

            return await generalAPIsService.Pagination(resData, PageNumber, PageSize);
        }
    }

}
