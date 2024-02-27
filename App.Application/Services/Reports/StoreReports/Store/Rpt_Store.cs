using App.Domain.Entities.Setup;
using App.Domain.Models.Request.Store.Reports.Store;
using App.Domain.Models.Response.Store.Reports.Store;
using App.Domain.Models.Response.Store.Reports.Stores;
using Microsoft.AspNetCore.Authentication;
using App.Infrastructure.settings;
using static App.Application.Services.Reports.StoreReports.Sales.RPT_Sales;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using MediatR;
using App.Application.Handlers.Reports.Store.Store.DetailedTransactoinsOfItem;
using App.Domain.Entities.Process;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Setup.ItemCard.Response;
using System.Security.Cryptography.X509Certificates;
using App.Application.Services.Printing.PrintFile;
using App.Application.Services.Printing.InvoicePrint;

namespace App.Application.Services.Reports.Items_Prices
{
    public class Rpt_Store : BaseClass, IRpt_Store
    {
        private readonly IRepositoryQuery<InvoiceDetails> _InvoiceDetailsQuery;
        private readonly IRepositoryQuery<InvoiceMaster> _invoiceMasterQuery;
        private readonly IRepositoryQuery<InvStpItemCardStores> _InvStpItemCardStoresQuery;
        private readonly IRepositoryQuery<InvStpItemCardUnit> _InvStpItemCardUnitQuery;
        private readonly IRepositoryQuery<InvStpUnits> _invStpUnitsQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> _invStpItemCardMasterQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> _invGeneralSettingsQuery;
        private readonly IRepositoryQuery<InvSerialTransaction> _InvSerialTransactionQuery;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IPrintService _iprintService;
        private readonly IGeneralAPIsService _IGeneralAPIsService;

        private readonly IprintFileService _iPrintFileService;
        private readonly IFilesMangerService _filesMangerService;
        private readonly ICompanyDataService _CompanyDataService;
        private readonly iUserInformation _iUserInformation;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IRoundNumbers roundNumbers;
        private readonly IReportFileService _iReportFileService;
        private readonly IRepositoryQuery<InvStpItemCardMaster> _itemCardMasterRepository;
        private readonly IRepositoryQuery<InvStpStores> _storeQuery;
        private readonly IGeneralPrint _iGeneralPrint;
        private readonly IMediator _mediator;




        public Rpt_Store(
                                IRepositoryQuery<InvoiceDetails> InvoiceDetailsQuery,
                                IRepositoryQuery<InvoiceMaster> InvoiceMasterQuery,
                                IRepositoryQuery<InvStpUnits> InvStpUnitsQuery,
                                IRepositoryQuery<InvStpItemCardMaster> InvStpItemCardMasterQuery,
                                IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsQuery,
                                IHttpContextAccessor httpContext, IPrintService iprintService,
            IprintFileService iPrintFileService, IFilesMangerService filesMangerService,
            ICompanyDataService CompanyDataService,
            iUserInformation iUserInformation,
            IWebHostEnvironment webHostEnvironment,
                     IRoundNumbers roundNumbers,
            IReportFileService iReportFileService, IRepositoryQuery<InvStpStores> storeQuery,
            IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository, IGeneralPrint iGeneralPrint, IGeneralAPIsService iGeneralAPIsService, IRepositoryQuery<InvSerialTransaction> invSerialTransactionQuery, IMediator mediator, IRepositoryQuery<InvStpItemCardStores> InvStpItemCardStoresQuery, IRepositoryQuery<InvStpItemCardUnit> invStpItemCardUnitQuery) : base(httpContext)
        {
            _InvoiceDetailsQuery = InvoiceDetailsQuery;
            _invoiceMasterQuery = InvoiceMasterQuery;
            _invStpUnitsQuery = InvStpUnitsQuery;
            _invStpItemCardMasterQuery = InvStpItemCardMasterQuery;
            _invGeneralSettingsQuery = InvGeneralSettingsQuery;
            _httpContext = httpContext;
            _iprintService = iprintService;
            _iPrintFileService = iPrintFileService;
            _filesMangerService = filesMangerService;
            _CompanyDataService = CompanyDataService;
            _iUserInformation = iUserInformation;
            _webHostEnvironment = webHostEnvironment;
            this.roundNumbers = roundNumbers;
            _iReportFileService = iReportFileService;
            _itemCardMasterRepository = itemCardMasterRepository;
            _storeQuery = storeQuery;
            _iGeneralPrint = iGeneralPrint;
            _IGeneralAPIsService = iGeneralAPIsService;
            _InvSerialTransactionQuery = invSerialTransactionQuery;
            _mediator = mediator;
            _InvStpItemCardStoresQuery = InvStpItemCardStoresQuery;
            _InvStpItemCardUnitQuery = invStpItemCardUnitQuery;
        }

