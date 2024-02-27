using App.Application.Handlers.Invoices.OfferPrice.GetOfferPriceById;
using App.Application.Handlers.Reports;
using App.Application.Handlers.Reports.SalesReports.ItemsProfitServices;
using App.Application.Handlers.Reports.SalesReports.SalesOfSalesMan;
using App.Application.Helpers.CashingService.GetFromCache;
using App.Application.Helpers.CashingService.SetInCash;
using App.Application.Helpers.ReportsHelper;
using App.Application.Helpers.Service_helper.Item_unit;
using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Application.Services.Printing.InvoicePrint;
using App.Application.Services.Printing.PrintFile;
using App.Application.Services.Reports.StoreReports.Invoices.Purchases;
using App.Domain.Entities.Setup;
using App.Domain.Models.Request.Store.Reports;
using App.Domain.Models.Request.Store.Reports.Sales;
using App.Domain.Models.Request.Store.Reports.Store;
using App.Domain.Models.Request.Store.Sales;
using App.Domain.Models.Response.Store;
using App.Domain.Models.Response.Store.Reports;
using App.Domain.Models.Response.Store.Reports.Sales;
using App.Infrastructure.settings;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Immutable;

namespace App.Application.Services.Reports.StoreReports.Sales
{
    public class RPT_Sales : iRPT_Sales
    {
        private readonly IRepositoryQuery<InvoiceMaster> _invoiceMasterQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> _invStpItemCardMasterQuery;
        private readonly IRepositoryQuery<InvoiceDetails> _invoiceDetailsQuery;
        private readonly IRepositoryQuery<InvStpUnits> _invStpUnitsQuery;
        private readonly IRepositoryQuery<GlReciepts> _glRecieptsQuery;
        private readonly IRoundNumbers _roundNumbers;
        private readonly IitemUnitHelperServices itemUnitHelperServices;
        private readonly IRepositoryQuery<InvoicePaymentsMethods> _invoicePaymentsMethodsQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> _invGeneralSettingsQuery;
        private readonly IRepositoryQuery<GLBranch> _GLBranchQuery;
        //private readonly IPrintService _iprintService;
        private readonly IMemoryCache _memoryCache;
        private readonly ISecurityIntegrationService _securityIntegrationService;


        //private readonly IprintFileService _iPrintFileService;
        //private readonly IFilesMangerService _filesMangerService;
        //private readonly ICompanyDataService _CompanyDataService;
        private readonly iUserInformation _iUserInformation;
        private readonly IGeneralPrint _iGeneralPrint;
        private readonly IRepositoryQuery<InvEmployees> _employeeQuery;
        private readonly IRepositoryQuery<InvPersons> _personQuery;
        private readonly IRepositoryQuery<InvSalesMan> salesManQuery;
        private readonly IMediator _mediator;
        private readonly IVatStatementService _vatStatementService;
        private readonly IMapper _mapper;
        public RPT_Sales(IRepositoryQuery<InvoiceMaster> invoiceMasterQuery,
                        IRepositoryQuery<InvStpItemCardMaster> InvStpItemCardMasterQuery,
                        IRepositoryQuery<InvoiceDetails> InvoiceDetailsQuery,
                        IRepositoryQuery<InvStpUnits> InvStpUnitsQuery,
                        IRepositoryQuery<GlReciepts> GlRecieptsQuery,
                        IRoundNumbers roundNumbers,
                        IitemUnitHelperServices itemUnitHelperServices,
                        IRepositoryQuery<InvoicePaymentsMethods> InvoicePaymentsMethodsQuery,
                        IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsQuery, IPrintService iprintService,
                        IprintFileService iPrintFileService, IFilesMangerService filesMangerService,
                        ICompanyDataService CompanyDataService,
                        iUserInformation iUserInformation,
                        IGeneralPrint iGeneralPrint,
                        IRepositoryQuery<InvEmployees> employeeQuery,
                        IMediator mediator,
                        IRepositoryQuery<InvPersons> personQuery,
                        IRepositoryQuery<InvSalesMan> salesManQuery,
                        IRepositoryQuery<GLBranch> gLBranchQuery,
                        IVatStatementService vatStatementService, IMapper mapper, IMemoryCache memoryCache, ISecurityIntegrationService securityIntegrationService)
        {
            _invoiceMasterQuery = invoiceMasterQuery;
            _invStpItemCardMasterQuery = InvStpItemCardMasterQuery;
            _invoiceDetailsQuery = InvoiceDetailsQuery;
            _invStpUnitsQuery = InvStpUnitsQuery;
            _glRecieptsQuery = GlRecieptsQuery;
            _roundNumbers = roundNumbers;
            this.itemUnitHelperServices = itemUnitHelperServices;
            _invoicePaymentsMethodsQuery = InvoicePaymentsMethodsQuery;
            _invGeneralSettingsQuery = InvGeneralSettingsQuery;
            //_iprintService = iprintService;
            //_iPrintFileService = iPrintFileService;
            //_filesMangerService = filesMangerService;
            //_CompanyDataService = CompanyDataService;
            _iUserInformation = iUserInformation;
            _iGeneralPrint = iGeneralPrint;
            _employeeQuery = employeeQuery;
            _mediator = mediator;
            _personQuery = personQuery;
            this.salesManQuery = salesManQuery;
            _GLBranchQuery = gLBranchQuery;
            _vatStatementService = vatStatementService;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _securityIntegrationService = securityIntegrationService;
        }

        public async Task<itemsSalesReponse> GetItemsSales(ItemSalesRequestDto parm, bool isPrint = false)
        {
            var branches = parm.branches.Split(',').Select(x => int.Parse(x)).ToArray();
            var units = _invStpUnitsQuery.TableNoTracking.ToList();
            var items = _invStpItemCardMasterQuery.TableNoTracking.Include(x => x.Units).ToList();
            //var settings = _invGeneralSettingsQuery.TableNoTracking.FirstOrDefault();
            var listOfSalesInvoices = Lists.SalesWithOutDeleteInvoicesList;
            var _invoices = _invoiceDetailsQuery.TableNoTracking
                                                             .Include(x => x.InvoicesMaster)
                                                             .Include(x => x.InvoicesMaster.Person)
                                                             .Include(x => x.InvoicesMaster.Person.InvEmployees)
                                                             .Include(x => x.Items)
                                                             .Include(x => x.Items.Category)
                                                             .Include(x => x.Items.Units);
            var userInfo = await _iUserInformation.GetUserInformation();
            var invoices = await itemsTransaction.ItemsData(userInfo, _roundNumbers, units, _invoices, branches, false, parm.itemId, listOfSalesInvoices.ToArray(), parm.dateFrom, parm.dateTo, parm.catId, 0, 0, parm.employeeId, parm.paymentType, -1, false);


            var data = !isPrint ? Pagenation<itemsSalesData>.pagenationList(parm.PageSize, parm.PageNumber, invoices) : invoices;
            double MaxPageNumber = invoices.ToList().Count() / Convert.ToDouble(parm.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);

            var rs = new itemsSalesReponseDTO()
            {
                data = data,
                TotalQyt = _roundNumbers.GetRoundNumber(invoices.Sum(c => c.qyt)),
                TotalAavgOfPrice = _roundNumbers.GetRoundNumber(invoices.Sum(c => c.avgOfPrice)),
                TotalPrice = _roundNumbers.GetRoundNumber(invoices.Sum(c => c.totalPrice)),
                TotalDiscount = _roundNumbers.GetRoundNumber(invoices.Sum(c => c.discount)),
                TotalNet = _roundNumbers.GetRoundNumber(invoices.Sum(c => c.net)),
            };
            return new itemsSalesReponse()
            {
                data = rs,
                notes = (countofFilter == parm.PageNumber ? Actions.EndOfData : ""),
                Result = Result.Success,
                TotalCount = invoices.Count(),
                dataCount = data.Count()
            };
        }
        public async Task<ResponseResult> itemsSales(ItemSalesRequestDto parm, bool isPrint = false)
        {
            var res = await GetItemsSales(parm, isPrint);
            return new ResponseResult()
            {
                Data = res.data,
                Note = res.notes,
                Result = res.data.data.Any() ? Result.Success : Result.NotFound,
                DataCount = res.dataCount,
                TotalCount = res.TotalCount
            };
        }
        public async Task<WebReport> itemsSalesReport(ItemSalesRequestDto param, exportType exportType, bool isArabic, int fileId = 0)
        {
            if (param.catId == null)
            {
                param.catId = 0;
            }
            if (param.itemId == null)
            {
                param.itemId = 0;
            }
            var data = await GetItemsSales(param, true);

            //var userInfo = await _iUserInformation.GetUserInformation();
            var otherdata = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, param.dateFrom, param.dateTo);
            if (param.employeeId != 0)
            {
                var userInfo = _iUserInformation.GetUserInformationById(param.employeeId);
                otherdata.EmployeeName = userInfo.employeeNameAr.ToString();
                otherdata.EmployeeNameEn = userInfo.employeeNameEn.ToString();

            }
            else
            {
                otherdata.EmployeeName = "الكل";
                otherdata.EmployeeNameEn = "All";
            }


            var tablesNames = new TablesNames()
            {

                ObjectName = "itemsSalesReponse",
                FirstListName = "itemsSalesData"
            };

            var report = await _iGeneralPrint.PrintReport<itemsSalesReponseDTO, itemsSalesData, object>(data.data, data.data.data, null, tablesNames, otherdata
             , (int)SubFormsIds.ItemSales, exportType, isArabic, fileId);
            return report;


        }
        public async Task<ResponseResult> SalesOfCasher(salesOfCasherDTO parm, bool isNotAccredit, bool isPrint = false)
        {
            var res = await GetSalesOfCasher(parm, isNotAccredit, isPrint);
            return new ResponseResult()
            {
                Data = res.data,
                Result = res.Result,
                Note = res.notes,
                TotalCount = res.TotalCount,
                DataCount = res.dataCount
            };
        }
        public async Task<salesOfCasherResponse> GetSalesOfCasher(salesOfCasherDTO parm, bool isNotAccredit, bool isPrint = false)
        {
            var transactionList = TransactionTypeList.transactionTypeModels();
            var branches = parm.branchIds.Split(',').ToArray().Select(x => int.Parse(x)).ToList();

            var userInfo = await _iUserInformation.GetUserInformation();
            var _invoices = _invoiceMasterQuery
                            .TableNoTracking
                            .Include(x => x.InvoicePaymentsMethods)
                            .Include(x => x.Person.InvEmployees)
                            .Where(x => !userInfo.otherSettings.posShowOtherPersonsInv ? x.EmployeeId == userInfo.employeeId : true)
                            .Where(x => !x.IsDeleted && x.PaymentType != (int)PaymentType.Delay)
                            .Where(x => x.InvoiceTypeId == (int)App.Domain.Enums.Enums.DocumentType.POS || x.InvoiceTypeId == (int)App.Domain.Enums.Enums.DocumentType.ReturnPOS)
                            .Where(x => branches.Contains(x.BranchId))
                            .Where(x => parm.employeeId != 0 ? x.EmployeeId == parm.employeeId : true)
                            .Where(x => x.InvoiceDate.Date >= parm.dateFrom.Date && x.InvoiceDate.Date <= parm.dateTo.Date)
                            .Where(x => isNotAccredit ? x.IsAccredite == false : true);
            if (!_invoices.Any())
                return new salesOfCasherResponse()
                {
                    Result = Result.NoDataFound,
                    notes = Actions.NotFound
                };
            var returnList = Lists.returnInvoiceList;
            var invoices = _invoices
                .ToList()
                .Select(x => new ListOfSalesOfCasherResponseDTO
                {
                    date = x.InvoiceDate.Date,
                    documentCode = x.InvoiceType,
                    documentTypeAR = x.InvoiceTypeId.ToString(), /*transactionList.Find(c=> c.id == x.InvoiceTypeId).arabicName*/
                    documentTypeEn = "",                         /*transactionList.Find(c => c.id == x.InvoiceTypeId).latinName*/

                    CashPaid = x.InvoiceTypeId != (int)Enums.DocumentType.ReturnPOS ? _roundNumbers.GetRoundNumber(((x.InvoicePaymentsMethods.Where(c => c.PaymentMethodId == 1).FirstOrDefault()?.Value ?? 0) + (x.Remain < 0 && x.InvoicePaymentsMethods.Where(c => c.PaymentMethodId == 1).Any() ? x.Remain : 0))) : _roundNumbers.GetRoundNumber(((x.InvoicePaymentsMethods.Where(c => c.PaymentMethodId == 1).FirstOrDefault()?.Value ?? 0) + (x.Remain < 0 && x.InvoicePaymentsMethods.Where(c => c.PaymentMethodId == 1).Any() ? x.Remain : 0)) * -1),
                    NetPaid = x.InvoiceTypeId != (int)Enums.DocumentType.ReturnPOS ? _roundNumbers.GetRoundNumber(((x.InvoicePaymentsMethods.Where(c => c.PaymentMethodId == 2).FirstOrDefault()?.Value ?? 0) + (x.Remain < 0 && x.InvoicePaymentsMethods.Where(c => c.PaymentMethodId == 2).Any() ? x.Remain : 0))) : _roundNumbers.GetRoundNumber(((x.InvoicePaymentsMethods.Where(c => c.PaymentMethodId == 2).FirstOrDefault()?.Value ?? 0) + (x.Remain < 0 && x.InvoicePaymentsMethods.Where(c => c.PaymentMethodId == 2).Any() ? x.Remain : 0)) * -1),
                    ChequesPaid = x.InvoiceTypeId != (int)Enums.DocumentType.ReturnPOS ? _roundNumbers.GetRoundNumber(((x.InvoicePaymentsMethods.Where(c => c.PaymentMethodId != 1 && c.PaymentMethodId != 2).FirstOrDefault()?.Value ?? 0) + (x.Remain < 0 && x.InvoicePaymentsMethods.Where(c => c.PaymentMethodId != 1 && c.PaymentMethodId != 2).Any() ? x.Remain : 0))) : _roundNumbers.GetRoundNumber((x.InvoicePaymentsMethods.Where(c => c.PaymentMethodId != 1 && c.PaymentMethodId != 2).FirstOrDefault()?.Value ?? 0 + (x.Remain < 0 && x.InvoicePaymentsMethods.Where(c => c.PaymentMethodId != 1 && c.PaymentMethodId != 2).Any() ? x.Remain : 0)) * -1),

                    Return = x.InvoiceTypeId == (int)Enums.DocumentType.ReturnPOS ? _roundNumbers.GetRoundNumber(x.VirualPaid) : 0,

                    Total = _roundNumbers.GetRoundNumber(x.Paid > x.Net ? x.Net : x.Paid) * (x.InvoiceTypeId == (int)Enums.DocumentType.ReturnPOS ? -1 : 1),
                    Serialize = x.Serialize,

                    rowClassName = returnList.Where(c => c == x.InvoiceTypeId)?.Any() == true ? "text-danger" : ""
                })
                .OrderBy(x => x.date)
                .ThenBy(x => x.Serialize)
                .ToList();
            invoices.ForEach(x =>
            {
                var transactionType = transactionList.Find(c => c.id.ToString() == x.documentTypeAR);
                x.documentTypeAR = transactionType.arabicName;
                x.documentTypeEn = transactionType.latinName;
            });
            var TotalCashPaid = _roundNumbers.GetRoundNumber(invoices.Sum(x => x.CashPaid));
            var TotalNetPaid = _roundNumbers.GetRoundNumber(invoices.Sum(x => x.NetPaid));
            var TotalChequesPaid = _roundNumbers.GetRoundNumber(invoices.Sum(x => x.ChequesPaid));
            var TotalReturn = _roundNumbers.GetRoundNumber(invoices.Sum(x => x.Return));
            var TotalTotal = _roundNumbers.GetRoundNumber(invoices.Sum(x => x.Total));

            var data = !isPrint ? Pagenation<ListOfSalesOfCasherResponseDTO>.pagenationList(parm.PageSize, parm.PageNumber, invoices.ToList()) : invoices.ToList();
            var res = new salesOfCasherResponseDTO()
            {
                TotalCashPaid = TotalCashPaid,
                TotalNetPaid = TotalNetPaid,
                TotalChequesPaid = TotalChequesPaid,
                TotalReturn = TotalReturn,
                TotalTotal = TotalTotal,
                Data = data
            };
            double MaxPageNumber = invoices.ToList().Count() / Convert.ToDouble(parm.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            return new salesOfCasherResponse()
            {
                data = res,
                notes = (countofFilter == parm.PageNumber ? Actions.EndOfData : ""),
                Result = Result.Success,
                dataCount = data.Count,
                TotalCount = invoices.Count
            };

        }
        public async Task<WebReport> GetSalesOfCasherReport(salesOfCasherDTO request, bool isNotAccredit, exportType exportType, bool isArabic, int fileId = 0)
        {

            var Casherdata = await GetSalesOfCasher(request, isNotAccredit, true);
            var userInfo = await _iUserInformation.GetUserInformation();
            int screenId = 0;
            var otherdata = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, request.dateFrom, request.dateTo);
            otherdata.EmployeeName = userInfo.employeeNameAr.ToString();
            otherdata.EmployeeNameEn = userInfo.employeeNameEn.ToString();





            if (request.employeeId != 0)
            {
                var casherInfo = _iUserInformation.GetUserInformationById(request.employeeId);

                otherdata.ArabicName = casherInfo.employeeNameAr.ToString();
                otherdata.LatinName = casherInfo.employeeNameEn.ToString();
            }
            else
            {

                otherdata.ArabicName = "الكل";
                otherdata.LatinName = "All";

            }
            if (!isNotAccredit)
            {

                screenId = (int)SubFormsIds.salesOfCasher;
            }
            else
            {
                screenId = (int)SubFormsIds.salesOfCasherNotAccredit;

            }

            var tablesNames = new TablesNames()
            {

                ObjectName = "salesOfCasherResponse",
                FirstListName = "salesOfCasherList"
            };

            var report = await _iGeneralPrint.PrintReport<salesOfCasherResponseDTO, ListOfSalesOfCasherResponseDTO, object>(Casherdata.data, Casherdata.data.Data, null, tablesNames, otherdata
             , screenId, exportType, isArabic, fileId);
            return report;



        }


