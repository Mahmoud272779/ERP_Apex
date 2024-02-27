using App.Domain.Entities.Process;
using App.Domain.Models.Response.GeneralLedger;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static App.Domain.Enums.Enums;
using App.Infrastructure;
using static App.Application.Helpers.Aliases;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using FastReport.Web;
using App.Domain.Enums;
using App.Domain.Entities;
using App.Domain.Models.Request;
using static App.Application.Services.Reports.StoreReports.RPT_BanksSafesServices.safeOrBankReportService;
using System.Data;
using System.Reflection;
using App.Application.Helpers;
using App.Application.Services.Process.Company_data;
using App.Application.Services.Process.StoreServices.Invoices.AccrediteInvoice;
using FastReport.Data;
using App.Domain.Models.Response.Store.Reports.Purchases;
using static App.Domain.Models.Security.Authentication.Response.Totals;
using App.Application.Services.Printing.InvoicePrint;

namespace App.Application.Services.Process.GLServices.ledger_Report
{
    public class LedgerReportService : ILedgerReportService
    {
        private readonly IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsQuery;
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountQuery;
        private readonly IRoundNumbers roundNumbers;
        private readonly IRepositoryQuery<InvGeneralSettings> invGeneralSettingsQuery;
        private readonly IPrintService _iprintService;

        private readonly IFilesMangerService _filesMangerService;
        private readonly ICompanyDataService _CompanyDataService;
        private readonly iUserInformation _iUserInformation;
        private readonly IGeneralPrint _iGeneralPrint;
        string previousBalance = "";

        public LedgerReportService(
              IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsQuery,
              IRepositoryQuery<GLFinancialAccount> financialAccountQuery,
              IRoundNumbers roundNumbers,
              IRepositoryQuery<InvGeneralSettings> invGeneralSettingsQuery,
              IPrintService iprintService, IFilesMangerService filesMangerService,
              ICompanyDataService CompanyDataService, iUserInformation iUserInformation, IGeneralPrint iGeneralPrint)
        {
            this.journalEntryDetailsQuery = journalEntryDetailsQuery;
            this.financialAccountQuery = financialAccountQuery;
            this.roundNumbers = roundNumbers;
            this.invGeneralSettingsQuery = invGeneralSettingsQuery;
            _iprintService = iprintService;
            _filesMangerService = filesMangerService;
            _CompanyDataService = CompanyDataService;
            _iUserInformation = iUserInformation;
            _iGeneralPrint = iGeneralPrint;
        }

