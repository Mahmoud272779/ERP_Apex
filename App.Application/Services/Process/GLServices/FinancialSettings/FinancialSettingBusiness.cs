using App.Application.Basic_Process;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using Attendleave.Erp.Core.APIUtilities;
using Attendleave.Erp.ServiceLayer.Abstraction;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.FinancialSettings
{
    public class FinancialSettingBusiness : BusinessBase<GLFinancialSetting>, IFinancialSettingBusiness
    {
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GLFinancialSetting> financialSettingRepositoryQuery;
        private readonly IRepositoryCommand<GLFinancialSetting> financialSettingRepositoryCommand;
        private readonly IRepositoryQuery<GLBank> banksRepositoryQuery;
        private readonly IRepositoryCommand<GLBank> banksRepositoryCommand;
        private readonly IRepositoryQuery<GLSafe> treasuryRepositoryQuery;
        private readonly IRepositoryCommand<GLSafe> treasuryRepositoryCommand;
        private readonly IRepositoryCommand<GLFinancialAccount> financialAccountRepositoryCommand;
        private readonly IHttpContextAccessor httpContext;
        public FinancialSettingBusiness(
            IRepositoryQuery<GLFinancialAccount> FinancialAccountRepositoryQuery,
            IRepositoryQuery<GLFinancialSetting> FinancialSettingRepositoryQuery,
            IRepositoryCommand<GLFinancialAccount> FinancialAccountRepositoryCommand,
             IRepositoryCommand<GLFinancialSetting> FinancialSettingRepositoryCommand,
            IRepositoryQuery<GLBank> BanksRepositoryQuery,
            IRepositoryCommand<GLBank> BanksRepositoryCommand,
            IRepositoryQuery<GLSafe> TreasuryRepositoryQuery,
            IRepositoryCommand<GLSafe> TreasuryRepositoryCommand,
            IHttpContextAccessor HttpContext,
            IRepositoryActionResult repositoryActionResult) : base(repositoryActionResult)
        {
            financialAccountRepositoryQuery = FinancialAccountRepositoryQuery;
            financialAccountRepositoryCommand = FinancialAccountRepositoryCommand;
            financialSettingRepositoryQuery = FinancialSettingRepositoryQuery;
            httpContext = HttpContext;
            banksRepositoryQuery = BanksRepositoryQuery;
            treasuryRepositoryQuery = TreasuryRepositoryQuery;
            financialSettingRepositoryCommand = FinancialSettingRepositoryCommand;
            treasuryRepositoryCommand = TreasuryRepositoryCommand;
            banksRepositoryCommand = BanksRepositoryCommand;
        }
        public async Task<IRepositoryActionResult> AddFinancialSetting(FinancialSettingParameter parameter)
        {
            try
            {
                var table = new GLFinancialSetting();
                table.IsAssumption = parameter.IsAssumption;
                table.IsBanks = parameter.IsBanks;
                if (table.IsBanks==true) 
                {
                    if (table.IsAssumption == true)
                    {
                        table.FinancialAccountId = parameter.FinancialAccountId;
                        table.UseFinancialAccount = parameter.UseFinancialAccount;
                        table.AddUnderFinancialAccount = parameter.AddUnderFinancialAccount;
                        if (table.UseFinancialAccount == true)
                        {
                            var banks = banksRepositoryQuery.FindAll(q=>q.Id>0);
                            foreach (var bank in banks)
                            {
                                bank.FinancialAccountId = table.FinancialAccountId;
                                await banksRepositoryCommand.UpdateAsyn(bank);
                            }
                        }
                        if (table.AddUnderFinancialAccount == true)
                        {
                            var banks = banksRepositoryQuery.FindAll(q => q.Id > 0);
                            foreach (var bank in banks)
                            {
                                var financial = new GLFinancialAccount();
                                financial.BranchId = 0;
                                financial.CurrencyId = 1;
                                financial.ParentId = table.FinancialAccountId;
                                financial.ArabicName = bank.ArabicName;
                                financial.LatinName = bank.LatinName;
                                financial.FA_Nature = 1;
                                financial.Status = 1;
                                financialAccountRepositoryCommand.Add(financial);
                                bank.FinancialAccountId = table.FinancialAccountId;
                                await banksRepositoryCommand.UpdateAsyn(bank);
                            }
                        }
                    }
                }
                if (table.IsTreasuries == true)
                {
                    if (table.IsAssumption == true)
                    {
                        table.FinancialAccountId = parameter.FinancialAccountId;
                        table.UseFinancialAccount = parameter.UseFinancialAccount;
                        if (table.UseFinancialAccount == true)
                        {
                            var treasuries = treasuryRepositoryQuery.FindAll(q => q.Id > 0);
                            foreach (var treasury in treasuries)
                            {
                                treasury.FinancialAccountId = table.FinancialAccountId;
                                await treasuryRepositoryCommand.UpdateAsyn(treasury);
                            }
                        }
                        if (table.AddUnderFinancialAccount == true)
                        {
                            var treasuries = treasuryRepositoryQuery.FindAll(q => q.Id > 0);
                            foreach (var treasury in treasuries)
                            {
                                var financial = new GLFinancialAccount();
                                financial.BranchId = 0;
                                financial.CurrencyId = 1;
                                financial.ParentId = table.FinancialAccountId;
                                financial.ArabicName = treasury.ArabicName;
                                financial.LatinName = treasury.LatinName;
                                financial.FA_Nature = 1;
                                financial.Status = 1;
                                financialAccountRepositoryCommand.Add(financial);
                                treasury.FinancialAccountId = financial.Id;
                                await treasuryRepositoryCommand.UpdateAsyn(treasury);
                            }
                        }
                    }

                }
                financialSettingRepositoryCommand.Add(table);
                return repositoryActionResult.GetRepositoryActionResult(table.Id, RepositoryActionStatus.Created, message: "Saved Successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex, RepositoryActionStatus.Created, message: "Saved Successfully");

            }
        }
        public async Task<IRepositoryActionResult> GetFinancialSettingForBank()
        {
            var financialBank =await financialSettingRepositoryQuery.GetByAsync(q=>q.IsBanks==true);
            var financialDto = new FinancialSettingDto();
            if (financialBank != null)
            {
                financialDto.AddUnderFinancialAccount = financialBank.AddUnderFinancialAccount;
                financialDto.UseFinancialAccount = financialBank.UseFinancialAccount;
                financialDto.IsAssumption = financialBank.IsAssumption;
                financialDto.FinancialAccountId = financialBank.FinancialAccountId;
            }

            return repositoryActionResult.GetRepositoryActionResult(financialDto,status: RepositoryActionStatus.Ok, message: "OK");

        }
        public async Task<IRepositoryActionResult> GetFinancialSettingForTreasury()
        {
            var financialBank = await financialSettingRepositoryQuery.GetByAsync(q => q.IsTreasuries == true);
            var financialDto = new FinancialSettingDto();
            if (financialBank != null)
            {
                financialDto.AddUnderFinancialAccount = financialBank.AddUnderFinancialAccount;
                financialDto.UseFinancialAccount = financialBank.UseFinancialAccount;
                financialDto.IsAssumption = financialBank.IsAssumption;
                financialDto.FinancialAccountId = financialBank.FinancialAccountId;
            }
            return repositoryActionResult.GetRepositoryActionResult(financialDto, status: RepositoryActionStatus.Ok, message: "OK");

        }
        public async Task<IRepositoryActionResult> UpdateFinancialSettingForBank(UpdateFinancialSettingParameter parameter)
        {
            var financialBank = await financialSettingRepositoryQuery.GetByAsync(q => q.IsBanks == true);

            financialBank.IsAssumption = parameter.IsAssumption;
            if (financialBank.IsAssumption == true)
            {
                financialBank.FinancialAccountId = parameter.FinancialAccountId;
                financialBank.UseFinancialAccount = parameter.UseFinancialAccount;
                financialBank.AddUnderFinancialAccount = parameter.AddUnderFinancialAccount;
                if (financialBank.UseFinancialAccount == true)
                {
                    var banks = banksRepositoryQuery.FindAll(q => q.Id > 0);
                    foreach (var bank in banks)
                    {
                        bank.FinancialAccountId = financialBank.FinancialAccountId;
                        await banksRepositoryCommand.UpdateAsyn(bank);
                    }
                }
                if (financialBank.AddUnderFinancialAccount == true)
                {
                    var banks = banksRepositoryQuery.FindAll(q => q.Id > 0);
                    foreach (var bank in banks)
                    {
                        var financial = new GLFinancialAccount();
                        financial.BranchId = 0;
                        financial.CurrencyId = 1;
                        financial.ParentId = financialBank.FinancialAccountId;
                        financial.ArabicName = bank.ArabicName;
                        financial.LatinName = bank.LatinName;
                        financial.FA_Nature = 1;
                        financial.Status = 1;
                        financialAccountRepositoryCommand.Add(financial);
                        bank.FinancialAccountId = financialBank.FinancialAccountId;
                        await banksRepositoryCommand.UpdateAsyn(bank);
                    }
                }
            }
            return repositoryActionResult.GetRepositoryActionResult(financialBank.Id, status: RepositoryActionStatus.Ok, message: "OK");

        }
        //    financialBank.AddUnderFinancialAccount = parameter.AddUnderFinancialAccount;
        //    financialBank.UseFinancialAccount = parameter.UseFinancialAccount;
        //    financialBank.FinancialAccountId = parameter.FinancialAccountId;

        //public async Task<IRepositoryActionResult> UpdateFinancialSettingForTreasury()
        //{
        //    var financialBank = await financialSettingRepositoryQuery.GetByAsync(q => q.IsTreasuries == true);
        //    var financialDto = new FinancialSettingDto();
        //    if (financialBank != null)
        //    {
        //        financialDto.AddUnderFinancialAccount = financialBank.AddUnderFinancialAccount;
        //        financialDto.UseFinancialAccount = financialBank.UseFinancialAccount;
        //        financialDto.IsAssumption = financialBank.IsAssumption;
        //        financialDto.FinancialAccountId = financialBank.FinancialAccountId;
        //    }
        //    return repositoryActionResult.GetRepositoryActionResult(financialDto, status: RepositoryActionStatus.Ok, message: "OK");

        //}
    }
}
