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

//namespace App.Application.Services.Process.IncomeListReport
//{
//    public class IncomeListBusiness : BusinessBase<GLFinancialAccount>, IIncomeListBusiness
//    {
//        private readonly IRepositoryQuery<GLCurrency> currencyRepositoryQuery;
//        private readonly IRepositoryQuery<GLJournalEntry> journalEntryRepositoryQuery;
//        private readonly IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsRepositoryQuery;
//        private readonly IRepositoryQuery<GLBalanceForLastPeriod> balanceForLastPeriodRepositoryQuery;
//        private readonly IRepositoryCommand<GLBalanceForLastPeriod> balanceForLastPeriodRepositoryCommand;
//        private readonly IFinancialAccountBusiness financialAccountBusiness;
//        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
//        private readonly IRepositoryQuery<GLFinancialAccountForOpeningBalance> financialAccountForOpeningBalanceRepositoryQuery;
//        private readonly IRepositoryQuery<GLJournalEntryDetailsAccounts> journalEntryDetailsAccountsRepositoryQuery;
//        public IncomeListBusiness(
//            IRepositoryQuery<GLCurrency> CurrencyRepositoryQuery,
//            IRepositoryQuery<GLJournalEntry> JournalEntryRepositoryQuery,
//            IRepositoryQuery<GLJournalEntryDetails> JournalEntryDetailsRepositoryQuery,
//            IRepositoryQuery<GLFinancialAccountForOpeningBalance> FinancialAccountForOpeningBalanceRepositoryQuery,
//            IRepositoryQuery<GLFinancialAccount> FinancialAccountRepositoryQuery,
//            IFinancialAccountBusiness FinancialAccountBusiness,
//            IRepositoryQuery<GLBalanceForLastPeriod> BalanceForLastPeriodRepositoryQuery,
//            IRepositoryCommand<GLBalanceForLastPeriod> BalanceForLastPeriodRepositoryCommand,
//            IRepositoryQuery<GLJournalEntryDetailsAccounts> JournalEntryDetailsAccountsRepositoryQuery,
//            IRepositoryActionResult repositoryActionResult) : base(repositoryActionResult)
//        {
//            currencyRepositoryQuery = CurrencyRepositoryQuery;
//            journalEntryRepositoryQuery = JournalEntryRepositoryQuery;
//            financialAccountBusiness = FinancialAccountBusiness;
//            balanceForLastPeriodRepositoryQuery = BalanceForLastPeriodRepositoryQuery;
//            balanceForLastPeriodRepositoryCommand = BalanceForLastPeriodRepositoryCommand;
//            journalEntryDetailsAccountsRepositoryQuery = JournalEntryDetailsAccountsRepositoryQuery;
//            financialAccountForOpeningBalanceRepositoryQuery = FinancialAccountForOpeningBalanceRepositoryQuery;
//            journalEntryDetailsRepositoryQuery = JournalEntryDetailsRepositoryQuery;
//            financialAccountRepositoryQuery = FinancialAccountRepositoryQuery;
//        }
//        public async Task<string> AddAutomaticCode()
//        {
//            var code = financialAccountRepositoryQuery.GetAll();
//            var finan = await financialAccountRepositoryQuery.GetByAsync(q => q.ArabicName == "تكلفة النشاط التجاري");
//            int codee = (Convert.ToInt32(code.OrderBy(q => q.AccountCode).Where(q => q.ParentId == finan.Id).Last().AccountCode));
            
//            // var finan = financialAccountRepositoryQuery.FindAll(q => q.AccountCode !=null).Contains(NewCode.ToString());
//            return "0"+codee.ToString();
//        }
//        public async Task<IRepositoryActionResult> IncomeListRoot(IncomeListParameter paramters)
//        {
//            try
//            {
//                var journalEntry = journalEntryDetailsAccountsRepositoryQuery.FindQueryable(q => q.JournalEntryId > 0);
//                if (paramters.Search != null)
//                {

//                    if (paramters.Search.From != null)
//                        journalEntry = journalEntry.Where(q => q.JournalEntry.FTDate.Value.Date >= paramters.Search.From.Value.Date);
//                    if (paramters.Search.To != null)
//                        journalEntry = journalEntry.Where(q => q.JournalEntry.FTDate.Value.Date <= paramters.Search.To.Value.Date);