        public async Task<ResponseResult> GetLedgerData(PageParameterLedgerReport paramters, bool isPrint = false)
        {
            if (paramters.From > paramters.To)
                return new ResponseResult { Result = Result.RequiredData, ErrorMessageAr = ErrorMessagesAr.StartDateAfterEndDate, ErrorMessageEn = ErrorMessagesEn.StartDateAfterEndDate };
            var MidpointRounding = invGeneralSettingsQuery.TableNoTracking.First().Other_Decimals;
            var financialCodeParameter = financialAccountQuery.TableNoTracking
                                                              .Where(a => (!string.IsNullOrEmpty(paramters.AccountId.ToString()) ? a.Id == paramters.AccountId : true))
                                                              .Select(a => new { a.autoCoding, a.ArabicName, a.LatinName})
                                                              .First();

            var JE_Detalis = journalEntryDetailsQuery
                                            .TableNoTracking
                                            .Include(a => a.GLFinancialAccount)
                                            .Include(navigationPropertyPath: a => a.journalEntry)
                                            .ThenInclude(a => a.currency)
                                            .Where(x => !x.journalEntry.IsBlock)
                                            .Where(a => (!string.IsNullOrEmpty(financialCodeParameter.autoCoding) ? ((a.GLFinancialAccount.autoCoding == financialCodeParameter.autoCoding
                                            || (paramters.DecumentId == (int)SubFormsIds.AccountStatementDetail_GL ? 0==1 : (   a.GLFinancialAccount.autoCoding.StartsWith(financialCodeParameter.autoCoding))))
                                            && a.GLFinancialAccount.IsMain == false) : true));
            if(paramters.DecumentId== (int)SubFormsIds.LedgerReport_GL)
            {
                if (JE_Detalis.Where(a => a.GLFinancialAccount.autoCoding == financialCodeParameter.autoCoding).Count() > 0)
                {
                    JE_Detalis = JE_Detalis.Where(a => a.GLFinancialAccount.autoCoding == financialCodeParameter.autoCoding);
                }
                else
                    JE_Detalis = JE_Detalis.Where(a => a.GLFinancialAccount.autoCoding.StartsWith(financialCodeParameter.autoCoding + "."));
            }
           


            // get credit and debit from details for each journal entry
            var detailsDataOfJournalEntry = JE_Detalis
                                            .Where(x => x.journalEntry.FTDate.Value.Date >= paramters.From.Date && x.journalEntry.FTDate.Value.Date <= paramters.To.Date)
                                            .Select(a => new
                                            {
                                                financialAccountId = a.GLFinancialAccount.Id,
                                                financialAccountNameAr = a.GLFinancialAccount.ArabicName,
                                                financialAccountNameEn = a.GLFinancialAccount.LatinName,
                                                financialAccountCode = a.GLFinancialAccount.AccountCode.Replace(".", ""),
                                                journalEntryDate = a.journalEntry.FTDate,
                                                journalEntryId = a.journalEntry.Id,
                                                journalEntryCode = a.journalEntry.Code,
                                                notes = a.DescriptionAr,
                                                notesEn = a.DescriptionEn,
                                                transCredit = a.Credit * a.journalEntry.currency.Factor,
                                                transDebit = a.Debit * a.journalEntry.currency.Factor,
                                                prevCredit = 0.0,
                                                prevDebit = 0.0
                                            })
                                            .OrderBy(a => a.financialAccountId)
                                            .ThenBy(a => a.journalEntryDate)
                                            .ThenBy(a => a.journalEntryId)
                                            .ToList();




            var previousBalance = JE_Detalis
                                 .Where(x => x.journalEntry.FTDate.Value.Date < paramters.From.Date)
                                 .ToList()
                                 .GroupBy(e => new { e.GLFinancialAccount.Id })
                                 .Select(a => new
                                 {
                                     financialAccountId = a.Key.Id,
                                     financialAccountNameAr = a.First().GLFinancialAccount.ArabicName,
                                     financialAccountNameEn = a.First().GLFinancialAccount.LatinName,
                                     financialAccountCode = a.First().GLFinancialAccount.AccountCode.Replace(".", ""),
                                     prevCredit = a.Sum(e => e.Credit * e.journalEntry.currency.Factor),
                                     prevDebit = a.Sum(e => e.Debit * e.journalEntry.currency.Factor)
                                 })
                                 .OrderBy(a => a.financialAccountId)
                                 .ToList();

            var totalLedgerResponse = new List<LedgerResponse>();
            var totalBalanceOfAccount = 0.0;
            var AccountsInPeriod = new List<int>();



            var prevOfAcc = 0.0;

            for (var i = 0; i < detailsDataOfJournalEntry.Count(); i++)
            {
                var ledgerResponse = new LedgerResponse();


                // if account changed set the previous balance
                if (i == 0 || detailsDataOfJournalEntry[i - 1].financialAccountId != detailsDataOfJournalEntry[i].financialAccountId)
                {
                    // Set row of financial account and previous balance
                    ledgerResponse.financialAccountId = detailsDataOfJournalEntry[i].financialAccountId;
                    ledgerResponse.financialAccountCode = detailsDataOfJournalEntry[i].financialAccountCode.Replace(".", "");
                    ledgerResponse.financialAccountNameAr = detailsDataOfJournalEntry[i].financialAccountNameAr;
                    ledgerResponse.financialAccountNameEn = detailsDataOfJournalEntry[i].financialAccountNameEn;
                    //ledgerResponse.journalEntryDate = Convert.ToDateTime( detailsDataOfJournalEntry[i].journalEntryDate );

                    if (previousBalance.Count > 0)
                    {
                        var prev = previousBalance.Where(a => a.financialAccountId == detailsDataOfJournalEntry[i].financialAccountId)
                          .Select(a => a.prevCredit - a.prevDebit).ToList();

                        if (prev.Count() > 0)
                        {
                            totalBalanceOfAccount = prev.First();

                            //   if (totalBalanceOfAccount > 0)
                            //         prevOfAcc = totalBalanceOfAccount;
                            //     else
                            prevOfAcc = totalBalanceOfAccount;

                            ledgerResponse.PreviousBalance = totalBalanceOfAccount;
                            //else
                            //    ledgerResponse.PreviousBalance = -totalOfBalance;

                            AccountsInPeriod.Add(detailsDataOfJournalEntry[i].financialAccountId);
                        }

                    }

                    totalLedgerResponse.Add(ledgerResponse);

                    // Set row for Header
                    ledgerResponse = new LedgerResponse();
                    ledgerResponse.financialAccountId = detailsDataOfJournalEntry[i].financialAccountId;
                    ledgerResponse.financialAccountCode = "#$[HEADER]$%";
                    totalLedgerResponse.Add(ledgerResponse);


                }

                ledgerResponse = new LedgerResponse();
                ledgerResponse.financialAccountId = detailsDataOfJournalEntry[i].financialAccountId;
                ledgerResponse.journalEntryId = detailsDataOfJournalEntry[i].journalEntryCode;
                ledgerResponse.journalEntryDate = detailsDataOfJournalEntry[i].journalEntryDate.Value;
                ledgerResponse.notes = detailsDataOfJournalEntry[i].notes;
                ledgerResponse.notesEn = detailsDataOfJournalEntry[i].notesEn;
                ledgerResponse.journalEntryDate = Convert.ToDateTime( detailsDataOfJournalEntry[i].journalEntryDate);

                ledgerResponse.transCredit = roundNumbers.GetRoundNumber(detailsDataOfJournalEntry[i].transCredit);
                ledgerResponse.transDebit = roundNumbers.GetRoundNumber(detailsDataOfJournalEntry[i].transDebit);

                // clc balance of all journal entry in account
                totalBalanceOfAccount += (detailsDataOfJournalEntry[i].transCredit - detailsDataOfJournalEntry[i].transDebit);
                if (totalBalanceOfAccount > 0)
                    ledgerResponse.balanceCredit = roundNumbers.GetRoundNumber(totalBalanceOfAccount);
                else
                    ledgerResponse.balanceDebit = -roundNumbers.GetRoundNumber(totalBalanceOfAccount);


                totalLedgerResponse.Add(ledgerResponse);
                // if account changed set the total
                if (i == detailsDataOfJournalEntry.Count() - 1 || detailsDataOfJournalEntry[i].financialAccountId != detailsDataOfJournalEntry[i + 1].financialAccountId)
                {
                    totalBalanceOfAccount = 0.0;
                    ledgerResponse = new LedgerResponse();
                    ledgerResponse.financialAccountId = detailsDataOfJournalEntry[i].financialAccountId;
                    ledgerResponse.financialAccountCode = "#$[TOTAL]$%";
                    ledgerResponse.transCredit = roundNumbers.GetRoundNumber(detailsDataOfJournalEntry.Where(a => a.financialAccountId == detailsDataOfJournalEntry[i].financialAccountId).Sum(a => a.transCredit));
                    ledgerResponse.transDebit = roundNumbers.GetRoundNumber(detailsDataOfJournalEntry.Where(a => a.financialAccountId == detailsDataOfJournalEntry[i].financialAccountId).Sum(a => a.transDebit));
                    if (prevOfAcc > 0)
                        ledgerResponse.transCredit += prevOfAcc;
                    else
                        ledgerResponse.transDebit += -prevOfAcc;
                    ledgerResponse.balanceCredit = totalLedgerResponse.Last().balanceCredit;
                    ledgerResponse.balanceDebit = totalLedgerResponse.Last().balanceDebit;
                    totalLedgerResponse.Add(ledgerResponse);
                    // totalBalance += ledgerResponse.balanceCredit - ledgerResponse.balanceDebit;

                    // Set empty row to separate  financial accounts
                    ledgerResponse = new LedgerResponse();
                    ledgerResponse.financialAccountId = detailsDataOfJournalEntry[i].financialAccountId;
                    ledgerResponse.financialAccountCode = "#$[EMPTY]$%";
                    totalLedgerResponse.Add(ledgerResponse);

                }
            }
            // Set Previous balance of financial account not in period
            var TotalPrevBalanceCredit = 0.0;
            var TotalPrevBalanceDebit = 0.0;
            if (previousBalance.Count() > 0)
            {
                var prevOutOfPeriod = previousBalance.Where(a => !AccountsInPeriod.Contains(a.financialAccountId)).ToList();

                foreach (var account in prevOutOfPeriod)
                {
                    var ledgerResponse = new LedgerResponse();
                    // Set row of financial account and previous balance
                    ledgerResponse.financialAccountId = account.financialAccountId;
                    ledgerResponse.financialAccountCode = account.financialAccountCode.Replace(".", "");
                    ledgerResponse.financialAccountNameAr = account.financialAccountNameAr;
                    ledgerResponse.financialAccountNameEn = account.financialAccountNameEn;
                    totalBalanceOfAccount = roundNumbers.GetRoundNumber(account.prevCredit - account.prevDebit);
                    ledgerResponse.PreviousBalance = roundNumbers.GetRoundNumber(totalBalanceOfAccount);
                    
                    TotalPrevBalanceCredit += account.prevCredit;
                    TotalPrevBalanceDebit += account.prevDebit;

                    totalLedgerResponse.Add(ledgerResponse);
                    //  totalBalance += totalBalanceOfAccount;

                    // Set empty row of nothing data
                    ledgerResponse = new LedgerResponse();
                    ledgerResponse.financialAccountId = account.financialAccountId;
                    ledgerResponse.financialAccountCode = "#$[NoDATA]$%";
                    totalLedgerResponse.Add(ledgerResponse);

                    // Set empty row to separate  financial accounts
                    ledgerResponse = new LedgerResponse();
                    ledgerResponse.financialAccountId = account.financialAccountId;
                    ledgerResponse.financialAccountCode = "#$[EMPTY]$%";
                    totalLedgerResponse.Add(ledgerResponse);

                }
            }


            totalLedgerResponse.OrderBy(a => a.financialAccountId).ToList();

            var dataCount = totalLedgerResponse.Count();
            double MaxPageNumber = dataCount / Convert.ToDouble(paramters.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);

            
            var finalResult = isPrint ? totalLedgerResponse : totalLedgerResponse.Skip((paramters.PageNumber - 1) * paramters.PageSize).Take(paramters.PageSize).ToList();//.GroupBy(e => e.financialAccountId );

            var FinalLedger = new FinalLedgerResponse()
            {
                FinancialAccountList = finalResult,
                FinancialAccountNameAr = financialCodeParameter.ArabicName,
                FinancialAccountNameEn = financialCodeParameter.LatinName,
                

            };

            FinalLedger.TotalTransCredit = roundNumbers.GetRoundNumber(totalLedgerResponse.Where(a => a.financialAccountCode == "#$[TOTAL]$%").Sum(a => a.transCredit) + TotalPrevBalanceCredit);

            FinalLedger.totalTransDebit = roundNumbers.GetRoundNumber(totalLedgerResponse.Where(a => a.financialAccountCode == "#$[TOTAL]$%").Sum(a => a.transDebit) + TotalPrevBalanceDebit);


            var totalBalance = roundNumbers.GetRoundNumber(FinalLedger.TotalTransCredit - FinalLedger.totalTransDebit);
            if (totalBalance > 0)
                FinalLedger.totalBalanceCredit = roundNumbers.GetRoundNumber(totalBalance);
            else
                FinalLedger.totalBalanceDebit = roundNumbers.GetRoundNumber(-totalBalance);

            return new ResponseResult { Data = FinalLedger, DataCount = dataCount, Note = countofFilter == paramters.PageNumber ? Actions.EndOfData : null, Result = Result.Success };
        }


