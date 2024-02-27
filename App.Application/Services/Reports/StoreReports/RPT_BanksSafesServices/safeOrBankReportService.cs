using App.Application.Services.Printing.InvoicePrint;
using App.Application.Services.Process.Invoices;
using App.Domain.Models.Request.Store.Reports;
using App.Domain.Models.Response.Store;
using App.Domain.Models.Response.Store.Reports;
using App.Infrastructure.settings;
//using DocumentFormat.OpenXml.Drawing.Charts;

//using DocumentFormat.OpenXml.Wordprocessing;
using static App.Application.Services.Reports.StoreReports.Sales.RPT_Sales;

namespace App.Application.Services.Reports.StoreReports.RPT_BanksSafesServices
{
    public class safeOrBankReportService : iSafeOrBankReportService
    {
        private readonly IRepositoryQuery<GlReciepts> _glRecieptsQuery;
        private readonly IRepositoryQuery<InvPersons> _invPersonsQuery;
        private readonly IRepositoryQuery<InvSalesMan> _invSalesManQuery;
        private readonly IRepositoryQuery<GLOtherAuthorities> _gLOtherAuthoritiesQuery;
        private readonly IRepositoryQuery<GLFinancialAccount> _gLFinancialAccountQuery;
        private readonly IRoundNumbers _roundNumbers;

        private readonly IPrintService _iprintService;

        private readonly IFilesMangerService _filesMangerService;
        private readonly ICompanyDataService _CompanyDataService;
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<GLBank> _GLBankQuery;
        private readonly IRepositoryQuery<GLSafe> _GLSafeQuery;
        private readonly IGeneralPrint _iGeneralPrint;


        // alaa test
        public safeOrBankReportService(
                                       IRepositoryQuery<GlReciepts> GlRecieptsQuery,
                                       IRepositoryQuery<InvPersons> InvPersonsQuery,
                                       IRepositoryQuery<InvSalesMan> InvSalesManQuery,
                                       IRepositoryQuery<GLOtherAuthorities> GLOtherAuthoritiesQuery,
                                       IRepositoryQuery<GLFinancialAccount> GLFinancialAccountQuery,
                                       IRoundNumbers roundNumbers,
                                       IPrintService iprintService,
                                       IRepositoryQuery<GLBank> GLBankQuery,
                                       IRepositoryQuery<GLSafe> GLSafeQuery,

                                       IFilesMangerService filesMangerService,
                                       ICompanyDataService CompanyDataService,
                                       iUserInformation iUserInformation
,
                                       IGeneralPrint iGeneralPrint)
        {
            _glRecieptsQuery = GlRecieptsQuery;
            _invPersonsQuery = InvPersonsQuery;
            _invSalesManQuery = InvSalesManQuery;
            _gLOtherAuthoritiesQuery = GLOtherAuthoritiesQuery;
            _gLFinancialAccountQuery = GLFinancialAccountQuery;
            _roundNumbers = roundNumbers;
            _iprintService = iprintService;

            _filesMangerService = filesMangerService;
            _CompanyDataService = CompanyDataService;
            _iUserInformation = iUserInformation;
            _GLBankQuery = GLBankQuery;
            _GLSafeQuery = GLSafeQuery;
            _iGeneralPrint = iGeneralPrint;
        }




        public async Task<ResponseResult> BanksOrSafeAccountStatement(safesRequestDTO parm)
        {
            var res = await getBanksOrSafeAccountStatement(parm);
            return new ResponseResult()
            {
                Data = res.data,
                Result = res.Result,
                TotalCount = res.totalCount,
                DataCount = res.dataCount,
                Note = res.notes
            };
        }
        public async Task<WebReport> BanksOrSafeAccountStatementReport(safesRequestDTO param, exportType exportType, bool isArabic, int fileId)
        {
            var data = await getBanksOrSafeAccountStatement(param, true);

            var userInfo = await _iUserInformation.GetUserInformation();

            string arabicName;
            string latinName;
            int screenId = 0;
            if (param.isSafe)
            {
                var additionalData = await _GLSafeQuery.GetByIdAsync(param.id);
                arabicName = additionalData.ArabicName;
                latinName = additionalData.LatinName;

                screenId = (int)SubFormsIds.SafeAccountStatement;
            }

            else
            {
                var additionalData = await _GLBankQuery.GetByIdAsync(param.id);
                arabicName = additionalData.ArabicName;
                latinName = additionalData.LatinName;
                screenId = (int)SubFormsIds.BankAccountStatement;

            }

            var dates = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, param.dateFrom, param.dateTo);

            var otherdata = new AdditionalData()
            {
                ArabicName = arabicName,
                LatinName = latinName,
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),
                DateFrom = dates.DateFrom,
                DateTo = dates.DateTo,
                Date = dates.Date

            };

            var tablesNames = new TablesNames()
            {
                ObjectName = "bankAndSafesResponse",
                FirstListName = "bankAndSafesResponseList"
            };