        public async Task<ResponseResult> DetailedMovementOfanItem(DetailedMovementOfanItemReqest param, bool isPrint = false)
        {
            #region validation
            if (string.IsNullOrEmpty(_httpContext.HttpContext.GetTokenAsync("access_token").Result))
                return new ResponseResult()
                {
                    Note = "UnAuthorized",
                    Result = Result.Failed
                };
            if (string.IsNullOrEmpty(param.dateFrom.ToString()) || string.IsNullOrEmpty(param.dateTo.ToString()))
                return new ResponseResult()
                {
                    Note = "Date from and Date to is Required",
                    Result = Result.Failed
                };
            if (!isPrint)
            {
                if (string.IsNullOrEmpty(param.PageNumber.ToString()) || string.IsNullOrEmpty(param.PageSize.ToString()))
                    return new ResponseResult()
                    {
                        Note = "Page Number and Page Size are Required",
                        Result = Result.Failed
                    };
            }

            if (string.IsNullOrEmpty(param.dateFrom.ToString()) || string.IsNullOrEmpty(param.dateTo.ToString()))
                return new ResponseResult()
                {
                    Note = "Date From And Date To are Required",
                    Result = Result.Failed
                };
            #endregion
            var returnInvoicesList = Lists.returnInvoiceList;
            var Transfers = Lists.Transfers;
            var transactionList = TransactionTypeList.transactionTypeModels();
            var stores = _storeQuery.TableNoTracking;
            var InvoicesOfItem = _InvoiceDetailsQuery.TableNoTracking
                .Include(x => x.InvoicesMaster)
                .Include(x => x.InvoicesMaster.storeTo)
                .Include(x => x.InvoicesMaster.store)
                .Include(x => x.Items)
                .Include(x => x.Items.Units)
                .Where(x => x.ItemId == param.itemId)
                .Where(x => x.InvoicesMaster.StoreId == param.storeId)
                .Where(x => !x.InvoicesMaster.IsDeleted)
                .ToList()
                .GroupBy(x => new { x.InvoiceId })
                .OrderBy(x => x.First().InvoicesMaster.InvoiceDate)
                .Select(x => new firstSelection
                {
                    id = x.First().InvoiceId,
                    date = x.First().InvoicesMaster.InvoiceDate.Date,
                    DocumentCode = x.First().InvoicesMaster.InvoiceType.Replace("_", string.Empty),
                    Qyt = ReportData<InvoiceDetails>.Quantity(x, x.First().Items.Units.Where(c => c.UnitId == param.unitId).FirstOrDefault()?.ConversionFactor ?? 0),
                    rowClassName = returnInvoicesList.Contains(x.First().InvoicesMaster.InvoiceTypeId) || x.First().InvoicesMaster.InvoiceSubTypesId == (int)SubType.RejectedTransfer ? defultData.text_danger : "",
                    TransactionTypeAr = (!Transfers.Where(c => c == x.First().InvoicesMaster.InvoiceTypeId).Any() ? transactionList.Find(f => f.id == x.First().InvoicesMaster.InvoiceTypeId)?.arabicName ?? "" : _IGeneralAPIsService.GetTransferDesc(x.First().InvoicesMaster.InvoiceTypeId, x.First().InvoicesMaster.StoreId, x.First().InvoicesMaster.StoreIdTo, x.First().InvoicesMaster.InvoiceSubTypesId == (int)SubType.RejectedTransfer ? true : false).descAr) + (x.First().InvoicesMaster.InvoiceType != x.First().InvoicesMaster.ParentInvoiceCode  && !string.IsNullOrEmpty(x.First().InvoicesMaster.ParentInvoiceCode) ? " - (" + x.First().InvoicesMaster.ParentInvoiceCode + ")" : ""),
                    TransactionTypeEn = (!Transfers.Where(c => c == x.First().InvoicesMaster.InvoiceTypeId).Any() ? transactionList.Find(f => f.id == x.First().InvoicesMaster.InvoiceTypeId)?.latinName ?? "" : _IGeneralAPIsService.GetTransferDesc(x.First().InvoicesMaster.InvoiceTypeId, x.First().InvoicesMaster.StoreId, x.First().InvoicesMaster.StoreIdTo, x.First().InvoicesMaster.InvoiceSubTypesId == (int)SubType.RejectedTransfer ? true : false).descEn)  + (x.First().InvoicesMaster.InvoiceType != x.First().InvoicesMaster.ParentInvoiceCode  && !string.IsNullOrEmpty(x.First().InvoicesMaster.ParentInvoiceCode) ? " - (" + x.First().InvoicesMaster.ParentInvoiceCode + ")" : ""),
                    notes = x.First().InvoicesMaster.Notes,
                    invoiceTypeId = x.First().InvoicesMaster.InvoiceTypeId,
                    parentType = x.First().InvoicesMaster.ParentInvoiceCode,
                    Serialize = x.First().InvoicesMaster.Serialize
                }).Where(x => x.Qyt != 0)
                  .ToList();
            
            var finalBalance = InvoicesOfItem.Sum(x => x.Qyt);
            #region Unit Valiation
            var checkIfItemHaveRequestedUnit = _invStpItemCardMasterQuery.TableNoTracking.Include(x => x.Units).Where(x => x.Id == param.itemId);
            if (checkIfItemHaveRequestedUnit.Any())
            {
                if (!checkIfItemHaveRequestedUnit.FirstOrDefault().Units.Where(x => x.UnitId == param.unitId).Any())
                    return new ResponseResult()
                    {
                        Note = Actions.ItemUnitIsNotExist,
                        Result = Result.Failed
                    };
            }
            else
            {
                return new ResponseResult()
                {
                    Note = Actions.ItemIsNotExist,
                    Result = Result.Failed
                };
            }
            #endregion



            var PreviousBalanceInvoices = InvoicesOfItem.Where(x => x.date.Date < param.dateFrom.Date).ToList();
            var PreviousBalance = InvoicesOfItem.Where(x => x.date.Date < param.dateFrom.Date).Sum(x => x.Qyt);
            var invoicesInDate = InvoicesOfItem.Where(x => x.date.Date >= param.dateFrom.Date && x.date.Date <= param.dateTo.Date)
                                               .OrderBy(x => x.date)
                                               .ThenBy(x=> x.Serialize);


            var data = new List<Data>();
            var prevBalanceElement = new Data
            {
                date = "",
                TransactionTypeAr = "  السابق",
                TransactionTypeEn = "Previous Balance",
                IncomingBalanc = 0,
                OutgoingBalanc = 0,
                Balanc = roundNumbers.GetRoundNumber(PreviousBalance)
            };
            bool isPrevBalanceAdded = true;
            double balance = 0;
            if (isPrevBalanceAdded && PreviousBalance != 0)
                data.Add(prevBalanceElement);
            foreach (var item in invoicesInDate)
            {
                if (isPrevBalanceAdded)
                {
                    balance = PreviousBalance;
                    isPrevBalanceAdded = false;
                }
                balance += item.Qyt;
                balance = roundNumbers.GetRoundNumber(balance);
                var dta = new Data()
                {
                    id = item.id,
                    date = item.date.ToString(defultData.datetimeFormat),
                    rowClassName = item.rowClassName,
                    DocumentCode = item.DocumentCode,
                    notes = item.notes,
                    TransactionTypeAr = item.TransactionTypeAr,
                    TransactionTypeEn = item.TransactionTypeEn,
                    IncomingBalanc = item.Qyt > 0 ? roundNumbers.GetRoundNumber(item.Qyt) : 0,
                    OutgoingBalanc = item.Qyt < 0 ? roundNumbers.GetRoundNumber(Math.Abs(item.Qyt)) : 0,
                    Balanc = roundNumbers.GetRoundNumber(balance),
                    Serialize = item.Serialize
                };
                data.Add(dta);
            }

            var _InvoicesInDate = isPrint ? data : Pagenation<Data>.pagenationList(param.PageSize, param.PageNumber, data);

            
                
            var response = new DetailedMovementOfanItemResponse()
            {
                PreviousBalance = PreviousBalance,
                data = _InvoicesInDate
            };
            double MaxPageNumber = invoicesInDate.ToList().Count() / Convert.ToDouble(param.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            var count = data.Count();
            return new ResponseResult()
            {
                Data = response,
                Result = Result.Success,
                Note = (countofFilter == param.PageNumber ? Actions.EndOfData : ""),
                DataCount = response.data.Count,
                TotalCount = count
            };
        }
        public async Task<WebReport> DetailedMovementOfanItemReport(DetailedMovementOfanItemReqest param, exportType exportType, bool isArabic, int fileId = 0)
        {

            //var data = await DetailedMovementOfanItem(param, true);
            
            var data = await _mediator.Send(new DetailedTransactoinsOfItemRequest
            {
                dateFrom = param.dateFrom,
                itemId = param.itemId,
                unitId = param.unitId,
                isPrint = true,
                dateTo = param.dateTo,
                PageNumber = param.PageNumber,
                PageSize = param.PageSize,
                storeId  = param.storeId
            });

            var userInfo = await _iUserInformation.GetUserInformation();
            var items = _itemCardMasterRepository.TableNoTracking.Where(x => x.Id == param.itemId).
                Select(x => new
                {

                    x.Id,
                    x.ArabicName,
                    x.LatinName,
                    x.ItemCode
                }).FirstOrDefault();

            var units = _invStpUnitsQuery.TableNoTracking.Where(x => x.Id == param.unitId).Select(x => new
            {
                x.Id,
                x.ArabicName,
                x.LatinName
            }).FirstOrDefault();
            var stores = _storeQuery.TableNoTracking
                        .Where(x => x.Id == param.storeId).Select(x => new
                        {
                            x.Id,
                            x.ArabicName,
                            x.LatinName
                        }).FirstOrDefault();

            //var itemDatad = _itemCardMasterRepository.TableNoTracking.Include(a => a.Category).
            //    Include(a => a.StorePlace).Where(x => x.Id == param.itemId);
           
          
        //    var itemData = itemDatad.
        //        Select(x => new ItemCardMainData
        //        {

        //            Id = x.Id,
        //            ArabicName = x.ArabicName,
        //            LatinName = x.LatinName,
        //            ItemCode = x.ItemCode,
        //            TypeId = x.TypeId,
        //            NationalBarcode = x.NationalBarcode,
        //            VAT = x.VAT,
        //            Model = x.Model,
        //            Description = x.Description,
        //            Status = x.Status,
        //            CategoryNameAr = x.Category!=null? x.Category.ArabicName:"",
        //            CatogeryNameEn = x.Category != null ? x.Category.LatinName:"",
        //            StorePlaceAr = x.StorePlace !=null? x.StorePlace.ArabicName:"",
        //            StorePlaceEn = x.StorePlace != null ? x.StorePlace.LatinName:"",
        //            StorePlaceStatus = x.StorePlace != null ? x.StorePlace.Status:0,
        //            StoreNameAr = stores != null ? stores.ArabicName :"",
        //            StoreNameEn = stores != null ? stores.LatinName :"",
        //            UnitNameAr = units != null ? units.ArabicName : "",
        //            UnitNameEn = units != null ? units.LatinName : ""
        //}).FirstOrDefault();

          var dates= ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, param.dateFrom, param.dateTo);
            AdditionalReportDataStore AdditionalReportdata = new AdditionalReportDataStore()
            {
                Code = items.ItemCode,
                ItemNameAr = items.ArabicName,
                ItemNameEn = items.LatinName,

                UnitNameAr = units.ArabicName,
                UnitNameEn = units.LatinName,

                ArabicName = stores.ArabicName,
                LatinName = stores.LatinName,

                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),

                DateFrom = dates.DateFrom,
                DateTo = dates.DateTo,
                Date = dates.Date

            };

