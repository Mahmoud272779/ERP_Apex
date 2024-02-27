using App.Application.Services.Printing.InvoicePrint;
using App.Application.Services.Printing.PrintFile;
using App.Domain.Models.Security.Authentication.Request.Reports.Invoices.Purchases;
using App.Domain.Models.Security.Authentication.Response.Reports.Purchases;
using DocumentFormat.OpenXml.Office2010.Word;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Org.BouncyCastle.Ocsp;
using System.Collections.Immutable;

namespace App.Application.Services.Reports.Invoices.Purchases.Supplier_Account
{
    public class PersonAccountService : IPersonAccountService
    {
        private readonly IRepositoryQuery<GlReciepts> RecieptQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> _generalSettingsRepositoryQuery;
        private readonly IPrintService _iprintService;
        private readonly IprintFileService _iPrintFileService;
        private readonly IFilesMangerService _filesMangerService;
        private readonly ICompanyDataService _CompanyDataService;
        private readonly iUserInformation _iUserInformation;
        private readonly IRoundNumbers roundNumbers;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IReportFileService _iReportFileService;
        private readonly IGeneralPrint _iGeneralPrint;
        private readonly iUserInformation _userInformation;
        private readonly IPersonHelperService _personHelperService;

        // private readonly IFilesMangerService _filesMangerService;

        // public PersonAccountService(IRepositoryQuery<GlReciepts> RecieptQuery, IRepositoryQuery<InvGeneralSettings> generalSettingsRepositoryQuery)
        public PersonAccountService(IRepositoryQuery<GlReciepts> RecieptQuery,
            IPrintService iprintService,
            IprintFileService iPrintFileService, IFilesMangerService filesMangerService,
            ICompanyDataService CompanyDataService,
            iUserInformation iUserInformation,
            IRoundNumbers roundNumbers,
            IWebHostEnvironment webHostEnvironment,
            IReportFileService iReportFileService, IRepositoryQuery<InvGeneralSettings> generalSettingsRepositoryQuery, IGeneralPrint iGeneralPrint, iUserInformation userInformation, IPersonHelperService personHelperService)
        {
            this.RecieptQuery = RecieptQuery;
            _generalSettingsRepositoryQuery = generalSettingsRepositoryQuery;
            _iprintService = iprintService;
            _iPrintFileService = iPrintFileService;
            _filesMangerService = filesMangerService;
            _CompanyDataService = CompanyDataService;
            _iUserInformation = iUserInformation;
            this.roundNumbers = roundNumbers;
            _webHostEnvironment = webHostEnvironment;
            _iReportFileService = iReportFileService;
            _iGeneralPrint = iGeneralPrint;
            _userInformation = userInformation;
            _personHelperService = personHelperService;
        }

        public Tuple<bool, string> checkDataValidation(SupplierAccountRequest _request)
        {
            if (_request.personId == null || _request.personId <= 0)
            {
                return Tuple.Create(false, "Supplier Id Empty");
            }
            if (_request.Branches == null || _request.Branches.Count() <= 0)
            {
                return Tuple.Create(false, "Branches Id Empty");
            }
            if (_request.DateFrom == null)
            {
                return Tuple.Create(false, "DateFrom Id Empty");
            }
            if (_request.DateTo == null)
            {
                return Tuple.Create(false, "DateTo Id Empty");
            }
            return Tuple.Create(true, "");
        }
        private int GetdocTypeId(bool isReciepts, int parentTypeId,int recTypeId)
        {
            if (parentTypeId == (int)Enums.DocumentType.AcquiredDiscount || parentTypeId == (int)Enums.DocumentType.PermittedDiscount)
                return (int)Enums.PersonAccountDataDocTypeId.discount;
            if (parentTypeId == (int)Enums.DocumentType.CustomerFunds || parentTypeId == (int)Enums.DocumentType.SuplierFunds)
                return 0;
            if (recTypeId == (int)Enums.DocumentType.SafeMultiCollectionReceipts || recTypeId == (int)Enums.DocumentType.BankMultiCollectionReceipts)
                return (int)Enums.PersonAccountDataDocTypeId.multiCollectionRec;
            else if (isReciepts)
            {
                return (int)Enums.PersonAccountDataDocTypeId.reciepts;
            }
            else
            {
                return (int)Enums.PersonAccountDataDocTypeId.invoices;
            }

            return 0;
        }

