using App.Application.Handlers.Invoices.Vat.GetTotalVatData;
using App.Application.Helpers;
using App.Application.Services.Printing.InvoicePrint;
using App.Application.Services.Printing.PrintFile;
using App.Application.Services.Process.Company_data;
using App.Application.Services.Process.FileManger.ReportFileServices;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Application.Services.Process.StoreServices.Invoices.AccrediteInvoice;
using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Enums;
using App.Domain.Models.Request;
using App.Domain.Models.Response.General;
using App.Domain.Models.Response.Store.Reports.Purchases;
using App.Domain.Models.Response.Store.Reports.Store;
using App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Purchases;
using App.Domain.Models.Security.Authentication.Response.Reports.Purchases;
using App.Domain.Models.Security.Authentication.Response.Store.Reports.Purchases;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.UserManagementDB;
using Attendleave.Erp.Core.APIUtilities;
using DocumentFormat.OpenXml.ExtendedProperties;
using FastReport.Web;
//using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Reports.StoreReports.Invoices.Purchases
{
    public class VatStatementService : IVatStatementService
    {
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterQuery;
        private readonly IRepositoryQuery<GlReciepts> ReceiptsQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> _generalSettingsRepositoryQuery;
        private readonly IRepositoryQuery<GlReciepts> _glRecieptsQuery;
        private readonly IRepositoryQuery<GLBranch> branchQuery;
        private readonly IPrintService _iprintService;

        private readonly IprintFileService _iPrintFileService;
        private readonly IFilesMangerService _filesMangerService;
        private readonly ICompanyDataService _CompanyDataService;
        private readonly iUserInformation _iUserInformation;
        private readonly IRoundNumbers roundNumbers;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IReportFileService _iReportFileService;
        private readonly IGeneralPrint _iGeneralPrint;

        public VatStatementService(
                                    IRepositoryQuery<InvoiceMaster> InvoiceMasterQuery,
                                    IRepositoryQuery<GlReciepts> ReceiptsQuery,
                                    IRepositoryQuery<InvGeneralSettings> generalSettingsRepositoryQuery,
                                    IRepositoryQuery<GlReciepts> GlRecieptsQuery,
                                    IRepositoryQuery<GLBranch> BranchQuery,
                                    IPrintService iprintService,
            IprintFileService iPrintFileService, IFilesMangerService filesMangerService,
            ICompanyDataService CompanyDataService,
            iUserInformation iUserInformation,
                     IRoundNumbers roundNumbers,
            IWebHostEnvironment webHostEnvironment,
            IReportFileService iReportFileService,
            IGeneralPrint iGeneralPrint
            )
        {
            this.InvoiceMasterQuery = InvoiceMasterQuery;
            this.ReceiptsQuery = ReceiptsQuery;
            _generalSettingsRepositoryQuery = generalSettingsRepositoryQuery;
            _glRecieptsQuery = GlRecieptsQuery;
            branchQuery = BranchQuery;
            _iprintService = iprintService;
            _iPrintFileService = iPrintFileService;
            _filesMangerService = filesMangerService;
            _CompanyDataService = CompanyDataService;
            _iUserInformation = iUserInformation;
            this.roundNumbers = roundNumbers;
            _webHostEnvironment = webHostEnvironment;
            _iReportFileService = iReportFileService;
            _iGeneralPrint = iGeneralPrint;
        }
        public Tuple<bool, string> checkDataValidation(VatStatmentRequest _request)
        {
            if (_request.InvoiceType == null || _request.InvoiceType <= 0)
            {
                return Tuple.Create(false, "InvoiceType  Empty");
            }
            if (_request.branches == null || _request.branches.Count() <= 0)
            {
                return Tuple.Create(false, "Branches Id Empty");
            }
           
            return Tuple.Create(true, "");
        }


        public async Task<ResponseResult> GetVatStatmentTransaction(VatStatmentRequest request, bool isPrint = false)
        {

            List<VatStatmentList> InvoiceMainData;
            var lstResInvData = new List<VatStatmentList>();
            var InvList = new VatStatmentList();
            //query to get prev data if flage of prev is true

            List<VatStatmentList> PrevInvoiceMainData = new List<VatStatmentList>();
            if (request.prevBalance)
            {
                PrevInvoiceMainData = InvoiceDataQuery(request, true);
                if (PrevInvoiceMainData.Count() > 0)   //add to list of data exist 
                {
                    InvList.Date = "";
                    InvList.InvoiceTypeAr = "رصيد سابق";
                    InvList.InvoiceTypeEn = "previous Balance";
                    InvList.Debtor = roundNumbers.GetDefultRoundNumber(PrevInvoiceMainData.Sum(a => a.Debtor));
                    InvList.Creaditor = roundNumbers.GetDefultRoundNumber(PrevInvoiceMainData.Sum(a => a.Creaditor));
                    InvList.balance = roundNumbers.GetDefultRoundNumber(InvList.Creaditor - InvList.Debtor);
                    lstResInvData.Add(InvList);
                }

            }
            InvoiceMainData = InvoiceDataQuery(request, false);
            //main query to get all data 
            // add row
            // sum
            var transactionList = TransactionTypeList.transactionTypeModels(); 
            double balanceRes = 0;
            bool prevAdded = true;
            foreach (var item in InvoiceMainData)
            {
                InvList = new VatStatmentList();
                InvList.Date = item.Date;
                InvList.InvoiceTypeId=item.InvoiceTypeId; 
                InvList.InvoiceId = item.InvoiceId;
                InvList.pageId = DocHelpers.GetPageIdOfDoc(item.InvoiceTypeId.Value);
                InvList.pageEnum = DocHelpers.GetPageEnum(item.InvoiceTypeId.Value);
                InvList.DocTypeId = DocHelpers.GetDocTypeId(item.InvoiceTypeId.Value);

                InvList.InvoiceTypeAr = transactionList.Where(x=> x.id == int.Parse(item.InvoiceTypeAr)).FirstOrDefault().arabicName;
                InvList.InvoiceTypeEn = transactionList.Where(x => x.id == int.Parse(item.InvoiceTypeAr)).FirstOrDefault().latinName;
                InvList.InvoiceCode = item.InvoiceCode;
                InvList.Total = roundNumbers.GetDefultRoundNumber(item.Total);
                InvList.totalAfterVat = roundNumbers.GetDefultRoundNumber(item.totalAfterVat);
                InvList.Debtor = roundNumbers.GetDefultRoundNumber(item.Debtor);
                InvList.Creaditor = roundNumbers.GetDefultRoundNumber(item.Creaditor);
                balanceRes += (item.Creaditor - item.Debtor);
                if (request.prevBalance && PrevInvoiceMainData.Count() > 0)
                {
                    if (prevAdded) 
                    {
                        balanceRes += lstResInvData.FirstOrDefault().balance;
                        prevAdded = false;
                    }
                }
                InvList.balance = roundNumbers.GetDefultRoundNumber(balanceRes);
                InvList.rowClassName = item.rowClassName;
                lstResInvData.Add(InvList);
            }

            //pagenation
           var FinalResult = isPrint? lstResInvData : Pagenation<VatStatmentList>.pagenationList(request.PageSize, request.PageNumber, lstResInvData);

           
            double MaxPageNumber = lstResInvData.ToList().Count() / Convert.ToDouble(request.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            var resultRespons = new VatStatmentResponse();
            resultRespons.totalDebtor = roundNumbers.GetDefultRoundNumber(lstResInvData.Sum(a => a.Debtor));
            resultRespons.totalCreaditor = roundNumbers.GetDefultRoundNumber(lstResInvData.Sum(a => a.Creaditor));
            resultRespons.totalBalance = roundNumbers.GetDefultRoundNumber(resultRespons.totalCreaditor - resultRespons.totalDebtor);
            resultRespons.VatStatmentResList = FinalResult;

    

            return new ResponseResult()
            {
                Data = resultRespons,
                DataCount = resultRespons.VatStatmentResList.Count(),
                Result = lstResInvData.Count() > 0 ? Result.Success : Result.NoDataFound,
                Note = (countofFilter == request.PageNumber ? Actions.EndOfData : ""),
                TotalCount = lstResInvData.Count()
            };

        }

        
        public async Task<WebReport> GetVatStatmentTransactionReport(VatStatmentRequest request, exportType exportType, bool isArabic, int fileId = 0)
        {
            var data = await GetVatStatmentTransaction(request,true);
            
            var userInfo = await _iUserInformation.GetUserInformation();
            string invoiceTpeAr = "";
            string invoiceTpeEn = "";

            if (request.InvoiceType == 1)
            {
                invoiceTpeAr = "الكل";
                invoiceTpeEn = "All";
            }
            else if (request.InvoiceType == 2)
            {
                invoiceTpeAr = "مشتريات";
                invoiceTpeEn = "Purchases";

            }
            else if (request.InvoiceType == 3)
            {
                invoiceTpeAr = "مبيعات";
                invoiceTpeEn = "Sales";

            }
            else if (request.InvoiceType == 4)
            {
                invoiceTpeAr = "مسدد";
                invoiceTpeEn = "Paid";

            }
            else
            {
                invoiceTpeAr = "";
                invoiceTpeEn = "";

            }

            var otherdata = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, request.dateFrom, request.dateTo);


            otherdata.EmployeeName = userInfo.employeeNameAr.ToString();
            otherdata.EmployeeNameEn = userInfo.employeeNameEn.ToString();
            

            
            otherdata.ArabicName = invoiceTpeAr;
            otherdata.LatinName = invoiceTpeEn;




            string[] words;
            string value;
            var resData = (VatStatmentResponse)(data.Data);
            value = resData.totalBalance.ToString();
            if(value.Contains("-") && value != "-0"){
                words = value.Split("-");
                if(isArabic)
                resData.totalBalaceData = words[1] + " مدين";
                else
                    resData.totalBalaceData = words[1] + " debtor";

            }
            else if (value == "-0")
            {
                value = value.Replace("-", "");
                resData.totalBalaceData = value;
            }
            else if (value !="0" || value != "-0")
            {
                if(isArabic)
                resData.totalBalaceData = value + " دائن";
                else
                    resData.totalBalaceData = value + " creditor";

            }
            DateTime date;
           
           foreach(var item in resData.VatStatmentResList)
            {
                if (item.Date == null || item.Date == "")
                {
                    item.Date = "----------";
                }
                else
                {
                    date = Convert.ToDateTime(item.Date);
                    item.Date = date.ToString("yyyy/MM/dd");

                }

                value = item.balance.ToString();
                if(value.Contains("-") && value != "-0")
                {
                    words = value.Split("-");
                    if(isArabic)
                    item.balanceListData = words[1] + " مدين";
                    else
                        item.balanceListData = words[1] + " debtor";

                }
                else if (value == "-0")
                {
                    value = value.Replace("-", "");
                    item.balanceListData = value;
                }
                else if (value !="0"|| value != "-0")
                {
                    if(isArabic)
                    item.balanceListData = value + " دائن";
                    else
                        item.balanceListData = value + " creditor";

                }
            }

            
            var tablesNames = new TablesNames()
            {
                ObjectName = "VatStatmentResponse",
                FirstListName = "VatStatmentResList"
            };
            



            var report = await _iGeneralPrint.PrintReport<VatStatmentResponse, VatStatmentList, object>(resData, resData.VatStatmentResList, null, tablesNames, otherdata
             , (int)SubFormsIds.GetVatStatmentData_Purchases, exportType, isArabic, fileId);
            return report;
            
        }
        private List<VatStatmentList> InvoiceDataQuery(VatStatmentRequest request, bool prev)
        {
            var branches = request.branches.Split(',').Select(x=> int.Parse(x)).ToArray();

            var recs = _glRecieptsQuery.TableNoTracking
                .Where(x => x.Authority == (int)AuthorityTypes.other && x.BenefitId == -1 && !x.IsBlock && branches.Contains(x.BranchId))
                .Select(x=> new InvoiceMaster
                {
                    BranchId = x.BranchId,
                    InvoiceDate = x.RecieptDate,
                    InvoiceTypeId = x.RecieptTypeId,
                    InvoiceType = x.RecieptType,
                    PriceWithVat = false,
                    TotalAfterDiscount = 0,
                    Net = 0,
                    TotalVat = x.Amount
                });
            
            var _InvoiceData = InvoiceMasterQuery.TableNoTracking
                                                 .Where(x=> !x.IsDeleted && x.ApplyVat && x.TotalVat > 0)
                                                 .Where(a => a.ApplyVat == true &&
                                                             a.IsDeleted == false &&
                                                             branches.Count() > 0 ? branches.Contains(a.BranchId) : true)
                                                 .ToList();
            _InvoiceData.AddRange(recs);



            #region Filter
            if (request.InvoiceType == (int)InvoiceTypeReport.Purchase)
                _InvoiceData = _InvoiceData.Where(x => x.InvoiceTypeId == (int)DocumentType.Purchase || x.InvoiceTypeId == (int)DocumentType.ReturnPurchase).ToList();
            else if(request.InvoiceType == (int)InvoiceTypeReport.Sales)
                _InvoiceData = _InvoiceData.Where(x => x.InvoiceTypeId == (int)DocumentType.Sales || 
                                                       x.InvoiceTypeId == (int)DocumentType.ReturnSales ||
                                                       x.InvoiceTypeId == (int)DocumentType.POS ||
                                                       x.InvoiceTypeId == (int)DocumentType.ReturnPOS
                                                       
                                                       ).ToList();
            else if (request.InvoiceType == (int)InvoiceTypeReport.TotalInovice)
                _InvoiceData = _InvoiceData.Where(x => 
                                                        x.InvoiceTypeId == (int)DocumentType.Sales || 
                                                        x.InvoiceTypeId == (int)DocumentType.ReturnSales || 
                                                        x.InvoiceTypeId == (int)DocumentType.Purchase || 
                                                        x.InvoiceTypeId == (int)DocumentType.ReturnPurchase ||
                                                        x.InvoiceTypeId == (int)DocumentType.BankCash ||
                                                        x.InvoiceTypeId == (int)DocumentType.SafeCash ||
                                                        x.InvoiceTypeId == (int)DocumentType.BankPayment ||
                                                        x.InvoiceTypeId == (int)DocumentType.SafePayment ||
                                                        x.InvoiceTypeId == (int)DocumentType.POS ||  
                                                        x.InvoiceTypeId == (int)DocumentType.ReturnPOS 
                                                        ).ToList();
            else if(request.InvoiceType == (int)InvoiceTypeReport.receipts)
                _InvoiceData = _InvoiceData.Where(x =>
                                                        x.InvoiceTypeId == (int)DocumentType.BankCash ||
                                                        x.InvoiceTypeId == (int)DocumentType.SafeCash ||
                                                        x.InvoiceTypeId == (int)DocumentType.BankPayment ||
                                                        x.InvoiceTypeId == (int)DocumentType.SafePayment
                                                        ).ToList();
            #endregion




            var invoices = new List<InvoiceMaster>();
            if (prev)   
            {
                 invoices = _InvoiceData.Where(a => a.InvoiceDate < request.dateFrom).ToList();                          
            }else
            {
                 invoices = _InvoiceData.Where(a => a.InvoiceDate.Date >= request.dateFrom.Date && a.InvoiceDate.Date <= request.dateTo.Date).ToList();
            }


            
            var InvoiceData = invoices.OrderBy(a => a.InvoiceDate)
            .Select(x => new VatStatmentList()
            {
                InvoiceTypeId = x.InvoiceTypeId,
            InvoiceId = x.InvoiceId,
            pageId = DocHelpers.GetPageIdOfDoc(x.InvoiceTypeId),
            pageEnum = DocHelpers.GetPageEnum(x.InvoiceTypeId),
            DocTypeId = DocHelpers.GetDocTypeId(x.InvoiceTypeId),
            Date = x.InvoiceDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                InvoiceTypeAr = x.InvoiceTypeId.ToString(),
                InvoiceCode = x.InvoiceType,
                Total = x.PriceWithVat ? (x.TotalAfterDiscount - x.TotalVat) : x.TotalAfterDiscount,
                totalAfterVat = x.Net,
                Debtor = (Lists.InvoiceDebtorList.Contains(x.InvoiceTypeId) || x.InvoiceTypeId == 18 || x.InvoiceTypeId == 20 ? x.TotalVat : 0),
                Creaditor = (Lists.InvoiceCreditorList.Contains(x.InvoiceTypeId) || x.InvoiceTypeId == 19 || x.InvoiceTypeId == 21 ? x.TotalVat : 0),
                rowClassName = x.InvoiceTypeId == (int)DocumentType.ReturnPOS || x.InvoiceTypeId == (int)DocumentType.ReturnPurchase || x.InvoiceTypeId == (int)DocumentType.ReturnSales ? "text-danger":""

            }).ToList();
            return InvoiceData;
        }




    }
}
