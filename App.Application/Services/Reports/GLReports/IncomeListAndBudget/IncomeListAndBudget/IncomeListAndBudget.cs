using App.Application.Services.HelperService;
using App.Application.Services.Printing;
using App.Application.Services.Printing.InvoicePrint;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Application.Services.Reports.Items_Prices;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Response;
using App.Infrastructure;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static App.Application.Services.Process.GLServices.ledger_Report.LedgerReportService;
using static App.Domain.Enums.Enums;
using static App.Domain.Models.Security.Authentication.Response.Totals;

namespace App.Application
{
    public class IncomeListAndBudget : IIncomeListAndBudget
    {
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> generalSettingsRepositoyQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> _invGeneralSettingsQuery;
        private readonly IRepositoryQuery<GLGeneralSetting> _gLGeneralSettingQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> _invStpItemCardMasterQuery;
        private readonly IHelperService _helperService;
        private readonly IRepositoryQuery<InvoiceDetails> _invoiceDetailsQuery;
        private readonly IRoundNumbers roundNumbers;
        private readonly IRepositoryQuery<GLJournalEntryDetails> _journalEntryRepositoryQuery;
        private readonly IPrintService _iprintService;
        private readonly IRepositoryQuery<InvStpItemCardUnit> _InvStpItemCardUnitQuery;

        private readonly IFilesMangerService _filesMangerService;
        private readonly ICompanyDataService _CompanyDataService;
        private readonly iUserInformation _iUserInformation;
        private readonly IGeneralPrint _iGeneralPrint;
        private readonly IRpt_Store _IRpt_Store;

        public InvGeneralSettings GLsetting
        {

            get { return (InvGeneralSettings)generalSettingsRepositoyQuery.GetAll().ToList().FirstOrDefault(); }

        }
        public GLGeneralSetting GLGeneralSetting
        {
            get { return _gLGeneralSettingQuery.TableNoTracking.FirstOrDefault(); }
        }
        public IncomeListAndBudget(

            IRepositoryQuery<GLFinancialAccount> FinancialAccountRepositoryQuery,
            IRepositoryQuery<InvGeneralSettings> generalSettingsRepositoyQuery,
            IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsQuery,
            IRepositoryQuery<GLGeneralSetting> GLGeneralSettingQuery,
            IRepositoryQuery<InvStpItemCardMaster> InvStpItemCardMasterQuery,
            IHelperService helperService,
             IRepositoryQuery<InvoiceDetails> InvoiceDetailsQuery,
                  IRoundNumbers roundNumbers,
            IRepositoryQuery<GLJournalEntryDetails> JournalEntryRepositoryQuery,
            IPrintService iprintService,
            IFilesMangerService filesMangerService,
            ICompanyDataService companyDataService,
            iUserInformation iUserInformation,
            IGeneralPrint iGeneralPrint,
            IRepositoryQuery<InvStpItemCardUnit> invStpItemCardUnitQuery,
            IRpt_Store iRpt_Store)

        {
            financialAccountRepositoryQuery = FinancialAccountRepositoryQuery;
            this.generalSettingsRepositoyQuery = generalSettingsRepositoyQuery;
            _invGeneralSettingsQuery = InvGeneralSettingsQuery;
            _gLGeneralSettingQuery = GLGeneralSettingQuery;
            _invStpItemCardMasterQuery = InvStpItemCardMasterQuery;
            _helperService = helperService;
            _invoiceDetailsQuery = InvoiceDetailsQuery;
            this.roundNumbers = roundNumbers;
            _journalEntryRepositoryQuery = JournalEntryRepositoryQuery;
            _iprintService = iprintService;
            _filesMangerService = filesMangerService;
            _CompanyDataService = companyDataService;
            _iUserInformation = iUserInformation;
            _iGeneralPrint = iGeneralPrint;
            _InvStpItemCardUnitQuery = invStpItemCardUnitQuery;
            _IRpt_Store = iRpt_Store;
        }
        public IQueryable<GLJournalEntryDetails> JounralEntry(DateTime From,DateTime To) => _journalEntryRepositoryQuery.TableNoTracking.Include(x => x.GLFinancialAccount).Include(x => x.journalEntry)
                                                                                            .Where(x => x.journalEntry.FTDate.Value.Date >= From.Date && x.journalEntry.FTDate.Value.Date <= To.Date);

