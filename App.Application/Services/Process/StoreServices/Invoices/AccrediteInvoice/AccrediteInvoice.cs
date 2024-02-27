using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Response.Store;
using App.Application.Services.Process.GLServices.ReceiptBusiness;
using App.Domain.Models.Request.Store.Invoices;
using App.Infrastructure;
//using App.Application.Services.Process.JournalEntries;
using Microsoft.Net.Http.Headers;
using MediatR;
using Microsoft.VisualBasic;
using System.Transactions;
using App.Domain.Entities.POS;
using DocumentFormat.OpenXml.Office2010.Excel;
using App.Application.Handlers.POS;
using App.Domain.Models.Application;
using App.Domain.Models.Response.POS;
using Org.BouncyCastle.Ocsp;
using DocumentFormat.OpenXml.Spreadsheet;

namespace App.Application.Services.Process.StoreServices.Invoices.AccrediteInvoice
{
    public class AccrediteInvoice : BaseClass, IAccrediteInvoice
    {

        private readonly IHttpContextAccessor httpContext;
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvoicePaymentsMethods> InvoicePaymentsMethodsQuery;
        private readonly IRepositoryQuery<GLSafe> SafeQuery;
        private readonly IRoundNumbers roundNumbers;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IRepositoryCommand<InvoiceMaster> InvoiceMasterRepositorsCommand;
        private readonly IRepositoryQuery<GLCurrency> currencyRepositoryQuery;
        private readonly IRepositoryCommand<GlReciepts> ReceiptCommand;
        private readonly IRepositoryQuery<GlReciepts> ReceiptQuery;
        private readonly IRepositoryCommand<GLFinancialAccount> financialAccountRepositoryCommand;
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryCommand<GLJournalEntry> JournalenteryCommand;
        private readonly IRepositoryQuery<GLJournalEntry> JournalenteryQuery;
        private readonly IRepositoryCommand<GLRecieptCostCenter> CostCenterREcieptCommand;
        private readonly IRepositoryQuery<GLRecieptCostCenter> CostCenterRecieptQuery;
        private readonly IRepositoryQuery<userAccount> UserAccountQuery;
        private readonly IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsQuery;
        private readonly IRepositoryCommand<GLJournalEntryDetails> journalEntryDetailsCommand;
        //private readonly IJournalEntryBusiness JournalEntryBusiness;
        private readonly IHelperService generalSetteing;
        private readonly IReceiptsService ReceiptsServices;
        private readonly IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsRepositoryQuery;
        private readonly IRepositoryCommand<InvoiceMasterHistory> InvoiceMasterHistoryRepositoryCommand;
        private readonly IRepositoryCommand<GLRecieptsHistory> receiptsHistoryCommand;
        private readonly IRepositoryQuery<GLRecieptsHistory> receiptsHistoryQuery;
        private SettingsOfInvoice SettingsOfInvoice;
        private readonly iUserInformation Userinformation;
        private readonly IRepositoryQuery<OtherSettingsSafes> OtherSettingsSafesQuery;
        private readonly IRepositoryQuery<InvPersons> PersonQuery;
        private readonly iUserInformation _iUserInformation;
        private readonly IGeneralPrint _iGeneralPrint;
        private readonly IMediator _mediator;
        private readonly IRepositoryQuery<POSSession> _POSSessionQurey;

