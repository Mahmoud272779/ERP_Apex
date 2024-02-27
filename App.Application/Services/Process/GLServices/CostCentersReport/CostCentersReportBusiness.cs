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

namespace App.Application.Services.Process.CostCentersReport
{
    public class CostCentersReportBusiness : BusinessBase<GLCostCenter>, ICostCentersReportBusiness
    {
    private readonly IRepositoryQuery<GLCurrency> currencyRepositoryQuery;
    private readonly IRepositoryQuery<GLJournalEntry> journalEntryRepositoryQuery;
    private readonly IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsRepositoryQuery;
    private readonly IRepositoryQuery<GLBalanceForLastPeriod> balanceForLastPeriodRepositoryQuery;
    private readonly IRepositoryCommand<GLBalanceForLastPeriod> balanceForLastPeriodRepositoryCommand;
    private readonly IFinancialAccountBusiness financialAccountBusiness;
    private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
    private readonly IRepositoryQuery<GLCostCenter> costCenterRepositoryQuery;
    private readonly IRepositoryQuery<GLFinancialAccountForOpeningBalance> financialAccountForOpeningBalanceRepositoryQuery;
    //private readonly IRepositoryQuery<GLJournalEntryDetailsAccounts> journalEntryDetailsAccountsRepositoryQuery;
    public CostCentersReportBusiness(
        IRepositoryQuery<GLCurrency> CurrencyRepositoryQuery,
        IRepositoryQuery<GLJournalEntry> JournalEntryRepositoryQuery,
        IRepositoryQuery<GLJournalEntryDetails> JournalEntryDetailsRepositoryQuery,
        IRepositoryQuery<GLFinancialAccountForOpeningBalance> FinancialAccountForOpeningBalanceRepositoryQuery,
        IRepositoryQuery<GLFinancialAccount> FinancialAccountRepositoryQuery,
        IFinancialAccountBusiness FinancialAccountBusiness,
        IRepositoryQuery<GLBalanceForLastPeriod> BalanceForLastPeriodRepositoryQuery,
        IRepositoryCommand<GLBalanceForLastPeriod> BalanceForLastPeriodRepositoryCommand,
        IRepositoryQuery<GLCostCenter> CostCenterRepositoryQuery,
       // IRepositoryQuery<GLJournalEntryDetailsAccounts> JournalEntryDetailsAccountsRepositoryQuery,
        IRepositoryActionResult repositoryActionResult) : base(repositoryActionResult)
    {
        currencyRepositoryQuery = CurrencyRepositoryQuery;
        journalEntryRepositoryQuery = JournalEntryRepositoryQuery;
        financialAccountBusiness = FinancialAccountBusiness;
        costCenterRepositoryQuery = CostCenterRepositoryQuery;
        balanceForLastPeriodRepositoryQuery = BalanceForLastPeriodRepositoryQuery;
        balanceForLastPeriodRepositoryCommand = BalanceForLastPeriodRepositoryCommand;
        //journalEntryDetailsAccountsRepositoryQuery = JournalEntryDetailsAccountsRepositoryQuery;
        financialAccountForOpeningBalanceRepositoryQuery = FinancialAccountForOpeningBalanceRepositoryQuery;
        journalEntryDetailsRepositoryQuery = JournalEntryDetailsRepositoryQuery;
        financialAccountRepositoryQuery = FinancialAccountRepositoryQuery;
    }
        public async Task<IRepositoryActionResult> CallRootCostCenter(PageParameterCostCenterReport paramters)
        {

            var journalEntry = journalEntryDetailsRepositoryQuery.FindQueryable(q => q.JournalEntryId > 0);
            if (paramters.Search != null)
            {
                if (paramters.Search.From != null)
                    journalEntry = journalEntry.Where(q => q.journalEntry.FTDate.Value.Date >= paramters.Search.From.Value.Date);
                if (paramters.Search.To != null)
                    journalEntry = journalEntry.Where(q => q.journalEntry.FTDate.Value.Date <= paramters.Search.To.Value.Date);
                if (paramters.Search.CostCenterId != 0)
                    journalEntry = journalEntry.Where(q => q.CostCenterId <= paramters.Search.CostCenterId);
            }
            var rootsQuery = costCenterRepositoryQuery.FindQueryable(q => q.ParentId == null);
          
            var roots = rootsQuery.ToList();
            var list = new List<CostCenterReportDto>();
            foreach (var item in roots)
            {
                var balance = new CostCenterReportDto();
                balance.Id = item.Id;
                balance.CostCenterName = item.ArabicName;
                var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.CostCenterId == balance.Id);
                var jouranlList = new List<CostCenterDetails>();
                double credit = 0;
                double debit = 0;
                double Balancecredit = 0;
                double Balancedebit = 0;
                foreach (var jouranl in journals)
                {
                    var journalDetailsDto = new CostCenterDetails();
                    journalDetailsDto.JournalCode = jouranl.journalEntry.Code;
                    journalDetailsDto.JournalDate = jouranl?.journalEntry?.FTDate;
                    journalDetailsDto.Notes = jouranl?.journalEntry?.Notes;
                    if (jouranl.Credit!=0 && balance.costCenterDetails.Count() == 0)
                    {
                        journalDetailsDto.ProcessCredit = jouranl.Credit;
                        credit += journalDetailsDto.ProcessCredit;
                        journalDetailsDto.BalanceCredit = journalDetailsDto.ProcessCredit;
                        Balancecredit = journalDetailsDto.BalanceCredit;
                    }
                    if (jouranl.Debit!=0&& balance.costCenterDetails.Count() ==0)
                    {
                        journalDetailsDto.ProcessDebit = jouranl.Debit;
                        debit += journalDetailsDto.ProcessDebit;
                        journalDetailsDto.BalanceDebit = journalDetailsDto.ProcessDebit;
                        Balancedebit = journalDetailsDto.BalanceDebit;
                    }
                    if (balance.costCenterDetails.Count() > 0 &&(jouranl.Debit != 0 || jouranl.Credit != 0))
                    {
                        if (jouranl.Debit != 0)
                        {
                            journalDetailsDto.ProcessDebit = jouranl.Debit;
                            debit += journalDetailsDto.ProcessDebit;
                            var lst = balance.costCenterDetails.Last().BalanceDebit;
                            if (balance.costCenterDetails.Last().BalanceDebit==0 && jouranl.Debit > Balancecredit)
                            {
                                journalDetailsDto.BalanceDebit = jouranl.Debit - Balancecredit;
                                Balancedebit = journalDetailsDto.BalanceDebit;
                            }
                            if (balance.costCenterDetails.Last().BalanceDebit == 0 && jouranl.Debit < Balancecredit)
                            {
                                journalDetailsDto.BalanceCredit = Balancecredit - jouranl.Debit;
                                Balancecredit = journalDetailsDto.BalanceCredit;
                            }
                            if (balance.costCenterDetails.Last().BalanceDebit != 0 )
                            {
                                journalDetailsDto.BalanceDebit = jouranl.Debit + Balancedebit;
                                Balancedebit = journalDetailsDto.BalanceDebit;
                            }
                        

                            if (jouranl.Debit == Balancecredit)
                            {
                                journalDetailsDto.BalanceCredit = 0;
                                Balancecredit = journalDetailsDto.BalanceCredit;
                                journalDetailsDto.BalanceDebit = 0;
                                Balancedebit = journalDetailsDto.BalanceDebit;
                            }
                        }
                        if (jouranl.Credit != 0)
                        {
                            journalDetailsDto.ProcessCredit = jouranl.Credit;
                            credit += journalDetailsDto.ProcessCredit;
                            if (balance.costCenterDetails.Last().BalanceCredit == 0 && jouranl.Credit > Balancedebit)
                            {
                                journalDetailsDto.BalanceCredit = jouranl.Credit - Balancedebit;
                                Balancecredit = journalDetailsDto.BalanceCredit;
                            }
                            if (balance.costCenterDetails.Last().BalanceCredit == 0 && jouranl.Credit < Balancedebit)
                            {
                                journalDetailsDto.BalanceDebit = Balancedebit - jouranl.Credit;
                                Balancedebit = journalDetailsDto.BalanceDebit;
                            }
                            if (balance.costCenterDetails.Last().BalanceCredit != 0)
                            {
                                journalDetailsDto.BalanceCredit = jouranl.Credit + Balancecredit;
                                Balancecredit = journalDetailsDto.BalanceCredit;
                            }
                            if (jouranl.Credit == Balancedebit)
                            {
                                journalDetailsDto.BalanceCredit = 0;
                                Balancecredit = journalDetailsDto.BalanceCredit;
                                journalDetailsDto.BalanceDebit = 0;
                                Balancedebit = journalDetailsDto.BalanceDebit;
                            }
                        }
                    }
                    balance.TotalsCredit = credit;
                    balance.TotalsDebit = debit;
                    balance.TotalsBalanceCredit = journalDetailsDto.BalanceCredit;
                    balance.TotalsBalanceDebit = journalDetailsDto.BalanceDebit;
                    balance.costCenterDetails.Add(journalDetailsDto);
                }
                var child = await CallChildren(item.Id, paramters);
                balance.costCenterReportChildDtos.AddRange(child);
                list.Add(balance);
            }
            return repositoryActionResult.GetRepositoryActionResult(list, RepositoryActionStatus.Ok);
        }
        public async Task<List<CostCenterReportChildDto>> CallChildren(int Id, PageParameterCostCenterReport paramters)
        {
            var children = costCenterRepositoryQuery.FindAll(q => q.ParentId == Id);
            var list = new List<CostCenterReportChildDto>();
            foreach (var item in children)
            {
                var child = new CostCenterReportChildDto();
                child.Id = item.Id;
                child.CostCenterName = item.ArabicName;
                // Check between period of times

                var journals = journalEntryDetailsRepositoryQuery.FindAll(q => q.CostCenterId == child.Id);
                var jouranlList = new List<CostCenterDetails>();
                double credit = 0;
                double debit = 0;
                double Balancecredit = 0;
                double Balancedebit = 0;
                foreach (var jouranl in journals)
                {
                    var journalDetailsDto = new CostCenterDetails();
                    journalDetailsDto.JournalCode = jouranl.journalEntry.Code;
                    journalDetailsDto.JournalDate = jouranl?.journalEntry?.FTDate;
                    journalDetailsDto.Notes = jouranl?.journalEntry?.Notes;
                    if (jouranl.Credit != 0 && child.costCenterDetails.Count() == 0)
                    {
                        journalDetailsDto.ProcessCredit = jouranl.Credit;
                        credit += journalDetailsDto.ProcessCredit;
                        journalDetailsDto.BalanceCredit = journalDetailsDto.ProcessCredit;
                        Balancecredit = journalDetailsDto.BalanceCredit;
                    }
                    if (jouranl.Debit != 0 && child.costCenterDetails.Count() == 0)
                    {
                        journalDetailsDto.ProcessDebit = jouranl.Debit;
                        debit += journalDetailsDto.ProcessDebit;
                        journalDetailsDto.BalanceDebit = journalDetailsDto.ProcessDebit;
                        Balancedebit = journalDetailsDto.BalanceDebit;
                    }
                    if (child.costCenterDetails.Count() > 0 && (jouranl.Debit != 0 || jouranl.Credit != 0))
                    {
                        if (jouranl.Debit != 0)
                        {
                            journalDetailsDto.ProcessDebit = jouranl.Debit;
                            debit += journalDetailsDto.ProcessDebit;
                            var lst = child.costCenterDetails.Last().BalanceDebit;
                            if (child.costCenterDetails.Last().BalanceDebit == 0 && jouranl.Debit > Balancecredit)
                            {
                                journalDetailsDto.BalanceDebit = jouranl.Debit - Balancecredit;
                                Balancedebit = journalDetailsDto.BalanceDebit;
                            }
                            if (child.costCenterDetails.Last().BalanceDebit == 0 && jouranl.Debit < Balancecredit)
                            {
                                journalDetailsDto.BalanceCredit = Balancecredit - jouranl.Debit;
                                Balancecredit = journalDetailsDto.BalanceCredit;
                            }
                            if (child.costCenterDetails.Last().BalanceDebit != 0)
                            {
                                journalDetailsDto.BalanceDebit = jouranl.Debit + Balancedebit;
                                Balancedebit = journalDetailsDto.BalanceDebit;
                            }


                            if (jouranl.Debit == Balancecredit)
                            {
                                journalDetailsDto.BalanceCredit = 0;
                                Balancecredit = journalDetailsDto.BalanceCredit;
                                journalDetailsDto.BalanceDebit = 0;
                                Balancedebit = journalDetailsDto.BalanceDebit;
                            }
                        }
                        if (jouranl.Credit != 0)
                        {
                            journalDetailsDto.ProcessCredit = jouranl.Credit;
                            credit += journalDetailsDto.ProcessCredit;
                            if (child.costCenterDetails.Last().BalanceCredit == 0 && jouranl.Credit > Balancedebit)
                            {
                                journalDetailsDto.BalanceCredit = jouranl.Credit - Balancedebit;
                                Balancecredit = journalDetailsDto.BalanceCredit;
                            }
                            if (child.costCenterDetails.Last().BalanceCredit == 0 && jouranl.Credit < Balancedebit)
                            {
                                journalDetailsDto.BalanceDebit = Balancedebit - jouranl.Credit;
                                Balancedebit = journalDetailsDto.BalanceDebit;
                            }
                            if (child.costCenterDetails.Last().BalanceCredit != 0)
                            {
                                journalDetailsDto.BalanceCredit = jouranl.Credit + Balancecredit;
                                Balancecredit = journalDetailsDto.BalanceCredit;
                            }
                            if (jouranl.Credit == Balancedebit)
                            {
                                journalDetailsDto.BalanceCredit = 0;
                                Balancecredit = journalDetailsDto.BalanceCredit;
                                journalDetailsDto.BalanceDebit = 0;
                                Balancedebit = journalDetailsDto.BalanceDebit;
                            }
                        }
                    }
                    child.TotalsCredit = credit;
                    child.TotalsDebit = debit;
                    child.TotalsBalanceCredit = journalDetailsDto.BalanceCredit;
                    child.TotalsBalanceDebit = journalDetailsDto.BalanceDebit;
                    child.costCenterDetails.Add(journalDetailsDto);
                }
             
                list.Add(child);
                if (item.ParentId != null)
                {
                    var childes = await CallChildren(item.Id, paramters);
                    child.childes.AddRange(childes);
                }

            }
            return list;
        }
    }
}