        public async Task<totalResponseResult> getAllDataIncomeinListAndBudgetById(IncomeListSearchParameter parametr, int ID)
        {
            try
            {

           
                //var DecimalNum = GLsetting.Other_Decimals;
                //var childIDs = await financialAccountRepositoryQuery.Get(h => new { h.Id, h.autoCoding }, h => h.ParentId == ID);
                var childIDs = await financialAccountRepositoryQuery.FindByAsyn(h=> h.Id==ID);
                //var LSTid=childIDs.Select(h=>h.autoCoding.Replace(h.autoCoding,"%"+h.autoCoding)).ToList();
                //var totalIDsTest =    financialAccountRepositoryQuery.TableNoTracking.ToList().Where(h => LSTid.Any(s => h.autoCoding.StartsWith(s))).ToList();
                //.Get(h => new { h.Id, h.autoCoding, h.ParentId }, h => childIDs.Select(d => d.autoCoding).Any(s => SqlFunctions.PatIndex("^" +s , h.autoCoding)!=null));//h => SqlFunctions.PatIndex("^"+ );


                var C_D = JounralEntry(parametr.From,parametr.To);

                var totalIDs = await financialAccountRepositoryQuery
                    .Get(h => new { h.Id, h.autoCoding },
                    h=>h.autoCoding
                    .StartsWith(childIDs.FirstOrDefault().autoCoding)
                    );
         
            // select accountCode of all chilren in topLevels
            //var total_ids = totalIDs.Where(h => childIDs.Select(d => d.autoCoding).Any(s => h.autoCoding.StartsWith(s)));

            var result =  getAllDataIncomeList(  totalIDs.Select(h => h.Id).ToList(), parametr.From, parametr.To);

            // claculate sum for the selected parent
            result.ForEach(
                 h =>
                 {
                     //h.debtor = Numbers.Roundedvalues( result.Where(a => a.FinancialAccountCode.StartsWith(h.FinancialAccountCode)).Sum(h => h.debtor),DecimalNum);
                     //h.creditor =Numbers.Roundedvalues(result.Where(a => a.FinancialAccountCode.StartsWith(h.FinancialAccountCode)).Sum(h => h.creditor),DecimalNum);
                     h.AccountBalance = _helperService.GetFinanicalAccountTotalAmount(h.Id, h.AutoCode, C_D).Result;
                 });
            //var finalResult = result.Where(a => childIDs.Select(e => e.Id).Contains(a.Id)).ToList();
                var finalResult = result.Where(h =>h.ParentId==ID).OrderBy(x=> x.AutoCode).ToList();
                //var EndTermMerchandise = SumEndTermMerchandis(parametr.To);
                
                //finalResult.ForEach(x =>
                //{
                //    if (x.Id == (int)FinanicalAccountDefultIds.EndTermMerchandise)
                //        x.AccountBalance = roundNumbers.GetRoundNumber(EndTermMerchandise);
                    
                //    if (x.Id == (int)FinanicalAccountDefultIds.Inventory)
                //    {
                //        if (EndTermMerchandise > 0)
                //            EndTermMerchandise = EndTermMerchandise * -1;
                //        x.AccountBalance = x.AccountBalance + roundNumbers.GetRoundNumber(x.AccountBalance + EndTermMerchandise > 0 ? EndTermMerchandise * -1 : EndTermMerchandise); 
                //    }
                //});
                return new totalResponseResult() { IncomingData = finalResult };

            }
            catch (Exception e)
            {

                throw;
            }

        }
        
