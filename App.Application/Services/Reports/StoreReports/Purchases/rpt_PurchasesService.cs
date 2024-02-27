
using App.Application.Handlers.Reports.Purchases.PurchasesTransaction;
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
using App.Domain.Models.Request.Store.Reports.Purchases;
using App.Domain.Models.Response.General;
using App.Domain.Models.Response.Store.Reports;
using App.Domain.Models.Response.Store.Reports.Purchases;
using App.Domain.Models.Security.Authentication.Response.Reports.Purchases;
using App.Domain.Models.Security.Authentication.Response.Store.Reports.Purchases;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.ExtendedProperties;
using FastReport.Web;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
//using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Reports
{
    public class rpt_PurchasesService : iRpt_PurchasesService
    {
        private readonly IRepositoryQuery<InvoiceMaster> _invoiceMasterQuery;
        private readonly IRepositoryQuery<InvoiceDetails> _invoiceDetailsQuery;

        private readonly iUserInformation _iUserInformation;
        private readonly IPrintService _iprintService;
        private readonly IRoundNumbers roundNumbers;

        //  public rpt_PurchasesService(IRepositoryQuery<InvoiceMaster> InvoiceMasterQuery, IRepositoryQuery<InvoiceDetails> InvoiceDetailsQuery, iUserInformation iUserInformation)
        private readonly IprintFileService _iPrintFileService;
        private readonly IFilesMangerService _filesMangerService;
        private readonly ICompanyDataService _CompanyDataService;
        private readonly IRepositoryQuery<GLBranch> branchQuery;
        private readonly IRepositoryQuery<GlReciepts> glRecieptsQuery;
        private readonly IGeneralPrint _iGeneralPrint;


        public rpt_PurchasesService(IRepositoryQuery<InvoiceMaster> InvoiceMasterQuery,
            IRepositoryQuery<InvoiceDetails> InvoiceDetailsQuery,
            iUserInformation iUserInformation,
            IPrintService iprintService,
            IRoundNumbers roundNumbers,
            IprintFileService iPrintFileService, IFilesMangerService filesMangerService,
            ICompanyDataService CompanyDataService,
            IRepositoryQuery<GLBranch> BranchQuery,
            IWebHostEnvironment webHostEnvironment,
            IRepositoryQuery<GlReciepts> GlRecieptsQuery,
            IReportFileService iReportFileService, IGeneralPrint iGeneralPrint, IMediator mediator)
        {
            _invoiceMasterQuery = InvoiceMasterQuery;
            _invoiceDetailsQuery = InvoiceDetailsQuery;
            _iUserInformation = iUserInformation;
            _iprintService = iprintService;
            this.roundNumbers = roundNumbers;
            _iPrintFileService = iPrintFileService;
            _filesMangerService = filesMangerService;
            _CompanyDataService = CompanyDataService;
            branchQuery = BranchQuery;
            glRecieptsQuery = GlRecieptsQuery;
            _iGeneralPrint = iGeneralPrint;
        }
        public async Task<ResponseResult> VatDetailedReport(VATDetailedReportRequest param, bool isPrint = false)
        {
            var checkValidation = isDataValid(param);
            if (checkValidation != null)
                return checkValidation;
            var invoiceTypeId = GetInvoiceTypeId(param.invoicesType);


            var branches = param.branchId.Split(',').Select(c => int.Parse(c)).ToArray();
            var invoices = _invoiceDetailsQuery
                            .TableNoTracking
                            .Include(x=> x.InvoicesMaster)
                            .Include(x=> x.InvoicesMaster.Person)
                            .Where(x=>  invoiceTypeId.Contains(x.InvoicesMaster.InvoiceTypeId) && !x.InvoicesMaster.IsDeleted && x.InvoicesMaster.TotalDiscountRatio != 100 && branches.Contains(x.InvoicesMaster.BranchId))
                            .Where(x=> x.parentItemId == null);

            if (!param.showInvoicesNotAllowedVat)
            {
                invoices = invoices.Where(x => x.InvoicesMaster.TotalVat > 0);
            }
            else
            {
                invoices = invoices.Where(x => x.InvoicesMaster.TotalVat == 0);

            }
            List<PreviousBalances> pervbalance = new List<PreviousBalances>();
         

            var items = invoices
                        .Where(x => x.InvoicesMaster.InvoiceDate.Date >= param.dateFrom.Value.Date && x.InvoicesMaster.InvoiceDate.Date <= param.dateTo.Value.Date )
                        .GroupBy(c=> c.InvoiceId)
                        .Select(x=> new data
                        {
                            invoiceCode = x.First().InvoicesMaster.InvoiceType,
                            invoiceDate = x.First().InvoicesMaster.InvoiceDate,
                            arabicName = x.First().InvoicesMaster.Person.ArabicName,
                            latinName = x.First().InvoicesMaster.Person.LatinName,
                            PersonVatNumber = x.First().InvoicesMaster.Person.TaxNumber,



                            InvoiceId=x.First().InvoicesMaster.InvoiceId,
                            InvoiceTypeId=x.First().InvoicesMaster.InvoiceTypeId,
                            pageId=DocHelpers.GetPageIdOfDoc(x.First().InvoicesMaster.InvoiceTypeId),
                            pageEnum=DocHelpers.GetPageEnum(x.First().InvoicesMaster.InvoiceTypeId),
                            DocTypeId= DocHelpers.GetDocTypeId(x.First().InvoicesMaster.InvoiceTypeId),



                            net = x.First().InvoicesMaster.PriceWithVat? roundNumbers.GetDefultRoundNumber(x.Sum(c=> c.TotalWithSplitedDiscount) - x.First().InvoicesMaster.TotalVat) :
                                                                        roundNumbers.GetDefultRoundNumber(x.Sum(c => c.TotalWithSplitedDiscount))  ,

                            totalHasVat = roundNumbers.GetDefultRoundNumber(!x.First().InvoicesMaster.PriceWithVat ? x.Where(c => c.VatValue != 0).Sum(c => c.TotalWithSplitedDiscount) :
                                                                x.Where(c => c.VatValue != 0).Sum(c => c.TotalWithSplitedDiscount - c.VatValue)),

                            vatValue = roundNumbers.GetDefultRoundNumber(x.First().InvoicesMaster.TotalVat),

                            totalAfterVat = x.First().InvoicesMaster.PriceWithVat ? roundNumbers.GetDefultRoundNumber(x.Where(c => c.VatRatio > 0).Sum(c => c.TotalWithSplitedDiscount)) :
                                                                                    roundNumbers.GetDefultRoundNumber(x.Where(c => c.VatRatio > 0).Sum(c => c.TotalWithSplitedDiscount) + x.First().InvoicesMaster.TotalVat)


                        }).ToList();




            double MaxPageNumber = items.ToList().Count() / Convert.ToDouble(param.pageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
           
             var   FinalResult = isPrint? items: Pagenation<data>.pagenationList(param.pageSize, param.pageNumber, items);
            
            //var res = !isPrint ? Pagenation<data>.pagenationList(param.pageSize, param.pageNumber, items) : items;

            VatDetailedReportResponse respose = new VatDetailedReportResponse()
            {
                items = FinalResult,


                Net = roundNumbers.GetDefultRoundNumber(items.Sum(x=> x.net)),
                TotalHasVat = roundNumbers.GetDefultRoundNumber(items.Sum(x => x.totalHasVat)),
                VatValue = roundNumbers.GetDefultRoundNumber(items.Sum(x => x.vatValue)),
                TotalAfterVat = roundNumbers.GetDefultRoundNumber(items.Sum(x => x.totalAfterVat))
            };

            return new ResponseResult()
            {
                Data = respose,
                Note = (countofFilter == param.pageNumber ? Actions.EndOfData : ""),
                Result = Result.Success,
                TotalCount = items.Count(),
                DataCount = respose.items.Count()

            };
        }
        public async Task<WebReport> VatDetailedReportPrint(VATDetailedReportRequest request, exportType exportType, bool isArabic, int fileId = 0)
        {
            var data = await VatDetailedReport(request,true);
            
            var userInfo = await _iUserInformation.GetUserInformation();
            string invoiceTypeAr = "";
            string invoiceTypeEn = "";
           

            if ( (int)(request.invoicesType) == 1)
            {
                invoiceTypeAr = "الكل";
                 invoiceTypeEn = "All";

            }
            else if ((int)(request.invoicesType) == 2)
            {
                invoiceTypeAr = "مشتريات";
                invoiceTypeEn = "Purchases";

            }
            else if ((int)(request.invoicesType) == 3)
            {
                invoiceTypeAr = "مبيعات";
                invoiceTypeEn = "Sales";

            }
            else if ((int)(request.invoicesType) == 4)
            {
                invoiceTypeAr = "مرتجع مشتريات";
                invoiceTypeEn = "ReturnPurchases";

            }
            else if ((int)(request.invoicesType) == 5)
            {
                invoiceTypeAr = " مرتجع مبيعات";
                invoiceTypeEn = "ReturnSales";

            }
            else
            {
                invoiceTypeAr = "";
                invoiceTypeEn = "";
            }
            DateTime dateFrom = (DateTime)(request.dateFrom);
            DateTime dateTo = (DateTime)(request.dateTo);

            var otherdata = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, dateFrom, dateTo);

            otherdata.EmployeeName = userInfo.employeeNameAr.ToString();
            otherdata.EmployeeNameEn = userInfo.employeeNameEn.ToString();

            otherdata.ArabicName = invoiceTypeAr;
            otherdata.LatinName = invoiceTypeEn;

            var tablesNames = new TablesNames()
            {
                ObjectName = "VatDetailedReportResponse",
                FirstListName = "data"
            };
           var resData = (VatDetailedReportResponse)(data.Data);
            


            var report = await _iGeneralPrint.PrintReport<VatDetailedReportResponse, data, object>(resData, resData.items, null, tablesNames, otherdata
             , (int)SubFormsIds.VatDetailedStatement, exportType, isArabic,fileId);
            return report;
        }

        private ResponseResult isDataValid(VATDetailedReportRequest parm)
        {
            if (string.IsNullOrEmpty(parm.dateFrom.ToString()) || string.IsNullOrEmpty(parm.dateTo.ToString()))
                return new ResponseResult()
                {
                    Note = "Date From And Data To Are Required",
                    Result = Result.Failed
                };
            if (parm.branchId==null)
                return new ResponseResult()
                {
                    Note = "Branch Id is Required",
                    Result = Result.Failed
                };
            if (parm.invoicesType == 0)
                return new ResponseResult()
                {
                    Note = "invoicesType is Required",
                    Result = Result.Failed
                };
            return null;
        }
        private int[] GetInvoiceTypeId(invoicesType invoicesType)
        {
            int[] invoiceTypeId = { 0 };
            if (invoicesType == invoicesType.all)
                invoiceTypeId = new int[] { (int)App.Domain.Enums.Enums.DocumentType.POS, (int)App.Domain.Enums.Enums.DocumentType.ReturnPOS,(int)App.Domain.Enums.Enums.DocumentType.Purchase, (int)App.Domain.Enums.Enums.DocumentType.Sales, (int)App.Domain.Enums.Enums.DocumentType.ReturnPurchase, (int)App.Domain.Enums.Enums.DocumentType.ReturnSales };
            if (invoicesType == invoicesType.Purchases)
                invoiceTypeId = new int[] { (int)App.Domain.Enums.Enums.DocumentType.Purchase };
            else if (invoicesType == invoicesType.sales)
                invoiceTypeId = new int[] { (int)App.Domain.Enums.Enums.DocumentType.Sales, (int)App.Domain.Enums.Enums.DocumentType.POS };
            else if (invoicesType == invoicesType.returnPurchases)
                invoiceTypeId = new int[] { (int)App.Domain.Enums.Enums.DocumentType.ReturnPurchase };
            else if (invoicesType == invoicesType.returnSales)
                invoiceTypeId = new int[] { (int)App.Domain.Enums.Enums.DocumentType.ReturnSales, (int)App.Domain.Enums.Enums.DocumentType.ReturnPOS};

            return invoiceTypeId;
        }

        public async Task<ResponseResult> VatTotalsReport(VATTotalsReportRequest param, bool isPrint = false)
        {
            var checkValidation = isDataaValid(param);
            if (checkValidation != null)
                return checkValidation;
            
            var transactionList = TransactionTypeList.transactionTypeModels();


            var items = ListOfInvoices(param);

            var typeList = items.GroupBy(i => i.InvoiceType)
                .Select(i => new TotalInvoicesVatList
                {
                    TypeId = i.Key,
                    ArabicName = transactionList.First(t => t.id == i.First().InvoiceType).arabicName,
                    LatinName = transactionList.First(t => t.id == i.First().InvoiceType).latinName,
                    TotalPriceWithoutVat = roundNumbers.GetDefultRoundNumber(i.Sum(a=>a.totalHasVat)),
                    Debitor = roundNumbers.GetDefultRoundNumber(i.Sum(a => a.Debitor)),
                    Creditor = roundNumbers.GetDefultRoundNumber(i.Sum(a => a.Creditor)),
                    Balance = roundNumbers.GetDefultRoundNumber(i.Sum(a => a.Creditor) - i.Sum(a => a.Debitor)),
                    Level = 1
                }).OrderBy(x=>x.TypeId).ToHashSet();


            double balance = 0;
             List<int> ids = new List<int> { 5, 6, 8, 9 ,44};
            
            foreach(var type in typeList)
            {
                //balance += type.Creditor - type.Debitor;
                //type.Balance = roundNumbers.GetDefultRoundNumber(balance);
                ids.Remove(type.TypeId);
                    
            };
            if (ids.Count > 0)
            {
                Parallel.ForEach(ids, item =>
                {
                    var trans = transactionList.Where(t => t.id == item).FirstOrDefault();

                    typeList.Add(new TotalInvoicesVatList
                    {
                        TypeId = trans.id,
                        ArabicName = trans.arabicName,
                        LatinName = trans.latinName,
                        TotalPriceWithoutVat = 0,
                        Creditor = 0,
                        Debitor = 0,
                        Balance = 0,
                        isExpanded = false,
                        Level=1
                        

                    });
                });
            }
      
            
            var respose = new TotalVatDataResponse();

            respose.TotalInvoiceList = typeList.OrderBy(x => x.TypeId).ToHashSet();
            respose.TotalPriceWithoutVat = roundNumbers.GetDefultRoundNumber(items.Sum(x => x.totalHasVat));
            respose.TotalDebitor = roundNumbers.GetDefultRoundNumber(items.Sum(a => a.Debitor));
            respose.TotalCreditor = roundNumbers.GetDefultRoundNumber(items.Sum(a => a.Creditor));
            respose.TotalBalance = roundNumbers.GetDefultRoundNumber(respose.TotalCreditor - respose.TotalDebitor);

            return new ResponseResult()
            {
                Data = respose,
                Result = Result.Success,
                TotalCount = items.Count(),
                DataCount = respose.TotalInvoiceList.Count()

            };
        }
        public async Task<ResponseResult> GetBranchesVatReport(VATTotalsReportRequest param, bool isPrint = false)
        {
            var checkValidation = isDataaValid(param);
            if (checkValidation != null)
                return checkValidation;


            var items =  ListOfInvoices(param).Where(x=>x.InvoiceType == param.InvoiceType);
            var TotalInvoicesVatList = items.GroupBy(x=>x.BranchId)
                .Select(i => new TotalInvoicesVatList
                {
                BranchId = i.First().BranchId,
                ArabicName = branchQuery.TableNoTracking.FirstOrDefault(b => b.Id == i.First().BranchId).ArabicName,
                LatinName = branchQuery.TableNoTracking.FirstOrDefault(b => b.Id == i.First().BranchId).LatinName,
                TotalPriceWithoutVat = roundNumbers.GetDefultRoundNumber(i.Sum(a => a.totalHasVat)),
                Debitor = roundNumbers.GetDefultRoundNumber(i.Sum(a => a.Debitor)),
                Creditor = roundNumbers.GetDefultRoundNumber(i.Sum(a => a.Creditor)),
                Balance = roundNumbers.GetDefultRoundNumber(i.Sum(a => a.Creditor) - i.Sum(a => a.Debitor)),
                TypeId = i.First().InvoiceType,
            }).OrderBy(x => x.BranchId).ToHashSet();


            return new ResponseResult()
            {
                Data = TotalInvoicesVatList,
                Result = Result.Success,

            };
        }

        public async Task<WebReport> TotalVatReportPrint(VATTotalsReportRequest request, exportType exportType, bool isArabic,string expandedTypeId, int fileId = 0)
        {
            
            var data = await VatTotalsReport(request, true);

            var mainData = ( TotalVatDataResponse)data.Data;
            var TotalInvoiceList = mainData.TotalInvoiceList.ToList();


            var userInfo = await _iUserInformation.GetUserInformation();

            var otherData = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, (DateTime) request.dateFrom, (DateTime)request.dateTo);


            
          
            var branches = request.branchId.Split(',').Select(b => int.Parse(b)).ToArray();
           
            if (branches.Count() > 0)
            {
                if (branches[0]==0)
                {
                    otherData.LatinName = "الكل";
                    otherData.LatinName = "all";
                }
                else
                {
                    var branchesName = branchQuery.TableNoTracking.Where(a=>branches.Contains(a.Id)).ToList();

                    otherData.ArabicName = string.Join(" , ", branchesName.Select(a => a.ArabicName));//انا هاجى بعد الصلاه اعمل join للاسم العربى
                    otherData.LatinName = string.Join(" , ", branchesName.Select(a => a.LatinName));

                }
            }
            otherData.EmployeeName = userInfo.employeeNameAr.ToString();
            otherData.EmployeeNameEn = userInfo.employeeNameEn.ToString();
           

            if (expandedTypeId != null) {
                var expandedTypes = expandedTypeId.Split(',').Select(c => int.Parse(c)).ToArray();
                if (expandedTypes.Count() > 0)
                {

                    foreach (var type in expandedTypes)
                    {
                        var InvoiceTypebranches = await GetBranchesVatReport(new VATTotalsReportRequest()
                        {
                            InvoiceType = type,
                            branchId = request.branchId,
                            dateFrom = request.dateFrom,
                            dateTo = request.dateTo,
                        });
                        
                        var testdata = TotalInvoiceList.Where(a => a.TypeId == type).FirstOrDefault();
                       
                        var invoiceTypeIndex = TotalInvoiceList.IndexOf(testdata);
                        //(List<TotalInvoicesVatList>)
                       TotalInvoiceList.InsertRange(invoiceTypeIndex + 1, (HashSet<TotalInvoicesVatList>)InvoiceTypebranches.Data);


                    }
                }
            }
            
                var tablesNames = new TablesNames()
            {

                ObjectName = "TotalVat",
                FirstListName = "InvoicesTypeList",
                };

            string value;
            string[] words;
            // main object
            value = mainData.TotalBalance.ToString();
            if (value.Contains("-") && value != "-0")
            {
                words = value.Split("-");
                if (isArabic)
                    mainData.TotalBalances = words[1] + " مدين";
                else
                    mainData.TotalBalances = words[1] + " debtor";

            }
            else if (value == "-0")
            {
                value = value.Replace("-", "");
                mainData.TotalBalances = value;
            }
            else if (value != "0" || value != "-0")
            {
                if (isArabic)
                    mainData.TotalBalances = value + " دائن";
                else
                    mainData.TotalBalances = value + " creditor";

            }
            //list 
            foreach (var item in TotalInvoiceList)
            {
                

                value = item.Balance.ToString();
                if (value.Contains("-") && value != "-0")
                {
                    words = value.Split("-");
                    if (isArabic)
                        item.Balaces = words[1] + " مدين";
                    else
                        item.Balaces = words[1] + " debtor";

                }
                else if (value == "-0")
                {
                    value = value.Replace("-", "");
                    item.Balaces = value;
                }
                else if(value == "0")
                {
                    item.Balaces = value;
                }
                
                else if (value != "0" || value != "-0")
                {
                    if (isArabic)
                        item.Balaces = value + " دائن";
                    else
                        item.Balaces = value + " creditor";

                }
               
            }

            var report = await _iGeneralPrint.PrintReport<TotalVatDataResponse, TotalInvoicesVatList, object>(mainData, TotalInvoiceList, null, tablesNames, otherData
             , (int)SubFormsIds.GetTotalVatData, exportType, isArabic, fileId);
            return report;
        }
        private ResponseResult isDataaValid(VATTotalsReportRequest parm)
        {
            if (string.IsNullOrEmpty(parm.dateFrom.ToString()) || string.IsNullOrEmpty(parm.dateTo.ToString()))
                return new ResponseResult()
                {
                    Note = "Date From And Data To Are Required",
                    Result = Result.Failed
                };
            if (!parm.branchId.Any())
                return new ResponseResult()
                {
                    Note = "Branch Id is Required",
                    Result = Result.Failed
                };
            

            return null;
        }

        private HashSet<InvoicesVatList> ListOfInvoices(VATTotalsReportRequest parm)
        {
                var invoiceTypeId = GetInvoiceTypeId(invoicesType.all);


                var branches = parm.branchId.Split(',').Select(c => int.Parse(c)).ToArray();
                var invoices = _invoiceDetailsQuery
                                .TableNoTracking
                                .Include(x => x.InvoicesMaster)
                                .Where(x => invoiceTypeId.Contains(x.InvoicesMaster.InvoiceTypeId) 
                                && !x.InvoicesMaster.IsDeleted 
                                && branches.Contains(x.InvoicesMaster.BranchId)
                                && x.InvoicesMaster.TotalVat > 0);

            var transactionList = TransactionTypeList.transactionTypeModels();

                var items = invoices
                            .Where(x => x.InvoicesMaster.InvoiceDate.Date >= parm.dateFrom.Value.Date && x.InvoicesMaster.InvoiceDate.Date <= parm.dateTo.Value.Date.AddDays(1))
                            .GroupBy(x => x.InvoiceId)
                            .Select(x => new InvoicesVatList
                            {
                                InvoiceType = x.First().InvoicesMaster.InvoiceTypeId == (int)App.Domain.Enums.Enums.DocumentType.POS ? (int)App.Domain.Enums.Enums.DocumentType.Sales :
                                              (x.First().InvoicesMaster.InvoiceTypeId == (int)App.Domain.Enums.Enums.DocumentType.ReturnPOS ? (int)App.Domain.Enums.Enums.DocumentType.ReturnSales :
                                              (x.First().InvoicesMaster.InvoiceTypeId == (int)App.Domain.Enums.Enums.DocumentType.BankCash ||
                                              x.First().InvoicesMaster.InvoiceTypeId == (int)App.Domain.Enums.Enums.DocumentType.SafeCash ||
                                              x.First().InvoicesMaster.InvoiceTypeId == (int)App.Domain.Enums.Enums.DocumentType.BankPayment ||
                                              x.First().InvoicesMaster.InvoiceTypeId == (int)App.Domain.Enums.Enums.DocumentType.SafePayment ? 44 :x.First().InvoicesMaster.InvoiceTypeId)),
                                BranchId = x.First().InvoicesMaster.BranchId,
                                totalHasVat = roundNumbers.GetDefultRoundNumber(!x.First().InvoicesMaster.PriceWithVat ? x.Where(c => c.VatValue != 0).Sum(c => c.TotalWithSplitedDiscount) :
                                                                    x.Where(c => c.VatValue != 0).Sum(c => c.TotalWithSplitedDiscount - c.VatValue)),

                                Debitor = x.First().InvoicesMaster.InvoiceTypeId == 6 || x.First().InvoicesMaster.InvoiceTypeId == 8 || x.First().InvoicesMaster.InvoiceTypeId == 11 ? roundNumbers.GetDefultRoundNumber(x.First().InvoicesMaster.TotalVat) : 0,
                                Creditor = x.First().InvoicesMaster.InvoiceTypeId == 5 || x.First().InvoicesMaster.InvoiceTypeId == 9 || x.First().InvoicesMaster.InvoiceTypeId == 12  ? roundNumbers.GetDefultRoundNumber(x.First().InvoicesMaster.TotalVat) : 0,

                            }).ToHashSet();

            var recs = glRecieptsQuery.TableNoTracking
            .Where(x => x.Authority == (int)AuthorityTypes.other && x.BenefitId == -1 && !x.IsBlock && branches.Contains(x.BranchId) &&
            x.RecieptDate.Date >= parm.dateFrom.Value.Date && x.RecieptDate.Date <= parm.dateTo.Value.Date.AddDays(1))
            .Select(x => new InvoicesVatList
            {
                InvoiceType = (int)DocumentType.PaidVAT,
                BranchId = x.BranchId,
                totalHasVat = 0,
                Debitor = x.Creditor,
                Creditor = x.Debtor
            }).ToHashSet();

           items.UnionWith(recs);


            return items;

        }

     
    }
}
