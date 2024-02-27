using App.Application.Helpers.Service_helper.History;
using App.Application.Services.Printing;
using App.Application.Services.Printing.InvoicePrint;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Request.Store.Reports.MainData;
using App.Domain.Models.Response.GeneralLedger;
using App.Domain.Models.Response.Store;
using App.Domain.Models.Response.Store.Reports.MainData;
using App.Domain.Models.Response.Store.Reports.Sales;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Application.Services.Process.GLServices.ledger_Report.LedgerReportService;
using static App.Application.Services.Reports.StoreReports.RPT_BanksSafesServices.safeOrBankReportService;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Reports.StoreReports.MainData
{
    public class RPT_MainData : iRPT_MainData
    {
        private readonly IRepositoryQuery<InvStpItemCardMaster> _invStpItemCardMasterQuery;
        private readonly IRepositoryQuery<InvStpUnits> _invStpUnitsQuery;
        private readonly IPrintService _iprintService;
        private readonly IFilesMangerService _filesMangerService;
        private readonly ICompanyDataService _CompanyDataService;
        private readonly iUserInformation _iUserInformation;
        private readonly IGeneralPrint _iGeneralPrint;




        public RPT_MainData(IRepositoryQuery<InvStpItemCardMaster> InvStpItemCardMasterQuery, IRepositoryQuery<InvStpUnits> InvStpUnitsQuery, IPrintService iprintService, IFilesMangerService filesMangerService, ICompanyDataService companyDataService, iUserInformation iUserInformation, IGeneralPrint iGeneralPrint)
        {
            _invStpItemCardMasterQuery = InvStpItemCardMasterQuery;
            _invStpUnitsQuery = InvStpUnitsQuery;
            _iprintService = iprintService;
            _filesMangerService = filesMangerService;
            _CompanyDataService = companyDataService;
            _iUserInformation = iUserInformation;
            _iGeneralPrint = iGeneralPrint;
        }
        public async Task<itemsPricesResponse> getItemsPrices(itemsPricesRequestDTO parm, bool isPrint = false)
        {
            var units = _invStpUnitsQuery.TableNoTracking.ToList();
            var items = _invStpItemCardMasterQuery.TableNoTracking
                                                  .Include(x => x.Units).Include(x => x.Category).Include(x => x.StorePlace)
                                                  .Where(x => x.TypeId != 6)
                                                  .Where(x => parm.itemId != 0 ? x.Id == parm.itemId : true)
                                                  .Where(x => parm.catId != 0 ? x.GroupId == parm.catId : true)
                                                  .Where(x => parm.statues != 0 ? x.Status == parm.statues : true)
                                                  .Where(x => parm.itemTypes != ItemTypes.all ? x.TypeId == (int)parm.itemTypes : true)
                                                  .ToList()

                                                  .Select(x => new itemsPricesResponseList
                                                  {

                                                      itemCode = x.ItemCode,
                                                      arabicName = x.ArabicName,
                                                      latinName = x.LatinName,
                                                      unitArabicName = units.Where(c => c.Id == x.ReportUnit).FirstOrDefault()?.ArabicName ?? "",
                                                      unitLatinName = units.Where(c => c.Id == x.ReportUnit).FirstOrDefault()?.LatinName ?? "",
                                                      purchasesPrice = x.Units.Where(c => c.UnitId == x.ReportUnit).First()?.PurchasePrice ?? 0,
                                                      salesPrice1 = x.Units.Where(c => c.UnitId == x.ReportUnit).First()?.SalePrice1 ?? 0,
                                                      salesPrice2 = x.Units.Where(c => c.UnitId == x.ReportUnit).First()?.SalePrice2 ?? 0,
                                                      salesPrice3 = x.Units.Where(c => c.UnitId == x.ReportUnit).First()?.SalePrice3 ?? 0,
                                                      salesPrice4 = x.Units.Where(c => c.UnitId == x.ReportUnit).First()?.SalePrice4 ?? 0,
                                                      //for print
                                                      CategoryNameAr = x.Category != null ? x.Category.ArabicName : "",
                                                      CatogeryNameEn = x.Category != null ? x.Category.LatinName : "",
                                                      Status=x.Status,
                                                      StorePlaceStatus=x.StorePlace !=null?x.StorePlace.Status:0,
                                                      StoreNameAr= x.StorePlace != null ? x.StorePlace.ArabicName : "",
                                                      StoreNameEn = x.StorePlace != null ? x.StorePlace.LatinName : "",
                                                      Model=x.Model,
                                                      NationalBarcode=x.NationalBarcode,
                                                      VAT=x.VAT,
                                                      Description=x.Description,
                                                      TypeId=x.TypeId
                                                  })
                                                  .ToList();
            double MaxPageNumber = items.Count() / Convert.ToDouble(parm.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            var resData = !isPrint ? Pagenation<itemsPricesResponseList>.pagenationList(parm.PageSize, parm.PageNumber, items) : items;
            return new itemsPricesResponse()
            {
                data= resData,
                dataCount = resData.Count(),
                TotalCount = items.Count(),
                Result = Result.Success,
                notes = (countofFilter == parm.PageNumber ? Actions.EndOfData : "")
            };
        }

        public async Task<WebReport> ItemsPricesReport(itemsPricesRequestDTO parm, exportType exportType,bool isArabic, int fileId = 0)
        {
            if (parm.itemId == null)
            {
                parm.itemId = 0;
            }
            var mainDataObject = await getItemsPrices(parm,true);
           // var findPerson = await _personsQuery.GetByIdAsync(mainDataObject.PersonId);

           
            var userInfo = await _iUserInformation.GetUserInformation();

            var otherData = new ReportOtherData()
            {
                EmployeeName= userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),
                Date=DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")

            };
            var tablesNames = new TablesNames()
            {
                FirstListName = "ItemsPrices"
            };

            var report = await _iGeneralPrint.PrintReport<object, itemsPricesResponseList, object>(null, mainDataObject.data, null, tablesNames, otherData
             , (int)SubFormsIds.itemsPrices, exportType, isArabic,fileId);
            return report;

        }
        public async Task<ResponseResult> ItemsPrices(itemsPricesRequestDTO parm)
        {
            var res = await getItemsPrices(parm);
            return new ResponseResult()
            {
                Note = res.notes,
                Data = res.data,
                TotalCount = res.TotalCount,
                DataCount = res.dataCount,
                Result = res.Result
            };
        }
    }
}