            var report = await _iGeneralPrint.PrintReport<bankAndSafesResponseDTO, bankAndSafesResponseData, object>(data.data, data.data.data, null, tablesNames, otherdata
             , screenId, exportType, isArabic, fileId);
            return report;
        }
        //private Tuple<string,string> getBenfit(int ParentTypeId,int parmId,int safeId,)
        //{
        //    string benfitAr = string.Empty;
        //    string benfitEn = string.Empty;

        //    if (ParentTypeId == (int)Enums.DocumentType.SafeFunds || ParentTypeId == (int)Enums.DocumentType.BankFunds)
        //    {
        //        benfitAr = "أرصدة اول المدة";
        //        benfitEn = "Entry Fund";
        //    }else if(parmId == safeId)
        //    {

        //    }

        //    return null;
        //}

        private string getPaymentTypeAr(string paymentTypeAr, int Id, int bankOrSafeId, int RecieptTypeId)
        {
            if (RecieptTypeId == (int)Enums.DocumentType.SafeTransfer || RecieptTypeId == (int)Enums.DocumentType.BankTransfer)
                if (bankOrSafeId == Id)
                    paymentTypeAr = paymentTypeAr + " - " + "صادر";
                else
                    paymentTypeAr = paymentTypeAr + " - " + "وارد";
            return paymentTypeAr;
        }
        private string getPaymentTypeEn(string paymentTypeEn, int Id, int bankOrSafeId, int RecieptTypeId)
        {
            if (RecieptTypeId == (int)Enums.DocumentType.SafeTransfer || RecieptTypeId == (int)Enums.DocumentType.BankTransfer)
                if (bankOrSafeId == Id)
                    paymentTypeEn = paymentTypeEn + " - " + "Outbound";
                else
                    paymentTypeEn = paymentTypeEn + " - " + "Inbound";
            return paymentTypeEn;
        }
        public async Task<bankAndSafesResponse> getBanksOrSafeAccountStatement(safesRequestDTO parm, bool isPrint = false)
        {
            var branches = parm.branches.Split(',').Select(x => int.Parse(x)).ToList();
            var returnsList = Lists.returnInvoiceList;

            var transactionTypes = TransactionTypeList.transactionTypeModels();


            var _recs = await getRecieptsAsync(parm, withoutDateFilter: true);
            var recs = _recs.ToList()
                            .Where(c => c.RecieptDate.Date <= parm.dateTo)
                            .GroupBy(x => new { x.RecieptType, x.PaymentMethodId })
                            .Select(x => new bankAndSafesResponseData
                            {
                                Id = x.First().Id,
                                date = x.First().RecieptDate,
                                cdate = x.First().CreationDate,
                                documentCode = x.First().RecieptType,
                                documentTypeAr = transactionTypes.Where(c => c.id.ToString() == x.First().RecieptTypeId.ToString()).FirstOrDefault().arabicName,
                                documentTypeEn = transactionTypes.Where(c => c.id.ToString() == x.First().RecieptTypeId.ToString()).FirstOrDefault().latinName,
                                //from list
                                subTypeId = x.First().SubTypeId != 0 ? x.First().SubTypeId : 0,
                                benfitAr = x.First().ParentTypeId == (int)Enums.DocumentType.SafeFunds || x.First().ParentTypeId == (int)Enums.DocumentType.BankFunds ? "أرصدة اول المدة"
                                            : (parm.id == x.First().SafeID || parm.id == x.First().BankId) ? benefitTypeName(x.First().Authority, x.First().BenefitId, false, x.First().RecieptTypeId, parm.isSafe, parm.id, x.First().BankId, x.First().SafeID).Result.Item1
                                            : benefitTypeName(x.FirstOrDefault().SafeID != null ? AuthorityTypes.Safe : AuthorityTypes.Bank, x.FirstOrDefault().SafeID != null ? x.First().SafeID.Value : x.First().BankId.Value, true, x.First().RecieptTypeId, parm.isSafe, parm.id, x.First().BankId, x.First().SafeID).Result.Item1,
                                benefitEn = x.First().ParentTypeId == (int)Enums.DocumentType.SafeFunds || x.First().ParentTypeId == (int)Enums.DocumentType.BankFunds ? "أرصدة اول المدة"
                                            : (parm.id == x.First().SafeID || parm.id == x.First().BankId) ? benefitTypeName(x.First().Authority, x.First().BenefitId, false, x.First().RecieptTypeId, parm.isSafe, parm.id, x.First().BankId, x.First().SafeID).Result.Item2
                                            : benefitTypeName(x.FirstOrDefault().SafeID != null ? AuthorityTypes.Safe : AuthorityTypes.Bank, x.FirstOrDefault().SafeID != null ? x.First().SafeID.Value : x.First().BankId.Value, true, x.First().RecieptTypeId, parm.isSafe, parm.id, x.First().BankId, x.First().SafeID).Result.Item2,

                                paymentTypeAr = getPaymentTypeAr(x.First().PaymentMethods.ArabicName, parm.id, parm.isSafe ? x.First().SafeID ?? 0 : x.First().BankId ?? 0, x.First().RecieptTypeId),
                                paymentTypeEn = getPaymentTypeAr(x.First().PaymentMethods.LatinName, parm.id, parm.isSafe ? x.First().SafeID ?? 0 : x.First().BankId ?? 0, x.First().RecieptTypeId),
                                Notes = x.First().Notes,
                                debtorTransaction = GetDebitAmount(x.First().RecieptTypeId, Math.Abs(x.First().Amount), x.First().Authority, x.First().SafeID, x.First().BankId, parm.id, x.First().BenefitId, parm.isSafe),
                                creditorTransaction = GetCreditorAmount(x.First().RecieptTypeId, Math.Abs(x.First().Amount), x.First().Authority, x.First().SafeID, x.First().BankId, parm.id, x.First().BenefitId, parm.isSafe),
                                //rowClassName = returnsList.Contains(x.First().ParentTypeId??0)? defultData.text_danger : ""
                                docTypeId = x.First().RecieptTypeId,
                                Serialize = x.First().Serialize
                            }).ToList();
            var ActualBalance = recs.Sum(x => x.creditorTransaction) - recs.Sum(x => x.debtorTransaction);

            double resBalance = 0;
            var BalancedataBeforePeriod = recs.Where(x => x.date.Value.Date < parm.dateFrom).ToList().Sum(x => x.creditorTransaction - x.debtorTransaction);
            var dataOfDate = new List<bankAndSafesResponseData>();
            dataOfDate = recs.Where(x => x.date.Value.Date >= parm.dateFrom && x.date.Value.Date <= parm.dateTo).ToList();
            var PeriodBalance = dataOfDate.Sum(x => x.creditorTransaction) - dataOfDate.Sum(x => x.debtorTransaction);
            var totalDebtorTransactionOfOfPeriod = _roundNumbers.GetRoundNumber(dataOfDate.Sum(x => Math.Abs(x.debtorTransaction)));
            var totalCreditorTransactionOfPeriod = _roundNumbers.GetRoundNumber(dataOfDate.Sum(x => Math.Abs(x.creditorTransaction)));
            var totalDebtorBalanceOfPeriod = PeriodBalance < 0 ? _roundNumbers.GetRoundNumber(Math.Abs(PeriodBalance)) : 0;
            var totalCreditorBalanceOfPeriod = PeriodBalance > 0 ? _roundNumbers.GetRoundNumber(Math.Abs(PeriodBalance)) : 0;

            if (Math.Abs(BalancedataBeforePeriod) > 0)
                dataOfDate.Add(new bankAndSafesResponseData()
                {
                    documentTypeAr = transactionTypes.Where(c => c.id == 0).FirstOrDefault().arabicName,
                    documentTypeEn = transactionTypes.Where(c => c.id == 0).FirstOrDefault().latinName,
                    debtorTransaction = BalancedataBeforePeriod < 0 ? _roundNumbers.GetRoundNumber(Math.Abs(BalancedataBeforePeriod)) : 0,
                    creditorTransaction = BalancedataBeforePeriod > 0 ? _roundNumbers.GetRoundNumber(BalancedataBeforePeriod) : 0
                });


            dataOfDate = dataOfDate
                .OrderBy(x => x.date.GetValueOrDefault().Date)
                .ThenByDescending(x => x.cdate)
                .ToList();



            dataOfDate.ForEach(x => 
            {
                var balance = x.creditorTransaction - x.debtorTransaction;
                resBalance += balance;

                x.creditorBalance += resBalance > 0 ? _roundNumbers.GetRoundNumber(Math.Abs(resBalance)) : 0;
                x.debtorBalance += resBalance < 0 ? _roundNumbers.GetRoundNumber(Math.Abs(resBalance)) : 0;
            });
            var data = !isPrint ? Pagenation<bankAndSafesResponseData>.pagenationList(parm.pageSize, parm.pageNumber, dataOfDate) : dataOfDate;
            double MaxPageNumber = recs.ToList().Count() / Convert.ToDouble(parm.pageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);


            var response = new bankAndSafesResponseDTO()
            {
                data = data,

                totalDebtorTransactionOfOfPeriod = totalDebtorTransactionOfOfPeriod,
                totalCreditorTransactionOfPeriod = totalCreditorTransactionOfPeriod,
                totalDebtorBalanceOfPeriod = totalDebtorBalanceOfPeriod,
                totalCreditorBalanceOfPeriod = totalCreditorBalanceOfPeriod,


                TotalActualDebtorTransaction = _roundNumbers.GetRoundNumber(dataOfDate.Sum(x => x.debtorTransaction)),
                TotalActualCreditorTransaction = _roundNumbers.GetRoundNumber(dataOfDate.Sum(x => x.creditorTransaction)),
                TotalActualDebtorBalance = ActualBalance < 0 ? _roundNumbers.GetRoundNumber(Math.Abs(ActualBalance)) : 0,
                TotalActualCreditorBalance = ActualBalance > 0 ? _roundNumbers.GetRoundNumber(Math.Abs(ActualBalance)) : 0,
            };
            return new bankAndSafesResponse()
            {
                data = response,
                notes = (countofFilter == parm.pageNumber ? Actions.EndOfData : ""),
                Result = Result.Success,
                totalCount = recs.Count(),
                dataCount = dataOfDate.Count()
            };
        }
        private double GetDebitAmount(int RecieptTypeId, double eleAmount, int Authority, int? safeId, int? bankId, int parmid, int BenefitId, bool isSafe)
        {
            double amount = 0;
            if (RecieptTypeId == (int)Enums.DocumentType.BankCash || RecieptTypeId == (int)Enums.DocumentType.SafeCash)
                return eleAmount;
            else if ((RecieptTypeId == (int)Enums.DocumentType.SafeMultiCollectionReceipts || RecieptTypeId == (int)Enums.DocumentType.BankMultiCollectionReceipts) && Authority == (int)Enums.AuthorityTypes.customers)
            {
                return eleAmount;
            }
            else if (RecieptTypeId == (int)Enums.DocumentType.BankTransfer)
            {
                if (isSafe)
                {
                    if (Authority == (int)AuthorityTypes.Safe)
                        if(BenefitId == parmid)
                            return eleAmount;
                    
                }
                else
                {
                    if (Authority == (int)AuthorityTypes.Bank)
                        if (BenefitId == parmid)
                            return eleAmount;
                }


            }
            else if (RecieptTypeId == (int)Enums.DocumentType.SafeTransfer)
            {
                if (isSafe)
                {
                    if (Authority == (int)AuthorityTypes.Safe)
                    {
                        if (BenefitId == parmid)
                            return eleAmount;
                    }
                }
                else
                {
                    if (Authority == (int)AuthorityTypes.Bank)
                        if (BenefitId == parmid)
                            return eleAmount;
                }
                
            }
            return amount;
        }
        private double GetCreditorAmount(int RecieptTypeId, double eleAmount, int Authority, int? safeId, int? bankId, int parmid, int BenefitId, bool isSafe)
        {
            double amount = 0;
            if (RecieptTypeId == (int)Enums.DocumentType.BankPayment || RecieptTypeId == (int)Enums.DocumentType.SafePayment)
                return eleAmount;
            else if ((RecieptTypeId == (int)Enums.DocumentType.SafeMultiCollectionReceipts || RecieptTypeId == (int)Enums.DocumentType.BankMultiCollectionReceipts) && Authority == (int)Enums.AuthorityTypes.suppliers)
            {
                return eleAmount;
            }
            else if (RecieptTypeId == (int)Enums.DocumentType.BankTransfer || RecieptTypeId == (int)Enums.DocumentType.SafeTransfer)
            {
                if (isSafe)
                {
                    if (RecieptTypeId == (int)Enums.DocumentType.SafeTransfer)
                        if (safeId == parmid)
                            return eleAmount;
                        
                }
                else
                {
                    if (RecieptTypeId == (int)Enums.DocumentType.BankTransfer)
                        if (bankId == parmid)
                            return eleAmount;
                }

            }
           
            return amount;
        }

        public async Task<ResponseResult> ExpensesAndReceipts(safesRequestDTO parm, bool isCash)
        {
            var res = await getExpensesAndReceipts(parm, isCash);
            return new ResponseResult()
            {
                Data = res.Data,
                Result = res.Result,
                TotalCount = res.totalCount,
                DataCount = res.dataCount,
                Note = res.notes
            };
        }
        public async Task<ExpensesAndReceiptsResponse> getExpensesAndReceipts(safesRequestDTO parm, bool isCash, bool isPrint = false)
        {
            var branches = parm.branches.Split(',').Select(x => int.Parse(x)).ToList();
            var recs = await getRecieptsAsync(parm);
            if (parm.isSafe)
            {
                if (isCash)
                {
                    recs = recs.Where(x => (x.RecieptTypeId == (int)DocumentType.SafeCash || (x.RecieptTypeId == (int)DocumentType.SafeMultiCollectionReceipts && x.Authority == (int)Enums.AuthorityTypes.customers)) || (x.Authority == AuthorityTypes.Safe && x.BenefitId == parm.id)).ToList();
                }
                else
                {
                    recs = recs.Where(x => (x.RecieptTypeId == (int)DocumentType.SafePayment || (x.RecieptTypeId == (int)DocumentType.SafeMultiCollectionReceipts && x.Authority == (int)Enums.AuthorityTypes.suppliers)) || (x.RecieptTypeId == (int)DocumentType.SafeTransfer && x.SafeID == parm.id)).ToList();
                }
            }
            else
            {
                if (isCash)
                {
                    recs = recs
                        .Where(x => (x.RecieptTypeId == (int)DocumentType.BankCash
                        || (x.RecieptTypeId == (int)DocumentType.BankMultiCollectionReceipts
                        && x.Authority == (int)Enums.AuthorityTypes.customers))
                        || (x.Authority == AuthorityTypes.Bank))
                        .ToList();
                }
                else
                {
                    recs = recs.Where(x => (x.RecieptTypeId == (int)DocumentType.BankPayment || (x.RecieptTypeId == (int)DocumentType.BankMultiCollectionReceipts && x.Authority == (int)Enums.AuthorityTypes.suppliers)) || x.RecieptTypeId == (int)DocumentType.BankTransfer).ToList();
                }
            }


            //var benfitAr = (recs.First().RecieptTypeId == (int)DocumentType.SafeCash) ? benefitTypeName(recs.First().Authority, recs.First().BenefitId).Result.Item1
            //                                                                                        : benefitTypeName(recs.First().SafeID.Value != null ? AuthorityTypes.Safe : AuthorityTypes.Bank,
            //                                                                                                           recs.First().SafeID.Value != null ? recs.First().SafeID.Value : recs.First().BankId.Value).Result.Item1;
            //var safeid = recs.First().SafeID != null ? recs.First().SafeID : 0;
            var _response = recs.GroupBy(x => new { x.RecieptType, x.PaymentMethodId })
                               .OrderBy(x => x.First().RecieptDate.Date)
                               .ThenBy(x => x.First().CreationDate.TimeOfDay);

            List<ExpensesAndReceiptsResponseData> response = new List<ExpensesAndReceiptsResponseData>();
            foreach (var x in _response)
            {
                var GetBenefitTypeName = !isCash ?
                    await benefitTypeName(x.First().Authority, x.First().BenefitId, false, x.First().RecieptTypeId, parm.isSafe, parm.id, x.First().BankId, x.First().SafeID, 1) :
                                    (x.First().RecieptTypeId == (int)DocumentType.SafeCash || x.First().RecieptTypeId == (int)DocumentType.BankCash || x.First().RecieptTypeId == (int)DocumentType.SafeMultiCollectionReceipts || x.First().RecieptTypeId == (int)DocumentType.BankMultiCollectionReceipts) ?
                    await benefitTypeName(x.First().Authority, x.First().BenefitId, false, x.First().RecieptTypeId, parm.isSafe, parm.id, x.First().BankId, x.First().SafeID, 2) :
                    await benefitTypeName(x.FirstOrDefault().SafeID != null ? AuthorityTypes.Safe : AuthorityTypes.Bank, x.FirstOrDefault().SafeID != null ? x.First().SafeID.Value : x.First().BankId.Value, true, x.First().RecieptTypeId, parm.isSafe, parm.id, x.First().BankId, x.First().SafeID, 3);

                response.Add(new ExpensesAndReceiptsResponseData
                {
                    Id = x.First().Id,
                    docTypeId = x.First().RecieptTypeId,
                    documentCode = x.First().RecieptType,
                    date = x.First().RecieptDate,
                    DocumentDate = x.First().RecieptDate.ToString("yyyy/MM/dd"),
                    benfitCode = GetBenefitTypeName.Item5,
                    benfitAr = GetBenefitTypeName.Item1,
                    benfitEn = GetBenefitTypeName.Item2,
                    benefitTypeAr = GetBenefitTypeName.Item3,
                    benefitTypeEn = GetBenefitTypeName.Item4,
                    paymentTypeAr = x.First().PaymentMethods != null ? x.First().PaymentMethods.ArabicName : "",
                    paymentTypeEn = x.First().PaymentMethods != null ? x.First().PaymentMethods.LatinName : "",
                    ChequesNum = x.First().ChequeNumber,
                    amount = Math.Abs(_roundNumbers.GetRoundNumber(x.Sum(c => c.Amount))),
                    serialize = x.First().Serialize
                });
            }

            #region old code
            //.Select(x => new ExpensesAndReceiptsResponseData
            //{
            //    Id = x.First().Id,
            //    docTypeId = x.First().RecieptTypeId,
            //    documentCode = x.First().RecieptType,
            //    date = x.First().RecieptDate,
            //    DocumentDate = x.First().RecieptDate.ToString("yyyy/MM/dd"),

            //    benfitCode = !isCash ? 
            //    benefitTypeName(x.First().Authority, x.First().BenefitId, false, x.First().RecieptTypeId, parm.isSafe, parm.id, x.First().BankId, x.First().SafeID).Result.Item5
            //                         : (x.First().RecieptTypeId == (int)DocumentType.SafeCash || x.First().RecieptTypeId == (int)DocumentType.BankCash) ? 
            //     benefitTypeName(x.First().Authority, x.First().BenefitId, false, x.First().RecieptTypeId, parm.isSafe, parm.id,x.First().BankId, x.First().SafeID).Result.Item5
            //    :benefitTypeName(x.FirstOrDefault().SafeID != null ? AuthorityTypes.Safe : AuthorityTypes.Bank, x.FirstOrDefault().SafeID != null ? x.First().SafeID.Value : x.First().BankId.Value, true, x.First().RecieptTypeId, parm.isSafe, parm.id, x.First().BankId, x.First().SafeID).Result.Item5,


            //    //benfitAr = benefitTypeName(x.First().Authority, x.First().BenefitId, false).Result.Item1,

            //    benfitAr = !isCash ? benefitTypeName(x.First().Authority, x.First().BenefitId, false, x.First().RecieptTypeId, parm.isSafe, parm.id, x.First().BankId, x.First().SafeID).Result.Item1
            //                         : (x.First().RecieptTypeId == (int)DocumentType.SafeCash || x.First().RecieptTypeId == (int)DocumentType.BankCash) ? benefitTypeName(x.First().Authority, x.First().BenefitId, false, x.First().RecieptTypeId, parm.isSafe, parm.id, x.First().BankId, x.First().SafeID).Result.Item1
            //                         : benefitTypeName(x.FirstOrDefault().SafeID != null ? AuthorityTypes.Safe : AuthorityTypes.Bank, x.FirstOrDefault().SafeID != null ? x.First().SafeID.Value : x.First().BankId.Value, true, x.First().RecieptTypeId, parm.isSafe, parm.id, x.First().BankId, x.First().SafeID).Result.Item1,


            //    benfitEn = !isCash ? benefitTypeName(x.First().Authority, x.First().BenefitId, false, x.First().RecieptTypeId, parm.isSafe, parm.id, x.First().BankId, x.First().SafeID).Result.Item2
            //                         : (x.First().RecieptTypeId == (int)DocumentType.SafeCash || x.First().RecieptTypeId == (int)DocumentType.BankCash) ? benefitTypeName(x.First().Authority, x.First().BenefitId, false, x.First().RecieptTypeId, parm.isSafe, parm.id, x.First().BankId, x.First().SafeID).Result.Item2
            //                         : benefitTypeName(x.FirstOrDefault().SafeID != null ? AuthorityTypes.Safe : AuthorityTypes.Bank, x.FirstOrDefault().SafeID != null ? x.First().SafeID.Value : x.First().BankId.Value, true, x.First().RecieptTypeId, parm.isSafe, parm.id, x.First().BankId, x.First().SafeID).Result.Item2,



            //    benefitTypeAr = !isCash ? benefitTypeName(x.First().Authority, x.First().BenefitId, false, x.First().RecieptTypeId, parm.isSafe, parm.id, x.First().BankId, x.First().SafeID).Result.Item3
            //                         : (x.First().RecieptTypeId == (int)DocumentType.SafeCash || x.First().RecieptTypeId == (int)DocumentType.BankCash) ? benefitTypeName(x.First().Authority, x.First().BenefitId, false, x.First().RecieptTypeId, parm.isSafe, parm.id, x.First().BankId, x.First().SafeID).Result.Item3
            //                         : benefitTypeName(x.FirstOrDefault().SafeID != null ? AuthorityTypes.Safe : AuthorityTypes.Bank, x.FirstOrDefault().SafeID != null ? x.First().SafeID.Value : x.First().BankId.Value, true, x.First().RecieptTypeId, parm.isSafe, parm.id, x.First().BankId, x.First().SafeID).Result.Item3,


            //    benefitTypeEn = !isCash ? benefitTypeName(x.First().Authority, x.First().BenefitId, false, x.First().RecieptTypeId, parm.isSafe, parm.id, x.First().BankId, x.First().SafeID).Result.Item4
            //                         : (x.First().RecieptTypeId == (int)DocumentType.SafeCash || x.First().RecieptTypeId == (int)DocumentType.BankCash) ? benefitTypeName(x.First().Authority, x.First().BenefitId, false, x.First().RecieptTypeId, parm.isSafe, parm.id, x.First().BankId, x.First().SafeID).Result.Item4
            //                         : benefitTypeName(x.FirstOrDefault().SafeID != null ? AuthorityTypes.Safe : AuthorityTypes.Bank, x.FirstOrDefault().SafeID != null ? x.First().SafeID.Value : x.First().BankId.Value, true, x.First().RecieptTypeId, parm.isSafe, parm.id, x.First().BankId, x.First().SafeID).Result.Item4,



            //    paymentTypeAr = x.First().PaymentMethods != null ? x.First().PaymentMethods.ArabicName : "",
            //    paymentTypeEn = x.First().PaymentMethods != null ? x.First().PaymentMethods.LatinName : "",


            //    ChequesNum = x.First().ChequeNumber,
            //    amount = _roundNumbers.GetRoundNumber(x.Sum(c => c.Amount)),
            //    serialize = x.First().Serialize
            //})
            //.ToList();
            #endregion

            var data = !isPrint ? Pagenation<ExpensesAndReceiptsResponseData>.pagenationList(parm.pageSize, parm.pageNumber, response) : response;
            double MaxPageNumber = recs.ToList().Count() / Convert.ToDouble(parm.pageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);

            var res = new ExpensesAndReceiptsResponseDTO()
            {
                total = _roundNumbers.GetRoundNumber(response.Sum(x => x.amount)),
                data = data
            };
            return new ExpensesAndReceiptsResponse()
            {
                Data = res,
                notes = (countofFilter == parm.pageNumber ? Actions.EndOfData : ""),
                Result = Result.Success,
                totalCount = response.Count(),
                dataCount = data.Count()
            };

        }
        public async Task<WebReport> SafeBankExpensesReceiptsReport(safesRequestDTO request, bool isCash, exportType exportType, bool isArabic, int fileId = 0)
        {
            var data = await getExpensesAndReceipts(request, isCash, true);

            var mainData = (ExpensesAndReceiptsResponseDTO)data.Data;

            var userInfo = await _iUserInformation.GetUserInformation();

            string safeOrBankAr = "";
            string safeOrBankEn = "";
            int screenId = 0;
            if (request.isSafe)
            {

                var safeOrBankData = _GLSafeQuery.TableNoTracking.Where(a => a.Id == request.id).FirstOrDefault();
                safeOrBankAr = safeOrBankData.ArabicName;
                safeOrBankEn = safeOrBankData.LatinName;

            }
            else
            {

                var safeOrBankData = _GLBankQuery.TableNoTracking.Where(a => a.Id == request.id).FirstOrDefault();
                safeOrBankAr = safeOrBankData.ArabicName;
                safeOrBankEn = safeOrBankData.LatinName;


            }
            if (!isCash)
            {
                if (request.isSafe)
                {

                    screenId = (int)SubFormsIds.SafeExpenses;
                }
                else
                {

                    screenId = (int)SubFormsIds.BankExpenses;

                }
            }
            else
            {
                if (request.isSafe)
                {


                    screenId = (int)SubFormsIds.SafeReceipts;
                }
                else
                {


                    screenId = (int)SubFormsIds.BankReceipts;

                }
            }
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
            var dates = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, request.dateFrom, request.dateTo);
            var otherdata = new AdditionalReportData()
            {
                PaymentTypeAr = paymentTypeAr,
                PaymentTypeEn = paymentTypeEn,
                ArabicName = safeOrBankAr,
                LatinName = safeOrBankEn,
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),
                DateFrom = dates.DateFrom,
                DateTo = dates.DateTo,
                Date = dates.Date

            };

            var tablesNames = new TablesNames()
            {

                ObjectName = "SafeBankExpenses",
                FirstListName = "SafeBankExpensesList"
            };
            var report = await _iGeneralPrint.PrintReport<ExpensesAndReceiptsResponseDTO, ExpensesAndReceiptsResponseData, object>(mainData, mainData.data, null, tablesNames, otherdata
             , screenId, exportType, isArabic, fileId);
            return report;


        }

        private async Task<List<GlReciepts>> getRecieptsAsync(safesRequestDTO parm, bool withoutDateFilter = false)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var branches = parm.isSafe ? null : parm.branches.Split(',').Select(x => int.Parse(x)).ToList();

            return _glRecieptsQuery.TableNoTracking
                                       .Include(x => x.Banks)
                                       .Include(x => x.Safes)
                                       .Include(x => x.person)
                                       .Include(x => x.PaymentMethods)
                                       .Where(x => parm.isSafe ? (x.SafeID == parm.id || (x.Authority == AuthorityTypes.Safe && x.BenefitId == parm.id)) : x.BankId == parm.id || (x.Authority == AuthorityTypes.Bank && x.BenefitId == parm.id))
                                       .Where(x => parm.isSafe ? true : branches.Contains(x.BranchId))
                                       .Where(x => !x.IsBlock)
                                       .Where(x => !x.Deferre)
                                       .Where(x => withoutDateFilter ? true : x.RecieptDate.Date >= parm.dateFrom.Date && x.RecieptDate.Date <= parm.dateTo.Date)
                                       .Where(x => (int)parm.paymentMethod != 0 ? x.PaymentMethodId == (int)parm.paymentMethod : true)
                                       .Where(x => x.ParentTypeId == (int)DocumentType.Purchase || x.ParentTypeId == (int)DocumentType.ReturnPurchase ? (!userInfo.otherSettings.purchasesShowOtherPersonsInv ? x.UserId == userInfo.employeeId : true) : true)
                                       .Where(x => x.ParentTypeId == (int)DocumentType.POS || x.ParentTypeId == (int)DocumentType.ReturnPOS ? (!userInfo.otherSettings.posShowOtherPersonsInv ? x.UserId == userInfo.employeeId : true) : true)
                                       .Where(x => x.ParentTypeId == (int)DocumentType.Sales || x.ParentTypeId == (int)DocumentType.ReturnSales ? (!userInfo.otherSettings.salesShowOtherPersonsInv ? x.UserId == userInfo.employeeId : true) : true)
                                       .Where(x => x.IsAccredit == true || x.ParentTypeId == (int)Enums.DocumentType.SafeFunds || x.ParentTypeId == (int)Enums.DocumentType.BankFunds)
                                       .ToList();
        }
        private async Task<Tuple<string, string, string, string, string>> benefitTypeName(int authorityTypes, int benefitId, bool TransferredFrom, int RecieptTypeId, bool isSafe, int paramId, int? bankId, int? safeId, int from = 0)
        {
            ///Items
            //Item 1 = benefitAr
            //item 2 = benefitEN
            //item 3 = benefit Type ar
            //item 4 = benefit Type EN
            //item 5 = benefit Code
            ///

            string benefitAr = "";
            string benefitEN = "";
            string benefitTypeAr = "";
            string benefitTypeEn = "";
            string benefitCode = "";
            NameResponse names = new NameResponse();
            if ((RecieptTypeId == (int)Enums.DocumentType.SafeMultiCollectionReceipts || RecieptTypeId == (int)Enums.DocumentType.BankMultiCollectionReceipts) && benefitId == 0)
            {
                benefitAr = authorityTypes == (int)Enums.AuthorityTypes.customers ? "عملاء" : "موردين";
                benefitEN = authorityTypes == (int)Enums.AuthorityTypes.customers ? "Customers" : "Suppliers";
                benefitTypeAr = authorityTypes == (int)Enums.AuthorityTypes.customers ? "عميل" : "مورد";
                benefitTypeEn = authorityTypes == (int)Enums.AuthorityTypes.customers ? "Customer" : "Suppler";
                benefitCode = "";
            }
            else if (RecieptTypeId != (int)DocumentType.BankTransfer && RecieptTypeId != (int)DocumentType.SafeTransfer)
            {
                if (authorityTypes == (int)AuthorityTypes.customers || authorityTypes == (int)AuthorityTypes.suppliers || ((RecieptTypeId == (int)Enums.DocumentType.SafeMultiCollectionReceipts || RecieptTypeId == (int)Enums.DocumentType.BankMultiCollectionReceipts) && benefitId != 0))
                {
                    var person = await _invPersonsQuery.GetByIdAsync(benefitId);
                    if (person != null)
                    {
                        benefitAr = person.ArabicName;
                        benefitEN = person.LatinName;
                        benefitTypeAr = person.IsCustomer ? "عميل" : "مورد";
                        benefitTypeEn = person.IsCustomer ? "Customer" : "Suppler";
                        benefitCode = person.Code.ToString();
                    }
                }
                else if (authorityTypes == (int)AuthorityTypes.other)
                {
                    var OtherAuthorities = await _gLOtherAuthoritiesQuery.GetByIdAsync(benefitId);
                    if (OtherAuthorities != null)
                    {
                        benefitAr = OtherAuthorities.ArabicName;
                        benefitEN = OtherAuthorities.LatinName;
                        benefitTypeAr = "جهات صرف اخري";
                        benefitTypeEn = "Other authorities";
                        benefitCode = OtherAuthorities.Code.ToString();
                    }
                }
                else if (authorityTypes == (int)AuthorityTypes.salesman)
                {
                    var invSalesMan = await _invSalesManQuery.GetByIdAsync(benefitId);
                    if (invSalesMan != null)
                    {
                        benefitAr = invSalesMan.ArabicName;
                        benefitEN = invSalesMan.LatinName;
                        benefitTypeAr = "مندوب مبيعات";
                        benefitTypeEn = "Salesman";
                        benefitCode = invSalesMan.Code.ToString();

                    }
                }
                else if (authorityTypes == (int)AuthorityTypes.DirectAccounts)
                {
                    var FinancialAccount = await _gLFinancialAccountQuery.GetByIdAsync(benefitId);
                    if (FinancialAccount != null)
                    {
                        benefitAr = FinancialAccount.ArabicName;
                        benefitEN = FinancialAccount.LatinName;
                        benefitTypeAr = "حسابات عامة";
                        benefitTypeEn = "General Ledger";
                        benefitCode = FinancialAccount.AccountCode.ToString().Replace(".", string.Empty);
                    }

                }
            }
            else
            {
                if (RecieptTypeId == (int)DocumentType.SafeTransfer)
                {
                    if (authorityTypes == (int)AuthorityTypes.Safe)
                    {
                        if (paramId == safeId && isSafe)
                        {
                            names = _GLSafeQuery.TableNoTracking.Where(s => s.Id == benefitId).Select(c => new NameResponse
                            {
                                code = c.Code,
                                arabicName = c.ArabicName,
                                latinName = c.LatinName
                            }).FirstOrDefault();

                            benefitAr = names.arabicName;
                            benefitEN = names.latinName;
                            benefitCode = names.code.ToString();
                            benefitTypeAr = "تحويل صادر";
                            benefitTypeEn = "Outgoing Transfer";
                        }
                        else
                        {
                            names = _GLSafeQuery.TableNoTracking.Where(s => s.Id == safeId).Select(c => new NameResponse
                            {
                                code = c.Code,
                                arabicName = c.ArabicName,
                                latinName = c.LatinName
                            }).FirstOrDefault();

                            benefitAr = names.arabicName;
                            benefitEN = names.latinName;
                            benefitCode = names.code.ToString();
                            benefitTypeAr = "تحويل وارد";
                            benefitTypeEn = "Incoming Transfer";
                        }
                    }
                    else
                    {
                        if (paramId == safeId && isSafe)
                        {
                            names = _GLBankQuery.TableNoTracking.Where(s => s.Id == benefitId).Select(c => new NameResponse
                            {
                                code = c.Code,
                                arabicName = c.ArabicName,
                                latinName = c.LatinName
                            }).FirstOrDefault();

                            benefitAr = names.arabicName;
                            benefitEN = names.latinName;
                            benefitCode = names.code.ToString();
                            benefitTypeAr = "تحويل صادر";
                            benefitTypeEn = "Outgoing Transfer";
                        }
                        else
                        {
                            names = _GLBankQuery.TableNoTracking.Where(s => s.Id == benefitId).Select(c => new NameResponse
                            {
                                code = c.Code,
                                arabicName = c.ArabicName,
                                latinName = c.LatinName
                            }).FirstOrDefault();

                            benefitAr = names.arabicName;
                            benefitEN = names.latinName;
                            benefitCode = names.code.ToString();
                            benefitTypeAr = "تحويل وارد";
                            benefitTypeEn = "Incoming Transfer";
                        }
                    }
                }
                else
                {
                    if (authorityTypes == (int)AuthorityTypes.Safe)
                    {
                        if (paramId == bankId && !isSafe)
                        {

                            names = _GLSafeQuery.TableNoTracking.Where(s => s.Id == benefitId).Select(c => new NameResponse
                            {
                                code = c.Code,
                                arabicName = c.ArabicName,
                                latinName = c.LatinName
                            }).FirstOrDefault();

                            benefitAr = names.arabicName;
                            benefitEN = names.latinName;
                            benefitCode = names.code.ToString();
                            benefitTypeAr = "تحويل صادر";
                            benefitTypeEn = "Outgoing Transfer";
                        }
                        else
                        {
                            names = _GLBankQuery.TableNoTracking.Where(s => s.Id == benefitId).Select(c => new NameResponse
                            {
                                code = c.Code,
                                arabicName = c.ArabicName,
                                latinName = c.LatinName
                            }).FirstOrDefault();

                            benefitAr = names.arabicName;
                            benefitEN = names.latinName;
                            benefitCode = names.code.ToString();
                            benefitTypeAr = "تحويل وارد";
                            benefitTypeEn = "Incoming Transfer";
                        }
                    }
                    else
                    {

                        if (paramId == bankId && !isSafe)
                        {
                            names = _GLBankQuery.TableNoTracking.Where(s => s.Id == benefitId).Select(c => new NameResponse
                            {
                                code = c.Code,
                                arabicName = c.ArabicName,
                                latinName = c.LatinName
                            }).FirstOrDefault();

                            benefitAr = names.arabicName;
                            benefitEN = names.latinName;
                            benefitCode = names.code.ToString();
                            benefitTypeAr = "تحويل صادر";
                            benefitTypeEn = "Outgoing Transfer";
                        }
                        else
                        {
                            names = _GLBankQuery.TableNoTracking.Where(s => s.Id == bankId).Select(c => new NameResponse
                            {
                                code = c.Code,
                                arabicName = c.ArabicName,
                                latinName = c.LatinName
                            }).FirstOrDefault();

                            benefitAr = names.arabicName;
                            benefitEN = names.latinName;
                            benefitCode = names.code.ToString();
                            benefitTypeAr = "تحويل وارد";
                            benefitTypeEn = "Incoming Transfer";
                        }
                    }

                }

            }


            return new Tuple<string, string, string, string, string>(benefitAr, benefitEN, benefitTypeAr, benefitTypeEn, benefitCode);
        }

        public async Task<PaymentsAndDisbursementsResponse> getPaymentsAndDisbursements(PaymentsAndDisbursementsRequestDTO parm, bool isPrint = false)
        {

            var _recs = await getRecieptsAsync(new safesRequestDTO()
            {
                branches = parm.branches,
                id = parm.Id,
                isSafe = true,
                paymentMethod = 0,
                dateFrom = parm.dateFrom,
                dateTo = parm.dateTo
            });
            var recs = _recs.Where(c => c.Authority == parm.authorityTypes)
            .Where(c => parm.benefitId != 0 ? c.BenefitId == parm.benefitId : true)
            .OrderBy(x => x.RecieptDate.Date)
            .ThenBy(x => x.CreationDate.TimeOfDay)
            .ToList();
            var types = TransactionTypeList.transactionTypeModels();
            var Paymentstypes = Lists.paymentTypes;
            List<PaymentsAndDisbursementsResponseList> receiptsAndDisbursementsResponseList = new List<PaymentsAndDisbursementsResponseList>();
            double balance = 0;

            var returnsList = Lists.returnInvoiceList;
            var recsBetweenDate = recs.Where(x => x.CreationDate.Date >= parm.dateFrom.Date && x.CreationDate.Date <= parm.dateTo.Date);
            foreach (var item in recsBetweenDate)
            {
                PaymentsAndDisbursementsResponseList receiptsAndDisbursementsResponse = new PaymentsAndDisbursementsResponseList();

                receiptsAndDisbursementsResponse.Id = item.Id;
                receiptsAndDisbursementsResponse.docTypeId = item.RecieptTypeId;

                receiptsAndDisbursementsResponse.documentCode = item.RecieptType;
                receiptsAndDisbursementsResponse.date = item.CreationDate.ToString(defultData.datetimeFormat);
                var benefitName = await getBenefitName(item.Authority, item.BenefitId);
                receiptsAndDisbursementsResponse.benefitAr = benefitName.Item1;
                receiptsAndDisbursementsResponse.benefitEn = benefitName.Item2;
                receiptsAndDisbursementsResponse.typeAr = types.Find(x => x.id == item.RecieptTypeId).arabicName;
                receiptsAndDisbursementsResponse.typeEn = types.Find(x => x.id == item.RecieptTypeId).latinName;
                receiptsAndDisbursementsResponse.paymentTypeAr = item.PaymentMethods.ArabicName;
                receiptsAndDisbursementsResponse.paymentTypeEn = item.PaymentMethods.LatinName;

                receiptsAndDisbursementsResponse.Notes = item.Notes;


                var amount = item.Amount * item.Signal;
                amount = _roundNumbers.GetRoundNumber(amount);
                receiptsAndDisbursementsResponse.debitorTransaction = amount > 0 ? amount : 0;
                receiptsAndDisbursementsResponse.creditorTransaction = amount < 0 ? Math.Abs(amount) : 0;

                balance += amount;
                balance = _roundNumbers.GetRoundNumber(balance);
                receiptsAndDisbursementsResponse.debitorBalance = balance > 0 ? balance : 0;
                receiptsAndDisbursementsResponse.creditorBalance = balance < 0 ? Math.Abs(balance) : 0;

                //receiptsAndDisbursementsResponse.rowClassName = returnsList.Contains(item.ParentTypeId ?? 0) ? defultData.text_danger : "";
                receiptsAndDisbursementsResponseList.Add(receiptsAndDisbursementsResponse);
            }
            var data = !isPrint ? Pagenation<PaymentsAndDisbursementsResponseList>.pagenationList(parm.PageSize, parm.PageNumber, receiptsAndDisbursementsResponseList) : receiptsAndDisbursementsResponseList;
            double MaxPageNumber = recsBetweenDate.ToList().Count() / Convert.ToDouble(parm.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);

            var TotalCreditorTransactionOfPeriod = receiptsAndDisbursementsResponseList.Where(c => c.documentCode != "0").Sum(c => c.creditorTransaction);
            var TotalDebitorTransactionOfPeriod = receiptsAndDisbursementsResponseList.Where(c => c.documentCode != "0").Sum(c => c.debitorTransaction);
            var balanceOfPeriod = TotalCreditorTransactionOfPeriod - TotalDebitorTransactionOfPeriod;

            var actualTotalCreditorTransaction = recs.Sum(c => c.Signal > 0 ? 0 : c.Amount);
            var actualTotalDebitorTransaction = recs.Sum(c => c.Signal > 0 ? c.Amount : 0);
            var actualBalanceAmount = actualTotalCreditorTransaction - actualTotalDebitorTransaction;
            var res = new PaymentsAndDisbursementsResponseDTO()
            {
                list = data,

                TotalCreditorBalanceOfPeriod = balanceOfPeriod > 0 ? _roundNumbers.GetRoundNumber(balanceOfPeriod) : 0,
                TotalDebitorBalanceOfPeriod = balanceOfPeriod > 0 ? 0 : Math.Abs(_roundNumbers.GetRoundNumber(balanceOfPeriod)),

                TotalCreditorTransactionOfPeriod = _roundNumbers.GetRoundNumber(TotalCreditorTransactionOfPeriod),
                TotalDebitorTransactionOfPeriod = _roundNumbers.GetRoundNumber(TotalDebitorTransactionOfPeriod),




                actualTotalCreditorBalance = actualBalanceAmount > 0 ? _roundNumbers.GetRoundNumber(actualBalanceAmount) : 0,
                actualTotalDebitorBalance = actualBalanceAmount > 0 ? 0 : Math.Abs(_roundNumbers.GetRoundNumber(actualBalanceAmount)),

                actualTotalCreditorTransaction = _roundNumbers.GetRoundNumber(actualTotalCreditorTransaction),
                actualTotalDebitorTransaction = _roundNumbers.GetRoundNumber(actualTotalDebitorTransaction),
            };
            return new PaymentsAndDisbursementsResponse()
            {
                Data = res,
                dataCount = data.Count(),
                totalCount = receiptsAndDisbursementsResponseList.Count(),
                Result = Result.Success,
                notes = (countofFilter == parm.PageNumber ? Actions.EndOfData : ""),
            };
        }

        public async Task<WebReport> PaymentsAndDisbursementsReport(PaymentsAndDisbursementsRequestDTO param, exportType exportType, bool isArabic, int fileId = 0)
        {

            var data = await getPaymentsAndDisbursements(param, true);
            var userInfo = await _iUserInformation.GetUserInformation();

            string arabicName;
            string latinName;
            string authorityArabicName;
            string authorityLatinName;

            if (param.authorityTypes == 1)
            {
                authorityArabicName = "عملاء";
                authorityLatinName = "Customers";
            }
            else if (param.authorityTypes == 2)
            {
                authorityArabicName = "موردين";
                authorityLatinName = "Suppliers";
            }
            else if (param.authorityTypes == 3)
            {
                authorityArabicName = "الحسابات العامة";
                authorityLatinName = "Public Accounts";
            }
            else if (param.authorityTypes == 4)
            {
                authorityArabicName = "جهات اخرى";
                authorityLatinName = "Other Parties";
            }
            else if (param.authorityTypes == 5)
            {
                authorityArabicName = "مندوب مبيعات";
                authorityLatinName = " SalesMan";
            }
            else
            {
                authorityArabicName = " ";
                authorityLatinName = " ";
            }



            var additionalData = await _GLSafeQuery.GetByIdAsync(param.Id);
            arabicName = additionalData.ArabicName;
            latinName = additionalData.LatinName;

            var dates = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, param.dateFrom, param.dateTo);

            var otherdata = new AdditionalData()
            {
                ArabicName = arabicName,
                LatinName = latinName,
                AuthorityNameAr = authorityArabicName,
                AuthorityNameEn = authorityLatinName,
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),
                DateFrom = dates.DateFrom,
                DateTo = dates.DateTo,
                Date = dates.Date

            };
            DateTime date;
            foreach (var item in data.Data.list)
            {
                date = Convert.ToDateTime(item.date);
                item.date = date.ToString("yyyy/MM/dd");
            }

            var tablesNames = new TablesNames()
            {

                ObjectName = "PaymentsAndDisbursementsResponse",
                FirstListName = "PaymentsAndDisbursementsResponseList"
            };

            var report = await _iGeneralPrint.PrintReport<PaymentsAndDisbursementsResponseDTO, PaymentsAndDisbursementsResponseList, object>(data.Data, data.Data.list, null, tablesNames, otherdata
             , (int)SubFormsIds.PaymentsAndDisbursements, exportType, isArabic, fileId);
            return report;

        }
        public async Task<ResponseResult> PaymentsAndDisbursements(PaymentsAndDisbursementsRequestDTO parm)
        {
            var res = await getPaymentsAndDisbursements(parm);
            return new ResponseResult()
            {
                Result = res.Result,
                TotalCount = res.totalCount,
                DataCount = res.dataCount,
                Data = res.Data,
                Note = res.notes
            };
        }
        private async Task<Tuple<string, string>> getBenefitName(int Authority, int benefitId)
        {
            string nameAr = "";
            string nameEn = "";
            if (benefitId == 0)
            {
                nameAr = "الكل";
                nameEn = "All";
            }

            else if (Authority == AuthorityTypes.customers || Authority == AuthorityTypes.suppliers)
            {
                var person = await _invPersonsQuery.GetByIdAsync(benefitId);
                nameAr = person.ArabicName;
                nameEn = person.LatinName;
            }
            else if (Authority == AuthorityTypes.salesman)
            {
                var salesman = await _invSalesManQuery.GetByIdAsync(benefitId);
                nameAr = salesman.ArabicName;
                nameEn = salesman.LatinName;
            }
            else if (Authority == AuthorityTypes.other)
            {
                var other = await _gLOtherAuthoritiesQuery.GetByIdAsync(benefitId);
                nameAr = other.ArabicName;
                nameEn = other.LatinName;
            }


            return Tuple.Create(nameAr, nameEn);
        }
        public class AdditionalData : ReportOtherData
        {


            public string AuthorityNameAr { get; set; }

            public string AuthorityNameEn { get; set; }



        }
    }
}
