using App.Application.Basic_Process;
using App.Application.Services.Printing.InvoicePrint;
using App.Application.Services.Process.FinancialAccounts;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Domain.Entities.Process;
using App.Domain.Models.Response.GeneralLedger;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Interfaces.Repository;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static App.Application.Services.Process.GLServices.ledger_Report.LedgerReportService;
using static App.Domain.Enums.Enums;
using static App.Domain.Models.Security.Authentication.Response.Totals;

namespace App.Application.Services.Process.BalanceReview
{
    public class BalanceReviewBusiness : BusinessBase<GLFinancialAccount>, IBalanceReviewBusiness
    {

        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRoundNumbers roundNumbers;
        private readonly IRepositoryQuery<InvGeneralSettings> generalSettingsRepositoyQuery;
        private readonly IPrintService _iprintService;

        private readonly IFilesMangerService _filesMangerService;
        private readonly ICompanyDataService _CompanyDataService;
        private readonly iUserInformation _iUserInformation;
        private readonly IGeneralPrint _iGeneralPrint;

        public InvGeneralSettings GLsetting
        {

            get { return (InvGeneralSettings)generalSettingsRepositoyQuery.GetAll().ToList().FirstOrDefault(); ; }

        }

        public BalanceReviewBusiness(
            IRepositoryQuery<InvGeneralSettings> generalSettingsRepositoyQuery,
            IRepositoryQuery<GLFinancialAccount> FinancialAccountRepositoryQuery, IRoundNumbers roundNumbers,
            IRepositoryActionResult repositoryActionResult, IPrintService iprintService, IFilesMangerService filesMangerService, ICompanyDataService companyDataService, iUserInformation iUserInformation, IGeneralPrint iGeneralPrint) : base(repositoryActionResult)
        {
            financialAccountRepositoryQuery = FinancialAccountRepositoryQuery;
            this.roundNumbers = roundNumbers;
            this.generalSettingsRepositoyQuery = generalSettingsRepositoyQuery;
            _iprintService = iprintService;
            _filesMangerService = filesMangerService;
            _CompanyDataService = companyDataService;
            _iUserInformation = iUserInformation;
            _iGeneralPrint = iGeneralPrint;
        }
        public async Task<ResponseResult> getAllDataBalanceById(BalanceReviewSearchParameter parameter, int ID)
        {
            int DecimalNum = GLsetting.Other_Decimals;
            if (string.IsNullOrEmpty(ID.ToString()))
                return new ResponseResult() { Data = null, Note = "Id Is Required", Result = Result.RequiredData };
            //var childIDs = await financialAccountRepositoryQuery.Get(h => new { h.Id, h.AccountCode }, h => h.ParentId == ID);

            var childIDs = await financialAccountRepositoryQuery.FindByAsyn(h => h.Id == ID);
            // var totalIDs = await financialAccountRepositoryQuery.Get(h => new { h.Id, h.AccountCode });// h.AccountCode.StartsWith(a.AccountCode));
            var totalIDs = await financialAccountRepositoryQuery
                    .Get(h => new { h.Id, h.autoCoding },
                    h => h.autoCoding
                    .StartsWith(childIDs.FirstOrDefault().autoCoding)
                    );
            // select accountCode of all chilren in topLevels
            //var total_ids = totalIDs.Where(h => childIDs.Select(d => d.AccountCode).Any(s => h.AccountCode.StartsWith(s)));


            var result = getAllDataBalanceReview(totalIDs.Select(a => a.Id).ToList(), parameter.From, parameter.To);
            var FResult = result.Where(h => h.parentId == ID).OrderBy(x=> x.autocode).ToList();

            FResult.ForEach(
              h =>
              {
                  h.pervDeptor =               roundNumbers.GetRoundNumber ((result.Where(a => a.financialCode.StartsWith(h.financialCode)).Sum(h => h.pervDeptor)));
                  h.pervCreditor =             roundNumbers.GetRoundNumber((result.Where(a => a.financialCode.StartsWith(h.financialCode)).Sum(h => h.pervCreditor)));
                  h.insideDebtor =             roundNumbers.GetRoundNumber((result.Where(a => a.financialCode.StartsWith(h.financialCode)).Sum(h => h.insideDebtor)));
                  h.insidecreditor =           roundNumbers.GetRoundNumber((result.Where(a => a.financialCode.StartsWith(h.financialCode)).Sum(h => h.insidecreditor)));
                  h.CollectingPeriodDebtor =   roundNumbers.GetRoundNumber((result.Where(a => a.financialCode.StartsWith(h.financialCode)).Sum(h => h.CollectingPeriodDebtor)));
                  h.collectingPeriodcreditor = roundNumbers.GetRoundNumber((result.Where(a => a.financialCode.StartsWith(h.financialCode)).Sum(h => h.collectingPeriodcreditor)));
                  h.balanceCreditor =          roundNumbers.GetRoundNumber ((h.collectingPeriodcreditor - h.CollectingPeriodDebtor < 0 ? 0 : h.collectingPeriodcreditor - h.CollectingPeriodDebtor));         
                  h.balanceDebtor =            roundNumbers.GetRoundNumber(((h.CollectingPeriodDebtor - h.collectingPeriodcreditor) < 0 ? 0 : (h.CollectingPeriodDebtor - h.collectingPeriodcreditor)));
              });
            //var finalResult = result.Where(h => h.parentId == ID);
         
             return new ResponseResult() { Data = FResult,  Result = Result.Success };
        }
        public async Task<ResponseResult> getTopLevelDataBalance(BalanceReviewSearchParameter parameter)
        {
            int DecimalNum = GLsetting.Other_Decimals;
            List<TopLevelFinantialAccount> topLevel=new List<TopLevelFinantialAccount> ();  
            if (parameter.FinancialAcountId != 0)
            {
                topLevel =await financialAccountRepositoryQuery.Get(h => new TopLevelFinantialAccount 
                { Id = h.Id, AccountCode = h.AccountCode }, h => h.Id == parameter.FinancialAcountId); 
            }
            else 
            {
                topLevel = await financialAccountRepositoryQuery.Get(h => new TopLevelFinantialAccount 
                { Id = h.Id, AccountCode = h.AccountCode }, h => h.ParentId == null);
            }
            //get all children of toplevel
            var totalIDs = await financialAccountRepositoryQuery.Get(h => new { h.Id, h.AccountCode });// h.AccountCode.StartsWith(a.AccountCode));

            // select accountCode of all chilren in topLevels
            var total_ids = totalIDs.Where(h => topLevel.Select(d => d.AccountCode).Any(s => h.AccountCode.StartsWith(s)));


            var result = getAllDataBalanceReview(total_ids.Select(a => a.Id).ToList(), parameter.From, parameter.To);
            //select toplevel only 
            var FResult = result.Where(a => (topLevel.Select(e => e.Id).Contains(a.Id))).OrderBy(x=> x.autocode).ToList();
            FResult.ForEach(
              h =>
              {
                  h.pervDeptor                  =   roundNumbers.GetRoundNumber((result.Where(a => a.autocode.StartsWith(h.autocode)).Sum(h => h.pervDeptor)));
                  h.pervCreditor                =   roundNumbers.GetRoundNumber((result.Where(a => a.autocode.StartsWith(h.autocode)).Sum(h => h.pervCreditor)));
                  h.insideDebtor                =   roundNumbers.GetRoundNumber((result.Where(a => a.autocode.StartsWith(h.autocode)).Sum(h => h.insideDebtor)));
                  h.insidecreditor              =   roundNumbers.GetRoundNumber((result.Where(a => a.autocode.StartsWith(h.autocode)).Sum(h => h.insidecreditor)));
                  h.CollectingPeriodDebtor      =   roundNumbers.GetRoundNumber((result.Where(a => a.autocode.StartsWith(h.autocode)).Sum(h => h.CollectingPeriodDebtor)));
                  h.collectingPeriodcreditor    =   roundNumbers.GetRoundNumber((result.Where(a => a.autocode.StartsWith(h.autocode)).Sum(h => h.collectingPeriodcreditor)));
                  h.balanceCreditor             =   roundNumbers.GetRoundNumber((h.collectingPeriodcreditor - h.CollectingPeriodDebtor < 0 ? 0 : h.collectingPeriodcreditor - h.CollectingPeriodDebtor));
                  h.balanceDebtor               =   roundNumbers.GetRoundNumber(((h.CollectingPeriodDebtor - h.collectingPeriodcreditor) < 0 ? 0 : (h.CollectingPeriodDebtor - h.collectingPeriodcreditor)));

              });
           
            var TotalResult = new FinalResponseResult();
            var x = result.Where(a => a.autocode.StartsWith(FResult.First().autocode)).Sum(h => h.insideDebtor);
            //totals
            TotalResult.TotalPrevCreditor       =   roundNumbers.GetRoundNumber (FResult.Sum(h => h.pervCreditor));
            TotalResult.TotalPrevDebtor         =   roundNumbers.GetRoundNumber(FResult.Sum(h => h.pervDeptor));
            TotalResult.TotalInsideCreditor     =   roundNumbers.GetRoundNumber(FResult.Sum(h => h.insidecreditor));
            TotalResult.TotalInsideDebtor       =   roundNumbers.GetRoundNumber(FResult.Sum(h => h.insideDebtor));
            TotalResult.TotalCollectingCreditor =   roundNumbers.GetRoundNumber(FResult.Sum(h => h.collectingPeriodcreditor));
            TotalResult.TotalCollectingDebtor   =   roundNumbers.GetRoundNumber(FResult.Sum(h => h.CollectingPeriodDebtor));
            TotalResult.TotalBalanceCreditor    =   roundNumbers.GetRoundNumber(FResult.Sum(h => h.balanceCreditor));
            TotalResult.TotalBalanceDebtor      =   roundNumbers.GetRoundNumber(FResult.Sum(h => h.balanceDebtor));
            TotalResult.FinalData = FResult;
             return new ResponseResult() { Data = TotalResult,  Result = Result.Success };


        }
        public async Task<WebReport> DetailedBalanceReviewReport(BalanceReviewSearchParameter parameter,string ids,exportType exportType,bool isArabic,int fileId=0)
        {
            var data = await getTopLevelDataBalance(parameter);
           
            var userInfo = await _iUserInformation.GetUserInformation();


            var mainData = (FinalResponseResult)data.Data;

           foreach(var item in mainData.FinalData)
            {
                item.Level = 1;
            }
            var otherdata = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic,parameter.From,parameter.To);



