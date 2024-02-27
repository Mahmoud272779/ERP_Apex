using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Request.Store.Reports.Store;
using App.Domain.Models.Response.Store.Reports.Store;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.settings;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Application.Services.Reports.StoreReports.Sales.RPT_Sales;
using static App.Domain.Enums.Enums;
using DocumentType = App.Domain.Enums.Enums.DocumentType;

namespace App.Application.Services.Reports.StoreReports.Invoices.Stores.DetailsOfSerialTransactions
{
    public class DetailsOfSerialReportService : BaseClass, IDetailsOfSerialReportService
    {
        private readonly IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvSerialTransaction> _InvSerialTransactionRepositoryQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> _InvStpItemCardMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvoiceDetails> _InvoiceDetailsQuery;
        private readonly IRepositoryQuery<InvPersons> _InvPersonsRepositoryQuery;
        private readonly iUserInformation _iUserInformation;
        private readonly IGeneralPrint _iGeneralPrint;

        public DetailsOfSerialReportService(IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery,
            IRepositoryQuery<InvSerialTransaction> InvSerialTransactionRepositoryQuery,
            IRepositoryQuery<InvPersons> InvPersonsRepositoryQuery,
            IRepositoryQuery<InvStpItemCardMaster> InvStpItemCardMasterRepositoryQuery,
            IHttpContextAccessor httpContextAccessor,
            iUserInformation iUserInformation,
            IGeneralPrint iGeneralPrint,
            IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery) : base(httpContextAccessor)
        {
            _InvoiceMasterRepositoryQuery = InvoiceMasterRepositoryQuery;
            _InvSerialTransactionRepositoryQuery = InvSerialTransactionRepositoryQuery;
            _InvPersonsRepositoryQuery = InvPersonsRepositoryQuery;
            _InvStpItemCardMasterRepositoryQuery = InvStpItemCardMasterRepositoryQuery;
            _iUserInformation = iUserInformation;
            _iGeneralPrint = iGeneralPrint;
            _InvoiceDetailsQuery = invoiceDetailsQuery;
        }