        public AccrediteInvoice(IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,

                              IRepositoryQuery<InvGeneralSettings> _InvGeneralSettingsRepositoryQuery,
                              iUserInformation _Userinformation, IReceiptsService receiptsServices,
                              IRepositoryQuery<InvoicePaymentsMethods> invoicePaymentsMethodsQuery,
                              IRepositoryCommand<InvoiceMaster> invoiceMasterRepositorsCommand,
                              IRepositoryQuery<GLCurrency> CurrencyRepositoryQuery,
                              IRepositoryQuery<GLJournalEntry> journalenteryQuery,
                              IRepositoryCommand<GLJournalEntry> journalenteryCommand,
                              //IJournalEntryBusiness journalEntryBusiness,
                              IHelperService generalSetteing,
                              IRepositoryQuery<GlReciepts> receiptQuery,
                              IRepositoryCommand<GLFinancialAccount> FinancialAccountRepositoryCommand,
                              IRepositoryQuery<GLFinancialAccount> FinancialAccountRepositoryQuery,
                              IRepositoryQuery<GLSafe> safeQuery,
                              IRoundNumbers roundNumbers,
                              ISystemHistoryLogsService systemHistoryLogsService,
                              IRepositoryCommand<GlReciepts> receiptCommand,
                              IRepositoryQuery<userAccount> useraccountQuery, IRepositoryQuery<GLJournalEntryDetails> JournalEntryDetailsQuery,
                              IRepositoryCommand<GLJournalEntryDetails> JournalEntryDetailsCommand,
                              IRepositoryCommand<GLRecieptCostCenter> costCenterREcieptCommand,
                              IRepositoryQuery<GLRecieptCostCenter> costCenterRecieptQuery,
                              IRepositoryQuery<OtherSettingsSafes> _otherSettingsSafesQuery,
                              IRepositoryCommand<InvoiceMasterHistory> invoiceMasterHistoryRepositoryCommand,
                              IRepositoryQuery<InvPersons> _PersonQuery,
                              IHttpContextAccessor _httpContext,
                              iUserInformation iUserInformation,
                              IGeneralPrint iGeneralPrint,
                              IMediator mediator, IRepositoryCommand<GLRecieptsHistory> receiptsHistoryCommand, IRepositoryQuery<GLRecieptsHistory> receiptsHistoryQuery, IRepositoryQuery<POSSession> pOSSessionQurey) : base(_httpContext)
        {
            InvoiceMasterRepositoryQuery = _InvoiceMasterRepositoryQuery;
            InvGeneralSettingsRepositoryQuery = _InvGeneralSettingsRepositoryQuery;
            httpContext = _httpContext;
            Userinformation = _Userinformation;
            InvoicePaymentsMethodsQuery = invoicePaymentsMethodsQuery;
            InvoiceMasterRepositorsCommand = invoiceMasterRepositorsCommand;
            currencyRepositoryQuery = CurrencyRepositoryQuery;
            ReceiptsServices = receiptsServices;
            ReceiptCommand = receiptCommand;
            ReceiptQuery = receiptQuery;
            financialAccountRepositoryCommand = FinancialAccountRepositoryCommand;
            financialAccountRepositoryQuery = FinancialAccountRepositoryQuery;
            JournalenteryCommand = journalenteryCommand;
            JournalenteryQuery = journalenteryQuery;
            SafeQuery = safeQuery;
            this.roundNumbers = roundNumbers;
            _systemHistoryLogsService = systemHistoryLogsService;
            UserAccountQuery = useraccountQuery;
            journalEntryDetailsQuery = JournalEntryDetailsQuery;
            journalEntryDetailsCommand = JournalEntryDetailsCommand;
            OtherSettingsSafesQuery = _otherSettingsSafesQuery;
            InvoiceMasterHistoryRepositoryCommand = invoiceMasterHistoryRepositoryCommand;
            CostCenterREcieptCommand = costCenterREcieptCommand;
            CostCenterRecieptQuery = costCenterRecieptQuery;
            //JournalEntryBusiness = journalEntryBusiness;
            this.generalSetteing = generalSetteing;
            PersonQuery = _PersonQuery;
            _iUserInformation = iUserInformation;
            _iGeneralPrint = iGeneralPrint;
            _mediator = mediator;
            this.receiptsHistoryCommand = receiptsHistoryCommand;
            this.receiptsHistoryQuery = receiptsHistoryQuery;
            _POSSessionQurey = pOSSessionQurey;
        }
        public async Task<ResponseResult> GetAccrediteInvoiceDataCount(AccreditInvoiceRequest parameter)
        {
            UserInformationModel userInfo = await Userinformation.GetUserInformation();
            if (userInfo == null)
                return new ResponseResult() { Data = null, Result = Result.NoDataFound, ErrorMessageAr = "no taken request", ErrorMessageEn = "no taken request" };

            int invReturn = GetInvoiceReturnType(parameter.InvType);
            var data = InvoiceMasterRepositoryQuery.TableNoTracking
                 .Where(h => (h.InvoiceTypeId == parameter.InvType
                 || h.InvoiceTypeId == invReturn)
                 && h.IsDeleted == false && h.IsAccredite == false
                 && h.InvoiceDate <= parameter.date
                 && h.BranchId == userInfo.CurrentbranchId
                 && h.EmployeeId == parameter.userId
                 ).Count();


            return new ResponseResult() { TotalCount = data, Result = Result.Success };
        }
        public async Task<ResponseResult> GetAccrediteInvoiceData(AccreditInvoiceRequest parameter, bool isPrint = false)
        {

            UserInformationModel userInfo = await Userinformation.GetUserInformation();
            if (userInfo == null)
                return new ResponseResult() { Data = null, Result = Result.NoDataFound, ErrorMessageAr = "no taken request", ErrorMessageEn = "no taken request" };

            var data = await InvoiceMasterRepositoryQuery.TableNoTracking
                 .Where(x=> parameter.sessionId == 0 ? x.IsAccredite == false : true)
                 .Where(x=> parameter.sessionId != 0 ? x.POSSessionId == parameter.sessionId : true)
                 .Where(x=> parameter.sessionId == 0 ? x.EmployeeId == parameter.userId : true)
                 .Where(x=> parameter.sessionId == 0 ? x.BranchId == userInfo.CurrentbranchId : true)
                 .Where(x=> parameter.sessionId == 0 ?  x.InvoiceDate <= parameter.date : true)
                 .Where(h => h.InvoiceTypeId == parameter.InvType && h.IsDeleted == false )
                 .Include(a => a.InvoicePaymentsMethods)
                 .ThenInclude(h => h.PaymentMethod)
                 .ToListAsync();


            var invData = data.Select(h => new AccrediteInvoiceResponseData()
            {
                InvoiceDate = h.InvoiceDate,
                InvoiceType = h.InvoiceType,
                InvoiceTypeId = h.InvoiceTypeId,
                Net = h.Net,
                Paied =(Lists.POSInvoicesList.Contains(parameter.InvType)? (h.Net > h.Paid ? h.Paid : h.Net) : h.Paid),
                transactionTypeAr = Aliases.InvoiceTransaction.PurchaseAr,
                transactionTypeEn = Aliases.InvoiceTransaction.PurchaseEn,
                InvoiceId = h.InvoiceId,
                paymentMethods = h.InvoicePaymentsMethods
                .Select(q => new { q.PaymentMethod, value =(parameter.InvType==(int)DocumentType.POS? 
                                                                   ( h.Net > q.Value ? q.Value : h.Net): q.Value),
                    q.InvoiceId, q.PaymentMethodId })
                .Select(a => new PaymentMethodsDataResponse()
                {
                    // change payment method name if id = -1 السحب من رصيد العميل 
                    // Hello shoraaa
                    //hello alaaaaa
                    arabicName = a.PaymentMethod.ArabicName,
                    latinName =  a.PaymentMethod.LatinName,
                    Value = a.value,
                    InvoiceId = a.InvoiceId,
                    PaymentMethodId = a.PaymentMethodId
                })
                .ToList(),


            });

          
            List<AccrediteInvoiceResponseData> pagenatData = isPrint ? invData.ToList() : Pagenation<AccrediteInvoiceResponseData>.pagenationList(parameter.PageSize, parameter.PageNumber, invData.ToList());

            List<PaymentMethodsDataResponse> paymentList = new List<PaymentMethodsDataResponse>();
            foreach (var item in pagenatData)
            {
                var invTypes = Aliases.listOfInvoicesNames.listOfNames().Where(h => h.invoiceTypeId == item.InvoiceTypeId);
                item.transactionTypeAr = invTypes.Select(a => a.NameAr).FirstOrDefault();
                item.transactionTypeEn = invTypes.Select(a => a.NameEn).FirstOrDefault();
                paymentList.AddRange(item.paymentMethods);
            }

            var paymentTypes = paymentList.GroupBy(h => h.arabicName).Select(h => h.First()).ToList();

            return new ResponseResult() { Data = new TotalAccrediteInvoiceResponse() { paymentMethodType = paymentTypes, accrediteInvoiceResponseData = pagenatData }, TotalCount = data.Count, Result = Result.Success };
        }
        public async Task<WebReport> GetAccrediteInvoiceDataReport(AccreditInvoiceRequest parameter, InvoiceClosing invoiceClosing, int screenId, exportType exportType, bool isArabic,int fileId=0)
        {
            var userInfo = _iUserInformation.GetUserInformationById(parameter.userId);



            var loginUser = await _iUserInformation.GetUserInformation();
            var tablesNames = new TablesNames()
            {

                FirstListName = "AccrediteInvoice",
                SecondListName = "PaymentMethods"
            };
            WebReport report = new WebReport();

            ClosingDataForReport otherData = new ClosingDataForReport()
            {
                EmployeeName = loginUser.employeeNameAr.ToString(),
                EmployeeNameEn = loginUser.employeeNameEn.ToString(),
                ArabicName = userInfo.employeeNameAr.ToString(),
                LatinName = userInfo.employeeNameEn.ToString(),
                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                DateTo = parameter.date.ToString("yyyy/MM/dd")
        };
            if (parameter.sessionId > 0)
            {
               
                //var openSeasson = await _mediator.Send(new POSSessionDetaliesRequest { SessionId = parameter.sessionId });
                //var sessionData = (POSSessionDetaliesResponse)openSeasson.Data;
                var sessionData =  _POSSessionQurey.TableNoTracking.Where(x => x.Id == parameter.sessionId).FirstOrDefault();

                otherData.DateFrom = sessionData.start.ToString("yyyy/MM/dd HH:mm");

                otherData.DateTo = sessionData.end?.ToString("yyyy/MM/dd HH:mm");
                otherData.Id = sessionData.sessionCode;
            }

            var data = new ResponseResult();

            if (invoiceClosing != InvoiceClosing.ClosingPaymentsAndReceipts)
            {
                //if(invoiceClosing != InvoiceClosing.ClosingInvoicesPeriod)

                //    screenId = 45;

                //else
                //    screenId = 59;

                data = await GetAccrediteInvoiceData(parameter, true);

                otherData.ClosingDate = parameter.date;

                var resData = (TotalAccrediteInvoiceResponse)(data.Data);
                var paymentMethods = new List<PaymentMethodsDataResponse>();
                if (resData.accrediteInvoiceResponseData.Count > 0)
                {

                    foreach (var item in resData.accrediteInvoiceResponseData)
                    {
                        if (item.paymentMethods.Count > 0)

                            paymentMethods.AddRange(item.paymentMethods);
                        else

                            paymentMethods.Add(new PaymentMethodsDataResponse());

                    }

                }
                else
                {
                    resData.accrediteInvoiceResponseData = new List<AccrediteInvoiceResponseData>()
                    {
                        new AccrediteInvoiceResponseData()
                    };
                }

                report = await _iGeneralPrint.PrintReport<object, AccrediteInvoiceResponseData, PaymentMethodsDataResponse>(null, resData.accrediteInvoiceResponseData, paymentMethods, tablesNames, otherData
                , screenId, exportType, isArabic,fileId, (int)invoiceClosing);
                report.Report.SetParameterValue("sessionId", parameter.sessionId);

            }
            else
            {
                double invoiceTotalValue = 0;
                double returnTotalValue = 0;
                if (parameter.sessionId > 0)
                {
                    var openSeasson = await _mediator.Send(new POSSessionDetaliesRequest { SessionId = parameter.sessionId });
                    var  datares = (POSSessionDetaliesResponse)openSeasson.Data;
                    var invoices = new List<PaymentMethodsDataResponse>();
                    var returns = new List<PaymentMethodsDataResponse>();
                    if (datares.POSSessionDetaliesResponse_Sales.Count > 0)
                    {
                        invoiceTotalValue = datares.POSSessionDetaliesResponse_Sales.Sum(a => a.total);
                        foreach (var item in datares.POSSessionDetaliesResponse_Sales)
                        {
                            invoices.Add(new PaymentMethodsDataResponse()
                            {
                                latinName = item.latinName,
                                arabicName = item.arabicName,
                                Value = item.total,
                                Count = item.count
                            });
                        }
                    }

                    else
                        invoices.Add(new PaymentMethodsDataResponse());

                    if (datares.POSSessionDetaliesResponse_Return.Count > 0)
                    {
                        returnTotalValue = datares.POSSessionDetaliesResponse_Return.Sum(a => a.total);
                        foreach (var item in datares.POSSessionDetaliesResponse_Return)
                        {
                            returns.Add(new PaymentMethodsDataResponse()
                            {
                                latinName = item.latinName,
                                arabicName = item.arabicName,
                                Value = item.total,
                                Count = item.count
                            });
                        }
                    }
                    else
                        returns.Add(new PaymentMethodsDataResponse());
                    report = await _iGeneralPrint.PrintReport<object, PaymentMethodsDataResponse, PaymentMethodsDataResponse>(null, invoices, returns, tablesNames, otherData
                     , screenId, exportType, isArabic,fileId, (int)invoiceClosing);
                }
                else
                {

                    data = await GetAccrediteInvoicPaymentTypeData(parameter, true);


                    var resData = (FinalPaymentMethodsDataResponse)(data.Data);

                    
                    if (resData.InvoiceDetails.Count > 0)

                        invoiceTotalValue = resData.InvoiceDetails.Sum(a => a.Value);

                    else
                        resData.InvoiceDetails = new List<PaymentMethodsDataResponse>()
                    {
                        new PaymentMethodsDataResponse()
                    };
                    if (resData.ReturnInvoiceDetails.Count > 0)
                        returnTotalValue = resData.ReturnInvoiceDetails.Sum(a => a.Value);
                    else
                        resData.ReturnInvoiceDetails = new List<PaymentMethodsDataResponse>()
                    {
                        new PaymentMethodsDataResponse()
                    };


                    report = await _iGeneralPrint.PrintReport<object, PaymentMethodsDataResponse, PaymentMethodsDataResponse>(null, resData.InvoiceDetails, resData.ReturnInvoiceDetails, tablesNames, otherData
                     , screenId, exportType, isArabic,fileId, (int)invoiceClosing);
                    report.Report.SetParameterValue("total", invoiceTotalValue - returnTotalValue);

                }

                report.Report.SetParameterValue("total", invoiceTotalValue - returnTotalValue);

                report.Report.SetParameterValue("sessionId", parameter.sessionId);


            }
            return report;

        }