            otherdata.EmployeeName = userInfo.employeeNameAr.ToString();
            otherdata.EmployeeNameEn = userInfo.employeeNameEn.ToString();





            if (ids == null || ids == "")
            {

            }
            else
            {
                string[] ParentIds = ids.Split(",");
                if (ParentIds.Count() > 0)
                {

                    foreach (var id in ParentIds.ToList())
                    {
                        var childList = await getAllDataBalanceById(parameter, Convert.ToInt32(id));
                        var objectData = mainData.FinalData.Where(a => a.Id == Convert.ToInt32(id)).FirstOrDefault();
                        int index = mainData.FinalData.IndexOf(objectData);
                        //mainData.FinalData.AddRange(index + 1, childList);
                        mainData.FinalData.InsertRange(index + 1, (List<finalData>)childList.Data);
                    }
                }
            }

            Regex pattern = new Regex("[.]");
            
            foreach(var item in mainData.FinalData)
            {
                item.financialCode= pattern.Replace(item.financialCode, "");
                
            }
            int screenId = 0;
            if (parameter.TypeId == 1)
            {
                 screenId = (int)SubFormsIds.DetailedTrialBalance_GL;

            }
            else if (parameter.TypeId == 2)
            {
                screenId = (int)SubFormsIds.TotalAccountBalance;

            }
            else if (parameter.TypeId == 3)
            {
                screenId = (int)SubFormsIds.BalanceReviewFunds;

            }
            var tablesNames = new TablesNames()
            {
                ObjectName = "FinancialAccount",
                FirstListName = "FinancialAccountList"
            };


