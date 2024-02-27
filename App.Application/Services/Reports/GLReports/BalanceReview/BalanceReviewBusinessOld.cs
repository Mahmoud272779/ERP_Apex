//using App.Application.Basic_Process;
//using App.Application.Services.Process.FinancialAccounts;
//using App.Domain.Entities.Process;
//using App.Domain.Models.Security.Authentication.Response;
//using App.Infrastructure.Interfaces.Repository;
//using Attendleave.Erp.Core.APIUtilities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static App.Domain.Enums.Enums;

//namespace App.Application.Services.Process.BalanceReview
//{
//    public class BalanceReviewBusinessOld : BusinessBase<GLFinancialAccount>, IBalanceReviewBusiness
//    {
//        private readonly IRepositoryQuery<GLCurrency> currencyRepositoryQuery;
//        private readonly IRepositoryQuery<GLJournalEntry> journalEntryRepositoryQuery;
//        private readonly IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsRepositoryQuery;
//        private readonly IFinancialAccountBusiness financialAccountBusiness;
//        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
//        private readonly IRepositoryQuery<GLFinancialAccountForOpeningBalance> financialAccountForOpeningBalanceRepositoryQuery;
//        private readonly IRepositoryQuery<GLJournalEntryDetailsAccounts> journalEntryDetailsAccountsRepositoryQuery;
//        public BalanceReviewBusinessOld(
//            IRepositoryQuery<GLCurrency> CurrencyRepositoryQuery,
//            IRepositoryQuery<GLJournalEntry> JournalEntryRepositoryQuery,
//            IRepositoryQuery<GLJournalEntryDetails> JournalEntryDetailsRepositoryQuery,
//            IRepositoryQuery<GLFinancialAccountForOpeningBalance> FinancialAccountForOpeningBalanceRepositoryQuery,
//            IRepositoryQuery<GLFinancialAccount> FinancialAccountRepositoryQuery,
//            IFinancialAccountBusiness FinancialAccountBusiness,
//            IRepositoryQuery<GLJournalEntryDetailsAccounts> JournalEntryDetailsAccountsRepositoryQuery,
//            IRepositoryActionResult repositoryActionResult) : base(repositoryActionResult)
//        {
//            currencyRepositoryQuery = CurrencyRepositoryQuery;
//            journalEntryRepositoryQuery = JournalEntryRepositoryQuery;
//            financialAccountBusiness = FinancialAccountBusiness;
//            journalEntryDetailsAccountsRepositoryQuery = JournalEntryDetailsAccountsRepositoryQuery;
//            financialAccountForOpeningBalanceRepositoryQuery = FinancialAccountForOpeningBalanceRepositoryQuery;
//            journalEntryDetailsRepositoryQuery = JournalEntryDetailsRepositoryQuery;
//            financialAccountRepositoryQuery = FinancialAccountRepositoryQuery;
//        }

//        public async Task<IRepositoryActionResult> getAllDataBalanceReview(BalanceReviewParameter paramters)
//        {
            