        public async Task<salesTransactionResponse> GetsalesTransaction(salesTransactionRequestDTO parm, bool isPrint = false)
        {

            var branches = parm.branches.Split(',').Select(x => int.Parse(x)).ToArray();
            var invoicesSearch = new List<int>();


            if (parm.invoiceTypes == invoiceTypes.all)
            {
                if (parm.isSales)
                {
                    invoicesSearch.AddRange(Lists.OnlyReturnWithOutDeleteInvoicesList.ToArray());
                    invoicesSearch.AddRange(Lists.OnlySalesWithOutDeleteInvoicesList.ToArray());
                }
                else
                {
                    invoicesSearch.AddRange(new[] { (int)DocumentType.Purchase, (int)DocumentType.ReturnPurchase });
                }
            }
            else if (parm.invoiceTypes == invoiceTypes.sales)
            {
                invoicesSearch.AddRange(Lists.OnlySalesWithOutDeleteInvoicesList.ToArray());
            }
            else if (parm.invoiceTypes == invoiceTypes.salesReturns)
            {
                invoicesSearch.AddRange(Lists.OnlyReturnWithOutDeleteInvoicesList.ToArray());
            }
            else if (parm.invoiceTypes == invoiceTypes.purchase)
            {
                invoicesSearch.Add((int)DocumentType.Purchase);
            }
            else if (parm.invoiceTypes == invoiceTypes.purchaseReturn)
            {
                invoicesSearch.Add((int)DocumentType.ReturnPurchase);
            }

            var userInfo = await _iUserInformation.GetUserInformation();

            var listOfReturnInvoices = Lists.returnInvoiceList;
            var invoices = _invoiceDetailsQuery.TableNoTracking
                                                .Include(x => x.InvoicesMaster)
                                                .ThenInclude(x => x.Person)
                                                .Where(x => invoicesSearch.Contains((int)DocumentType.POS) ? (!userInfo.otherSettings.posShowOtherPersonsInv ? (x.InvoicesMaster.InvoiceTypeId == (int)DocumentType.POS ? x.InvoicesMaster.EmployeeId == userInfo.employeeId : true) : true) : true)
                                                .Where(x => invoicesSearch.Contains((int)DocumentType.Sales) ? (!userInfo.otherSettings.salesShowOtherPersonsInv ? (x.InvoicesMaster.InvoiceTypeId == (int)DocumentType.Sales ? x.InvoicesMaster.EmployeeId == userInfo.employeeId : true) : true) : true)
                                                .Where(x => invoicesSearch.Contains((int)DocumentType.Purchase) ? ((!userInfo.otherSettings.purchasesShowOtherPersonsInv ? (x.InvoicesMaster.InvoiceTypeId == (int)DocumentType.Purchase ? x.InvoicesMaster.EmployeeId == userInfo.employeeId : true) : true)
                                                           && (parm.isExpenses.Value ? x.InvoicesMaster.InvoiceTypeId == (int)DocumentType.Purchase
                                                                              && x.InvoicesMaster.isExpenses == parm.isExpenses :
                                                                              (parm.invoiceTypes == invoiceTypes.all ? true : x.InvoicesMaster.InvoiceTypeId == (int)DocumentType.Purchase
                                                                                 && !x.InvoicesMaster.isExpenses))) : true)


                                                .Where(x => invoicesSearch.Contains((int)DocumentType.ReturnPOS) ? (!userInfo.otherSettings.posShowOtherPersonsInv ? (x.InvoicesMaster.InvoiceTypeId == (int)DocumentType.ReturnPOS ? x.InvoicesMaster.EmployeeId == userInfo.employeeId : true) : true) : true)
                                                .Where(x => invoicesSearch.Contains((int)DocumentType.ReturnSales) ? (!userInfo.otherSettings.salesShowOtherPersonsInv ? (x.InvoicesMaster.InvoiceTypeId == (int)DocumentType.ReturnSales ? x.InvoicesMaster.EmployeeId == userInfo.employeeId : true) : true) : true)
                                                .Where(x => invoicesSearch.Contains((int)DocumentType.ReturnPurchase) ? ((!userInfo.otherSettings.purchasesShowOtherPersonsInv ? (x.InvoicesMaster.InvoiceTypeId == (int)DocumentType.ReturnPurchase ? x.InvoicesMaster.EmployeeId == userInfo.employeeId : true) : true)
                                                      && (parm.isExpenses.Value ? x.InvoicesMaster.InvoiceTypeId == (int)DocumentType.ReturnPurchase
                                                                              && x.InvoicesMaster.isExpenses == parm.isExpenses :
                                                                              (parm.invoiceTypes == invoiceTypes.all ? true : x.InvoicesMaster.InvoiceTypeId == (int)DocumentType.ReturnPurchase
                                                                                 && !x.InvoicesMaster.isExpenses))) : true)



                                                .Where(x => !x.InvoicesMaster.IsDeleted)
                                                .Where(x => x.InvoicesMaster.InvoiceDate.Date >= parm.dateFrom.Date && x.InvoicesMaster.InvoiceDate.Date <= parm.dateTo.Date)
                                                .Where(x => branches.Contains(x.InvoicesMaster.BranchId))
                                                .Where(x => parm.salesmanId != 0 ? x.InvoicesMaster.SalesManId == parm.salesmanId : true)
                                                .Where(x => parm.paymentTypes != PaymentType.all ? x.InvoicesMaster.PaymentType == (int)parm.paymentTypes : true)
                                                .Where(x => invoicesSearch.Contains(x.InvoicesMaster.InvoiceTypeId))
                                                .Where(x => !parm.isSales && parm.personId != 0 ? x.InvoicesMaster.PersonId == parm.personId : true)
                                                .ToList()
                                                .GroupBy(x => x.InvoiceId)
                                                .OrderBy(x => x.First().InvoicesMaster.InvoiceDate.Date)
                                                .ThenBy(x => x.First().InvoicesMaster.Serialize)
                                                .Select(x => new SalesTransactionDetalies
                                                {
                                                    isCustomer = x.First().InvoicesMaster.Person.IsCustomer,
                                                    documentCode = x.First().InvoicesMaster.InvoiceType,
                                                    date = x.First().InvoicesMaster.InvoiceDate,
                                                    documentTypeAr = x.First().InvoicesMaster.InvoiceTypeId.ToString(),
                                                    paymentTypeAr = x.First().InvoicesMaster.PaymentType.ToString(),
                                                    amount = _roundNumbers.GetRoundNumber(x.First().InvoicesMaster.TotalPrice * (listOfReturnInvoices.Contains(x.First().InvoicesMaster.InvoiceTypeId) ? -1 : 1)),
                                                    discount = _roundNumbers.GetRoundNumber(x.First().InvoicesMaster.TotalDiscountValue * (listOfReturnInvoices.Contains(x.First().InvoicesMaster.InvoiceTypeId) ? -1 : 1)),
                                                    totalAfterDiscount = _roundNumbers.GetRoundNumber(x.First().InvoicesMaster.TotalAfterDiscount * (listOfReturnInvoices.Contains(x.First().InvoicesMaster.InvoiceTypeId) ? -1 : 1)),
                                                    vat = _roundNumbers.GetRoundNumber(x.First().InvoicesMaster.TotalVat * (listOfReturnInvoices.Contains(x.First().InvoicesMaster.InvoiceTypeId) ? -1 : 1)),
                                                    net = _roundNumbers.GetRoundNumber(x.First().InvoicesMaster.Net * (listOfReturnInvoices.Contains(x.First().InvoicesMaster.InvoiceTypeId) ? -1 : 1)),
                                                    paid = x.First().InvoicesMaster.Paid < x.First().InvoicesMaster.Net ? _roundNumbers.GetRoundNumber(x.First().InvoicesMaster.Paid * (listOfReturnInvoices.Contains(x.First().InvoicesMaster.InvoiceTypeId) ? -1 : 1)) : (x.First().InvoicesMaster.InvoiceTypeId == (int)DocumentType.POS ? x.First().InvoicesMaster.Net * (listOfReturnInvoices.Contains(x.First().InvoicesMaster.InvoiceTypeId) ? -1 : 1) : x.First().InvoicesMaster.Paid * (listOfReturnInvoices.Contains(x.First().InvoicesMaster.InvoiceTypeId) ? -1 : 1)),
                                                    remin = x.First().InvoicesMaster.Paid < x.First().InvoicesMaster.Net ? _roundNumbers.GetRoundNumber(x.First().InvoicesMaster.Remain * (listOfReturnInvoices.Contains(x.First().InvoicesMaster.InvoiceTypeId) ? -1 : 1)) : 0,
                                                    rowClassName = listOfReturnInvoices.Contains(x.First().InvoicesMaster.InvoiceTypeId) ? defultData.text_danger : "",
                                                    isExpenses = x.First().InvoicesMaster.isExpenses,
                                                    invoiceTypeId = x.First().InvoicesMaster.InvoiceTypeId,
                                                    InvoiceId = x.First().InvoicesMaster.InvoiceId,
                                                    pageId = DocHelpers.GetPageIdOfDoc(x.First().InvoicesMaster.InvoiceTypeId),
                                                    pageEnum = DocHelpers.GetPageEnum(x.First().InvoicesMaster.InvoiceTypeId),
                                                    DocTypeId = -1
                                                })
                                                .ToList();

            double totalAmount = invoices.Sum(x => x.amount);
            double totalDiscount = invoices.Sum(x => x.discount);
            double totalAfterDiscount = invoices.Sum(x => x.totalAfterDiscount);
            double totalVat = invoices.Sum(x => _roundNumbers.GetRoundNumber(x.vat));
            double totalNet = invoices.Sum(x => x.net);
            double totalPaid = invoices.Sum(x => x.paid);
            double totalRemin = invoices.Sum(x => x.remin);

            double MaxPageNumber = invoices.ToList().Count() / Convert.ToDouble(parm.pageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            var ResList = !isPrint ? Pagenation<SalesTransactionDetalies>.pagenationList(parm.pageSize, parm.pageNumber, invoices) : invoices;
            var transactionList = TransactionTypeList.transactionTypeModels();
            var paymentTypes = Lists.paymentTypes;
            ResList.ForEach(x =>
                {
                    var transactionType = transactionList.Find(f => f.id.ToString() == x.documentTypeAr);
                    var _paymentTypes = paymentTypes.Find(f => f.id.ToString() == x.paymentTypeAr);
                    x.documentTypeAr = x.isExpenses.Value ? (x.invoiceTypeId == (int)DocumentType.Purchase ? "مصروفات" : "مرتجع مصروفات") : transactionType?.arabicName ?? "";
                    x.documentTypeEn = x.isExpenses.Value ? (x.invoiceTypeId == (int)DocumentType.Purchase ? "Expenses" : "Return expenses") : transactionType?.latinName ?? "";

                    x.paymentTypeAr = _paymentTypes?.arabicName ?? "";
                    x.paymentTypeEn = _paymentTypes?.latinName ?? "";
                });
            var response = new salesTransactionResponseDTO()
            {
                totalAmount = _roundNumbers.GetRoundNumber(totalAmount),
                totalDiscount = _roundNumbers.GetRoundNumber(totalDiscount),
                totalAfterDiscount = _roundNumbers.GetRoundNumber(totalAfterDiscount),
                totalVat = _roundNumbers.GetRoundNumber(totalVat),
                net = _roundNumbers.GetRoundNumber(totalNet),
                totalPaid = _roundNumbers.GetRoundNumber(totalPaid),
                totalRemin = _roundNumbers.GetRoundNumber(totalRemin),
                data = ResList
            };
            return new salesTransactionResponse()
            {
                data = response,
                notes = (countofFilter == parm.pageNumber ? Actions.EndOfData : ""),
                Result = Result.Success,
                TotalCount = ResList.Count(),
                dataCount = invoices.Count()
            };
        }

        public async Task<WebReport> SalesTransactionReport(salesTransactionRequestDTO param, int screenId, exportType exportType, bool isArabic, int fileId = 0)
        {
            var data = await GetsalesTransaction(param, true);

            var userInfo = await _iUserInformation.GetUserInformation();
            string arabicName = "";
            string latinName = "";
            if (param.personId != 0)
            {
                var personData = _personQuery.TableNoTracking.Where(p => p.Id == param.personId).FirstOrDefault();
                arabicName = personData.ArabicName;
                latinName = personData.LatinName;

            }
            else
            {
                arabicName = "الكل";
                latinName = "All";
            }

            string salesTypeAr = "";
            string paymentTypeAr = "";
            string salesTypeEn = "";
            string paymentTypeEn = "";


            // Payment Types
            if ((int)param.paymentTypes == 0)
            {
                paymentTypeAr = "الكل";
                paymentTypeEn = "All";

            }
            else if ((int)param.paymentTypes == 1)
            {
                paymentTypeAr = "مسدد";
                paymentTypeEn = "Paid";

            }
            else if ((int)param.paymentTypes == 2)
            {
                paymentTypeAr = "جزئى";
                paymentTypeEn = "Partial";


            }
            else
            {
                paymentTypeAr = "اجل";
                paymentTypeEn = "Deferred";

            }

            //Sales Types
            if ((int)param.invoiceTypes == 0)
            {
                salesTypeAr = "الكل";
                salesTypeEn = "All";

            }
            else if ((int)param.invoiceTypes == 1)
            {
                salesTypeAr = "مبيعات";
                salesTypeEn = "Sales";

            }
            else if ((int)param.invoiceTypes == 2)
            {
                salesTypeAr = "مرتجعات";
                salesTypeEn = "Returns";

            }
            else if ((int)param.invoiceTypes == 3)
            {
                salesTypeAr = "مشتريات";
                salesTypeEn = "Purchases";

            }
            else if ((int)param.invoiceTypes == 4)
            {
                salesTypeAr = "مرتجع مشتريات";
                salesTypeEn = "Return Purchases";

            }
            var dates = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, param.dateFrom, param.dateTo);

            var otherdata = new AdditionalReportData()
            {
                SalesTypeAr = salesTypeAr,
                SalesTypeEn = salesTypeEn,
                PaymentTypeAr = paymentTypeAr,
                PaymentTypeEn = paymentTypeEn,
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),
                DateFrom = dates.DateFrom,
                DateTo = dates.DateTo,
                Date = dates.Date,
                ArabicName = arabicName,
                LatinName = latinName

            };
            foreach (var item in data.data.data)
            {
                item.DocumentDate = item.date.ToString("yyyy/MM/dd");
                item.DocumentTime = item.date.ToString("HH:mm:ss");
            }

            var tablesNames = new TablesNames()
            {

                ObjectName = "salesTransactionResponse",
                FirstListName = "SalesTransactionDetalies"
            };

