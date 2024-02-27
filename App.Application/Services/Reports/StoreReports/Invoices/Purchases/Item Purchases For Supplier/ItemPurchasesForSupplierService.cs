using App.Application.Helpers;
using App.Application.Helpers.Service_helper.Item_unit;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Application.Services.Reports.Invoices.Purchases.Items_Purchases;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Request.Store.Reports.Purchases;
using App.Domain.Models.Security.Authentication.Request.Reports.Invoices.Purchases;
using App.Domain.Models.Security.Authentication.Response.Reports.Purchases;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Services.Reports.Items_Prices.Rpt_Store;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Reports.Invoices.Purchases.Item_Purchases_For_Supplier
{
    public class ItemPurchasesForSupplierService : IItemPurchasesForSupplierService
    {
        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsQuery;
        private readonly IRepositoryQuery<InvoiceMaster> _invoiceMasterQuery;
        private readonly IRepositoryQuery<InvPersons> _personQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> _itemQuery;
        private readonly iUserInformation _iUserInformation;
        private readonly IGeneralPrint _iGeneralPrint;

        private readonly IRoundNumbers _roundNumbers;

        // private readonly IRepositoryQuery<InvStpItemCardUnit> ItemCardUnitQuery;
        public IitemUnitHelperServices itemUnitHelperServices;


        public ItemPurchasesForSupplierService(IRepositoryQuery<InvoiceDetails> InvoiceDetailsQuery,
                                                IRoundNumbers roundNumbers,
                                                IitemUnitHelperServices itemUnitHelperServices,
                                                IRepositoryQuery<InvoiceMaster> invoiceMasterQuery,
                                                IRepositoryQuery<InvPersons> personQuery,
                                                IRepositoryQuery<InvStpItemCardMaster> itemQuery,
                                                iUserInformation iUserInformation,
                                                IGeneralPrint iGeneralPrint)
        {
            this.InvoiceDetailsQuery = InvoiceDetailsQuery;
            _roundNumbers = roundNumbers;
            this.itemUnitHelperServices = itemUnitHelperServices;
            _invoiceMasterQuery = invoiceMasterQuery;
            _personQuery = personQuery;
            _itemQuery = itemQuery;
            _iUserInformation = iUserInformation;
            _iGeneralPrint = iGeneralPrint;
        }

        public async Task<ResponseResult> GetItemPurchasesForSupplierData(ItemPurchasesForSupplierRequest request,bool isPrint=false)
        {
            var branches = request.Branches.Split(',').Select(c=> int.Parse(c)).ToArray();
            var unitOfReport = await itemUnitHelperServices.getRptUnitData(request.ItemId);



            var resdata = (InvoiceDetailsQuery.TableNoTracking.Include(a => a.InvoicesMaster).Where(a => (a.ItemId == request.ItemId) && a.InvoicesMaster.PersonId == request.personId
                                         && branches.Contains(a.InvoicesMaster.BranchId)
                                         && (a.InvoicesMaster.InvoiceDate.Date >= request.DateFrom.Date
                                         && a.InvoicesMaster.InvoiceDate.Date <= request.DateTo.Date)
                                         && a.InvoicesMaster.IsDeleted == false)
                                            )
                    .ToList()
                    .OrderBy(a => a.InvoicesMaster.InvoiceDate.Date)
                    .ThenBy(a => a.InvoicesMaster.Serialize)
                    .GroupBy(e => new { e.InvoiceId, e.ItemId })
                    .Select(x => new ItemPurchasesForSupplierResponseList()
                    {
                        InvoiceId = x.Key.InvoiceId,
                        ItemId = x.Key.ItemId,
                        InvoiceDate = x.First().InvoicesMaster.InvoiceDate,
                        InvoiceTypeId = x.First().InvoicesMaster.InvoiceTypeId,
                        TransactionAr = TransactionTypeList.transactionTypeModels().Where(a => a.id == x.First().InvoicesMaster.InvoiceTypeId).FirstOrDefault().arabicName,
                        TransactionEn = TransactionTypeList.transactionTypeModels().Where(a => a.id == x.First().InvoicesMaster.InvoiceTypeId).FirstOrDefault().latinName,
                        InvoiceType = x.First().InvoicesMaster.InvoiceType,
                        UnitId = x.First().UnitId,
                        rptUnitAR = unitOfReport.rptUnitAR,
                        rptUnitEn = unitOfReport.rptUnitEn,
                        Quantity = ReportData<InvoiceDetails>.Quantity(x, unitOfReport.rptFactor),
                        AvgPrice = _roundNumbers.GetRoundNumber(ReportData<InvoiceDetails>.avgPrice(x, unitOfReport.rptFactor)),
                        Total = _roundNumbers.GetRoundNumber(ReportData<InvoiceDetails>.Total(x)),
                        Discount = _roundNumbers.GetRoundNumber(ReportData<InvoiceDetails>.Discount(x)),
                        Net = _roundNumbers.GetRoundNumber(ReportData<InvoiceDetails>.Net(x)),
                        Vat = _roundNumbers.GetRoundNumber(ReportData<InvoiceDetails>.Vat(x)),
                        NetWithVat = _roundNumbers.GetRoundNumber(ReportData<InvoiceDetails>.NetWithVat(x))
                    }) ;

            var FinalResultData  = isPrint?resdata: Pagenation<ItemPurchasesForSupplierResponseList>.pagenationList(request.PageSize, request.PageNumber, resdata.ToList());

            var ResList = new ItemPurchasesForSupplierResponse();
            ResList.Details = resdata.ToList();
            ResList.SumQuantity =  _roundNumbers.GetRoundNumber(resdata.Sum(a => a.Quantity));
            ResList.SumTotal =  _roundNumbers.GetRoundNumber(resdata.Sum(a => a.Total));
            ResList.SumDiscount =  _roundNumbers.GetRoundNumber(resdata.Sum(a => a.Discount));
            ResList.SumNet =  _roundNumbers.GetRoundNumber(resdata.Sum(a => a.Net));
            ResList.SumAvgPrice = _roundNumbers.GetRoundNumber(resdata.Sum(a => a.AvgPrice));

            ResList.SumVat =  _roundNumbers.GetRoundNumber(resdata.Sum(a => a.Vat));
            ResList.SumNetWithVat =  _roundNumbers.GetRoundNumber(resdata.Sum(a => a.NetWithVat));
            

            return new ResponseResult() { Id = null, Data = ResList, DataCount = resdata.Count(), Result = resdata.Count() > 0 ? Result.Success : Result.NoDataFound, Note = "" };


        }
        public async Task<WebReport> ItemPurchasesForSupplierReport(ItemPurchasesForSupplierRequest request, exportType exportType, bool isArabic,int fileId=0)
        {
            var data = await GetItemPurchasesForSupplierData(request,true);
            var mainData = (ItemPurchasesForSupplierResponse)data.Data;
            var personData = _personQuery.TableNoTracking.Where(a => a.Id == request.personId).FirstOrDefault();
            var itemData = _itemQuery.TableNoTracking.Where(a => a.Id == request.ItemId).FirstOrDefault();
           
           
           
            var userInfo = await _iUserInformation.GetUserInformation();

            var dates = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, request.DateFrom, request.DateTo);

            var otherdata = new AdditionalReportDataStore()
            {
                Id = request.personId,
                ArabicName = personData.ArabicName,
                LatinName = personData.LatinName,
                ItemNameAr=itemData.ArabicName,
                ItemNameEn=itemData.LatinName,

                Code = request.ItemId.ToString(),
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),

                DateFrom = dates.DateFrom,
                DateTo = dates.DateTo,
                Date = dates.Date

            };
            foreach(var item in mainData.Details)
            {
                item.Date = item.InvoiceDate.ToString("yyyy/MM/dd");
            }
           
            var tablesNames = new TablesNames()
            {

                ObjectName = "ItemPurchasesForSupplier",
                FirstListName = "ItemPurchasesForSupplierList"
            };
            int screenId = 0;
            if (personData.IsSupplier)
            {
                screenId = (int)SubFormsIds.supplierItemsPurchased_Purchases;
            }
            else if (personData.IsCustomer)
            {

                screenId = (int)SubFormsIds.itemSalesForCustomer;
            }
            var report = await _iGeneralPrint.PrintReport<ItemPurchasesForSupplierResponse, ItemPurchasesForSupplierResponseList, object>(mainData, mainData.Details, null, tablesNames, otherdata
             , screenId, exportType, isArabic, fileId);
            return report;


        }
    }
}