            var report = await _iGeneralPrint.PrintReport<FinalResponseResult, finalData, object>(mainData, mainData.FinalData, null, tablesNames, otherdata
                , screenId, exportType, isArabic,fileId);
            return report;





            
        }

        public List<finalData> getAllDataBalanceReview(List<int> IDs, DateTime From, DateTime To)
        {
            //خلال الفتره
            var InPeriodData = financialAccountRepositoryQuery.TableNoTracking.Include(h=>h.Currency)
                .Include(h => h.GLJournalEntryDetails.Where(h => h.journalEntry.IsBlock == false))
                .Where(h =>  IDs.Contains(h.Id))
                .Select(h => new BalanceReviewInsidePeriodDTO
                {
                    Id = h.Id,
                    ArabicName = h.ArabicName,
                    LatinName = h.LatinName,
                    FinancialAccountCode = h.AccountCode,
                    AutoCode = h.autoCoding,
                    hasChild = h.IsMain,
                    ParentId= h.ParentId,
                             
                    InsidePeriodDebtor = h.GLJournalEntryDetails.Where(h => (h.journalEntry.FTDate.Value.Date >= From.Date) && (h.journalEntry.FTDate.Value.Date <= To.Date) && h.journalEntry.IsBlock == false).Sum(h => h.Debit ),
                    InsidePeriodCreditor = h.GLJournalEntryDetails.Where(h => (h.journalEntry.FTDate.Value.Date >= From.Date) && (h.journalEntry.FTDate.Value.Date <= To.Date) && h.journalEntry.IsBlock == false).Sum(h => h.Credit)
                }).ToList();

            //قبل الفتره
            var PrevData = financialAccountRepositoryQuery.TableNoTracking
                .Include(h => h.GLJournalEntryDetails.Where((h => h.journalEntry.FTDate.Value.Date < From.Date ))
                )
                .Where( h=> IDs.Contains(h.Id))
                
                .Select(h => new BalanceReviewPrevPeriodDTO
                {
                    Id = h.Id,
                    ArabicName = h.ArabicName,
                    LatinName = h.LatinName,
                    FinancialAccountCode = h.AccountCode,
                    AutoCode = h.autoCoding,
                    hasChild = h.IsMain,
                    PrevPeriodDebtor = h.GLJournalEntryDetails.Where (h=> h.journalEntry !=null && h.journalEntry.FTDate.Value.Date < From.Date && h.journalEntry.IsBlock == false).Sum(h => h.Debit),
                    PrevPeriodCreditor = h.GLJournalEntryDetails.Where(h => h.journalEntry != null && h.journalEntry.FTDate.Value.Date < From.Date && h.journalEntry.IsBlock == false).Sum(h => h.Credit)
                }).ToList();
            
            //محموع الكل
            var CollectingData = financialAccountRepositoryQuery.TableNoTracking
                .Include(h => h.GLJournalEntryDetails.Where(h => h.journalEntry.IsBlock==false)    )
                .Where(h =>  IDs.Contains(h.Id))
                .Select(h => new BalanceReviewCollectingPeriodDTO
                {
                    Id = h.Id,
                    ArabicName = h.ArabicName,
                    LatinName = h.LatinName,
                    hasChild=h.IsMain,
                    FinancialAccountCode = h.AccountCode,
                    AutoCode = h.autoCoding,
                    curencyFactor=h.Currency.Factor,
                    ParentId = h.ParentId,
                    CollectingPeriodDebtor = h.GLJournalEntryDetails.Where(h => h.journalEntry.FTDate.Value.Date <= To.Date && h.journalEntry.IsBlock == false).Sum(h => h.Debit),
                    CollectingPeriodCreditor = h.GLJournalEntryDetails.Where(h => h.journalEntry.FTDate.Value.Date <= To.Date && h.journalEntry.IsBlock == false).Sum(h => h.Credit)
                }).ToList();

            var res = PrevData.Join(InPeriodData, s => s.Id, i => i.Id, (p, h) =>
            new
            {
                Id = p.Id,
                autocode = h.AutoCode,
                ArabicName = h.ArabicName,
                LatinName = h.LatinName,
                financialCode = h.FinancialAccountCode,
                pervDeptor = p.PrevPeriodDebtor,
                pervCreditor = p.PrevPeriodCreditor,
                insideDebtor = h.InsidePeriodDebtor,
                insidecreditor = h.InsidePeriodCreditor

            }).Join(CollectingData, a => a.Id, t => t.Id, (a, h) => new finalData()
            {
                Id                  = a.Id,
                parentId                  = h.ParentId,
                autocode            = a.autocode,
                financialCode       = a.financialCode,
                pervDeptor          = a.pervDeptor*h.curencyFactor,  
                pervCreditor        = a.pervCreditor*h.curencyFactor,
                insideDebtor        = a.insideDebtor * h.curencyFactor,
                ArabicName          = a.ArabicName,
                LatinName           = a.LatinName,
                insidecreditor      = a.insidecreditor * h.curencyFactor,
                hasChild=h.hasChild,
                CollectingPeriodDebtor   = h.CollectingPeriodDebtor * h.curencyFactor,
                collectingPeriodcreditor = h.CollectingPeriodCreditor * h.curencyFactor,
                //balanceDebtor       = ((h.CollectingPeriodDebtor - h.CollectingPeriodCreditor) < 0 ? 0 : (h.CollectingPeriodDebtor - h.CollectingPeriodCreditor)),
                //balanceCreditor     = h.CollectingPeriodCreditor - h.CollectingPeriodDebtor < 0 ? 0 : h.CollectingPeriodCreditor - h.CollectingPeriodDebtor
            }) ;



            return res.ToList();
        }

       

      
    }
}