        public async Task<ResponseResult> DetailsOfSerialTransactions(DetailsOfSerialsRequest request, bool isPrint = false)
        {

            var serialInfo = _InvSerialTransactionRepositoryQuery.TableNoTracking.Where(q => q.SerialNumber == request.serial).OrderBy(x => x.Id).ToList();
            if (serialInfo.Count == 0)
                return new ResponseResult()
                {
                    Note = "This serial not found in this store",
                    Result = Result.NoDataFound
                };

            if (serialInfo.Any(a => string.IsNullOrEmpty(a.AddedInvoice) && string.IsNullOrEmpty(a.ExtractInvoice)))
                return new ResponseResult()
                {
                    Note = "No Transaction Yet",
                    Result = Result.NoDataFound
                };

            var itemIDs = serialInfo.GroupBy(x => x.ItemId).Select(x => x.First().ItemId).ToList();
            var itemInfo = _InvStpItemCardMasterRepositoryQuery.TableNoTracking.Where(x => itemIDs.Contains(x.Id));

            var listOfInvoiceTypeIds = Lists.MainInvoiceList;
            listOfInvoiceTypeIds.AddRange(new[] { (int)DocumentType.ReturnPOS, (int)DocumentType.ReturnPurchase, (int)DocumentType.ReturnSales });
            var invoices = _InvoiceDetailsQuery.TableNoTracking
                                               .Include(x => x.InvoicesMaster)
                                               .ThenInclude(x => x.store)
                                               .Include(x => x.InvoicesMaster.storeTo)
                                               .Where(x => (serialInfo.Select(a => a.AddedInvoice).Contains(x.InvoicesMaster.InvoiceType) || serialInfo.Select(a => a.ExtractInvoice).Contains(x.InvoicesMaster.InvoiceType)))
                                               //.Where(x=> serialInfo.Select(c=> c.ExtractInvoice).ToArray().Contains(x.InvoicesMaster.ParentInvoiceCode))
                                               .Where(x => itemIDs.Contains(x.ItemId))
                                               .Where(x=> listOfInvoiceTypeIds.Contains(x.InvoicesMaster.InvoiceTypeId))
                                               .OrderBy(x => x.InvoicesMaster.Serialize)
                                               .ToList()
                                               .GroupBy(x => x.InvoiceId);


            invoices = isPrint ? invoices : (request.PageSize > 0 && request.PageNumber > 0 ? invoices.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList() : invoices);

            var listOfReturns = Lists.rowClassNameTransaction;
            List<SerialDetailsReport> response = new List<SerialDetailsReport>();
            foreach (var inv in invoices)
            {
                InvPersons person = null;
                var serlialElement = serialInfo.Where(x => x.ExtractInvoice == inv.FirstOrDefault().InvoicesMaster.InvoiceType || x.AddedInvoice == inv.FirstOrDefault().InvoicesMaster.InvoiceType).FirstOrDefault();
                int indexOfRow = inv.First().Signal > 0 ? serlialElement?.indexOfSerialForAdd ?? 0 : serlialElement?.indexOfSerialForExtract ?? 0;
                var element = inv.Count() > 1 ? inv.Where(x => x.indexOfItem == indexOfRow).FirstOrDefault() : inv.FirstOrDefault();
                int transferStatues = serlialElement.TransferStatus;

                //bool isTransferRejected = serialInfo.Where(x => x.AddedInvoice == element.InvoicesMaster.InvoiceType).FirstOrDefault()?.TransferStatus??0;
                    bool isTransferRejected = false;




                if (element.InvoicesMaster.InvoiceTypeId == (int)DocumentType.IncomingTransfer || element.InvoicesMaster.InvoiceTypeId == (int)DocumentType.OutgoingTransfer)
                    isTransferRejected = transferStatues == (int)TransferStatusEnum.Rejected ? true : false;
                if (element.InvoicesMaster.PersonId != null && element.InvoicesMaster.PersonId != 0)
                    person = await _InvPersonsRepositoryQuery.GetByIdAsync(element.InvoicesMaster.PersonId);
                if (response.Where(x => x.DocumentCode == element.InvoicesMaster.InvoiceType).Any())
                    continue;



                SerialDetailsReport serialDetailsReport = new SerialDetailsReport()
                {
                    InvoiceId = element.InvoicesMaster.InvoiceId,
                    InvoiceTypeId = element.InvoicesMaster.InvoiceTypeId,
                    pageId = DocHelpers.GetPageIdOfDoc(element.InvoicesMaster.InvoiceTypeId),
                    pageEnum=DocHelpers.GetPageEnum(element.InvoicesMaster.InvoiceTypeId),
                    DocTypeId = DocHelpers.GetDocTypeId(element.InvoicesMaster.InvoiceTypeId),
                    Date = element.InvoicesMaster.InvoiceDate,
                    DocumentDate = element.InvoicesMaster.InvoiceDate.ToString("yyyy/mm/dd"),
                    DocumentCode = element.InvoicesMaster.InvoiceType,


                    DocumentTypeAr =  (listOfInvoicesNames.listOfNames().FirstOrDefault(a => a.invoiceTypeId == element.InvoicesMaster.InvoiceTypeId)?.NameAr ?? "") + (element.InvoicesMaster.IsDeleted ? " - محذوف" : (isTransferRejected ? " - مرفوض" : "")    ),
                    DocumentTypeEn = (listOfInvoicesNames.listOfNames().FirstOrDefault(a => a.invoiceTypeId == element.InvoicesMaster.InvoiceTypeId)?.NameEn ?? "")  + (element.InvoicesMaster.IsDeleted ? " - Deleted" : (isTransferRejected ? " - Rejected" : "")) ,


                    ArabicName = person == null ? "" : person.ArabicName,
                    latinName = person == null ? "" : person.LatinName,
                    ItemNameAr = itemInfo.Where(c => c.Id == element.ItemId).FirstOrDefault().ArabicName,
                    ItemNameEn = itemInfo.Where(c => c.Id == element.ItemId).FirstOrDefault().LatinName,

                    rowClassName = listOfReturns.Any(x => x == element.InvoicesMaster.InvoiceTypeId) || (isTransferRejected) || element.InvoicesMaster.IsDeleted ? defultData.text_danger : "",

                    storeNameAr = element.InvoicesMaster.store.ArabicName,
                    storeNameEn = element.InvoicesMaster.store.LatinName
                };
                response.Add(serialDetailsReport);
            }

            if (response.Count == 0)
                return new ResponseResult() { Data = null, Result = Result.NoDataFound, Note = "No Data Found" };

            return new ResponseResult() { Data = response, Result = Result.Success, DataCount = response.Count };
        }
        public async Task<WebReport> DetailsOfSerialTransactionsReport(DetailsOfSerialsRequest request, exportType exportType, bool isArabic, int fileId = 0)
        {
            var data = await DetailsOfSerialTransactions(request, true);

            var mainData = (List<SerialDetailsReport>)data.Data;
            var userInfo = await _iUserInformation.GetUserInformation();



            var AdditionalReportdata = new ReportOtherData()
            {
                Code = request.serial,
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),
                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
            };




            var tablesNames = new TablesNames()
            {

                FirstListName = "DetailsOfSerialTransactions"

            };




            var report = await _iGeneralPrint.PrintReport<object, SerialDetailsReport, object>(null, mainData, null, tablesNames, AdditionalReportdata
             , (int)SubFormsIds.DetailsOfSerialTransactions, exportType, isArabic,fileId);
            return report;
        }

    }
}
