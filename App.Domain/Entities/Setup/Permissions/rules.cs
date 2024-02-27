using App.Domain.Models.Security.Authentication.Response.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities
{
    //Note
    /*
     * To Add permission prop start the prop name with flag fot the apps
     * example:
     *          store = store_ <<<--- the flag
     *          next the permission name
     *          set rule prop defult value as false
     */
    public class rules
    {
        public int Id { get; set; }
        public int mainFormCode { get; set; }
        public int subFormCode { get; set; }
        public int applicationId { get; set; }
        public bool isVisible { get; set; } = false;
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public int permissionListId { get; set; }
        public bool isShow { get; set; }
        public bool isAdd { get; set; }
        public bool isEdit { get; set; }
        public bool isDelete { get; set; }
        public bool isPrint { get; set; }
        public DateTime UTime { get; set; }
        public permissionList permissionList { get; set; }
    }
    public class MainForms
    {
        public int Id { get; set; }
        public int appId { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        
    }
    public enum applicationIds
    {
        Genral = 0,
        store = 1,
        GeneralLedger = 2,
        Restaurant = 3
    }
    public enum MainFormsIds
    {
        ItemsFund = 1,
        MainData = 2,
        Repository = 3,
        Purchases = 4,
        Sales = 5,
        pos =6,
        safes = 7,
        banks = 8,
        Settelments = 9,
        GeneralLedgers = 10,
        Users = 11,
        Settings = 12,
        Closing=13,
        Restaurants = 14,
    }
    public enum InvoiceClosing
    {
        ClosingInvoicesPeriod=2,
        ClosingReturnsPeriod = 3,
        ClosingPaymentsAndReceipts = 4,
    }
    public class ClosingScreens
    {
        public int ScreenId { get; set; }
        public int ClosingStep { get; set; }
        public string FileName { get; set; }
    }
    public static class ReturnClosingScreens
    {
        public static List<ClosingScreens> ClosingPrintFilesScreens()
        {
            List<ClosingScreens> ListNames = new List<ClosingScreens>();

            #region Closing Purchases
            ListNames.Add(new ClosingScreens()
            {
                ScreenId= 47,
                ClosingStep=2,
                FileName= "ClosingInvoivesPeroidPurchases"
            });
            ListNames.Add(new ClosingScreens()
            {
                ScreenId = 47,
                ClosingStep = 3,
                FileName = "ClosingReturnPeroidPurchases"
            });
            ListNames.Add(new ClosingScreens()
            {
                ScreenId = 47,
                ClosingStep = 4,
                FileName = "ClosingPaymentsAndReceiptsPurchases"
            });
            #endregion
            #region Closing Sales
            ListNames.Add(new ClosingScreens()
            {
                ScreenId = 60,
                ClosingStep = 2,
                FileName = "ClosingInvoivesPeroidSales"
            });
            ListNames.Add(new ClosingScreens()
            {
                ScreenId = 60,
                ClosingStep = 3,
                FileName = "ClosingReturnPeroidSales"
            });
            ListNames.Add(new ClosingScreens()
            {
                ScreenId = 60,
                ClosingStep = 4,
                FileName = "ClosingPaymentsAndReceiptsSales"
            });
            #endregion
            #region Closing POS
            ListNames.Add(new ClosingScreens()
            {
                ScreenId = 71,
                ClosingStep = 2,
                FileName = "ClosingInvoivesPeroidPOS"
            });
            ListNames.Add(new ClosingScreens()
            {
                ScreenId = 71,
                ClosingStep = 3,
                FileName = "ClosingReturnPeroidPOS"
            });
            ListNames.Add(new ClosingScreens()
            {
                ScreenId = 71,
                ClosingStep = 4,
                FileName = "ClosingPaymentsAndReceiptsPOS"
            });
            #endregion
            return ListNames;
        }
    }
    public enum SubFormsIds
    {
        //Funds
        items_Fund = 1, //ارصدة اول المده اصناف
        Safes_Fund = 2, // ارصدة اول المدة خزائن
        Banks_Fund = 3,  //ارصدة اول المدة بنوك
        Suppliers_Fund = 4,  //ارصدة اول المدة موردين
        Customres_Fund = 5,  //ارصدة اول المودة عملاء
        //Funds

        //MainData
        Branches_MainData = 6,  //الفروع
        Currencies_MainData = 7,  //العملات
        Safes_MainData = 8,  //البنوك    
        Banks_MainData = 9,  //بنوك
        OtherAuthorities_MainData = 10,  //جهات صرف اخري
        //MainData

        //GL
        CalculationGuide_GL = 11,  //شجره الحسابات
        OpeningBalance_GL = 12,  //الارصدة الافتتاحية
        AccountingEntries_GL = 13,  //القيود
        CostCenter_GL = 14,  //مراكذ التكلفة

        PayReceiptForSafe = 15,  //سند صرف خزينة
        PayReceiptForBank = 16,  //سند صرف بنك
        CashReceiptForSafe = 17,  //سند قبض خزائن
        CashReceiptForBank = 18,  //سند قبض بنك


        IncomeList_GL = 19,  //قائمة الدخل  - تقارير 
        DetailedTrialBalance_GL = 20,  //ميزان المرجعه التفصيلي  - تقارير 
        PublicBudget_GL = 21,  //الميزانية العمومية  - تقارير 
        LedgerReport_GL = 22,  //دفتر الاستاذ  - تقارير 
        CostCenterReport_GL = 23,  //مراكذ التكفلة  - تقارير 
        AccountStatementDetail_GL = 24,  //كشف حساب تفصيلي - تقارير

        GeneralLedgersSettings_GL = 25,  //اعدادات العامة
        //GL


        //MainUnits
        Units_MainUnits = 26,  //الوحدات
        Sizes_MainUnits = 27,  //الاحجام
        Color_MainUnits = 28,  //الالوان
        Categories_MainUnits = 29,  //مجموعات الاصناف
        ItemCard_MainUnits = 30,  //كارت الصنف
        Employees_MainUnits = 31,  //الموظفين
        Jobs_MainUnits = 32,  //الوظائف
        Stores_MainUnits = 33,  //المستودعات
        StorePlaces_MainUnits = 34,  //اماكن التخزين
        //MainUnits


        //Repository
        AddPermission_Repository = 35,  //اذن اضافة
        Barcode_Repository = 36,  //الباركود

        DetailedMovementOfAnItem_Repository = 37,  //الحركة التفصيلية لصنف  - تقارير
        ItemBalanceInStore_Repository = 38,  //رصيد صنف في المخازن  - تقارير
        TotalItemBalancesInStore_Repository = 39,  //ارصدة اصناف في المخازن  - تقارير
        InventoryValuation_Repository = 40,  //تقييم المخزون  - تقارير
        DetailedTransactoinsOfItem_Repository = 41,  //  - تقارير
        getItemBalanceInStores_Repository = 42,  //  - تقارير
        getTotalTransactionsOfItems_Repository = 43,  //اجمالي حركة صنف في المخازن  - تقارير
        //Repository


        //Purchases
        Suppliers_Purchases = 44,  //الموردين    
        Purchases = 45,  //المشتريات
        PurchasesReturn_Purchases = 46,  //مرتجع المشتريات
        PruchasesClosing_Purchases = 47,  //اقفال المشتريات
        purchasesTransaction_Purchases = 48,  //حركة المشتريات

        SupplierStatement_Purchases = 49,  //كشف حساب مورد - تقارير
        GetVatStatmentData_Purchases = 50,  //كشف حساب القيمة المضافة - تقارير
        GetSuppliersAccountData_Purchases = 51,  //بيانات الموردين - تقارير
        //VatDetailedReport_Purchases = 52,  //كشف حساب القمية المضافة تفصيلي - تقارير
        supplierItemsPurchased_Purchases = 53,  //مشتريات صنف من مورد - تقارير
        itemPurchases_Purchases = 54,  //مشتريات صنف - تقارير
        ItemsPurchases_Purchases = 55,  //مشتريات اصناف 
        //Purchases


        //Sales
        Customers_Sales = 56,  //العملاء
        CommissionList_Sales = 57,  //لائحة العمولات
        Salesmen_Sales = 58,  //مناديب البيع
        Sales = 59,  //المبيعات
        SalesClosing_Sales = 60,  //اقفال المبيعات
        CustomerStatement_Sales = 61,  //كشف حساب عميل - تقرير
        SalesReturn_Sales = 62,  //مرتجع المبيعات
        //Sales


        //Settelments
        EarnedDiscount_Settelments = 63,  //خصم مكتسب
        PermittedDiscount_Settelments = 64,  // خصم مسموح بة
        //Settelments



        Permission_Users = 65,  //الصلاحيات
        Users = 66,  //المستخدمين

        GeneralSettings_Settings = 67,  //الاعدادات العامة
        StoreSettings_Settings = 68,  //اعدادات المخازن
        CompanyInformation_Settings = 69,  //بيانات الشركة







        pay_permission   = 70,  //اذن صرف
        POSClosing_Sales = 71,  //نقاط البيع اقفال 
        POS = 72,  //نقاط البيع
        returnPOS = 73,  //مرتجع نقاط البيع

        ItemSales = 74,  //مبيعات اصناف
        //detailedTransactionOfItem = 75,  //الحركة التفصيلية لصنف
        salesOfCasher = 76,  //مبيعات الكاشير
        salesOfCasherNotAccredit = 77,  //مبيعات الكاشير غير معتمدة
        salesOfAnItem = 78,  //مبيعات صنف
        supplierData = 79,  //بيانات الموردين
        VatDetailedStatement = 80,  //كشف حساب الضريبة التفصيلي
        itemsTransaction = 81,  //الحركة التفصيلية لصنف
        SalesTransaction = 82,  //حركه المبيعات
        SafeAccountStatement = 83,  //كشف حساب خزنه 
        BankAccountStatement = 84,  //كشف حساب بنك 
        SafeReceipts = 85, //مقبوضات خزينة
        BankReceipts = 86, //مقبوضات بنك
        SafeExpenses = 87, //مصروفات خزنة
        BankExpenses = 88, //مصروفات بنك
        CustomersData = 89, //بيانات العملاء
        CustomersBalances = 90, //ارصدة العملاء
        itemSalesForCustomer = 91, //مبيعات صنف لعيمل
        itemsSalesForCustomer = 92, //مبيعات اصناف لعميل
        itemSalesForCustomers = 93, //مبيعات صنف لعملاء
        ItemNotSold = 94, //اصناف لم تبع
        SalesAndReturnSalesTransaction = 95, //حركة المبيعات و المرتجعات
        DetailsOfSerialTransactions = 96, //الحركة التفصيلية لسيريال
        DemandLimit = 97, //حد الطلب
        TotalSalesOfBranch = 98, //اجمالي مبيعات الفرع
        itemsSoldMost = 99, //الاصناف الاكثر مبيعا
        itemsPrices = 100, //الاصناف الاكثر مبيعا
        GetDetailsOfExpiredItems = 101, //إنتهاء الصلاحية
        PaymentsAndDisbursements = 102, //جهات قبض و صرف
        totalBranchTransaction = 103, //اجمالي حركات الفرع
        IncomingTransfer = 104,  //تحويل وارد
        OutgoingTransfer = 105,  //تحويل صادر
        ReviewWarehouseTransfersReport = 106, //مراجعة تحويلات المخازن
        DetailedTransferReport = 107, //تقرير تحويلات تفصيلي
        paymentMethods = 108, //طرق الدفع
        purchasesTransactionOfBranch = 109, //حركه المشتريات
        offerPrice_Sales = 110,//عرض السعر 
        SupplierItemsPurchases_Purchases = 111,  //مشتريات اصناف من مورد - تقارير
        SalesBranchProfit = 112,// ربحيه مبيعات الفرع
        GetCompanySubscriptionInformation = 113,// بانات الاشتراك
        PurchaseOrder_Purchases=114,// أمر شراء
        POSSession = 115,// جلسات نقاط البيع
        TotalAccountBalance = 116,// اجمالى رصيد الحساب
        BalanceReviewFunds = 117,//  ميزان المراجعة ارصدة
        costCenterForAccount = 118,// مراكز التكلفه لحساب
        SalesOfSalesMan=119, //  مبيعات مندوب المبيعات
        SalesProfitOfItems = 120 ,// ربحية مبيعات الاصناف
        Additions = 121, // اضافات لفواتير الشراء
        PurchasesWithoutVat=122,  // مشتريات بدون ضريبه
        ReturnPurchasesWithoutVat = 123,  //مرتجع مشتريات بدون ضريبه
        GetTotalVatData = 124, //إجمالي الضريبة 
        DebtAgingForCustomers = 125, // أعمار الديون لفواتير العملاء
        DebtAgingForSupplier = 126, // أعمار الديون لفواتير الموردين
        Printers = 127,             //شاشه الطابعات
        Kitchens = 128 ,          // شاشه المطابخ
        CollectionReceipts = 129,              // سند تحصيل
        userActions = 130,              //حركات المستخدمين
        offerPriceReport = 131,          //تقرير عروض الاسعار
        transferFromBanks =140 , // تحويل من البنوك
        transferFromSafes= 141, // تحويل من الخزائن
        DetailedInvoices=142, // تقرير تفصيلى الفواتير 
        SafeMultiCollectionReceipt = 143, //سند مجمع خزائن
        BankMultiCollectionReceipt = 144 //سند مجمع بنوك
    }
    public static class returnSubForms
    {
        public static List<rules> returnRules(bool defult = false,int permissionId = 0)
        {
            var list = new List<rules>();
            #region Items Fund
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.ItemsFund,
                subFormCode = (int)SubFormsIds.items_Fund,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "الاصناف",
                latinName = "items",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 
            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.ItemsFund,
                subFormCode = (int)SubFormsIds.Safes_Fund,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "الخزائن",
                latinName = "Safes",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.ItemsFund,
                subFormCode = (int)SubFormsIds.Banks_Fund,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "البنوك",
                latinName = "Banks",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 


            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.ItemsFund,
                subFormCode = (int)SubFormsIds.Suppliers_Fund,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "الموردين",
                latinName = "Suppliers",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()    
            {
                mainFormCode = (int)MainFormsIds.ItemsFund,
                subFormCode = (int)SubFormsIds.Customres_Fund,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "العملاء",
                latinName = "Customres",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            #endregion
            #region Main Data
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.MainData,
                subFormCode = (int)SubFormsIds.Units_MainUnits,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "الوحدات",
                latinName = "Units",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.MainData,
                subFormCode = (int)SubFormsIds.Categories_MainUnits,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "مجموعات الاصناف",
                latinName = "Categories",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });

            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.MainData,
                subFormCode = (int)SubFormsIds.ItemCard_MainUnits,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "كارت الصنف",
                latinName = "Item Card",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.MainData,
                subFormCode = (int)SubFormsIds.Employees_MainUnits,
                applicationId = (int)applicationIds.Genral,
                isVisible = true,
                arabicName = "الموظفين",
                latinName = "Employees",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.MainData,
                subFormCode = (int)SubFormsIds.Jobs_MainUnits,
                applicationId = (int)applicationIds.Genral,
                isVisible = true,
                arabicName = "الوظائف",
                latinName = "Vacancies",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.MainData,
                subFormCode = (int)SubFormsIds.Stores_MainUnits,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "المستودعات",
                latinName = "Stores",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.MainData,
                subFormCode = (int)SubFormsIds.StorePlaces_MainUnits,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "أماكن التخزين",
                latinName = "Store places",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.MainData,
                subFormCode = (int)SubFormsIds.Currencies_MainData,
                applicationId = (int)applicationIds.Genral,
                isVisible = true,
                arabicName = "العملات",
                latinName = "Currencies",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 


            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.MainData,
                subFormCode = (int)SubFormsIds.Branches_MainData,
                applicationId = (int)applicationIds.Genral,
                isVisible = true,
                arabicName = "الفروع",
                latinName = "Branches",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 


            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.MainData,
                subFormCode = (int)SubFormsIds.Sizes_MainUnits,
                applicationId = (int)applicationIds.Genral,
                isVisible = false,
                arabicName = "الاحجام",
                latinName = "Sizes",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.MainData,
                subFormCode = (int)SubFormsIds.Color_MainUnits,
                applicationId = (int)applicationIds.Genral,
                isVisible = false,
                arabicName = "اللون",
                latinName = "Color",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.MainData,
                subFormCode = (int)SubFormsIds.paymentMethods,
                applicationId = (int)applicationIds.Genral,
                isVisible = true,
                arabicName = "طرق السداد",
                latinName = "Payment Methods",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.MainData,
                subFormCode = (int)SubFormsIds.Printers,
                applicationId = (int)applicationIds.Restaurant,
                isVisible = false,
                arabicName = "طابعات",
                latinName = "Printers",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
            });

            //Reports
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.MainData,
                subFormCode = (int)SubFormsIds.itemsPrices,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "اسعار الاصناف",
                latinName = "Items Prices",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            #endregion
            #region Repository
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Repository,
                subFormCode = (int)SubFormsIds.AddPermission_Repository,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "اذن اضافة ",
                latinName = "Add Permission",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Repository,
                subFormCode = (int)SubFormsIds.pay_permission,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "اذن صرف ",
                latinName = "Pay Permission",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Repository,
                subFormCode = (int)SubFormsIds.Barcode_Repository,
                applicationId = (int)applicationIds.store,
                isVisible = false,
                arabicName = "الباركود",
                latinName = "Barcode",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Repository,
                subFormCode = (int)SubFormsIds.IncomingTransfer,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "تحويل وارد",
                latinName = "Incoming Transfer",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Repository,
                subFormCode = (int)SubFormsIds.OutgoingTransfer,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "تحويل صادر",
                latinName = "Outgoing Transfer",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            //Reports
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Repository,
                subFormCode = (int)SubFormsIds.DetailedMovementOfAnItem_Repository,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "الحركة التفصيلية لصنف",
                latinName = "Detailed Movement Of an Item",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Repository,
                subFormCode = (int)SubFormsIds.ItemBalanceInStore_Repository,
                applicationId = (int)applicationIds.store,
                isVisible = false,
                arabicName = "رصيد صنف في المخازن",
                latinName = "Item Balance in Store",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Repository,
                subFormCode = (int)SubFormsIds.InventoryValuation_Repository,
                applicationId = (int)applicationIds.store,
                isVisible = true    ,
                arabicName = "تقيم المخزون",
                latinName = "Inventory Valuation",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Repository,
                subFormCode = (int)SubFormsIds.getTotalTransactionsOfItems_Repository,
                applicationId = (int)applicationIds.store,
                isVisible = false,
                arabicName = "اجمالي حركة الصنف",
                latinName = "get Total Transactions Of Items",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Repository,
                subFormCode = (int)SubFormsIds.itemsTransaction,
                applicationId = (int)applicationIds.store,
                isVisible = false,
                arabicName = "حركة الاصناف",
                latinName = "Items Transaction",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Repository,
                subFormCode = (int)SubFormsIds.DetailsOfSerialTransactions,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "الحركة التفصيلية لسيريال",
                latinName = "Details Of Serial Transactions",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Repository,
                subFormCode = (int)SubFormsIds.DemandLimit,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "حد الطلب",
                latinName = "Demand Limit",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Repository,
                subFormCode = (int)SubFormsIds.TotalItemBalancesInStore_Repository,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "ارصدة اصناف في المخازن",
                latinName = "Total Item Balances In Store",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Repository,
                subFormCode = (int)SubFormsIds.GetDetailsOfExpiredItems,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "إنتهاء الصلاحية",
                latinName = "Details O fExpired Itemse",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Repository,
                subFormCode = (int)SubFormsIds.ReviewWarehouseTransfersReport,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "مراجعة تحويلات المخازن",
                latinName = "Review Warehouse Transfers Report",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Repository,
                subFormCode = (int)SubFormsIds.DetailedTransferReport,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "تقرير تحويلات تفصيلي",
                latinName = "Detailed Transfer Report",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            #endregion
            #region Purchases
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Purchases,
                subFormCode = (int)SubFormsIds.Suppliers_Purchases,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "الموردين",
                latinName = "Suppliers",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Purchases,
                subFormCode = (int)SubFormsIds.Purchases,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "المشتريات",
                latinName = "Purchases",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Purchases,
                subFormCode = (int)SubFormsIds.PurchasesReturn_Purchases,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "مرتجع المشتريات",
                latinName = "Purchases Return",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Purchases,
                subFormCode = (int)SubFormsIds.PruchasesClosing_Purchases,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "اقفال المشتريات",
                latinName = "Pruchases Closing",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            //Reports
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Purchases,
                subFormCode = (int)SubFormsIds.SupplierStatement_Purchases,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "كشف حساب مورد",
                latinName = "Supplier statement",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Purchases,
                subFormCode = (int)SubFormsIds.purchasesTransaction_Purchases,
                applicationId = (int)applicationIds.store,
                isVisible = false,
                arabicName = "حركة المشتريات",
                latinName = "purchases-transaction",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Purchases,
                subFormCode = (int)SubFormsIds.GetVatStatmentData_Purchases,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "تقرير ضريبة القيمة المضافة",
                latinName = "VAT Statment",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Purchases,
                subFormCode = (int)SubFormsIds.VatDetailedStatement,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "كشف حساب الضريبة التفصيلي",
                latinName = "Vat Detailed Statement",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Purchases,
                subFormCode = (int)SubFormsIds.GetSuppliersAccountData_Purchases,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "ارصدة الموردين",
                latinName = "Suppliers Account",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Purchases,
                subFormCode = (int)SubFormsIds.supplierItemsPurchased_Purchases,
                applicationId = (int)applicationIds.store,
                isVisible = false,
                arabicName = "مشتريات صنف من مورد",
                latinName = "Supplier Items Purchased",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Purchases,
                subFormCode = (int)SubFormsIds.itemPurchases_Purchases,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "مشتريات اصناف",
                latinName = "Item purchases",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Purchases,
                subFormCode = (int)SubFormsIds.SupplierItemsPurchases_Purchases,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "مشتريات اصناف من مورد",
                latinName = "Supplier Items purchases",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Purchases,
                subFormCode = (int)SubFormsIds.ItemsPurchases_Purchases,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "مشتريات اصناف",
                latinName = "Items purchases",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Purchases,
                subFormCode = (int)SubFormsIds.GetTotalVatData,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "اجمالى الضريبه",
                latinName = "Total Vat",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,



            }); 
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.SalesBranchProfit,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "ربحيه مبيعات الفرع",
                latinName = "branch sales Profit",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
           
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Purchases,
                subFormCode = (int)SubFormsIds.purchasesTransactionOfBranch,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "حركة مشتريات الفرع",
                latinName = "Branch Purcahses Transaction",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Purchases,
                subFormCode = (int)SubFormsIds.PurchaseOrder_Purchases,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "أمر شراء",
                latinName = "Purchase order",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Purchases,
                subFormCode = (int)SubFormsIds.Additions,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "اضافات",
                latinName = "Additions",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Purchases,
                subFormCode = (int)SubFormsIds.PurchasesWithoutVat,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "مشتريات بدون ضريبة",
                latinName = "Purchases without vat",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });

            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Purchases,
                subFormCode = (int)SubFormsIds.ReturnPurchasesWithoutVat,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "مرتجع مشتريات بدون ضريبة",
                latinName = "Return purchases without vat",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Purchases,
                subFormCode = (int)SubFormsIds.DebtAgingForSupplier,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "أعمار الديون لفواتيرالموردين",
                latinName = "DebtAgingForSuppliers",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,



            });
            #endregion
            #region Sales
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.Customers_Sales,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "العملاء",
                latinName = "Customers",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Purchases,
                subFormCode = (int)SubFormsIds.supplierData,
                applicationId = (int)applicationIds.store,
                isVisible = false,
                arabicName = "بيانات الموردين",
                latinName = "Supplier Data ",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.Salesmen_Sales,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "مندوبي المبيعات",
                latinName = "Salesmen",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.CommissionList_Sales,
                applicationId = (int)applicationIds.store,
                isVisible = false,
                arabicName = "لائحة العمولات",
                latinName = "Commission List",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.Sales,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "مبيعات",
                latinName = "Sales",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.SalesReturn_Sales,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "مرتجع مبيعات",
                latinName = "Sales Return",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.offerPrice_Sales,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "عرض السعر",
                latinName = "Offer Price",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.SalesClosing_Sales,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "اقفال المبيعات",
                latinName = "Sales Closing",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            //Reports 
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.CustomerStatement_Sales,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "كشف حساب عميل",
                latinName = "Customer Statement",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.ItemSales,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "مبيعات اصناف",
                latinName = "Items Sales",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.salesOfAnItem,
                applicationId = (int)applicationIds.store,
                isVisible = false,
                arabicName = "مبيعات صنف",
                latinName = "Sales Of an Item",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });

            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.SalesTransaction,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "حركة المبيعات",
                latinName = "Sales Transaction",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.CustomersData,
                applicationId = (int)applicationIds.store,
                isVisible = false,
                arabicName = "بيانات العملاء",
                latinName = "Customers Data",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.CustomersBalances,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "ارصدة العملاء",
                latinName = "Customers Balances",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.itemSalesForCustomer,
                applicationId = (int)applicationIds.store,
                isVisible = false,
                arabicName = "مبعات صنف لعميل",
                latinName = "Item Sales For Customer",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.itemsSalesForCustomer,
                applicationId = (int)applicationIds.store,
                isVisible = false,
                arabicName = "مبعات اصناف لعميل",
                latinName = "Items Sales For Customer",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.itemSalesForCustomers,
                applicationId = (int)applicationIds.store,
                isVisible = false,
                arabicName = "مبعات صنف لعملاء",
                latinName = "Item Sales For Customers",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.ItemNotSold,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "اصناف لم تبع",
                latinName = "Item Not Sold",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.SalesAndReturnSalesTransaction,
                applicationId = (int)applicationIds.store,
                isVisible = false,
                arabicName = "حركة المبيعات و المرتجعات",
                latinName = "Sales And Return Sales Transaction",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.itemsSoldMost,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "الاصناف الاكثر مبيعاً",
                latinName = "items Sold Most",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.totalBranchTransaction,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "اجمالي حركات الفرع",
                latinName = "Total Branch Transaction",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.TotalSalesOfBranch,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "اجمالي مبيعات الفرع",
                latinName = "Total Sales Of Branch",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.SalesOfSalesMan,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "مبيعات مندوب المبيعات",
                latinName = "Sales Of SalesMan",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.SalesProfitOfItems,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "ربحية مبيعات الاصناف",
                latinName = "Sales Profit Of Items",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,

                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.DebtAgingForCustomers,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "أعمار الديون لفواتير العملاء",
                latinName = "DebtAgingForCustomer",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,



            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.offerPriceReport,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "تقرير عروض الاسعار",
                latinName = "Offer Prices Report",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,



            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Sales,
                subFormCode = (int)SubFormsIds.DetailedInvoices,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "تقرير تفصيلى الفواتير",
                latinName = "Detialed Invoices report",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,



            });
            #endregion
            #region POS
            list.Add(new rules()
        {
            mainFormCode = (int)MainFormsIds.pos,
            subFormCode = (int)SubFormsIds.POS,
            applicationId = (int)applicationIds.store,
            isVisible = true,
            arabicName = "نقاط البيع",
            latinName = "Point Of Sales",

            permissionListId = permissionId,
            isAdd = defult,
            isDelete = defult,
            isEdit = defult,
            isPrint = defult,
            isShow = defult,

             

            });
        list.Add(new rules()
        {
            mainFormCode = (int)MainFormsIds.pos,
            subFormCode = (int)SubFormsIds.returnPOS,
            applicationId = (int)applicationIds.store,
            isVisible = true,
            arabicName = "مرتجع نقاط البيع",
            latinName = "Return Point Of Sales",

            permissionListId = permissionId,
            isAdd = defult,
            isDelete = defult,
            isEdit = defult,
            isPrint = defult,
            isShow = defult,

             

        });
        list.Add(new rules()
        {
            mainFormCode = (int)MainFormsIds.pos,
            subFormCode = (int)SubFormsIds.POSClosing_Sales,
            applicationId = (int)applicationIds.store,
            isVisible = true,
            arabicName = "اقفال نقاط البيع",
            latinName = "POS Closing",

            permissionListId = permissionId,
            isAdd = defult,
            isDelete = defult,
            isEdit = defult,
            isPrint = defult,
            isShow = defult,

             

        });
        list.Add(new rules()
        {
            mainFormCode = (int)MainFormsIds.pos,
            subFormCode = (int)SubFormsIds.salesOfCasher,
            applicationId = (int)applicationIds.store,
            isVisible = true,
            arabicName = "ايرادات و مبيعات الكاشير",
            latinName = "Sales Of Casher",

            permissionListId = permissionId,
            isAdd = defult,
            isDelete = defult,
            isEdit = defult,
            isPrint = defult,
            isShow = defult,

             

        });
        list.Add(new rules()
        {
            mainFormCode = (int)MainFormsIds.pos,
            subFormCode = (int)SubFormsIds.salesOfCasherNotAccredit,
            applicationId = (int)applicationIds.store,
            isVisible = true,
            arabicName = "ايرادات و مبيعات الكاشير غير معتمد",
            latinName = "Sales Of Casher not Accredit",

            permissionListId = permissionId,
            isAdd = defult,
            isDelete = defult,
            isEdit = defult,
            isPrint = defult,
            isShow = defult,

             

        });
        list.Add(new rules()
        {
            mainFormCode = (int)MainFormsIds.pos,
            subFormCode = (int)SubFormsIds.POSSession,
            applicationId = (int)applicationIds.store,
            isVisible = true,
            arabicName = "جلسات نقاط البيع",
            latinName = "Point of sales sessions",

            permissionListId = permissionId,
            isAdd = defult,
            isDelete = defult,
            isEdit = defult,
            isPrint = defult,
            isShow = defult,

             

        });
            #endregion
            #region Restaurants
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Restaurants,
                subFormCode = (int)SubFormsIds.Kitchens,
                applicationId = (int)applicationIds.Restaurant,
                isVisible = false,
                arabicName = "مطابخ",
                latinName = "Kitchens",
                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult
            });
            #endregion
            #region safes
            list.Add(new rules()
        {
            mainFormCode = (int)MainFormsIds.safes,
            subFormCode = (int)SubFormsIds.Safes_MainData,
            applicationId = (int)applicationIds.Genral,
            isVisible = true,
            arabicName = "الخزائن",
            latinName = "Safes",

            permissionListId = permissionId,
            isAdd = defult,
            isDelete = defult,
            isEdit = defult,
            isPrint = defult,
            isShow = defult,

             


            });
        list.Add(new rules()
        {
            mainFormCode = (int)MainFormsIds.safes,
            subFormCode = (int)SubFormsIds.OtherAuthorities_MainData,
            applicationId = (int)applicationIds.Genral,
            isVisible = true,
            arabicName = "جهات صرف اخري",
            latinName = "Other Authorities",

            permissionListId = permissionId,
            isAdd = defult,
            isDelete = defult,
            isEdit = defult,
            isPrint = defult,
            isShow = defult,

             

        });
        list.Add(new rules()
        {
            mainFormCode = (int)MainFormsIds.safes,
            subFormCode = (int)SubFormsIds.PayReceiptForSafe,
            applicationId = (int)applicationIds.Genral,
            isVisible = true,
            arabicName = "سند صرف خزينة",
            latinName = "Pay Receipt For Safe",

            permissionListId = permissionId,
            isAdd = defult,
            isDelete = defult,
            isEdit = defult,
            isPrint = defult,
            isShow = defult,

             

        });
        list.Add(new rules()
        {
            mainFormCode = (int)MainFormsIds.safes,
            subFormCode = (int)SubFormsIds.CashReceiptForSafe,
            applicationId = (int)applicationIds.Genral,
            isVisible = true,
            arabicName = "سند قبض خزينة",
            latinName = "Catch Receipt For Safe",

            permissionListId = permissionId,
            isAdd = defult,
            isDelete = defult,
            isEdit = defult,
            isPrint = defult,
            isShow = defult,

             

        });
        list.Add(new rules()
        {
            mainFormCode = (int)MainFormsIds.safes,
            subFormCode = (int)SubFormsIds.SafeMultiCollectionReceipt,
            applicationId = (int)applicationIds.Genral,
            isVisible = true,
            arabicName = "سند مجمع خزائن",
            latinName = "Safe Multi Collection Receipt",

            permissionListId = permissionId,
            isAdd = defult,
            isDelete = defult,
            isEdit = defult,
            isPrint = defult,
            isShow = defult,

             

        });

        list.Add(new rules()
        {
            mainFormCode = (int)MainFormsIds.safes,
            subFormCode = (int)SubFormsIds.SafeAccountStatement,
            applicationId = (int)applicationIds.Genral,
            isVisible = true,
            arabicName = "كشف حساب خزينة",
            latinName = "Safe Account Statement",

            permissionListId = permissionId,
            isAdd = defult,
            isDelete = defult,
            isEdit = defult,
            isPrint = defult,
            isShow = defult,

             

        });
        list.Add(new rules()
        {
            mainFormCode = (int)MainFormsIds.safes,
            subFormCode = (int)SubFormsIds.SafeReceipts,
            applicationId = (int)applicationIds.Genral,
            isVisible = true,
            arabicName = "مقبوضات خزينة",
            latinName = "Safe Receipts",

            permissionListId = permissionId,
            isAdd = defult,
            isDelete = defult,
            isEdit = defult,
            isPrint = defult,
            isShow = defult,

             

        });
        list.Add(new rules()
        {
            mainFormCode = (int)MainFormsIds.safes,
            subFormCode = (int)SubFormsIds.SafeExpenses,
            applicationId = (int)applicationIds.Genral,
            isVisible = true,
            arabicName = "مصروفات خزينة",
            latinName = "Safe Expenses",

            permissionListId = permissionId,
            isAdd = defult,
            isDelete = defult,
            isEdit = defult,
            isPrint = defult,
            isShow = defult,

             

        });
        list.Add(new rules()
        {
            mainFormCode = (int)MainFormsIds.safes,
            subFormCode = (int)SubFormsIds.PaymentsAndDisbursements,
            applicationId = (int)applicationIds.Genral,
            isVisible = true,
            arabicName = "جهات قبض و صرف",
            latinName = "Payments And Disbursements",

            permissionListId = permissionId,
            isAdd = defult,
            isDelete = defult,
            isEdit = defult,
            isPrint = defult,
            isShow = defult,
             

        });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.safes,
                subFormCode = (int)SubFormsIds.transferFromSafes,
                applicationId = (int)applicationIds.Genral,
                isVisible = true,
                arabicName = "تحويل من الخزائن",
                latinName = "Transfer From Safes",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,


            });
            #endregion
            #region banks
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.banks,
                subFormCode = (int)SubFormsIds.Banks_MainData,
                applicationId = (int)applicationIds.Genral,
                isVisible = true,
                arabicName = "البنوك",
                latinName = "Banks",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 


            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.banks,
                subFormCode = (int)SubFormsIds.PayReceiptForBank,
                applicationId = (int)applicationIds.Genral,
                isVisible = true,
                arabicName = "سند صرف بنكي",
                latinName = "Pay Receipt For Bank",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.banks,
                subFormCode = (int)SubFormsIds.CashReceiptForBank,
                applicationId = (int)applicationIds.Genral,
                isVisible = true,
                arabicName = "سند قبض بنكي",
                latinName = "Catch Receipt For Bank",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.banks,
                subFormCode = (int)SubFormsIds.BankMultiCollectionReceipt,
                applicationId = (int)applicationIds.Genral,
                isVisible = true,
                arabicName = "سند مجمع بنك",
                latinName = "Bank Multi Collection Receipt",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.banks,
                subFormCode = (int)SubFormsIds.BankAccountStatement,
                applicationId = (int)applicationIds.Genral,
                isVisible = true,
                arabicName = "كشف حساب بنك",
                latinName = "Bank Account Statement",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.banks,
                subFormCode = (int)SubFormsIds.BankReceipts,
                applicationId = (int)applicationIds.Genral,
                isVisible = true,
                arabicName = "مقبوضات بنك",
                latinName = "Bank Receipts",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.banks,
                subFormCode = (int)SubFormsIds.BankExpenses,
                applicationId = (int)applicationIds.Genral,
                isVisible = true,
                arabicName = "مصروفات بنك",
                latinName = "Bank Expenses",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.banks,
                subFormCode = (int)SubFormsIds.transferFromBanks,
                applicationId = (int)applicationIds.Genral,
                isVisible = true,
                arabicName = "تحويل من البنوك ",
                latinName = "Transfer From Banks",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,


            });
            #endregion
            #region General Ledgers
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.GeneralLedgers,
                subFormCode = (int)SubFormsIds.CalculationGuide_GL,
                applicationId = (int)applicationIds.GeneralLedger,
                isVisible = true,
                arabicName = "الدليل المحاسبي",
                latinName = "Calculation Guide",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.GeneralLedgers,
                subFormCode = (int)SubFormsIds.OpeningBalance_GL,
                applicationId = (int)applicationIds.GeneralLedger,
                isVisible = true,
                arabicName = "الأرصدة الافتتاحية",
                latinName = "Opening Balance",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.GeneralLedgers,
                subFormCode = (int)SubFormsIds.AccountingEntries_GL,
                applicationId = (int)applicationIds.GeneralLedger,
                isVisible = true,
                arabicName = "القيود",
                latinName = "Accounting Entries",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.GeneralLedgers,
                subFormCode = (int)SubFormsIds.CostCenter_GL,
                applicationId = (int)applicationIds.GeneralLedger,
                isVisible = true,
                arabicName = "مراكز التكلفه",
                latinName = "Cost Center",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.GeneralLedgers,
                subFormCode = (int)SubFormsIds.IncomeList_GL,
                applicationId = (int)applicationIds.GeneralLedger,
                isVisible = true,
                arabicName = "قائمة الدخل",
                latinName = "Income List",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.GeneralLedgers,
                subFormCode = (int)SubFormsIds.DetailedTrialBalance_GL,
                applicationId = (int)applicationIds.GeneralLedger,
                isVisible = true,
                arabicName = "ميزان المراجعه تفصيلي",
                latinName = "Detailed Trial Balance",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            //
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.GeneralLedgers,
                subFormCode = (int)SubFormsIds.BalanceReviewFunds,
                applicationId = (int)applicationIds.GeneralLedger,
                isVisible = true,
                arabicName = "ميزان المراجعه ارصده",
                latinName = "Balance Review Funds",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.GeneralLedgers,
                subFormCode = (int)SubFormsIds.TotalAccountBalance,
                applicationId = (int)applicationIds.GeneralLedger,
                isVisible = true,
                arabicName = "اجمالي رصيد الحساب",
                latinName = "Total Balance Review ",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            //
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.GeneralLedgers,
                subFormCode = (int)SubFormsIds.PublicBudget_GL,
                applicationId = (int)applicationIds.GeneralLedger,
                isVisible = true,
                arabicName = "الميزانيه العموميه",
                latinName = "Public Budget",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.GeneralLedgers,
                subFormCode = (int)SubFormsIds.LedgerReport_GL,
                applicationId = (int)applicationIds.GeneralLedger,
                isVisible = true,
                arabicName = "دفتر الاستاذ",
                latinName = "Ledger Report",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.GeneralLedgers,
                subFormCode = (int)SubFormsIds.CostCenterReport_GL,
                applicationId = (int)applicationIds.GeneralLedger,
                isVisible = true,
                arabicName = "تقرير مراكز التكلفه",
                latinName = "Cost Centers Report",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.GeneralLedgers,
                subFormCode = (int)SubFormsIds.costCenterForAccount,
                applicationId = (int)applicationIds.GeneralLedger,
                isVisible = true,
                arabicName = "تقرير مراكز التكلفه لحساب",
                latinName = "Cost Centers for account report",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.GeneralLedgers,
                subFormCode = (int)SubFormsIds.AccountStatementDetail_GL,
                applicationId = (int)applicationIds.GeneralLedger,
                isVisible = true,
                arabicName = "كشف حساب تفصيلي",
                latinName = "Account Statement Detail",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            #endregion
            #region Settelments
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Settelments,
                subFormCode = (int)SubFormsIds.EarnedDiscount_Settelments,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "خصم مكتسب",
                latinName = "Earned Discount",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Settelments,
                subFormCode = (int)SubFormsIds.PermittedDiscount_Settelments,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "خصم مسموح به",
                latinName = "Permitted Discount",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            #endregion
            #region Users
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Users,
                subFormCode = (int)SubFormsIds.Permission_Users,
                applicationId = (int)applicationIds.Genral,
                isVisible = true,
                arabicName = "الصلاحيات",
                latinName = "Permission",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Users,
                subFormCode = (int)SubFormsIds.Users,
                applicationId = (int)applicationIds.Genral,
                isVisible = true,
                arabicName = "المستخدمين",
                latinName = "Users",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Users,
                subFormCode = (int)SubFormsIds.userActions,
                applicationId = (int)applicationIds.Genral,
                isVisible = true,
                arabicName = "حركات المستخدمين",
                latinName = "Users Actions",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,


            });
            #endregion
            #region Settings
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Settings,
                subFormCode = (int)SubFormsIds.GeneralSettings_Settings,
                applicationId = (int)applicationIds.Genral,
                isVisible = true,
                arabicName = "الاعدادات العامة",
                latinName = "General Settings",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Settings,
                subFormCode = (int)SubFormsIds.StoreSettings_Settings,
                applicationId = (int)applicationIds.store,
                isVisible = true,
                arabicName = "اعدادات المخازن",
                latinName = "Store Settings",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Settings,
                subFormCode = (int)SubFormsIds.GeneralLedgersSettings_GL,
                applicationId = (int)applicationIds.GeneralLedger,
                isVisible = true,
                arabicName = "إعدادات الحسابات العامه",
                latinName = "General ledgers settings",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Settings,
                subFormCode = (int)SubFormsIds.CompanyInformation_Settings,
                applicationId = (int)applicationIds.Genral,
                isVisible = true,
                arabicName = "بيانات الشركة",
                latinName = "Company Information",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            list.Add(new rules()
            {
                mainFormCode = (int)MainFormsIds.Settings,
                subFormCode = (int)SubFormsIds.GetCompanySubscriptionInformation,
                applicationId = (int)applicationIds.Genral,
                isVisible = true,
                arabicName = "بيانات الاشتراك",
                latinName = " Subscription Information",

                permissionListId = permissionId,
                isAdd = defult,
                isDelete = defult,
                isEdit = defult,
                isPrint = defult,
                isShow = defult,
                 

            });
            #endregion
        
            return list;
        }
    }
    public static class returnMainForms
    {
        public static List<MainForms> mainForms()
        {
            var list = new List<MainForms>();
            list.Add(new MainForms()
            {
                Id = (int)MainFormsIds.ItemsFund,
                appId = (int)applicationIds.store,
                arabicName = "أرصدة اول مدة",
                latinName = "Items Fund",
            });
            list.Add(new MainForms()
            {
                Id = (int)MainFormsIds.MainData,
                appId = (int)applicationIds.Genral,
                arabicName = "البيانات الاساسية",
                latinName = "Main Data",
            });
            list.Add(new MainForms()
            {
                Id = (int)MainFormsIds.Repository,
                appId = (int)applicationIds.store,
                arabicName = "المستودعات",
                latinName = "Repository",
            });
            list.Add(new MainForms()
            {
                Id = (int)MainFormsIds.Purchases,
                appId = (int)applicationIds.store,
                arabicName = "المشتريات",
                latinName = "Purchases",
            });
            list.Add(new MainForms()
            {
                Id = (int)MainFormsIds.Sales,
                appId = (int)applicationIds.store,
                arabicName = "المبيعات",
                latinName = "Sales",
            });
            list.Add(new MainForms()
            {
                Id = (int)MainFormsIds.pos,
                appId = (int)applicationIds.store,
                arabicName = "نقاط البيع",
                latinName = "Point Of Sales",
            });
            list.Add(new MainForms()
            {
                Id = (int)MainFormsIds.Restaurants,
                appId = (int)applicationIds.Restaurant,
                arabicName = "مطاعم",
                latinName = "Restaurants",
            });
            list.Add(new MainForms()
            {
                Id = (int)MainFormsIds.safes,
                appId = 1,
                arabicName = "الخزائن",
                latinName = "Safes",
            });
            list.Add(new MainForms()
            {
                Id = (int)MainFormsIds.banks,
                appId = (int)applicationIds.Genral,
                arabicName = "البنوك",
                latinName = "Banks",
            });
            list.Add(new MainForms()
            {
                Id = (int)MainFormsIds.Settelments,
                appId = (int)applicationIds.store,
                arabicName = "تسويات",
                latinName = "Settelments",
            });
            list.Add(new MainForms()
            {
                Id = (int)MainFormsIds.GeneralLedgers,
                appId = 2,
                arabicName = "حسابات عامة",
                latinName = "General Ledgers",
            });
            list.Add(new MainForms()
            {
                Id = (int)MainFormsIds.Users,
                appId = (int)applicationIds.Genral,
                arabicName = "المستخدمين",
                latinName = "Users",
            });
            list.Add(new MainForms()
            {
                Id = (int)MainFormsIds.Settings,
                appId = (int)applicationIds.Genral,
                arabicName = "الاعدادات",
                latinName = "Settings",
            });
            return list;
        }
    }
}