        public async Task<WebReport> ledgerReport(PageParameterLedgerReport paramters, SubFormsIds accountType, exportType exportType, bool isArabic,int fileId=0)
        {
            var data = await GetLedgerData(paramters, true);
            var userInfo = await _iUserInformation.GetUserInformation();
            var mainData = (FinalLedgerResponse)data.Data;
            var otherdata = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, paramters.From, paramters.To);




            otherdata.ArabicName = mainData.FinancialAccountNameAr;
            otherdata.LatinName = mainData.FinancialAccountNameEn;

            otherdata.Code = mainData.FinancialAccountList[0].financialAccountCode;
            otherdata.EmployeeName = userInfo.employeeNameAr.ToString();
            otherdata.EmployeeNameEn = userInfo.employeeNameEn.ToString();






            var tableData = (int)accountType == 22? mainData.FinancialAccountList.Where(f => f.financialAccountCode == null||f.financialAccountCode.Contains("TOTAL")).ToList():
                mainData.FinancialAccountList.Where(f => f.financialAccountCode == null).ToList();
            //foreach (var item in tableData)
            //{
            //    item.Date = item.journalEntryDate.ToString("yyyy/MM/dd");
            //}
            var tablesNames = new TablesNames();
            

            if ((int)accountType == 22)
            {

                tablesNames.FirstListName = "FinancialAccount";
                tablesNames.SecondListName = "FinancialAccountList";

            
                // string previousBalance = "";
                int j = 1;
                int y = 1;
                List<LedgerGroup> group = new List<LedgerGroup>();
                LedgerGroup datamain = new LedgerGroup();
                foreach (var item in mainData.FinancialAccountList)
                {
                    if (item.financialAccountCode != null)
                    {
                        if (item.financialAccountCode.Contains("EMPTY"))
                        {

                            datamain.EmptyCode = "EMPTY";
                            item.GroupId = j;
                            j++;

                        }
                        else if (item.financialAccountCode.Contains("HEADER"))
                        {

                            datamain.HeaderCode = "HEADER";

                            item.GroupId = j;

                        }
                        else if (item.financialAccountCode.Contains("TOTAL"))
                        {

                            datamain.TotalCode = "TOTAL";

                            datamain.GroupTotalBalaceCredit = item.balanceCredit;
                            datamain.GroupTotalBalanceDebit = item.balanceDebit;
                            datamain.GroupTotalTransCredit = item.transCredit;
                            datamain.GroupTotalTransDebit = item.transDebit;

                            item.GroupId = j;

                        }
                        else if (item.financialAccountCode.Contains("NoDATA"))
                        {


                            var data1 = mainData.FinancialAccountList.Where(f => f.financialAccountId == item.financialAccountId).FirstOrDefault();
                            data1.NoDataGroup = "NoData";
                            // mainData.FinancialAccountList.Add(data1);
                            data1.PreviousBalanceString = ConvertDoubleToString(data1.PreviousBalance, isArabic);
                            tableData.Add(data1);

                        }
                        else
                        {
                            //if (item.financialAccountCode.Contains("NoDATA"))
                            //{
                            //    datamain.NoData = "NoDATA";
                            //    var objectData  = mainData.FinancialAccountList.IndexOf(item)-1;


                            //}

                            //else
                            //{
                            //    datamain.BasicCode = item.financialAccountCode;
                            //    item.GroupId = j;

                            //}
                            datamain.BasicCode = item.financialAccountCode;

                            datamain.AcountNameAr = item.financialAccountNameAr;
                            datamain.AcountNameEn = item.financialAccountNameEn;
                            previousBalance = ConvertDoubleToString(item.PreviousBalance, isArabic);

                            //if (previousBalance.Contains("-"))
                            //{
                            //    string[] words;
                            //    words = previousBalance.Split("-");
                            //    previousBalance = words[1] + " مدين";
                            //    //datamain.PreviousBalance = ;

                            //}
                            //else if (previousBalance != "0")
                            //{
                            //    previousBalance = previousBalance + " دائن";
                            //}

                            datamain.PreviousBalance = previousBalance;
                            datamain.TotalBalaceCredit = mainData.totalBalanceCredit;
                            datamain.TotalBalanceDebit = mainData.totalBalanceDebit;
                            datamain.TotalTransCredit = mainData.TotalTransCredit;
                            datamain.TotalTransDebit = mainData.totalTransDebit;
                            item.GroupId = j;

                        }

                    }
                    else
                    {
                        item.GroupId = j;
                    }

                    if (datamain.HeaderCode != null && datamain.TotalCode != null &&
                        datamain.BasicCode != null && datamain.AcountNameAr != null&& datamain.NoData==null)
                    {
                        datamain.GroupId = y;
                        group.Add(datamain);
                        datamain = new LedgerGroup();
                        y++;
                    }
                    //else if(datamain.NoData != null)
                    //{
                    //    datamain.GroupId = y;
                    //    y++;
                    //}

                }

                if (group.Count == 0 && tableData.Count > 0)
                {
                    group = new List<LedgerGroup>()
                    {
                        new LedgerGroup()
                        {
                            TotalBalaceCredit = mainData.totalBalanceCredit,
                            TotalBalanceDebit = mainData.totalBalanceDebit,
                            TotalTransCredit = mainData.TotalTransCredit,
                            TotalTransDebit = mainData.totalTransDebit,
                        }
                
                    };
                }

                var report = await _iGeneralPrint.PrintReport<object, LedgerGroup, LedgerResponse>(null, group, tableData, tablesNames, otherdata
               , 22, exportType, isArabic,fileId);
                return report;
            }
            else if ((int)accountType == 24)
            {

                tablesNames.ObjectName = "FinancialAccount";
                tablesNames.FirstListName = "FinancialAccountList";

                //mainData.FinancialAccountCode = mainData.FinancialAccountList[0].financialAccountCode;
                mainData.PreviousBalanceString = ConvertDoubleToString(mainData.FinancialAccountList[0].PreviousBalance,isArabic);
                if (tableData.Count == 0)
                {
                    tableData = new List<LedgerResponse>()
                    {
                        new LedgerResponse()
                    };
                    mainData.Count = 0;
                }
                var report = await _iGeneralPrint.PrintReport<FinalLedgerResponse, LedgerResponse, object>(mainData, tableData, null, tablesNames, otherdata
               , 24, exportType, isArabic,fileId);
                return report;
               
            }
            else
                return new WebReport();
        }

