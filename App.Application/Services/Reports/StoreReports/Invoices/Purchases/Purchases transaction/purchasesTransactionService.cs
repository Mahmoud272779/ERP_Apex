using App.Application.Helpers;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request.Reports.Invoices.Purchases;
using App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Purchases;
using App.Domain.Models.Security.Authentication.Response.Reports.Purchases;
using App.Domain.Models.Security.Authentication.Response.Store.Reports.Purchases;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Reports.StoreReports.Invoices.Purchases.Purchases_transaction
{
    public class purchasesTransactionService: IpurchasesTransactionService
    {
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterQuery;
        private readonly iUserInformation _iUserInformation;
        private readonly IGeneralPrint _iGeneralPrint;
        //  private readonly IRepositoryQuery<InvoicePaymentsMethods> InvoicePaymentsMethodsQuery;
        private readonly IMediator _mediator;

        public purchasesTransactionService(IRepositoryQuery<InvoiceMaster> InvoiceMasterQuery, iUserInformation iUserInformation, IGeneralPrint iGeneralPrint, IMediator mediator)
        {
            this.InvoiceMasterQuery = InvoiceMasterQuery;
            _iUserInformation = iUserInformation;
            _iGeneralPrint = iGeneralPrint;
            _mediator = mediator;
        }



        public async Task<ResponseResult> PurchasesTransaction(purchasesTransactionRequest request, bool isPrint = false)
        {
            // base from invoicePaymentMethod include invoicemastre theninclude supplier
            #region impComment
            //var purchasesTrans = InvoicePaymentsMethodsQuery.TableNoTracking.Include(a => a.InvoicesMaster)
            //    .ThenInclude(a => a.Supplier).Include(a=>a.PaymentMethod)
            //    .Where(a => a.InvoicesMaster.InvoiceTypeId == (int)DocumentType.Purchase && a.InvoicesMaster.IsDeleted == false &&
            //             (a.InvoicesMaster.InvoiceDate.Date >= request.dateFrom && a.InvoicesMaster.InvoiceDate.Date <= request.dateTo.Date)
            //             && (request.supplierId !=null ? a.InvoicesMaster.SupplierId == request.supplierId : true)
            //             && (request.branches.Count() > 0 ? request.branches.Contains(a.BranchId) : true)
            //             && request.paymentMethod !=null ? a.PaymentMethodId == request.paymentMethod :true ).ToList()
            //             .GroupBy(e => new {  e.InvoicesMaster.SupplierId ,e.PaymentMethodId })
            //             .Select(x => new purchasesTransactionList()
            //             {
            //                 supplierId = x.Key.SupplierId,
            //                 supplierName = x.First().InvoicesMaster.Supplier.ArabicName,
            //                 paymentMethodId = x.First().PaymentMethodId,
            //                 paymentMethod =ReportData<InvoicePaymentsMethods>.paymentMethod(x),
            //                 purchasesCount = x.Count(),
            //                 purchasesNet = x.Sum(a => a.InvoicesMaster.Net)
            //             });

            #endregion
            var branches = request.branches.Split(',').ToArray().Select(c => int.Parse(c)).ToList();
            var purchasesTrans = InvoiceMasterQuery.TableNoTracking.Include(a => a.Person)
             .Where(a => (a.InvoiceTypeId == (int)DocumentType.Purchase) && a.IsDeleted == false &&
                      (a.InvoiceDate.Date >= request.dateFrom && a.InvoiceDate.Date <= request.dateTo.Date)
                      && (request.supplierId != null ? a.PersonId == request.supplierId : true)
                      && (request.branches.Count() > 0 ? branches.Contains(a.BranchId) : true)
                      && (request.paymentMethod != null ? a.PaymentType == request.paymentMethod : true)).ToList()
                     .OrderBy(a=>a.PersonId)
                      .GroupBy(e => new { e.PersonId, e.PaymentType })
                      .Select(x => new purchasesTransactionList()
                      {
                          supplierId = x.Key.PersonId,
                          supplierName = x.First().Person.ArabicName,
                          supplierNameEn=x.First().Person.LatinName,
                          paymentMethodId = x.First().PaymentType,
                          paymentMethod = ReportData<InvoiceMaster>.paymentMethod(x),
                          paymentMethodEn= x.First().PaymentType == (int)PaymentType.Complete ? "Cash":(x.First().PaymentType == (int)PaymentType.Delay ? "Deferred" : "Partial"),
                          purchasesCount = x.Count(),
                          purchasesNet = x.Sum(a => a.Net)
                      });
            var returnPurchasesTrans = InvoiceMasterQuery.TableNoTracking.Include(a => a.Person)
            .Where(a => (a.InvoiceTypeId == (int)DocumentType.ReturnPurchase) && a.IsDeleted == false &&
                     (a.InvoiceDate.Date >= request.dateFrom && a.InvoiceDate.Date <= request.dateTo.Date)
                     && (request.supplierId != null ? a.PersonId == request.supplierId : true)
                     && (request.branches.Count() > 0 ? branches.Contains(a.BranchId) : true)
                     && (request.paymentMethod != null ? a.PaymentType == request.paymentMethod : true)).ToList()
                     .OrderBy(a => a.PersonId)
                     .GroupBy(e => new { e.PersonId, e.PaymentType })
                     .Select(x => new purchasesTransactionList()
                     {
                         supplierId = x.Key.PersonId,
                         supplierName = x.First().Person.ArabicName,
                         supplierNameEn = x.First().Person.LatinName,

                         paymentMethodId = x.First().PaymentType,
                         paymentMethod = ReportData<InvoiceMaster>.paymentMethod(x),
                         paymentMethodEn = x.First().PaymentType == (int)PaymentType.Complete ? "Paid" : (x.First().PaymentType == (int)PaymentType.Delay ? "Deferred" : "Cash"),

                         returnPurchasesCount = x.Count(),
                         returnPurchasesNet = x.Sum(a => a.Net)
                     });
            var transactions = purchasesTrans.Union(returnPurchasesTrans).ToList()
                .OrderBy(a => a.supplierId).GroupBy(a => new { a.supplierId, a.paymentMethodId });
           var transactions1= transactions.Select(x => new purchasesTransactionList()
                   {

               supplierId = x.Key.supplierId,
               supplierName = x.First().supplierName,
               supplierNameEn = x.First().supplierNameEn,

               paymentMethodId = x.First().paymentMethodId,
               paymentMethod = x.First().paymentMethod,
               paymentMethodEn = x.First().paymentMethodEn,

               purchasesCount = x.Sum(a => a.purchasesCount),
               purchasesNet = x.Sum(a => a.purchasesNet),
               returnPurchasesCount = x.Sum(a => a.returnPurchasesCount),
               returnPurchasesNet = x.Sum(a => a.returnPurchasesNet),
               Net = x.Sum(a => a.purchasesNet) - x.Sum(a => a.returnPurchasesNet)
           });

            var totalRes = new purchasesTransactionResponse()
            {
                totalPurchasesNet = transactions1.Sum(a => a.purchasesNet),
                totalReturnPurchasesNet = transactions1.Sum(a => a.returnPurchasesNet),
                totalNet = transactions1.Sum(a => a.Net),
                purchasesTransactionList =isPrint? transactions1.ToList(): Pagenation<purchasesTransactionList>.pagenationList(request.PageSize, request.PageNumber, transactions1.ToList())

        };
            return new ResponseResult { Data = totalRes , DataCount= transactions.Count() , Id=null , Result = transactions.Count()>0 ?Result.Success : Result.NoDataFound};
        }
        public async Task<WebReport> PurchasesTransactionReport(purchasesTransactionRequest request, exportType exportType, bool isArabic,int fileId=0)
        {
            var data = await PurchasesTransaction(request, true);
            var mainData = (purchasesTransactionResponse)data.Data;
            var userInfo = await _iUserInformation.GetUserInformation();
            string paymentTypeAr = "";
            string paymentTypeEn = "";
            // Payment Types
            if ((int)request.paymentMethod == 0)
            {
                paymentTypeAr = "الكل";
                paymentTypeEn = "All";

            }
            else if ((int)request.paymentMethod == 1)
            {
                paymentTypeAr = "نقدى";
                paymentTypeEn = "Cash";

            }
            else if ((int)request.paymentMethod == 2)
            {
                paymentTypeAr = "جزئى";
                paymentTypeEn = "Partial";


            }
            else
            {
                paymentTypeAr = "اجل";
                paymentTypeEn = "Deferred";

            }

            var otherdata = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, request.dateFrom, request.dateTo);




            otherdata.ArabicName = paymentTypeAr;
            otherdata.LatinName = paymentTypeEn;
            otherdata.EmployeeName = userInfo.employeeNameAr.ToString();
            otherdata.EmployeeNameEn = userInfo.employeeNameEn.ToString();


            var tablesNames = new TablesNames()
            {

                ObjectName = "PurchasesTransaction",
                FirstListName = "PurchasesTransactionList"
            };
            var report = await _iGeneralPrint.PrintReport<purchasesTransactionResponse, purchasesTransactionList, object>(mainData, mainData.purchasesTransactionList, null, tablesNames, otherdata
             , (int)SubFormsIds.purchasesTransaction_Purchases, exportType, isArabic, fileId);
            return report;


        }

        public async Task<WebReport> PurchasesTransactionOfBrachReport(purchasesTransactionRequest request, exportType exportType, bool isArabic)
        {
            var data = await _mediator.Send(request);

            var mainData = (purchasesTransactionResponse)data;
            var userInfo = await _iUserInformation.GetUserInformation();
            string paymentTypeAr = "";
            string paymentTypeEn = "";
            // Payment Types
            if ((int)request.paymentMethod == 0)
            {
                paymentTypeAr = "الكل";
                paymentTypeEn = "All";

            }
            else if ((int)request.paymentMethod == 1)
            {
                paymentTypeAr = "نقدى";
                paymentTypeEn = "Cash";

            }
            else if ((int)request.paymentMethod == 2)
            {
                paymentTypeAr = "جزئى";
                paymentTypeEn = "Partial";


            }
            else
            {
                paymentTypeAr = "اجل";
                paymentTypeEn = "Deferred";

            }

            var otherdata = new ReportOtherData()
            {

                ArabicName = paymentTypeAr,
                LatinName = paymentTypeEn,
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),
                DateFrom = request.dateFrom.ToString("yyyy/MM/dd"),
                DateTo = request.dateTo.ToString("yyyy/MM/dd"),
                Date = DateTime.Now.ToString("yyyy/MM/dd")

            };

            var tablesNames = new TablesNames()
            {

                ObjectName = "PurchasesTransaction",
                FirstListName = "PurchasesTransactionList"
            };
            var report = await _iGeneralPrint.PrintReport<purchasesTransactionResponse, purchasesTransactionList, object>(mainData, mainData.purchasesTransactionList, null, tablesNames, otherdata
             , (int)SubFormsIds.purchasesTransaction_Purchases, exportType, isArabic);
            return report;


        }

    }
}
