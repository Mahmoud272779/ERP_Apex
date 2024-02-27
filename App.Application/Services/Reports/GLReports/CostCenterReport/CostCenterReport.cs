using App.Application.Services.Printing.InvoicePrint;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Domain.Entities.Process;
using App.Domain.Models.Request.GeneralLedger;
using App.Domain.Models.Response.Store;
using App.Domain.Models.Response.Store.Reports.Purchases;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2010.Word.DrawingShape;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application
{
    public class CostCenterReport : ICostCenterReport
    {

        private readonly IRepositoryQuery<GLJournalEntryDetails> JournalEntryDetailsQuery;
        private readonly IRoundNumbers roundNumbers;
        private readonly IRepositoryQuery<InvGeneralSettings> generalSettingsRepositoyQuery;
        private readonly IRepositoryQuery<GLCostCenter> costCenterQuery;
        private readonly iUserInformation _iUserInformation;

        private readonly IGeneralPrint _iGeneralPrint;



        private readonly ICompanyDataService _CompanyDataService;
        public InvGeneralSettings GLsetting
        {

            get { return (InvGeneralSettings)generalSettingsRepositoyQuery.TableNoTracking.FirstOrDefault(); }

        }

        public CostCenterReport(
            IRepositoryQuery<InvGeneralSettings> generalSettingsRepositoyQuery,
            IRepositoryQuery<GLJournalEntryDetails> JournalEntryDetailsQuery,
                 IRoundNumbers roundNumbers,
            IRepositoryQuery<GLCostCenter> costCentersQuery,
            iUserInformation iUserInformation,
            IPrintService iprintService,
            IFilesMangerService filesMangerService,
            ICompanyDataService companyDataService,
            IGeneralPrint iGeneralPrint)

        {
            this.JournalEntryDetailsQuery = JournalEntryDetailsQuery;
            this.roundNumbers = roundNumbers;
            this.generalSettingsRepositoyQuery = generalSettingsRepositoyQuery;
            this.costCenterQuery = costCentersQuery;
            _iUserInformation = iUserInformation;

            _CompanyDataService = companyDataService;
            _iGeneralPrint = iGeneralPrint;
        }


        public async Task<ResponseResult> GetCostCenterReport(CostCenterReportRequest Parameter, bool isPrint = false)
        {
            // int DecimalNum = GLsetting.Other_Decimals;
            var costCenterData = await costCenterQuery.GetAsync(Parameter.CostCenterID);
            if (costCenterData == null)
                return new ResponseResult() { Data = null, Result = Result.NotFound, Note = "costCenter notFound" };
            var allData = await getAllData(
                h => h.journalEntry.FTDate >= Parameter.From
                && h.journalEntry.FTDate <= Parameter.To
                && h.CostCenterId == Parameter.CostCenterID
                && h.journalEntry.IsBlock == false
                && (Parameter.financialAccountId != null ? h.FinancialAccountId == Parameter.financialAccountId : true)
                , Parameter
                );


            //pagenation

            var allDataPaging = isPrint ? allData : Pagenation<CostCenterReportReportData>.pagenationList(Parameter.PageSize, Parameter.PageNumber, allData.ToList());


            var prevData = await getAllData(
                h => h.journalEntry.FTDate < Parameter.From
                && h.CostCenterId == Parameter.CostCenterID
                && h.journalEntry.IsBlock == false
                && (Parameter.financialAccountId != null ? h.FinancialAccountId == Parameter.financialAccountId : true)
                , Parameter);

            //get prevouse data (-1) deptor (+1)Creditor
            var TotalPrevData = costCenterData.CC_Nature == (int)FA_Nature.Debit
                ? costCenterData.InitialBalance * -1
                : costCenterData.InitialBalance;
            TotalPrevData += prevData.Sum(h => h.Credit - h.Debit);
            var balanceDataList = new List<FinalCostCenterReportReportData>();
            var balance = TotalPrevData;
            foreach (var item in allDataPaging)
            {
                balance += item.Credit - item.Debit;
                var obj = new FinalCostCenterReportReportData();

                obj.FinantialAccNameAr = item.FinantialAccNameAr;
                obj.FinantialAccNameEn = item.FinantialAccNameEn;
                obj.Date = item.Date;
                obj.Note = item.Note;
                obj.DescriptionAr = item.DescriptionAr;
                obj.DescriptionEn = item.DescriptionEn;
                obj.periodCredit = roundNumbers.GetRoundNumber(item.Credit);
                obj.periodDebit = roundNumbers.GetRoundNumber(item.Debit);
                obj.balanceCredit = roundNumbers.GetRoundNumber((balance > 0 ? balance : 0));
                obj.balanceDebit = roundNumbers.GetRoundNumber((balance < 0 ? balance * -1 : 0));
                obj.JournalEntryId = item.JournalEntryId;
                obj.JournalEntryCode = item.JournalEntryCode;
                balanceDataList.Add(obj);

            }
            var TotalBalance = allData.Sum(h => h.Credit - h.Debit) + TotalPrevData;
            var totalDebit = allData.Sum(h => h.Debit);
            var totalCredit = allData.Sum(h => h.Credit);
            var finalResult = new FinalCostCenterReportReportAllData()
            {
                PreviousBalance = TotalPrevData,
                FinalCostCenterReportReportDataList = balanceDataList,

                TotalbalanceCredit = TotalBalance > 0 ? roundNumbers.GetRoundNumber(TotalBalance) : 0,
                TotalbalanceDebit = TotalBalance < 0 ? roundNumbers.GetRoundNumber(TotalBalance) : 0,

                TotalperiodCredit = TotalPrevData > 0 ? roundNumbers.GetRoundNumber((totalCredit + TotalPrevData)) : roundNumbers.GetRoundNumber(totalCredit),
                TotalperiodDebit = TotalPrevData < 0 ? roundNumbers.GetRoundNumber((TotalPrevData - totalDebit)) : roundNumbers.GetRoundNumber(totalDebit),

            };
            return new ResponseResult() { Data = finalResult, Result = Result.Success };
        }
        public async Task<WebReport> CostCenterPrint(CostCenterReportRequest Parameter, exportType exportType, bool isArabic,int fileId=0)
        {
            var returndata = await GetCostCenterReport(Parameter);
            var data = (FinalCostCenterReportReportAllData)returndata.Data;

            var userInfo = await _iUserInformation.GetUserInformation();
            var costCenterData = await costCenterQuery.GetAsync(Parameter.CostCenterID);
            var otherdata = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, Parameter.From, Parameter.To);




            otherdata.ArabicName = costCenterData.ArabicName;
            otherdata.LatinName = costCenterData.LatinName;
            otherdata.EmployeeName = userInfo.employeeNameAr.ToString();
            otherdata.EmployeeNameEn = userInfo.employeeNameEn.ToString();



            //main data
            string[] words;


            data.PrevBalance = data.PreviousBalance.ToString();
            if (data.PrevBalance.Contains("-") && data.PrevBalance != "-0")
            {
                words = data.PrevBalance.Split("-");
                if (isArabic)
                    data.PrevBalance = words[1] + " مدين";
                else
                    data.PrevBalance = words[1] + " debtor ";

            }
            else if (data.PrevBalance == "-0")
            {
                data.PrevBalance = data.PrevBalance.Replace("-", "");
            }
            else
            {
                if (isArabic)
                    data.PrevBalance = data.PrevBalance + " دائن";
                else
                    data.PrevBalance = data.PrevBalance + " creditor ";


            }
            data.TotalperiodDebit = Math.Abs(data.TotalperiodDebit);
            //if (data.TotalDeriodsDebit.Contains("-"))
            //{
            //    data.TotalDeriodsDebit = data.TotalDeriodsDebit.Replace("-", "");
            //}
            data.TotalbalanceDebit = Math.Abs(data.TotalbalanceDebit);
            //if (data.TotalBalancesDebit.Contains("-"))
            //{
            //    data.TotalBalancesDebit = data.TotalBalancesDebit.Replace("-", "");
            //}
            // end main data

            //list data 
            foreach (var item in data.FinalCostCenterReportReportDataList)
            {
                item.JournalDate = item.Date?.ToString("yyyy/MM/dd");
                item.periodsDebit = item.periodDebit.ToString();
                if (item.periodsDebit.Contains("-"))
                {
                    item.periodsDebit = item.periodsDebit.Replace("-", "");

                }
                item.balancesDebit = item.balanceDebit.ToString();
                if (item.balancesDebit.Contains("-"))
                {
                    item.balancesDebit = item.balancesDebit.Replace("-", "");
                }
            }
            if (data.FinalCostCenterReportReportDataList.Count == 0)
            {
                data.FinalCostCenterReportReportDataList = new List<FinalCostCenterReportReportData>()
                {
                    new FinalCostCenterReportReportData()
                };
                data.Count = 0;
            }

            // end list
            int screenId = 0;
            if (Parameter.financialAccountId !=null)
            {
                screenId = (int)SubFormsIds.costCenterForAccount;
            }
            else 
            {
                
                screenId = (int)SubFormsIds.CostCenterReport_GL;
            }


            var tablesNames = new TablesNames()
            {
                ObjectName = "FinancialAccount",
                FirstListName = "FinancialAccountList"
            };
            var report = await _iGeneralPrint.PrintReport<FinalCostCenterReportReportAllData, FinalCostCenterReportReportData, object>(data, data.FinalCostCenterReportReportDataList, null, tablesNames, otherdata
                , screenId, exportType, isArabic,fileId);
            return report;

        }

        public async Task<List<CostCenterReportReportData>> getAllData(Expression<Func<GLJournalEntryDetails, bool>> filter, CostCenterReportRequest Parameter)
        {
            var data = await JournalEntryDetailsQuery.TableNoTracking
                                .Include(h => h.GLFinancialAccount)
                                .Include(h => h.journalEntry)
                                .Where(filter)
                                .Select(h => new CostCenterReportReportData()
                                {
                                    FinantialAccNameAr = h.GLFinancialAccount.ArabicName,
                                    FinantialAccNameEn = h.GLFinancialAccount.LatinName,
                                    JournalEntryId = h.JournalEntryId,
                                    JournalEntryCode = h.journalEntry.Code,

                                    Date = h.journalEntry.FTDate,
                                    Credit = h.Credit,
                                    Debit = h.Debit,
                                    Note = h.journalEntry.Notes,
                                    DescriptionEn = h.DescriptionEn,
                                    DescriptionAr = h.DescriptionAr,

                                }).OrderBy(h => h.Date).ToListAsync();
            return data;

        }





    }

}