//                }
//                var llist = journalEntry.ToList();
//                var rootsQuery = financialAccountRepositoryQuery.FindQueryable(q => q.ParentId == null && q.FinalAccount == 2);
//                var roots = rootsQuery.ToList();
//                var list = new List<IncomeListDto>();
//                var totalsLists = new List<Totalss>();
//                var totalss = new Totalss();
//                double totalsFirst = 0;
//                double totalsSecond = 0;
//                int totalsFirstType = 0;
//                int totalsSecondType = 0;
//                foreach (var item in roots)
//                {
//                    var balance = new IncomeListDto();
//                    balance.ParentId = item.ParentId;
//                    balance.ArabicName = item.ArabicName;
//                    balance.LatinName = item.LatinName;
//                    balance.FinancialAccountCode = item.AccountCode;
//                    balance.Id = item.Id;
//                    var child = await CallChildren(item.Id, paramters);
//                    double credit = 0;
//                    double debit = 0;
//                    foreach (var ch in child)
//                    {
//                        if (ch.Type == 2)
//                        {
//                            credit += ch.TotalBalance;
//                        }
//                        else if (ch.Type == 1)
//                        {
//                            debit += ch.TotalBalance;
//                        }

//                    }
//                    if (credit > debit)
//                    {
//                        balance.TotalBalance = credit - debit;
//                        balance.Type = 2;
//                    }
//                    if (credit < debit)
//                    {
//                        balance.TotalBalance = debit - credit;
//                        balance.Type = 1;
//                    }
//                    if (item.ArabicName == "الايرادات")
//                    {
//                        totalsFirst = balance.TotalBalance;
//                        totalsFirstType = balance.Type;
//                    }
//                    if (item.ArabicName == "المصروفات والتكاليف")
//                    {
//                        totalsSecond = balance.TotalBalance;
//                        totalsSecondType = balance.Type;
//                    }
//                    balance.incomeListChildDtos.AddRange(child);
//                    totalss.incomeListDtos.Add(balance);
//                }
//                totalss.Totals = totalsSecond - totalsFirst;
//                var totalIncome = await balanceForLastPeriodRepositoryQuery.GetByAsync(q => q.BranchId == 1);
//                if (totalIncome!=null) {
//                    totalIncome.TotalIncomeList = totalss.Totals;
//                    await balanceForLastPeriodRepositoryCommand.UpdateAsyn(totalIncome);
//                    if (totalss.Totals < 0)
//                    {
//                        totalss.TotalType = 1;
//                    }
//                    else
//                    {
//                        totalss.TotalType = 2;
//                    }
//                }
//                    totalsLists.Add(totalss);
//                return repositoryActionResult.GetRepositoryActionResult(totalsLists, RepositoryActionStatus.Ok);
//            }
//            catch (Exception ex)
//            {
//                return repositoryActionResult.GetRepositoryActionResult(ex);

//            }
//        }
//        public async Task<List<IncomeListChildDto>> CallChildren(int Id, IncomeListParameter paramters)
//        {
//            var children = financialAccountRepositoryQuery.FindAll(q => q.ParentId == Id);
//            var list = new List<IncomeListChildDto>();
//            foreach (var item in children)
//            {
//                var child = new IncomeListChildDto();
//                child.Id = item.Id;
//                child.ParentId = item.ParentId;
//                child.ArabicName = item.ArabicName;
//                child.LatinName = item.LatinName;
//                child.FinancialAccountCode = item.AccountCode;
//                if (paramters.Search.To != null && paramters.Search.From != null)
//                {
//                    // Check between period of times
//                    var journalEntry = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= paramters.Search.From.Value.Date) &&
//                                                                                                               (q.journalEntry.FTDate <= paramters.Search.To.Value.Date));
//                    foreach (var journal in journalEntry)
//                    {
//                        double total = 0;
//                        if (journal.Credit != 0 && journal.Debit == 0 && child.TotalBalance == 0)
//                        {
//                            child.TotalBalance += journal.Credit;
//                            total = child.TotalBalance;
//                            child.Type = 2;
//                        }
//                        if (journal.Debit != 0 && journal.Credit == 0 && child.TotalBalance == 0)
//                        {
//                            child.TotalBalance += journal.Debit;
//                            total = child.TotalBalance;
//                            child.Type = 1;
//                        }

//                        if (journal.Debit != 0 && child.TotalBalance != 0 && journalEntry.First().Credit != 0 && total == 0)
//                        {
//                            if (journal.Debit > child.TotalBalance && child.Type == 2)
//                            {
//                                child.TotalBalance = journal.Debit - child.TotalBalance;
//                                total = child.TotalBalance;
//                                child.Type = 1;
//                            }
//                            else
//                            {
//                                child.TotalBalance = child.TotalBalance - journal.Debit;
//                                total = child.TotalBalance;
//                                child.Type = 2;
//                            }

