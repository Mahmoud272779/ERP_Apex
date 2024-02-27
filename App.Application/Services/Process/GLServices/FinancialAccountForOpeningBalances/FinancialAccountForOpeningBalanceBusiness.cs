using App.Application.Basic_Process;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using Attendleave.Erp.Core.APIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.FinancialAccountForOpeningBalances
{
    public class FinancialAccountForOpeningBalanceBusiness : BusinessBase<GLFinancialAccountForOpeningBalance>, IFinancialAccountForOpeningBalanceBusiness
    {
        private readonly IRepositoryQuery<GLFinancialAccountForOpeningBalance> financialAccountOpeningBalanceRepositoryQuery;
        private readonly IRepositoryQuery<GLGeneralSetting> generalSettingRepositoryQuery;
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
         private readonly IRepositoryCommand<GLFinancialAccount> financialAccountRepositoryCommand;
        private readonly IRepositoryQuery<GLCurrency> currencyRepositoryQuery;
        private readonly IRepositoryQuery<GLJournalEntryDetails> journalEntryRepositoryQuery;
        private readonly IRepositoryCommand<GLFinancialAccountForOpeningBalance> financialAccountOpeningBalanceRepositoryCommand;
       // private readonly IPagedList<FinancialAccountDto, FinancialAccountDto> pagedListFinancialAccountDto;


        public FinancialAccountForOpeningBalanceBusiness(
            IRepositoryQuery<GLFinancialAccountForOpeningBalance> FinancialAccountOpeningBalanceRepositoryQuery,
            IRepositoryCommand<GLFinancialAccountForOpeningBalance> FinancialAccountOpeningBalanceRepositoryCommand,
            IRepositoryQuery<GLGeneralSetting> GeneralSettingRepositoryQuery,
            IRepositoryQuery<GLCurrency> CurrencyRepositoryQuery,
              IRepositoryQuery<GLFinancialAccount> FinancialAccountRepositoryQuery,
       IRepositoryCommand<GLFinancialAccount> FinancialAccountRepositoryCommand,
        IRepositoryQuery<GLJournalEntryDetails> JournalEntryRepositoryQuery,
           // IPagedList<FinancialAccountDto, FinancialAccountDto> PagedListFinancialAccountDto,
            IRepositoryActionResult repositoryActionResult) : base(repositoryActionResult)
        {
            financialAccountOpeningBalanceRepositoryQuery = FinancialAccountOpeningBalanceRepositoryQuery;
            financialAccountOpeningBalanceRepositoryCommand = FinancialAccountOpeningBalanceRepositoryCommand;
            generalSettingRepositoryQuery = GeneralSettingRepositoryQuery;
            financialAccountRepositoryQuery = FinancialAccountRepositoryQuery;
            financialAccountRepositoryCommand = FinancialAccountRepositoryCommand;
            journalEntryRepositoryQuery = JournalEntryRepositoryQuery;
            currencyRepositoryQuery = CurrencyRepositoryQuery;
            //pagedListFinancialAccountDto = PagedListFinancialAccountDto;
        }
        public async Task<IRepositoryActionResult> AddFinancialAccountForOpeningBalance(FinancialAccountForOpeningBalanceParameter parameter)
        {
            try
            {
                var table = Mapping.Mapper.Map<FinancialAccountForOpeningBalanceParameter, GLFinancialAccountForOpeningBalance>(parameter);
                table.Date = DateTime.Now;
                var financialAccount = await financialAccountRepositoryQuery.GetByAsync(q => q.AccountCode == table.AccountCode);
                if (financialAccount != null)
                {
                  
                    if (financialAccount.Debit == 0&& financialAccount.Credit == 0)
                    {
                        financialAccount.Debit = table.Debit;
                        financialAccount.Credit = table.Credit;
                    }
                    if (financialAccount.Debit!=0&& table.Debit!=0)
                    {
                        financialAccount.Debit = financialAccount.Debit+table.Debit;
                    }
                    if (financialAccount.Debit != 0 && table.Credit != 0)
                    {
                        if (financialAccount.Debit > table.Credit)
                        {
                            financialAccount.Debit = financialAccount.Debit + table.Credit;
                        }
                        if (financialAccount.Debit < table.Credit)
                        {
                            financialAccount.Debit =  table.Credit- financialAccount.Debit ;
                        }
                    }
                    if (financialAccount.Credit != 0 && table.Credit != 0)
                    {
                        financialAccount.Credit = financialAccount.Credit + table.Credit;
                    }
                    if (financialAccount.Credit != 0 && table.Debit != 0)
                    {
                        if (financialAccount.Credit > table.Debit)
                        {
                            financialAccount.Credit = financialAccount.Credit + table.Debit;
                        }
                        if (financialAccount.Credit < table.Debit)
                        {
                            financialAccount.Credit = table.Debit - financialAccount.Credit;
                        }
                    }
                    financialAccount.OpenningCredit = table.Credit;
                    financialAccount.OpenningDebit = table.Debit;
                    await financialAccountRepositoryCommand.UpdateAsyn(financialAccount);
                }
                financialAccountOpeningBalanceRepositoryCommand.Add(table);
                return repositoryActionResult.GetRepositoryActionResult(table.Id, RepositoryActionStatus.Created, message: "Saved Successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex, RepositoryActionStatus.BadRequest);

            }
            //return  new Task<bool>(() => true);
        }
        public async Task<IRepositoryActionResult> UpdateFinancialAccountForOpeningBalance(UpdateFinancialAccountForOpeningBalanceParameter parameter)
        {
            try
            {
                var updateData = await financialAccountOpeningBalanceRepositoryQuery.GetByAsync(q => q.Id == parameter.Id);
                var table = Mapping.Mapper.Map<UpdateFinancialAccountForOpeningBalanceParameter, GLFinancialAccountForOpeningBalance>(parameter, updateData);
                var financialAccount =await financialAccountRepositoryQuery.GetByAsync(q=>q.AccountCode== updateData.AccountCode);
                if (financialAccount!=null)
                {
                    if (financialAccount.Debit == 0 && financialAccount.Credit == 0)
                    {
                        financialAccount.Debit = table.Debit;
                        financialAccount.Credit = table.Credit;
                    }
                    if (financialAccount.Debit != 0 && table.Debit != 0)
                    {
                        financialAccount.Debit = financialAccount.Debit + table.Debit;
                    }
                    if (financialAccount.Debit != 0 && table.Credit != 0)
                    {
                        if (financialAccount.Debit > table.Credit)
                        {
                            financialAccount.Debit = financialAccount.Debit + table.Credit;
                        }
                        if (financialAccount.Debit < table.Credit)
                        {
                            financialAccount.Debit = table.Credit - financialAccount.Debit;
                        }
                    }
                    if (financialAccount.Credit != 0 && table.Credit != 0)
                    {
                        financialAccount.Credit = financialAccount.Credit + table.Credit;
                    }
                    if (financialAccount.Credit != 0 && table.Debit != 0)
                    {
                        if (financialAccount.Credit > table.Debit)
                        {
                            financialAccount.Credit = financialAccount.Credit + table.Debit;
                        }
                        if (financialAccount.Credit < table.Debit)
                        {
                            financialAccount.Credit = table.Debit - financialAccount.Credit;
                        }
                    }
                    financialAccount.OpenningCredit = table.Credit;
                    financialAccount.OpenningDebit = table.Debit;
                    await financialAccountRepositoryCommand.UpdateAsyn(financialAccount);
                }
                table.Date = DateTime.Now;
                await financialAccountOpeningBalanceRepositoryCommand.UpdateAsyn(table);
                return repositoryActionResult.GetRepositoryActionResult(table.Id, RepositoryActionStatus.Updated, message: "Updated Successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex, RepositoryActionStatus.BadRequest);

            }
            //return  new Task<bool>(() => true);
        }
    }
}
