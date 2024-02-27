using App.Application.Helpers.ReportsHelper;
using App.Application.Helpers.Service_helper.Item_unit;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Stores;
using App.Domain.Models.Security.Authentication.Response.Store.Reports.Stores;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Reports.StoreReports.Invoices.Stores.Total_transactions_of_items
{
   public class totalTransactionsOfItemsServices : ItotalTransactionsOfItemsServices
    {
        readonly private IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery;
        public IitemUnitHelperServices itemUnitHelperServices;
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<InvStpStores> _storeQuery;
        private readonly IGeneralPrint _iGeneralPrint;

        public totalTransactionsOfItemsServices(IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery,
            IitemUnitHelperServices itemUnitHelperServices,
            iUserInformation iUserInformation,
            IRepositoryQuery<InvStpStores> storeQuery,
            IGeneralPrint iGeneralPrint)
        {
            this.invoiceDetailsQuery = invoiceDetailsQuery;
            this.itemUnitHelperServices = itemUnitHelperServices;
            _iUserInformation = iUserInformation;
            _storeQuery = storeQuery;
            _iGeneralPrint = iGeneralPrint;
        }

        public async Task<ResponseResult> getTotalTransactionsOfItems(totalTransactionsOfItemsRequest request, bool isPrint=false)
        {
            if (string.IsNullOrEmpty(request.storeId.ToString()))
                return new ResponseResult()
                {
                    Note = "Store Id is Required",
                    Result = Result.Failed
                };
            if (string.IsNullOrEmpty(request.pageNumber.ToString()) || string.IsNullOrEmpty(request.pageNumber.ToString()))
                return new ResponseResult()
                {
                    Note = "Page number and Page size are required",
                    Result = Result.Failed,
                };
            //var data = invoiceDetailsQuery.TableNoTracking.Include(a => a.InvoicesMaster).Include(a => a.Items)
            //                                                            .ThenInclude(a => a.Units).ThenInclude(a => a.Unit)
            //                       .Where(a => a.InvoicesMaster.StoreId == request.storeId &&
            //                                 (request.itemId > 0 ? a.ItemId == request.itemId : true)
            //                       ).ToList().GroupBy(a => new { a.ItemId });
            var resData = invoiceDetailsQuery.TableNoTracking.Include(a => a.InvoicesMaster).Include(a => a.Items)
                                                                       .ThenInclude(a => a.Units).ThenInclude(a => a.Unit)
                                  .Where(a =>  a.InvoicesMaster.StoreId == request.storeId &&
                                            (request.itemId > 0 ? a.ItemId == request.itemId : true)
                                  ).ToList().GroupBy(a => new { a.ItemId })
                                  .Select( a=> new totalTransactionsOfItemsResponseList()
                                  {
                                      itemId = a.Key.ItemId,
                                      itemCode = a.First().Items!=null? a.First().Items.ItemCode:"",
                                      itemName = a.First().Items != null ? a.First().Items.ArabicName:"",
                                      itemNameEn = a.First().Items != null ? a.First().Items.LatinName:"",

                                      unitName = a.First().Units != null ? a.First().Units.ArabicName:"",
                                      unitNameEn = a.First().Units != null ? a.First().Units.LatinName:"",

                                      previous = a.Where(a => (a.InvoicesMaster.InvoiceDate.Date < request.dateFrom.Date))
                                                     .Sum(a => a.Quantity * a.ConversionFactor * a.Signal),
                                      inComingQuantity = a.Where(a => a.Signal > 0 && (a.InvoicesMaster.InvoiceDate.Date >= request.dateFrom.Date
                                            && a.InvoicesMaster.InvoiceDate.Date <= request.dateTo.Date)).Sum(a => a.Quantity * a.ConversionFactor),
                                      outComingQuantity = a.Where(a => a.Signal < 0 && (a.InvoicesMaster.InvoiceDate.Date >= request.dateFrom.Date
                                           && a.InvoicesMaster.InvoiceDate.Date <= request.dateTo.Date)).Sum(a => a.Quantity * a.ConversionFactor),
                                      balance =0
                                  });
            resData =isPrint? resData:resData.Skip((request.pageNumber - 1) * request.pageSize).Take(request.pageSize);

            var totalTransOfItemsList = new List<totalTransactionsOfItemsResponseList>();
            foreach (var item in resData)
            {
                var unitOfReport = await itemUnitHelperServices.getRptUnitData(item.itemId);
                var totalTransOfItems = new totalTransactionsOfItemsResponseList();
                totalTransOfItems.itemId = item.itemId;
                totalTransOfItems.itemCode = item.itemCode;
                totalTransOfItems.itemName = item.itemName;
                totalTransOfItems.itemNameEn = item.itemNameEn;
                totalTransOfItems.unitName = unitOfReport.rptUnitAR;
                totalTransOfItems.unitNameEn = unitOfReport.rptUnitEn;

                totalTransOfItems.previous = item.previous / unitOfReport.rptFactor;
                totalTransOfItems.inComingQuantity = item.inComingQuantity / unitOfReport.rptFactor;
                totalTransOfItems.outComingQuantity = item.outComingQuantity / unitOfReport.rptFactor;
                totalTransOfItems.balance = totalTransOfItems.previous+ totalTransOfItems.inComingQuantity - totalTransOfItems.outComingQuantity;

                totalTransOfItemsList.Add(totalTransOfItems);
            }
            var totalTransactionsOfItems = new totalTransactionsOfItemsResponse();

            totalTransactionsOfItems.detailsOfTransactions = totalTransOfItemsList;
            totalTransactionsOfItems.totalPrevious = totalTransOfItemsList.Sum(a => a.previous);
            totalTransactionsOfItems.totalInComingQuantity = totalTransOfItemsList.Sum(a => a.inComingQuantity);
            totalTransactionsOfItems.totalOutComingQuantity = totalTransOfItemsList.Sum(a => a.outComingQuantity);
            totalTransactionsOfItems.totalBalance = totalTransOfItemsList.Sum(a => a.balance);

            return new ResponseResult { Data = totalTransactionsOfItems, DataCount = totalTransOfItemsList.Count(), Result = totalTransOfItemsList.Count() > 0 ? Result.Success : Result.NoDataFound };

        }
        public async Task<WebReport> TotalTransactionsOfItemReport(totalTransactionsOfItemsRequest request,exportType exportType,bool isArabic, int fileId = 0)
        {
            var data = await getTotalTransactionsOfItems(request,true);
            
            var mainData = (totalTransactionsOfItemsResponse)data.Data;

            var userInfo = await _iUserInformation.GetUserInformation();
            var storeData = _storeQuery.TableNoTracking.Where(a => a.Id == request.storeId).FirstOrDefault();


            var otherdata = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, request.dateFrom, request.dateTo);


            otherdata.ArabicName = storeData.ArabicName;
            otherdata.LatinName = storeData.LatinName;
            otherdata.EmployeeName = userInfo.employeeNameAr.ToString();
            otherdata.EmployeeNameEn = userInfo.employeeNameEn.ToString();




            var tablesNames = new TablesNames()
            {
                ObjectName= "TotalTransactionsOfItem",
                FirstListName = "TotalTransactionsOfItemlist",

            };
            int screenId = 0;
            if(request.itemId ==0)
            {
                screenId = (int)SubFormsIds.getTotalTransactionsOfItems_Repository;
            }
            else
                screenId = (int)SubFormsIds.itemsTransaction;

            


        var report = await _iGeneralPrint.PrintReport<totalTransactionsOfItemsResponse, totalTransactionsOfItemsResponseList, object>(mainData, mainData.detailsOfTransactions, null, tablesNames, otherdata
             , screenId, exportType, isArabic,fileId);
            return report;
        }
    }
}