//                        }
//                        if (journal.Credit != 0 && child.TotalBalance != 0 && journalEntry.First().Debit != 0 && total == 0)
//                        {
//                            if (journal.Credit > child.TotalBalance && child.Type == 1)
//                            {
//                                child.TotalBalance = journal.Credit - child.TotalBalance;
//                                total = child.TotalBalance;
//                                child.Type = 2;
//                            }
//                            else
//                            {
//                                child.TotalBalance = child.TotalBalance - journal.Credit;
//                                total = child.TotalBalance;
//                                child.Type = 1;
//                            }

//                        }
//                        if (journal.Credit != 0 && child.TotalBalance != 0 && total == 0)
//                        {
//                            child.TotalBalance += journal.Credit;
//                            child.Type = 2;
//                        }
//                        if (journal.Debit != 0 && child.TotalBalance != 0 && total == 0)
//                        {
//                            child.TotalBalance += journal.Debit;
//                            child.Type = 1;
//                        }
//                    }
//                    var openingBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= paramters.Search.From.Value.Date) &&
//                                                                                                                (q.Date.Date <= paramters.Search.To.Value.Date));
//                    if (openingBalance != null)
//                    {
//                        if (openingBalance.Debit != 0 && child.Type == 1)
//                        {
//                            child.TotalBalance += openingBalance.Debit;
//                            child.Type = 1;
//                        }
//                        if (openingBalance.Credit != 0 && child.Type == 2)
//                        {
//                            child.TotalBalance += openingBalance.Credit;
//                            child.Type = 2;
//                        }
//                        if (openingBalance.Debit != 0 && child.Type == 2)
//                        {
//                            if (openingBalance.Debit > child.TotalBalance)
//                            {
//                                child.TotalBalance = openingBalance.Debit - child.TotalBalance;
//                                child.Type = 1;
//                            }
//                            else
//                            {
//                                child.TotalBalance = child.TotalBalance - openingBalance.Debit;
//                                child.Type = 2;
//                            }
//                        }
//                        if (openingBalance.Credit != 0 && child.Type == 1)
//                        {
//                            if (openingBalance.Credit > child.TotalBalance)
//                            {
//                                child.TotalBalance = openingBalance.Credit - child.TotalBalance;
//                                child.Type = 1;
//                            }
//                            else
//                            {
//                                child.TotalBalance = child.TotalBalance - openingBalance.Credit;
//                                child.Type = 2;
//                            }
//                        }
//                    }
//                }
//                if (paramters.Search.To == null && paramters.Search.From == null)
//                {
//                    var today = DateTime.Now;
//                    var StartDate = new DateTime(today.Year, today.Month, 1);
//                    var EndDate = DateTime.Now;
//                    // Check between period of times
//                    var journalEntry = journalEntryDetailsRepositoryQuery.FindAll(q => q.FinancialAccountId == child.Id && (q.journalEntry.FTDate >= StartDate) &&
//                                                                                                               (q.journalEntry.FTDate <= EndDate));
//                    foreach (var journal in journalEntry)
//                    {
//                        double total = 0;
//                        if (journal.Credit != 0 && journal.Debit == 0 && child.TotalBalance == 0)
//                        {
//                            child.TotalBalance += journal.Credit;
//                            total = child.TotalBalance;
//                            child.Type = 2;
//                        }
//                        if (journal.Debit != 0 && journal.Credit == 0 && child.TotalBalance == 0)
//                        {
//                            child.TotalBalance += journal.Debit;
//                            total = child.TotalBalance;
//                            child.Type = 1;
//                        }

//                        if (journal.Debit != 0 && child.TotalBalance != 0 && journalEntry.First().Credit != 0 && total == 0)
//                        {
//                            if (journal.Debit > child.TotalBalance && child.Type == 2)
//                            {
//                                child.TotalBalance = journal.Debit - child.TotalBalance;
//                                total = child.TotalBalance;
//                                child.Type = 1;
//                            }
//                            else
//                            {
//                                child.TotalBalance = child.TotalBalance - journal.Debit;
//                                total = child.TotalBalance;
//                                child.Type = 2;
//                            }

//                        }
//                        if (journal.Credit != 0 && child.TotalBalance != 0 && journalEntry.First().Debit != 0 && total == 0)
//                        {
//                            if (journal.Credit > child.TotalBalance && child.Type == 1)
//                            {
//                                child.TotalBalance = journal.Credit - child.TotalBalance;
//                                total = child.TotalBalance;
//                                child.Type = 2;
//                            }
//                            else
//                            {
//                                child.TotalBalance = child.TotalBalance - journal.Credit;
//                                total = child.TotalBalance;
//                                child.Type = 1;
//                            }