        //public byte[] ReadAllBytes(string fileName)
        //{
        //    byte[] buffer = null;
        //    using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        //    {
        //        buffer = new byte[fs.Length];
        //        fs.Read(buffer, 0, (int)fs.Length);
        //    }
        //    return buffer;
        //}
        public async Task<ResponseResult> GetAccrediteInvoicPaymentTypeData(AccreditInvoiceRequest parameter, bool isPrint = false)
        {
            UserInformationModel userInfo = await Userinformation.GetUserInformation();
            if (userInfo == null)
                return new ResponseResult() { Data = null, Result = Result.NoDataFound, ErrorMessageAr = "no taken request", ErrorMessageEn = "no taken request" };
            //var setting = InvGeneralSettingsRepositoryQuery.GetAll().FirstOrDefault();
            // int decemalNum = setting.Other_Decimals;



            int invReturn = GetInvoiceReturnType(parameter.InvType);
            if (invReturn <= 0)
                return new ResponseResult() { Data = null, Result = Result.NoDataFound, ErrorMessageAr = "type Error", ErrorMessageEn = "Type Error" };



            //delay 
            var AllDelaydata = await InvoiceMasterRepositoryQuery.TableNoTracking
                .Where(h => (h.InvoiceTypeId == parameter.InvType || h.InvoiceTypeId == invReturn)
                                   && h.IsDeleted == false
                                   && h.IsAccredite == false
                                   && h.InvoiceDate <= parameter.date
                                   && (h.PaymentType == (int)PaymentType.Delay || h.PaymentType == (int)PaymentType.Partial)
                                   && h.BranchId == userInfo.CurrentbranchId
                                   && h.EmployeeId == parameter.userId)
                .ToListAsync();

            var DelaydataReturn = new PaymentMethodsDataResponse()
            {
                Count = AllDelaydata.Where(h => h.InvoiceTypeId == invReturn).ToList().Count,
                Value = roundNumbers.GetRoundNumber(AllDelaydata.Where(h => h.InvoiceTypeId == invReturn).ToList().Sum(a => a.Remain)),
                arabicName = "أجل",
                latinName = "Deferre" 
            };
            var DelaydataInvoice = new PaymentMethodsDataResponse()
            {
                Count = AllDelaydata.Where(h => h.InvoiceTypeId == parameter.InvType).ToList().Count,
                Value = roundNumbers.GetRoundNumber((AllDelaydata.Where(h => h.InvoiceTypeId == parameter.InvType).ToList().Sum(a => a.Remain))),
                arabicName = "أجل",
                latinName = "Deferre"
            };

            //main invoice
            var AllPayment = await InvoicePaymentsMethodsQuery.TableNoTracking
                            .Include(h => h.InvoicesMaster)
                            .Include(a => a.PaymentMethod)
                            .Where(h => (h.InvoicesMaster.InvoiceTypeId == parameter.InvType || h.InvoicesMaster.InvoiceTypeId == invReturn)
                                               && h.InvoicesMaster.IsDeleted == false
                                               && h.InvoicesMaster.IsAccredite == false
                                               && h.InvoicesMaster.InvoiceDate <= parameter.date
                                               && h.InvoicesMaster.EmployeeId == parameter.userId
                                               && h.BranchId == userInfo.CurrentbranchId)
                            .Select(a => new
                            {
                                InvoiceTypeId = a.InvoicesMaster.InvoiceTypeId,
                                PaymentMethodId = a.PaymentMethodId,
                                Value = (Lists.POSInvoicesList.Contains(parameter.InvType) ?( a.InvoicesMaster.Net > a.Value ? a.Value : a.InvoicesMaster.Net):a.Value),
                                PaymentMethod = a.PaymentMethod,

                            })  
                            .ToListAsync();


            var ReturnInvoiceAccredited = AllPayment.Where(a => a.InvoiceTypeId == invReturn)
               .GroupBy(h => h.PaymentMethodId)
               .Select(h => (new PaymentMethodsDataResponse()
               {
                   Count = h.Count(),
                   Value = roundNumbers.GetRoundNumber(h.Sum(a => a.Value)),
                   arabicName = h.FirstOrDefault().PaymentMethod.ArabicName,
                   latinName = h.FirstOrDefault().PaymentMethod.LatinName,

               })).ToList();
         
            var InvoiceAccredited = AllPayment.Where(a => a.InvoiceTypeId == parameter.InvType)
             .GroupBy(h => h.PaymentMethodId)
             .Select(h => (new PaymentMethodsDataResponse()
             {
                 Count = h.Count(),
                 Value = roundNumbers.GetRoundNumber(h.Sum(a => a.Value)),
                 arabicName = h.FirstOrDefault().PaymentMethod.ArabicName,
                 latinName = h.FirstOrDefault().PaymentMethod.LatinName,

             })).ToList();

            if (DelaydataInvoice.Value > 0)
                InvoiceAccredited.Add(DelaydataInvoice);

            if (DelaydataReturn.Value > 0)
                ReturnInvoiceAccredited.Add(DelaydataReturn);

            List<PaymentMethodsDataResponse> pagenatInvoiceData = Pagenation<PaymentMethodsDataResponse>.pagenationList(parameter.PageSize, parameter.PageNumber, InvoiceAccredited);
            List<PaymentMethodsDataResponse> pagenatReturnInvoiceData = Pagenation<PaymentMethodsDataResponse>.pagenationList(parameter.PageSize, parameter.PageNumber, ReturnInvoiceAccredited);


            var finalResult = new FinalPaymentMethodsDataResponse()
            {
                InvoiceDetails = pagenatInvoiceData,
                ReturnInvoiceDetails = pagenatReturnInvoiceData
            };

            return new ResponseResult() { Data = finalResult, Result = Result.Success, TotalCount = InvoiceAccredited.Count };

        }
        //public async Task<WebReport> AccrediteInvoicPaymentTypeReport(AccreditInvoiceRequest parameter, exportType exportType, bool isArabic)
        //{
        //    var data = await GetAccrediteInvoicPaymentTypeData(parameter, true);
        //    var userInfo = _iUserInformation.GetUserInformationById(parameter.userId);
        //      ReportOtherData otherData = new ReportOtherData()
        //    {
        //        EmployeeName = userInfo.employeeNameAr.ToString(),
        //        EmployeeNameEn = userInfo.employeeNameEn.ToString(),
        //        DateTo = parameter.date.ToString("yyyy/MM/dd"),
        //        Date = DateTime.Now.ToString("yyyy/MM/dd"),

        //    };

        //    var resData = (FinalPaymentMethodsDataResponse)(data.Data);

        //    var tablesNames = new TablesNames()
        //    {

        //        FirstListName = "AccrediteInvoice",
        //        SecondListName = "PaymentMethods"
        //    };




