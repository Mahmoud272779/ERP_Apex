using App.Application.Helpers;
using App.Application.Helpers.Service_helper.Item_unit;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Request.Reports.Invoices.Purchases;
using App.Domain.Models.Security.Authentication.Response.Reports.Purchases;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Services.Reports.StoreReports.Sales.RPT_Sales;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Reports.Invoices.Purchases.Items_Purchases_For_Supplier
{
    public class ItemsPurchasesForSupplierService : IItemsPurchasesForSupplierService
    {
        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsQuery;
        private readonly IRepositoryQuery<InvStpUnits> UnitsQuery;
        private readonly IRoundNumbers _roundNumbers;
        public IitemUnitHelperServices itemUnitHelperServices;
        private readonly IRepositoryQuery<InvPersons> _personQuery;
        private readonly iUserInformation _iUserInformation;
        private readonly IGeneralPrint _iGeneralPrint;


        public ItemsPurchasesForSupplierService(IRepositoryQuery<InvoiceDetails> InvoiceDetailsQuery,
                                               IRepositoryQuery<InvStpUnits> UnitsQuery,
                                               IRoundNumbers roundNumbers,
                                               IitemUnitHelperServices itemUnitHelperServices,
                                               IRepositoryQuery<InvPersons> personQuery,
                                               iUserInformation iUserInformation,
                                               IGeneralPrint iGeneralPrint)
        {
            this.InvoiceDetailsQuery = InvoiceDetailsQuery;
            this.UnitsQuery = UnitsQuery;
            _roundNumbers = roundNumbers;
            this.itemUnitHelperServices = itemUnitHelperServices;
            _personQuery = personQuery;
            _iUserInformation = iUserInformation;
            _iGeneralPrint = iGeneralPrint;
        }

        public async Task<ResponseResult> GetItemsPurchasesForSupplierData(ItemsPurchasesForSupplierRequest request,bool isPrint=false)
        {
            var branches = request.Branches.Split(',').Select(c => int.Parse(c)).ToArray();
            var itemTypes = new List<int> { (int)ItemTypes.Note, (int)ItemTypes.Service };


            var resdata1 = InvoiceDetailsQuery
                .TableNoTracking
                .Include(a => a.InvoicesMaster)
                .Include(a => a.Items)
                .ThenInclude(a => a.Units)
                .ThenInclude(a => a.Unit)
                         .Where(a => a.InvoicesMaster.PersonId == request.personId
                          && (request.InvoiceTypeId > 0 ? a.InvoicesMaster.InvoiceTypeId == request.InvoiceTypeId : true)
                          && (request.Branches.Count() > 0 ? branches.Contains(a.InvoicesMaster.BranchId) : true)
                          && (a.InvoicesMaster.InvoiceDate.Date >= request.DateFrom.Date
                          && a.InvoicesMaster.InvoiceDate.Date <= request.DateTo.Date)
                          && a.InvoicesMaster.IsDeleted == false
                          && !itemTypes.Contains(a.ItemTypeId))
                         .ToList()
                         .OrderBy(a => a.ItemId)
                         .GroupBy(a => new { a.ItemId });

            var resDataList = new List<ItemsPurchasesResponseList>();
            foreach (var item in resdata1)
            {
                var ItemsPurchasesForSupplier = new ItemsPurchasesResponseList();
                var unitOfReport = await itemUnitHelperServices.getRptUnitData(item.First().ItemId);

                ItemsPurchasesForSupplier.ItemId     = item.First().ItemId;
                ItemsPurchasesForSupplier.itemCode   = item.Select(a => a.Items.ItemCode).First();
                ItemsPurchasesForSupplier.ItemNameAr = item.Select(a => a.Items.ArabicName).First();
                ItemsPurchasesForSupplier.ItemNameEn = item.Select(a => a.Items.LatinName).First();
                ItemsPurchasesForSupplier.UnitId     = unitOfReport.rptUnit;
                ItemsPurchasesForSupplier.rptUnitAR  = unitOfReport.rptUnitAR;
                ItemsPurchasesForSupplier.rptUnitEn  = unitOfReport.rptUnitEn; // item.Where(a => a.UnitId == rptUnitId).Select(a => a.Items.Units.Select(a=>a.Unit.LatinName)).ToString();
                ItemsPurchasesForSupplier.Quantity   = ReportData<InvoiceDetails>.Quantity(item, unitOfReport.rptFactor);
                ItemsPurchasesForSupplier.AvgPrice   = _roundNumbers.GetRoundNumber(ReportData<InvoiceDetails>.avgPrice(item, unitOfReport.rptFactor));
                ItemsPurchasesForSupplier.Total      = _roundNumbers.GetRoundNumber(ReportData<InvoiceDetails>.Total(item));
                ItemsPurchasesForSupplier.Discount   = _roundNumbers.GetRoundNumber(ReportData<InvoiceDetails>.DiscountForEachItem(item));
                ItemsPurchasesForSupplier.Net        = _roundNumbers.GetRoundNumber(ReportData<InvoiceDetails>.NetForEachItem(item));
                ItemsPurchasesForSupplier.Vat        = _roundNumbers.GetRoundNumber(ReportData<InvoiceDetails>.VatForEachItem(item));
                ItemsPurchasesForSupplier.NetWithVat = _roundNumbers.GetRoundNumber(ReportData<InvoiceDetails>.NetWithVatForEachItem(item));
                resDataList.Add(ItemsPurchasesForSupplier);
            }
            //pagenation
            var FinalResult = isPrint? resDataList: Pagenation<ItemsPurchasesResponseList>.pagenationList(request.PageSize, request.PageNumber, resDataList);

            var ResList = new ItemsPurchasesResponse();
            ResList.Details = FinalResult.ToList();
            ResList.SumQuantity = resDataList.Sum(a => a.Quantity);
            ResList.SumTotal = _roundNumbers.GetRoundNumber(resDataList.Sum(a => a.Total));
            ResList.SumDiscount = _roundNumbers.GetRoundNumber(resDataList.Sum(a => a.Discount));
            ResList.SumNet = _roundNumbers.GetRoundNumber(resDataList.Sum(a => a.Net));
            ResList.SumVat =  _roundNumbers.GetRoundNumber(_roundNumbers.GetRoundNumber(resDataList.Sum(a => a.Vat)));
            ResList.SumNetWithVat = _roundNumbers.GetRoundNumber(resDataList.Sum(a => a.NetWithVat));


            return new ResponseResult() { Id = null, Data = ResList, DataCount = resDataList.Count(), Result = resDataList.Count() > 0 ? Result.Success : Result.NoDataFound, Note = "" };


        }
        public async Task<WebReport> ItemsPurchasesForSupplierReport(ItemsPurchasesForSupplierRequest request, exportType exportType, bool isArabic,int fileId=0)
        {
            var data = await GetItemsPurchasesForSupplierData(request, true);
            var mainData = (ItemsPurchasesResponse)data.Data;
            var personData = _personQuery.TableNoTracking.Where(a => a.Id == request.personId).FirstOrDefault();
            //var itemData = _itemQuery.TableNoTracking.Where(a => a.Id == request.ItemId).FirstOrDefault();

            string salesTypeAr = "";
            string paymentTypeAr = "";
            string salesTypeEn = "";
            string paymentTypeEn = "";
            // Payment Types
            if ((int)request.PaymentTypeId == 0)
            {
                paymentTypeAr = "الكل";
                paymentTypeEn = "All";

            }
            else if ((int)request.PaymentTypeId == 1)
            {
                paymentTypeAr = "مسدد";
                paymentTypeEn = "Paid";

            }
            else if ((int)request.PaymentTypeId == 2)
            {
                paymentTypeAr = "جزئى";
                paymentTypeEn = "Partial";


            }
            else
            {
                paymentTypeAr = "اجل";
                paymentTypeEn = "Deferred";

            }

            //purchases Types
            if ((int)request.InvoiceTypeId == 0)
            {
                salesTypeAr = "الكل";
                salesTypeEn = "All";

            }
            else if ((int)request.InvoiceTypeId == 1)
            {
                salesTypeAr = "مشتريات";
                salesTypeEn = "Purchases";

            }
            else if ((int)request.InvoiceTypeId == 2)
            {
                salesTypeAr = "مرتجعات";
                salesTypeEn = "Returns";

            }

            var userInfo = await _iUserInformation.GetUserInformation();

            var dates = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, request.DateFrom, request.DateTo);

            var otherdata = new AdditionalReportData()
            {
                Id = request.personId,
                ArabicName = personData.ArabicName,
                LatinName = personData.LatinName,
                SalesTypeAr = salesTypeAr,
                SalesTypeEn = salesTypeEn,
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

                ObjectName = "ItemPurchasesForSupplier",
                FirstListName = "ItemPurchasesForSupplierList"
            };




            var report = await _iGeneralPrint.PrintReport<ItemsPurchasesResponse, ItemsPurchasesResponseList, object>(mainData, mainData.Details, null, tablesNames, otherdata
             , (int)SubFormsIds.supplierItemsPurchased_Purchases, exportType, isArabic, fileId);
            return report;


        }

    }
}