//                        }
//                        if (journal.Credit != 0 && child.TotalBalance != 0 && total == 0)
//                        {
//                            child.TotalBalance += journal.Credit;
//                            child.Type = 2;
//                        }
//                        if (journal.Debit != 0 && child.TotalBalance != 0 && total == 0)
//                        {
//                            child.TotalBalance += journal.Debit;
//                            child.Type = 1;
//                        }
//                    }
//                    var openingBalance = await financialAccountForOpeningBalanceRepositoryQuery.GetByAsync(q => q.AccountCode == item.AccountCode && (q.Date.Date >= StartDate) &&
//                                                                                                                (q.Date.Date <= EndDate));
//                    if (openingBalance != null)
//                    {
//                        if (openingBalance.Debit != 0 && child.Type == 1)
//                        {
//                            child.TotalBalance += openingBalance.Debit;
//                            child.Type = 1;
//                        }
//                        if (openingBalance.Credit != 0 && child.Type == 2)
//                        {
//                            child.TotalBalance += openingBalance.Credit;
//                            child.Type = 2;
//                        }
//                        if (openingBalance.Debit != 0 && child.Type == 2)
//                        {
//                            if (openingBalance.Debit > child.TotalBalance)
//                            {
//                                child.TotalBalance = openingBalance.Debit - child.TotalBalance;
//                                child.Type = 1;
//                            }
//                            else
//                            {
//                                child.TotalBalance = child.TotalBalance - openingBalance.Debit;
//                                child.Type = 2;
//                            }
//                        }
//                        if (openingBalance.Credit != 0 && child.Type == 1)
//                        {
//                            if (openingBalance.Credit > child.TotalBalance)
//                            {
//                                child.TotalBalance = openingBalance.Credit - child.TotalBalance;
//                                child.Type = 1;
//                            }
//                            else
//                            {
//                                child.TotalBalance = child.TotalBalance - openingBalance.Credit;
//                                child.Type = 2;
//                            }
//                        }
//                    }
//                }
//                list.Add(child);
//                var gg = await AddAutomaticCode();
//                if (list.Last().FinancialAccountCode==gg)
//                {
//                    var childs = new IncomeListChildDto();
//                    childs.ArabicName = "بضاعة اخر مدة";
//                    childs.LatinName = "بضاعة اخر مدة";
//                    var lastBalance =await balanceForLastPeriodRepositoryQuery.GetByAsync(q=>q.BranchId==1);
//                    childs.TotalBalance = lastBalance.Balance;
//                    childs.Type = 1;
//                    list.Add(childs);
//                }
//                if (item.ParentId != null)
//                {
//                    var childes = await CallChildren(item.Id, paramters);
//                    double credit = 0;
//                    double debit = 0;
//                    if (item.ArabicName == "تكلفة النشاط التجاري")
//                    {
//                        var lastBalance = await balanceForLastPeriodRepositoryQuery.GetByAsync(q => q.BranchId == 1);
//                        if (lastBalance != null)
//                        {
//                            foreach (var ch in childes)
//                            {
//                                if (ch.TotalBalance != lastBalance.Balance)
//                                {
//                                    if (ch.Type == 2)
//                                    {
//                                        credit += ch.TotalBalance;
//                                    }
//                                    else if (ch.Type == 1)
//                                    {
//                                        debit += ch.TotalBalance;
//                                    }
//                                }
//                            }
//                            if (credit > debit)
//                            {
//                                child.TotalBalance = credit - debit;
//                                child.Type = 2;
//                            }
//                            if (credit < debit)
//                            {
//                                child.TotalBalance = debit - credit;
//                                child.Type = 1;
//                            }
//                            child.TotalBalance = lastBalance.Balance - child.TotalBalance;
//                        }
//                    }
//                    else
//                    {
//                        foreach (var ch in childes)
//                        {
//                            if (ch.Type == 2)
//                            {
//                                credit += ch.TotalBalance;
//                            }
//                            else if (ch.Type == 1)
//                            {
//                                debit += ch.TotalBalance;
//                            }

//                        }
//                        if (credit > debit)
//                        {
//                            child.TotalBalance = credit - debit;
//                            child.Type = 2;
//                        }
//                        if (credit < debit)
//                        {
//                            child.TotalBalance = debit - credit;
//                            child.Type = 1;
//                        }
//                    }
//                    child.Childes.AddRange(childes);
//                }
//            }
//            return list;
//        }
//    }
//}
