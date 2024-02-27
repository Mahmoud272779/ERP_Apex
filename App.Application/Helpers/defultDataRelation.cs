using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Entities.Process.Store;
using App.Domain.Enums;
using App.Domain.Models.Response.Store;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.settings;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;
using static App.Domain.Models.Shared.accountantTree;

namespace App.Application.Helpers
{
    public interface iDefultDataRelation
    {
        Task<bool> BranchsRelation(int branchId);
        Task<bool> AdministratorUserRelation(int Type, int Id);
    }
    internal class defultDataRelation : iDefultDataRelation
    {
        private readonly IRepositoryCommand<OtherSettingsStores> _otherSettingsStoresCommand;
        private readonly IRepositoryQuery<GLFinancialAccount> _GLFinancialAccountQuery;
        private readonly IRepositoryCommand<OtherSettingsSafes> _otherSettingsSafesCommand;
        private readonly IRepositoryCommand<OtherSettingsBanks> _otherSettingsBanksCommand;
        private readonly IRepositoryCommand<InvEmployeeBranch> _invEmployeeBranchCommand;
        private readonly IRepositoryCommand<InvPersons_Branches> _InvPersons_BranchesCommand;
        private readonly IRepositoryCommand<InvSalesMan_Branches> _InvSalesMan_BranchesCommand;
        private readonly IRepositoryCommand<GLIntegrationSettings> _gLIntegrationSettingsCommand;
        private readonly IRepositoryQuery<GLIntegrationSettings> _gLIntegrationSettingsQuery;
        private readonly IRepositoryCommand<GLPurchasesAndSalesSettings> _gLPurchasesAndSalesSettingsCommand;
        private readonly IRepositoryQuery<GLPurchasesAndSalesSettings> _gLPurchasesAndSalesSettingsQuery;

        public defultDataRelation(
                                    IRepositoryCommand<OtherSettingsStores> OtherSettingsStoresCommand,
                                    IRepositoryCommand<OtherSettingsSafes> OtherSettingsSafesCommand,
                                    IRepositoryCommand<OtherSettingsBanks> OtherSettingsBanksCommand,
                                    IRepositoryCommand<InvEmployeeBranch> InvEmployeeBranchCommand,
                                    IRepositoryCommand<InvPersons_Branches> InvPersons_BranchesCommand,
                                    IRepositoryCommand<InvSalesMan_Branches> InvSalesMan_BranchesCommand,
                                    IRepositoryCommand<GLIntegrationSettings> GLIntegrationSettingsCommand,
                                    IRepositoryCommand<GLPurchasesAndSalesSettings> GLPurchasesAndSalesSettingsCommand
,
                                    IRepositoryQuery<GLFinancialAccount> gLFinancialAccountQuery
,
                                    IRepositoryQuery<GLPurchasesAndSalesSettings> gLPurchasesAndSalesSettingsQuery,
                                    IRepositoryQuery<GLIntegrationSettings> gLIntegrationSettingsQuery)
        {
            _otherSettingsStoresCommand = OtherSettingsStoresCommand;
            _otherSettingsSafesCommand = OtherSettingsSafesCommand;
            _otherSettingsBanksCommand = OtherSettingsBanksCommand;
            _invEmployeeBranchCommand = InvEmployeeBranchCommand;
            _InvPersons_BranchesCommand = InvPersons_BranchesCommand;
            _InvSalesMan_BranchesCommand = InvSalesMan_BranchesCommand;
            _gLIntegrationSettingsCommand = GLIntegrationSettingsCommand;
            _gLPurchasesAndSalesSettingsCommand = GLPurchasesAndSalesSettingsCommand;
            _GLFinancialAccountQuery = gLFinancialAccountQuery;
            _gLPurchasesAndSalesSettingsQuery = gLPurchasesAndSalesSettingsQuery;
            _gLIntegrationSettingsQuery = gLIntegrationSettingsQuery;
        }
        /// <summary>
        /// type : 
        /// 1-bank
        /// 2-safe
        /// 3-store
        /// </summary>
        public async Task<bool> AdministratorUserRelation(int Type, int Id)
        {
            bool saved = false;
            if (Type == 1)
            {
                var banks = new OtherSettingsBanks()
                {
                    gLBankId = Id,
                    otherSettingsId = 1
                };
                saved = await _otherSettingsBanksCommand.AddAsync(banks);
            }
            if (Type == 2)
            {
                var safe = new OtherSettingsSafes()
                {
                    gLSafeId = Id,
                    otherSettingsId = 1
                };
                saved = await _otherSettingsSafesCommand.AddAsync(safe);
            }
            if (Type == 3)
            {
                var stores = new OtherSettingsStores()
                {
                    InvStpStoresId = Id,
                    otherSettingsId = 1
                };
                saved = await _otherSettingsStoresCommand.AddAsync(stores);
            }
            return saved;
        }

        public async Task<bool> BranchsRelation(int branchId)
        {
            bool saved = false;
            //Emplyees
            var empBranch = new InvEmployeeBranch()
            {
                BranchId = branchId,
                EmployeeId = 1
            };
            saved = await _invEmployeeBranchCommand.AddAsync(empBranch);
            //persons
            var personBranchs = new List<InvPersons_Branches>();
            personBranchs.AddRange(new[]
            {
                new InvPersons_Branches
                {
                    BranchId = branchId,
                    PersonId = 1
                },
                new InvPersons_Branches
                {
                    BranchId = branchId,
                    PersonId = 2
                }
            });
            _InvPersons_BranchesCommand.AddRange(personBranchs);
            saved = await _InvPersons_BranchesCommand.SaveAsync();
            //salesman
            var salesManBranches = new InvSalesMan_Branches()
            {
                BranchId = branchId,
                SalesManId = 1
            };
            _InvSalesMan_BranchesCommand.Add(salesManBranches);
            await _InvSalesMan_BranchesCommand.SaveAsync();

            var gLIntegrationSettings = _gLIntegrationSettingsQuery.TableNoTracking.Where(c=> c.GLBranchId == 1).ToList();
            gLIntegrationSettings.ForEach(c =>  { c.GLBranchId = branchId;c.Id = 0; });
            var list = gLIntegrationSettings;


            _gLIntegrationSettingsCommand.AddRange(list);
            saved = await _gLIntegrationSettingsCommand.SaveChanges() > 0 ? true : false;


            var PurchasesAndSalesSettings = _gLPurchasesAndSalesSettingsQuery.TableNoTracking.Where(c => c.branchId == 1).ToList();
            PurchasesAndSalesSettings.ForEach(c => { c.branchId = branchId;c.Id = 0; });

            SqlConnection con = new SqlConnection(_gLPurchasesAndSalesSettingsQuery.connectionString());
            con.Open();
            try
            {
                var SQLQuery = $"INSERT INTO [dbo].[InvFundsCustomerSupplier]([PersonId],[Credit],[Debit],[branchId]) select Id,0,0,{branchId} from InvPersons where not exists(select Id from [InvFundsCustomerSupplier] where branchId = {branchId})";
                con.Execute(SQLQuery);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                con.Close();
            }

            _gLPurchasesAndSalesSettingsCommand.AddRange(PurchasesAndSalesSettings);
            saved = await _gLPurchasesAndSalesSettingsCommand.SaveAsync();

            return saved;

        }

    }
}
