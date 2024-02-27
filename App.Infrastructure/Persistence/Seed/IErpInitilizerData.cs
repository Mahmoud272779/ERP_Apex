using App.Domain;
using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Entities.Process.Barcode;
using App.Domain.Entities.Process.Store;
using App.Domain.Entities.Process.Store.Barcode;
using App.Domain.Entities.Setup;

namespace App.Infrastructure.Persistence.Seed
{
    public interface IErpInitilizerData
    {
        //branch tables
        GLBranch[] ReturnBranchTypeList();
        InvStoreBranch[] setStoreBranch();


        //currency table
        GLCurrency[] ReturnWithCurrencyList();


        //financial account tables
        GLFinancialAccount[] ReturnWithFinancialAccountList();
        GLFinancialBranch[] ReturnFinancialAccountBranches();


        //stores table
        InvStpStores[] setDefultStores();
        InvStorePlaces[] setDefultStoresPlaces();


        //bank tables
        GLBank[] ReturnBanksTypeList();
        GLBankBranch[] GLBankBranch();


        // safes table
        GLSafe[] ReturnTreasuryTypeList();


        //journal entry table
        GLJournalEntry[] SetGLJournalEntry();


        //employee tables
        InvJobs[] setInvJobs();
        InvEmployees[] setDefultEmployees();
        InvEmployeeBranch[] setEmployeesBranches();


        //user and permission tables
        permissionList[] setPermissionLists();
        userAccount[] SetUserAccount();
        UserAndPermission[] setUserAndPermissions();
        otherSettings[] setOtherSettings();
        OtherSettingsBanks[] setOtherSettingBanks();
        OtherSettingsSafes[] setOtherSettingSafes();
        OtherSettingsStores[] setOtherSettingStores();
        rules[] setRules();


        //item card tables
        InvSizes[] setInvSizes();
        InvStpUnits[] setInvStpUnits();
        InvCategories[] setInvCategories();
        InvStpItemCardMaster[] setInvStpItemCardMaster();
        InvStpItemCardUnit[] setInvStpItemCardUnit();
        InvColors[] setColors();



        //cost Center
        GLCostCenter[] setGLCostCenter();


        //persons tables
        InvSalesMan[] setInvSalesMan();
        InvSalesMan_Branches[] setInvSalesMan_Branches();
        InvPersons[] SetInvPersons();
        InvPersons_Branches[] setInvPersons_Branches();
        InvFundsCustomerSupplier[] setInvFundsCustomerSupplier();




        //payment method
        InvPaymentMethods[] setInvPaymentMethods();


        //Settings
        GLGeneralSetting[] SetGlGeneralSettings();
        SubCodeLevels[] setSubCodeLevels();
        InvCompanyData[] setInvCompanyData();
        InvGeneralSettings[] setInvGeneralSettings();
        GLPurchasesAndSalesSettings[] setGLPurchasesAndSalesSettings();
        GLOtherAuthorities[] setGLOtherAuthorities();
        ScreenName[] setScreenName();
        GLIntegrationSettings[] setGLIntegrationSettings();



        // not using
        InvBarcodeTemplate[] setInvBarcodeTemplate();
        InvBarcodeItems[] setInvBarcodeItems();




    }
}