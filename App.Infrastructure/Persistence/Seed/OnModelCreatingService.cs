using App.Domain.Entities.Process.Store;
using App.Domain.Entities.Process;
using App.Domain.Entities;
using App.Infrastructure.Interfaces.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Domain.Entities.Setup;
using App.Domain.Entities.Process.Barcode;
using App.Domain;

namespace App.Infrastructure.Persistence.Seed
{
    public class OnModelCreatingService
    {
        private readonly IErpInitilizerData _initilizerData;

        public OnModelCreatingService(IErpInitilizerData erpInitilizerData)
        {
            _initilizerData = erpInitilizerData;
        }
        public void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InvStpStores>().HasData(_initilizerData.setDefultStores());
            modelBuilder.Entity<InvStorePlaces>().HasData(_initilizerData.setDefultStoresPlaces());
            modelBuilder.Entity<InvJobs>().HasData(_initilizerData.setInvJobs());
            modelBuilder.Entity<InvEmployees>().HasData(_initilizerData.setDefultEmployees());
            modelBuilder.Entity<GLBranch>().HasData(_initilizerData.ReturnBranchTypeList());
            modelBuilder.Entity<InvStoreBranch>().HasData(_initilizerData.setStoreBranch());
            modelBuilder.Entity<GLSafe>().HasData(_initilizerData.ReturnTreasuryTypeList());
            modelBuilder.Entity<GLBank>().HasData(_initilizerData.ReturnBanksTypeList());
            modelBuilder.Entity<GLBankBranch>().HasData(_initilizerData.GLBankBranch());
            modelBuilder.Entity<GLCurrency>().HasData(_initilizerData.ReturnWithCurrencyList());
            modelBuilder.Entity<InvEmployeeBranch>().HasData(_initilizerData.setEmployeesBranches());
            modelBuilder.Entity<userAccount>().HasData(_initilizerData.SetUserAccount());
            modelBuilder.Entity<GLOtherAuthorities>().HasData(_initilizerData.setGLOtherAuthorities());
            modelBuilder.Entity<otherSettings>().HasData(_initilizerData.setOtherSettings());
            modelBuilder.Entity<OtherSettingsBanks>().HasData(_initilizerData.setOtherSettingBanks());
            modelBuilder.Entity<OtherSettingsSafes>().HasData(_initilizerData.setOtherSettingSafes());
            modelBuilder.Entity<OtherSettingsStores>().HasData(_initilizerData.setOtherSettingStores());
            modelBuilder.Entity<permissionList>().HasData(_initilizerData.setPermissionLists());
            modelBuilder.Entity<UserAndPermission>().HasData(_initilizerData.setUserAndPermissions());
            modelBuilder.Entity<rules>().HasData(_initilizerData.setRules());


            modelBuilder.Entity<GLFinancialAccount>().HasData(_initilizerData.ReturnWithFinancialAccountList());
            modelBuilder.Entity<GLPurchasesAndSalesSettings>().HasData(_initilizerData.setGLPurchasesAndSalesSettings());
            modelBuilder.Entity<GLIntegrationSettings>().HasData(_initilizerData.setGLIntegrationSettings()); 




            modelBuilder.Entity<GLFinancialBranch>().HasData(_initilizerData.ReturnFinancialAccountBranches());
            modelBuilder.Entity<InvColors>().HasData(_initilizerData.setColors());
            modelBuilder.Entity<InvSizes>().HasData(_initilizerData.setInvSizes());
            modelBuilder.Entity<InvStpUnits>().HasData(_initilizerData.setInvStpUnits());
            modelBuilder.Entity<InvCategories>().HasData(_initilizerData.setInvCategories());
            modelBuilder.Entity<InvGeneralSettings>().HasData(_initilizerData.setInvGeneralSettings());
            modelBuilder.Entity<InvSalesMan>().HasData(_initilizerData.setInvSalesMan());
            modelBuilder.Entity<InvSalesMan_Branches>().HasData(_initilizerData.setInvSalesMan_Branches());
            modelBuilder.Entity<InvPersons>().HasData(_initilizerData.SetInvPersons());
            modelBuilder.Entity<InvPersons_Branches>().HasData(_initilizerData.setInvPersons_Branches());
            modelBuilder.Entity<InvFundsCustomerSupplier>().HasData(_initilizerData.setInvFundsCustomerSupplier());
            modelBuilder.Entity<InvPaymentMethods>().HasData(_initilizerData.setInvPaymentMethods());
            modelBuilder.Entity<InvStpItemCardMaster>().HasData(_initilizerData.setInvStpItemCardMaster());
            modelBuilder.Entity<InvStpItemCardUnit>().HasData(_initilizerData.setInvStpItemCardUnit());
            modelBuilder.Entity<InvCompanyData >().HasData(_initilizerData.setInvCompanyData());
            modelBuilder.Entity<InvBarcodeTemplate>().HasData(_initilizerData.setInvBarcodeTemplate());
            modelBuilder.Entity<InvBarcodeItems>().HasData(_initilizerData.setInvBarcodeItems());
            modelBuilder.Entity<GLGeneralSetting>().HasData(_initilizerData.SetGlGeneralSettings());
            modelBuilder.Entity<SubCodeLevels>().HasData(_initilizerData.setSubCodeLevels());
            modelBuilder.Entity<GLJournalEntry>().HasData(_initilizerData.SetGLJournalEntry());
            modelBuilder.Entity<ScreenName>().HasData(_initilizerData.setScreenName());
            modelBuilder.Entity<GLCostCenter>().HasData(_initilizerData.setGLCostCenter()); 

            
        }
    }
}