        public string ConvertDoubleToString( double value,bool isArabic)
        {
            previousBalance = value.ToString();
            if (previousBalance.Contains("-"))
            {
                string[] words;
                words = previousBalance.Split("-");
                if (isArabic) 
                previousBalance = words[1] + " مدين";
                else
                    previousBalance = words[1] + " debtor ";

                //datamain.PreviousBalance = ;

            }
            else if (previousBalance != "0")
            {
                if(isArabic)
                previousBalance = previousBalance + " دائن";
                else
                    previousBalance = previousBalance + " creditor ";

            }
            return previousBalance;
        }
        public class LegerData : AdditionalData
        {
            public string FinancialAccountCode { get; set; }
            public string MainAccountNameAr { get; set; }
            public string MainAccountNameEn { get; set; }
            //   public string MainAccuntCode { get; set; }
        }
        //public class LedgerGroup
        //public class LegerData :AdditionalData
        //{
        //    public string FinancialAccountCode { get; set; }
        //    public string MainAccountNameAr { get; set; }
        //    public string MainAccountNameEn { get; set; }
        // //   public string MainAccuntCode { get; set; }
        //}
        public class LedgerGroup 
        {
            public int GroupId { get; set; }
            public string EmptyCode { get; set; }
            public string BasicCode { get; set; }
            public string HeaderCode { get; set; }
            public string TotalCode { get; set; }
            public string NoData { get; set; }
            public double GroupTotalTransCredit { get; set; }
            public double GroupTotalTransDebit { get; set; }
            public double GroupTotalBalanceDebit { get; set; }
            public double GroupTotalBalaceCredit { get; set; }

            public double TotalTransCredit { get; set; }
            public double TotalTransDebit { get; set; }
            public double TotalBalanceDebit { get; set; }
            public double TotalBalaceCredit { get; set; }

            public string PreviousBalance { get; set; }

            public string AcountNameAr { get; set; }
            public string AcountNameEn { get; set; }







        }
    }
}