            var report = await _iGeneralPrint.PrintReport<salesTransactionResponseDTO, SalesTransactionDetalies, object>(data.data, data.data.data, null, tablesNames, otherdata
             , screenId, exportType, isArabic, fileId);
            return report;


        }
        public async Task<WebReport> PriceOfferReport(GetOfferPriceByIdRequest param, exportType exportType, bool isArabic, int fileId = 0)
        {
            //var data = await GetsalesTransaction(param, true);

            var result = await _mediator.Send(param);


            var userInfo = await _iUserInformation.GetUserInformation();


            //string salesTypeAr = "";
            //string paymentTypeAr = "";
            //string salesTypeEn = "";
            //string paymentTypeEn = "";


            //// Payment Types
            //if ((int)param.paymentTypes == 0)
            //{
            //    paymentTypeAr = "الكل";
            //    paymentTypeEn = "All";

            //}
            //else if ((int)param.paymentTypes == 1)
            //{
            //    paymentTypeAr = "مسدد";
            //    paymentTypeEn = "Paid";

            //}
            //else if ((int)param.paymentTypes == 2)
            //{
            //    paymentTypeAr = "جزئى";
            //    paymentTypeEn = "Partial";


            //}
            //else
            //{
            //    paymentTypeAr = "اجل";
            //    paymentTypeEn = "Deferred";

            //}

            ////Sales Types
            //if ((int)param.salesTypes == 0)
            //{
            //    salesTypeAr = "الكل";
            //    salesTypeEn = "All";

            //}
            //else if ((int)param.salesTypes == 1)
            //{
            //    salesTypeAr = "مبيعات";
            //    salesTypeEn = "Sales";

            //}
            //else if ((int)param.salesTypes == 2)
            //{
            //    salesTypeAr = "مرتجعات";
            //    salesTypeEn = "Returns";

            //}

            var otherdata = new AdditionalReportData()
            {
                //SalesTypeAr = salesTypeAr,
                //SalesTypeEn = salesTypeEn,
                //PaymentTypeAr = paymentTypeAr,
                //PaymentTypeEn = paymentTypeEn,
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),
                //DateFrom = param.dateFrom.ToString("yyyy/MM/dd"),
                //DateTo = param.dateTo.ToString("yyyy/MM/dd"),
                Date = DateTime.Now.ToString("yyyy/MM/dd")


            };
            //foreach (var item in data.data.data)
            //{
            //    item.DocumentDate = item.date.ToString("yyyy/MM/dd");
            //    item.DocumentTime = item.date.ToString("HH:mm:ss");
            //}

            var tablesNames = new TablesNames()
            {

                ObjectName = "salesTransactionResponse",
                FirstListName = "SalesTransactionDetalies"
            };

            var report = await _iGeneralPrint.PrintReport<salesTransactionResponseDTO, SalesTransactionDetalies, object>(null, null, null, tablesNames, otherdata
             , (int)SubFormsIds.SalesTransaction, exportType, isArabic, fileId);
            return report;


        }

        public async Task<ResponseResult> salesTransaction(salesTransactionRequestDTO parm, bool isPrint = false)
        {
            var res = await GetsalesTransaction(parm, isPrint);
            return new ResponseResult()
            {
                Data = res.data,
                TotalCount = res.TotalCount,
                Note = res.notes,
                Result = res.Result,
                DataCount = res.dataCount
            };
        }

        public async Task<totalSalesOfBranchesResponse> getTotalSalesOfBranch(totalSalesOfBranchesRequestDTO parm, bool isPrint = false)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var branches = parm.branches.Split(',').Select(x => int.Parse(x)).ToArray();
            var branchesData = _GLBranchQuery.TableNoTracking.Where(c => branches.Contains(c.Id));
            var paymentTypes = Lists.paymentTypes;
            var sales = Lists.salesInvoicesList;
            var pos = Lists.POSInvoicesList;
            var payments = _invoiceMasterQuery.TableNoTracking
                                                       .Include(x => x.InvoicesDetails)
                                                       .Include(x => x.Branch)
                                                       .Where(x => !x.IsDeleted)
                                                       .Where(x => !userInfo.otherSettings.salesShowOtherPersonsInv && sales.Contains(x.InvoiceTypeId) ? x.EmployeeId == userInfo.employeeId : true)
                                                       .Where(x => !userInfo.otherSettings.posShowOtherPersonsInv && pos.Contains(x.InvoiceTypeId) ? x.EmployeeId == userInfo.employeeId : true)
                                                       .Where(x => parm.paymentTypes != PaymentType.all ? x.PaymentType == (int)parm.paymentTypes : true)
                                                       .Where(x => branches[0] != 0 ? branches.Contains(x.BranchId) : true)
                                                       .Where(x => x.InvoiceDate.Date >= parm.dateFrom.Date && x.InvoiceDate.Date.Date <= parm.dateTo.Date)
                                                       .Where(x => sales.Contains(x.InvoiceTypeId) || pos.Contains(x.InvoiceTypeId))
                                                       .ToList()
                                                       .GroupBy(x => new { x.PaymentType, x.BranchId })
                                                       .Select(x => new totalSalesOfBranchesResponseList
                                                       {
                                                           paymentMethodId = x.First().PaymentType,
                                                           paymentTypeAr = paymentTypes.Where(c => c.id == x.First().PaymentType).First().arabicName,
                                                           paymentTypeEn = paymentTypes.Where(c => c.id == x.First().PaymentType).First().latinName,
                                                           amount = _roundNumbers.GetRoundNumber(x.Sum(c => c.TotalPrice * c.InvoicesDetails.FirstOrDefault().Signal)) * -1,
                                                           discount = _roundNumbers.GetRoundNumber(x.Sum(c => c.TotalDiscountValue * c.InvoicesDetails.FirstOrDefault().Signal)) * -1,
                                                           totalAfterDiscount = _roundNumbers.GetRoundNumber(x.Sum(c => c.TotalAfterDiscount * c.InvoicesDetails.FirstOrDefault().Signal)) * -1,
                                                           vat = _roundNumbers.GetRoundNumber(x.Sum(c => c.TotalVat * c.InvoicesDetails.FirstOrDefault().Signal)) * -1,
                                                           net = _roundNumbers.GetRoundNumber(x.Sum(c => c.Net * c.InvoicesDetails.FirstOrDefault().Signal)) * -1,
                                                           paid = _roundNumbers.GetRoundNumber(x.Sum(c => (c.InvoiceTypeId == (int)DocumentType.POS ? (c.Paid < c.Net ? c.Paid : c.Net) : c.Paid) * c.InvoicesDetails.FirstOrDefault().Signal)) * -1,
                                                           branchId = x.First().BranchId,
                                                           branch = new totalSalesOfBranches
                                                           {
                                                               arabicName = x.First().Branch.ArabicName,
                                                               latinName = x.First().Branch.LatinName
                                                           }
                                                       })
                                                       .OrderBy(c => c.branchId)
                                                       .ThenBy(c => c.paymentMethodId)
                                                       .ToList();

            var companyTotalSalesBranches = new List<CompanyTotalSalesBranches>();
            foreach (var branch in branchesData)
            {

                var branchData = payments.Where(c => c.branchId == branch.Id);



                var CompanyTotalSalesResponseBranches_Detalies = new List<CompanyTotalSalesResponseBranches_Detalies>();
                foreach (var paymentType in paymentTypes.Where(c => c.id != 0))
                {
                    CompanyTotalSalesResponseBranches_Detalies.Add(new Domain.Models.Response.Store.CompanyTotalSalesResponseBranches_Detalies
                    {
                        paymentTypeAr = paymentType.arabicName,
                        paymentTypeEn = paymentType.latinName,
                        amount = branchData.FirstOrDefault(c => c.paymentTypeEn == paymentType.latinName)?.amount ?? 0,
                        discount = branchData.FirstOrDefault(c => c.paymentTypeEn == paymentType.latinName)?.discount ?? 0,
                        net = branchData.FirstOrDefault(c => c.paymentTypeEn == paymentType.latinName)?.totalAfterDiscount ?? 0,
                        vat = branchData.FirstOrDefault(c => c.paymentTypeEn == paymentType.latinName)?.vat ?? 0,
                        due = branchData.FirstOrDefault(c => c.paymentTypeEn == paymentType.latinName)?.net ?? 0,
                        paied = branchData.FirstOrDefault(c => c.paymentTypeEn == paymentType.latinName)?.paid ?? 0,
                    });
                }

                companyTotalSalesBranches.Add(new CompanyTotalSalesBranches
                {
                    //branch info
                    Id = branch.Id,
                    arabicName = branch.ArabicName,
                    latinName = branch.LatinName,

                    //payment info
                    paymentTypeAr = parm.paymentTypes == PaymentType.all ? "الكل" : "",
                    paymentTypeEn = parm.paymentTypes == PaymentType.all ? "All" : "",

                    //totals info
                    amount = _roundNumbers.GetRoundNumber(branchData.Sum(c => c.amount)),
                    discount = _roundNumbers.GetRoundNumber(branchData.Sum(c => c.discount)),
                    net = _roundNumbers.GetRoundNumber(branchData.Sum(c => c.totalAfterDiscount)),
                    vat = _roundNumbers.GetRoundNumber(branchData.Sum(c => c.vat)),
                    due = _roundNumbers.GetRoundNumber(branchData.Sum(c => c.net)),
                    paied = _roundNumbers.GetRoundNumber(branchData.Sum(c => c.paid)),
                    isExpanded = false,
                    detalies = CompanyTotalSalesResponseBranches_Detalies
                });
            }

            var responseData = new CompanyTotalSalesResponseDTO
            {
                Totalamount = _roundNumbers.GetRoundNumber(payments.Sum(c => c.amount)),
                Totaldiscount = _roundNumbers.GetRoundNumber(payments.Sum(c => c.discount)),
                Totalnet = _roundNumbers.GetRoundNumber(payments.Sum(c => c.totalAfterDiscount)),
                Totalvat = _roundNumbers.GetRoundNumber(payments.Sum(c => c.vat)),
                Totaldue = _roundNumbers.GetRoundNumber(payments.Sum(c => c.net)),
                Totalpaied = _roundNumbers.GetRoundNumber(payments.Sum(c => c.paid)),
                data = companyTotalSalesBranches
            };


            return new totalSalesOfBranchesResponse()
            {
                data = responseData,
                Result = Result.Success,
            };
            throw new NotImplementedException();
        }
        public async Task<WebReport> TotalSalesOfBranchReport(totalSalesOfBranchesRequestDTO request, exportType exportType, bool isArabic, string expnadedBranchId, int fileId = 0)
        {
            var data = await getTotalSalesOfBranch(request, true);
            var branchesData = data.data.data;
            //var branches = branchesData.Select(a => a.detalies);


            if (expnadedBranchId != null)
            {

                var ids = expnadedBranchId.Split(',').Select(x => int.Parse(x)).ToArray();
                foreach (var id in ids)
                {

                    var branch = branchesData.Where(a => a.Id == id).FirstOrDefault();

                    var branchDetailes = branch.detalies.Select(b => new CompanyTotalSalesBranches
                    {
                        arabicName = "",
                        latinName = "",
                        paymentTypeAr = b.paymentTypeAr,
                        paymentTypeEn = b.paymentTypeEn,
                        amount = b.amount,
                        discount = b.discount,
                        net = b.net,
                        vat = b.vat,
                        due = b.due,
                        paied = b.paied,

                        Id = 0

                    });

                    //var branch = branchesData.Where(a => a.Id == id).FirstOrDefault();

                    int index = branchesData.IndexOf(branch);
                    //branch.detalies.Clear();
                    branchesData.InsertRange(index + 1, branchDetailes);

                }
            }
            branchesData.Where(a => a.Id != 0).ToList().ForEach(a => a.detalies.Clear());
            //var DetailesOfBranch = branchesData.Select(a => a.detalies).ToList();
            //var branches= branchesData.exc(DetailesOfBranch)

            string paymentTypeAr = "";

            string paymentTypeEn = "";
            // Payment Types
            if ((int)request.paymentTypes == 0)
            {
                paymentTypeAr = "الكل";
                paymentTypeEn = "All";

            }
            else if ((int)request.paymentTypes == 1)
            {
                paymentTypeAr = "نقدى";
                paymentTypeEn = "Cash";

            }
            else if ((int)request.paymentTypes == 2)
            {
                paymentTypeAr = "جزئى";
                paymentTypeEn = "Partial";


            }
            else
            {
                paymentTypeAr = "اجل";
                paymentTypeEn = "Deferred";

            }



            var userInfo = await _iUserInformation.GetUserInformation();

            var otherdata = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, request.dateFrom, request.dateTo);

            otherdata.ArabicName = paymentTypeAr;
            otherdata.LatinName = paymentTypeEn;
            otherdata.EmployeeName = userInfo.employeeNameAr.ToString();
            otherdata.EmployeeNameEn = userInfo.employeeNameEn.ToString();



            var tablesNames = new TablesNames()
            {

                ObjectName = "TotalSalesOfBranch",
                FirstListName = "TotalSalesOfBranchList"
            };




            var report = await _iGeneralPrint.PrintReport<CompanyTotalSalesResponseDTO, CompanyTotalSalesBranches, object>(data.data, branchesData, null, tablesNames, otherdata
             , (int)SubFormsIds.TotalSalesOfBranch, exportType, isArabic, fileId);
            return report;
        }

        public async Task<ResponseResult> TotalSalesOfBranch(totalSalesOfBranchesRequestDTO parm)
        {
            var res = await getTotalSalesOfBranch(parm);
            return new ResponseResult()
            {
                Data = res.data,
                DataCount = res.dataCount,
                TotalCount = res.TotalCount,
                Note = res.notes,
                Result = res.Result
            };
        }

        public async Task<itemSalesForCustomersResponse> getItemSalesForCustomers(itemSalesForCustomersRequestDTO parm, bool isPrint = false)
        {
            var branches = parm.branches.Split(',').Select(x => int.Parse(x)).ToArray();
            var paymentTypes = Lists.paymentTypes;
            var salesEnum = Lists.salesInvoicesList;
            var posEnum = Lists.POSInvoicesList;

            var reportItem = await _invStpItemCardMasterQuery.GetByIdAsync(parm.itemId);
            var invoices = _invoiceDetailsQuery.TableNoTracking
                                               .Include(x => x.InvoicesMaster)
                                               .Include(x => x.InvoicesMaster.Person)
                                               .Where(x => !x.InvoicesMaster.IsDeleted)
                                               .Where(x => salesEnum.Contains(x.InvoicesMaster.InvoiceTypeId) || posEnum.Contains(x.InvoicesMaster.InvoiceTypeId))
                                               .Where(x => x.ItemId == parm.itemId)
                                               .Where(x => x.InvoicesMaster.PersonId != null)
                                               .Where(x => branches.Contains(x.InvoicesMaster.BranchId))
                                               .Where(x => x.InvoicesMaster.InvoiceDate.Date >= parm.dateFrom && x.InvoicesMaster.InvoiceDate.Date <= parm.dateTo)
                                               .ToList()
                                               .GroupBy(x => new { x.InvoicesMaster.PersonId, x.InvoicesMaster.PaymentType })
                                               .Select(x => new itemSalesForCustomersResponseList
                                               {
                                                   PersonCode = x.First().InvoicesMaster.Person.Code,
                                                   arabicName = x.First().InvoicesMaster.Person.ArabicName,
                                                   latinName = x.First().InvoicesMaster.Person.LatinName,

                                                   paymentMethodAr = paymentTypes.Where(c => c.id == x.First().InvoicesMaster.PaymentType).FirstOrDefault()?.arabicName ?? "",
                                                   paymentMethodEn = paymentTypes.Where(c => c.id == x.First().InvoicesMaster.PaymentType).FirstOrDefault()?.latinName ?? "",
                                                   qyt = _roundNumbers.GetRoundNumber(Math.Abs(ReportData<InvoiceDetails>.Quantity(x, reportItem.ReportUnit ?? 0))),
                                                   avgPrice = _roundNumbers.GetRoundNumber(Math.Abs(ReportData<InvoiceDetails>.avgPrice(x, reportItem.ReportUnit ?? 0))),
                                                   total = _roundNumbers.GetRoundNumber(Math.Abs(ReportData<InvoiceDetails>.Total(x))),
                                                   discount = _roundNumbers.GetRoundNumber(Math.Abs(ReportData<InvoiceDetails>.Discount(x))),
                                                   vat = _roundNumbers.GetRoundNumber(Math.Abs(ReportData<InvoiceDetails>.Vat(x))),
                                                   net = _roundNumbers.GetRoundNumber(Math.Abs(ReportData<InvoiceDetails>.Net(x))),
                                               })
                                               .Where(x => x.qyt != 0)
                                               .OrderBy(x => x.PersonCode)
                                               .ToList();
            double MaxPageNumber = invoices.Count() / Convert.ToDouble(parm.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            var resData = !isPrint ? Pagenation<itemSalesForCustomersResponseList>.pagenationList(parm.PageSize, parm.PageNumber, invoices) : invoices;
            itemSalesForCustomersResponseDTO itemSalesForCustomersResponseDTO = new itemSalesForCustomersResponseDTO()
            {
                totalQyt = _roundNumbers.GetRoundNumber(invoices.Sum(x => x.qyt)),
                totalDiscount = _roundNumbers.GetRoundNumber(invoices.Sum(x => x.discount)),
                totalNet = _roundNumbers.GetRoundNumber(invoices.Sum(x => x.net)),
                totalTotal = _roundNumbers.GetRoundNumber(invoices.Sum(x => x.total)),
                totalVat = _roundNumbers.GetRoundNumber(invoices.Sum(x => x.vat)),
                totalAvgPrice = _roundNumbers.GetRoundNumber(invoices.Sum(x => x.avgPrice)),

                itemSalesForCustomersResponseLists = resData
            };
            return new itemSalesForCustomersResponse()
            {
                data = itemSalesForCustomersResponseDTO,
                dataCount = itemSalesForCustomersResponseDTO.itemSalesForCustomersResponseLists.Count(),
                TotalCount = invoices.Count(),
                Result = Result.Success,
                notes = (countofFilter == parm.PageNumber ? Actions.EndOfData : "")
            };
            throw new NotImplementedException();
        }
        public async Task<WebReport> ItemSalesForCustomersReport(itemSalesForCustomersRequestDTO request, exportType exportType, bool isArabic, int fileId = 0)
        {
            var data = await getItemSalesForCustomers(request, true);

            var itemData = _invStpItemCardMasterQuery.TableNoTracking.Where(a => a.Id == request.itemId).FirstOrDefault();
            string paymentTypeAr = "";

            string paymentTypeEn = "";
            // Payment Types
            if ((int)request.paymentType == 0)
            {
                paymentTypeAr = "الكل";
                paymentTypeEn = "All";

            }
            else if ((int)request.paymentType == 1)
            {
                paymentTypeAr = "مسدد";
                paymentTypeEn = "Paid";

            }
            else if ((int)request.paymentType == 2)
            {
                paymentTypeAr = "جزئى";
                paymentTypeEn = "Partial";


            }
            else
            {
                paymentTypeAr = "اجل";
                paymentTypeEn = "Deferred";

            }



            var userInfo = await _iUserInformation.GetUserInformation();
            var dates = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, request.dateFrom, request.dateTo);


            var otherdata = new AdditionalReportData()
            {
                Id = itemData.Id,
                ArabicName = itemData.ArabicName,
                LatinName = itemData.LatinName,

                PaymentTypeAr = paymentTypeAr,
                PaymentTypeEn = paymentTypeEn,


                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),

                DateFrom = dates.DateFrom,
                DateTo = dates.DateTo,
                Date = dates.Date

            };

            var tablesNames = new TablesNames()
            {

                ObjectName = "ItemSalesForCustomers",
                FirstListName = "ItemSalesForCustomersList"
            };




            var report = await _iGeneralPrint.PrintReport<itemSalesForCustomersResponseDTO, itemSalesForCustomersResponseList, object>(data.data, data.data.itemSalesForCustomersResponseLists, null, tablesNames, otherdata
             , (int)SubFormsIds.itemSalesForCustomers, exportType, isArabic, fileId);
            return report;
        }


        public async Task<ResponseResult> ItemSalesForCustomers(itemSalesForCustomersRequestDTO parm)
        {
            var res = await getItemSalesForCustomers(parm);
            return new ResponseResult()
            {
                Data = res.data,
                DataCount = res.dataCount,
                TotalCount = res.TotalCount,
                Note = res.notes,
                Result = res.Result
            };
        }

        public async Task<itemsNotSoldResponse> getItemsNotSold(itemsNotSoldRequstDTO parm, bool isPrint = false)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var branches = parm.branches.Split(',').Select(c => int.Parse(c)).ToArray();
            var salesList = Lists.salesInvoicesList;
            var posList = Lists.POSInvoicesList;
            salesList.AddRange(posList);
            var itemsIdSold = _invoiceDetailsQuery.TableNoTracking
                                               .Include(x => x.InvoicesMaster)
                                               .Include(x => x.Items)
                                               .Where(x => salesList.Contains(x.InvoicesMaster.InvoiceTypeId) && !userInfo.otherSettings.salesShowOtherPersonsInv ? x.InvoicesMaster.EmployeeId == userInfo.employeeId : true)
                                               .Where(x => posList.Contains(x.InvoicesMaster.InvoiceTypeId) && !userInfo.otherSettings.salesShowOtherPersonsInv ? x.InvoicesMaster.EmployeeId == userInfo.employeeId : true)
                                               .Where(x => branches.Contains(x.InvoicesMaster.BranchId))
                                               .Where(x => !x.InvoicesMaster.IsDeleted)
                                               .Where(x => parm.catId != null ? x.Items.GroupId == parm.catId : true)
                                               .Where(x => x.InvoicesMaster.InvoiceDate.Date >= parm.dateFrom.Date && x.InvoicesMaster.InvoiceDate.Date <= parm.dateTo.Date)
                                               .Where(x => salesList.Contains(x.InvoicesMaster.InvoiceTypeId))
                                               .GroupBy(x => x.ItemId)
                                               .Where(x => x.Sum(c => c.Quantity * c.Signal) != 0)
                                               .Select(x => x.FirstOrDefault().ItemId)
                                               .ToList()
                                               .ToArray();

            var itemsNotSold = _invStpItemCardMasterQuery.TableNoTracking
                                                     .Include(x => x.Category)
                                                     .Where(x => !itemsIdSold.Contains(x.Id))
                                                     .Where(x => parm.itemId != null ? x.Id == parm.itemId : true)
                                                     .Where(x => x.TypeId != (int)ItemTypes.Note)
                                                     .Select(x => new itemsNotSoldResponseList
                                                     {
                                                         itemCode = x.ItemCode,
                                                         arabicName = x.ArabicName,
                                                         latinName = x.LatinName,
                                                         catNameAr = x.Category.ArabicName,
                                                         catNameEn = x.Category.LatinName
                                                     }).ToList();
            double MaxPageNumber = itemsNotSold.Count() / Convert.ToDouble(parm.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            var resData = !isPrint ? Pagenation<itemsNotSoldResponseList>.pagenationList(parm.PageSize, parm.PageNumber, itemsNotSold) : itemsNotSold;
            return new itemsNotSoldResponse()
            {
                itemsNotSoldResponseLists = resData,
                dataCount = resData.Count(),
                totalCount = itemsNotSold.Count(),
                Result = Result.Success,
                notes = (countofFilter == parm.PageNumber ? Actions.EndOfData : "")
            };
        }
        public async Task<WebReport> ItemsNotSoldReport(itemsNotSoldRequstDTO param, exportType exportType, bool isArabic, int fileId = 0)
        {


            if (param.catId == 0)
            {
                param.catId = null;
            }
            if (param.itemId == 0)
            {
                param.itemId = null;
            }


            var data = await getItemsNotSold(param, true);

            var userInfo = await _iUserInformation.GetUserInformation();

            var otherdata = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, param.dateFrom, param.dateTo);

            otherdata.EmployeeName = userInfo.employeeNameAr.ToString();
            otherdata.EmployeeNameEn = userInfo.employeeNameEn.ToString();

            var tablesNames = new TablesNames()
            {
                FirstListName = "ItemsNotSoldResponseList"
            };

            var report = await _iGeneralPrint.PrintReport<object, itemsNotSoldResponseList, object>(null, data.itemsNotSoldResponseLists, null, tablesNames, otherdata
             , (int)SubFormsIds.ItemNotSold, exportType, isArabic, fileId);
            return report;

        }

        public async Task<ResponseResult> ItemsNotSold(itemsNotSoldRequstDTO parm)
        {
            var res = await getItemsNotSold(parm);
            return new ResponseResult()
            {
                Data = res.itemsNotSoldResponseLists,
                DataCount = res.dataCount,
                TotalCount = res.totalCount,
                Result = res.Result,
                Note = res.notes
            };
        }

        public async Task<salesAndSalesReturnTransactionResponse> getsalesAndSalesReturnTransaction(salesAndSalesReturnTransactionRequstDTO parm, bool isPrint = false)
        {
            var salesLists = new List<int>();
            if (parm.salesType == salesType.all)
                salesLists = Lists.SalesWithOutDeleteInvoicesList;
            else if (parm.salesType == salesType.sales)
                salesLists = Lists.OnlySalesWithOutDeleteInvoicesList;
            else if (parm.salesType == salesType.returnInvoices)
                salesLists = Lists.OnlyReturnWithOutDeleteInvoicesList;
            var paymentType = Lists.paymentTypes;
            var transactionList = TransactionTypeList.transactionTypeModels();
            var units = _invStpUnitsQuery.TableNoTracking;
            var returnsList = Lists.returnInvoiceList;
            var itemsInInvoices = _invoiceDetailsQuery.TableNoTracking
                                                      .Include(x => x.InvoicesMaster)
                                                      .Include(x => x.Items)
                                                      .Include(x => x.Items.Units)
                                                      .Where(x => x.InvoicesMaster.InvoiceDate.Date >= parm.dateFrom.Date && x.InvoicesMaster.InvoiceDate.Date <= parm.dateTo.Date)
                                                      .Where(x => salesLists.Contains(x.InvoicesMaster.InvoiceTypeId))
                                                      .Where(x => !x.InvoicesMaster.IsDeleted)
                                                      .Where(x => parm.itemId != null ? x.ItemId == parm.itemId : true)
                                                      .Where(x => parm.paymentTypes != 0 ? x.InvoicesMaster.PaymentType == (int)parm.paymentTypes : true)
                                                      .ToList()
                                                      .Select(x => new salesAndSalesReturnTransactionResponseList
                                                      {
                                                          // date = x.InvoicesMaster.InvoiceDate.ToString(defultData.datetimeFormat),
                                                          date = x.InvoicesMaster.InvoiceDate.ToString("yyyy/mm/dd"),

                                                          invoiceCode = x.InvoicesMaster.InvoiceType,
                                                          invoiceTypeAr = transactionList.Where(c => c.id == x.InvoicesMaster.InvoiceTypeId).FirstOrDefault()?.arabicName ?? "",
                                                          invoiceTypeEn = transactionList.Where(c => c.id == x.InvoicesMaster.InvoiceTypeId).FirstOrDefault()?.latinName ?? "",
                                                          itemArabicName = x.Items.ArabicName,
                                                          itemLatinName = x.Items.LatinName,
                                                          unitArabicName = units.Where(c => c.Id == x.Items.ReportUnit).FirstOrDefault()?.ArabicName ?? "",
                                                          unitLatinName = units.Where(c => c.Id == x.Items.ReportUnit).FirstOrDefault()?.LatinName ?? "",
                                                          qyt = _roundNumbers.GetRoundNumber((x.Quantity * x.ConversionFactor) / x.Items.Units.Where(c => c.UnitId == x.Items.ReportUnit).FirstOrDefault()?.ConversionFactor ?? 1),
                                                          price = _roundNumbers.GetRoundNumber(x.Price),
                                                          total = _roundNumbers.GetRoundNumber(x.TotalWithSplitedDiscount - x.DiscountValue),
                                                          discount = _roundNumbers.GetRoundNumber(x.DiscountValue),
                                                          net = _roundNumbers.GetRoundNumber(x.TotalWithSplitedDiscount),
                                                          ItemCode = x.Items.ItemCode,

                                                          rowClassName = returnsList.Where(c => c == x.InvoicesMaster.InvoiceTypeId).Any() ? defultData.text_danger : ""
                                                      })
                                                      .ToList();
            double MaxPageNumber = itemsInInvoices.Count() / Convert.ToDouble(parm.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            var resData = !isPrint ? Pagenation<salesAndSalesReturnTransactionResponseList>.pagenationList(parm.PageSize, parm.PageNumber, itemsInInvoices) : itemsInInvoices;
            salesAndSalesReturnTransactionResponseDTO salesAndSalesReturnTransactionResponseDTO = new salesAndSalesReturnTransactionResponseDTO()
            {
                list = resData,
                TotalDiscount = _roundNumbers.GetRoundNumber(itemsInInvoices.Sum(x => x.discount)),
                TotalPrice = _roundNumbers.GetRoundNumber(itemsInInvoices.Sum(x => x.price)),
                Total = _roundNumbers.GetRoundNumber(itemsInInvoices.Sum(x => x.total)),
                TotalNet = _roundNumbers.GetRoundNumber(itemsInInvoices.Sum(x => x.net)),
            };
            return new salesAndSalesReturnTransactionResponse()
            {
                data = salesAndSalesReturnTransactionResponseDTO,
                dataCount = resData.Count(),
                totalCount = itemsInInvoices.Count(),
                Result = Result.Success,
                notes = (countofFilter == parm.PageNumber ? Actions.EndOfData : "")
            };
            throw new NotImplementedException();
        }
        public async Task<WebReport> salesAndSalesReturnTransactionReport(salesAndSalesReturnTransactionRequstDTO request, exportType exportType, bool isArabic, int fileId = 0)
        {
            var data = await getsalesAndSalesReturnTransaction(request, true);

            var itemData = _invStpItemCardMasterQuery.TableNoTracking.Where(a => a.Id == request.itemId).FirstOrDefault();
            string salesTypeAr = "";
            string paymentTypeAr = "";
            string salesTypeEn = "";
            string paymentTypeEn = "";
            // Payment Types
            if ((int)request.paymentTypes == 0)
            {
                paymentTypeAr = "الكل";
                paymentTypeEn = "All";

            }
            else if ((int)request.paymentTypes == 1)
            {
                paymentTypeAr = "مسدد";
                paymentTypeEn = "Paid";

            }
            else if ((int)request.paymentTypes == 2)
            {
                paymentTypeAr = "جزئى";
                paymentTypeEn = "Partial";


            }
            else
            {
                paymentTypeAr = "اجل";
                paymentTypeEn = "Deferred";

            }
            //sales type
            if ((int)request.salesType == 0)
            {
                salesTypeAr = "الكل";
                salesTypeEn = "All";

            }
            else if ((int)request.salesType == 1)
            {
                salesTypeAr = "مبيعات";
                salesTypeEn = "Sales";

            }
            else if ((int)request.salesType == 2)
            {
                salesTypeAr = "مرتجعات";
                salesTypeEn = "Returns";

            }


            var userInfo = await _iUserInformation.GetUserInformation();


            var dates = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, request.dateFrom, request.dateTo);
            var otherdata = new AdditionalReportData()
            {
                // Id = itemData.Id,
                ArabicName = salesTypeAr,
                LatinName = salesTypeEn,
                PaymentTypeAr = paymentTypeAr,
                PaymentTypeEn = paymentTypeEn,
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),
                DateFrom = dates.DateFrom,
                DateTo = dates.DateTo,
                Date = dates.Date
            };

            var tablesNames = new TablesNames()
            {

                ObjectName = "SalesAndSalesReturnTransaction",
                FirstListName = "SalesAndSalesReturnTransactionList"
            };




            var report = await _iGeneralPrint.PrintReport<salesAndSalesReturnTransactionResponseDTO, salesAndSalesReturnTransactionResponseList, object>(data.data, data.data.list, null, tablesNames, otherdata
             , (int)SubFormsIds.SalesAndReturnSalesTransaction, exportType, isArabic, fileId);
            return report;
        }

        public async Task<ResponseResult> salesAndSalesReturnTransaction(salesAndSalesReturnTransactionRequstDTO parm)
        {
            var res = await getsalesAndSalesReturnTransaction(parm);
            return new ResponseResult()
            {
                Data = res.data,
                DataCount = res.dataCount,
                TotalCount = res.totalCount,
                Result = res.Result,
                Note = res.notes
            };
        }

        public async Task<itemsSoldMostResponse> getitemsSoldMost(itemsSoldMostRequstDTO parm, bool fromDashboard = false, bool isPrint = false)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var branches = parm.branches.Split(',').Select(c => int.Parse(c)).ToArray();
            var salesInvoicesEnum = Lists.SalesWithOutDeleteInvoicesList;
            var POSInvoicesEnum = Lists.POSInvoicesList;
            var units = _invStpUnitsQuery.TableNoTracking.ToList();
            var invoices = _invoiceDetailsQuery.TableNoTracking
                                               .Include(x => x.InvoicesMaster)
                                               .Include(x => x.Items)
                                               .Include(x => x.Items.Units)
                                               .Where(x => Lists.salesInvoicesList.Contains(x.InvoicesMaster.InvoiceTypeId) ? ((!fromDashboard ? !userInfo.otherSettings.salesShowOtherPersonsInv : !userInfo.otherSettings.showDashboardForAllUsers) ? x.InvoicesMaster.EmployeeId == userInfo.employeeId : true) : true)
                                               .Where(x => Lists.POSInvoicesList.Contains(x.InvoicesMaster.InvoiceTypeId) ? ((!fromDashboard ? !userInfo.otherSettings.salesShowOtherPersonsInv : !userInfo.otherSettings.showDashboardForAllUsers) ? x.InvoicesMaster.EmployeeId == userInfo.employeeId : true) : true)
                                               .Where(x => !x.InvoicesMaster.IsDeleted)
                                               .Where(x => !fromDashboard ? branches.Contains(x.InvoicesMaster.BranchId) : x.InvoicesMaster.BranchId == userInfo.CurrentbranchId)
                                               .Where(x => salesInvoicesEnum.Contains(x.InvoicesMaster.InvoiceTypeId))
                                               .Where(x => x.InvoicesMaster.InvoiceDate.Date >= parm.dateFrom.Date && x.InvoicesMaster.InvoiceDate.Date <= parm.dateTo.Date)
                                               .Where(x => parm.itemId != null ? x.ItemId == parm.itemId : true)
                                               .Where(x => parm.catId != null ? x.Items.GroupId == parm.catId : true)
                                               .Where(x => x.ItemTypeId != (int)ItemTypes.Note)
                                               .Where(x => x.parentItemId == null)
                                               .ToList()
                                               .GroupBy(x => new { x.ItemId })
            .Select(x => new itemsSoldMostResponseList
            {
                itemCode = x.First().Items.ItemCode,
                arabicName = x.First().Items.ArabicName,
                latinName = x.First().Items.LatinName,
                unitArabicName = units.Where(c => c.Id == x.First().Items.ReportUnit).FirstOrDefault()?.ArabicName ?? "",
                unitLatinName = units.Where(c => c.Id == x.First().Items.ReportUnit).FirstOrDefault()?.LatinName ?? "",

                Qyt = ReportData<InvoiceDetails>.Quantity(x, x.First().Items.Units.Where(d => d.UnitId == x.First().Items.ReportUnit).First().ConversionFactor),
                AvgPrice = ReportData<InvoiceDetails>.avgPrice(x, x.First().Items.Units.Where(d => d.UnitId == x.First().Items.ReportUnit).First().ConversionFactor),

                Discount = ReportData<InvoiceDetails>.Discount(x) * -1,
                Vat = x.Sum(c => c.VatValue * (c.Signal * -1)),

                Cost = 0/*Math.Abs(_roundNumbers.GetRoundNumber(x.Sum(c => c.Total)))*/,
                Net = 0/*Math.Abs(_roundNumbers.GetRoundNumber(x.Sum(c => c.Total - c.DiscountValue)))*/,
                NetWithVat = _roundNumbers.GetRoundNumber(x.Sum(c => c.TotalWithSplitedDiscount + (c.InvoicesMaster.PriceWithVat ? 0 : c.VatValue)))
            })
            .Where(x => x.Qyt != 0).ToList();


            invoices.ForEach(x =>
            {
                var qyt = x.Qyt * -1;
                var cost = qyt * x.AvgPrice;
                x.Cost = _roundNumbers.GetRoundNumber(cost);
                x.Net = _roundNumbers.GetRoundNumber(cost - x.Discount);
                x.Vat = _roundNumbers.GetRoundNumber(x.Vat) * (qyt > 0 ? 1 : -1);
                x.Qyt = _roundNumbers.GetRoundNumber(qyt);
                x.AvgPrice = _roundNumbers.GetRoundNumber(x.AvgPrice);
            });


            if (parm.itemSoldMostEnum == itemSoldMostEnum.cost)
            {
                invoices = invoices.OrderByDescending(x => x.Cost).Take(parm.searchValue).ToList();
            }
            else if (parm.itemSoldMostEnum == itemSoldMostEnum.qyt)
            {
                invoices = invoices.OrderByDescending(x => x.Qyt).Take(parm.searchValue).ToList();
            }
            else if (parm.itemSoldMostEnum == itemSoldMostEnum.avgPrice)
            {
                invoices = invoices.OrderByDescending(x => x.AvgPrice).Take(parm.searchValue).ToList();
            }

            var totalQyt = _roundNumbers.GetRoundNumber(invoices.Sum(c => c.Qyt));
            var totalAvgPrice = _roundNumbers.GetRoundNumber(invoices.Sum(c => c.AvgPrice));
            var totalCost = _roundNumbers.GetRoundNumber(invoices.Sum(c => c.Cost));
            var totalDiscount = _roundNumbers.GetRoundNumber(invoices.Sum(c => c.Discount));
            var totalVat = _roundNumbers.GetRoundNumber(invoices.Sum(c => c.Vat));
            var totalNet = _roundNumbers.GetRoundNumber(invoices.Sum(c => c.Net));
            double MaxPageNumber = invoices.Count() / Convert.ToDouble(parm.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            var resData = !isPrint ? Pagenation<itemsSoldMostResponseList>.pagenationList(parm.PageSize, parm.PageNumber, invoices) : invoices;

            itemsSoldMostResponseDTO itemsSoldMostResponseDTO = new itemsSoldMostResponseDTO()
            {
                TotalQyt = totalQyt,
                TotalAvgPrice = totalAvgPrice,
                TotalCost = totalCost,
                TotalVat = totalVat,
                TotalNet = totalNet,
                TotalDiscount = totalDiscount,
                itemsSoldMostResponseLists = resData
            };
            return new itemsSoldMostResponse()
            {
                data = itemsSoldMostResponseDTO,
                dataCount = resData.Count(),
                totalCount = invoices.Count(),
                Result = invoices.Any() ? Result.Success : Result.NoDataFound,
                notes = (countofFilter == parm.PageNumber ? Actions.EndOfData : "")
            };


        }
        public async Task<WebReport> ItemsSoldMostReport(itemsSoldMostRequstDTO param, exportType exportType, bool isArabic, int fileId = 0)
        {
            string salesTypeAr = "";
            string salesTypeEn = "";

            if (param.catId == 0)
            {
                param.catId = null;
            }
            if (param.itemId == 0)
            {
                param.itemId = null;
            }

            if ((int)param.itemSoldMostEnum == 1)
            {
                salesTypeAr = "الكمية";
                salesTypeEn = "Amount";
            }
            else if ((int)param.itemSoldMostEnum == 2)
            {
                salesTypeAr = "متوسط السعر";
                salesTypeEn = "Average Price";

            }
            else if ((int)param.itemSoldMostEnum == 3)
            {
                salesTypeAr = "القيمة";
                salesTypeEn = "Value";

            }
            var data = await getitemsSoldMost(param, true);

            var userInfo = await _iUserInformation.GetUserInformation();

            var dates = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, param.dateFrom, param.dateTo);

            var otherdata = new AdditionalReportData()
            {
                SalesTypeAr = salesTypeAr,
                SalesTypeEn = salesTypeEn,

                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),


                DateFrom = dates.DateFrom,
                DateTo = dates.DateTo,
                Date = dates.Date

            };



            var tablesNames = new TablesNames()
            {
                ObjectName = "ItemsSoldMostResponse",
                FirstListName = "ItemsSoldMostResponseList"
            };

            var report = await _iGeneralPrint.PrintReport<itemsSoldMostResponseDTO, itemsSoldMostResponseList, object>(data.data, data.data.itemsSoldMostResponseLists, null, tablesNames, otherdata
             , (int)SubFormsIds.itemsSoldMost, exportType, isArabic, fileId);
            return report;

            #region commented
            //Type OtherDataType = otherdata.GetType();

            //    IList<PropertyInfo> otherDataProps = new List<PropertyInfo>(OtherDataType.GetProperties());

            //    Type myTypeCompany = companydata.Data.GetType();

            //    IList<PropertyInfo> propsCompany = new List<PropertyInfo>(myTypeCompany.GetProperties());


            //    Type mainDataType = data.data.GetType();
            //    Type listType = data.data.itemsSoldMostResponseLists[0].GetType();

            //    IList<PropertyInfo> propsmainData = new List<PropertyInfo>(mainDataType.GetProperties());
            //    IList<PropertyInfo> propsList = new List<PropertyInfo>(listType.GetProperties());

            //    DataTable mainDataTable = new DataTable();
            //    DataRow drmainData = mainDataTable.NewRow();

            //    DataTable listTable = new DataTable();
            //    DataRow drlist = listTable.NewRow();

            //    DataTable otherDataTable = new DataTable();

            //    DataRow drotherData = otherDataTable.NewRow();


            //    DataTable CompanyTable = new DataTable();
            //    DataRow drCompany = CompanyTable.NewRow();
            //    foreach (var Property in propsCompany)
            //    {
            //        CompanyTable.Columns.Add(Property.Name);

            //        if (Property.Name != "imageFile")
            //        {
            //            var value = Property.GetValue(companydata.Data);
            //            if (value == null)
            //            {
            //                drCompany[Property.Name] = "";
            //            }
            //            else
            //            {


            //                var columnData = Property.GetValue(companydata.Data).ToString();
            //                if (columnData == "null")
            //                {
            //                    drCompany[Property.Name] = "";
            //                }
            //                else
            //                    drCompany[Property.Name] = Property.GetValue(companydata.Data).ToString();

            //            }
            //        }
            //    }
            //    CompanyTable.Rows.Add(drCompany);

            //    foreach (var Property in otherDataProps)
            //    {

            //        otherDataTable.Columns.Add(Property.Name);
            //        drotherData[Property.Name] = Property.GetValue(otherdata);

            //    }
            //    otherDataTable.Rows.Add(drotherData);

            //    foreach (var Property in propsmainData)
            //    {
            //        mainDataTable.Columns.Add(Property.Name);
            //        drmainData[Property.Name] = Property.GetValue(data.data);
            //    }
            //    mainDataTable.Rows.Add(drmainData);

            //    for (int i = 0; i < data.data.itemsSoldMostResponseLists.Count(); i++)
            //    {
            //        foreach (var prop in propsList)
            //        {
            //            if (i == 0)
            //            {
            //                listTable.Columns.Add(prop.Name);
            //            }
            //            drlist[columnName: prop.Name] = prop.GetValue(data.data.itemsSoldMostResponseLists[i]);

            //        }
            //        listTable.Rows.Add(drlist);
            //        drlist = listTable.NewRow();
            //    }



            //    mainDataTable.TableName = "ItemsSoldMostResponse";
            //    listTable.TableName = "ItemsSoldMostResponseList";
            //    CompanyTable.TableName = "CompanyData";
            //    otherDataTable.TableName = "ReportOtherData";
            //    List<DataTable> tables = new List<DataTable>
            //{
            //    mainDataTable,
            //    listTable,
            //    CompanyTable,
            //    otherDataTable
            //};




            //    ReportRequestDto reportRequest = new ReportRequestDto()
            //    {
            //        screenId = (int)SubFormsIds.itemsSoldMost,
            //        isArabic = isArabic,
            //        exportType = exportType
            //    };

            //    var fileContents = await _filesMangerService.GetPrintFiles(reportRequest.screenId, reportRequest.isArabic);

            //    return await _iprintService.Report(tables, fileContents.Files, exportType);

            #endregion
        }


        public async Task<ResponseResult> itemsSoldMost(itemsSoldMostRequstDTO parm)
        {
            var res = await getitemsSoldMost(parm);
            return new ResponseResult()
            {
                Data = res.data,
                DataCount = res.dataCount,
                TotalCount = res.totalCount,
                Result = res.Result,
                Note = res.notes
            };
        }

        public async Task<totalBranchTransactionResponse> getTotalBranchTransaction(totalBranchTransactionRequestDTO parm, bool isPrint = false)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var branches = parm.branches.Split(',').Select(x => int.Parse(x)).ToArray();
            var db_Branches = _GLBranchQuery.TableNoTracking.Where(c => branches.Contains(c.Id));
            #region Invoices
            var invoicesLists = Lists.invociesTransaction;
            var transactionTypes = TransactionTypeList.transactionTypeModels();
            var _invoices = _invoiceMasterQuery.TableNoTracking
                .Include(x => x.InvoicesDetails)
                .Include(x => x.Branch)
                .Where(x => branches.Contains(x.BranchId))
                .Where(x => x.InvoiceDate.Date >= parm.dateFrom.Date && x.InvoiceDate.Date <= parm.dateTo.Date)
                .Where(x => parm.employeeId != 0 ? x.EmployeeId == parm.employeeId : true)
                .Where(x => invoicesLists.Contains(x.InvoiceTypeId))
                //.Where(x=> userInfo.otherSettings.posShowOtherPersonsInv ? true : ((x.InvoiceTypeId == (int)Enums.DocumentType.POS || x.InvoiceTypeId == (int)Enums.DocumentType.ReturnPOS) ? x.EmployeeId == userInfo.employeeId : true))
                //.Where(x=> userInfo.otherSettings.salesShowOtherPersonsInv ? true : ((x.InvoiceTypeId == (int)Enums.DocumentType.Sales || x.InvoiceTypeId == (int)Enums.DocumentType.ReturnSales) ? x.EmployeeId == userInfo.employeeId : true))
                //.Where(x=> userInfo.otherSettings.purchasesShowOtherPersonsInv ? true : ((x.InvoiceTypeId == (int)Enums.DocumentType.Purchase || x.InvoiceTypeId == (int)Enums.DocumentType.ReturnPurchase) ? x.EmployeeId == userInfo.employeeId : true))
                .ToHashSet()
                .AsParallel();

            //sales
            var totalBranchSales_branchesDetails = new List<totalBranchTransaction_branchesDetails>();//فروع المبيعات
            var totalBranchReturnSales_branchesDetails = new List<totalBranchTransaction_branchesDetails>();//فروع مرتج المبيعات
            var totalBranchPurchase_branchesDetails = new List<totalBranchTransaction_branchesDetails>();//فروع المشتريات
            var totalBranchReturnPurchase_branchesDetails = new List<totalBranchTransaction_branchesDetails>();//فروع مرتجع المشتريات
            var invoicesSplitedByBranch = _invoices.GroupBy(c => c.BranchId);
            foreach (var item in invoicesSplitedByBranch)
            {
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
                var totalBranchSales_Invoices = item.Where(x => x.InvoiceTypeId == (int)Enums.DocumentType.Sales || x.InvoiceTypeId == (int)Enums.DocumentType.POS);
                totalBranchSales_branchesDetails.Add(new totalBranchTransaction_branchesDetails
                {
                    id = item.First().BranchId,
                    arabicName = item.First().Branch.ArabicName,
                    latinName = item.First().Branch.LatinName,
                    totalRemind = _roundNumbers.GetRoundNumber(totalBranchSales_Invoices.Sum(x => x.PaymentType == (int)PaymentType.Delay ? x.TotalPrice : 0)),
                    totalPartial = _roundNumbers.GetRoundNumber(totalBranchSales_Invoices.Sum(x => x.PaymentType == (int)PaymentType.Partial ? x.TotalPrice : 0)),
                    totalPaid = _roundNumbers.GetRoundNumber(totalBranchSales_Invoices.Sum(x => x.PaymentType == (int)PaymentType.Complete ? x.TotalPrice : 0)),
                    totalDiscount = _roundNumbers.GetRoundNumber(totalBranchSales_Invoices.Sum(x => x.TotalDiscountValue)),
                    total = _roundNumbers.GetRoundNumber(totalBranchSales_Invoices.Sum(x => x.Net - x.TotalVat)),
                    vat = _roundNumbers.GetRoundNumber(totalBranchSales_Invoices.Sum(x => x.TotalVat)),
                    net = _roundNumbers.GetRoundNumber(totalBranchSales_Invoices.Sum(x => x.Net)),
                    eleType = 2,
                    GroupId = 1
                });
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
                var totalBranchReturnSales_Invoices = item.Where(x => x.InvoiceTypeId == (int)Enums.DocumentType.ReturnSales || x.InvoiceTypeId == (int)Enums.DocumentType.ReturnPOS);
                totalBranchReturnSales_branchesDetails.Add(new totalBranchTransaction_branchesDetails
                {
                    id = item.First().BranchId,
                    arabicName = item.First().Branch.ArabicName,
                    latinName = item.First().Branch.LatinName,
                    totalRemind = _roundNumbers.GetRoundNumber(totalBranchReturnSales_Invoices.Sum(x => x.PaymentType == (int)PaymentType.Delay ? x.TotalPrice : 0)) * -1,
                    totalPartial = _roundNumbers.GetRoundNumber(totalBranchReturnSales_Invoices.Sum(x => x.PaymentType == (int)PaymentType.Partial ? x.TotalPrice : 0)) * -1,
                    totalPaid = _roundNumbers.GetRoundNumber(totalBranchReturnSales_Invoices.Sum(x => x.PaymentType == (int)PaymentType.Complete ? x.TotalPrice : 0)) * -1,
                    totalDiscount = _roundNumbers.GetRoundNumber(totalBranchReturnSales_Invoices.Sum(x => x.TotalDiscountValue)) * -1,

                    total = _roundNumbers.GetRoundNumber(totalBranchReturnSales_Invoices.Sum(x => x.Net - x.TotalVat)) * -1,
                    vat = _roundNumbers.GetRoundNumber(totalBranchReturnSales_Invoices.Sum(x => x.TotalVat)) * -1,
                    net = _roundNumbers.GetRoundNumber(totalBranchReturnSales_Invoices.Sum(x => x.Net)) * -1,
                    className = defultData.text_danger,
                    eleType = 2,
                    GroupId = 1

                });
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
                var totalBranchPurchase_Invoices = item.Where(x => x.InvoiceTypeId == (int)Enums.DocumentType.Purchase);
                totalBranchPurchase_branchesDetails.Add(new totalBranchTransaction_branchesDetails
                {
                    id = item.First().BranchId,
                    arabicName = item.First().Branch.ArabicName,
                    latinName = item.First().Branch.LatinName,
                    totalRemind = _roundNumbers.GetRoundNumber(totalBranchPurchase_Invoices.Sum(x => x.PaymentType == (int)PaymentType.Delay ? x.TotalPrice : 0)),
                    totalPartial = _roundNumbers.GetRoundNumber(totalBranchPurchase_Invoices.Sum(x => x.PaymentType == (int)PaymentType.Partial ? x.TotalPrice : 0)),
                    totalPaid = _roundNumbers.GetRoundNumber(totalBranchPurchase_Invoices.Sum(x => x.PaymentType == (int)PaymentType.Complete ? x.TotalPrice : 0)),
                    totalDiscount = _roundNumbers.GetRoundNumber(totalBranchPurchase_Invoices.Sum(x => x.TotalDiscountValue)),

                    total = _roundNumbers.GetRoundNumber(totalBranchPurchase_Invoices.Sum(x => x.Net - x.TotalVat)),
                    vat = _roundNumbers.GetRoundNumber(totalBranchPurchase_Invoices.Sum(x => x.TotalVat)),
                    net = _roundNumbers.GetRoundNumber(totalBranchPurchase_Invoices.Sum(x => x.Net)),
                    eleType = 2,
                    GroupId = 2

                });
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
                var totalBranchReturnPurchase_Invoices = item.Where(x => x.InvoiceTypeId == (int)Enums.DocumentType.ReturnPurchase);
                totalBranchReturnPurchase_branchesDetails.Add(new totalBranchTransaction_branchesDetails
                {
                    id = item.First().BranchId,
                    arabicName = item.First().Branch.ArabicName,
                    latinName = item.First().Branch.LatinName,
                    totalRemind = _roundNumbers.GetRoundNumber(totalBranchReturnPurchase_Invoices.Sum(x => x.PaymentType == (int)PaymentType.Delay ? x.TotalPrice : 0)) * -1,
                    totalPartial = _roundNumbers.GetRoundNumber(totalBranchReturnPurchase_Invoices.Sum(x => x.PaymentType == (int)PaymentType.Partial ? x.TotalPrice : 0)) * -1,
                    totalPaid = _roundNumbers.GetRoundNumber(totalBranchReturnPurchase_Invoices.Sum(x => x.PaymentType == (int)PaymentType.Complete ? x.TotalPrice : 0)) * -1,
                    totalDiscount = _roundNumbers.GetRoundNumber(totalBranchReturnPurchase_Invoices.Sum(x => x.TotalDiscountValue)) * -1,

                    total = _roundNumbers.GetRoundNumber(totalBranchReturnPurchase_Invoices.Sum(x => x.Net - x.TotalVat)) * -1,
                    vat = _roundNumbers.GetRoundNumber(totalBranchReturnPurchase_Invoices.Sum(x => x.TotalVat)) * -1,
                    net = _roundNumbers.GetRoundNumber(totalBranchReturnPurchase_Invoices.Sum(x => x.Net)) * -1,
                    className = defultData.text_danger,
                    eleType = 2,
                    GroupId = 2

                });

            }
            var invoices = _invoices.Select(x => new totalBranchTransaction
            {
                id = x.InvoiceTypeId,
                arabicName = transactionTypes.Where(c => c.id == x.InvoiceTypeId)?.FirstOrDefault().arabicName ?? "",
                latinName = transactionTypes.Where(c => c.id == x.InvoiceTypeId)?.FirstOrDefault().latinName ?? "",

                totalRemind = x.PaymentType == (int)PaymentType.Delay ? x.TotalPrice : 0,
                totalPartial = x.PaymentType == (int)PaymentType.Partial ? x.TotalPrice : 0,
                totalPaid = x.PaymentType == (int)PaymentType.Complete ? x.TotalPrice : 0,
                totalDiscount = x.TotalDiscountValue,
                total = x.Net - x.TotalVat,
                vat = x.TotalVat,
                net = x.Net,
                paymentType = x.PaymentType
            }).ToHashSet();
            var salesInvoices = invoices.Where(x => x.id == (int)Enums.DocumentType.Sales || x.id == (int)Enums.DocumentType.POS);
            var totalBranchSales = new totalBranchTransaction()
            {
                id = (int)Enums.DocumentType.Sales,
                arabicName = "مبيعات",
                latinName = "Sales",

                totalRemind = _roundNumbers.GetRoundNumber(salesInvoices.Where(c => c.paymentType == (int)Enums.PaymentType.Delay).Sum(x => x.totalRemind)),
                totalPartial = _roundNumbers.GetRoundNumber(salesInvoices.Where(c => c.paymentType == (int)Enums.PaymentType.Partial).Sum(x => x.totalPartial)),
                totalPaid = _roundNumbers.GetRoundNumber(salesInvoices.Where(c => c.paymentType == (int)Enums.PaymentType.Complete).Sum(x => x.totalPaid)),
                totalDiscount = _roundNumbers.GetRoundNumber(salesInvoices.Sum(c => c.totalDiscount)),

                total = _roundNumbers.GetRoundNumber(salesInvoices.Sum(x => x.total)),

                vat = _roundNumbers.GetRoundNumber(salesInvoices.Sum(x => x.vat)),
                net = _roundNumbers.GetRoundNumber(salesInvoices.Sum(x => x.net)),
                branchesDetails = totalBranchSales_branchesDetails.OrderBy(c => c.id).ToList(),
                eleType = 1,
                GroupId = 1
            };
            //return sales 
            var ReturnSalesInvoices = invoices.Where(x => x.id == (int)Enums.DocumentType.ReturnSales || x.id == (int)Enums.DocumentType.ReturnPOS);
            var totalBranchReturnSales = new totalBranchTransaction()
            {
                id = (int)Enums.DocumentType.ReturnSales,
                arabicName = "مرتجع مبيعات",
                latinName = "Sales Return",

                totalRemind = _roundNumbers.GetRoundNumber(ReturnSalesInvoices.Where(c => c.paymentType == (int)Enums.PaymentType.Delay).Sum(x => x.totalRemind)) * -1,
                totalPartial = _roundNumbers.GetRoundNumber(ReturnSalesInvoices.Where(c => c.paymentType == (int)Enums.PaymentType.Partial).Sum(x => x.totalPartial)) * -1,
                totalPaid = _roundNumbers.GetRoundNumber(ReturnSalesInvoices.Where(c => c.paymentType == (int)Enums.PaymentType.Complete).Sum(x => x.totalPaid)) * -1,
                totalDiscount = _roundNumbers.GetRoundNumber(ReturnSalesInvoices.Sum(c => c.totalDiscount)) * -1,

                total = _roundNumbers.GetRoundNumber(ReturnSalesInvoices.Sum(x => x.total)) * -1,

                vat = _roundNumbers.GetRoundNumber(ReturnSalesInvoices.Sum(x => x.vat)) * -1,
                net = _roundNumbers.GetRoundNumber(ReturnSalesInvoices.Sum(x => x.net)) * -1,
                branchesDetails = totalBranchReturnSales_branchesDetails.OrderBy(c => c.id).ToList(),
                className = defultData.text_danger,
                eleType = 1,
                GroupId = 1


            };
            //Purchases
            var PurchaseInvoices = invoices.Where(x => x.id == (int)Enums.DocumentType.Purchase);
            var totalBranchPurchase = new totalBranchTransaction()
            {
                id = (int)Enums.DocumentType.Purchase,
                arabicName = "مشتريات",
                latinName = "Purchases",

                totalRemind = _roundNumbers.GetRoundNumber(PurchaseInvoices.Where(c => c.paymentType == (int)Enums.PaymentType.Delay).Sum(x => x.totalRemind)),
                totalPartial = _roundNumbers.GetRoundNumber(PurchaseInvoices.Where(c => c.paymentType == (int)Enums.PaymentType.Partial).Sum(x => x.totalPartial)),
                totalPaid = _roundNumbers.GetRoundNumber(PurchaseInvoices.Where(c => c.paymentType == (int)Enums.PaymentType.Complete).Sum(x => x.totalPaid)),
                totalDiscount = _roundNumbers.GetRoundNumber(PurchaseInvoices.Sum(c => c.totalDiscount)),

                total = _roundNumbers.GetRoundNumber(PurchaseInvoices.Sum(x => x.total)),

                vat = _roundNumbers.GetRoundNumber(PurchaseInvoices.Sum(x => x.vat)),
                net = _roundNumbers.GetRoundNumber(PurchaseInvoices.Sum(x => x.net)),
                branchesDetails = totalBranchPurchase_branchesDetails.OrderBy(c => c.id).ToList(),
                eleType = 1,
                GroupId = 2


            };
            //Return Purchases
            var ReturnPurchaseInvoices = invoices.Where(x => x.id == (int)Enums.DocumentType.ReturnPurchase);
            var totalBranchReturnPurchase = new totalBranchTransaction()
            {
                id = (int)Enums.DocumentType.ReturnPurchase,
                arabicName = "مرتجع مشتريات",
                latinName = "Purchases Return",

                totalRemind = _roundNumbers.GetRoundNumber(ReturnPurchaseInvoices.Where(c => c.paymentType == (int)Enums.PaymentType.Delay).Sum(x => x.totalRemind)) * -1,
                totalPartial = _roundNumbers.GetRoundNumber(ReturnPurchaseInvoices.Where(c => c.paymentType == (int)Enums.PaymentType.Partial).Sum(x => x.totalPartial)) * -1,
                totalPaid = _roundNumbers.GetRoundNumber(ReturnPurchaseInvoices.Where(c => c.paymentType == (int)Enums.PaymentType.Complete).Sum(x => x.totalPaid)) * -1,
                totalDiscount = _roundNumbers.GetRoundNumber(ReturnPurchaseInvoices.Sum(c => c.totalDiscount)) * -1,

                total = _roundNumbers.GetRoundNumber(ReturnPurchaseInvoices.Sum(x => x.total)) * -1,

                vat = _roundNumbers.GetRoundNumber(ReturnPurchaseInvoices.Sum(x => x.vat)) * -1,
                net = _roundNumbers.GetRoundNumber(ReturnPurchaseInvoices.Sum(x => x.net)) * -1,
                branchesDetails = totalBranchReturnPurchase_branchesDetails.OrderBy(c => c.id).ToList(),
                className = defultData.text_danger,
                eleType = 1,
                GroupId = 2


            };

            var listOfBranchTransaction_Sales = new List<totalBranchTransaction>();
            listOfBranchTransaction_Sales.Add(totalBranchSales);
            listOfBranchTransaction_Sales.Add(totalBranchReturnSales);

            var listOfBranchTransaction_Purchases = new List<totalBranchTransaction>();
            listOfBranchTransaction_Purchases.Add(totalBranchPurchase);
            listOfBranchTransaction_Purchases.Add(totalBranchReturnPurchase);
            #endregion
            #region Banks And Safes 
            var recs = _glRecieptsQuery.TableNoTracking
                                       .Include(x => x.person)
                                       .Where(x => branches.Contains(x.BranchId))
                                       .Where(x => !x.IsBlock)
                                       .Where(x => x.IsAccredit)
                                       .Where(x => !x.Deferre)
                                       .Where(x => parm.employeeId != 0 ? x.person.InvEmployeesId == parm.employeeId : true)
                                       .Where(x => x.RecieptDate.Date >= parm.dateFrom.Date && x.RecieptDate.Date <= parm.dateTo.Date)
                                       //.Where(x => userInfo.otherSettings.posShowOtherPersonsInv ? true : ((x.ParentTypeId == (int)Enums.DocumentType.POS || x.ParentTypeId == (int)Enums.DocumentType.ReturnPOS) ? x.UserId == userInfo.employeeId : true))
                                       //.Where(x => userInfo.otherSettings.salesShowOtherPersonsInv ? true : ((x.ParentTypeId == (int)Enums.DocumentType.Sales || x.ParentTypeId == (int)Enums.DocumentType.ReturnSales) ? x.UserId == userInfo.employeeId : true))
                                       //.Where(x => userInfo.otherSettings.purchasesShowOtherPersonsInv ? true : ((x.ParentTypeId == (int)Enums.DocumentType.Purchase || x.ParentTypeId == (int)Enums.DocumentType.ReturnPurchase) ? x.UserId == userInfo.employeeId : true))
                                       .ToHashSet()
                                       .Select(x => new totalSafesAndBanksTransaction
                                       {
                                           id = x.SafeID != null ? 1 : 2,
                                           arabicName = x.SafeID != null ? "حركة الخزائن" : "حركة البنوك",
                                           latinName = x.SafeID != null ? "Safes Transaction" : "Bank Transaction",
                                           expenses = x.Signal == -1 ? x.Amount : 0,
                                           payments = x.Signal == -1 ? 0 : x.Amount,
                                           branchId = x.BranchId
                                       })
                                       .ToHashSet()
                                       .GroupBy(c => c.id);

            var _banks = recs.Where(c => c.Where(x => x.id == 2).Any())
                .Select(x => new totalSafesAndBanksTransaction
                {
                    id = x.First().id,
                    arabicName = x.First().arabicName,
                    latinName = x.First().latinName,
                    expenses = _roundNumbers.GetRoundNumber(x.Sum(c => c.expenses)),
                    payments = _roundNumbers.GetRoundNumber(x.Sum(c => c.payments)),
                    GroupId = 1,
                    branchesDetails = x.GroupBy(c => c.branchId).ToList().Select(c => new totalSafesAndBanksTransaction_branchesDetails
                    {
                        id = c.First().branchId,
                        arabicName = db_Branches.First(x => x.Id == c.First().branchId).ArabicName,
                        latinName = db_Branches.First(x => x.Id == c.First().branchId).LatinName,
                        expenses = _roundNumbers.GetRoundNumber(c.Sum(f => f.expenses)),
                        payments = _roundNumbers.GetRoundNumber(c.Sum(f => f.payments)),
                        GroupId = 1
                    }).ToList()
                }).FirstOrDefault();
            var banks = new totalSafesAndBanksTransaction
            {
                id = 2,
                arabicName = "حركة البنوك",
                latinName = "Bank Transaction",
                branchId = _banks?.branchId ?? 0,
                payments = _banks?.payments ?? 0,
                expenses = _banks?.expenses ?? 0,
                Level = 1,
                GroupId = _banks?.GroupId ?? 0,
                branchesDetails = _banks?.branchesDetails ?? null,
                isExpanded = false
            };
            var _safes = recs.Where(c => c.Where(x => x.id == 1).Any())
                .Select(x => new totalSafesAndBanksTransaction
                {
                    id = x.First().id,
                    arabicName = x.First().arabicName,
                    latinName = x.First().latinName,
                    expenses = _roundNumbers.GetRoundNumber(x.Sum(c => c.expenses)),
                    payments = _roundNumbers.GetRoundNumber(x.Sum(c => c.payments)),
                    GroupId = 2,
                    branchesDetails = x.GroupBy(c => c.branchId).ToHashSet()
                    .Select(c => new totalSafesAndBanksTransaction_branchesDetails
                    {
                        id = c.First().branchId,
                        arabicName = db_Branches.First(x => x.Id == c.First().branchId).ArabicName,
                        latinName = db_Branches.First(x => x.Id == c.First().branchId).LatinName,
                        expenses = _roundNumbers.GetRoundNumber(c.Sum(f => f.expenses)),
                        payments = _roundNumbers.GetRoundNumber(c.Sum(f => f.payments)),
                        GroupId = 2,
                    }).ToList()
                }).FirstOrDefault();
            var safes = new totalSafesAndBanksTransaction
            {
                id = 1,
                arabicName = "حركة الخزائن",
                latinName = "Safes Transaction",
                branchId = _safes?.branchId ?? 0,
                payments = _safes?.payments ?? 0,
                expenses = _safes?.expenses ?? 0,
                Level = 1,
                GroupId = _safes?.GroupId ?? 0,
                branchesDetails = _safes?.branchesDetails ?? null,
                isExpanded = false
            };
            var _banksAndSafes = new List<totalSafesAndBanksTransaction>();
            _banksAndSafes.Add(safes);
            _banksAndSafes.Add(banks);

            #endregion
            var totalBranchTransactionResponseDTO_Sales = new totalBranchTransactionResponseDTO_Sales
            {
                totalBranchTransaction = listOfBranchTransaction_Sales,
                totalBranchTransaction_Totals = new totalBranchTransaction_Totals
                {
                    net = _roundNumbers.GetRoundNumber(listOfBranchTransaction_Sales.Sum(c => c.net)),
                    total = _roundNumbers.GetRoundNumber(listOfBranchTransaction_Sales.Sum(c => c.total)),
                    totalPaid = _roundNumbers.GetRoundNumber(listOfBranchTransaction_Sales.Sum(c => c.totalPaid)),
                    totalPartial = _roundNumbers.GetRoundNumber(listOfBranchTransaction_Sales.Sum(c => c.totalPartial)),
                    totalRemind = _roundNumbers.GetRoundNumber(listOfBranchTransaction_Sales.Sum(c => c.totalRemind)),
                    totalDiscount = _roundNumbers.GetRoundNumber(listOfBranchTransaction_Sales.Sum(c => c.totalDiscount)),
                    vat = _roundNumbers.GetRoundNumber(listOfBranchTransaction_Sales.Sum(c => c.vat)),
                    GroupId = 1
                },
            };
            var totalBranchTransactionResponseDTO_purchases = new totalBranchTransactionResponseDTO_purchases
            {
                totalBranchTransaction = listOfBranchTransaction_Purchases,
                totalBranchTransaction_Totals = new totalBranchTransaction_Totals
                {
                    net = _roundNumbers.GetRoundNumber(listOfBranchTransaction_Purchases.Sum(c => c.net)),
                    total = _roundNumbers.GetRoundNumber(listOfBranchTransaction_Purchases.Sum(c => c.total)),
                    totalPaid = _roundNumbers.GetRoundNumber(listOfBranchTransaction_Purchases.Sum(c => c.totalPaid)),
                    totalPartial = _roundNumbers.GetRoundNumber(listOfBranchTransaction_Purchases.Sum(c => c.totalPartial)),
                    totalRemind = _roundNumbers.GetRoundNumber(listOfBranchTransaction_Purchases.Sum(c => c.totalRemind)),
                    totalDiscount = _roundNumbers.GetRoundNumber(listOfBranchTransaction_Purchases.Sum(c => c.totalDiscount)),
                    vat = _roundNumbers.GetRoundNumber(listOfBranchTransaction_Purchases.Sum(c => c.vat)),
                    GroupId = 2

                },
            };
            var totalBranchTransactionResponse = new totalBranchTransactionResponseDTO()
            {
                totalBranchTransactionResponseDTO_Sales = totalBranchTransactionResponseDTO_Sales,
                totalBranchTransactionResponseDTO_purchases = totalBranchTransactionResponseDTO_purchases,
                banksAndSafes = _banksAndSafes,
                BanksSafes_Totals = new totalSafesAndBanksTransaction_Totals
                {
                    expenses = _roundNumbers.GetRoundNumber(banks.expenses + safes.expenses),
                    payments = _roundNumbers.GetRoundNumber(banks.payments + safes.payments)
                }
            };
            if (listOfBranchTransaction_Sales.Sum(c => c.net) == 0 && listOfBranchTransaction_Purchases.Sum(c => c.net) == 0 && (totalBranchTransactionResponse.BanksSafes_Totals.expenses + totalBranchTransactionResponse.BanksSafes_Totals.payments) == 0)
            {
                return null;
            }
            return new totalBranchTransactionResponse()
            {
                data = totalBranchTransactionResponse,
                Result = Result.Success
            };
        }
        public async Task<ResponseResult> totalBranchTransaction(totalBranchTransactionRequestDTO parm)
        {
            var res = await getTotalBranchTransaction(parm);
            if (res == null)
                return new ResponseResult
                {
                    Result = Result.NoDataFound
                };
            return new ResponseResult()
            {
                Result = res.Result,
                Note = res.notes,
                Data = res.data,
                TotalCount = res.TotalCount,
                DataCount = res.dataCount,
            };

        }
        public async Task<WebReport> TotalBranchTransactionReport(totalBranchTransactionRequestDTO request, exportType exportType, bool isArabic, string expandedTypeId, int fileId = 0)
        {

            var data = await getTotalBranchTransaction(request, true);
            var banksSafes = data.data.banksAndSafes.Where(a => a.Level == 1).ToList();


            if (expandedTypeId != null)
            {
                var expandedTypeIds = expandedTypeId.Split(',').Select(c => int.Parse(c)).ToArray();

                //Purchases
                var expandedPurchases = data.data.totalBranchTransactionResponseDTO_purchases.totalBranchTransaction.Where(a => expandedTypeIds.Contains(a.id)).ToList();
                if (expandedPurchases.Count > 0)
                {
                    foreach (var item in expandedPurchases)
                    {
                        var branches = new List<totalBranchTransaction>();

                        foreach (var branch in item.branchesDetails)
                        {
                            branches.Add(new totalBranchTransaction
                            {
                                id = branch.id,
                                arabicName = branch.arabicName,
                                latinName = branch.latinName,
                                totalRemind = branch.totalRemind,
                                totalPartial = branch.totalPartial,
                                totalPaid = branch.totalPaid,
                                totalDiscount = branch.totalDiscount,
                                total = branch.total,
                                vat = branch.vat,
                                net = branch.net,
                                paymentType = item.paymentType,
                                eleType = 2,
                                GroupId = 2


                            });
                        }
                        if (branches.Count > 0)
                        {
                            int index = data.data.totalBranchTransactionResponseDTO_purchases.totalBranchTransaction.IndexOf(item);
                            data.data.totalBranchTransactionResponseDTO_purchases.totalBranchTransaction.InsertRange(index + 1, branches);
                        }

                    }
                }

                //Sales

                var expandedSales = data.data.totalBranchTransactionResponseDTO_Sales.totalBranchTransaction.Where(a => expandedTypeIds.Contains(a.id)).ToList();
                if (expandedSales.Count > 0)
                {
                    foreach (var item in expandedSales)
                    {
                        var branches = new List<totalBranchTransaction>();

                        foreach (var branch in item.branchesDetails)
                        {
                            branches.Add(new totalBranchTransaction
                            {
                                id = branch.id,
                                arabicName = branch.arabicName,
                                latinName = branch.latinName,
                                totalRemind = branch.totalRemind,
                                totalPartial = branch.totalPartial,
                                totalPaid = branch.totalPaid,
                                totalDiscount = branch.totalDiscount,
                                total = branch.total,
                                vat = branch.vat,
                                net = branch.net,
                                paymentType = item.paymentType,
                                eleType = 2,
                                GroupId = 1



                            });
                        }
                        if (branches.Count > 0)
                        {
                            int index = data.data.totalBranchTransactionResponseDTO_Sales.totalBranchTransaction.IndexOf(item);
                            data.data.totalBranchTransactionResponseDTO_Sales.totalBranchTransaction.InsertRange(index + 1, branches);
                        }

                    }
                }
                //safes


                var expandedSafes = banksSafes.Where(a => expandedTypeIds.Contains(a.id)).ToList();

                if (expandedSafes.Count > 0)
                {
                    foreach (var item in expandedSafes)
                    {
                        var branches = new List<totalSafesAndBanksTransaction>();

                        foreach (var branch in item.branchesDetails)
                        {
                            branches.Add(new totalSafesAndBanksTransaction
                            {
                                id = item.id,
                                arabicName = branch.arabicName,
                                latinName = branch.latinName,
                                expenses = item.expenses,
                                payments = item.payments,
                                Level = 2,
                                GroupId = 2




                            });
                        }
                        if (branches.Count > 0)
                        {
                            int index = banksSafes.IndexOf(item);
                            banksSafes.InsertRange(index + 1, branches);
                        }

                    }
                }
                //banks
                var expandedBanks = data.data.banksAndSafes.Where(a => expandedTypeIds.Contains(a.id)).ToList();

                if (expandedBanks.Count > 0)
                {
                    foreach (var item in expandedBanks)
                    {
                        var branches = new List<totalSafesAndBanksTransaction>();

                        foreach (var branch in item.branchesDetails)
                        {
                            branches.Add(new totalSafesAndBanksTransaction
                            {
                                id = item.id,
                                arabicName = branch.arabicName,
                                latinName = branch.latinName,
                                expenses = item.expenses,
                                payments = item.payments,

                                Level = 2,
                                GroupId = 1


                            });
                        }
                        if (branches.Count > 0)
                        {
                            int index = data.data.banksAndSafes.IndexOf(item);
                            data.data.banksAndSafes.InsertRange(index + 1, branches);
                        }

                    }
                }
            }





            //var mainData = new totalBranchTransactionResponseDTO();

            var InvoicesList = new List<totalBranchTransaction>();
            var purchasesInvoices = data.data.totalBranchTransactionResponseDTO_purchases.totalBranchTransaction; ;

            var SalesInvoices = data.data.totalBranchTransactionResponseDTO_Sales.totalBranchTransaction; ;

            InvoicesList.AddRange(purchasesInvoices);
            InvoicesList.AddRange(SalesInvoices);
            //purchasesInvoices.AddRange(purchasesInvoices);
            var purchasesTotals = data.data.totalBranchTransactionResponseDTO_purchases.totalBranchTransaction_Totals;
            var salesTotals = data.data.totalBranchTransactionResponseDTO_Sales.totalBranchTransaction_Totals;

            var salesTotals2 = new List<object>()
            {
                purchasesTotals,salesTotals

            };

            //InvoicesList.Add(new totalBranchTransaction()
            //{
            //    totalRemind = purchasesTotals.totalRemind,
            //    totalPartial = purchasesTotals.totalPartial,

            //    totalPaid = purchasesTotals.totalPaid,

            //    totalDiscount = purchasesTotals.totalDiscount,

            //    total = purchasesTotals.total,
            //    vat = purchasesTotals.vat,
            //    net = purchasesTotals.net,
            //    GroupId = 22,
            //    invoiceTypeId = 5


            //});
            //InvoicesList.Add(new totalBranchTransaction()
            //{
            //    totalRemind = salesTotals.totalRemind,
            //    totalPartial = salesTotals.totalPartial,

            //    totalPaid = salesTotals.totalPaid,

            //    totalDiscount = salesTotals.totalDiscount,

            //    total = salesTotals.total,
            //    vat = salesTotals.vat,
            //    net = salesTotals.net,
            //    GroupId = 11,
            //    invoiceTypeId = 8


            //});

            var safesBanksList = new List<totalSafesAndBanksTransaction>();

            safesBanksList.AddRange(banksSafes);
            //safesBanksList.AddRange(expandedBanks);

            //var safes = data.data.banksAndSafes;
            //var banks=data.
            //var safesBcnksList = new List<totalSafesAndBanksTransaction>();

            var userInfo = await _iUserInformation.GetUserInformation();
            var employeeData = _employeeQuery.TableNoTracking.Where(e => request.employeeId > 0 ? e.Id == request.employeeId : false).FirstOrDefault();

            var dates = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, request.dateFrom, request.dateTo);
            var otherdata = new TotalInvoicesOtherData();
            var branchesList = request.branches.Split(',').Select(c => int.Parse(c)).ToArray();

            if (branchesList.Count() > 0)
            {
                if (branchesList[0] == 0)
                {
                    otherdata.BranchNameAr = "الكل";
                    otherdata.BranchNameEn = "all";
                }
                else
                {
                    var branchesName = _GLBranchQuery.TableNoTracking.Where(a => branchesList.Contains(a.Id)).ToList();

                    otherdata.BranchNameAr = string.Join(" , ", branchesName.Select(a => a.ArabicName));
                    otherdata.BranchNameEn = string.Join(" , ", branchesName.Select(a => a.LatinName));

                }
            }





            otherdata.ArabicName = request.employeeId > 0 ? employeeData.ArabicName : "الكل";
            otherdata.LatinName = request.employeeId > 0 ? employeeData.LatinName : "All";
            otherdata.EmployeeName = userInfo.employeeNameAr.ToString();
            otherdata.EmployeeNameEn = userInfo.employeeNameEn.ToString();
            otherdata.DateFrom = dates.DateFrom;
            otherdata.DateTo = dates.DateTo;
            otherdata.Date = dates.Date;



            var tablesNames = new TablesNames()
            {

                FirstListName = "InvoicesTotalsList",
                SecondListName = "SafesBanksList",
                ObjectName = "SafesBanksTotals"
            };
            ////tatal safes and banks
            //if (data.data.BanksSafes_Totals.expenses==-0)
            //{
            //    data.data.BanksSafes_Totals.expenses = 0;
            //}
            //if (data.data.BanksSafes_Totals.payments == -0)
            //{
            //    data.data.BanksSafes_Totals.payments = 0;
            //}
            //// safes and bank Lists
            //foreach(var item in safesBanksList)
            //{
            //    if (item.expenses == -0)
            //    {
            //        item.expenses = 0;
            //    }
            //    if (item.payments == -0)
            //    {
            //        item.payments = 0;
            //    }
            //}

            ////purchases sales 

            //foreach(var item in InvoicesList)
            //{
            //    if (item.totalRemind == -0)
            //    {
            //        item.totalRemind = 0;
            //    }

            //    if (item.totalPartial == -0)
            //    {
            //        item.totalPartial = 0;
            //    }
            //    if(item.totalPaid== -0)
            //    {
            //        item.totalPaid = 0;
            //    }
            //    if (item.totalDiscount == -0)
            //    {
            //        item.totalDiscount = 0;
            //    }
            //    if (item.totalDiscount == -0)
            //    {
            //        item.totalDiscount = 0;
            //    }
            //}

            var report = await _iGeneralPrint.PrintReport<totalSafesAndBanksTransaction_Totals, totalBranchTransaction, totalSafesAndBanksTransaction>(data.data.BanksSafes_Totals, InvoicesList, safesBanksList, tablesNames, otherdata
             , (int)SubFormsIds.totalBranchTransaction, exportType, isArabic, fileId, 0, false, salesTotals2);
            return report;
            return null;
        }

        public async Task<WebReport> SalesOfSalesManReport(SalesOfSalesManRequest request, exportType exportType, bool isArabic, int fileId = 0)
        {
            var data = await _mediator.Send(request);
            var mainData = (TotalsOfSalesManData)data.Data;
            if (mainData._SalesOfSalesManResponseList.Count() == 0)
            {
                mainData._SalesOfSalesManResponseList = new List<SalesOfSalesManResponse>()
               {
                   new SalesOfSalesManResponse()
               };
            }
            var salesManData = salesManQuery.TableNoTracking.Where(s => s.Id == request.SalesManID).FirstOrDefault();

            var userInfo = await _iUserInformation.GetUserInformation();
            var dates = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, request.dateFrom, request.dateTo);
            var otherdata = new AdditionalReportData()
            {
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),
                ArabicName = salesManData.ArabicName,
                LatinName = salesManData.LatinName,

                DateFrom = dates.DateFrom,
                DateTo = dates.DateTo,
                Date = dates.Date
            };

            var tablesNames = new TablesNames()
            {

                ObjectName = "SalesOfSalesMan",
                FirstListName = "SalesOfSalesManData"
            };




            var report = await _iGeneralPrint.PrintReport<TotalsOfSalesManData, SalesOfSalesManResponse, object>(mainData, mainData._SalesOfSalesManResponseList, null, tablesNames, otherdata
             , (int)SubFormsIds.SalesOfSalesMan, exportType, isArabic, fileId);
            return report;

        }
        public async Task<WebReport> ItemsProfitReport(ItemsProfitRequest request, exportType exportType, bool isArabic, int fileId = 0)
        {

            var data = await _mediator.Send(request);
            var mainData = (TotalsItemsProfitResponse)data.Data;
            if (mainData.Data.Count() == 0)
            {
                mainData.Data = new List<ItemsProfitResponse>()
               {
                   new ItemsProfitResponse(_roundNumbers)
               };
            }
            //var salesManData = salesManQuery.TableNoTracking.Where(s => s.Id == request.SalesManID).FirstOrDefault();

            var userInfo = await _iUserInformation.GetUserInformation();
            var dates = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, request.dateFrom, request.dateTo);

            var otherdata = new AdditionalReportData()
            {
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),
                DateFrom = dates.DateFrom,
                DateTo = dates.DateTo,
                Date = dates.Date
            };

            var tablesNames = new TablesNames()
            {

                ObjectName = "ItemsProfit",
                FirstListName = "ItemsProfitData"
            };




            var report = await _iGeneralPrint.PrintReport<TotalsItemsProfitResponse, ItemsProfitResponse, object>(mainData, mainData.Data, null, tablesNames, otherdata
             , (int)SubFormsIds.SalesProfitOfItems, exportType, isArabic, fileId);
            return report;

        }



        // Debt aging for cutomer or suplyer ( تحليل تقارير أعمار الديون لفواتير العملاء - الموردين )
        public async Task<ResponseResult> GetDebtAgingForCustomresOrSuplier(DebtAgingForCustomersOrSuppliersRequest request)
        {
            var res = await DebtAgingForCustomresOrSuplier(request);
            return new ResponseResult()
            {
                Data = res.Data,
                Result = res.Result,
                TotalCount = res.TotalCount,
                DataCount = res.DataCount,
                Note = res.Note
            };

        }

        public async Task<ResponseResult> DebtAgingForCustomresOrSuplier(DebtAgingForCustomersOrSuppliersRequest param)
        {

            int g = 0;
            var companyInfo = await _securityIntegrationService.getCompanyInformation();
            var invoices = ListOfInvoices(param).Where(c => c.Balance > 0).Where(x => param.DontshowInvoicesLessThanOne ? x.Balance > 1 : true);

            string cashKey = string.Empty;

            if (param.Department == (int)Enums.DocumentType.Sales)
            {
                cashKey = defultData.DebtAgingForCustomres_CashKey + "_" + companyInfo.companyLogin;
            }
            else if (param.Department == (int)Enums.DocumentType.Purchase)
            {
                cashKey = defultData.DebtAgingForSuplier_CashKey + "_" + companyInfo.companyLogin;
            }

            var invoicesByCustomer_Cashed = await _mediator.Send(new GetFromCacheRequest { key = cashKey });
            var invoicesByCustomer = new HashSet<TotalsValueOfInvoicesList>();
            if (invoicesByCustomer_Cashed != null)
                invoicesByCustomer = (HashSet<TotalsValueOfInvoicesList>)invoicesByCustomer_Cashed;

            if ((param.pageNumber == 1 || invoicesByCustomer_Cashed == null) && !param.isPrint)
            {
                invoicesByCustomer = invoices.GroupBy(x => new { x.PersonId, x.SalesManId })
                                            .Select(x => new TotalsValueOfInvoicesList
                                            {
                                                SalesmanId = x.First().SalesManId,
                                                SalesManNameAr = param.Department == (int)DocumentType.Sales ? salesManQuery.TableNoTracking.First(s => s.Id == (int)x.First().SalesManId).ArabicName : "",
                                                SalesManNameEn = param.Department == (int)DocumentType.Sales ? salesManQuery.TableNoTracking.First(s => s.Id == (int)x.First().SalesManId).LatinName : "",
                                                PersonID = x.First().PersonId,
                                                PersonNameAr = _personQuery.TableNoTracking.First(s => s.Id == (int)x.First().PersonId).ArabicName,
                                                PersonNameEn = _personQuery.TableNoTracking.First(s => s.Id == (int)x.First().PersonId).LatinName,
                                                Phone = _personQuery.TableNoTracking.First(s => s.Id == (int)x.First().PersonId).Phone,
                                                PersonCode = _personQuery.TableNoTracking.First(s => s.Id == (int)x.First().PersonId).Code,
                                                Total = _roundNumbers.GetDefultRoundNumber(x.Sum(t => t.Net)),
                                                TotalPaied = _roundNumbers.GetDefultRoundNumber(x.Sum(t => t.Paied)),
                                                TotalBalance = _roundNumbers.GetDefultRoundNumber(x.Sum(t => t.Balance)),
                                                groupId = ++g,
                                                InvoicesList = invoices.Where(y => (param.Department == (int)DocumentType.Sales ? y.SalesManId == x.First().SalesManId : 1 == 1)
                                                                                    && y.PersonId == x.First().PersonId).ToHashSet(),
                                                TotalReturnValue = _roundNumbers.GetDefultRoundNumber(x.Sum(t => t.returnValue))
                                            }).
                                            OrderBy(x => x.SalesmanId)
                                            .ToHashSet();
                _mediator.Send(new SetInCashRequest
                {
                    data = invoicesByCustomer,
                    expirationTime = TimeSpan.FromMinutes(10),
                    key = cashKey
                });
            }





            var response = new DebtAgingForCustomersOrSupliersResponse();


            response.TotalInvoicesPrice = _roundNumbers.GetDefultRoundNumber(invoicesByCustomer.Sum(x => x.Total));
            response.TotalPaied = _roundNumbers.GetDefultRoundNumber(invoicesByCustomer.Sum(x => x.TotalPaied));
            response.TotalBalance = _roundNumbers.GetDefultRoundNumber(invoicesByCustomer.Sum(a => a.TotalBalance));
            response.InvoicesList = param.isPrint ? invoicesByCustomer : invoicesByCustomer.Skip((param.pageNumber - 1) * param.pageSize).Take(param.pageSize).ToHashSet();
            response.TotalReturnValue = _roundNumbers.GetDefultRoundNumber(invoicesByCustomer.Sum(a => a.TotalReturnValue));
            if (param.pageSize * param.pageNumber >= invoicesByCustomer.Count())
            {
                return new ResponseResult()
                {
                    Data = response,
                    Result = Result.Success,
                    DataCount = response.InvoicesList.Count(),
                    TotalCount = invoices.Count(),
                    Note = "End of data"
                };
            }

            return new ResponseResult()
            {

                Data = response,
                Result = Result.Success,
                DataCount = response.InvoicesList.Count(),
                TotalCount = invoices.Count()
            };
        }

        private HashSet<InvoicesList> ListOfInvoices(DebtAgingForCustomersOrSuppliersRequest param)
        {

            int[] salesmen = { 0 };
            var branches = param.branches.Split(',').Select(c => int.Parse(c)).ToArray();
            var persons = param.persons.Split(',').Select(c => int.Parse(c)).ToArray();
            if (param.Department == (int)DocumentType.Sales)
                salesmen = param.salesmen.Split(',').Select(c => int.Parse(c)).ToArray();


            var AllInvoices = _invoiceMasterQuery.TableNoTracking;
            var _AllInvoices = AllInvoices
                .Where(x => x.Remain > 0)
                .Where(x => (param.persons != "0") ? persons.Contains(x.PersonId.Value) : true)
                .Where(x => param.Department == (int)DocumentType.Sales ? (param.salesmen != "0" ? (x.SalesManId != null ? salesmen.Contains(x.SalesManId.Value) : true) : true) : true)
                .Where(x => param.branches != "0" ? branches.Contains(x.BranchId) : true)
                .Where(x => x.InvoiceDate.Date <= param.dateTo.Date)
                .Where(x => !x.IsDeleted && x.InvoiceSubTypesId != (int)SubType.TotalReturn)
                .Where(x => param.DontshowInvoicesLessThanOne ? x.Remain > 1 : true);
            //.Where(x=>x.Net<1)


            var _invoices = _AllInvoices.Select(x => new InvoicesList
            {
                InvoiceCode = x.InvoiceType,
                Date = x.InvoiceDate.ToString("dd-MM-yyyy"),
                ageOfInvoiceByDays = (int)(DateTime.Today.AddDays(1) - x.InvoiceDate).TotalDays,
                Net = _roundNumbers.GetDefultRoundNumber(x.Net),
                Paied = _roundNumbers.GetDefultRoundNumber(x.Paid),
                returnValue = _roundNumbers.GetDefultRoundNumber(AllInvoices.Where(c => c.ParentInvoiceCode == x.InvoiceType && (c.InvoiceTypeId == (int)Enums.DocumentType.ReturnSales || c.InvoiceTypeId == (int)Enums.DocumentType.ReturnPurchase)).Sum(c => c.Remain)),
                Balance = _roundNumbers.GetDefultRoundNumber(x.Remain - AllInvoices.Where(c => c.ParentInvoiceCode == x.InvoiceType && (c.InvoiceTypeId == (int)Enums.DocumentType.ReturnSales || c.InvoiceTypeId == (int)Enums.DocumentType.ReturnPurchase)).Sum(c => c.Remain)),
                SalesManId = param.Department == (int)DocumentType.Sales ? (int)x.SalesManId : 0,
                PersonId = (int)x.PersonId,
                InvoiceId = x.InvoiceId,
                InvoiceTypeId = x.InvoiceTypeId,
                PageId = DocHelpers.GetPageIdOfDoc(x.InvoiceTypeId),
                PageEnum = DocHelpers.GetPageEnum(x.InvoiceTypeId),
                DocTypeId = -1,
                ParentInvoiceCode = x.ParentInvoiceCode


            });

            var invoices = _invoices.Where(x => x.InvoiceTypeId == param.Department).ToHashSet();



            #region commened
            //var invoices = 
            //                _invoiceMasterQuery
            //                .TableNoTracking
            //                .Where(x => x.Remain > 0
            //                && persons.Contains((int)x.PersonId)
            //                && salesmen.Contains((int)x.SalesManId)
            //                && branches.Contains((int)x.BranchId)
            //                && x.InvoiceDate <= param.dateTo.AddDays(1)
            //                && x.InvoiceTypeId == param.Department
            //                && !x.IsDeleted && x.InvoiceSubTypesId != 2)
            //                .Select(x => new InvoicesList
            //                {
            //                    InvoiceCode = x.InvoiceType,
            //                    Date = x.InvoiceDate.ToString("dd-MM-yyyy"),
            //                    ageOfInvoiceByDays = (int)(DateTime.Today.AddDays(1) - x.InvoiceDate).TotalDays,
            //                    Net = _roundNumbers.GetDefultRoundNumber(x.Net),
            //                    Paied = _roundNumbers.GetDefultRoundNumber(x.Paid),
            //                    Balance = _roundNumbers.GetDefultRoundNumber(x.Remain),
            //                    SalesManId = (int)x.SalesManId,
            //                    PersonId = (int)x.PersonId,

            //                }).ToHashSet(); //: param.Department == (int)DocumentType.Purchase ?
            //                _invoiceMasterQuery
            //                .TableNoTracking
            //                .Where(x => x.Remain > 0
            //                && persons.Contains((int)x.PersonId)
            //                && branches.Contains((int)x.BranchId)
            //                && x.InvoiceDate <= param.dateTo.AddDays(1)
            //                && x.InvoiceTypeId == param.Department
            //                && !x.IsDeleted && x.InvoiceSubTypesId != 2)
            //                .Select(x => new InvoicesList
            //                {
            //                    InvoiceCode = x.InvoiceType,
            //                    Date = x.InvoiceDate.ToString("dd-MM-yyyy"),
            //                    ageOfInvoiceByDays = (int)(DateTime.Today.AddDays(1) - x.InvoiceDate).TotalDays,
            //                    Net = _roundNumbers.GetDefultRoundNumber(x.Net),
            //                    Paied = _roundNumbers.GetDefultRoundNumber(x.Paid),
            //                    Balance = _roundNumbers.GetDefultRoundNumber(x.Remain),
            //                    PersonId = (int)x.PersonId,

            //                }).ToHashSet() : null ;

            #endregion
            return invoices;

        }

        public async Task<WebReport> DebtAgingForCustomresOrSuplierReport(DebtAgingForCustomersOrSuppliersRequest request, exportType exportType, bool isArabic, int fileId = 0)
        {
            request.isPrint = true;
            var data = await GetDebtAgingForCustomresOrSuplier(request);
            var mainData = (DebtAgingForCustomersOrSupliersResponse)data.Data;
            var CustomersTotalsList = mainData.InvoicesList.ToList();
            var InvoiceOCustomerList = new List<InvoicesList>();

            //Parallel.ForEach(CustomersTotalsList, types =>
            //{
            //    lock (InvoiceOCustomerList) InvoiceOCustomerList.AddRange(types.InvoicesList);
            //});



            //foreach (var item in CustomersTotalsList)
            //{

            //    InvoiceOCustomerList.AddRange(item.InvoicesList);
            //}
            int groupid = 1;
            foreach (var item in CustomersTotalsList)
            {
                item.groupId = groupid;
                foreach (var invoice in item.InvoicesList)
                {
                    invoice.groupId = groupid;
                }
                InvoiceOCustomerList.AddRange(item.InvoicesList);
                groupid++;
            }

            var userInfo = await _iUserInformation.GetUserInformation();
            int screenId = 0;
            if (request.Department == 8)
            {
                screenId = (int)SubFormsIds.DebtAgingForCustomers;
            }
            else
            {
                screenId = (int)SubFormsIds.DebtAgingForSupplier;
            }

            var otherdata = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, DateTime.Now, request.dateTo);



            otherdata.EmployeeName = userInfo.employeeNameAr.ToString();
            otherdata.EmployeeNameEn = userInfo.employeeNameEn.ToString();




            var tablesNames = new TablesNames()
            {

                ObjectName = "SalesManTotals",
                FirstListName = "CustomersTotals",
                SecondListName = "InvoicesOfCustomer"

            };




            var report = await _iGeneralPrint.PrintReport<DebtAgingForCustomersOrSupliersResponse, TotalsValueOfInvoicesList, InvoicesList>(mainData, CustomersTotalsList, InvoiceOCustomerList, tablesNames, otherdata
             , screenId, exportType, isArabic, fileId);
            return report;

        }

        public async Task<ResponseResult> GetDetailedInvoicesReport(DetialedInvoicesReportRequest parameter, bool isPrint = false)
        {
            UserInformationModel userInfo = await _iUserInformation.GetUserInformation();
            if (userInfo == null)
                return new ResponseResult() { Data = null, Result = Result.NoDataFound, ErrorMessageAr = "no taken request", ErrorMessageEn = "no taken request" };
            // var paymentMethodsIds = parameter.paymentMethod.Split(',').Select(x => int.Parse(x)).ToArray();
            var branches = parameter.branches.Split(',').Select(x => int.Parse(x)).ToArray();
            //var branchestotal =  _GLBranchQuery.TableNoTracking.Where(a => branches.Contains(a.Id)).ToList();
            //var testInvoices = await _invoiceMasterQuery.TableNoTracking
            //var hours = parameter.dateTo.AddHours(DateTime.Now.Hour);

            //     //.Where(x => x.EmployeeId == parameter.userId)
            //     .Where(x => branches[0] == 0 ? true : branches.Contains(x.BranchId)).ToListAsync();

            //var salesTypes = Lists.salesInvoicesList.Where(a => a != (int)DocumentType.DeleteSales);
            //var posTypes = Lists.POSInvoicesList.Where(a => a != (int)DocumentType.DeletePOS);

            //var parchasesTypes= Lists.purchasesInvoicesList.Where(a => a != (int)DocumentType.DeletePurchase);

            //var allSalesTypes = new List<int>();
            //allSalesTypes.AddRange(salesTypes);
            //allSalesTypes.AddRange(posTypes);

            //var allInvoicesTypes = new List<int>();
            //allInvoicesTypes.AddRange(salesTypes);
            //allInvoicesTypes.AddRange(posTypes);
            //allInvoicesTypes.AddRange(parchasesTypes);


            var data = await _invoiceMasterQuery.TableNoTracking
                .Include(c=> c.Person)
                 .Where(x => parameter.userId != 0 ? x.EmployeeId == parameter.userId : true)
                 .Where(x => branches[0] == 0 ? true : branches.Contains(x.BranchId))
                 .Where(x => x.InvoiceDate >= parameter.dateFrom && x.InvoiceDate <= parameter.dateTo.AddDays(1))
                 .Where(x => parameter.documentCode != null ? x.InvoiceType.Contains(parameter.documentCode) : true)
                 .Where(h => (parameter.invoiceTypeId != 0 ? (parameter.invoiceTypeId == (int)DocumentType.Sales ?
                 h.InvoiceTypeId == (int)DocumentType.Sales
                 || h.InvoiceTypeId == (int)DocumentType.POS
                 : (parameter.invoiceTypeId == (int)DocumentType.ReturnSales ?
                 h.InvoiceTypeId == (int)DocumentType.ReturnSales ||
                 h.InvoiceTypeId == (int)DocumentType.ReturnPOS : h.InvoiceTypeId == parameter.invoiceTypeId)) : (
                 h.InvoiceTypeId == (int)DocumentType.Sales ||
                 h.InvoiceTypeId == (int)DocumentType.ReturnSales ||
                 h.InvoiceTypeId == (int)DocumentType.POS ||
                 h.InvoiceTypeId == (int)DocumentType.ReturnPOS ||
                 h.InvoiceTypeId == (int)DocumentType.Purchase ||
                 h.InvoiceTypeId == (int)DocumentType.ReturnPurchase))
                 && h.IsDeleted == false).Include(a => a.Person).Include(a => a.Branch)
                 .Include(a => a.InvoicePaymentsMethods.Where(p => parameter.paymentMethod == 0 ? true : p.PaymentMethod.PaymentMethodId == parameter.paymentMethod))
                 .ThenInclude(h => h.PaymentMethod)
                 .ToListAsync();


            var invData = data.Select(h => new AccrediteInvoiceResponseData()
            {
                InvoiceDate = h.InvoiceDate,
                InvoiceType = h.InvoiceType,
                InvoiceTypeId = h.InvoiceTypeId,
                Net = h.Net,
                isCustomer = h.Person.IsCustomer,
                Paied = /*h.Net > h.VirualPaid ? h.VirualPaid :*/ h.InvoicePaymentsMethods.Sum(m => m.Value),

                // Paied = h.Net > h.Paid ? h.Paid : h.VirualPaid,
                ArabicName = h.Person != null ? h.Person.ArabicName : "",
                LatinName = h.Person != null ? h.Person.LatinName : "",
                transactionTypeAr = Aliases.InvoiceTransaction.PurchaseAr,
                transactionTypeEn = Aliases.InvoiceTransaction.PurchaseEn,
                InvoiceId = h.InvoiceId,
                PageId = DocHelpers.GetPageIdOfDoc(h.InvoiceTypeId),
                PageEnum = DocHelpers.GetPageEnum(h.InvoiceTypeId),
                DocTypeId = -1,
                paymentMethods = h.InvoicePaymentsMethods
                .Select(q => new { q.PaymentMethod, value = /*h.Net > h.VirualPaid*/ /*&& h.InvoicePaymentsMethods.Count == 1*/ /*?*/ q.Value /*: h.Paid*/, q.InvoiceId, q.PaymentMethodId })
                .Select(a => new PaymentMethodsDataResponse()
                {


                    arabicName = a.PaymentMethod.ArabicName,
                    latinName = a.PaymentMethod.LatinName,
                    Value = a.value,
                    InvoiceId = a.InvoiceId,
                    PaymentMethodId = a.PaymentMethodId
                })
                .ToList(),


            });
            List<AccrediteInvoiceResponseData> pagenatData = isPrint ? invData.ToList() : Pagenation<AccrediteInvoiceResponseData>.pagenationList(parameter.PageSize, parameter.PageNumber, invData.ToList());

            List<PaymentMethodsDataResponse> paymentList = new List<PaymentMethodsDataResponse>();
            foreach (var item in pagenatData)
            {
                var invTypes = Aliases.listOfInvoicesNames.listOfNames().Where(h => h.invoiceTypeId == item.InvoiceTypeId);
                if (item.InvoiceTypeId == (int)DocumentType.ReturnSales)
                    invTypes.Where(h => h.invoiceTypeId == (int)DocumentType.ReturnSales).FirstOrDefault().NameAr = "مرتجع فاتورة مبيعات";

                if (item.InvoiceTypeId == (int)DocumentType.Sales)

                    invTypes.Where(h => h.invoiceTypeId == (int)DocumentType.Sales).FirstOrDefault().NameAr = "فاتورة مبيعات";


                item.transactionTypeAr = invTypes.Select(a => a.NameAr).FirstOrDefault();
                item.transactionTypeEn = invTypes.Select(a => a.NameEn).FirstOrDefault();
                paymentList.AddRange(item.paymentMethods);
            }

            var paymentTypes = paymentList.GroupBy(h => h.arabicName).Select(h => h.First()).ToList();
            //foreach(var item in paymentTypes)
            //{
            //    item.InvoiceId = 0;
            //    item.Value = 0;
            //    item.Count = 0;

            //}

            return new ResponseResult() { Data = new TotalAccrediteInvoiceResponse() { paymentMethodType = paymentTypes, accrediteInvoiceResponseData = pagenatData }, TotalCount = data.Count, Result = Result.Success };




























            //UserInformationModel userInfo = await _iUserInformation.GetUserInformation();
            //if (userInfo == null)
            //    return new ResponseResult() { Data = null, Result = Result.NoDataFound, ErrorMessageAr = "no taken request", ErrorMessageEn = "no taken request" };
            //// var paymentMethodsIds = parameter.paymentMethod.Split(',').Select(x => int.Parse(x)).ToArray();
            //var branches = parameter.branches.Split(',').Select(x => int.Parse(x)).ToArray();
            //#region Comment
            ////var branchestotal =  _GLBranchQuery.TableNoTracking.Where(a => branches.Contains(a.Id)).ToList();
            ////var testInvoices = await _invoiceMasterQuery.TableNoTracking
            ////var hours = parameter.dateTo.AddHours(DateTime.Now.Hour);

            ////     //.Where(x => x.EmployeeId == parameter.userId)
            ////     .Where(x => branches[0] == 0 ? true : branches.Contains(x.BranchId)).ToListAsync();

            ////var salesTypes = Lists.salesInvoicesList.Where(a => a != (int)DocumentType.DeleteSales);
            ////var posTypes = Lists.POSInvoicesList.Where(a => a != (int)DocumentType.DeletePOS);

            ////var parchasesTypes= Lists.purchasesInvoicesList.Where(a => a != (int)DocumentType.DeletePurchase);

            ////var allSalesTypes = new List<int>();
            ////allSalesTypes.AddRange(salesTypes);
            ////allSalesTypes.AddRange(posTypes);

            ////var allInvoicesTypes = new List<int>();
            ////allInvoicesTypes.AddRange(salesTypes);
            ////allInvoicesTypes.AddRange(posTypes);
            ////allInvoicesTypes.AddRange(parchasesTypes); 
            //#endregion

            //#region Filtiring & Build Query & Get Data from DB
            ////var Sales = (int)DocumentType.Sales;        
            ////var POS = (int)DocumentType.POS;
            //int[] SalesOrPOS = new int[] { (int)DocumentType.Sales, (int)DocumentType.POS };
            ////var ReturnSales = (int)DocumentType.ReturnSales;
            ////var ReturnPOS = (int)DocumentType.ReturnPOS;
            //int[] ReturnPOSReturnSales = new int[] { (int)DocumentType.ReturnSales, (int)DocumentType.ReturnPOS };
            //var Purchase = (int)DocumentType.Purchase;
            //var ReturnPurchase = (int)DocumentType.ReturnPurchase;


            //var data = await _invoiceMasterQuery.TableNoTracking
            //     .Where(x => parameter.userId != 0 ? x.EmployeeId == parameter.userId : true)
            //     .Where(x => branches[0] == 0 ? true : branches.Contains(x.BranchId))
            //     .Where(x => x.InvoiceDate >= parameter.dateFrom && x.InvoiceDate <= parameter.dateTo.AddDays(1))
            //     .Where(x => parameter.documentCode != null ? x.InvoiceType.Contains(parameter.documentCode) : true)
            //     .Where(h => (parameter.invoiceTypeId != 0 ?

            //     (SalesOrPOS.Contains(h.InvoiceTypeId) ? SalesOrPOS.Contains(h.InvoiceTypeId)
            //     : (ReturnPOSReturnSales.Contains(h.InvoiceTypeId) ? ReturnPOSReturnSales.Contains(h.InvoiceTypeId)
            //     : h.InvoiceTypeId == parameter.invoiceTypeId))

            //     : (SalesOrPOS.Contains(h.InvoiceTypeId) ||
            //     ReturnPOSReturnSales.Contains(h.InvoiceTypeId) ||
            //      h.InvoiceTypeId == Purchase ||
            //      h.InvoiceTypeId == ReturnPurchase))
            //      && h.IsDeleted == false).Include(a => a.Person).Include(a => a.Branch).Include(a => a.InvoicePaymentsMethods
            //                              .Where(p => parameter.paymentMethod == 0 ? true : p.PaymentMethod.PaymentMethodId == parameter.paymentMethod))
            //                              .ThenInclude(h => h.PaymentMethod)
            //                              .ToListAsync();

            //#endregion

            //#region Response Of Data Will Be Return 
            //var invData = data.Select(h => new AccrediteInvoiceResponseData()
            //{
            //    InvoiceDate = h.InvoiceDate,
            //    InvoiceType = h.InvoiceType,
            //    InvoiceTypeId = h.InvoiceTypeId,
            //    Net = h.Net,
            //    Paied = /*h.Net > h.VirualPaid ? h.VirualPaid :*/ h.InvoicePaymentsMethods.Sum(m => m.Value),
            //    transactionTypeAr = Aliases.InvoiceTransaction.PurchaseAr,
            //    transactionTypeEn = Aliases.InvoiceTransaction.PurchaseEn,
            //    InvoiceId = h.InvoiceId,
            //    ArabicName = h.Person != null ? h.Person.ArabicName : "",
            //    LatinName = h.Person != null ? h.Person.LatinName : "",

            //    paymentMethods = h.InvoicePaymentsMethods
            //       .Select(q => new { q.PaymentMethod, value = /*h.Net > h.VirualPaid*/ /*&& h.InvoicePaymentsMethods.Count == 1*/ /*?*/ q.Value /*: h.Paid*/, q.InvoiceId, q.PaymentMethodId })
            //       .Select(a => new PaymentMethodsDataResponse()
            //       {
            //           arabicName = a.PaymentMethod.ArabicName,
            //           latinName = a.PaymentMethod.LatinName,
            //           Value = a.value,
            //           InvoiceId = a.InvoiceId,
            //           PaymentMethodId = a.PaymentMethodId
            //       })
            //       .ToList(),


            //});
            //#endregion

            //List<AccrediteInvoiceResponseData> pagenatData = isPrint ? invData.ToList() : Pagenation<AccrediteInvoiceResponseData>.pagenationList(parameter.PageSize, parameter.PageNumber, invData.ToList());

            //#region Payment Response "Dynamic"

            //List<PaymentMethodsDataResponse> paymentList = new List<PaymentMethodsDataResponse>();
            //foreach (var item in pagenatData)
            //{
            //    var invTypes = Aliases.listOfInvoicesNames.listOfNames().Where(h => h.invoiceTypeId == item.InvoiceTypeId);
            //    if (item.InvoiceTypeId == ReturnPOSReturnSales[0])
            //        invTypes.Where(h => h.invoiceTypeId == ReturnPOSReturnSales[0]).FirstOrDefault().NameAr = "مرتجع فاتورة مبيعات";

            //    if (item.InvoiceTypeId == SalesOrPOS[0])

            //        invTypes.Where(h => h.invoiceTypeId == SalesOrPOS[0]).FirstOrDefault().NameAr = "فاتورة مبيعات";


            //    item.transactionTypeAr = invTypes.Select(a => a.NameAr).FirstOrDefault();
            //    item.transactionTypeEn = invTypes.Select(a => a.NameEn).FirstOrDefault();
            //    paymentList.AddRange(item.paymentMethods);
            //}

            //var paymentTypes = paymentList.GroupBy(h => h.arabicName).Select(h => h.First()).ToList();
            //#region comment
            ////foreach(var item in paymentTypes)
            ////{
            ////    item.InvoiceId = 0;
            ////    item.Value = 0;
            ////    item.Count = 0;

            ////} 
            //#endregion

            //return new ResponseResult() { Data = new TotalAccrediteInvoiceResponse() { paymentMethodType = paymentTypes, accrediteInvoiceResponseData = pagenatData }, TotalCount = data.Count, Result = Result.Success };
        }


        public async Task<WebReport> DetailedInvoicesPrint(DetialedInvoicesReportRequest parameter, exportType exportType, bool isArabic, int fileId = 0)
        {
            var userInfo = new GetUserByIdModel();
            if (parameter.userId != 0)
            {
                userInfo = _iUserInformation.GetUserInformationById(parameter.userId);

            }

            var invoiceType = Aliases.listOfInvoicesNames.listOfNames().Where(h => h.invoiceTypeId == parameter.invoiceTypeId).FirstOrDefault();
            if (parameter.invoiceTypeId == (int)DocumentType.ReturnSales)
                invoiceType.NameAr = "مرتجع فاتورة مبيعات";

            if (parameter.invoiceTypeId == (int)DocumentType.Sales)

                invoiceType.NameAr = "فاتورة مبيعات";

            var branchesIds = parameter.branches.Split(',').Select(x => int.Parse(x)).ToArray();
            var branches = Enumerable.Empty<GLBranch>().AsQueryable();
            if (branchesIds[0] != 0)
            {
                branches = _GLBranchQuery.TableNoTracking.Where(a => branchesIds.Contains(a.Id));

            }

            var data = await GetDetailedInvoicesReport(parameter, true);
            var mainData = (TotalAccrediteInvoiceResponse)data.Data;

            var loginUser = await _iUserInformation.GetUserInformation();
            var tablesNames = new TablesNames()
            {

                FirstListName = "DetailesInvoices",
                SecondListName = "PaymentMethods"
            };
            WebReport report = new WebReport();
            var dates = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, parameter.dateFrom, parameter.dateTo);
            var paymentMethods = new List<PaymentMethodsForPrint>();
            if (mainData.accrediteInvoiceResponseData.Count > 0)
            {

                foreach (var item in mainData.accrediteInvoiceResponseData)
                {
                    if (item.paymentMethods.Count > 0)
                    {
                        paymentMethods.Add(new PaymentMethodsForPrint
                        {
                            TotalCash = item.paymentMethods.Where(a => a.PaymentMethodId == 1).Sum(a => a.Value),
                            TotalNet = item.paymentMethods.Where(a => a.PaymentMethodId == 2).Sum(a => a.Value),
                            Other = item.paymentMethods.Where(a => a.PaymentMethodId > 2).Sum(a => a.Value),
                            InvoiveId = item.InvoiceId

                        });
                        //item.totalOtherPaymentMethodsValues = item.paymentMethods.Where(a => a.PaymentMethodId > 2).Sum(a => a.Value);


                        //paymentMethods.AddRange(item.paymentMethods.OrderBy(a => a.PaymentMethodId));
                    }
                    else

                        paymentMethods.Add(new PaymentMethodsForPrint());

                }

            }
            AdditionalReportData otherData = new AdditionalReportData()
            {
                SalesTypeAr = parameter.invoiceTypeId == 0 ? "الكل" : invoiceType.NameAr,
                SalesTypeEn = parameter.invoiceTypeId == 0 ? "All" : invoiceType.NameEn,

                PaymentTypeEn = parameter.paymentMethod == 0 ? "All" : (parameter.paymentMethod == 1 ? "Paid" : (parameter.paymentMethod == 2 ? "Partial" : "Deferred")),
                PaymentTypeAr = parameter.paymentMethod == 0 ? "الكل" : (parameter.paymentMethod == 1 ? "مسدد" : (parameter.paymentMethod == 2 ? "جزئى" : "آجل")),

                ArabicName = parameter.userId == 0 ? "الكل" : userInfo.employeeNameAr.ToString(),
                LatinName = parameter.userId == 0 ? "All" : userInfo.employeeNameEn.ToString(),
                BranchNameAr = branchesIds[0] != 0 ? string.Join(" , ", branches.Select(a => a.ArabicName)) : "الكل",
                BranchNameEn = branchesIds[0] != 0 ? string.Join(" , ", branches.Select(a => a.LatinName)) : "All",
                Date = dates.Date,
                DateFrom = dates.DateFrom,
                DateTo = dates.DateTo,
                EmployeeName = loginUser.employeeNameAr.ToString(),
                EmployeeNameEn = loginUser.employeeNameEn.ToString(),
            };
            report = await _iGeneralPrint.PrintReport<object, AccrediteInvoiceResponseData, PaymentMethodsForPrint>(null, mainData.accrediteInvoiceResponseData, paymentMethods, tablesNames, otherData
            , (int)SubFormsIds.DetailedInvoices, exportType, isArabic, fileId);

            return report;

        }

        public class AdditionalReportData : ReportOtherData
        {
            public string SalesTypeAr { get; set; }
            public string SalesTypeEn { get; set; }

            public string PaymentTypeEn { get; set; }
            public string PaymentTypeAr { get; set; }

            // for detailed invoices report
            public string BranchNameAr { get; set; }
            public string BranchNameEn { get; set; }


        }

        public class TotalInvoicesOtherData : ReportOtherData
        {
            public string BranchNameAr { get; set; }
            public string BranchNameEn { get; set; }
        }


    }
}