//            var journalEntry = journalEntryDetailsAccountsRepositoryQuery.FindQueryable(q => q.JournalEntryId > 0);
//            if (paramters.Search != null)
//            {
//                if (paramters.Search.Currency != null)
//                    journalEntry = journalEntry.Where(q => q.FinancialAccount.CurrencyId == paramters.Search.Currency);
//                if (paramters.Search.FinancialAcountId > 0)
//                    journalEntry = journalEntry.Where(q => q.FinancialAccount.Id == paramters.Search.FinancialAcountId);
//                if (paramters.Search.From != null)
//                    journalEntry = journalEntry.Where(q => q.JournalEntry.FTDate.Value.Date >= paramters.Search.From.Value.Date);
//                if (paramters.Search.To != null)
//                    journalEntry = journalEntry.Where(q => q.JournalEntry.FTDate.Value.Date <= paramters.Search.To.Value.Date);
//                if (!string.IsNullOrEmpty(paramters.Search.FinancialAccountName))
//                    journalEntry = journalEntry.Where(q => q.FinancialAccount.ArabicName.Contains(PrepareSearchCreteria(paramters.Search.FinancialAccountName)));
//            }
//            var rootsQuery = financialAccountRepositoryQuery.FindAll(q => q.ParentId == null);
//            //if (paramters.Search.FinancialAcountId >0)
//            //{
//            //    rootsQuery = rootsQuery.Where(q => q.Id == paramters.Search.FinancialAcountId);
//            //}
//            var roots = rootsQuery.ToList();
//            var list = new List<BalanceReviewDto>();
//            var totalsLists = new List<Totals>();
//            var totals = new Totals();
//            double totalBalanceBalanceBetweenPeriodCredit = 0;
//            double totalBalanceBalanceBetweenPeriodDebit = 0;
//            double totalBalanceBeforePeriodDebit = 0;
//            double totalBalanceBeforePeriodCredit = 0;
//            double totalsBalanceCredit = 0;
//            double totalsBalanceDebit = 0;
//            double balanceCreditTotals = 0;
//            double balanceDebitTotals = 0;
//            foreach (var item in roots)
//            {
//                var balance = new BalanceReviewDto();
//                balance.BalanceBalanceBetweenPeriodCredit = 0;
//                balance.BalanceBalanceBetweenPeriodDebit = 0;
//                balance.ParentId = item.ParentId;
//                balance.ArabicName = item.ArabicName;
//                balance.LatinName = item.LatinName;
//                balance.FinancialAccountCode = item.AccountCode;
//                balance.Id = item.Id;
//                var child= await CallChildren(item.Id,paramters);
//                foreach (var it in child)
//                {
//                    balance.BalanceBalanceBetweenPeriodCredit += it.BalanceBetweenPeriodCredit;
//                    balance.BalanceBalanceBetweenPeriodDebit += it.BalanceBalanceBetweenPeriodDebit;
//                    balance.BalanceBeforePeriodCredit += it.BalanceBeforePeriodCredit;
//                    balance.BalanceBeforePeriodDebit += it.BalanceBeforePeriodDebit;
//                }
//                totalBalanceBalanceBetweenPeriodCredit += balance.BalanceBalanceBetweenPeriodCredit;
//                totalBalanceBalanceBetweenPeriodDebit += balance.BalanceBalanceBetweenPeriodDebit;
//                totalBalanceBeforePeriodDebit += balance.BalanceBeforePeriodDebit;
//                totalBalanceBeforePeriodCredit += balance.BalanceBeforePeriodCredit;
//                balance.TotalBalanceCredit = balance.BalanceBalanceBetweenPeriodCredit + balance.BalanceBeforePeriodCredit;
//                totalsBalanceCredit += balance.TotalBalanceCredit;
//                balance.TotalBalanceDebit = balance.BalanceBalanceBetweenPeriodDebit + balance.BalanceBeforePeriodDebit;
//                totalsBalanceDebit += balance.TotalBalanceDebit;
//                if (balance.TotalBalanceCredit > balance.TotalBalanceDebit)
//                {
//                    balance.BalanceCredit = balance.TotalBalanceCredit - balance.TotalBalanceDebit;
//                }
//                balanceCreditTotals += balance.BalanceCredit;
//                if (balance.TotalBalanceCredit < balance.TotalBalanceDebit)
//                {
//                    balance.BalanceDebit = balance.TotalBalanceDebit - balance.TotalBalanceCredit;
//                }
//                balanceDebitTotals += balance.BalanceDebit;
//                balance.balanceReviewChildsDtos.AddRange(child);

//                totals.balanceReviewDtos.Add(balance);

//            }
          