        #region GetPersonAccountData helper
        private int GetPageId(int docTypeId,string type = "")
        {
            if (docTypeId == (int)Enums.DocumentType.SafeMultiCollectionReceipts)
                return (int)SubFormsIds.SafeMultiCollectionReceipt;
            if (docTypeId == (int)Enums.DocumentType.BankMultiCollectionReceipts)
                return (int)SubFormsIds.BankMultiCollectionReceipt;

            if (docTypeId == (int)Enums.DocumentType.SafeCash)
                return (int)SubFormsIds.CashReceiptForSafe;
            if (docTypeId == (int)Enums.DocumentType.SafePayment)
                return (int)SubFormsIds.PayReceiptForSafe;

            if (docTypeId == (int)Enums.DocumentType.BankCash)
                return (int)SubFormsIds.CashReceiptForBank;
            if (docTypeId == (int)Enums.DocumentType.BankPayment)
                return (int)SubFormsIds.PayReceiptForBank;

            if (docTypeId == (int)Enums.DocumentType.Purchase || docTypeId == (int)Enums.DocumentType.DeletePurchase)
                return (int)SubFormsIds.Purchases;

            if (docTypeId == (int)Enums.DocumentType.ReturnPurchase)
                return (int)SubFormsIds.PurchasesReturn_Purchases;


            if (docTypeId == (int)Enums.DocumentType.Sales || docTypeId == (int)Enums.DocumentType.DeleteSales)
                return (int)SubFormsIds.Sales;
            if (docTypeId == (int)Enums.DocumentType.ReturnSales)
                return (int)SubFormsIds.SalesReturn_Sales;


            if (docTypeId == (int)Enums.DocumentType.AcquiredDiscount)
                return (int)SubFormsIds.EarnedDiscount_Settelments;

            if (docTypeId == (int)Enums.DocumentType.PermittedDiscount)
                return (int)SubFormsIds.PermittedDiscount_Settelments;


            if(docTypeId == (int)Enums.DocumentType.POS)
                return (int)SubFormsIds.POS;
            if (docTypeId == (int)Enums.DocumentType.ReturnPOS)
                return (int)SubFormsIds.returnPOS;

            


            return 0;
        }
        private string GetDocumentId(int? ParentTypeId,string? PaperNumber,string? RecieptType,int? CollectionMainCode,int recTypeId)
        {
            string DocumentId = string.Empty;
            if (recTypeId == (int)Enums.DocumentType.BankMultiCollectionReceipts || recTypeId == (int)Enums.DocumentType.SafeMultiCollectionReceipts)
                return RecieptType;
            if (ParentTypeId == (int)Enums.DocumentType.CustomerFunds || ParentTypeId == (int)Enums.DocumentType.SuplierFunds)
                return DocumentId;
            if (ParentTypeId == (int)Enums.DocumentType.AcquiredDiscount || ParentTypeId == (int)Enums.DocumentType.PermittedDiscount)
                return RecieptType;
            if (PaperNumber != null && RecieptType == null && CollectionMainCode == null)
                return PaperNumber;
            else if(PaperNumber != null && RecieptType != null && CollectionMainCode == null)
                return PaperNumber;
            else
                return RecieptType;

            return DocumentId;
        }
        private int getId(int? ParentTypeId, string? PaperNumber, string? RecieptType, int? CollectionMainCode,int elemId,int? parentId)
        {
            int Id = 0;
            if (ParentTypeId == (int)Enums.DocumentType.CustomerFunds || ParentTypeId == (int)Enums.DocumentType.SuplierFunds)
                return parentId??0;
            if (ParentTypeId == (int)Enums.DocumentType.AcquiredDiscount || ParentTypeId == (int)Enums.DocumentType.PermittedDiscount)
                return parentId ?? 0;
            if (PaperNumber != null && RecieptType == null && CollectionMainCode == null)
                return parentId??0;
            else if (PaperNumber != null && RecieptType != null && CollectionMainCode == null)
                return parentId ?? 0;
            else
                return elemId;

            return Id;
        }
        private bool isReciepts(int? ParentTypeId, string? PaperNumber, string? RecieptType, int? CollectionMainCode)
        {
            bool _isInvoice = false;
            if (ParentTypeId == (int)Enums.DocumentType.CustomerFunds || ParentTypeId == (int)Enums.DocumentType.SuplierFunds)
                return _isInvoice;
            if (PaperNumber != null && RecieptType == null && CollectionMainCode == null)
                return _isInvoice;
            else if (PaperNumber != null && RecieptType != null && CollectionMainCode == null)
                return _isInvoice;
            else
                _isInvoice = true;

            return _isInvoice;
        }
        private int getInvoiceTypeId(int? ParentTypeId, string? PaperNumber, string? RecieptType, int? CollectionMainCode,int RecieptTypeId)
        {
            int InvoiceTypeId = 0;
            if (ParentTypeId == (int)Enums.DocumentType.CustomerFunds || ParentTypeId == (int)Enums.DocumentType.SuplierFunds)
                return ParentTypeId ?? 0;
            if (PaperNumber != null && RecieptType == null && CollectionMainCode == null)
                return ParentTypeId ?? 0;
            else if (PaperNumber != null && RecieptType != null && CollectionMainCode == null)
                return ParentTypeId ?? 0;
            else
                return RecieptTypeId;

            return InvoiceTypeId;
        }
        #endregion
        public async Task<ResponseResult> GetPersonAccountData(SupplierAccountRequest request, bool isPrint = false)
        {
            var userInfo = await _userInformation.GetUserInformation();
            // data in period
            Tuple<bool, string> checkData = checkDataValidation(request);

            if (!checkData.Item1)
            {
                return new ResponseResult { Data = null, Result = Result.RequiredData, Note = checkData.Item2 };
            }
            var isCustomer = await _personHelperService.checkIsCustomer(request.personId);
            //if(int.Parse(checkDataValidation(request).ToString())== 0) 
            var data = RecieptQuery
                         .TableNoTracking
                         .Include(c=> c.person)
                         .Where(x => !isCustomer ? (!userInfo.otherSettings.purchasesShowOtherPersonsInv ? x.UserId == userInfo.employeeId : true) : (x.ParentTypeId == (int)Enums.DocumentType.POS || x.ParentTypeId == (int)Enums.DocumentType.ReturnPOS) ? (!userInfo.otherSettings.posShowOtherPersonsInv ? x.UserId == userInfo.employeeId : true) : (!userInfo.otherSettings.salesShowOtherPersonsInv ? x.UserId == userInfo.employeeId : true))
                         .Where(a => a.PersonId == request.personId && !a.IsBlock && (a.Creditor + a.Debtor) != 0)
                         .Where(a => request.Branches.Contains(a.BranchId))
                         .Where(a => !request.PaidPurchase ? (a.Debtor != a.Creditor || a.isPartialPaid == (int)PaymentType.Partial) : true)
                         //.Where(c => c.RecieptDate.Date <= request.DateTo.Date)
                         .OrderBy(x => x.CreationDate.Date)
                         .ThenBy(x => x.Serialize)
                         .ToList();

            // var settings = _generalSettingsRepositoryQuery.TableNoTracking.FirstOrDefault();

            // Reciept
            var Part1 = data
                         .Where(a => a.CreationDate.Date >= request.DateFrom.Date && a.CreationDate.Date <= request.DateTo.Date)
                         .Where(c => c.ParentTypeId == null)
                         .GroupBy(keySelector: e => new { e.RecieptType, e.Code })
                         .Select(a => new SupplierAccountList()
                         {
                             Id = a.First().Id,
                             docTypeId = GetdocTypeId(true, a.First().RecieptTypeId,a.First().RecieptTypeId),
                             code = a.First().Code,
                             Serialize = a.First().Serialize,
                             DocumentId = a.First().ParentTypeId == (int)Enums.DocumentType.CustomerFunds || a.First().ParentTypeId == (int)Enums.DocumentType.SuplierFunds ? "" : a.First().RecieptType,
                             DebtorTrans = roundNumbers.GetRoundNumber(a.Sum(e => e.Debtor)),
                             CreditorTrans = roundNumbers.GetRoundNumber(a.Sum(e => e.Creditor)),
                             DebtorBalance = 0,
                             CreditorBalance = 0,
                             collectionSubType = a.First().SubTypeId,
                             InvoiceTypeId = !string.IsNullOrEmpty(a.First().ParentTypeId.ToString()) ? a.First().ParentTypeId.Value : a.First().RecieptTypeId,
                             Signal = a.First().Signal,
                             Transactiondate = a.First().RecieptDate.ToString("yyyy-MM-dd"),
                             Notes = a.First().Code == 0 ? a.First().InvoiceNotes : a.First().Notes,
                             pageId =  GetPageId(a.First().RecieptTypeId),
                             CreationDate = a.First().CreationDate
                         });
            //Invoices
            var Part2 = data
                        .Where(a => a.CreationDate.Date >= request.DateFrom.Date && a.CreationDate.Date <= request.DateTo.Date)
                        .Where(c => c.ParentTypeId != null)
                        .GroupBy(e => new { e.ParentTypeId,e.Serialize,e.CollectionCode})
                        .Select(a => new SupplierAccountList()
                        {
                            Id = a.First().RecieptTypeId == (int)Enums.DocumentType.SafeMultiCollectionReceipts || a.First().RecieptTypeId == (int)Enums.DocumentType.BankMultiCollectionReceipts  ? a.First().MultiCollectionReceiptParentId??0 : getId(a.Last().ParentTypeId, a.Last().PaperNumber, a.Last().RecieptType, a.Last().CollectionMainCode,a.Last().Id,a.Last().ParentId),
                            docTypeId = GetdocTypeId(isReciepts(a.Last().ParentTypeId, a.Last().PaperNumber, a.Last().RecieptType, a.Last().CollectionMainCode), a.Last().ParentTypeId??0, a.First().RecieptTypeId),
                            parentId = a.First().ParentId??0,
                            recTypeId = a.First().RecieptTypeId,
                            code = a.Last().Code,
                            Serialize = a.Last().Serialize,
                            DocumentId = GetDocumentId(a.Last().ParentTypeId,a.Last().PaperNumber,a.Last().RecieptType,a.Last().CollectionMainCode,a.First().RecieptTypeId),
                            PaperNumber = a.First().RecieptTypeId == (int)Enums.DocumentType.SafeMultiCollectionReceipts || a.First().RecieptTypeId == (int)Enums.DocumentType.BankMultiCollectionReceipts ? a.First().RecieptType : a.Last().PaperNumber,

                            DebtorTrans = roundNumbers.GetRoundNumber(a.Sum(e => e.Debtor)),
                            CreditorTrans = roundNumbers.GetRoundNumber(a.Sum(e => e.Creditor)),

                            DebtorBalance = 0,
                            CreditorBalance = 0,
                            collectionSubType = a.Last().SubTypeId,
                            InvoiceTypeId = getInvoiceTypeId(a.Last().ParentTypeId, a.Last().PaperNumber, a.Last().RecieptType, a.Last().CollectionMainCode, a.Last().RecieptTypeId),
                            Signal = a.Last().Signal,
                            Transactiondate = a.Last().RecieptDate.ToString("yyyy-MM-dd"),
                            Notes = a.Last().Code == 0 ? a.Last().InvoiceNotes : a.Last().Notes,
                            pageId = GetPageId(isReciepts(a.Last().ParentTypeId, a.Last().PaperNumber, a.Last().RecieptType, a.Last().CollectionMainCode) 
                            || (a.First().RecieptTypeId == (int)Enums.DocumentType.SafeMultiCollectionReceipts || a.First().RecieptTypeId == (int)Enums.DocumentType.BankMultiCollectionReceipts) ?
                            a.Last().RecieptTypeId : a.Last().ParentTypeId.Value),
                            CreationDate = a.Last().CreationDate,
                            isCustomer = a.Last().person.IsCustomer,
                            InvoiceTypeAr = a.First().NoteAR,
                            InvoiceTypeEn = a.First().NoteEN
                        });



            var AllData = Part1.Union(Part2).ToList()
                        .OrderBy(a => a.Transactiondate)
                        .ThenBy(a => a.CreationDate.ToString("HHmmss"))
                        //.ThenBy(a => a.Serialize)
                        .GroupBy(a => new { a.DocumentId, a.InvoiceTypeId, a.code ,a.parentId});



            // previous Data
            var prev = data. Where(a => a.RecieptDate.Date < request.DateFrom.Date)
                            .GroupBy(e => new { e.Serialize }).Select(a => new SupplierAccountList()
                            {
                                //Serialize = a.Key.Serialize,
                                DebtorTrans = roundNumbers.GetRoundNumber(a.Sum(e => e.Debtor)),
                                CreditorTrans = roundNumbers.GetRoundNumber(a.Sum(e => e.Creditor))
                            });


            var resDataList = new List<SupplierAccountList>();
            double TotalBalance = 0;


            var invoicestransaction = TransactionTypeList.transactionTypeModels();
            if (prev.Count() > 0)
            {
                var resData = new SupplierAccountList();
                resData.InvoiceTypeAr = invoicestransaction.Where(c => c.id == 0).FirstOrDefault()?.arabicName ?? "";
                resData.InvoiceTypeEn = invoicestransaction.Where(c => c.id == 0).FirstOrDefault()?.latinName ?? "";
                resData.DebtorTrans = roundNumbers.GetRoundNumber(prev.Sum(a => a.DebtorTrans));
                resData.CreditorTrans = roundNumbers.GetRoundNumber(prev.Sum(a => a.CreditorTrans));
                TotalBalance += resData.CreditorTrans - resData.DebtorTrans;

                if (TotalBalance > 0)
                    resData.CreditorBalance = roundNumbers.GetRoundNumber(TotalBalance);
                else
                    resData.DebtorBalance = roundNumbers.GetRoundNumber(-TotalBalance);
                resDataList.Add(resData);
            }


            //var invoicestransaction = TransactionTypeList.transactionTypeModels();
            var returnInvoicesList = Lists.returnInvoiceList;
            foreach (var item in AllData.ToList())
            {
                var resData = new SupplierAccountList();
                //resData.Serialize = item.First().Serialize;

                resData.DebtorTrans = item.First().DebtorTrans;
                resData.CreditorTrans = item.First().CreditorTrans;

                resData.DebtorBalance = item.First().DebtorBalance;
                resData.CreditorBalance = item.First().CreditorBalance;

                TotalBalance += resData.CreditorTrans - resData.DebtorTrans;
                if (TotalBalance > 0)
                    resData.CreditorBalance = roundNumbers.GetRoundNumber(TotalBalance);
                else
                    resData.DebtorBalance = roundNumbers.GetRoundNumber(-TotalBalance);

                resData.Transactiondate = item.First().Transactiondate;
                resData.DocumentId = item.First().DocumentId;
                resData.InvoiceTypeId = item.First().recTypeId == (int)Enums.DocumentType.SafeMultiCollectionReceipts || item.First().recTypeId == (int)Enums.DocumentType.BankMultiCollectionReceipts ? item.First().recTypeId : item.First().InvoiceTypeId;
                resData.Signal = item.First().Signal;
                resData.Notes = item.First().Notes;
                resData.rowClassName = returnInvoicesList.Where(c => c == item.First().InvoiceTypeId).Any() ? "text-danger" : "";
                resData.Id = item.First().Id;
                resData.docTypeId = item.First().docTypeId;
                resData.pageId = item.First().pageId;

                if (resData.InvoiceTypeId != (int)Enums.DocumentType.SafeMultiCollectionReceipts && resData.InvoiceTypeId != (int)Enums.DocumentType.BankMultiCollectionReceipts)
                {
                    resData.InvoiceTypeAr = invoicestransaction.Where(c => c.id == item.First().InvoiceTypeId).FirstOrDefault()?.arabicName ?? "";
                    resData.InvoiceTypeEn = invoicestransaction.Where(c => c.id == item.First().InvoiceTypeId).FirstOrDefault()?.latinName ?? "";
                    //resData.InvoiceTypeAr += TransactionTypeList.collectionRec.Where(c => c.id == item.First().collectionSubType).FirstOrDefault()?.arabicName ?? "";
                    //resData.InvoiceTypeEn += TransactionTypeList.collectionRec.Where(c => c.id == item.First().collectionSubType).FirstOrDefault()?.latinName ?? "";
                }
                else
                {
                    resData.InvoiceTypeAr = item.First().InvoiceTypeAr;
                    resData.InvoiceTypeEn = item.First().InvoiceTypeEn;
                }


                resData.isCustomer = item.First().isCustomer;
                resDataList.Add(resData);
            }



            var FinalResult = new List<SupplierAccountList>();

            //pagenation
            if (!isPrint)
            {
                FinalResult = Pagenation<SupplierAccountList>.pagenationList(request.PageSize, request.PageNumber, resDataList);
            }
            else
            {
                FinalResult = resDataList;
            }

            var totalData = new SupplierAccountResponse();

            totalData.SupplierAccountData = FinalResult;



            //الرصيد الفعلي
            if (resDataList.Count() > 0)
            {
                var debtor = roundNumbers.GetRoundNumber(data.Sum(a => a.Debtor));
                var Creditor = roundNumbers.GetRoundNumber(data.Sum(a => a.Creditor));
                var total = Creditor - debtor;
                totalData.ActualDebtorTrans = roundNumbers.GetRoundNumber(debtor);
                totalData.ActualCreditorTrans = roundNumbers.GetRoundNumber(Creditor);

                totalData.ActualDebtorBalance = total < 0 ? roundNumbers.GetRoundNumber(Math.Abs(total)) : 0;
                totalData.ActualCreditorBalance = total > 0 ? roundNumbers.GetRoundNumber(total) : 0;
            }



            // الرصيد عن فترة
            //var x=  AllData.Count();
            totalData.PeriodDebtorTrans = roundNumbers.GetRoundNumber(AllData.Sum(a => a.First().DebtorTrans));
            totalData.PeriodCreditorTrans = roundNumbers.GetRoundNumber(AllData.Sum(a => a.First().CreditorTrans));
            var PeriodBalance = roundNumbers.GetRoundNumber(totalData.PeriodCreditorTrans - totalData.PeriodDebtorTrans);
            if (PeriodBalance > 0)
                totalData.PeriodCreditorBalance = PeriodBalance;
            else
                totalData.PeriodDebtorBalance = -PeriodBalance;

            if (request.PageNumber * request.PageSize >= AllData.Count())
                return new ResponseResult { Data = totalData, Id = null, Result = resDataList.Any() ? Result.Success : Result.NotFound, DataCount = resDataList.Count(), Note = "End of data" };

            return new ResponseResult { Data = totalData, Id = null, Result = resDataList.Any() ? Result.Success : Result.NotFound, DataCount = totalData.SupplierAccountData.Count(), TotalCount = resDataList.Count() };
        }

