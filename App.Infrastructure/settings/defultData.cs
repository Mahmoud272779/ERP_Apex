using App.Domain.Entities.Process;
using App.Domain.Enums;
using App.Domain.Models.Shared;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static App.Domain.Enums.Enums;
using static App.Domain.Models.Shared.accountantTree;

namespace App.Infrastructure.settings
{
    public class defultData
    {
        public const int updateNumber = 14;
        public const string TechUser = "$#^%^#$%#%^%#*asd-TechnicalUser";
        public const int JWT_TOKEN_VALIDAIT_Time = 30;
        public const string JWT_SECURITY_KEY = "qmnf8u1rvesudp2va527mni6jnc215lyhz3vzackeuki9d3pob3g4bct4flcad0xi9i790xu082frbbls4rnlt6feyhu1nhp25mibp21na74b8vtic69c";
        public const string encryptStringKey = "b23af4858a4e4133bbhe2ey7355a1926";
        public const string site = "http://www.Test.com";
        public const string userManagmentApplicationSecurityKey = "%wW5HexK6w77a4rS6$^s^DNG";
        public const bool isAppAuth = true;
        public const string userManagmentApplication_APISURL = "http://41.39.232.30:803";
        public const string SignalRKey = "SignalR";
        public const string ProfitKey = "Profit";
        public const double timeBetweenForget = 1;
        public const string offlineDevPassword = "!@$%#$%ASFASFDasdqw545@#$@#$@#$@_offlineDev";
        public const string DebtAgingForCustomres_CashKey = "DebtAgingForCustomres";
        public const string DebtAgingForSuplier_CashKey = "DebtAgingForSuplier";


        //SignalR Methods 
        public const string POSSessionClose = "POSSessionClose";
        public const string privateChat = "privateChat";
        public const string joinGroup = "joinGroup";
        public const string groupMessage = "groupMessage";
        public const string addeddToChatGroup = "addeddToChatGroup";
        public const string removedFromGroup = "removedFromGroup";
        public const string leftGroup = "leftGroup";
        public const string broadcast = "broadcast";
        public const string DemandLimit = "DemandLimit";
        public const string ProfitProgress = "ProfitProgress";
        public const string Notification = "Notification";
        public const string LogoutNotification = "LogoutNotification";
        public const string ReloadNotification = "forceReload";
        public const string deployingSystem = "deployingSystem";



        public const string datetimeFormat = "yyyy-MM-ddTHH:mm:ss";
        public const string text_danger = "text-danger";
        public const string CollationsCaseSensitivity = "SQL_Latin1_General_CP1_CS_AS";

        //Securty Defult Values Ids
        public const int additionalUser = 5;
        public const int pos = 2;
        public const int additionalEmployee = 3;
        public const int AdittionStore = 1;
        public const bool isMigration = false;

        public static string ERPConnectionString(IConfiguration _configuration,string dbName)
        {
            var connection =    @"Data Source=" + _configuration["ApplicationSetting:serverName"] + ";" +
                                "Initial Catalog=" + dbName + ";" +
                                "user id=" + _configuration["ApplicationSetting:UID"] + ";" +
                                "password=" + _configuration["ApplicationSetting:Password"] + ";" +
                                "MultipleActiveResultSets=true;";
            return connection;
        }