//            totals.TotalBalanceBalanceBetweenPeriodCredit += totalBalanceBalanceBetweenPeriodCredit;
//            totals.TotalBalanceBalanceBetweenPeriodDebit += totalBalanceBalanceBetweenPeriodDebit;
//            totals.TotalBalanceBeforePeriodDebit += totalBalanceBeforePeriodDebit;
//            totals.TotalBalanceBeforePeriodCredit += totalBalanceBeforePeriodCredit;
//            totals.TotalsBalanceCredit += totalsBalanceCredit;
//            totals.TotalsBalanceDebit += totalsBalanceDebit;
//            totals.BalanceCreditTotals = balanceCreditTotals;
//            totals.BalanceDebitTotals = balanceDebitTotals;
//            totalsLists.Add(totals);
//            return repositoryActionResult.GetRepositoryActionResult(totalsLists, RepositoryActionStatus.Ok);
//        }
//        public async Task<List<BalanceReviewChildsDto>> CallChildren(int Id, BalanceReviewParameter paramters)
//        {
//            var children = financialAccountRepositoryQuery.FindAll(q=>q.ParentId==Id).ToList();
//            var list = new List<BalanceReviewChildsDto>();
//            foreach (var item in children)
//            {
//                var child = new BalanceReviewChildsDto();
//                child.Id = item.Id;
//                child.ParentId = item.ParentId;
//                child.ArabicName = item.ArabicName;
//                child.LatinName = item.LatinName;
//                child.FinancialAccountCode = item.AccountCode;
//                if (paramters.Search.To !=null && paramters.Search.From!=null)
//                {
//                    // Check between period of times
//                    var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= paramters.Search.From.Value.Date) &&
//                                                                                                           (q.Date.Date <= paramters.Search.To.Value.Date));
//                    if (OpenBalance != null)
//                    {
//                        if (OpenBalance.Credit != 0)
//                        {
//                            child.BalanceBetweenPeriodCredit += OpenBalance.Credit;
//                        }
//                        if (OpenBalance.Debit != 0)
//                        {
//                            child.BalanceBalanceBetweenPeriodDebit += OpenBalance.Debit;
//                        }
//                    }

//                    var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate.Value.Date >= paramters.Search.From.Value.Date)
//                                                                                          && (q.journalEntry.FTDate.Value.Date <= paramters.Search.To.Value.Date));
//                    foreach (var journal in journals)
//                    {
//                        if (journal.Credit != 0 && (child.BalanceBetweenPeriodCredit == 0 || child.BalanceBetweenPeriodCredit != 0))
//                        {
//                            child.BalanceBetweenPeriodCredit += journal.Credit;
//                        }
//                        if (journal.Debit != 0 && (child.BalanceBalanceBetweenPeriodDebit == 0 || child.BalanceBalanceBetweenPeriodDebit != 0))
//                        {
//                            child.BalanceBalanceBetweenPeriodDebit += journal.Debit;
//                        }
//                    }
//                    //Check before period of times
//                    var OpenBalanceBefore = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date < paramters.Search.From.Value.Date) &&
//                                                                                                          (q.Date.Date < paramters.Search.To.Value.Date));
//                    if (OpenBalanceBefore != null)
//                    {
//                        if (OpenBalanceBefore.Credit != 0)
//                        {
//                            child.BalanceBeforePeriodCredit += OpenBalanceBefore.Credit;
//                        }
//                        if (OpenBalanceBefore.Debit != 0)
//                        {
//                            child.BalanceBeforePeriodDebit += OpenBalanceBefore.Debit;
//                        }
//                    }

//                    var journalsBefore = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate.Value.Date < paramters.Search.From.Value.Date)
//                                                                                          && (q.journalEntry.FTDate.Value.Date < paramters.Search.To.Value.Date));
//                    foreach (var journal in journalsBefore)
//                    {
//                        if (journal.Credit != 0 && (child.BalanceBeforePeriodCredit == 0 || child.BalanceBeforePeriodCredit != 0))
//                        {
//                            child.BalanceBeforePeriodCredit += journal.Credit;
//                        }
//                        if (journal.Debit != 0 && (child.BalanceBeforePeriodDebit == 0 || child.BalanceBeforePeriodDebit != 0))
//                        {
//                            child.BalanceBeforePeriodDebit += journal.Debit;
//                        }
//                    }
//                }
//                if (paramters.Search.To == null && paramters.Search.From == null)
//                {
//                    var today = DateTime.Now;
//                    var StartDate = new DateTime(today.Year, today.Month, 1);
//                    var EndDate = DateTime.Now;
//                    // Check between period of times
//                    var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= StartDate) &&
//                                                                                                           (q.Date.Date <= EndDate));
//                    if (OpenBalance != null)
//                    {
//                        if (OpenBalance.Credit != 0)
//                        {
//                            child.BalanceBetweenPeriodCredit += OpenBalance.Credit;
//                        }
//                        if (OpenBalance.Debit != 0)
//                        {
//                            child.BalanceBalanceBetweenPeriodDebit += OpenBalance.Debit;
//                        }
//                    }