        public async Task<WebReport> GetPersonAccountDataReport(SupplierAccountRequest request, exportType exportType, bool isArabic, int fileId = 0)
        {
            var additionalData = RecieptQuery
                        .TableNoTracking.Where(a => a.PersonId == request.personId).Include(b => b.person).Select(a => new
                        {
                            personid = a.PersonId,
                            personnameAr = a.person.ArabicName,
                            personnameEn = a.person.LatinName,

                            personcode = a.person.Code,
                            isSupplier = a.person.IsSupplier,
                        }).FirstOrDefault();
            var data = await GetPersonAccountData(request, true);

            var userInfo = await _iUserInformation.GetUserInformation();

            var otherdata = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, request.DateFrom, request.DateTo);


            otherdata.Id = additionalData.personid;
            otherdata.ArabicName = additionalData.personnameAr;
            otherdata.LatinName = additionalData.personnameEn;


            otherdata.Code = additionalData.personcode.ToString();
            otherdata.EmployeeName = userInfo.employeeNameAr.ToString();
            otherdata.EmployeeNameEn = userInfo.employeeNameEn.ToString();


            //if (isArabic)
            //{
            //    otherdata.DateFrom = request.DateFrom.ToString("yyyy/MM/dd");
            //    otherdata.DateTo = request.DateTo.ToString("yyyy/MM/dd");
            //    otherdata.Date = DateTime.Now.ToString("yyyy/MM/dd");
            //}
            //else
            //{
            //    otherdata.DateFrom = request.DateFrom.ToString("dd/MM/yyyy");
            //    otherdata.DateTo = request.DateTo.ToString("dd/MM/yyyy");
            //    otherdata.Date = DateTime.Now.ToString("dd/MM/yyyy");
            //}
            int screenId = 0;
            if (additionalData.isSupplier)
            {
                screenId = (int)SubFormsIds.SupplierStatement_Purchases;

            }
            else
            {
                screenId = (int)SubFormsIds.CustomerStatement_Sales;
            }

            var tablesNames = new TablesNames()
            {

                ObjectName = "SupplierAccountResponse",
                FirstListName = "SuplierAcountList"
            };


            var resData = (SupplierAccountResponse)(data.Data);

            var report = await _iGeneralPrint.PrintReport<SupplierAccountResponse, SupplierAccountList, object>(resData, resData.SupplierAccountData, null, tablesNames, otherdata
             , screenId, exportType, isArabic, fileId);
            return report;


        }

    }
}