        public async Task<totalResponseResult> getTopLevelIncomingListAndBudget(IncomeListSearchParameter parameter)
        {
            //var DecimalNum = GLsetting. Other_Decimals;
            var C_D = JounralEntry(parameter.From, parameter.To);
            var topLevel = await financialAccountRepositoryQuery
                   .Get(h => new { h.Id, h.autoCoding, h.FA_Nature }
                   , h => h.ParentId == null && h.FinalAccount == parameter.finalAccount);
            //  && (parameter.finalAccount==(int)finalAccount.Balance)? h.FA_Nature ==  parameter.naturalAccount: true);

            //get all children of toplevel
            var totalIDs = await financialAccountRepositoryQuery.Get(h => new { h.Id, h.autoCoding });// h.AccountCode.StartsWith(a.AccountCode));

            // select accountCode of all chilren in topLevels
            var total_ids =  totalIDs.Where(h => topLevel.Select(d => d.autoCoding).Any(s => h.autoCoding.StartsWith(s)));

            // claculate sum for each parent
            var result1 = getAllDataIncomeList(total_ids.Select(a => a.Id).ToList(), parameter.From, parameter.To);
            result1.ForEach(
                h =>
                {
                    //h.debtor = result1.Where(a => a.AutoCode.StartsWith(h.AutoCode)).Sum(h => h.debtor);
                    //h.creditor = result1.Where(a => a.AutoCode.StartsWith(h.AutoCode)).Sum(h => h.creditor);
                    h.AccountBalance = _helperService.GetFinanicalAccountTotalAmount(h.Id,h.AutoCode,C_D).Result;
                });

            // return 2 top level only
            var finalResult = result1.Where(a => topLevel.Select(e => e.Id).Contains(a.Id) ||
                                  topLevel.Select(a => a.Id).ToList().Contains(a.ParentId.GetValueOrDefault(0))).ToList()
                                  .Select(h => new IncomeingListFilterData
                                  {
                                      Id = h.Id,
                                      ArabicName = h.ArabicName,
                                      LatinName = h.LatinName,
                                      FinancialAccountCode = h.FinancialAccountCode,
                                      AutoCode = h.AutoCode.Replace(".",string.Empty),
                                      ParentId = h.ParentId,
                                      debtor =  roundNumbers.GetRoundNumber(h.debtor),
                                      creditor = roundNumbers.GetRoundNumber(h.creditor),
                                      hasChild = h.hasChild,
                                      accountnNatural = h.accountnNatural,
                                      AccountBalance = roundNumbers.GetRoundNumber(h.AccountBalance),  

                                  }).OrderBy(x=> x.AutoCode).ToList();
            //var EndTermMerchandise = SumEndTermMerchandis(parameter.To);
            int EndTermMerchandise = 0;
            finalResult.ForEach(x =>
            {
                if (x.Id == (int)accountantTree.FinanicalAccountDefultIds.businessCost)
                    x.AccountBalance = roundNumbers.GetRoundNumber(x.AccountBalance + EndTermMerchandise);
                if (x.Id == (int)accountantTree.FinanicalAccountDefultIds.expensesAndCosts)
                    x.AccountBalance = roundNumbers.GetRoundNumber(x.AccountBalance + EndTermMerchandise);
            });
            var totalBalance =roundNumbers.GetRoundNumber(finalResult.Where(a => a.ParentId == null).Sum(a => a.AccountBalance));

            return new totalResponseResult() { IncomingData = finalResult, totalBalance = totalBalance };

           

        }
        public async Task<WebReport> IncomingListReport(IncomeListSearchParameter parameter,string ids,exportType exportType,bool isArabic,int fileId=0)
        {
            var data = await getTopLevelIncomingListAndBudget(parameter);
            
            var userInfo = await _iUserInformation.GetUserInformation();


            var mainData = data.IncomingData.Where(i=>i.ParentId==null).ToList();

            foreach (var item in mainData)
            {
                item.Level = 1;

            }
            var dates=ArabicEnglishDate.OtherDataWithDatesArEn(isArabic,parameter.From,parameter.To);
            var otherdata = new IncomingListData()
            {


                TotalBalance = data.totalBalance,
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),

                DateFrom = dates.DateFrom,
                DateTo = dates.DateTo,
                Date = dates.Date

            };
            if (ids == null || ids=="")
            {

            }
            else
            {
                string[] parentId = ids.Split(",");
                if (parentId.Count() > 0)
                {

                    foreach (var id in parentId)
                    {
                        var childList = await getAllDataIncomeinListAndBudgetById(parameter, Convert.ToInt32( id));
                        var objectData = mainData.Where(a => a.Id == Convert.ToInt32(id)).FirstOrDefault();
                        int index = mainData.IndexOf(objectData);
                        //mainData.FinalData.AddRange(index + 1, childList);
                        mainData.InsertRange(index + 1, (List<IncomeingListFilterData>)childList.IncomingData);
                    }
                }
            }
            foreach(var item in mainData)
            {
                item.AccountBalance = Math.Abs(item.AccountBalance);
            }
           