//                    var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate.Value.Date >= StartDate)
//                                                                                          && (q.journalEntry.FTDate.Value.Date <= EndDate));
//                    foreach (var journal in journals)
//                    {
//                        if (journal.Credit != 0 && (child.BalanceBetweenPeriodCredit == 0 || child.BalanceBetweenPeriodCredit != 0))
//                        {
//                            child.BalanceBetweenPeriodCredit += journal.Credit;
//                        }
//                        if (journal.Debit != 0 && (child.BalanceBalanceBetweenPeriodDebit == 0 || child.BalanceBalanceBetweenPeriodDebit != 0))
//                        {
//                            child.BalanceBalanceBetweenPeriodDebit += journal.Debit;
//                        }
//                    }
//                    //Check before period of times
//                    var OpenBalanceBefore = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date < StartDate) &&
//                                                                                                          (q.Date.Date < EndDate));
//                    if (OpenBalanceBefore != null)
//                    {
//                        if (OpenBalanceBefore.Credit != 0)
//                        {
//                            child.BalanceBeforePeriodCredit += OpenBalanceBefore.Credit;
//                        }
//                        if (OpenBalanceBefore.Debit != 0)
//                        {
//                            child.BalanceBeforePeriodDebit += OpenBalanceBefore.Debit;
//                        }
//                    }

//                    var journalsBefore = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate.Value.Date < StartDate)
//                                                                                          && (q.journalEntry.FTDate.Value.Date < EndDate));
//                    foreach (var journal in journalsBefore)
//                    {
//                        if (journal.Credit != 0 && (child.BalanceBeforePeriodCredit == 0 || child.BalanceBeforePeriodCredit != 0))
//                        {
//                            child.BalanceBeforePeriodCredit += journal.Credit;
//                        }
//                        if (journal.Debit != 0 && (child.BalanceBeforePeriodDebit == 0 || child.BalanceBeforePeriodDebit != 0))
//                        {
//                            child.BalanceBeforePeriodDebit += journal.Debit;
//                        }
//                    }
//                }
//                child.TotalBalanceCredit = child.BalanceBetweenPeriodCredit + child.BalanceBeforePeriodCredit;
//                child.TotalBalanceDebit = child.BalanceBalanceBetweenPeriodDebit + child.BalanceBeforePeriodDebit;
//                if (child.TotalBalanceCredit> child.TotalBalanceDebit)
//                {
//                    child.BalanceCredit = child.TotalBalanceCredit - child.TotalBalanceDebit;
//                }
//                if (child.TotalBalanceCredit < child.TotalBalanceDebit)
//                {
//                    child.BalanceDebit = child.TotalBalanceDebit - child.TotalBalanceCredit;
//                }
//                list.Add(child);
//                if (item.ParentId != null)
//                {
//                    var childes=await CallChildren(item.Id,paramters);
//                    foreach (var it in childes)
//                    {
//                        child.BalanceBetweenPeriodCredit += it.BalanceBetweenPeriodCredit;
//                        child.BalanceBalanceBetweenPeriodDebit += it.BalanceBalanceBetweenPeriodDebit;
//                        child.BalanceBeforePeriodCredit += it.BalanceBeforePeriodCredit;
//                        child.BalanceBeforePeriodDebit += it.BalanceBeforePeriodDebit;
//                    }
//                    child.TotalBalanceCredit = child.BalanceBetweenPeriodCredit + child.BalanceBeforePeriodCredit;
//                    child.TotalBalanceDebit = child.BalanceBalanceBetweenPeriodDebit + child.BalanceBeforePeriodDebit;
//                    if (child.TotalBalanceCredit > child.TotalBalanceDebit)
//                    {
//                        child.BalanceCredit = child.TotalBalanceCredit - child.TotalBalanceDebit;
//                    }
//                    if (child.TotalBalanceCredit < child.TotalBalanceDebit)
//                    {
//                        child.BalanceDebit = child.TotalBalanceDebit - child.TotalBalanceCredit;
//                    }
//                    child.Childes.AddRange(childes);
//                }
                
//            }
//            return list;
//        }



//    }
//}