        public static List<GLPurchasesAndSalesSettings> New_getlistOfGLPurchasesAndSalesSettingsTable()
        {
            var list = new List<GLPurchasesAndSalesSettings>();
            list.AddRange(new[]
            {
                #region Purchases
                new GLPurchasesAndSalesSettings
                {
                    Id = -1,
                    ArabicName = purAndSalesSettingNames.PurchaseAR,
                    LatinName = purAndSalesSettingNames.PurchaseEN,
                    MainType = (int)MainInvoiceType.Purchases,
                    RecieptsType = (int)DocumentType.Purchase,
                    ReceiptElemntID = 1,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.InventoryMainStore,
                    branchId = 1,
                },
                new GLPurchasesAndSalesSettings
                {
                    Id = -2,
                    ArabicName = purAndSalesSettingNames.PurchaseReturnAR,
                    LatinName = purAndSalesSettingNames.PurchaseReturnEN,
                    MainType = (int)MainInvoiceType.Purchases,
                    RecieptsType = (int)DocumentType.ReturnPurchase,
                    ReceiptElemntID = 1,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.InventoryMainStore,
                    branchId = 1,
                },
                new GLPurchasesAndSalesSettings
                {
                    Id =-3,
                    ArabicName = purAndSalesSettingNames.PurchaseWithoutVatAR,
                    LatinName = purAndSalesSettingNames.PurchaseWithoutVatEN,
                    MainType = (int)MainInvoiceType.Purchases,
                    RecieptsType = (int)DocumentType.wov_purchase,
                    ReceiptElemntID = 1,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.InventoryMainStore,
                    branchId = 1,

                },
                new GLPurchasesAndSalesSettings
                {
                    Id =-4,
                    ArabicName = purAndSalesSettingNames.PurchaseWithoutVatReturnAR,
                    LatinName = purAndSalesSettingNames.PurchaseWithoutVatReturnEN,
                    MainType = (int)MainInvoiceType.Purchases,
                    RecieptsType = (int)DocumentType.ReturnWov_purchase,
                    ReceiptElemntID = 1,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.InventoryMainStore,
                    branchId = 1,
                },
                new GLPurchasesAndSalesSettings
                {
                    Id =-5,
                    ArabicName = purAndSalesSettingNames.DiscountPurAR,
                    LatinName = purAndSalesSettingNames.DiscountPurEN,
                    MainType = (int)MainInvoiceType.Purchases,
                    RecieptsType = (int)DocumentType.AcquiredDiscount,
                    ReceiptElemntID = 1,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.EarnedDiscount,
                    branchId = 1,
                },
                new GLPurchasesAndSalesSettings
                {
                    Id =-6,
                    ArabicName = purAndSalesSettingNames.VATpurchasesAR,
                    LatinName = purAndSalesSettingNames.VATpurchasesEN,
                    MainType = (int)MainInvoiceType.Purchases,
                    RecieptsType = (int)DocumentType.VATPurchase,
                    ReceiptElemntID = 1,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.ValueAddedTaxPurchases,
                    branchId = 1,
                },
                #endregion


                #region Sales
                new GLPurchasesAndSalesSettings
                {
                    Id =-7,
                    ArabicName = purAndSalesSettingNames.SalesAR,
                    LatinName = purAndSalesSettingNames.SalesEN,
                    MainType = (int)MainInvoiceType.Sales,
                    RecieptsType = (int)DocumentType.Sales,
                    ReceiptElemntID = 1,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.Sales,
                    branchId = 1,
                },
                new GLPurchasesAndSalesSettings
                {
                    Id =-8,
                    ArabicName = purAndSalesSettingNames.SalesReturnAR,
                    LatinName = purAndSalesSettingNames.SalesReturnEN,
                    MainType = (int)MainInvoiceType.Sales,
                    RecieptsType = (int)DocumentType.ReturnSales,
                    ReceiptElemntID = 1,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.SalesReturns,
                    branchId = 1,
                },
                new GLPurchasesAndSalesSettings
                {
                    Id =-9,
                    ArabicName = purAndSalesSettingNames.POSAR,
                    LatinName = purAndSalesSettingNames.POSEN,
                    MainType = (int)MainInvoiceType.Sales,
                    RecieptsType = (int)DocumentType.POS,
                    ReceiptElemntID = 1,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.Sales,
                    branchId = 1,
                },
                new GLPurchasesAndSalesSettings
                {
                    Id =-10,
                    ArabicName= purAndSalesSettingNames.POSreturnAR,
                    LatinName = purAndSalesSettingNames.POSreturnEN,
                    MainType = (int)MainInvoiceType.Sales,
                    RecieptsType = (int)DocumentType.ReturnPOS,
                    ReceiptElemntID = 1,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.SalesReturns,
                    branchId = 1,
                },
                new GLPurchasesAndSalesSettings
                {
                    Id =-11,
                    ArabicName = purAndSalesSettingNames.VATsalesAR,
                    LatinName = purAndSalesSettingNames.VATsalesEN,
                    MainType = (int)MainInvoiceType.Sales,
                    RecieptsType = (int)DocumentType.VATSale,
                    ReceiptElemntID = 1,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.ValueAddedTaxSales,
                    branchId = 1,
                },
                new GLPurchasesAndSalesSettings
                {
                    Id =-12,
                    ArabicName = purAndSalesSettingNames.DiscountSalesAR,
                    LatinName = purAndSalesSettingNames.DiscountSalesEN,
                    MainType = (int)MainInvoiceType.Sales,
                    RecieptsType = (int)DocumentType.PermittedDiscount,
                    ReceiptElemntID = 1,
                    FinancialAccountId= (int)FinanicalAccountDefultIds.DiscountPermitted,
                    branchId = 1,
                },
                    new GLPurchasesAndSalesSettings
                {
                    Id =-209,
                    ArabicName = purAndSalesSettingNames.SalesCostAr,
                    LatinName = purAndSalesSettingNames.SalesCostEn,
                    MainType = (int)MainInvoiceType.Sales,
                    RecieptsType = (int)DocumentType.Inventory,
                    ReceiptElemntID = 1,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.CostOfGoodsSold,
                    branchId = 1,
                },
                #endregion


                #region Permissions
                //AddPermission
                new GLPurchasesAndSalesSettings//credit
                {
                    Id =-13,
                    ArabicName = purAndSalesSettingNames.SettlementsAddPermissionAr,
                    LatinName = purAndSalesSettingNames.SettlementsAddPermissionEn,
                    MainType = (int)MainInvoiceType.Settlements,
                    RecieptsType = (int)DocumentType.AddPermission,
                    ReceiptElemntID =  (int)DebitoAndCredito.creditor,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.OtherCreditBalances,
                    branchId = 1,
                },
          

                //ExtractPermission
                new GLPurchasesAndSalesSettings//credit
                {
                    Id =-14,
                    ArabicName = purAndSalesSettingNames.SettlementsExtractPermissionAr,
                    LatinName = purAndSalesSettingNames.SettlementsExtractPermissionEn,
                    MainType = (int)MainInvoiceType.Settlements,
                    RecieptsType = (int)DocumentType.ExtractPermission,
                    ReceiptElemntID =  (int)DebitoAndCredito.creditor,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.InventoryMainStore,
                    branchId = 1,
                },






                new GLPurchasesAndSalesSettings
                {
                    Id =-15,
                    ArabicName = purAndSalesSettingNames.SettlementsstockBalanceExcessiveAr,
                    LatinName = purAndSalesSettingNames.SettlementsstockBalanceExcessiveEn,
                    MainType = (int)MainInvoiceType.Settlements,
                    RecieptsType = (int)DocumentType.stockBalanceExcessive,
                    ReceiptElemntID = 1,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.InventoryMainStore,
                    branchId = 1,
                },
                new GLPurchasesAndSalesSettings
                {
                    Id =-16,
                    ArabicName = purAndSalesSettingNames.SettlementsStockBalanceDeficitAr,
                    LatinName = purAndSalesSettingNames.SettlementsStockBalanceDeficitEn,
                    MainType = (int)MainInvoiceType.Settlements,
                    RecieptsType = (int)DocumentType.stockBalanceDeficit,
                    ReceiptElemntID = 1,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.InventoryMainStore,
                    branchId = 1,
                },
                #endregion

                #region Funds
                new GLPurchasesAndSalesSettings
                {
                    Id =-17,
                    ArabicName = purAndSalesSettingNames.FundsSafesAr,
                    LatinName = purAndSalesSettingNames.FundsSafesEn,
                    MainType = (int)MainInvoiceType.funds,
                    RecieptsType = (int)DocumentType.SafeFunds,
                    ReceiptElemntID = 1,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.OtherCreditBalances,
                    branchId = 1,
                },
                new GLPurchasesAndSalesSettings
                {
                    Id =-18,
                    ArabicName = purAndSalesSettingNames.FundsBanksAr,
                    LatinName = purAndSalesSettingNames.FundsBanksEn,
                    MainType = (int)MainInvoiceType.funds,
                    RecieptsType = (int)DocumentType.BankFunds,
                    ReceiptElemntID = 1,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.OtherCreditBalances,
                    branchId = 1,
                },
                new GLPurchasesAndSalesSettings
                {
                    Id =-19,
                     ArabicName = purAndSalesSettingNames.FundsCustomersAr,
                    LatinName = purAndSalesSettingNames.FundsCustomersEn,
                    MainType = (int)MainInvoiceType.funds,
                    RecieptsType = (int)DocumentType.CustomerFunds,
                    ReceiptElemntID = 1,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.OtherCreditBalances,
                    branchId = 1,
                },
                new GLPurchasesAndSalesSettings
                {
                    Id =-20,
                    ArabicName = purAndSalesSettingNames.FundsSuppliersAr,
                    LatinName = purAndSalesSettingNames.FundsSuppliersEn,
                    MainType = (int)MainInvoiceType.funds,
                    RecieptsType = (int)DocumentType.SuplierFunds,
                    ReceiptElemntID = 1,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.theOtherDebitBalances,
                    branchId = 1,
                },
                new GLPurchasesAndSalesSettings
                {
                    Id =-21,
                    ArabicName = purAndSalesSettingNames.DebitFundsItemsAr,
                    LatinName = purAndSalesSettingNames.DebitFundsItemsEn,
                    MainType = (int)MainInvoiceType.funds,
                    RecieptsType = (int)DocumentType.itemsFund,
                    ReceiptElemntID = (int)DebitoAndCredito.debit,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.InventoryMainStore,
                    branchId = 1
                },
                new GLPurchasesAndSalesSettings
                {
                    Id =-22,
                    ArabicName = purAndSalesSettingNames.CreditorFundsItemsAr,
                    LatinName = purAndSalesSettingNames.CreditorFundsItemsEn,
                    MainType = (int)MainInvoiceType.funds,
                    RecieptsType = (int)DocumentType.itemsFund,
                    ReceiptElemntID = (int)DebitoAndCredito.creditor,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.OtherCreditBalances,
                    branchId = 1
                },
                #endregion


                #region new permissions 
                //Add
                new GLPurchasesAndSalesSettings//debit
                {
                    Id =-23,
                    ArabicName = purAndSalesSettingNames.SettlementsAddPermissionAr,
                    LatinName = purAndSalesSettingNames.SettlementsAddPermissionEn,
                    MainType = (int)MainInvoiceType.Settlements,
                    RecieptsType = (int)DocumentType.AddPermission,
                    ReceiptElemntID =  (int)DebitoAndCredito.debit,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.InventoryMainStore,
                    branchId = 1,

                },
                //Extract
                new GLPurchasesAndSalesSettings//debit
                {
                    Id =-24,
                    ArabicName = purAndSalesSettingNames.SettlementsExtractPermissionAr,
                    LatinName = purAndSalesSettingNames.SettlementsExtractPermissionEn,
                    MainType = (int)MainInvoiceType.Settlements,
                    RecieptsType = (int)DocumentType.ExtractPermission,
                    ReceiptElemntID = (int)DebitoAndCredito.debit,
                    FinancialAccountId = (int)FinanicalAccountDefultIds.theOtherDebitBalances,
                    branchId = 1,

                },
                #endregion
            });
            return list;
        }

    }





}
 