            var resData = (DetailedMovementOfanItemResponse)(data.Data);
            DateTime date;
           
            foreach (var item in resData.data)
            {
                if(item.date==null || item.date == "")
                {
                    item.date = "----------";
                }
                else
                {
                    date = Convert.ToDateTime(item.date);
                    item.date = date.ToString("yyyy/MM/dd");

                }
               
            }
            var tablesNames = new TablesNames()
            {
                
                FirstListName = "Data"
            };




            var report = await _iGeneralPrint.PrintReport<object, Data, object>( null, resData.data, null, tablesNames, AdditionalReportdata
             , (int)SubFormsIds.DetailedMovementOfAnItem_Repository, exportType, isArabic,fileId);
            return report;


        }
        public double ConvertQyt(int itemId, int ReportUnitId, double qyt)
        {
            var item = _invStpItemCardMasterQuery.TableNoTracking.Include(x => x.Units).Where(x => x.Id == itemId);
            InvStpItemCardUnit ItemUnit = new InvStpItemCardUnit();
            if (!item.Any())
                return 0;

            var FirstUnitOfitem = item.FirstOrDefault().Units.FirstOrDefault().ConversionFactor;
            var QytWithFirstUnit = qyt * FirstUnitOfitem;
            var ConversionFactorForReportUnit = item.FirstOrDefault().Units.Where(x => x.UnitId == ReportUnitId).FirstOrDefault().ConversionFactor;
            var FinalQyt = QytWithFirstUnit / ConversionFactorForReportUnit;

            return Round(FinalQyt);
        }
        public double Round(double Num)
        {
            var MidpointRounding = _invGeneralSettingsQuery.TableNoTracking.First().Other_Decimals;
            return roundNumbers.GetRoundNumber(Num);
        }

