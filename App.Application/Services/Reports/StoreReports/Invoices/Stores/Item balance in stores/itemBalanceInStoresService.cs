using App.Application.Handlers.Reports.Store.Store.ItemsBalanceInStores;
using App.Application.Helpers;
using App.Application.Helpers.Service_helper.Item_unit;
using App.Application.Services.Printing.InvoicePrint;
using App.Application.Services.Process.Company_data;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Application.Services.Process.StoreServices.Invoices.AccrediteInvoice;
using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Enums;
using App.Domain.Models.Request;
using App.Domain.Models.Request.Store.Reports.Store;
using App.Domain.Models.Response.Store.Reports;
using App.Domain.Models.Response.Store.Reports.MainData;
using App.Domain.Models.Response.Store.Reports.Sales;
using App.Domain.Models.Response.Store.Reports.Store;
using App.Domain.Models.Security.Authentication.Response.Store.Reports.Stores;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using DocumentFormat.OpenXml.Office2010.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using FastReport.Web;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Application.Services.Reports.StoreReports.RPT_BanksSafesServices.safeOrBankReportService;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Reports.StoreReports.Invoices.Stores.Item_balance_in_stores
{
    public class itemBalanceInStoresService : IitemBalanceInStoresService
    {
        readonly private IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery;
        private readonly IRepositoryQuery<InvStpUnits> _invStpUnitsQuery;
        private readonly IRepositoryQuery<InvStpItemCardUnit> _InvStpItemCardUnitQuery;
        private readonly IRoundNumbers _roundNumbers;
        public IitemUnitHelperServices itemUnitHelperServices;
        private readonly IPrintService _iprintService;

        private readonly IFilesMangerService _filesMangerService;
        private readonly ICompanyDataService _CompanyDataService;
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<InvStpStores> _storeQuery;
        private readonly IGeneralPrint _iGeneralPrint;
        private readonly IRepositoryQuery<InvStpItemCardMaster> _invStpItemCardMasterQuery;
        private readonly IMediator _mediator;

        public itemBalanceInStoresService(IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery,
                                            IRepositoryQuery<InvStpUnits> InvStpUnitsQuery,
                                            IRoundNumbers roundNumbers,
                                            IitemUnitHelperServices itemUnitHelperServices,
                                            IFilesMangerService filesMangerService,
                                            ICompanyDataService CompanyDataService,
                                            iUserInformation iUserInformation,
                                            IPrintService iprintService,
                                            IRepositoryQuery<InvStpStores> storeQuery,
                                            IGeneralPrint iGeneralPrint,
                                            IRepositoryQuery<InvStpItemCardMaster> invStpItemCardMasterQuery,
                                            IRepositoryQuery<InvStpItemCardUnit> invStpItemCardUnitQuery,
                                            IMediator mediator)
        {
            this.invoiceDetailsQuery = invoiceDetailsQuery;
            _invStpUnitsQuery = InvStpUnitsQuery;
            _roundNumbers = roundNumbers;
            this.itemUnitHelperServices = itemUnitHelperServices;
            _iprintService = iprintService;
            _filesMangerService = filesMangerService;
            _CompanyDataService = CompanyDataService;
            _iUserInformation = iUserInformation;
            _storeQuery = storeQuery;
            _iGeneralPrint = iGeneralPrint;
            _invStpItemCardMasterQuery = invStpItemCardMasterQuery;
            _InvStpItemCardUnitQuery = invStpItemCardUnitQuery;
            _mediator = mediator;
        }

        public async Task<ResponseResult> getItemBalanceInStores(int itemId)
        {

            var unitOfReport = await itemUnitHelperServices.getRptUnitData(itemId);

            var resData = invoiceDetailsQuery.TableNoTracking.Include(a => a.Units).Include(a => a.InvoicesMaster.store)
                .Where(a => a.ItemId == itemId).ToList().OrderBy(a => a.InvoicesMaster.StoreId)
                .GroupBy(a => a.InvoicesMaster.StoreId).Select(a => new itemBalanceInStoresResponse()
                {
                    storeId = a.First().InvoicesMaster.StoreId,
                    storeName = a.First().InvoicesMaster.store.ArabicName,
                    storeNameEn=a.First().InvoicesMaster.store.LatinName,
                    unitName = unitOfReport.rptUnitAR,
                    unitNameEn=unitOfReport.rptUnitEn,
                    balance = a.Sum(a => (a.Quantity * a.ConversionFactor * a.Signal) / unitOfReport.rptFactor)
                });

            return new ResponseResult { Data = resData, DataCount = resData.Count(), Result = resData.Count() > 0 ? Result.Success : Result.NoDataFound };

        }
        public async Task<WebReport> ItemBalanceInStoresReport(int itemId,exportType exportType,bool isArabic, int fileId = 0)
        {
            var data = await getItemBalanceInStores(itemId);
            var mainData = (IEnumerable<itemBalanceInStoresResponse>)data.Data;

            var userInfo = await _iUserInformation.GetUserInformation();
            var itemData = _invStpItemCardMasterQuery.TableNoTracking.Where(a => a.Id == itemId).FirstOrDefault();


            

           var otherdata = new ReportOtherData()
            {
                Id=itemId,
                ArabicName = itemData.ArabicName,
                LatinName = itemData.LatinName,
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),

                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")

            };

            var tablesNames = new TablesNames()
            {

                FirstListName = "ItemBalanceInStores",
               
            };




            var report = await _iGeneralPrint.PrintReport<object, itemBalanceInStoresResponse, object>(null,mainData.ToList(), null, tablesNames, otherdata
             , (int)SubFormsIds.getItemBalanceInStores_Repository, exportType, isArabic,fileId);
            return report;
        }


        public async Task<itemBalanceInStoreResponse> getItemsBalanceInStores(itemsBalanacesInStoreRequestDTO parm, bool isPrint = false)
        {
            itemBalanceInStoreResponse res = await _mediator.Send(new ItemsBalanceInStoresRequest
            {
                storeId = parm.storeId,
                PageSize = parm.PageSize,
                itemTypes = parm.itemTypes,
                PageNumber = parm.PageNumber,
                catId = parm.catId,
                isPrint = isPrint,
                itemId = parm.itemId
            });
            return res;
        }
        public async Task<WebReport> ItemsBalanceInStoresReport(itemsBalanacesInStoreRequestDTO param, exportType exportType, bool isArabic, int fileId = 0)
        {
            var data = await getItemsBalanceInStores(param, true);
            var companydata = await _CompanyDataService.GetCompanyData(true);
            var userInfo = await _iUserInformation.GetUserInformation();
            var stores = _storeQuery.TableNoTracking
                        .Where(x => x.Id == param.storeId).Select(x => new
                        {
                            x.Id,
                            x.ArabicName,
                            x.LatinName
                        }).FirstOrDefault();
            
            var otherdata = new ReportOtherData()
            {
                ArabicName = stores.ArabicName,
                LatinName = stores.LatinName,
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn=userInfo.employeeNameEn.ToString(),
                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),


            };
            
            var tablesNames = new TablesNames()
            {
                FirstListName = "itemBalanceInStore"
            };

            var report = await _iGeneralPrint.PrintReport<object, itemBalanceInStoreResponseDTO, object>(null, data.data, null, tablesNames, otherdata
             , (int)SubFormsIds.TotalItemBalancesInStore_Repository, exportType, isArabic, fileId);
            return report;

        }

        public async Task<ResponseResult> ItemsBalanceInStores(itemsBalanacesInStoreRequestDTO parm)
        {
            var res = await getItemsBalanceInStores(parm);
            return new ResponseResult()
            {
                Data = res.data,
                DataCount = res.dataCount,
                TotalCount = res.totalCount,
                Result = res.Result,
                Note = res.notes
            };

        }
       
    }
}