        //    var report = await _iGeneralPrint.PrintReport<object, PaymentMethodsDataResponse, PaymentMethodsDataResponse>(null, resData.accrediteInvoiceResponseData, resData.paymentMethodType, tablesNames, otherData
        //     , (int)SubFormsIds.PruchasesClosing_Purchases, exportType, isArabic);
        //    return report;
        //}
        public async Task<ResponseResult> GetAccrediteInvoicSafeBankData(AccreditInvoiceRequest parameter)
        {
            UserInformationModel userInfo = await Userinformation.GetUserInformation();
            if (userInfo == null)
                return new ResponseResult() { Data = null, Result = Result.NoDataFound, ErrorMessageAr = "no taken request", ErrorMessageEn = "no taken request" };

            int invReturn = GetInvoiceReturnType(parameter.InvType);

            if (invReturn <= 0)
                return new ResponseResult() { Data = null, Result = Result.NoDataFound, ErrorMessageAr = "type Error", ErrorMessageEn = "Type Error" };

            var safeData = await SafeQuery.TableNoTracking.Where(h => h.Id == parameter.safeId).ToListAsync();

            if (safeData.Count <= 0)
                return new ResponseResult() { Data = null, Result = Result.NoDataFound, ErrorMessageAr = "safe not found ", ErrorMessageEn = "safe not found" };


            var Alldata = await InvoicePaymentsMethodsQuery.TableNoTracking
                          .Include(h => h.InvoicesMaster)
                          .Include(a => a.PaymentMethod).ThenInclude(a => a.bank)
                          .Include(a => a.PaymentMethod).ThenInclude(a => a.safe)
                          .Where(h => (h.InvoicesMaster.InvoiceTypeId == parameter.InvType || h.InvoicesMaster.InvoiceTypeId == invReturn)
                                               && h.InvoicesMaster.IsDeleted == false
                                               && h.InvoicesMaster.IsAccredite == false
                                               && h.InvoicesMaster.InvoiceDate <= parameter.date
                                               && h.BranchId == userInfo.CurrentbranchId
                                               && h.InvoicesMaster.EmployeeId == parameter.userId)
                          .OrderBy(o => o.InvoiceId)
                          .ToListAsync();

            var returnInvoice = Filterinvoices(Alldata, invReturn, safeData);

            var InvoiceData = Filterinvoices(Alldata, parameter.InvType, safeData);
            List<SafeOrBankDataResponse> pagenatInvoiceData = Pagenation<SafeOrBankDataResponse>.pagenationList(parameter.PageSize, parameter.PageNumber, InvoiceData);
            List<SafeOrBankDataResponse> pagenatReturnInvoiceData = Pagenation<SafeOrBankDataResponse>.pagenationList(parameter.PageSize, parameter.PageNumber, returnInvoice);

            var finalResult = new FinalBankOrSafeDataResponse()
            {
                InvoiceDetails = pagenatInvoiceData,
                ReturnInvoiceDetails = pagenatReturnInvoiceData
            };

            return new ResponseResult() { Data = finalResult, TotalCount = InvoiceData.Count, Result = Result.Success };
        }

        private List<SafeOrBankDataResponse> Filterinvoices(List<InvoicePaymentsMethods> alldata, int invType, List<GLSafe> safeData)
        {
            var invData = alldata.Where(h => h.InvoicesMaster.InvoiceTypeId == invType)
                                           .GroupBy(h => new { h.InvoiceId, h.PaymentMethodId })
                                           .Select(a => new SafeOrBankDataResponse()
                                           {
                                               Value =( Lists.POSInvoicesList.Contains(invType) ? a.Sum(h => h.InvoicesMaster.Net > h.Value ? h.Value : h.InvoicesMaster.Net): a.Sum(h =>  h.Value)),
                                               InvoiceType = a.FirstOrDefault().InvoicesMaster.InvoiceType,
                                               safeOrBankNameAr =(a.FirstOrDefault().PaymentMethodId==(int)PaymentMethod.PersonBalance?"السحب من رصيد المستفيد": a.FirstOrDefault().PaymentMethod.safe == null ? a.FirstOrDefault().PaymentMethod.bank.ArabicName : safeData.FirstOrDefault().ArabicName),
                                               safeOrBankNameEn = (a.FirstOrDefault().PaymentMethodId == (int)PaymentMethod.PersonBalance ? "Withdrawal from the beneficiary's balance" : a.FirstOrDefault().PaymentMethod.safe == null ? a.FirstOrDefault().PaymentMethod.bank.LatinName : safeData.FirstOrDefault().LatinName)
                                           });
            return invData.ToList();
        }

        private int GetInvoiceReturnType(int InvType)
        {
            int invReturn = 0;
            if (InvType == (int)DocumentType.Purchase)
                invReturn = (int)DocumentType.ReturnPurchase;

            else if (InvType == (int)DocumentType.Sales)
                invReturn = (int)DocumentType.ReturnSales;

            else if (InvType == (int)DocumentType.POS)
                invReturn = (int)DocumentType.ReturnPOS;
            return invReturn;
        }


        public async Task<bool> checkUserInfo(int safeId, int empId, int currentUserId, int branchId)
        {
            var users = UserAccountQuery.TableNoTracking
                .Include(x => x.otherSettings)
                .Include(x => x.employees)
                .Where(a => a.employees.EmployeeBranches.Select(c => c.BranchId).ToArray().Contains(branchId));

            //هل له صلاحيه كل المستخدمين
            var currentUserAccredditForAllUsers = users.Where(x => x.id == currentUserId).FirstOrDefault().otherSettings.FirstOrDefault().accredditForAllUsers;
            var response = users.Where(x => 1 == 1);

            //هجيب المستخدم فقط 
            if (!currentUserAccredditForAllUsers)
            {
                response = users.Where(x => x.id == currentUserId);
            }
            var userExist = response.Where(h => h.employeesId == empId);
            if (userExist.Count() <= 0)
                return false;

            //check if safe is in users permission
            var safes = await OtherSettingsSafesQuery
                .GetAllIncludingAsync(0, 0,
                x => x.otherSettingsId == userExist.FirstOrDefault().otherSettings.FirstOrDefault().Id,
                 x => x.otherSettings, x => x.otherSettings.userAccount);

            var res = safes.Where(h => h.gLSafeId == safeId).Any();
            if (!res)
                return false;
            return true;
        }