        public async Task<InventoryValuationResponse> getInventoryValuation(InventoryValuationRequest param, bool isPrint = false)
        {
            #region task
            var invoicesDetalis = _InvoiceDetailsQuery.TableNoTracking
                                                .Include(x => x.Items)
                                                .Include(x => x.InvoicesMaster)
                                                .Where(x => x.parentItemId == null)
                                                .Where(x =>
                                                        param.storeId != -1 ? x.InvoicesMaster.StoreId == param.storeId : true &&
                                                        x.InvoicesMaster.InvoiceDate.Date <= param.dateTo.Value.Date &&
                                                        x.ItemTypeId != 6);

            if (param.itemId != null)
                invoicesDetalis = invoicesDetalis.Where(x => x.ItemId == param.itemId);
            if (param.CategoryId != 0)
                invoicesDetalis = invoicesDetalis.Where(x => x.Items.GroupId == param.CategoryId);




            var itemIds = invoicesDetalis.GroupBy(x => x.ItemId).Select(y => y.OrderBy(x => x.Id).LastOrDefault()).ToList();
            var itemsPrices = _InvStpItemCardUnitQuery.TableNoTracking;
            var count = itemIds.Count();

            var items = _invStpItemCardMasterQuery.TableNoTracking.Include(x => x.Units).Where(x => x.TypeId != (int)ItemTypes.Note);

            var units = _invStpUnitsQuery.TableNoTracking;

            //double totalItemsPrice = SumTotalPriceForInventoryValuation(param.inventoryValuationType, invoicesDetalis, items, itemsPrices);
            List<InventoryValuationData> ListinventoryValuationData = new List<InventoryValuationData>();


            foreach (var item in itemIds)
            {
                double itemPrice = 0;
                var _ReportUnit = items.Where(x => x.Id == item.ItemId).FirstOrDefault();

                if (_ReportUnit == null)
                    continue;
                var ReportUnit = items.Where(x => x.Id == item.ItemId)?.FirstOrDefault().ReportUnit??0;

                var itemInfo = items.Where(x => x.Id == item.ItemId)
                    .FirstOrDefault();
                var UnitInfo = itemInfo.Units
                           .Where(x => x.UnitId == ReportUnit)
                           .FirstOrDefault();
                double Balance = invoicesDetalis.Where(x => x.ItemId == item.ItemId).Where(x=> x.InvoicesMaster.InvoiceDate.Date <= param.dateTo.Value.Date).Select(x => new { Quantity = x.Quantity * x.ConversionFactor, signal = x.Signal }).Sum(x => x.Quantity * x.signal) / UnitInfo.ConversionFactor;
                if (Balance == 0)
                {
                    count--;
                    continue;
                }
                InventoryValuationData inventoryValuationData = new InventoryValuationData();
                inventoryValuationData.itemCode = items.Where(x => x.Id == item.ItemId).FirstOrDefault()?.ItemCode;
                inventoryValuationData.ArabicName = items.Where(x => x.Id == item.ItemId).FirstOrDefault()?.ArabicName;
                inventoryValuationData.latinName = items.Where(x => x.Id == item.ItemId).FirstOrDefault()?.LatinName;

                inventoryValuationData.unitNameAr = units.Where(x => x.Id == ReportUnit).FirstOrDefault()?.ArabicName;
                inventoryValuationData.unitNameEn = units.Where(x => x.Id == ReportUnit).FirstOrDefault()?.LatinName;

                inventoryValuationData.Balance = roundNumbers.GetRoundNumber(Balance);




                if (param.inventoryValuationType == InventoryValuationType.PurchasePrice)
                    itemPrice = UnitInfo.PurchasePrice;
                else if (param.inventoryValuationType == InventoryValuationType.SalesPrice1)
                    itemPrice = UnitInfo.SalePrice1;
                else if (param.inventoryValuationType == InventoryValuationType.SalesPrice2)
                    itemPrice = UnitInfo.SalePrice2;
                else if (param.inventoryValuationType == InventoryValuationType.SalesPrice3)
                    itemPrice = UnitInfo.SalePrice3;
                else if (param.inventoryValuationType == InventoryValuationType.SalesPrice4)
                    itemPrice = UnitInfo.SalePrice4;
                else if (param.inventoryValuationType == InventoryValuationType.CostPrice)
                {
                    var itemCosts = invoicesDetalis.Where(x => x.ItemId == item.ItemId && x.InvoicesMaster.IsDeleted == false).OrderByDescending(a => a.InvoicesMaster.Serialize);
                    var ItemCost = roundNumbers.GetRoundNumber(itemCosts.FirstOrDefault().Cost * UnitInfo.ConversionFactor);
                    itemPrice = itemCosts.Any() ? ItemCost : itemsPrices.Where(x => x.ItemId == item.ItemId).FirstOrDefault().PurchasePrice;
                }
                else if (param.inventoryValuationType == InventoryValuationType.AverageSalesPrice)
                    itemPrice = invoicesDetalis.Where(x => x.ItemId == item.ItemId && x.Signal == -1).Select(x => new { price = (x.Quantity * x.ConversionFactor) * x.Price }).Sum(x => x.price) / invoicesDetalis.Where(x => x.ItemId == item.ItemId && x.Signal == -1).Select(x => new { Quantity = x.Quantity * x.ConversionFactor }).Sum(x => x.Quantity);
                else if (param.inventoryValuationType == InventoryValuationType.AveragePurchaesPrice)
                    itemPrice = invoicesDetalis.Where(x => x.ItemId == item.ItemId && x.Signal == 1).Select(x => new { price = (x.Quantity * x.ConversionFactor) * x.Price }).Sum(x => x.price) / invoicesDetalis.Where(x => x.ItemId == item.ItemId && x.Signal == 1).Select(x => new { Quantity = x.Quantity * x.ConversionFactor }).Sum(x => x.Quantity);




                inventoryValuationData.price = roundNumbers.GetRoundNumber(itemPrice);
                double totalPrice = itemPrice * Balance;
                inventoryValuationData.totalPrice = roundNumbers.GetRoundNumber(totalPrice);
                //totalItemsPrice += inventoryValuationData.totalPrice;

                ListinventoryValuationData.Add(inventoryValuationData);
            }
            var _ListinventoryValuationData = isPrint ? ListinventoryValuationData : ListinventoryValuationData.Skip((param.PageNumber - 1) * param.PageSize).Take(param.PageSize).ToList();
            double MaxPageNumber = count / Convert.ToDouble(param.PageSize);

            var countofFilter = Math.Ceiling(MaxPageNumber);
            #endregion

            InventoryValuationResponse inventoryValuationResponse = new InventoryValuationResponse()
            {
                totalItemsPrice = roundNumbers.GetRoundNumber(ListinventoryValuationData.Sum(x=> x.totalPrice)),
                Items = _ListinventoryValuationData,
                Note = (countofFilter == param.PageNumber ? Actions.EndOfData : ""),
                DataCount = count,
                TotalCount = count
            };
            return inventoryValuationResponse;
        }
        public async Task<ResponseResult> InventoryValuation(InventoryValuationRequest param, bool isPrint = false)
        {
            #region Checker
            if (string.IsNullOrEmpty(param.storeId.ToString()))
                return new ResponseResult()
                {
                    Note = "Store Id is Required",
                    Result = Result.Failed
                };
            if (string.IsNullOrEmpty(((int)param.inventoryValuationType).ToString()))
                return new ResponseResult()
                {
                    Note = "inventory Valuation Type Id is Required",
                    Result = Result.Failed
                };
            if (string.IsNullOrEmpty(param.dateTo.ToString()))
                return new ResponseResult()
                {
                    Note = "Ddate To is Required",
                    Result = Result.Failed
                };
            if (param.inventoryValuationType == 0)
                return new ResponseResult()
                {
                    Note = "inventory Valuation Type is Required",
                    Result = Result.Failed
            };
            #endregion
            var inventoryValuationResponse = await getInventoryValuation(param, isPrint);


            return new ResponseResult()
            {
                Data = inventoryValuationResponse,
                Result = Result.Success,
                Note = inventoryValuationResponse.Note,
                DataCount = inventoryValuationResponse.DataCount,
                TotalCount = inventoryValuationResponse.TotalCount
            };
        }
        public async Task<WebReport> InventoryValuationReport(InventoryValuationRequest param, exportType exportType, bool isArabic, int fileId = 0)
        {

            if (param.itemId == 0)
            {
                param.itemId = null;
            }
            var data = await InventoryValuation(param, true);
            var mainData = (InventoryValuationResponse)data.Data;
            var userInfo = await _iUserInformation.GetUserInformation();

            var stores = _storeQuery.TableNoTracking
                        .Where(x => x.Id == param.storeId).Select(x => new
                        {
                            x.Id,
                            x.ArabicName,
                            x.LatinName
                        }).FirstOrDefault();
            string salesTypeAr = "";
            string salesTypeEn = "";


            #region SalesTypeOptions
            if ((int)param.inventoryValuationType == 1)
            {
                salesTypeAr = "سعر شراء";
                salesTypeEn = "Purchase Price";
            }
            else if ((int)param.inventoryValuationType == 2)
            {
                salesTypeAr = "سعر بيع 1";
                salesTypeEn = "Sales Price 1";
            }
            else if ((int)param.inventoryValuationType == 3)
            {
                salesTypeAr = "سعر بيع 2";
                salesTypeEn = "Sales Price 2";
            }
            else if ((int)param.inventoryValuationType == 4)
            {
                salesTypeAr = "سعر بيع 3";
                salesTypeEn = "Sales Price 3";
            }
            else if ((int)param.inventoryValuationType == 5)
            {
                salesTypeAr = "سعر بيع 4";
                salesTypeEn = "Sales Price 4";
            }
            else if ((int)param.inventoryValuationType == 6)
            {
                salesTypeAr = "سعر التكلفة";
                salesTypeEn = "Cost Price";
            }
            else if ((int)param.inventoryValuationType == 7)
            {
                salesTypeAr = "متوسط سعر البيع";
                salesTypeEn = "Average Sales Price";
            }
            else if ((int)param.inventoryValuationType == 8)
            {
                salesTypeAr = "متوسط سعر الشراء";
                salesTypeEn = "Average Purchase Price";
            }
            #endregion


            
            var AdditionalReportdata = new AdditionalReportData()
            {

                SalesTypeAr = salesTypeAr,
                SalesTypeEn = salesTypeEn,
                ArabicName = stores.ArabicName,
                LatinName = stores.LatinName,

                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),
                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),

            };




            var tablesNames = new TablesNames()
            {
                ObjectName = "InventoryValuation",
                FirstListName = "InventoryValuationList"

            };




            var report = await _iGeneralPrint.PrintReport<InventoryValuationResponse, InventoryValuationData, object>(mainData, mainData.Items, null, tablesNames, AdditionalReportdata
             , (int)SubFormsIds.InventoryValuation_Repository, exportType, isArabic,fileId);
            return report;
        }

        public static double SumTotalPriceForInventoryValuation(InventoryValuationType inventoryValuationType, IQueryable<InvoiceDetails> invoicesDetalis, IQueryable<InvStpItemCardMaster> items, IQueryable<InvStpItemCardUnit> itemsPrices)
        {
            double totalPrice = 0;
            var itemIds = invoicesDetalis.GroupBy(x => x.ItemId).Select(y => y.FirstOrDefault()).ToList();
            foreach (var item in itemIds)
            {
                double itemPrice = 0;

                var ReportUnit = items.Where(x => x.Id == item.ItemId).FirstOrDefault().ReportUnit;

                var itemInfo = items.Where(x => x.Id == item.ItemId)
                    .FirstOrDefault();
                var UnitInfo = itemInfo.Units
                           .Where(x => x.UnitId == ReportUnit)
                           .FirstOrDefault();
                double Balance = invoicesDetalis.Where(x => x.ItemId == item.ItemId).Select(x => new { Quantity = x.Quantity * x.ConversionFactor, signal = x.Signal }).Sum(x => x.Quantity * x.signal) / UnitInfo.ConversionFactor;
                if (inventoryValuationType == InventoryValuationType.PurchasePrice)
                    itemPrice = UnitInfo.PurchasePrice;
                else if (inventoryValuationType == InventoryValuationType.SalesPrice1)
                    itemPrice = UnitInfo.SalePrice1;
                else if (inventoryValuationType == InventoryValuationType.SalesPrice2)
                    itemPrice = UnitInfo.SalePrice2;
                else if (inventoryValuationType == InventoryValuationType.SalesPrice3)
                    itemPrice = UnitInfo.SalePrice3;
                else if (inventoryValuationType == InventoryValuationType.SalesPrice4)
                    itemPrice = UnitInfo.SalePrice4;
                else if (inventoryValuationType == InventoryValuationType.CostPrice)
                {
                    var itemCosts = invoicesDetalis.Where(x => x.ItemId == item.ItemId && x.InvoicesMaster.IsDeleted == false).OrderByDescending(a => a.InvoicesMaster.Serialize);
                    var ItemCost = itemCosts.FirstOrDefault().Cost * UnitInfo.ConversionFactor;
                    itemPrice = itemCosts.Any() ? ItemCost : itemsPrices.Where(x => x.ItemId == item.ItemId).FirstOrDefault().PurchasePrice;
                }
                else if (inventoryValuationType == InventoryValuationType.AverageSalesPrice)
                    itemPrice = invoicesDetalis.Where(x => x.ItemId == item.ItemId && x.Signal == -1).Select(x => new { price = (x.Quantity * x.ConversionFactor) * x.Price }).Sum(x => x.price) / invoicesDetalis.Where(x => x.ItemId == item.ItemId && x.Signal == -1).Select(x => new { Quantity = x.Quantity * x.ConversionFactor }).Sum(x => x.Quantity);
                else if (inventoryValuationType == InventoryValuationType.AveragePurchaesPrice)
                    itemPrice = invoicesDetalis.Where(x => x.ItemId == item.ItemId && x.Signal == 1).Select(x => new { price = (x.Quantity * x.ConversionFactor) * x.Price }).Sum(x => x.price) / invoicesDetalis.Where(x => x.ItemId == item.ItemId && x.Signal == 1).Select(x => new { Quantity = x.Quantity * x.ConversionFactor }).Sum(x => x.Quantity);



                totalPrice += itemPrice * Balance;

            }
            return totalPrice;
        }

        public async Task<TotalTransactionOfItemsResponse> getTotalTransactionOfItems(TotalTransactionOfItemsRequestDTO parm, bool isPrint = false)
        {
            var invoices = _InvoiceDetailsQuery.TableNoTracking
                                                .Include(x => x.Items)
                                                .Include(x => x.Units)
                                                .Include(x => x.InvoicesMaster)
                                                .Where(x => x.InvoicesMaster.StoreId == parm.storeId)
                                                .Where(x => parm.itemId != null ? x.ItemId == parm.itemId : true)
                                                .ToList();

            var units = _invStpUnitsQuery.TableNoTracking;
            var previousInvoices = invoices.Where(x => x.InvoicesMaster.InvoiceDate.Date < parm.dateFrom.Date)
                                           .GroupBy(x => x.ItemId)
                                           .Select(x => new TotalTransactionOfItemsList
                                           {
                                               itemId = x.First().ItemId,
                                               itemCode = x.First().Items.ItemCode,
                                               arabicName = x.First().Items.ArabicName,
                                               latinName = x.First().Items.LatinName,
                                               unitNameAr = units.Where(c => c.Id == x.First().Items.ReportUnit).First().ArabicName,
                                               unitNameEn = units.Where(c => c.Id == x.First().Items.ReportUnit).First().LatinName,
                                               previous = ConvertQyt(x.First().ItemId, x.First().Items.ReportUnit ?? 0, x.Sum(c => c.Quantity * c.Signal)),
                                               incoming = 0,
                                               outgoing = 0,
                                               balance = 0
                                           });
            var invoicesOfPeriod = invoices.Where(x => x.InvoicesMaster.InvoiceDate.Date >= parm.dateFrom && x.InvoicesMaster.InvoiceDate.Date <= parm.dateTo)
                                           .GroupBy(x => x.ItemId)
                                           .Select(x => new TotalTransactionOfItemsList
                                           {
                                               itemId = x.First().ItemId,
                                               itemCode = x.First().Items.ItemCode,
                                               arabicName = x.First().Items.ArabicName,
                                               latinName = x.First().Items.LatinName,
                                               unitNameAr = units.Where(c => c.Id == x.First().Items.ReportUnit).First().ArabicName,
                                               unitNameEn = units.Where(c => c.Id == x.First().Items.ReportUnit).First().LatinName,
                                               previous = 0,
                                               incoming = ConvertQyt(x.First().ItemId, x.First().Items.ReportUnit ?? 0, x.Where(c => c.Signal == 1).Sum(c => c.Quantity)),
                                               outgoing = ConvertQyt(x.First().ItemId, x.First().Items.ReportUnit ?? 0, x.Where(c => c.Signal == -1).Sum(c => c.Quantity)),
                                               balance = ConvertQyt(x.First().ItemId, x.First().Items.ReportUnit ?? 0, x.Sum(c => c.Quantity * c.Signal)) + ConvertQyt(x.First().ItemId, x.First().Items.ReportUnit ?? 0, previousInvoices.Where(c => c.itemId == x.First().ItemId).FirstOrDefault()?.previous ?? 0)
                                           });
            var allData = invoicesOfPeriod.Union(previousInvoices)
                                          .GroupBy(x => x.itemId)
                                          .Select(x => new TotalTransactionOfItemsList
                                          {
                                              itemId = x.First().itemId,
                                              itemCode = x.First().itemCode,
                                              arabicName = x.First().arabicName,
                                              latinName = x.First().latinName,
                                              unitNameAr = x.First().unitNameAr,
                                              unitNameEn = x.First().unitNameEn,
                                              previous = x.Last().previous,
                                              incoming = x.First().incoming,
                                              outgoing = x.First().outgoing,
                                              balance = x.First().balance
                                          })
                                          .ToList();



            var data = !isPrint ? Pagenation<TotalTransactionOfItemsList>.pagenationList(parm.pageSize, parm.pageNumber, allData) : invoicesOfPeriod;
            double MaxPageNumber = invoicesOfPeriod.ToList().Count() / Convert.ToDouble(parm.pageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            var response = new TotalTransactionOfItemsResponseDTO()
            {
                TotalPrevious = previousInvoices.Sum(c => c.previous),
                TotalIncoming = invoicesOfPeriod.Sum(c => c.incoming),
                TotalOutgoing = invoicesOfPeriod.Sum(c => c.outgoing),
                TotalBalance = invoicesOfPeriod.Sum(c => c.balance),
                data = data.ToList()
            };
            return new TotalTransactionOfItemsResponse()
            {
                data = response,
                notes = (countofFilter == parm.pageNumber ? Actions.EndOfData : ""),
                Result = Result.Success,
                totalCount = invoicesOfPeriod.Count(),
                dataCount = data.Count()
            };
        }

        public async Task<ResponseResult> TotalTransactionOfItems(TotalTransactionOfItemsRequestDTO parm)
        {
            var res = await getTotalTransactionOfItems(parm);

            return new ResponseResult()
            {
                Data = res.data,
                Note = res.notes,
                DataCount = res.dataCount,
                Result = res.Result,
                TotalCount = res.totalCount
            };
        }
        //public async Task<WebReport> TotalTransactionsOfItemsReports(TotalTransactionOfItemsRequestDTO parm, exportType exportType,bool isArabic)
        //{
        //    var data = await getTotalTransactionOfItems(parm, true);


        //    var userInfo = _iUserInformation.GetUserInformation();



        //    var AdditionalReportdata = new ReportOtherData()
        //    {
        //        //Code = request.serial,
        //        EmployeeName = userInfo.employeeNameAr.ToString(),
        //        EmployeeNameEn = userInfo.employeeNameEn.ToString(),
        //        Date = DateTime.Now.ToString("yyyy/MM/dd"),
        //    };




        //    var tablesNames = new TablesNames()
        //    {

        //        FirstListName = "DetailsOfSerialTransactions"

        //    };




        //    var report = await _iGeneralPrint.PrintReport<TotalTransactionOfItemsResponseDTO, TotalTransactionOfItemsList, object>(data.data, data.data.data, null, tablesNames, AdditionalReportdata
        //     , (int)SubFormsIds.DetailsOfSerialTransactions, exportType, isArabic);
        //    return report;
        //}
        public async Task<DemandLimitResponse> getDemandLimit(DemandLimitRequestDTO parm, bool isPrint = false)
            {
            var units = _invStpUnitsQuery.TableNoTracking;
            var itemUnits = _InvStpItemCardUnitQuery.TableNoTracking;
            var items = _itemCardMasterRepository.TableNoTracking;
            if(itemUnits.Count() == 0)
                return new DemandLimitResponse()
                {
                    Result = Result.NoDataFound
                };
            var _invoices = _InvoiceDetailsQuery.TableNoTracking
                                               .Include(x => x.InvoicesMaster)
                                               .Include(x => x.Items)
                                               .Include(x => x.Items.Stores)
                                               .Include(x => x.Items.Units)
                                               //.Include(x=> x.Items.Units.Where(c=> c.UnitId == x.Items.ReportUnit).First().Unit)
                                               .Where(x => x.InvoicesMaster.StoreId == parm.storeId)
                                               .Where(x => x.Items.Stores.Where(c => c.StoreId == parm.storeId).First().DemandLimit != 0)
                                               .Where(x=> itemUnits.Select(c=> c.ItemId).Contains(x.ItemId))
                                               .ToList()
                                               .GroupBy(x => x.ItemId)
                                               .ToList();
            var invoices = new List<DemandLimitResponseDTO>();
            foreach (var x in _invoices)
            {

                var store = x.First().Items.Stores.Where(c => c.StoreId == parm.storeId);
                if (store.Count() == 0)
                    continue;
                var itemReportUnit =  items.Where(c => c.Id == x.First().ItemId).FirstOrDefault(c => c.Id == x.First().ItemId).ReportUnit ?? 0;
                var DemandLimitNum = Convert.ToDouble(store.First().DemandLimit) / itemUnits.Where(d => d.ItemId == x.First().ItemId && d.UnitId == itemReportUnit).First().ConversionFactor;

                var balance = ReportData<InvoiceDetails>.Quantity(x, itemUnits.Where(d => d.ItemId == x.First().ItemId && d.UnitId == itemReportUnit).First().ConversionFactor);
                if (DemandLimitNum < balance)
                    continue;
                var RequiredLimitBalance = DemandLimitNum - balance;
                invoices.Add(new DemandLimitResponseDTO()
                {
                    itemId = x.First().ItemId,
                    itemCode =  x.First().Items.ItemCode,
                    arabicName =  x.First().Items.ArabicName,
                    latinName = x.First().Items.LatinName,
                    DemandLimitNum = DemandLimitNum,
                    balance = balance,
                    RequiredLimitBalance = RequiredLimitBalance,
                    unitNameAr = units.Where(c => c.Id == x.First().Items.ReportUnit).First()?.ArabicName ?? "",
                    unitNameEn = units.Where(c => c.Id == x.First().Items.ReportUnit).First()?.LatinName??""

                });
            }


            var data = !isPrint ? Pagenation<DemandLimitResponseDTO>.pagenationList(parm.pageSize, parm.pageNumber, invoices) : invoices;
            double MaxPageNumber = invoices.ToList().Count() / Convert.ToDouble(parm.pageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            return new DemandLimitResponse()
            {
                data = invoices,
                notes = (countofFilter == parm.pageNumber ? Actions.EndOfData : ""),
                Result = Result.Success,
                totalCount = invoices.Count(),
                dataCount = data.Count()
            };
        }

        public async Task<ResponseResult> DemandLimit(DemandLimitRequestDTO parm)
        {
            var res = await getDemandLimit(parm);
            return new ResponseResult()
            {
                Data = res.data,
                Note = res.notes,
                DataCount = res.dataCount,
                Result = res.Result,
                TotalCount = res.totalCount
            };
        }
        public async Task<WebReport> DemandLimitReport(DemandLimitRequestDTO parm, exportType exportType, bool isArabic, int fileId = 0)
        {
            var data = await getDemandLimit(parm, true);


            var userInfo = await _iUserInformation.GetUserInformation();

            var stores = _storeQuery.TableNoTracking
                        .Where(x => x.Id == parm.storeId).Select(x => new
                        {
                            x.Id,
                            x.ArabicName,
                            x.LatinName
                        }).FirstOrDefault();

            var AdditionalReportdata = new ReportOtherData()
            {
                //Code = request.serial,
                LatinName=stores.LatinName,
                ArabicName=stores.ArabicName,
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),
                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
            };




            var tablesNames = new TablesNames()
            {

                FirstListName = "DemandLimit"

            };




            var report = await _iGeneralPrint.PrintReport<object, DemandLimitResponseDTO, object>(null, data.data, null, tablesNames, AdditionalReportdata
             , (int)SubFormsIds.DemandLimit, exportType, isArabic, fileId);
            return report;
        }

        public async Task<List<ExpiredItemsReportResponseDTO>> GetDetailsOfExpiredItems(ExpiredItemsReportRequestDTO parm, bool isPrint = false)
        {
            var units = _invStpUnitsQuery.TableNoTracking;


            var itemDetails = _InvoiceDetailsQuery.TableNoTracking
                                                .Include(x => x.Items)
                                                .ThenInclude(x => x.Units)
                                                .Include(x => x.InvoicesMaster)
                                                .Where(x=> !x.InvoicesMaster.IsDeleted)
                                                .Where(a => a.ItemTypeId == (int)ItemTypes.Expiary)
                                                .Where(a=> a.InvoicesMaster.StoreId == parm.storeId)
                                                .Where(a=> a.ExpireDate.Value.Date > DateTime.Now.Date && a.ExpireDate.Value.Date <= DateTime.Now.AddDays(parm.NumberOfDays).Date)
                                                .ToList()
                                                .GroupBy(x=> new { x.ExpireDate,x.ItemId })
                                                .Select(x => new ExpiredItemsReportResponseDTO()
                                                {
                                                    ItemCode = x.First().Items.ItemCode,
                                                    ItemNameAr = x.First().Items.ArabicName,
                                                    ItemNameEn = x.First().Items.LatinName,
                                                    UnitNameAr = units.Where(c => c.Id == x.First().Items.ReportUnit).First().ArabicName,
                                                    UnitNameEn = units.Where(c => c.Id == x.First().Items.ReportUnit).First().LatinName,
                                                    Quantity =ReportData<InvoiceDetails>.Quantity(x,x.First().Items.Units.Where(c=> c.UnitId == x.First().Items.ReportUnit).First().ConversionFactor),
                                                    ExpireDate = x.First().ExpireDate.Value,
                                                    ExpireDateForPrint=x.First().ExpireDate.Value.ToString("yyyy/MM/dd")
                                                }).ToList();


            var data = !isPrint ? Pagenation<ExpiredItemsReportResponseDTO>.pagenationList(parm.pageSize, parm.pageNumber, itemDetails) : itemDetails;

            return data;
        }

        public async Task<ResponseResult> DetailsOfExpiredItems(ExpiredItemsReportRequestDTO parm)
        {
            var res = await GetDetailsOfExpiredItems(parm);

            if (res.Count == 0)
                return new ResponseResult() { Result = Result.NotFound };

            return new ResponseResult() { Data = res, DataCount = res.Count, Result = Result.Success };
        }
        public async Task<WebReport> DetailsOfExpiredItemsReport(ExpiredItemsReportRequestDTO parm,exportType exportType,bool isArabic, int fileId = 0)
        {
            var data = await GetDetailsOfExpiredItems(parm, true);
            
            
            var userInfo = await _iUserInformation.GetUserInformation();

            var store = _storeQuery.TableNoTracking
                       .Where(x => x.Id == parm.storeId).Select(x => new
                       {
                           x.Id,
                           x.ArabicName,
                           x.LatinName
                       }).FirstOrDefault();
            


            var AdditionalReportdata = new ReportOtherData()
            {
                //Code = request.serial,
                Id=parm.NumberOfDays,
                ArabicName = store.ArabicName,
                LatinName = store.LatinName,
                DateTo=DateTime.Now.AddDays(parm.NumberOfDays).ToString("yyyy/MM/dd HH:mm:ss"),
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),
                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
            };




            var tablesNames = new TablesNames()
            {

                FirstListName = "ExpiredItems"

            };




            var report = await _iGeneralPrint.PrintReport<object, ExpiredItemsReportResponseDTO, object>(null, data, null, tablesNames, AdditionalReportdata
             , (int)SubFormsIds.GetDetailsOfExpiredItems, exportType, isArabic ,fileId);
            return report;
        }
        public async Task<ReviewWarehouseTransfersResponse> GetReviewWarehouseTransfers(ReviewWarehouseTransfersRequest parm, bool isPrint)
        {
            var listOfTransferTypes = Lists.transferStore;
            var AllInvoices = _invoiceMasterQuery.TableNoTracking
                                              .Where(x => !x.IsDeleted)
                                              .Where(x => listOfTransferTypes.Contains(x.InvoiceTypeId));
            var invoices = AllInvoices.Where(x => x.StoreId == parm.storeFrom && (x.StoreIdTo == parm.storeTo || x.StoreIdTo == null))
                                      .Where(x => parm.status != TransferStatusEnum.all ? x.transferStatus == (int)parm.status : true)
                                      //.Where(x => x.InvoiceDate.Date >= parm.from && x.InvoiceDate.Date <= parm.to)
                                      .ToList();
            var incomingTransfers = AllInvoices.Where(x => x.InvoiceTypeId == 34);
            var ReportData = invoices
                                    .Where(x => x.InvoiceTypeId == (int)Enums.DocumentType.OutgoingTransfer)
                                    .OrderByDescending(x => x.InvoiceId)
                                    .Select(x => new ReviewWarehouseTransfers
                                    {
                                        InvoiceId=x.InvoiceId,
                                        InvoiceTypeId=x.InvoiceTypeId,
                                        pageId = DocHelpers.GetPageIdOfDoc(x.InvoiceTypeId),
                                        pageEnum = DocHelpers.GetPageEnum(x.InvoiceTypeId),
                                        DocTypeId = DocHelpers.GetDocTypeId(x.InvoiceTypeId),

                                        documentId = x.InvoiceTransferType,
                                        date = x.InvoiceDate.ToString(defultData.datetimeFormat),
                                        // date = x.InvoiceDate.ToString("yyy/mm/dd"),

                                        status = x.transferStatus,
                                        statusTypeAr = getTransferType(x.transferStatus).transferTypeAr,
                                        statusTypeEn = getTransferType(x.transferStatus).transferTypeEn,
                                        receivedDate = incomingTransfers.Where(c => c.ParentInvoiceCode == x.InvoiceTransferType).FirstOrDefault()?.InvoiceDate.ToString(defultData.datetimeFormat),
                                        //receivedDate = incomingTransfers.Where(c => c.ParentInvoiceCode == x.InvoiceType).FirstOrDefault()?.InvoiceDate.ToString("yyyy/mm/dd") ?? "ــــــــ",

                                    }).ToList();

            var data = !isPrint ? Pagenation<ReviewWarehouseTransfers>.pagenationList(parm.PageSize ?? 0, parm.PageNumber ?? 0, ReportData) : ReportData;
            double MaxPageNumber = ReportData.ToList().Count() / Convert.ToDouble(parm.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            var response = new ReviewWarehouseTransfersResponse()
            {
                data = data,
                dataCount = data.Count(),
                totalCount = ReportData.Count(),
                Note = (countofFilter == parm.PageNumber ? Actions.EndOfData : ""),
                Result = data.Any() ? Result.Success : Result.NoDataFound
            };
            return response;

        }
        private transferTypeModel getTransferType(int status)
        {
            string transferTypeAr = string.Empty;
            string transferTypeEn = string.Empty;
            if (status == (int)App.Domain.Enums.TransferStatusEnum.Accepted)
            {
                transferTypeAr = "مسلتم كليا";
                transferTypeEn = "Accepted";
            }
            else if (status == (int)App.Domain.Enums.TransferStatusEnum.PartialAccepted)
            {
                transferTypeAr = "مستلم جزئيا";
                transferTypeEn = "Partial Accepted";
            }
            else if (status == (int)App.Domain.Enums.TransferStatusEnum.Rejected)
            {
                transferTypeAr = "تم رفضة";
                transferTypeEn = "Rejected";
            }
            else if (status == (int)App.Domain.Enums.TransferStatusEnum.Binded)
            {
                transferTypeAr = "معلق";
                transferTypeEn = "Binded";
            }
            else if (status == 0)
            {
                transferTypeAr = "الكل";
                transferTypeEn = "All";
            }
            return new transferTypeModel()
            {
                transferTypeAr = transferTypeAr,
                transferTypeEn = transferTypeEn
            };
        }
        public async Task<ResponseResult> ReviewWarehouseTransfers(ReviewWarehouseTransfersRequest parm)
        {
            var response = await GetReviewWarehouseTransfers(parm, false);
            return new ResponseResult()
            {
                Data = response.data,
                DataCount = response.dataCount,
                TotalCount = response.totalCount,
                Note = response.Note
            };
        }
        public async Task<WebReport> ReviewWarehouseTransfersReport(ReviewWarehouseTransfersRequest parm, exportType exportType, bool isArabic, int fileId = 0)
        {
            var data = await GetReviewWarehouseTransfers(parm, true);
            DateTime date;
           foreach(var item in data.data)
            {
                date = Convert.ToDateTime(item.date);
                item.date = date.ToString("yyyy/MM/dd");
                date = Convert.ToDateTime(item.receivedDate);
                item.receivedDate = date.ToString("yyyy/MM/dd");
            }
            var userInfo = await _iUserInformation.GetUserInformation();

            var fromStore = _storeQuery.TableNoTracking
                       .Where(x => x.Id == parm.storeFrom).Select(x => new
                       {
                           x.Id,
                           x.ArabicName,
                           x.LatinName
                       }).FirstOrDefault();
            var toStore = _storeQuery.TableNoTracking
                      .Where(x => x.Id == parm.storeTo).Select(x => new
                      {
                          x.Id,
                          x.ArabicName,
                          x.LatinName
                      }).FirstOrDefault();
            var type = getTransferType((int)parm.status);


            var AdditionalReportdata = new AdditionalReportDataStore()
            {
                //Code = request.serial,
                ArabicName = fromStore.ArabicName,
                LatinName = fromStore.LatinName,
                ToStoreNameAr = toStore.ArabicName,
                ToStoreNameEn = toStore.LatinName,
                transferTypeAr = type.transferTypeAr,
                transferTypeEn = type.transferTypeEn,
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),
                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
            };




            var tablesNames = new TablesNames()
            {

                FirstListName = "ReviewWarehouseTransfers"

            };




            var report = await _iGeneralPrint.PrintReport<object, ReviewWarehouseTransfers, object>(null, data.data, null, tablesNames, AdditionalReportdata
             , (int)SubFormsIds.ReviewWarehouseTransfersReport, exportType, isArabic, fileId);
            return report;
        }
        private int DetailedTransferReportStatues(double acceptedQyt, double transferedQyt, int transferStatus)
        {
            if (transferStatus == (int)TransferStatusEnum.Binded)
                return (int)TransferStatusEnum.Binded;
            int status = 0;
            var calc = acceptedQyt / transferedQyt;
            if (calc == 0)
                return (int)TransferStatusEnum.Rejected;
            else if (calc == 1)
                return (int)TransferStatusEnum.Accepted;
            else
                return (int)TransferStatusEnum.PartialAccepted;
            return status;
        }
        private string GetSerials(string invoiceType,int itemId,List<InvSerialTransaction> serialTransaction,bool isIncoming)
        {
            var serials = serialTransaction.Where(c => (isIncoming ? c.AddedInvoice == invoiceType : c.ExtractInvoice == invoiceType) && c.ItemId == itemId).Select(c => c.SerialNumber).ToArray();
            return string.Join(",", serials);
        }
        
        public async Task<DetailedTransferReportResponse> GetDetailedTransferReport(DetailedTransferReportRequest parm, bool isPrint = false)
        {
            var listOfTransferTypes = Lists.transferStore;
            var units = _invStpUnitsQuery.TableNoTracking;
            var invoices = _InvoiceDetailsQuery.TableNoTracking
                                               .Include(x => x.InvoicesMaster)
                                               .Include(x => x.Items)
                                               .Include(x => x.Items.Units)
                                               //.ThenInclude(x=> x.First().Unit)
                                               .Where(x => !x.InvoicesMaster.IsDeleted)
                                               .Where(x => listOfTransferTypes.Contains(x.InvoicesMaster.InvoiceTypeId))
                                               .Where(x => x.InvoicesMaster.InvoiceDate.Date >= parm.dateFrom.Date && x.InvoicesMaster.InvoiceDate.Date <= parm.dateTo.Date)
                                               .ToList();
            var serialTransaction = _InvSerialTransactionQuery.TableNoTracking.ToList();

            var transfers = invoices.Where(x => x.InvoicesMaster.InvoiceTypeId == (int)Enums.DocumentType.OutgoingTransfer)
                                    .Where(x => x.InvoicesMaster.StoreId == parm.transferFrom && x.InvoicesMaster.StoreIdTo == parm.transferTo)
                                    .OrderByDescending(x => x.InvoiceId)
                                    .Select(x => new DetailedTransferReport
                                    {
                                        InvoiceId=x.InvoiceId,
                                        InvoiceTypeId=x.InvoicesMaster.InvoiceTypeId,
                                        pageId= DocHelpers.GetPageIdOfDoc(x.InvoicesMaster.InvoiceTypeId),
                                        pageEnum = DocHelpers.GetPageEnum(x.InvoicesMaster.InvoiceTypeId),
                                        DocTypeId= DocHelpers.GetDocTypeId(x.InvoicesMaster.InvoiceTypeId),
                                        date = x.InvoicesMaster.InvoiceDate.ToString(defultData.datetimeFormat),
                                        arabicName = x.Items.ArabicName,
                                        latinName = x.Items.LatinName,
                                        itemCode = x.Items.ItemCode,
                                        price = x.Price,
                                        transferCode= x.InvoicesMaster.InvoiceTransferType,
                                        status = DetailedTransferReportStatues(x.TransQuantity,x.Quantity,x.InvoicesMaster.transferStatus),
                                        statusAr = getTransferType(DetailedTransferReportStatues(x.TransQuantity,x.Quantity,x.InvoicesMaster.transferStatus)).transferTypeAr,
                                        statusEn = getTransferType(DetailedTransferReportStatues(x.TransQuantity, x.Quantity, x.InvoicesMaster.transferStatus)).transferTypeEn,
                                        transferedQyt = x.Quantity,
                                        unitAr = units.Where(c => c.Id == x.UnitId).FirstOrDefault().ArabicName,
                                        unitEn = units.Where(c => c.Id == x.UnitId).FirstOrDefault().LatinName,
                                        acceptedQyt = x.TransQuantity,
                                        totalPrice = roundNumbers.GetRoundNumber(x.TransQuantity * x.Price),
                                        outgoingSerials = GetSerials(x.InvoicesMaster.InvoiceType, x.ItemId, serialTransaction,false),
                                        incomingSerials = GetSerials(invoices.Where(c=> c.InvoicesMaster.ParentInvoiceCode == x.InvoicesMaster.InvoiceType).Select(c=> c.InvoicesMaster.InvoiceType).FirstOrDefault(), x.ItemId, serialTransaction, true),
                                    })
                                    .Where(x => parm.status != TransferStatusEnum.all ? x.status == (int)parm.status : true)
                                    .ToList();
            var data = !isPrint ? Pagenation<DetailedTransferReport>.pagenationList(parm.PageSize ?? 0, parm.PageNumber ?? 0, transfers) : transfers;
            double MaxPageNumber = transfers.Count() / Convert.ToDouble(parm.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            var response = new DetailedTransferReportResponse()
            {
                data = data,
                dataCount = data.Count(),
                totalCount = transfers.Count(),
                Result = data.Any() ? Result.Success : Result.Failed,
                Note = (countofFilter == parm.PageNumber ? Actions.EndOfData : "")
            };
            return response;
        }

        public async Task<ResponseResult> DetailedTransferReport(DetailedTransferReportRequest parm)
        {
            var response = await GetDetailedTransferReport(parm);
            return new ResponseResult()
            {
                Data = response.data,
                DataCount = response.dataCount,
                TotalCount = response.totalCount,
                Result = response.Result,
                Note = response.Note,
            };
        }
        public async Task<WebReport> DetailedTransferPrint(DetailedTransferReportRequest parm, exportType exportType, bool isArabic, int fileId = 0)
        {
            var data = await GetDetailedTransferReport(parm, true);
            DateTime date;
            foreach(var item in data.data)
            {
                date = Convert.ToDateTime(item.date);
                item.date = date.ToString("yyyy/MM/dd");
            }

            var userInfo = await _iUserInformation.GetUserInformation();

            var fromStore = _storeQuery.TableNoTracking
                       .Where(x => x.Id == parm.transferFrom).Select(x => new
                       {
                           x.Id,
                           x.ArabicName,
                           x.LatinName
                       }).FirstOrDefault();
            var toStore = _storeQuery.TableNoTracking
                      .Where(x => x.Id == parm.transferTo).Select(x => new
                      {
                          x.Id,
                          x.ArabicName,
                          x.LatinName
                      }).FirstOrDefault();
            var type = getTransferType((int)parm.status);

            var dates= ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, parm.dateFrom, parm.dateTo);
            var AdditionalReportdata = new AdditionalReportDataStore()
            {
                //Code = request.serial,
                ArabicName = fromStore.ArabicName,
                LatinName = fromStore.LatinName,
                ToStoreNameAr = toStore.ArabicName,
                ToStoreNameEn = toStore.LatinName,
                transferTypeAr = type.transferTypeAr,
                transferTypeEn = type.transferTypeEn,
                DateFrom= dates.DateFrom,
                DateTo = dates.DateTo,
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),
                Date = dates.Date,
            };




            var tablesNames = new TablesNames()
            {

                FirstListName = "DetailedTransfer"

            };




            var report = await _iGeneralPrint.PrintReport<object, DetailedTransferReport, object>(null, data.data, null, tablesNames, AdditionalReportdata
             , (int)SubFormsIds.DetailedTransferReport, exportType, isArabic, fileId);
            return report;
        }


        public class AdditionalReportDataStore : ReportOtherData
        {
            //public string ItemCode { get; set; }
            public string ItemNameAr { get; set; }
            public string ItemNameEn { get; set; }

            public string UnitNameAr { get; set; }
            public string UnitNameEn { get; set; }
            //   public string 
            //public string StoreName { get; set; }
            //for transfer
            public string ToStoreNameAr { get; set; }
            public string ToStoreNameEn { get; set; }
            public string transferTypeAr { get; set; }
            public string transferTypeEn { get; set; }

        }
        public class TransferDesc
        {
            public string descAr { get; set; }
            public string descEn { get; set; }
        }
        public class transferTypeModel
        {
            public string transferTypeAr { get; set; }
            public string transferTypeEn { get; set; }
        }
    }
}