            int screenId = (int)SubFormsIds.IncomeList_GL;
            var tablesNames = new TablesNames()
            {
                FirstListName = "FinancialAccount",
            };


            var report = await _iGeneralPrint.PrintReport<object, IncomeingListFilterData, object>(null,mainData, null, tablesNames, otherdata
                , screenId, exportType, isArabic,  fileId );
            return report;


        }

        public double SumEndTermMerchandis(DateTime dateTo)
        {
            InventoryValuationType inventoryValuationType = new InventoryValuationType();
            if (GLGeneralSetting.evaluationMethodOfEndOfPeriodStockType == 1)
                inventoryValuationType = InventoryValuationType.PurchasePrice;
            else if(GLGeneralSetting.evaluationMethodOfEndOfPeriodStockType == 3)
                inventoryValuationType = InventoryValuationType.CostPrice;
            var invoices = _invoiceDetailsQuery.TableNoTracking
                                                .Include(x => x.InvoicesMaster)
                                                .Where(x =>  x.InvoicesMaster.InvoiceDate.Date <= dateTo.Date && x.ItemTypeId != 6);
            var items = _invStpItemCardMasterQuery.TableNoTracking.Include(x => x.Units).Where(x => x.TypeId != 6);

            var itemsPrices = _InvStpItemCardUnitQuery.TableNoTracking;

            var total = _IRpt_Store.getInventoryValuation(new Domain.Models.Request.Store.Reports.Store.InventoryValuationRequest
            {
                PageSize = 1,
                PageNumber = 1,
                inventoryValuationType = inventoryValuationType,
                dateTo = dateTo,
                storeId = -1
            },false).Result.totalItemsPrice;
            return total;
        }
        public List<IncomeingListFilterData> getAllDataIncomeList(List<int> IDs, DateTime From, DateTime To)
        {
            var Data = financialAccountRepositoryQuery.TableNoTracking
            .Include(h => h.GLJournalEntryDetails
            .Where(h => h.journalEntry.FTDate.Value.Date >= From.Date && h.journalEntry.FTDate.Value.Date <= To.Date))
            .Where(h => IDs.Contains(h.Id))
            .Select(h => new IncomeingListFilterData
            {
                Id = h.Id,
                ArabicName = h.ArabicName,
                LatinName = h.LatinName,
                FinancialAccountCode = h.AccountCode,
                AutoCode = h.autoCoding,
                ParentId = h.ParentId,
                debtor = h.GLJournalEntryDetails.Where(h =>h.journalEntry !=null && h.journalEntry.IsBlock==false &&  h.journalEntry.FTDate >= From && h.journalEntry.FTDate <= To).Sum(h =>  h.Debit),
                creditor = h.GLJournalEntryDetails.Where(h => h.journalEntry != null && h.journalEntry.IsBlock == false && h.journalEntry.FTDate >= From && h.journalEntry.FTDate <= To).Sum(h => h.Credit),
                hasChild = h.IsMain,
                accountnNatural = h.FA_Nature,
                AccountBalance = 0.0

            }).ToList (); 
            return Data;
        }

        public class IncomingListData: ReportOtherData
        {
            public double TotalBalance { get; set; }
            public double TotalBalanceDebitor { get; set; }

        }
    }
}