        //in case of accredit and add receipts the same time
        //first Time 
        public async Task<ResponseResult> accrediteAllInvoice(AccreditInvoiceRequest parameter)
        {
            DateTime AccreaditDate = DateTime.Now;
            UserInformationModel userInfo = await Userinformation.GetUserInformation();
            if (userInfo == null)
                return new ResponseResult() { Data = null, Result = Result.Failed, ErrorMessageAr = "no taken request", ErrorMessageEn = "no taken request" };


            bool checkedInfo = await checkUserInfo(parameter.safeId, parameter.userId, userInfo.userId, userInfo.CurrentbranchId);
            if (!checkedInfo)
                return new ResponseResult() { Data = null, Result = Result.Failed, ErrorMessageAr = "userPermissionError", ErrorMessageEn = "UserPermission Error" };
            int invReturn = GetInvoiceReturnType(parameter.InvType);

            if (invReturn <= 0)
                return new ResponseResult() { Data = null, Result = Result.Failed, ErrorMessageAr = "type Error", ErrorMessageEn = "Type Error" };

            var oldInvoice = await InvoiceMasterRepositoryQuery.FindAllAsync(h => (h.InvoiceTypeId == parameter.InvType || h.InvoiceTypeId == invReturn)
                                          && h.IsDeleted == false
                                          && h.IsAccredite == false
                                          && h.InvoiceDate <= parameter.date
                                          && h.BranchId == userInfo.CurrentbranchId);
            if (oldInvoice.Count <= 0)
                return new ResponseResult() { Data = null, Result = Result.NoDataFound, ErrorMessageAr = "No Data for Accredit", ErrorMessageEn = "No Data for Accredit" };

            /////////////////////
            var Alldata = await InvoicePaymentsMethodsQuery.TableNoTracking
                          .Include(h => h.InvoicesMaster)
                          .Include(a => a.PaymentMethod).ThenInclude(a => a.bank)
                          .Include(a => a.PaymentMethod).ThenInclude(a => a.safe)
                          .Where(h => (h.InvoicesMaster.InvoiceTypeId == parameter.InvType || h.InvoicesMaster.InvoiceTypeId == invReturn)
                                               && h.InvoicesMaster.IsDeleted == false
                                               && h.InvoicesMaster.IsAccredite == false
                                               && h.InvoicesMaster.InvoiceDate <= parameter.date
                                               && h.BranchId == userInfo.CurrentbranchId
                                               && h.InvoicesMaster.EmployeeId == parameter.userId)
                          .GroupBy(h => new { h.InvoiceId, h.PaymentMethodId })
                          .Select(a => new RecieptsRequest()
                          {
                              //SafeID = a.FirstOrDefault().PaymentMethod.SafeId,
                              SafeID = a.FirstOrDefault().PaymentMethod.SafeId != null ? parameter.safeId : null,
                              BankId = a.FirstOrDefault().PaymentMethod.BankId,
                              Amount = a.Sum(h => h.Value),
                              RecieptDate = AccreaditDate,
                              PaymentMethodId = a.FirstOrDefault().PaymentMethodId,
                              ChequeBankName = "",
                              ChequeNumber = a.FirstOrDefault().Cheque,
                              ChequeDate = a.FirstOrDefault().InvoicesMaster.InvoiceDate,
                              ParentType = a.FirstOrDefault().InvoicesMaster.InvoiceType,
                              ParentTypeId = a.FirstOrDefault().InvoicesMaster.InvoiceTypeId,
                              //Creditor = a.Sum(h => h.Value),
                              //Debtor = a.Sum(h => h.Value),
                              //RecieptTypeId= a.FirstOrDefault().InvoicesMaster.InvoiceTypeId,
                              IsAccredit = false,
                              ParentId = a.FirstOrDefault().InvoicesMaster.InvoiceId,
                              Serialize = a.FirstOrDefault().InvoicesMaster.Serialize,
                              Code = a.FirstOrDefault().InvoicesMaster.Code,
                              BenefitId = a.FirstOrDefault().InvoicesMaster.PersonId.Value,

                          })
                          .ToListAsync();


            foreach (var item in Alldata)
            {
                item.RecieptTypeId = GetReceiptsType(item.SafeID, item.BankId, item.ParentTypeId);

                if (Lists.purchasesInvoicesList.Contains(item.ParentTypeId.Value))
                    item.Authority = AuthorityTypes.suppliers;
                else if (Lists.salesInvoicesList.Contains(item.ParentTypeId.Value))
                    item.Authority = AuthorityTypes.customers;

                //var InviceName = Aliases.listOfInvoicesNames.listOfNames();

                var result = await ReceiptsServices.AddReceipt(item);
            }


            //oldInvoice.ForEach(h => h.IsAccredite = true);


            // add accridit invoice in history 
            //await InvoiceMasterRepositorsCommand.UpdateAsyn(oldInvoice);
            bool isSave = await setInvoiceAccreditHistory(oldInvoice, userInfo);




            return new ResponseResult() { Result = Result.Success };
        }
        //hitory of invoiceAccredit
        //second time

        private async Task<bool> setHistory(List<InvoiceMaster> oldInvoice, List<int> Ids, UserInformationModel userinfo)
        {
            try
            {
                var hSaved=await setInvoiceAccreditHistory(oldInvoice, userinfo);
                //var historyr= receiptsHistoryQuery.Table.Where(a => Ids.Contains(a.ReceiptsId));
                var historyr = receiptsHistoryCommand.Table.Where(predicate: a => Ids.Contains(a.ReceiptsId)).ToList();

                historyr.Select(a => a.employeesId = userinfo.employeeId).ToList();
               var recsave= await receiptsHistoryCommand.SaveAsync();
                return hSaved;
            }
            catch (Exception e)
            {
                return false;

            }

        }
        private async Task<bool> setInvoiceAccreditHistory(List<InvoiceMaster> oldInvoice, UserInformationModel uInfo)
        {
            //var userInfo = Userinformation.GetUserInformation();
            InvoiceMasterHistory history = new InvoiceMasterHistory();
            var listOfInvoiceMasterHistory = new List<InvoiceMasterHistory>();
            foreach (var item in oldInvoice)
            {
                item.IsAccredite = true;
                history = new InvoiceMasterHistory()
                {
                    LastDate = DateTime.Now,
                    LastAction = HistoryActions.Accredit,
                    LastTransactionAction = null,
                    AddTransactionUser = HistoryActions.Accredit,
                    BranchId = item.BranchId,
                    Notes = item.Notes,
                    BookIndex = item.BookIndex,
                    Code = item.Code,
                    InvoiceDate = item.InvoiceDate,
                    InvoiceId = item.InvoiceId,
                    InvoiceType = item.InvoiceType,
                    InvoiceTypeId = item.InvoiceTypeId,
                    SubType = item.InvoiceSubTypesId,
                    IsDeleted = item.IsDeleted,
                    ParentInvoiceCode = item.ParentInvoiceCode,
                    Serialize = item.Serialize,
                    StoreId = item.StoreId,
                    TotalPrice = item.TotalPrice,
                    employeesId = uInfo.employeeId,
                    BrowserName = uInfo.browserName.ToString(),

                };
                listOfInvoiceMasterHistory.Add(history);
            }


            SystemActionEnum systemActionEnum = new SystemActionEnum();
            if (oldInvoice.Where(x => x.InvoiceTypeId == (int)DocumentType.Sales || x.InvoiceTypeId == (int)DocumentType.ReturnSales).Any())
                systemActionEnum = SystemActionEnum.addSalesAccredite;
            else if (oldInvoice.Where(x => x.InvoiceTypeId == (int)DocumentType.Purchase || x.InvoiceTypeId == (int)DocumentType.ReturnPurchase).Any())
               systemActionEnum = SystemActionEnum.addPurchaseInvoicesAccredite;
            else if (oldInvoice.Where(x => x.InvoiceTypeId == (int)DocumentType.POS || x.InvoiceTypeId == (int)DocumentType.ReturnPOS).Any())
                systemActionEnum = SystemActionEnum.addPurchaseInvoicesAccredite;

            await _systemHistoryLogsService.SystemHistoryLogsService(systemActionEnum);

            InvoiceMasterHistoryRepositoryCommand.AddRange(listOfInvoiceMasterHistory);
            var saved = await InvoiceMasterHistoryRepositoryCommand.SaveAsync();
            return saved;
        }

        private int GetReceiptsType(int? safeID, int? bankId, int? ParentTypeId)
        {
            if (ParentTypeId == null)
                return 0;
            if (ParentTypeId == (int)DocumentType.Purchase || ParentTypeId == (int)DocumentType.ReturnSales || ParentTypeId == (int)DocumentType.ReturnPOS || ParentTypeId == (int)DocumentType.wov_purchase)
            {
                if (safeID != null && safeID > 0)//recepttype = سند صرف خزينه
                    return (int)DocumentType.SafePayment;
                else if (bankId != null && bankId > 0)
                    return (int)DocumentType.BankPayment;

            }
            if (ParentTypeId == (int)DocumentType.ReturnPurchase || ParentTypeId == (int)DocumentType.Sales || ParentTypeId == (int)DocumentType.POS)
            {
                if (safeID != null && safeID > 0)//recepttype = سند قبض خزينه
                    return (int)DocumentType.SafeCash;
                else if (bankId != null && bankId > 0)
                    return (int)DocumentType.BankCash;
            }
            return 0;
        }

        public async Task<ResponseResult> setAccreditReceipts(AccreditInvoiceRequest parameter)
        {
            #region validation

            DateTime AccreaditDate = DateTime.Now;
            UserInformationModel userInfo = await Userinformation.GetUserInformation();
            if (userInfo == null)
                return new ResponseResult() { Data = null, Result = Result.NoDataFound, ErrorMessageAr = "no taken request", ErrorMessageEn = "no taken request" };
            bool checkedInfo = await checkUserInfo(parameter.safeId, userInfo.userId, userInfo.userId, userInfo.CurrentbranchId);
            if (!checkedInfo)
                return new ResponseResult() { Data = null, Result = Result.Failed, ErrorMessageAr = "userPermissionError", ErrorMessageEn = "UserPermission Error" };
            int invReturn = GetInvoiceReturnType(parameter.InvType);

            if (invReturn <= 0)
                return new ResponseResult() { Data = null, Result = Result.Failed, ErrorMessageAr = "type Error", ErrorMessageEn = "Type Error" };


            #endregion

            //select invoice Data and update is accredit
            var oldInvoice = await InvoiceMasterRepositoryQuery.FindAllAsync(h =>
                                          (h.InvoiceTypeId == parameter.InvType || h.InvoiceTypeId == invReturn)
                                          && h.IsDeleted == false
                                          && h.IsAccredite == false
                                          && h.InvoiceDate <= parameter.date
                                          && h.BranchId == userInfo.CurrentbranchId
                                          && h.EmployeeId == parameter.userId
                                          );

            if (oldInvoice == null || oldInvoice.Count <= 0)
                return new ResponseResult() { Data = null, Result = Result.NoDataFound, ErrorMessageAr = "No Data for Accredit", ErrorMessageEn = "No Data for Accredit" };


            // update is accredit in invoice
            oldInvoice.ForEach(h => h.IsAccredite = true);
            bool save = await InvoiceMasterRepositorsCommand.UpdateAsyn(oldInvoice);

            var invoiceIds = oldInvoice.Select(h => h.InvoiceId).ToList();
            if (!save)
                return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = "error in invoceUpdate", ErrorMessageEn = "error in invoceUpdate" };

