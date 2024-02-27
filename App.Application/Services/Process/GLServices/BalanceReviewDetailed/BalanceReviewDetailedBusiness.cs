using App.Application.Basic_Process;
using App.Application.Services.Process.FinancialAccounts;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Response;
using App.Infrastructure.Interfaces.Repository;
using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.BalanceReviewDetailed
{
    public class BalanceReviewDetailedBusiness : BusinessBase<GLFinancialAccount>, IBalanceReviewDetailedBusiness
    {
        private readonly IRepositoryQuery<GLCurrency> currencyRepositoryQuery;
        private readonly IRepositoryQuery<GLJournalEntry> journalEntryRepositoryQuery;
        private readonly IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsRepositoryQuery;
        private readonly IFinancialAccountBusiness financialAccountBusiness;
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GLFinancialAccountForOpeningBalance> financialAccountForOpeningBalanceRepositoryQuery;
       // private readonly IRepositoryQuery<GLJournalEntryDetailsAccounts> journalEntryDetailsAccountsRepositoryQuery;
        public BalanceReviewDetailedBusiness(
            IRepositoryQuery<GLCurrency> CurrencyRepositoryQuery,
            IRepositoryQuery<GLJournalEntry> JournalEntryRepositoryQuery,
            IRepositoryQuery<GLJournalEntryDetails> JournalEntryDetailsRepositoryQuery,
            IRepositoryQuery<GLFinancialAccountForOpeningBalance> FinancialAccountForOpeningBalanceRepositoryQuery,
            IRepositoryQuery<GLFinancialAccount> FinancialAccountRepositoryQuery,
            IFinancialAccountBusiness FinancialAccountBusiness,
            //IRepositoryQuery<GLJournalEntryDetailsAccounts> JournalEntryDetailsAccountsRepositoryQuery,
            IRepositoryActionResult repositoryActionResult) : base(repositoryActionResult)
        {
            currencyRepositoryQuery = CurrencyRepositoryQuery;
            journalEntryRepositoryQuery = JournalEntryRepositoryQuery;
            financialAccountBusiness = FinancialAccountBusiness;
            //journalEntryDetailsAccountsRepositoryQuery = JournalEntryDetailsAccountsRepositoryQuery;
            financialAccountForOpeningBalanceRepositoryQuery = FinancialAccountForOpeningBalanceRepositoryQuery;
            journalEntryDetailsRepositoryQuery = JournalEntryDetailsRepositoryQuery;
            financialAccountRepositoryQuery = FinancialAccountRepositoryQuery;
        }
        public async Task<IRepositoryActionResult> CallRootDetails(BalanceReviewDetailedParameter paramters)
        {

            //var journalEntry = journalEntryDetailsAccountsRepositoryQuery.FindQueryable(q => q.JournalEntryId > 0);
            //if (paramters.Search != null)
            //{
            //    if (paramters.Search.FinancialAcountId > 0)
            //        journalEntry = journalEntry.Where(q => q.FinancialAccount.Id == paramters.Search.FinancialAcountId);
            //}
            var rootsQuery = financialAccountRepositoryQuery.FindQueryable(q => q.ParentId == null);
            if (paramters.Search.FinancialAcountId > 0)
            {
                rootsQuery = rootsQuery.Where(q => q.Id == paramters.Search.FinancialAcountId);
            }
            var roots = rootsQuery.ToList();
            var list = new List<BalanceReviewDetailedDto>();
            var totalsLists = new List<TotalsDetailed>();
            var totals = new TotalsDetailed();
            double totalBalanceBalanceBetweenPeriodCredit = 0;
            double totalBalanceBalanceBetweenPeriodDebit = 0;
            double totalBalanceBeforePeriodDebit = 0;
            double totalBalanceBeforePeriodCredit = 0;
            double totalsBalanceCredit = 0;
            double totalsBalanceDebit = 0;
            double TotalPeriodCredit = 0;
            double TotalsPeriodDebit = 0;
            foreach (var item in roots)
            {
                var balance = new BalanceReviewDetailedDto();
                balance.BalanceBalanceBetweenPeriodCredit = 0;
                balance.BalanceBalanceBetweenPeriodDebit = 0;
                balance.ParentId = item.ParentId;
                balance.ArabicName = item.ArabicName;
                balance.LatinName = item.LatinName;
                balance.FinancialAccountCode = item.AccountCode;
                balance.Id = item.Id;
                var child = await CallChildren(item.Id, paramters);
                var periodList = new List<BalanceReviewPeriods>();
                if (paramters.Search.TypeOfPeriod == 1)
                {
                    int[] array = { 1, 2, 3 };
                    foreach (var arr in array)
                    {
                        var period = new BalanceReviewPeriods();
                        period.MonthName = arr;
                        foreach (var it in child)
                        {
                            foreach (var per in it.balanceReviewPeriods.Where(Q => Q.MonthName == arr))
                            {
                                period.BalanceMonthPeriodCredit += per.BalanceMonthPeriodCredit;
                                TotalPeriodCredit += period.BalanceMonthPeriodCredit;
                                period.BalanceMonthPeriodDebit += per.BalanceMonthPeriodDebit;
                                TotalsPeriodDebit += period.BalanceMonthPeriodDebit;
                            }

                        }
                        periodList.Add(period);
                    }
                    balance.balanceReviewPeriods.AddRange(periodList);
                }
                if (paramters.Search.TypeOfPeriod == 2)
                {
                    int[] array = { 1, 2, 3 ,4 ,5 ,6};
                    foreach (var arr in array)
                    {
                        var period = new BalanceReviewPeriods();
                        period.MonthName = arr;
                        foreach (var it in child)
                        {
                            foreach (var per in it.balanceReviewPeriods.Where(Q => Q.MonthName == arr))
                            {
                                period.BalanceMonthPeriodCredit += per.BalanceMonthPeriodCredit;
                                TotalPeriodCredit += period.BalanceMonthPeriodCredit;
                                period.BalanceMonthPeriodDebit += per.BalanceMonthPeriodDebit;
                                TotalsPeriodDebit += period.BalanceMonthPeriodDebit;
                            }

                        }
                        periodList.Add(period);
                    }
                    balance.balanceReviewPeriods.AddRange(periodList);
                }
                if (paramters.Search.TypeOfPeriod == 3)
                {
                    int[] array = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                    foreach (var arr in array)
                    {
                        var period = new BalanceReviewPeriods();
                        period.MonthName = arr;
                        foreach (var it in child)
                        {
                            foreach (var per in it.balanceReviewPeriods.Where(Q => Q.MonthName == arr))
                            {
                                period.BalanceMonthPeriodCredit += per.BalanceMonthPeriodCredit;
                                TotalPeriodCredit += period.BalanceMonthPeriodCredit;
                                period.BalanceMonthPeriodDebit += per.BalanceMonthPeriodDebit;
                                TotalsPeriodDebit += period.BalanceMonthPeriodDebit;
                            }

                        }
                        periodList.Add(period);
                    }
                    balance.balanceReviewPeriods.AddRange(periodList);
                }
                foreach (var itt in child)
                {
                    balance.BalanceBalanceBetweenPeriodCredit += itt.BalanceBetweenPeriodCredit;
                    balance.BalanceBalanceBetweenPeriodDebit += itt.BalanceBalanceBetweenPeriodDebit;
                    balance.BalanceBeforePeriodCredit += itt.BalanceBeforePeriodCredit;
                    balance.BalanceBeforePeriodDebit += itt.BalanceBeforePeriodDebit;
                }
                totalBalanceBalanceBetweenPeriodCredit += balance.BalanceBalanceBetweenPeriodCredit;
                totalBalanceBalanceBetweenPeriodDebit += balance.BalanceBalanceBetweenPeriodDebit;
                totalBalanceBeforePeriodDebit += balance.BalanceBeforePeriodDebit;
                totalBalanceBeforePeriodCredit += balance.BalanceBeforePeriodCredit;
                balance.TotalBalanceCredit = balance.BalanceBalanceBetweenPeriodCredit + balance.BalanceBeforePeriodCredit;
                totalsBalanceCredit += balance.TotalBalanceCredit;
                balance.TotalBalanceDebit = balance.BalanceBalanceBetweenPeriodDebit + balance.BalanceBeforePeriodDebit;
                totalsBalanceDebit += balance.TotalBalanceDebit;
        
                balance.balanceReviewChildsDtos.AddRange(child);

                totals.balanceReviewDtos.Add(balance);
       
            }

            totals.TotalBalanceBalanceBetweenPeriodCredit += totalBalanceBalanceBetweenPeriodCredit;
            totals.TotalBalanceBalanceBetweenPeriodDebit += totalBalanceBalanceBetweenPeriodDebit;
            totals.TotalBalanceBeforePeriodDebit += totalBalanceBeforePeriodDebit;
            totals.TotalBalanceBeforePeriodCredit += totalBalanceBeforePeriodCredit;
            totals.TotalsBalanceCredit += totalsBalanceCredit;
            totals.TotalsBalanceDebit += totalsBalanceDebit;
            totalsLists.Add(totals);
            return repositoryActionResult.GetRepositoryActionResult(totalsLists, RepositoryActionStatus.Ok);
        }
        public async Task<List<BalanceReviewChildsDetailedDto>> CallChildren(int Id, BalanceReviewDetailedParameter paramters)
        {
            var children = financialAccountRepositoryQuery.FindAll(q => q.ParentId == Id);
            var list = new List<BalanceReviewChildsDetailedDto>();
            foreach (var item in children)
            {
                var child = new BalanceReviewChildsDetailedDto();
                child.Id = item.Id;
                child.ParentId = item.ParentId;
                child.ArabicName = item.ArabicName;
                child.LatinName = item.LatinName;
                child.FinancialAccountCode = item.AccountCode;
                // Check between period of times
               
                var periodList = new List<BalanceReviewPeriods>();

                if (paramters.Search.TypeOfPeriod == 1 && paramters.Search.Monthes == 1)
                {
                    int[] array ={ 1,2,3};
                    foreach(int month in array)
                    {
                        var period = new BalanceReviewPeriods();

                        period.MonthName = month;
                        if (period.MonthName == 1)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("01-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("01-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("01-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("01-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 2)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("02-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("02-28-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("02-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("02-28-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 3)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("03-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("03-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("06-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("06-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        periodList.Add(period);
                        child.BalanceBalanceBetweenPeriodDebit += period.BalanceMonthPeriodDebit;
                        child.BalanceBetweenPeriodCredit += period.BalanceMonthPeriodCredit;
                    }
                }
                if (paramters.Search.TypeOfPeriod == 1 && paramters.Search.Monthes == 2)
                {
                    int[] array = { 1, 2, 3 };
                    foreach (int month in array)
                    {
                        var period = new BalanceReviewPeriods();

                        period.MonthName = month;
                        if (period.MonthName == 1)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("04-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("04-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("04-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("04-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 2)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("05-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("05-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("05-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("05-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 3)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("06-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("06-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("06-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("06-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        periodList.Add(period);
                        child.BalanceBalanceBetweenPeriodDebit += period.BalanceMonthPeriodDebit;
                        child.BalanceBetweenPeriodCredit += period.BalanceMonthPeriodCredit;
                    }
                }
                if (paramters.Search.TypeOfPeriod == 1 && paramters.Search.Monthes == 3)
                {
                    int[] array = { 1, 2, 3 };
                   
                    foreach (int month in array)
                    {
                        var period = new BalanceReviewPeriods();

                        period.MonthName = month;
                        if (period.MonthName == 1)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("07-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("07-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("07-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("07-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 2)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("08-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("08-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("08-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("08-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 3)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("09-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("09-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("09-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("09-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }

                        periodList.Add(period);
                        child.BalanceBalanceBetweenPeriodDebit += period.BalanceMonthPeriodDebit;
                        child.BalanceBetweenPeriodCredit += period.BalanceMonthPeriodCredit;
                    }
                }
                if (paramters.Search.TypeOfPeriod == 1 && paramters.Search.Monthes == 4)
                {
                    int[] array = { 1, 2, 3 };
                    foreach (int month in array)
                    {
                        var period = new BalanceReviewPeriods();

                        period.MonthName = month;
                        if (period.MonthName == 1)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("10-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("10-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("10-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("10-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 2)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("11-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("11-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("11-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("11-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 3)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("12-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("12-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("06-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("06-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        periodList.Add(period);
                        child.BalanceBalanceBetweenPeriodDebit += period.BalanceMonthPeriodDebit;
                        child.BalanceBetweenPeriodCredit += period.BalanceMonthPeriodCredit;
                    }
                }
                if (paramters.Search.TypeOfPeriod == 2 && paramters.Search.Monthes == 5)
                {
                    int[] array = { 1, 2, 3,4,5,6 };
                    foreach (int month in array)
                    {
                        var period = new BalanceReviewPeriods();

                        period.MonthName = month;
                        if (period.MonthName == 1)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("01-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("01-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("01-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("01-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 2)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("02-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("02-28-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("02-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("02-28-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 3)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("03-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("03-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("03-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("03-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 4)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("04-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("04-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("04-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("04-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 5)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("05-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("05-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("05-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("05-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 6)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("06-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("06-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("06-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("06-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        periodList.Add(period);
                        child.BalanceBalanceBetweenPeriodDebit += period.BalanceMonthPeriodDebit;
                        child.BalanceBetweenPeriodCredit += period.BalanceMonthPeriodCredit;
                    }
                }
                if (paramters.Search.TypeOfPeriod == 2 && paramters.Search.Monthes == 6)
                {
                    int[] array = { 1, 2, 3,4,5,6 };
                    foreach (int month in array)
                    {
                        var period = new BalanceReviewPeriods();

                        period.MonthName = month;
                        if (period.MonthName == 1)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("07-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("07-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("07-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("07-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 2)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("08-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("08-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("08-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("08-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 3)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("09-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("09-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("09-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("09-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 4)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("10-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("10-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("10-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("10-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 5)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("11-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("11-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("11-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("11-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 6)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("12-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("12-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("12-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("12-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        periodList.Add(period);
                        child.BalanceBalanceBetweenPeriodDebit += period.BalanceMonthPeriodDebit;
                        child.BalanceBetweenPeriodCredit += period.BalanceMonthPeriodCredit;
                    }
                }
                if (paramters.Search.TypeOfPeriod == 3 && paramters.Search.Monthes == 7)
                {
                    int[] array = { 1, 2, 3 ,4 ,5 ,6 ,7 ,8 ,9 ,10 ,11 ,12};
                    foreach (int month in array)
                    {
                        var period = new BalanceReviewPeriods();

                        period.MonthName = month;
                        if (period.MonthName == 1)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("01-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("01-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("01-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("01-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 2)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("02-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("02-28-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("02-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("02-28-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 3)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("03-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("03-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("03-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("03-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 4)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("04-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("04-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("04-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("04-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 5)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("05-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("05-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("05-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("05-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 6)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("06-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("06-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("06-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("06-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 7)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("07-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("07-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("07-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("07-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 8)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("08-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("08-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("08-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("08-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 9)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("09-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("09-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("09-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("09-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 10)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("10-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("10-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("10-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("10-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 11)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("11-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("11-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("11-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("11-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        if (period.MonthName == 12)
                        {
                            var OpenBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("12-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("12-30-2021")));
                            if (OpenBalance != null)
                            {
                                if (OpenBalance.Credit != 0)
                                {
                                    period.BalanceMonthPeriodCredit += OpenBalance.Credit;
                                }
                                if (OpenBalance.Debit != 0)
                                {
                                    period.BalanceMonthPeriodDebit += OpenBalance.Debit;
                                }
                            }
                            var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("12-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("12-30-2021")));
                            foreach (var journal in journals)
                            {
                                if (journal.Credit != 0 && (period.BalanceMonthPeriodCredit == 0 || period.BalanceMonthPeriodCredit != 0))
                                {
                                    period.BalanceMonthPeriodCredit += journal.Credit;
                                }
                                if (journal.Debit != 0 && (period.BalanceMonthPeriodDebit == 0 || period.BalanceMonthPeriodDebit != 0))
                                {
                                    period.BalanceMonthPeriodDebit += journal.Debit;
                                }
                            }
                        }
                        periodList.Add(period);
                        child.BalanceBalanceBetweenPeriodDebit += period.BalanceMonthPeriodDebit;
                        child.BalanceBetweenPeriodCredit += period.BalanceMonthPeriodCredit;
                    }
                }
                child.balanceReviewPeriods.AddRange(periodList);

                //Check before period of times

                if ((paramters.Search.TypeOfPeriod == 1 && paramters.Search.Monthes == 1) ||
                    (paramters.Search.TypeOfPeriod == 2 && paramters.Search.Monthes == 5) ||
                    (paramters.Search.TypeOfPeriod == 3 && paramters.Search.Monthes == 7))
                {
                    child.BalanceBeforePeriodCredit = 0;
                    child.BalanceBeforePeriodDebit = 0;
                }
                if (paramters.Search.TypeOfPeriod == 1 && paramters.Search.Monthes == 2)
                {
                    var OpenBalanceBefore = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("01-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("03-30-2021")));
                    if (OpenBalanceBefore != null)
                    {
                        if (OpenBalanceBefore.Credit != 0)
                        {
                            child.BalanceBeforePeriodCredit += OpenBalanceBefore.Credit;
                        }
                        if (OpenBalanceBefore.Debit != 0)
                        {
                            child.BalanceBeforePeriodDebit += OpenBalanceBefore.Debit;
                        }
                    }

                    var journalsBefore = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("01-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("03-30-2021")));
                    foreach (var journal in journalsBefore)
                    {
                        if (journal.Credit != 0 && (child.BalanceBeforePeriodCredit == 0 || child.BalanceBeforePeriodCredit != 0))
                        {
                            child.BalanceBeforePeriodCredit += journal.Credit;
                        }
                        if (journal.Debit != 0 && (child.BalanceBeforePeriodDebit == 0 || child.BalanceBeforePeriodDebit != 0))
                        {
                            child.BalanceBeforePeriodDebit += journal.Debit;
                        }
                    }
                }
                if (paramters.Search.TypeOfPeriod == 1 && paramters.Search.Monthes == 3 || paramters.Search.TypeOfPeriod == 2 && paramters.Search.Monthes == 6)
                {
                    var OpenBalanceBefore = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("01-01-2021")) &&
                                                                                                                   (q.Date.Date <= DateTime.Parse("06-30-2021")));
                    if (OpenBalanceBefore != null)
                    {
                        if (OpenBalanceBefore.Credit != 0)
                        {
                            child.BalanceBeforePeriodCredit += OpenBalanceBefore.Credit;
                        }
                        if (OpenBalanceBefore.Debit != 0)
                        {
                            child.BalanceBeforePeriodDebit += OpenBalanceBefore.Debit;
                        }
                    }

                    var journalsBefore = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("01-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("06-30-2021")));
                    foreach (var journal in journalsBefore)
                    {
                        if (journal.Credit != 0 && (child.BalanceBeforePeriodCredit == 0 || child.BalanceBeforePeriodCredit != 0))
                        {
                            child.BalanceBeforePeriodCredit += journal.Credit;
                        }
                        if (journal.Debit != 0 && (child.BalanceBeforePeriodDebit == 0 || child.BalanceBeforePeriodDebit != 0))
                        {
                            child.BalanceBeforePeriodDebit += journal.Debit;
                        }
                    }
                }
                if (paramters.Search.TypeOfPeriod == 1 && paramters.Search.Monthes == 4)
                {
                    var OpenBalanceBefore = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= DateTime.Parse("01-01-2021")) &&
                                                                                                                (q.Date.Date <= DateTime.Parse("09-30-2021")));
                    if (OpenBalanceBefore != null)
                    {
                        if (OpenBalanceBefore.Credit != 0)
                        {
                            child.BalanceBeforePeriodCredit += OpenBalanceBefore.Credit;
                        }
                        if (OpenBalanceBefore.Debit != 0)
                        {
                            child.BalanceBeforePeriodDebit += OpenBalanceBefore.Debit;
                        }
                    }

                    var journalsBefore = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= DateTime.Parse("01-01-2021")) &&
                                                                                                                       (q.journalEntry.FTDate <= DateTime.Parse("09-30-2021")));
                    foreach (var journal in journalsBefore)
                    {
                        if (journal.Credit != 0 && (child.BalanceBeforePeriodCredit == 0 || child.BalanceBeforePeriodCredit != 0))
                        {
                            child.BalanceBeforePeriodCredit += journal.Credit;
                        }
                        if (journal.Debit != 0 && (child.BalanceBeforePeriodDebit == 0 || child.BalanceBeforePeriodDebit != 0))
                        {
                            child.BalanceBeforePeriodDebit += journal.Debit;
                        }
                    }
                }


                child.TotalBalanceCredit = child.BalanceBetweenPeriodCredit + child.BalanceBeforePeriodCredit;
                child.TotalBalanceDebit = child.BalanceBalanceBetweenPeriodDebit + child.BalanceBeforePeriodDebit;
          
                list.Add(child);
                if (item.ParentId != null)
                {
                    var childes = await CallChildren(item.Id, paramters);
                    foreach (var it in childes)
                    {
                        child.BalanceBetweenPeriodCredit += it.BalanceBetweenPeriodCredit;
                        child.BalanceBalanceBetweenPeriodDebit += it.BalanceBalanceBetweenPeriodDebit;
                        child.BalanceBeforePeriodCredit += it.BalanceBeforePeriodCredit;
                        child.BalanceBeforePeriodDebit += it.BalanceBeforePeriodDebit;
                    }
                    child.TotalBalanceCredit = child.BalanceBetweenPeriodCredit + child.BalanceBeforePeriodCredit;
                    child.TotalBalanceDebit = child.BalanceBalanceBetweenPeriodDebit + child.BalanceBeforePeriodDebit;
           
                    child.Childes.AddRange(childes);
                }

            }
            return list;
        }
    }
}
