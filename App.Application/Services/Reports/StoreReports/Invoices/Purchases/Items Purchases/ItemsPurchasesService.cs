using App.Application.Helpers.ReportsHelper;
using App.Application.Helpers.Service_helper.Item_unit;
using App.Application.Services.Printing.InvoicePrint;
using App.Application.Services.Reports.StoreReports.Invoices.Purchases.Items_Purchases;
using App.Domain.Models;
using App.Domain.Models.Response.Store.Reports.Sales;
using App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Purchases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Reports.Invoices.Purchases.Items_Purchases
{
    public class ItemsPurchasesService : IItemsPurchasesService
    {
        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsQuery;
        private readonly IRepositoryQuery<InvStpUnits> UnitsQuery;
        private readonly IRepositoryQuery<InvPersons> _InvPersonsQuery;
        private readonly IRoundNumbers _roundNumbers;
        public IitemUnitHelperServices itemUnitHelperServices;

        private readonly IPrintService _iprintService;

        private readonly IFilesMangerService _filesMangerService;
        private readonly ICompanyDataService _CompanyDataService;
        private readonly iUserInformation _iUserInformation;
        private readonly IGeneralPrint _iGeneralPrint;
        

        public ItemsPurchasesService(IRepositoryQuery<InvoiceDetails> InvoiceDetailsQuery,
                                               IRepositoryQuery<InvStpUnits> UnitsQuery,
                                               IRoundNumbers roundNumbers,
                                               IitemUnitHelperServices itemUnitHelperServices,
                                               IFilesMangerService filesMangerService,
                                               ICompanyDataService CompanyDataService,
                                               iUserInformation iUserInformation,
                                               IPrintService iprintService,
                                               IGeneralPrint iGeneralPrint,
                                               IRepositoryQuery<InvPersons> invPersonsQuery)
        {
            this.InvoiceDetailsQuery = InvoiceDetailsQuery;
            this.UnitsQuery = UnitsQuery;
            _roundNumbers = roundNumbers;
            this.itemUnitHelperServices = itemUnitHelperServices;
            _iprintService = iprintService;
            _filesMangerService = filesMangerService;
            _CompanyDataService = CompanyDataService;
            _iUserInformation = iUserInformation;
            _iGeneralPrint = iGeneralPrint;
            _InvPersonsQuery = invPersonsQuery;
        }

        public async Task<ResponseResult> GetItemsPurchasesData(ItemsPurchasesRequest request,bool isPrint=false)
        {
            var branches = request.Branches.Split(',').Select(c => int.Parse(c)).ToArray();
            var itemTypes = new List<int> { (int)ItemTypes.Note, (int)ItemTypes.Service };
            var units = UnitsQuery.TableNoTracking.ToList();
            var listOfPurchasesInvoices = Lists.purchasesInvoicesList;
            if(request.supplierId != 0)
            {
                var supplier = await _InvPersonsQuery.GetByIdAsync(request.supplierId);
                if (supplier != null)
                    if (!supplier.IsSupplier)
                        return new ResponseResult
                        {
                            Result = Result.Failed,
                            Note = "You have to input supplier Id " 
                        };
            }
            var userInfo = await _iUserInformation.GetUserInformation();

            var _invoices = InvoiceDetailsQuery.TableNoTracking
                                                             .Include(x => x.InvoicesMaster)
                                                             .Include(x => x.InvoicesMaster.Person)
                                                             .Include(x => x.InvoicesMaster.Person.InvEmployees)
                                                             .Include(x => x.Items)
                                                             .Include(x => x.Items.Category)
                                                             .Include(x => x.Items.Units);

            var invoices = await itemsTransaction.ItemsData(
                userInfo: userInfo ,
                _roundNumbers:_roundNumbers, 
                units: units,
                invoices:_invoices, 
                branches: branches,
                showDeleted:false,
                itemId:request.itemId,
                invoicesTypes:listOfPurchasesInvoices.ToArray(),
                dateFrom:request.DateFrom,
                dateTo:request.DateTo,
                catId:request.categoryId,
                storeId:request.storeId??0,
                personId:request.supplierId??0,
                employeeId:0,
                paymentType: PaymentType.all,
                salesSignal:1,
                true);




            var FinalResult = isPrint? invoices : Pagenation<itemsSalesData>.pagenationList(request.PageSize, request.PageNumber, invoices);
            double MaxPageNumber = invoices.ToList().Count() / Convert.ToDouble(request.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            var ResList = new itemsSalesReponseDTO();
            ResList.data = FinalResult.ToList();

            ResList.TotalQyt = _roundNumbers.GetRoundNumber(invoices.Sum(a => a.qyt));
            ResList.TotalAavgOfPrice = _roundNumbers.GetRoundNumber(invoices.Sum(a => a.avgOfPrice));
            ResList.TotalPrice = _roundNumbers.GetRoundNumber(invoices.Sum(a => a.totalPrice));
            ResList.TotalDiscount = _roundNumbers.GetRoundNumber(invoices.Sum(a => a.discount));
            ResList.TotalNet = _roundNumbers.GetRoundNumber(invoices.Sum(a => a.net));



            return new ResponseResult() 
            {
                Id = null, 
                Data = ResList,
                DataCount = FinalResult.Count(), 
                Result = invoices.Count() > 0 ? Result.Success : Result.NoDataFound, 
                Note = (countofFilter == request.PageNumber ? Actions.EndOfData : ""),
                TotalCount = invoices.Count()
            };


        }

        public async Task<WebReport> ItemsPurchasesDataReport(ItemsPurchasesRequest request, exportType exportType, bool isArabic,int fileId=0)
        {
            //if (request.categoryId == 0)
            //{
            //    request.categoryId = null;
            //}
            if (request.itemId == null)
            {
                request.itemId = 0;
            }

            var data = await GetItemsPurchasesData(request, true);
            var companydata = await _CompanyDataService.GetCompanyData(true);
            var userInfo = await _iUserInformation.GetUserInformation();

            //itemsSalesReponseDTO itemsMainData = new itemsSalesReponseDTO();
            var itemsMainData = (itemsSalesReponseDTO) data.Data;

            int screenId = 0;
             
            var otherdata = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, request.DateFrom, request.DateTo);


            otherdata.EmployeeName = userInfo.employeeNameAr.ToString();

            otherdata.EmployeeNameEn = userInfo.employeeNameEn.ToString();


                
            

            if (request.supplierId != 0)
            {
                var personData = _InvPersonsQuery.TableNoTracking.Where(p => p.Id == request.supplierId).FirstOrDefault();

                screenId = (int)SubFormsIds.SupplierItemsPurchases_Purchases;
                otherdata.LatinName = personData.LatinName;
                otherdata.ArabicName = personData.ArabicName;
            }
            else
            {
                screenId = (int)SubFormsIds.ItemsPurchases_Purchases;
            }


            var tablesNames = new TablesNames()
            {

                ObjectName = "itemsSalesReponse",
                FirstListName = "itemsSalesData"
            };

            var report = await _iGeneralPrint.PrintReport<itemsSalesReponseDTO, itemsSalesData, object>(itemsMainData, itemsMainData.data, null, tablesNames, otherdata
             , screenId, exportType, isArabic, fileId);
            return report;

            
        }
    }
}