            //update receipts is accredit
            var SelectAccreditReceipts = await ReceiptCommand.Get(a => a.ParentId != null ? invoiceIds.Contains(a.ParentId.Value) : a.ParentId != null && a.IsAccredit == false);
            var ReceiptIs = SelectAccreditReceipts.Select(h => h.Id).ToList();
            foreach (var item in SelectAccreditReceipts)
            {

                if (item.SafeID != null)
                    item.SafeID = parameter.safeId;
                item.IsAccredit = true;
                item.RecieptDate = AccreaditDate;
            }
            save = await ReceiptCommand.SaveAsync();
            if (!save)
                return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = "error in receiptsUpdate", ErrorMessageEn = "error in receiptsUpdate" };

            //update journal Entery is accredit

            var SelectAccreditJEnery = await JournalenteryCommand.Get(a => a.ReceiptsId != null ? ReceiptIs.Contains(a.ReceiptsId.Value) : a.ReceiptsId != null && a.IsAccredit == false);

            foreach (var item in SelectAccreditJEnery)
            {

                item.IsAccredit = true;
                item.Code = JournalenteryQuery.GetMaxCode(e => e.Code, e => e.Code >= 0) + 1;
                item.FTDate = AccreaditDate;
            }
            if (SelectAccreditJEnery.Count > 0)
                save = await JournalenteryCommand.SaveAsync();
            if (!save)
                return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = "error in JEnteryUpdate", ErrorMessageEn = "error in JEnteryUpdate" };

            save = await setInvoiceAccreditHistory(oldInvoice, userInfo);
            if (!save)
                return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = "error in add history", ErrorMessageEn = "error in Add History" };

            return new ResponseResult() { Result = Result.Success };
        }
        //third time
        public async Task<ResponseResult> AccrediteInvoiceLast(AccreditInvoiceRequest parameter)
        { 
            try
            {


                #region validation

                DateTime AccreaditDate = DateTime.Now;
                UserInformationModel userInfo = await Userinformation.GetUserInformation();
                if (userInfo == null)
                    return new ResponseResult() { Data = null, Result = Result.NoDataFound, ErrorMessageAr = "no taken request", ErrorMessageEn = "no taken request" };

                //invoiceReturn
                int invReturn = GetInvoiceReturnType(parameter.InvType);

                if (invReturn <= 0)
                    return new ResponseResult() { Data = null, Result = Result.Failed, ErrorMessageAr = "type Error", ErrorMessageEn = "Type Error" };

                 
                #endregion

                //select invoice Data and update is accredit
                var oldInvoice = await InvoiceMasterRepositoryQuery.FindAllAsync(h =>
                                              (h.InvoiceTypeId == parameter.InvType || h.InvoiceTypeId == invReturn)
                                              && h.IsDeleted == false
                                              && h.IsAccredite == false
                                              && h.InvoiceDate <= parameter.date
                                              && h.BranchId == userInfo.CurrentbranchId
                                              && h.EmployeeId == parameter.userId
                                              );

                if (oldInvoice == null || oldInvoice.Count <= 0)
                {
                    return new ResponseResult() { Data = null, Result = Result.NoDataFound, ErrorMessageAr = "No Data for Accredit", ErrorMessageEn = "No Data for Accredit" };
                }

                // update is accredit in invoice

                InvoiceMasterRepositorsCommand.StartTransaction();
                oldInvoice.ForEach(h => h.IsAccredite = true);
                //ReceiptCommand.StartTransaction();
                //JournalenteryCommand.StartTransaction();
                //journalEntryDetailsCommand.StartTransaction();

                bool saveinvMaster = false;
                var SelectAccreditReceipts = new List<GlReciepts>();
                
                int batchSize = 2000;
                for (int i = 0; i < oldInvoice.Count; i += batchSize)
                {
                    var currentBatch = oldInvoice.Skip(i).Take(batchSize).ToList();

                    saveinvMaster = await InvoiceMasterRepositorsCommand.UpdateAsynWithOutSave(currentBatch);
                    var invoiceIds = currentBatch.Select(h => h.InvoiceId).ToList();

                    SelectAccreditReceipts.AddRange(await ReceiptQuery.Get(a => a
                                                                        , (a => (a.ParentId != null ? invoiceIds.Contains(a.ParentId.Value) : a.ParentId != null) && a.IsAccredit == false),
                                                                          a => a.Safes, a => a.Banks, a => a.person));
                    
                }


                if (!saveinvMaster)
                {
                    return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = "error in invoceUpdate", ErrorMessageEn = "error in invoceUpdate" };
                }

                //update receipts is accredit
                //var SelectAccreditReceipts = await ReceiptCommand.Get(a => a.ParentId != null ? invoiceIds.Contains(a.ParentId.Value) : a.ParentId != null && a.IsAccredit == false && a.Deferre == true);
                //var SelectAccreditReceipts = await ReceiptQuery.Get(a => a
                //                                                        , (a => (a.ParentId != null ? invoiceIds.Contains(a.ParentId.Value) : a.ParentId != null) && a.IsAccredit == false),
                //                                                          a => a.Safes, a => a.Banks, a => a.person);

                var ReceiptIs = SelectAccreditReceipts.Select(h => h.Id).ToList();

                // if compined
                var reciptTypeInfo = GetCompinedRecieptType(SelectAccreditReceipts.Select(a=>a.RecieptTypeId).FirstOrDefault(), parameter.IsCompined);
                int masterId = 0;
                int code = 1;
                if (parameter.IsCompined)
                {
                    code = await ReceiptsServices.Autocode(reciptTypeInfo.Item1, userInfo.CurrentbranchId, parameter.IsCompined);
                    masterId = await AddCompinedMasterReciept(SelectAccreditReceipts, code, AccreaditDate, reciptTypeInfo);
                    if (masterId == 0)
                        return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = "error in receiptsUpdate", ErrorMessageEn = "error in receiptsUpdate" };
                }

                var list = SelectAccreditReceipts.GroupBy(h => h.RecieptTypeId);
                //InvoiceMasterRepositorsCommand.CommitTransaction();

                foreach (var type in list)
                {
                    
                    var recTypeInfo = GetCompinedRecieptType(type.First().RecieptTypeId, parameter.IsCompined);
                    // int c = await ReceiptsServices.Autocode(recTypeInfo.Item1, userInfo.CurrentbranchId, parameter.IsCompined);

                    foreach (var item in SelectAccreditReceipts.OrderBy(h => h.ParentId).Where(e => e.RecieptTypeId == type.First().RecieptTypeId))
                    {
                        if (item.SafeID != null)
                            item.SafeID = parameter.safeId;

                        item.IsAccredit = true;
                        item.RecieptDate = AccreaditDate;

                        if (parameter.IsCompined)
                        {
                            item.CompinedParentId = masterId;
                            item.RecieptTypeId = recTypeInfo.Item1;
                            item.Code = code;
                            item.IsCompined = true;
                            item.RecieptType = item.BranchId + "-" + recTypeInfo.Item2 + "-" + code;
                            item.PaperNumber = item.BranchId + "-" + recTypeInfo.Item2 + "-" + code;
                            item.NoteAR = string.Concat(recTypeInfo.Item3, " _ ", item.RecieptType);
                            item.NoteEN = string.Concat(recTypeInfo.Item4, " _ ", item.RecieptType);

                        }
                        else if(recTypeInfo!=null)
                        {
                            item.Code = item.Deferre == false ? await ReceiptsServices.Autocode(recTypeInfo.Item1, userInfo.CurrentbranchId, parameter.IsCompined) : 0;
                            item.RecieptType = item.Deferre == false ? item.BranchId + "-" + recTypeInfo.Item2 + "-" + item.Code : null;
                            item.NoteAR = string.Concat(recTypeInfo.Item3, " _ ", item.RecieptType);
                            item.NoteEN = string.Concat(recTypeInfo.Item4, " _ ", item.RecieptType);
                        }
                        var saveRec = await ReceiptCommand.SaveAsync();
                        if (!saveRec)
                        {
                            return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = "error in receiptsUpdate", ErrorMessageEn = "error in receiptsUpdate" };
                        }
                    }
                        
                    //ReceiptCommand.AddAsync(item);   
                }

              //  await ReceiptCommand.SaveAsync();



                var receiptForJournalEnery = SelectAccreditReceipts.Where(a => a.Deferre == false);
                var SaveGEntery = false;
                foreach (var item in oldInvoice)
                {
                    var listOfData = receiptForJournalEnery.Where(a => a.ParentId == item.InvoiceId).ToList();
                    SaveGEntery = await updateRecieptsInJournalEntry(listOfData, item.InvoiceId, parameter.safeId, item.InvoiceTypeId);
                }
                var journalentrySaved = await JournalenteryCommand.SaveAsync();
                var journalentryDetailSaved = await journalEntryDetailsCommand.SaveAsync();

                var SetHis = await setHistory(oldInvoice, ReceiptIs, userInfo);

                if (saveinvMaster && SetHis)
                {
                    InvoiceMasterRepositorsCommand.CommitTransaction();
                    //ReceiptCommand.CommitTransaction();
                    //JournalenteryCommand.CommitTransaction();
                    // journalEntryDetailsCommand.CommitTransaction();
                    return new ResponseResult() { Result = Result.Success };
                }
                else
                {
                    InvoiceMasterRepositorsCommand.Rollback();
                    //ReceiptCommand.Rollback();
                    //JournalenteryCommand.Rollback();
                    //journalEntryDetailsCommand.Rollback();
                    return new ResponseResult() { Result = Result.Failed };
                }


            }
            catch (Exception e)
            {
                InvoiceMasterRepositorsCommand.Rollback();
                //ReceiptCommand.Rollback();
                //JournalenteryCommand.Rollback();
                //journalEntryDetailsCommand.Rollback();

                return new ResponseResult() { Result = Result.Failed, Note = e.Message + " exeption " };
            }
        }

        public async Task<financialData> getFinantialAccIdForPerson(int type, int ID)
        {
            var FA = await PersonQuery.TableNoTracking.Where(a => a.Id == ID && (type == AuthorityTypes.customers ? a.IsCustomer == true : a.IsSupplier == true))
                            .Include(h => h.FinancialAccount)
                            .Select(s => new financialData()
                            {
                                financialId = s.FinancialAccountId.GetValueOrDefault(0),
                                financialCode = s.FinancialAccount.AccountCode,
                                FinancialName = s.FinancialAccount.ArabicName
                            }).FirstOrDefaultAsync();

            return FA;
        }
        public async Task<int> AddCompinedMasterReciept(List<GlReciepts> req, int code, DateTime recdate, Tuple<int, string, string, string> recieptTypeInfo)
        {
            GlReciepts master = new GlReciepts();
            master.Code = code;
            master.BranchId = req.First().BranchId;
            master.UserId = req.First().UserId;
            master.RecieptTypeId = recieptTypeInfo.Item1;
            master.RecieptDate = recdate;
            master.SafeID = req.First().SafeID == 0 ? null : req.First().SafeID;
            master.BankId = req.First().BankId == 0 ? null : req.First().BankId;
            master.IsCompined = true;
            master.PaymentMethodId = 1;
            master.RecieptType = req.First().BranchId + "-" + recieptTypeInfo.Item2 + "-" + code;
            master.PaperNumber = req.First().BranchId + "-" + recieptTypeInfo.Item2 + "-" + code;
            master.Amount = roundNumbers.GetDefultRoundNumber(req.Sum(a => a.Amount));
            master.NoteAR = string.Concat(recieptTypeInfo.Item3, " _ ", master.RecieptType);
            master.NoteEN = string.Concat(recieptTypeInfo.Item4, " _ ", master.RecieptType);

            var saved = await ReceiptCommand.AddAsync(master);
            return saved ? master.Id : 0;
        }
        
        public Tuple<int, string, string, string> GetCompinedRecieptType(int recieptType , bool isCompined)
        {
            if(isCompined)
            {

                if (recieptType == (int)DocumentType.BankCash)
                    return new Tuple<int, string, string, string>((int)DocumentType.CompinedBankCash, InvoicesCode.CompinedBankCash, NotesOfReciepts.CompinedBankCashAr, NotesOfReciepts.CompinedBankCashEn);
                else if (recieptType == (int)DocumentType.BankPayment)
                    return new Tuple<int, string, string, string>((int)DocumentType.CompinedBankPayment, InvoicesCode.CompinedBankPayment, NotesOfReciepts.CompinedBankPaymentAr, NotesOfReciepts.CompinedBankPaymentEn);
                else if (recieptType == (int)DocumentType.SafeCash)
                    return new Tuple<int, string, string, string>((int)DocumentType.CompinedSafeCash, InvoicesCode.CompinedSafeCash, NotesOfReciepts.CompinedSafeCashAr, NotesOfReciepts.CompinedSafeCashEn);
                else if (recieptType == (int)DocumentType.SafePayment)
                    return new Tuple<int, string, string, string>((int)DocumentType.CompinedSafePayment, InvoicesCode.CompinedSafePayment, NotesOfReciepts.CompinedSafePaymentAr, NotesOfReciepts.CompinedSafePaymentEn);

            }
            else
            {
                if (recieptType == (int)DocumentType.BankCash)
                    return new Tuple<int, string, string, string>((int)DocumentType.BankCash, InvoicesCode.BankCash, NotesOfReciepts.BankCashAr, NotesOfReciepts.BankCashEn);
                else if (recieptType == (int)DocumentType.BankPayment)
                    return new Tuple<int, string, string, string>((int)DocumentType.BankPayment, InvoicesCode.BankPayment, NotesOfReciepts.BankPaymentAr, NotesOfReciepts.BankPaymentEn);
                else if (recieptType == (int)DocumentType.SafeCash)
                    return new Tuple<int, string, string, string>((int)DocumentType.SafeCash, InvoicesCode.SafeCash, NotesOfReciepts.SafeCashAr, NotesOfReciepts.SafeCashEn);
                else if (recieptType == (int)DocumentType.SafePayment)
                    return new Tuple<int, string, string, string>((int)DocumentType.SafePayment, InvoicesCode.SafePayment, NotesOfReciepts.SafePaymentAr, NotesOfReciepts.SafePaymentEn);

            }
            return null;
        }

        private async Task<bool> updateRecieptsInJournalEntry(List<GlReciepts> data, int invoiceId, int safeID, int docType)
        {
            // update journal entiry of safe and bank 
            try
            {
                var costCenterRecieptsList = new List<GLRecieptCostCenter>();
                //need refactor for this code               
                int journalEntryId = 0;
                // here
                var journalEntry = JournalenteryQuery.TableNoTracking.Where(x => x.InvoiceId == invoiceId && x.DocType == docType).FirstOrDefault();

                if (journalEntry == null)
                {
                    return false; 
                }
                journalEntryId = journalEntry.Id;
                UpdateJournalEntryfromAccredit JEntry = new UpdateJournalEntryfromAccredit();
                JEntry.Id = journalEntryId;
                JEntry.Notes = journalEntry.Notes;
                JEntry.FTDate = journalEntry.FTDate;
                JEntry.IsAccredit = true;
                JEntry.BranchId = journalEntry.BranchId;
                //JEntry.journalEntryDetails.AddRange(GetOldJournalEnteryDetails(journalEntry.JournalEntryDetails.ToList()));
                foreach (var receipts in data)
                {
                    if (receipts.PaymentMethodId == (int)PaymentMethod.PersonBalance)
                        continue;
                    int finantialBankOrSafe = getSafeFinancialAccount(receipts, safeID);
                    int FinancialBenfituser = getFinancialBenfitUser(receipts);
                    JEntry.journalEntryDetails.AddRange(getJournalEnteryDetails(receipts, finantialBankOrSafe, FinancialBenfituser, JEntry.Id));
                }



                #region costcenter


                ////فى حاله لو فيه مراكز تكلفه  هينفذ التالى
                //if (parameter.costCenterReciepts != null)
                //{
                //    double totalCostcenterAmount = parameter.costCenterReciepts.Sum(h => h.Number);
                //    foreach (var item in parameter.costCenterReciepts)
                //    {
                //        //add new journal enitity details for every costcenter
                //        JEntry.journalEntryDetails.Add(new JournalEntryDetail()
                //        {
                //            FinancialAccountId = financialForBenfiteUser.financialId,
                //            FinancialCode = financialForBenfiteUser.financialCode,
                //            FinancialName = financialForBenfiteUser.FinancialName,
                //            Credit = data.Signal > 0 ? item.Number : 0,
                //            Debit = data.Signal < 0 ? item.Number : 0,
                //            CostCenterId = item.CostCenterId,
                //            DescriptionAr = data.NoteAR,
                //            DescriptionEn = data.NoteEN,
                //        });
                //        //add costcent in table to use it un get data
                //        #region save costcenter
                //        var center = Mapping.Mapper.Map<UpdateCostCenterReciepts, GLRecieptCostCenter>(item);
                //        center.SupportId = data.Id;
                //        costCenterRecieptsList.Add(center);
                //        #endregion

                //    }
                //    bool Costsave = await costCenterREcieptCommand.AddAsync(costCenterRecieptsList);

                //    if (totalCostcenterAmount < data.Amount)
                //    {
                //        double restAmount = data.Amount - totalCostcenterAmount;
                //        JEntry.journalEntryDetails.Add(new JournalEntryDetail()
                //        {
                //            FinancialAccountId = financialForBenfiteUser.financialId,
                //            FinancialCode = financialForBenfiteUser.financialCode,
                //            FinancialName = financialForBenfiteUser.FinancialName,
                //            Credit = data.Signal > 0 ? restAmount : 0,
                //            Debit = data.Signal < 0 ? restAmount : 0,
                //            DescriptionAr = data.NoteAR,
                //            DescriptionEn = data.NoteEN,
                //        });
                //    }

                //}
                //else
                //    JEntry.journalEntryDetails.Add(new JournalEntryDetail()
                //    {
                //        FinancialAccountId = financialForBenfiteUser.financialId,
                //        FinancialCode = financialForBenfiteUser.financialCode,
                //        FinancialName = financialForBenfiteUser.FinancialName,
                //        Credit = data.Signal > 0 ? data.Amount : 0,
                //        Debit = data.Signal < 0 ? data.Amount : 0,
                //        DescriptionAr = data.NoteAR,
                //        DescriptionEn = data.NoteEN,
                //    });

                #endregion


                var res = await updateJournalEntery(JEntry);
                return res;
            }
            catch (Exception e)
            {

                string x = e.Message;
                return false;
            }

        }
        public async Task<bool> updateJournalEntery(UpdateJournalEntryfromAccredit parameter)
        {



            var list = new List<UpdateJournalEntryParameter>();
            var journalentry = await JournalenteryQuery.GetByAsync(q => q.Id == parameter.Id);
            var table = new UpdateJournalEntryParameter();
            journalentry.BranchId = parameter.BranchId;
            // journalentry.CurrencyId = currencyRepositoryQuery.TableNoTracking.Where(x => x.IsDefault).FirstOrDefault().Id;
            journalentry.FTDate = parameter.FTDate;
            journalentry.Notes = parameter.Notes != "null" ? parameter.Notes : "";
            // journalentry.IsTransfer = journalentry.IsTransfer;
            journalentry.CreditTotal += parameter.journalEntryDetails.Sum(a => a.Credit);
            journalentry.DebitTotal += parameter.journalEntryDetails.Sum(a => a.Debit);
            journalentry.BrowserName = contextHelper.GetBrowserName(httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString());


            //here save only
            // var journalentrySaved = await JournalenteryCommand.SaveAsync();

            journalEntryDetailsCommand.AddRange(parameter.journalEntryDetails);

            return true;


        }

        private int getFinancialBenfitUser(GlReciepts receipts)
        {
            int financialId = 0;
            if (receipts.Authority == AuthorityTypes.customers || receipts.Authority == AuthorityTypes.suppliers)
            {
                financialId = receipts.person.FinancialAccountId.Value;
            }

            else if (receipts.Authority == AuthorityTypes.salesman)
            {
            }
            else if (receipts.Authority == AuthorityTypes.DirectAccounts)
            {
            }
            else if (receipts.Authority == AuthorityTypes.other)
            {
            }
            return financialId;
        }

        private int getSafeFinancialAccount(GlReciepts receipts, int safeId)
        {
            int financialId = 0;
            if (receipts.BankId == null)
                financialId = (int)SafeQuery.TableNoTracking.Where(h => h.Id == safeId).Select(a => a.FinancialAccountId).FirstOrDefault();
            else
                financialId = receipts.Banks.FinancialAccountId.Value;

            return financialId;
        }
        private List<GLJournalEntryDetails> getJournalEnteryDetails(GlReciepts data, int? financialIdOfSafeOfBank, int financialForBenfiteUser, int jEnteryId)
        {
            List<GLJournalEntryDetails> journalEntryDetails = new List<GLJournalEntryDetails>();
            journalEntryDetails.Add(new GLJournalEntryDetails()//add the main data of journal entery
            {
                JournalEntryId = jEnteryId,
                FinancialAccountId = financialIdOfSafeOfBank,
                Credit = data.Signal < 0 ? data.Amount : 0,
                Debit = data.Signal > 0 ? data.Amount : 0,
                DescriptionAr = data.NoteAR,
                DescriptionEn = data.NoteEN,

            });
            #region
            ////فى حاله لو فيه مراكز تكلفه  هينفذ التالى
            //if (parameter.costCenterReciepts != null)
            //{
            //    double totalCostcenterAmount = parameter.costCenterReciepts.Sum(h => h.Number);
            //    foreach (var item in parameter.costCenterReciepts)
            //    {
            //        //add new journal enitity details for every costcenter
            //        journalEntryDetails.Add(new JournalEntryDetail()
            //        {
            //            FinancialAccountId = financialForBenfiteUser.financialId,
            //            FinancialCode = financialForBenfiteUser.financialCode,
            //            FinancialName = financialForBenfiteUser.FinancialName,
            //            Credit = data.Signal > 0 ? item.Number : 0,
            //            Debit = data.Signal < 0 ? item.Number : 0,
            //            CostCenterId = item.CostCenterId,
            //            DescriptionAr = data.NoteAR,
            //            DescriptionEn = data.NoteEN,
            //        });
            //        //add costcent in table to use it un get data
            //        #region save costcenter
            //        var center = Mapping.Mapper.Map<UpdateCostCenterReciepts, GLRecieptCostCenter>(item);
            //        center.SupportId = data.Id;
            //        costCenterRecieptsList.Add(center);
            //        #endregion

            //    }
            //    bool Costsave = await costCenterREcieptCommand.AddAsync(costCenterRecieptsList);

            //    if (totalCostcenterAmount < data.Amount)
            //    {
            //        double restAmount = data.Amount - totalCostcenterAmount;
            //        journalEntryDetails.Add(new JournalEntryDetail()
            //        {
            //            FinancialAccountId = financialForBenfiteUser.financialId,
            //            FinancialCode = financialForBenfiteUser.financialCode,
            //            FinancialName = financialForBenfiteUser.FinancialName,
            //            Credit = data.Signal > 0 ? restAmount : 0,
            //            Debit = data.Signal < 0 ? restAmount : 0,
            //            DescriptionAr = data.NoteAR,
            //            DescriptionEn = data.NoteEN,
            //        });
            //    }

            //}
            #endregion

            journalEntryDetails.Add(new GLJournalEntryDetails()
            {
                JournalEntryId = jEnteryId,
                FinancialAccountId = financialForBenfiteUser,
                Credit = data.Signal > 0 ? data.Amount : 0,
                Debit = data.Signal < 0 ? data.Amount : 0,
                DescriptionAr = data.NoteAR,
                DescriptionEn = data.NoteEN,
            });

            return journalEntryDetails;
        }
        private List<JournalEntryDetail> GetOldJournalEnteryDetails(List<GLJournalEntryDetails> LST_JEneryDetails)
        {
            List<JournalEntryDetail> journalEntryDetails = new List<JournalEntryDetail>();
            foreach (var JEneryDetails in LST_JEneryDetails)
            {


                journalEntryDetails.Add(new JournalEntryDetail()//add the main data of journal entery
                {
                    FinancialAccountId = JEneryDetails.FinancialAccountId,
                    Credit = JEneryDetails.Credit,
                    Debit = JEneryDetails.Debit,
                    DescriptionAr = JEneryDetails.DescriptionAr,
                    DescriptionEn = JEneryDetails.DescriptionEn,

                });
            }
            return journalEntryDetails;
        }
        public class ClosingDataForReport : ReportOtherData
        {
            public DateTime ClosingDate { get; set; }
            //public int SessionCode { get; set; }
            //public string StartSession { get; set; }
            //public string EndtSession { get; set; }

        }

    }

}
