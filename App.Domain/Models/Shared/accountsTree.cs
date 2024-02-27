using App.Domain.Entities.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Shared
{
    public class accountantTree
    {
        public enum FinanicalAccountDefultIds
        {
            /*Level 1*/
            /*1*/
            assets = -1,     //الاصول 
            longTermOpponents = -2,     //الاصول المتداولة
            Cashandequivalent = -102002,    //النقد وما يعادلة
            Cashinthesafe = -3,   //النقدية في الخزينة
            Mainbranchtreasury = -102002001,        //خزينة الفرع الرئيسي
            cashinthebank = -102001,   //النقدية في البنك 
            AlRajhiBank = -102001001,        //بنك الراجحي
            BankAlbilad = -8,     //بنك البلاد 
            RiyadBank = -9,        //بنك الرياض
            Customers = -102003, //العملاء
            MainbranchCustomers = -11,         //عملاء الفرع الرئيسي 
            cashCustomer = -102003001,        //عميل نقدي
            Advancedexpenses = -13, //مصروفات مقدمة
            Advanceshowroomrent = -14,      //ايجار معرض مقدم
            Advancehousingrent = -15,   //ايجار سكن مقدم
            Upfrontwarehouserent = -16,      //ايجار مستودع مقدم
            Providedmedicalinsurance = -17,   //تأمين طبي مقدم
            Advanceexpense = -18,      // مصروف مقدم
            OtherDebitBalances = -102006001,    //أرصدة مدينة اخري
            theOtherDebitBalances = -102006002,    //أرصدة مدينة اخري
            withdrawals = -20,      //المسحوبات
            PartnerwithdrawalsA = -21,     //مسحوبات الشريك ا
            PartnerwithdrawalsB = -22,     //مسحوبات الشريك ب 
            employeeadvances = -23,      //سلف العاملين
            ValueAddedTaxPurchases = -202002001,      //الضريبة القيمه المضافه ( المشتريات )
            Inventory = -25,    //المخزون
            InventoryMainStore = -501002,    //المخزون المخزن الرئيسي
            noncurrentassets = -27,  //الاصول غير المتداولة
            FixedAssets = -28,    //الاصول الثايته
            cars = -29,      //السيارات 
            MainBranchCars = -30,        //سيارات  الفرع الرئيسي       
            MachineryAndEquipment = -31,      //الات والمعدات
            MachineryAndEquipmentMainBranch = -32,        //الات ومعدات  الفرع الرئيسي
            OfficeEquipmentAndPrinters = -33,      //أجهزة مكتبية وطابعات
            OfficeEquipmentAndPrintersMainBranch = -34,        //أجهزة مكتبية وطابعات  الفرع الرئيسي 
            IntangibleAssets = -35,    //الأصول غير الملموسة
            AccountingPrograms = -36,      //برامج محاسبية 
            liabilities = -37,     //الالتزامات
            currentLiabilities = -38,     //الالتزامات المتداولة 
            Suppliers = -202001,    //الموردين
            MainBranchSppliers = -40,   //موردين  الفرع الرئيسي
            cashSuppliers = -202001001,     //مورد نقدي
            Creditors = -42,    //دائنــــــــــــــون
            OtherCreditBalances = -202003001,      //أرصدة دائنة اخري
            CurrentPartners = -44,    //جاري الشركاء
            CurrentPartnersA = -45,      //جاري الشريك أ
            CurrentPartnersB = -46,      //جاري الشريك ب
            AccruedExpenses = -47,    //مصروفات مستحقة
            DueShowroomRent = -48,      //ايجار معرض مستحق
            DueHousingRent = -49,      //ايجار سكن مستحق
            DueWarehouseRent = -50,      //ايجار مستودع مستحق
            DueMedicalInsurance = -51,      //تأمين طبي مستحق
            AccruedExpense = -52,      //مصروف مستحق 
            SalariesOwed = -502006,    //الرواتب المستحقة
            ShortTermLoans = -54,    //قروض قصيرة الأجل
            ValueAddedTaxSales = -202002002,    //الضريبة القيمه المضافه ( المبيعات)
            TaxesOwedSettlement = -56,    //الضرائب المستحقة - تسوية 
            UnearnedRevenue = -57,    //إيرادات غير مكتسبة
            DepreciationComplex = -58,     //مجمع الاهـــــــــــــــــــلاك
            DepreciationComplexForCars = -59,      //مجمع الاهلاك للسيارات
            DepreciationComplexForMachineryAndEquipment = -60,      //مجمع الاهلاك الات والمعدات
            DepreciationComplexOfficeEquipmentAndPrinters = -61,      //مجمع الاهلاك أجهزة مكتبية وطابعات
            Allocations = -62,     //المخصصـــــــــــــات
            ProvisionForEndOfSeverancePay = -63,       //مخصص مكافأة نهاية الخدمة
            noncurrentLiabilities = -64,  //الالتزامات الغير المتداولة
            LongTermLoans = -65,    //قروض طويلة الاجل 
            propertyRights = -66,     //حقوق الملكية
            capital = -67,     //راس المال 
            PartnersCapitalA = -68,   //رأس مال الشريك أ
            PartnersCapitalB = -69,   //رأس مال الشريك ب
            Reserves = -70, //احتياطيات
            StatutoryReserve = -71,   //احتياطي نظامي
            TheRetainedEarningsOrLosses = -72,     //الأرباح المبقاة او الخسائر 
            profitAndGeneralLosses = -73,    //الأرباح والخسائر العام
            RetainedEarningsOrLosses = -74,    //الأرباح المبقاة او الخسائر 
            revenues = -75,     //الايرادات
            businessRevenue = -76,     //إيرادات النشاط 
            Sales = -401001,       //المبيعات			           
            SalesReturns = -401002,    //مردودات المبيعات
            SalesAllowances = -79,    //مسموحات المبيعات       
            DiscountPermitted = -401004,    //خصم مسموح بة			
            OtherIncomeNotRelatedToTheActivity = -81,     //ايرادات اخري غير متعلقه بالنشاط           
            otherIncome = -82,       //إيرادات اخري
            expensesAndCosts = -83,     //المصروفات و التكاليف
            businessCost = -84,     // تكلفة النشاط          
            CostOfGoodsSold = -501007,    //تكلفة البضاعة المباعة
            EarnedDiscount = -501006,    //خصم مكتسب                
            ExpensesAndOperationalCosts = -87,     //المصروفات و التكاليف التشغيلية
            StaffSalariesAndWages = -88,    //رواتب  واجور الموظفين
            Employees = -89,   //الموظفين
            DefultEmployee = -502006001,     //موظف افتراضي
            DefultCasher = -502006002,     //كاشير افتراضي
            commissions = -92,    //العمولات 
            salesMan = -502002002,   //عمولات المناديب
            Rewards = -94,    //مكافات
            perks = -95,    //اكراميات
            InternalTransferBetweenBranchesAndClients = -96,    //نقل داخلى بين الفروع والعملاء
            RenewalOfResidenceAndPassports = -97,    //تجديد الاقامات والجوازات
            NewVisaIssuanceFees = -98,     //رسوم اصدار التاشيرات جديده
            IssuanceOfExitAndReturnVisas = -99,    //اصدار تاشيرات الخروج والعودة 
            LabourOffice = -100,    //مكتب العمل 
            WarrantyTransferFee = -101,     //رسوم نقل كفالة
            Maintenance = -102,     //الصيــــــــــــــانة
            CarMaintenance = -103,       //صيانة السيارات
            ShowroomMaintenance = -104,       //صيانة معارض
            MaintenanceOfDevicesAndEquipment = -105,       //صيانة اجهزة ومعدات 
            SocialSecurity = -106,     //التأمينات الاجتماعية
            MedicalInsuranceForEmployees = -107,     //تأمين طبي للموظفين
            GovernmentFees = -108,     //الرسوم الحكومية
            FeesAndSubscriptions = -109,       //رسوم واشتراكات 
            CommercialRegistrationRenewalFees = -110,       //رسوم تجديد سجلات التجارية
            MunicipalFine = -111,       //غرامة البلدية			
            pettyCash = -112,     //رسوم نثرية
            TheRents = -113,     //الايجــــــــــارات
            TheRent = -114,       //الايجـــــار
            equipmentRent = -115,       //ايجار معدات
            CarsRent = -116,       //ايجار سيارات
            travelingTickets = -117,     //تذاكر سفر
            OfficeExpensesAndPublications = -118,     //مصاريف مكتبية ومطبوعات
            HospitalityExpenses = -119,     //مصاريف ضيافة
            BankExpenses = -120,     //مصاريف بنكية
            BankCommissions = -121,       //عمولات بنكية
            BankTransferFees = -122,       //رسوم تحويلات بنكية
            BankingNetworkFees = -123,       //رسوم شبكات بنكية
            TransportationExpenses = -124,     //مصروف نقل ومواصلات
            CleaningTools = -125,     //ادوات نظافة 
            SafetyAndSecurityTools = -126,     //ادوات امن وسلامة 
            MarketingExpenses = -127,     //مصاريف تسويقية
            AdvertisingAndPublications = -128,       //الاعلان ومطبوعات
            AdvertisingCampaigns = -129,       //حملات اعلانية 
            ThePeriodicInspectionOfVehicles = -130,     //الفحص الدوري للسيارات 
            depreciationExpenses = -131,     //مصاريف الاهلاك
            machineryAndEquipmentDepreciationExpense = -132,    //مصروف إهلاك الات والمعدات  
            CarDepreciationExpense = -133,    //مصروف اهلاك السيارات
            OfficeEquipmentAndPintersDepreciationExpense = -134,    //مصروف إهلاك أجهزة مكتبية وطابعات
            OtherAuthorities = -502005,     //جهات صرف اخري
            WaterBill = -502001002,     //جهات صرف اخري
            ElectricBill = -502001001,     //جهات صرف اخري
            NonoperatingExpenses = -138,  //مصاريف غير التشغيلية
            Taxes = -139,     //الضرائب
            HypotheticalBenefits = -140,    //فوائد فروض'
        }
        public static List<GLFinancialAccount> DefultGLFinancialAccountList()
        {
            var list = new List<GLFinancialAccount>();
            list.AddRange(new[]
            {
                
            /*Level 1*/
            /*1*/   
                new GLFinancialAccount
                {
                    Id = (int)FinanicalAccountDefultIds.assets,
                    IsMain=true,
                    ArabicName= "الاصول" ,
                    LatinName="Assets",
                    Status=(int)Status.Active,
                    ParentId=null,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01",
                    autoCoding ="1",
                    MainCode=1,
                    SubCode=0
                }, //الاصول
            /*2*/   
                new GLFinancialAccount
                {
                    Id = (int)FinanicalAccountDefultIds.longTermOpponents,
                    IsMain=true,
                    ArabicName= "الاصول المتداولة " ,
                    LatinName="long Term Opponents",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.assets,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001",
                    autoCoding ="1.1",
                    MainCode=1,
                    SubCode=1
                },//الاصول المتداولة 
		    /*3*/  
                new GLFinancialAccount
                {
                    Id = (int)FinanicalAccountDefultIds.Cashandequivalent,
                    IsMain=true,
                    ArabicName= "النقد وما يعادلة " ,
                    LatinName="Cashandequivalent",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.longTermOpponents,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0001",
                    autoCoding ="1.1.1",
                    MainCode=1,
                    SubCode=1
                },//النقد وما يعادلة
				/*4*/   new GLFinancialAccount
                {
                    Id = (int)FinanicalAccountDefultIds.Cashinthesafe,
                    IsMain=true,
                    ArabicName= "النقدية في الخزينة " ,
                    LatinName="Cashinthesafe",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.Cashandequivalent,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0001.0001",
                    autoCoding ="1.1.1.1",
                    MainCode=1,
                    SubCode=1
                },//النقدية في الخزينة
				/*5*/   new GLFinancialAccount
                {
                Id = (int)FinanicalAccountDefultIds.Mainbranchtreasury,
                    IsMain=false,
                    ArabicName= "خزينة الفرع الرئيسي " ,
                    LatinName="Mainbranchtreasury",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.Cashinthesafe,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0001.0001.0001",
                    autoCoding ="1.1.1.1.1",
                    MainCode=1,
                    SubCode=1
                },//خزينة الفرع الرئيسي
				/*6*/   new GLFinancialAccount
                {
                Id = (int)FinanicalAccountDefultIds.cashinthebank,
                    IsMain=true,
                    ArabicName= "النقدية في البنك " ,
                    LatinName="cashinthebank",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.Cashandequivalent,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0001.0002",
                    autoCoding ="1.1.1.2",
                    MainCode=1,
                    SubCode=2
                },//النقدية في البنك
				
				/*7*/   new GLFinancialAccount
                {
                Id = (int)FinanicalAccountDefultIds.AlRajhiBank,
                    IsMain=false,
                    ArabicName= "بنك الراجحي " ,
                    LatinName="AlRajhiBank",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.cashinthebank,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0001.0002.0001",
                    autoCoding ="1.1.1.2.1",
                    MainCode=1,
                    SubCode=1
                },//بنك الراجحي
				/*8*/   new GLFinancialAccount
                {
                Id = (int)FinanicalAccountDefultIds.BankAlbilad,
                    IsMain=false,
                    ArabicName= "بنك البلاد " ,
                    LatinName="BankAlbilad",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.cashinthebank,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0001.0002.0002",
                    autoCoding ="1.1.1.2.2",
                    MainCode=1,
                    SubCode=2
                },//بنك البلاد 
				/*9*/   new GLFinancialAccount
                {
                Id = (int)FinanicalAccountDefultIds.RiyadBank,
                    IsMain=false,
                    ArabicName= "بنك الرياض " ,
                    LatinName="RiyadBank",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.cashinthebank,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0001.0002.0003",
                    autoCoding ="1.1.1.2.3",
                    MainCode=1,
                    SubCode=3
                 },//بنك الرياض	
                /*10*/   new GLFinancialAccount
                {
                Id = (int)FinanicalAccountDefultIds.Customers,
                    IsMain=true,
                    ArabicName= "العملاء " ,
                    LatinName="Customers",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.longTermOpponents,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0002",
                    autoCoding ="1.1.2",
                    MainCode=1,
                    SubCode=2
                },//العملاء	
				/*11*/   new GLFinancialAccount
                {
                Id = (int)FinanicalAccountDefultIds.MainbranchCustomers,
                    IsMain=true,
                    ArabicName= "عملاء الفرع الرئيسي  " ,
                    LatinName="Main branch Customers",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.Customers,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0002.001",
                    autoCoding ="1.1.2.1",
                    MainCode=1,
                    SubCode=1
                },//عملاء الفرع الرئيسي 		
				/*12*/   new GLFinancialAccount
                {
                Id = (int)FinanicalAccountDefultIds.cashCustomer,
                    IsMain=false,
                    ArabicName= "عميل نقدي  " ,
                    LatinName="Cash Customer",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.MainbranchCustomers,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0002.001.001	",
                    autoCoding ="1.1.2.1.1",
                    MainCode=1,
                    SubCode=1
                },//عميل نقدي 	
				
		        /*13*/   new GLFinancialAccount
                {
                Id = (int)FinanicalAccountDefultIds.Advancedexpenses,
                    IsMain=true,
                    ArabicName= "مصروفات مقدمة " ,
                    LatinName="Advancedexpenses",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.longTermOpponents,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0003",
                    autoCoding ="1.1.3",
                    MainCode=1,
                    SubCode=3
                },//مصروفات مقدمة			
				/*14*/   new GLFinancialAccount
                {
                Id = (int)FinanicalAccountDefultIds.Advanceshowroomrent,
                    IsMain=false,
                    ArabicName= "ايجار معرض مقدم " ,
                    LatinName="Advanceshowroomrent",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.Advancedexpenses,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0003.001",
                    autoCoding ="1.1.3.1",
                    MainCode=1,
                    SubCode=1
                },//ايجار معرض مقدم
				/*15*/   new GLFinancialAccount
                {
                Id = (int)FinanicalAccountDefultIds.Advancehousingrent,
                    IsMain=false,
                    ArabicName= "ايجار سكن مقدم " ,
                    LatinName="Advancehousingrent",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.Advancedexpenses,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0003.002",
                    autoCoding ="1.1.3.2",
                    MainCode=1,
                    SubCode=2
                },//ايجار سكن مقدم
				/*16*/   new GLFinancialAccount
                {
                Id = (int)FinanicalAccountDefultIds.Upfrontwarehouserent,
                    IsMain=false,
                    ArabicName= "ايجار مستودع مقدم " ,
                    LatinName="Upfrontwarehouserent",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.Advancedexpenses,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0003.003",
                    autoCoding ="1.1.3.3",
                    MainCode=1,
                    SubCode=3
                },//ايجار مستودع مقدم
				/*17*/   new GLFinancialAccount
                {
                Id = (int)FinanicalAccountDefultIds.Providedmedicalinsurance,
                    IsMain=false,
                    ArabicName= "تأمين طبي مقدم " ,
                    LatinName="Providedmedicalinsurance",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.Advancedexpenses,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0003.004",
                    autoCoding ="1.1.3.4",
                    MainCode=1,
                    SubCode=4
                },//تأمين طبي مقدم
				/*18*/   new GLFinancialAccount
                {
                Id = (int)FinanicalAccountDefultIds.Advanceexpense,
                    IsMain=false,
                    ArabicName= "مصروف مقدم " ,
                    LatinName="Advanceexpense",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.Advancedexpenses,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0003.005",
                    autoCoding ="1.1.3.5",
                    MainCode=1,
                    SubCode=5
                },//مصروف مقدم
				
				 /*19*/   new GLFinancialAccount
                {
                Id = (int)FinanicalAccountDefultIds.OtherDebitBalances,
                    IsMain=true,
                    ArabicName= "أرصدة مدينة اخري " ,
                    LatinName="OtherDebitBalances",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.longTermOpponents,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0004",
                    autoCoding ="1.1.4",
                    MainCode=1,
                    SubCode=4
                },//أرصدة مدينة اخري
				 /*20*/   new GLFinancialAccount
                {
                Id = (int)FinanicalAccountDefultIds.withdrawals,
                    IsMain=true,
                    ArabicName= "المسحوبات " ,
                    LatinName="Owithdrawals",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.OtherDebitBalances,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0004.001",
                    autoCoding ="1.1.4.1",
                    MainCode=1,
                    SubCode=1
                },//المسحوبات
				 /*21*/   new GLFinancialAccount
                {
                Id = (int)FinanicalAccountDefultIds.PartnerwithdrawalsA,
                    IsMain=false,
                    ArabicName= "مسحوبات الشريك ا " ,
                    LatinName="PartnerwithdrawalsA",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.withdrawals,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0004.001.001",
                    autoCoding ="1.1.4.1.1",
                    MainCode=1,
                    SubCode=1
                },//مسحوبات الشريك ا
				 /*22*/   new GLFinancialAccount
                {
                Id = (int)FinanicalAccountDefultIds.PartnerwithdrawalsB,
                    IsMain=false,
                    ArabicName= "مسحوبات الشريك ب " ,
                    LatinName="PartnerwithdrawalsB",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.withdrawals,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0004.001.002",
                    autoCoding ="1.1.4.1.2",
                    MainCode=1,
                    SubCode=1
                },//مسحوبات الشريك ب
			
				/*23*/   new GLFinancialAccount
                {
                Id = (int)FinanicalAccountDefultIds.ValueAddedTaxPurchases,
                    IsMain=false,
                    ArabicName= "الضريبة القيمه المضافه ( المشتريات )" ,
                    LatinName="Value Added Tax Purchases",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.OtherDebitBalances,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0004.002",
                    autoCoding ="1.1.4.2",
                    MainCode=1,
                    SubCode=2
                },//الضريبة القيمه المضافه ( المشتريات )
                	/*24*/   new GLFinancialAccount
                {
                    Id = (int)FinanicalAccountDefultIds.employeeadvances,
                    IsMain=true,
                    ArabicName= "سلف العاملين " ,
                    LatinName="employeeadvances",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.OtherDebitBalances,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0004.003",
                    autoCoding ="1.1.4.3",
                    MainCode=1,
                    SubCode=3
                },//سلف العاملين
                /*24*/   
                new GLFinancialAccount
                {
                    Id = (int)FinanicalAccountDefultIds.theOtherDebitBalances,
                    IsMain=false,
                    ArabicName= "ارصدة مدينة اخري " ,
                    LatinName="The Other Debit Balances",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.OtherDebitBalances,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0004.004",
                    autoCoding ="1.1.4.4",
                    MainCode=1,
                    SubCode=4
                },//ارصدة مدينة اخري

				/*25*/   new GLFinancialAccount
                {
                Id = (int)FinanicalAccountDefultIds.Inventory,
                    IsMain=true,
                    ArabicName= "المخزون " ,
                    LatinName="Inventory",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.longTermOpponents,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0005",
                    autoCoding ="1.1.5",
                    MainCode=1,
                    SubCode=5
                },//المخزون				
				/*26*/   new GLFinancialAccount
                {
                Id = (int)FinanicalAccountDefultIds.InventoryMainStore,
                    IsMain=false,
                    ArabicName= "المخزون الفرع الرئيسي " ,
                    LatinName="Inventory Main Branch",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.Inventory,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0001.0005.001",
                    autoCoding ="1.1.5.1",
                    MainCode=1,
                    SubCode=1
                },//المخزون المخزن الرئيسي				
				 /*27*/   new GLFinancialAccount
                {
                 Id = (int)FinanicalAccountDefultIds.noncurrentassets,
                    IsMain=true,
                    ArabicName= "الاصول غير المتداولة " ,
                    LatinName="non-current assets",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.assets,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0002",
                    autoCoding ="1.2",
                    MainCode=1,
                    SubCode=2
                },//الاصول غير المتداولة				
				 /*28*/   new GLFinancialAccount
                {
                 Id = (int)FinanicalAccountDefultIds.FixedAssets,
                    IsMain=true,
                    ArabicName= "الاصول الثايته " ,
                    LatinName="FixedAssets",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.noncurrentassets,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0002.001",
                    autoCoding ="1.2.1",
                    MainCode=1,
                    SubCode=1
                },//الاصول الثايته				
				/*29*/   new GLFinancialAccount
                {
                 Id = (int)FinanicalAccountDefultIds.cars,
                    IsMain=true,
                    ArabicName= "السيارات " ,
                    LatinName="cars",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.FixedAssets,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0002.001.001",
                    autoCoding ="1.2.1.1",
                    MainCode=1,
                    SubCode=1
                },//السيارات
				/*30*/   new GLFinancialAccount
                {
                 Id = (int)FinanicalAccountDefultIds.MainBranchCars,
                    IsMain=true,
                    ArabicName= "سيارات  الفرع الرئيسي " ,
                    LatinName="MainBranchCars",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.cars,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0002.001.001.001",
                    autoCoding ="1.2.1.1.1",
                    MainCode=1,
                    SubCode=1
                },//سيارات  الفرع الرئيسي
				/*31*/   new GLFinancialAccount
                {
                 Id = (int)FinanicalAccountDefultIds.MachineryAndEquipment,
                    IsMain=true,
                    ArabicName= "الات والمعدات " ,
                    LatinName="Machinery And Equipment",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.FixedAssets,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0002.001.002",
                    autoCoding ="1.2.1.2",
                    MainCode=1,
                    SubCode=2
                },//الات والمعدات
				/*32*/   new GLFinancialAccount
                {
                 Id = (int)FinanicalAccountDefultIds.MachineryAndEquipmentMainBranch,
                    IsMain=true,
                    ArabicName= "الات ومعدات  الفرع الرئيسي " ,
                    LatinName="MachineryAndEquipmentMainBranch",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.MachineryAndEquipment,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0002.001.002.001",
                    autoCoding ="1.2.1.2.1",
                    MainCode=1,
                    SubCode=1
                },//الات ومعدات  الفرع الرئيسي
				/*33*/   new GLFinancialAccount
                {
                 Id = (int)FinanicalAccountDefultIds.OfficeEquipmentAndPrinters,
                    IsMain=true,
                    ArabicName= "أجهزة مكتبية وطابعات " ,
                    LatinName="Office Equipment AndPrinters",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.FixedAssets,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0002.001.003",
                    autoCoding ="1.2.1.3",
                    MainCode=1,
                    SubCode=3
                },//أجهزة مكتبية وطابعات 
				
				/*34*/   new GLFinancialAccount
                {
                 Id = (int)FinanicalAccountDefultIds.OfficeEquipmentAndPrintersMainBranch,
                    IsMain=true,
                    ArabicName= "أجهزة مكتبية وطابعات  الفرع الرئيسي  " ,
                    LatinName="OfficeEquipmentAndPrintersMainBranch",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.OfficeEquipmentAndPrinters,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0002.001.003.001",
                    autoCoding ="1.2.1.3.1",
                    MainCode=1,
                    SubCode=1
                },//أجهزة مكتبية وطابعات  الفرع الرئيسي 	
				/*35*/   new GLFinancialAccount
                {
                 Id = (int)FinanicalAccountDefultIds.IntangibleAssets,
                    IsMain=true,
                    ArabicName= "الأصول غير الملموسة" ,
                    LatinName="Intangible Assets",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.noncurrentassets,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0002.002",
                    autoCoding ="1.2.2",
                    MainCode=1,
                    SubCode=2
                },//الأصول غير الملموسة
				/*36*/   new GLFinancialAccount
                {
                 Id = (int)FinanicalAccountDefultIds.AccountingPrograms,
                    IsMain=false,
                    ArabicName= "برامج المحاسبه" ,
                    LatinName="Accounting Programs",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.IntangibleAssets,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Debit,
                    FinalAccount=1,
                    AccountCode="01.0002.002.001",
                    autoCoding ="1.2.2.1",
                    MainCode=1,
                    SubCode=1
                },//الأصول غير الملموسة

            /*Level 2*/
            /*37*/  new GLFinancialAccount
                {
                    Id = (int)FinanicalAccountDefultIds.liabilities,
                    IsMain=true,
                    ArabicName= "الالتزامات" ,
                    LatinName="Liabilities",
                    Status=(int)Status.Active,
                    ParentId=null,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Credit,
                    FinalAccount=1,
                    AccountCode="02",
                    autoCoding ="2",
                    MainCode=2,
                    SubCode=0
                }, //"الالتزامات"								
            /*38*/  new GLFinancialAccount
                {
                    Id = (int)FinanicalAccountDefultIds.currentLiabilities,
                    IsMain=true,
                    ArabicName= "الالتزامات المتداولة  " ,
                    LatinName="Current Liabilities",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.liabilities,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Credit,
                    FinalAccount=1,
                    AccountCode="02.0001",
                    autoCoding ="2.1",
                    MainCode=2,
                    SubCode=1
                }, //"الالتزامات المتداولة  "				
				/*39*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.Suppliers,
                        IsMain=true,
                        ArabicName= "الموردين" ,
                        LatinName="Suppliers",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.currentLiabilities,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0001",
                        autoCoding ="2.1.1",
                        MainCode=2,
                        SubCode=1
                    },//"الموردين"					
					/*40*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.MainBranchSppliers,
                        IsMain=true,
                        ArabicName= "موردين  الفرع الرئيسي" ,
                        LatinName="MainBranchSppliers",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.Suppliers,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0001.001",
                        autoCoding ="2.1.1.1",
                        MainCode=2,
                        SubCode=1
                    },//"موردين  الفرع الرئيسي"					
                    /*41*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.cashSuppliers,
                        IsMain=false,
                        ArabicName= "مورد نقدي" ,
                        LatinName="Cash Suppliers",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.MainBranchSppliers,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0001.0001.0001",
                        autoCoding ="2.2.1.1.1",
                        MainCode=2,
                        SubCode=1
                    },//"مورد نقدي"									
					/*42*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.Creditors,
                        IsMain=true,
                        ArabicName= "دائنــــــــــــــون" ,
                        LatinName="Creditors",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.currentLiabilities,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0002",
                        autoCoding ="2.1.2",
                        MainCode=2,
                        SubCode=2
                    },//"دائنــــــــــــــون"					
					/*43*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.OtherCreditBalances,
                        IsMain=false,
                        ArabicName= "أرصدة دائنة اخري" ,
                        LatinName="OtherCreditBalances",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.Creditors,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0002.0001",
                        autoCoding ="2.1.2.1",
                        MainCode=2,
                        SubCode=1
                    },//"أرصدة دائنة اخري"					
					/*44*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.CurrentPartners,
                        IsMain=true,
                        ArabicName= "جاري الشركاء" ,
                        LatinName="CurrentPartners",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.currentLiabilities,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0003",
                        autoCoding ="2.1.3",
                        MainCode=2,
                        SubCode=3
                    },//"جاري الشركاء"
					/*45*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.CurrentPartnersA,
                        IsMain=false,
                        ArabicName= "جاري الشريك أ" ,
                        LatinName="CurrentPartnersA",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.CurrentPartners,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0003.0001",
                        autoCoding ="2.1.3.1",
                        MainCode=2,
                        SubCode=1
                    },//جاري الشريك أ"					
					/*46*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.CurrentPartnersB,
                        IsMain=false,
                        ArabicName= "جاري الشريك ب" ,
                        LatinName="CurrentPartnersB",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.CurrentPartners,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0003.0002",
                        autoCoding ="2.1.3.2",
                        MainCode=2,
                        SubCode=2
                    },//جاري الشريك ب"										
					/*47*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.AccruedExpenses,
                        IsMain=true,
                        ArabicName= "مصروفات مستحقة" ,
                        LatinName="AccruedExpenses",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.currentLiabilities,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0004",
                        autoCoding ="2.1.4",
                        MainCode=2,
                        SubCode=4
                    },//"مصروفات مستحقة"				
					/*48*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.DueShowroomRent,
                        IsMain=false,
                        ArabicName= "ايجار معرض مستحق" ,
                        LatinName="DueShowroomRent",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.AccruedExpenses,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0004.0001",
                        autoCoding ="2.1.4.1",
                        MainCode=2,
                        SubCode=1
                    },//"ايجار معرض مستحق"					
					/*49*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.DueHousingRent,
                        IsMain=false,
                        ArabicName= "ايجار سكن مستحق" ,
                        LatinName="DueHousingRent",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.AccruedExpenses,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0004.0002",
                        autoCoding ="2.1.4.2",
                        MainCode=2,
                        SubCode=2
                    },//"ايجار سكن مستحق"			
				    /*50*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.DueWarehouseRent,
                        IsMain=false,
                        ArabicName= "ايجار مستودع مستحق" ,
                        LatinName="DueWarehouseRent",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.AccruedExpenses,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0004.0003",
                        autoCoding ="2.1.4.3",
                        MainCode=2,
                        SubCode=3
                    },//"ايجار مستودع مستحق"					
					/*51*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.DueMedicalInsurance,
                        IsMain=false,
                        ArabicName= "تأمين طبي مستحق" ,
                        LatinName="DueMedicalInsurance",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.AccruedExpenses,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0004.0004",
                        autoCoding ="2.1.4.4",
                        MainCode=2,
                        SubCode=4
                    },//"تأمين طبي مستحق"					
					/*52*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.AccruedExpense,
                        IsMain=false,
                        ArabicName= "مصروف مستحق " ,
                        LatinName="AccruedExpense",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.AccruedExpenses,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0004.0005",
                        autoCoding ="2.1.4.5",
                        MainCode=2,
                        SubCode=5
                    },//"مصروف مستحق "					
					/*53*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.SalariesOwed,
                        IsMain=true,
                        ArabicName= "الرواتب المستحقة" ,
                        LatinName="SalariesOwed",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.currentLiabilities,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0005",
                        autoCoding ="2.1.5",
                        MainCode=2,
                        SubCode=5
                    },//"الرواتب المستحقة"					
                    /*54*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.ShortTermLoans,
                        IsMain=true,
                        ArabicName= "قروض قصيرة الأجل" ,
                        LatinName="ShortTermLoans",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.currentLiabilities,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0006",
                        autoCoding ="2.1.6",
                        MainCode=2,
                        SubCode=6
                    },//"قروض قصيرة الأجل"				     					 
                    /*55*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.ValueAddedTaxSales,
                        IsMain=false,
                        ArabicName= "الضريبة القيمه المضافه ( المبيعات)" ,
                        LatinName="Value Added Tax Sales",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.currentLiabilities,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0007",
                        autoCoding ="2.1.7",
                        MainCode=2,
                        SubCode=7
                    },//"الضريبة القيمه المضافه ( المبيعات)"					
					/*56*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.TaxesOwedSettlement,
                        IsMain=false,
                        ArabicName= "الضرائب المستحقة - تسوية " ,
                        LatinName="TaxesOwed-Settlement",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.currentLiabilities,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0008",
                        autoCoding ="2.1.8",
                        MainCode=2,
                        SubCode=8
                    },//"الضرائب المستحقة - تسوية "
					/*57*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.UnearnedRevenue,
                        IsMain=false,
                        ArabicName= "إيرادات غير مكتسبة " ,
                        LatinName="Unearned Revenue",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.currentLiabilities,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0009",
                        autoCoding ="2.1.9",
                        MainCode=2,
                        SubCode=9
                    },//"إيرادات غير مكتسبة "
					/*58*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.DepreciationComplex,
                        IsMain=true,
                        ArabicName= "مجمع الاهـــــــــــــــــــلاك " ,
                        LatinName="Depreciation Complex",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.currentLiabilities,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0010",
                        autoCoding ="2.1.10",
                        MainCode=2,
                        SubCode=10
                    },//"مجمع الاهـــــــــــــــــــلاك "
                    /*59*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.DepreciationComplexForCars,
                        IsMain=false,
                        ArabicName= "مجمع الاهلاك للسيارات " ,
                        LatinName="Depreciation complex for cars",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.DepreciationComplex,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0010.0001",
                        autoCoding ="2.1.10.1",
                        MainCode=2,
                        SubCode=1
                    },//"مجمع الاهلاك للسيارات "
					/*60*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.DepreciationComplexForMachineryAndEquipment,
                        IsMain=false,
                        ArabicName= "مجمع الاهلاك الات والمعدات " ,
                        LatinName="Depreciation Complex For Machinery And Equipment",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.DepreciationComplex,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0010.0002",
                        autoCoding ="2.1.10.2",
                        MainCode=2,
                        SubCode=2
                    },//"مجمع الاهلاك الات والمعدات "
					/*61*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.DepreciationComplexOfficeEquipmentAndPrinters,
                        IsMain=false,
                        ArabicName= "مجمع الاهلاك أجهزة مكتبية وطابعات" ,
                        LatinName="Depreciation Complex Office Equipment And Printers",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.DepreciationComplex,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0010.0003",
                        autoCoding ="2.1.10.3",
                        MainCode=2,
                        SubCode=3
                    },//"مجمع الاهلاك أجهزة مكتبية وطابعات "
					/*62*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.Allocations,
                        IsMain=true,
                        ArabicName= "المخصصـــــــــــــات " ,
                        LatinName="Allocations",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.currentLiabilities,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0011",
                        autoCoding ="2.1.11",
                        MainCode=2,
                        SubCode=11
                    },//"المخصصـــــــــــــات "
					/*63*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.ProvisionForEndOfSeverancePay,
                        IsMain=false,
                        ArabicName= "مخصص مكافأة نهاية الخدمة " ,
                        LatinName="Provision For End Of Severance Pay",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.Allocations,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="02.0001.0011.0001",
                        autoCoding ="2.1.11.1",
                        MainCode=2,
                        SubCode=1
                    },//"مخصص مكافأة نهاية الخدمة "									
            /*64*/  new GLFinancialAccount
                {
                    Id = (int)FinanicalAccountDefultIds.noncurrentLiabilities,
                    IsMain=true,
                    ArabicName= "الالتزامات الغير المتداولة  " ,
                    LatinName="non-current Liabilities",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.liabilities,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Credit,
                    FinalAccount=1,
                    AccountCode="02.0002",
                    autoCoding ="2.2",
                    MainCode=2,
                    SubCode=2
                }, //"الالتزامات الغير المتداولة  "                
				/*65*/  new GLFinancialAccount
                {
                    Id = (int)FinanicalAccountDefultIds.LongTermLoans,
                    IsMain=false,
                    ArabicName= "قروض طويلة الاجل " ,
                    LatinName="LongTermLoans",
                    Status=(int)Status.Active,
                    ParentId=(int)FinanicalAccountDefultIds.noncurrentLiabilities,
                    CurrencyId=1,
                    BranchId=1,
                    FA_Nature=(int)FA_Nature.Credit,
                    FinalAccount=1,
                    AccountCode="02.0002.0001",
                    autoCoding ="2.2.1",
                    MainCode=2,
                    SubCode=1
                }, //"قروض طويلة الاجل  "   
			
			/*Level 3*/
            /*66*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.propertyRights,
                        IsMain=true,
                        ArabicName= "حقوق الملكية" ,
                        LatinName="Property Rights",
                        Status=(int)Status.Active,
                        ParentId=null,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="03",
                        autoCoding ="3",
                        MainCode=3,
                        SubCode=0
                    },//"حقوق الملكية"					
            /*67*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.capital,
                        IsMain=true,
                        ArabicName= "راس المال" ,
                        LatinName="capital",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.propertyRights,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="03.0001",
                        autoCoding ="3.1",
                        MainCode=3,
                        SubCode=1
                    },//"راس المال "
            /*68*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.PartnersCapitalA,
                        IsMain=false,
                        ArabicName= "رأس مال الشريك أ" ,
                        LatinName="Partners Capital A",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.capital,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="03.0001.0001",
                        autoCoding ="3.1.1",
                        MainCode=3,
                        SubCode=1
                    },//رأس مال الشريك أ
            /*69*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.PartnersCapitalB,
                        IsMain=false,
                        ArabicName= "رأس مال الشريك ب" ,
                        LatinName="Partners Capital B ",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.capital,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="03.0001.0002",
                        autoCoding ="3.1.2",
                        MainCode=3,
                        SubCode=2
                    },//رأس مال الشريك ب					
			/*70*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.Reserves,
                        IsMain=true,
                        ArabicName= "احتياطيات" ,
                        LatinName="Reserves",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.propertyRights,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="03.0002",
                        autoCoding ="3.2",
                        MainCode=3,
                        SubCode=2
                    },//"احتياطيات "					
				/*71*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.StatutoryReserve,
                        IsMain=false,
                        ArabicName= "احتياطي نظامي" ,
                        LatinName="StatutoryReserve",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.Reserves,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="03.0002.0001",
                        autoCoding ="3.2.1",
                        MainCode=3,
                        SubCode=1
                    },//"احتياطي نظامي "						
            /*72*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.TheRetainedEarningsOrLosses,
                        IsMain=true,
                        ArabicName= "الأرباح المبقاة او الخسائر  " ,
                        LatinName="The Retained Earnings Or Losses",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.propertyRights,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="03.0003",
                        autoCoding ="3.3",
                        MainCode=3,
                        SubCode=3
                        },//"الأرباح المبقاة او الخسائر "						
		           /*73*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.profitAndGeneralLosses,
                        IsMain=false,
                        ArabicName= "الأرباح والخسائر العام  " ,
                        LatinName="profit And General Losses",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.TheRetainedEarningsOrLosses,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="03.0003.0001",
                        autoCoding ="3.3.1",
                        MainCode=3,
                        SubCode=1
                    },//"الأرباح والخسائر العام "
                   /*74*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.RetainedEarningsOrLosses,
                        IsMain=false,
                        ArabicName= "الأرباح والخسائر العام  " ,
                        LatinName="Retained Earnings Or Losse",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.TheRetainedEarningsOrLosses,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=1,
                        AccountCode="03.0003.0002",
                        autoCoding ="3.3.2",
                        MainCode=3,
                        SubCode=2
                    },//"الأرباح المبقاة او الخسائر  "					
						        
            /*Level 4*/
            /*75*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.revenues,
                        IsMain=true,
                        ArabicName= "الايرادات",
                        LatinName="Revenues",
                        Status=(int)Status.Active,
                        ParentId=null,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=2,
                        AccountCode="04",
                        autoCoding ="4",
                        MainCode=4,
                        SubCode=0
                    },//"الايرادات"
            /*76*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.businessRevenue,
                        IsMain=true,
                        ArabicName= "إيرادات النشاط التجاري",
                        LatinName="Business Revenue",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.revenues,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=2,
                        AccountCode="04.0001",
                        autoCoding ="4.1",
                        MainCode=4,
                        SubCode=1
                    },//إيرادات النشاط التجاري
            /*77*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.Sales,
                        IsMain=false,
                        ArabicName= "المبيعات",
                        LatinName="Sales",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.businessRevenue,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=2,
                        AccountCode="04.0001.0001",
                        autoCoding ="4.1.1",
                        MainCode=4,
                        SubCode=1
                    },//المبيعات
            /*78*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.SalesReturns,
                        IsMain=false,
                        ArabicName= "مردودات المبيعات",
                        LatinName="Sales Returns",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.businessRevenue,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="04.0001.0002",
                        autoCoding ="4.1.2",
                        MainCode=4,
                        SubCode=2
                    },//مردودات المبيعات
            /*79*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.SalesAllowances,
                        IsMain=false,
                        ArabicName= "مسموحات المبيعات",
                        LatinName="Sales Allowances",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.businessRevenue,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="04.0001.0003",
                        autoCoding ="4.1.3",
                        MainCode=4,
                        SubCode=3
                    },//مسموحات المبيعات
            /*80*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.DiscountPermitted,
                        IsMain=false,
                        ArabicName= "خصم مسموح بة",
                        LatinName="Discount Permitted",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.businessRevenue,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="04.0001.0004",
                        autoCoding ="4.1.4",
                        MainCode=4,
                        SubCode=4
                    },//خصم مسموح بة
            /*81*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.OtherIncomeNotRelatedToTheActivity,
                        IsMain=true,
                        ArabicName= "ايرادات اخري غير متعلقه بالنشاط",
                        LatinName="Other Income Not Related To The Activity",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.revenues,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=2,
                        AccountCode="04.0002",
                        autoCoding ="4.2",
                        MainCode=4,
                        SubCode=2
                    },//ايرادات اخري غير متعلقه بالنشاط
            /*82*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.otherIncome,
                        IsMain=true,
                        ArabicName= "إيرادات اخري",
                        LatinName="otherIncome",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.OtherIncomeNotRelatedToTheActivity,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=2,
                        AccountCode="04.0002.0001",
                        autoCoding ="4.2.1",
                        MainCode=4,
                        SubCode=1
                    },//إيرادات اخري
										
            /*Level 5*/
            /*83*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.expensesAndCosts,
                        IsMain=true,
                        ArabicName= "المصروفات و التكاليف",
                        LatinName="Expenses And Costs",
                        Status=(int)Status.Active,
                        ParentId=null,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05",
                        autoCoding ="5",
                        MainCode=5,
                        SubCode=0
                    },//المصروفات و التكاليف
            /*84*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.businessCost,
                        IsMain=true,
                        ArabicName= "تكلفة النشاط التجاري",
                        LatinName="Expenses And Costs",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.expensesAndCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0001",
                        autoCoding ="5.1",
                        MainCode=5,
                        SubCode=1
                    },//تكلفة النشاط التجاري   
            /*85*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.CostOfGoodsSold,
                        IsMain=false,
                        ArabicName= "تكلفة البضاعة المباعة",
                        LatinName="Cost Of Goods Sold",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.businessCost,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0001.0001",
                        autoCoding ="5.1.1",
                        MainCode=5,
                        SubCode=1
                    },//تكلفة البضاعة المباعة            
            /*86*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.EarnedDiscount,
                        IsMain=false,
                        ArabicName= "خصم مكتسب",
                        LatinName="Earned Discount",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.businessCost,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Credit,
                        FinalAccount=2,
                        AccountCode="05.0001.0002",
                        autoCoding ="5.1.2",
                        MainCode=5,
                        SubCode=2
                    },//خصم مكتسب
            /*87*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        IsMain=true,
                        ArabicName= "المصروفات و التكاليف التشغيلية",
                        LatinName="Expenses And Operational Costs",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.expensesAndCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002",
                        autoCoding ="5.2",
                        MainCode=5,
                        SubCode=2
                    },//المصروفات و التكاليف التشغيلية
            /*88*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.StaffSalariesAndWages,
                        IsMain=true,
                        ArabicName= "رواتب  واجور الموظفين",
                        LatinName="Staff Salaries And Wages",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0001",
                        autoCoding ="5.2.1",
                        MainCode=5,
                        SubCode=1
                    },//رواتب  واجور الموظفين
            /*89*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.Employees,
                        IsMain=true,
                        ArabicName= "الموظفين",
                        LatinName="Employees",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.StaffSalariesAndWages,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0001.0001",
                        autoCoding ="5.2.1.1",
                        MainCode=5,
                        SubCode=1
                    },//الموظفين 
					 /*90*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.DefultEmployee,
                        IsMain=false,
                        ArabicName= "موظف افتراضي",
                        LatinName="Defult Employee",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.Employees,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0001.0001.0001",
                        autoCoding ="5.2.1.1.1",
                        MainCode=5,
                        SubCode=1
                    },//موظف افتراضي 
					/*91*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.DefultCasher,
                        IsMain=false,
                        ArabicName= "كاشير افتراضي",
                        LatinName="Defult Casher",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.Employees,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0001.0001.0002",
                        autoCoding ="5.2.1.1.2",
                        MainCode=5,
                        SubCode=2
                    },//كاشير افتراضي 					
				/*92*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.commissions,
                        IsMain=true,
                        ArabicName= "العمولات ",
                        LatinName="commissions",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0002",
                        autoCoding ="5.2.2",
                        MainCode=5,
                        SubCode=2
                    },//العمولات						
                    /*93*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.salesMan,
                        IsMain=false,
                        ArabicName= "عمولات المناديب",
                        LatinName="sales Man",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.commissions,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0002.0001",
                        autoCoding ="5.2.2.1",
                        MainCode=5,
                        SubCode=1
                    },//عمولات المناديب					
				/*94*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.Rewards,
                        IsMain=false,
                        ArabicName= "مكافات ",
                        LatinName="Rewards",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0003",
                        autoCoding ="5.2.3",
                        MainCode=5,
                        SubCode=3
                    },//مكافات						
				/*95*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.perks,
                        IsMain=false,
                        ArabicName= "اكراميات ",
                        LatinName="perks",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0004",
                        autoCoding ="5.2.4",
                        MainCode=5,
                        SubCode=4
                    },//اكراميات
				/*96*/ 
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.InternalTransferBetweenBranchesAndClients,
                        IsMain=false,
                        ArabicName= "نقل داخلى بين الفروع والعملاء ",
                        LatinName="Internal Transfer Between Branches And Clients",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0005",
                        autoCoding ="5.2.5",
                        MainCode=5,
                        SubCode=5
                    },//نقل داخلى بين الفروع والعملاء			
				/*97*/ 
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.RenewalOfResidenceAndPassports,
                        IsMain=false,
                        ArabicName= "تجديد الاقامات والجوازات ",
                        LatinName="Renewal Of Residence And Passports",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0006",
                        autoCoding ="5.2.6",
                        MainCode=5,
                        SubCode=6
                    },//تجديد الاقامات والجوازات				
				/*98*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.NewVisaIssuanceFees,
                        IsMain=false,
                        ArabicName= "رسوم اصدار التاشيرات جديده",
                        LatinName="New Visa Issuance Fees",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0007",
                        autoCoding ="5.2.7",
                        MainCode=5,
                        SubCode=7
                    },//رسوم اصدار التاشيرات جديده
				/*99*/ 
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.IssuanceOfExitAndReturnVisas,
                        IsMain=false,
                        ArabicName= "اصدار تاشيرات الخروج والعودة ",
                        LatinName="Issuance Of Exit And Return Visas",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0008",
                        autoCoding ="5.2.8",
                        MainCode=5,
                        SubCode=8
                    },//اصدار تاشيرات الخروج والعودة 
                /*100*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.LabourOffice,
                        IsMain=false,
                        ArabicName= "مكتب العمل  ",
                        LatinName="Labour Office",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0009",
                        autoCoding ="5.2.9",
                        MainCode=5,
                        SubCode=9
                    },//مكتب العمل 
				/*101*/ 
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.WarrantyTransferFee,
                        IsMain=false,
                        ArabicName= "رسوم نقل كفالة  ",
                        LatinName="Warranty TransferFee",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0010",
                        autoCoding ="5.2.10",
                        MainCode=5,
                        SubCode=10
                    },//رسوم نقل كفالة					
				/*102*/ 
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.Maintenance,
                        IsMain=true,
                        ArabicName= "الصيــــــــــــــانة ",
                        LatinName="Maintenance",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0011",
                        autoCoding ="5.2.11",
                        MainCode=5,
                        SubCode=11
                    },//الصيــــــــــــــانة
				    /*103*/
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.CarMaintenance,
                        IsMain=false,
                        ArabicName= "صيانة السيارات ",
                        LatinName="Car Maintenance",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.Maintenance,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0011.0001",
                        autoCoding ="5.2.11.1",
                        MainCode=5,
                        SubCode=1
                    },//صيانة السيارات	
					/*104*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.ShowroomMaintenance,
                        IsMain=false,
                        ArabicName= "صيانة معارض ",
                        LatinName="Showroom Maintenance",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.Maintenance,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0011.0002",
                        autoCoding ="5.2.11.2",
                        MainCode=5,
                        SubCode=2
                    },//صيانة معارض 	
					/*105*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.MaintenanceOfDevicesAndEquipment,
                        IsMain=false,
                        ArabicName= "صيانة اجهزة ومعدات  ",
                        LatinName="Maintenance Of Devices And Equipment",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.Maintenance,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0011.0003",
                        autoCoding ="5.2.11.3",
                        MainCode=5,
                        SubCode=3
                    },//صيانة اجهزة ومعدات 					
				/*106*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.SocialSecurity,
                        IsMain=false,
                        ArabicName= "التأمينات الاجتماعية ",
                        LatinName="Social Security",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0012",
                        autoCoding ="5.2.12",
                        MainCode=5,
                        SubCode=12
                    },//التأمينات الاجتماعية
                /*107*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.MedicalInsuranceForEmployees,
                        IsMain=false,
                        ArabicName= "تأمين طبي للموظفين ",
                        LatinName="Medical Insurance For Employees",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0013",
                        autoCoding ="5.2.13",
                        MainCode=5,
                        SubCode=13
                    },//تأمين طبي للموظفين
				/*108*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.GovernmentFees,
                        IsMain=true,
                        ArabicName= "الرسوم الحكومية",
                        LatinName="Government Fees",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0014",
                        autoCoding ="5.2.14",
                        MainCode=5,
                        SubCode=14
                    },//الرسوم الحكومية
					/*109*/ 
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.FeesAndSubscriptions,
                        IsMain=false,
                        ArabicName= " رسوم واشتراكات ",
                        LatinName="Fees And Subscriptions",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.GovernmentFees,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0014.0001",
                        autoCoding ="5.2.14.1",
                        MainCode=5,
                        SubCode=1
                    },//رسوم واشتراكات 
					/*110*/ 
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.CommercialRegistrationRenewalFees,
                        IsMain=false,
                        ArabicName= "رسوم تجديد سجلات التجارية ",
                        LatinName="Commercial Registration Renewal Fees",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.GovernmentFees,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0014.0002",
                        autoCoding ="5.2.14.2",
                        MainCode=5,
                        SubCode=2
                    },//رسوم تجديد سجلات التجارية
					/*111*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.MunicipalFine,
                        IsMain=false,
                        ArabicName= "غرامة البلدية ",
                        LatinName="Municipal Fine",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.GovernmentFees,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0014.0003",
                        autoCoding ="5.2.14.3",
                        MainCode=5,
                        SubCode=3
                    },//غرامة البلدية

				/*113*/ 
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.TheRents,
                        IsMain=true,
                        ArabicName= "الايجــــــــــارات",
                        LatinName="The Rents",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0015",
                        autoCoding ="5.2.15",
                        MainCode=5,
                        SubCode=15
                    },//الايجــــــــــارات		
					/*114*/ 
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.TheRent,
                        IsMain=false,
                        ArabicName= "الايجـــــار",
                        LatinName="The Rent",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.TheRents,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0015.0001",
                        autoCoding ="5.2.15.1",
                        MainCode=5,
                        SubCode=1
                    },//الايجـــــار	       
					/*115*/ 
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.equipmentRent,
                        IsMain=false,
                        ArabicName= "ايجار معدات",
                        LatinName="equipment Rent",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.TheRents,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0015.0002",
                        autoCoding ="5.2.15.2",
                        MainCode=5,
                        SubCode=2
                    },//ايجار معدات	
					/*116*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.CarsRent,
                        IsMain=false,
                        ArabicName= "ايجار سيارات",
                        LatinName="Cars Rent",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.TheRents,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0015.0003",
                        autoCoding ="5.2.15.3",
                        MainCode=5,
                        SubCode=3
                    },//ايجار سيارات
				/*117*/ 
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.travelingTickets,
                        IsMain=false,
                        ArabicName= "تذاكر سفر",
                        LatinName="traveling Tickets",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0016",
                        autoCoding ="5.2.16",
                        MainCode=5,
                        SubCode=16
                    },//تذاكر سفر						
				/*118*/ 
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.OfficeExpensesAndPublications,
                        IsMain=false,
                        ArabicName= "مصاريف مكتبية ومطبوعات",
                        LatinName="Office Expenses And Publications",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0017",
                        autoCoding ="5.2.17",
                        MainCode=5,
                        SubCode=17
                    },//مصاريف مكتبية ومطبوعات		
				/*119*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.HospitalityExpenses,
                        IsMain=false,
                        ArabicName= "مصاريف ضيافة",
                        LatinName="Hospitality Expenses",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0018",
                        autoCoding ="5.2.18",
                        MainCode=5,
                        SubCode=18
                    },//مصاريف ضيافة							
				/*120*/ 
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.BankExpenses,
                        IsMain=true,
                        ArabicName= "مصاريف بنكية",
                        LatinName="Bank Expenses",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0019",
                        autoCoding ="5.2.19",
                        MainCode=5,
                        SubCode=19
                    },//مصاريف بنكية	
                    /*121*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.BankCommissions,
                        IsMain=false,
                        ArabicName= "عمولات بنكية",
                        LatinName="Bank Commissions",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.BankExpenses,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0019.0001",
                        autoCoding ="5.2.19.1",
                        MainCode=5,
                        SubCode=1
                    },//عمولات بنكية	
					/*122*/ 
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.BankTransferFees,
                        IsMain=false,
                        ArabicName= "رسوم شبكات بنكية",
                        LatinName="Bank TransferFees",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.BankExpenses,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0019.0002",
                        autoCoding ="5.2.19.2",
                        MainCode=5,
                        SubCode=2
                    },//رسوم شبكات بنكية
					/*123*/
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.BankingNetworkFees,
                        IsMain=false,
                        ArabicName= "رسوم شبكات بنكية",
                        LatinName="Banking Network Fees",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.BankExpenses,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0019.0003",
                        autoCoding ="5.2.19.3",
                        MainCode=5,
                        SubCode=3
                    },//مصروف نقل ومواصلات			
                /*124*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.TransportationExpenses,
                        IsMain=false,
                        ArabicName= "مصروف نقل ومواصلات",
                        LatinName="Transportation Expenses",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0020",
                        autoCoding ="5.2.20",
                        MainCode=5,
                        SubCode=20
                    },//مصروف نقل ومواصلات					
				/*125*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.CleaningTools,
                        IsMain=false,
                        ArabicName= "ادوات نظافة",
                        LatinName="Cleaning Tools",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0021",
                        autoCoding ="5.2.21",
                        MainCode=5,
                        SubCode=21
                    },//ادوات نظافة			
				/*126*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.SafetyAndSecurityTools,
                        IsMain=false,
                        ArabicName= "ادوات امن وسلامة",
                        LatinName="Safety And SecurityTools",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0022",
                        autoCoding ="5.2.22",
                        MainCode=5,
                        SubCode=22
                    },//ادوات امن وسلامة		
				/*127*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.MarketingExpenses,
                        IsMain=true,
                        ArabicName= "مصاريف تسويقية",
                        LatinName="Marketing Expenses",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0023",
                        autoCoding ="5.2.23",
                        MainCode=5,
                        SubCode=23
                    },//مصاريف تسويقية	
					/*128*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.AdvertisingAndPublications,
                        IsMain=false,
                        ArabicName= "الاعلان ومطبوعات",
                        LatinName="Advertising And Publications",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.MarketingExpenses,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0023.0001",
                        autoCoding ="5.2.23.1",
                        MainCode=5,
                        SubCode=1
                    },//الاعلان ومطبوعات	
					/*129*/ 
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.AdvertisingCampaigns,
                        IsMain=false,
                        ArabicName= "حملات اعلانية ",
                        LatinName="Advertising Campaigns",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.MarketingExpenses,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0023.0002",
                        autoCoding ="5.2.23.2",
                        MainCode=5,
                        SubCode=2
                    },//حملات اعلانية 				
				/*130*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.ThePeriodicInspectionOfVehicles,
                        IsMain=false,
                        ArabicName= "الفحص الدوري للسيارات ",
                        LatinName="The Periodic Inspection Of Vehicles",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0024",
                        autoCoding ="5.2.24",
                        MainCode=5,
                        SubCode=24
                    },//الفحص الدوري للسيارات 
					
				/*131*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.depreciationExpenses,
                        IsMain=true,
                        ArabicName= "مصاريف الاهلاك ",
                        LatinName="depreciation Expenses",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0025",
                        autoCoding ="5.2.25",
                        MainCode=5,
                        SubCode=25
                    },//مصاريف الاهلاك	
					/*132*/ 
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.machineryAndEquipmentDepreciationExpense,
                        IsMain=false,
                        ArabicName= "مصروف إهلاك الات والمعدات   ",
                        LatinName="machinery And Equipment Depreciation Expense",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.depreciationExpenses,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0025.0001",
                        autoCoding ="5.2.25.1",
                        MainCode=5,
                        SubCode=1
                    },//مصروف إهلاك الات والمعدات  
					/*133*/ 
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.CarDepreciationExpense,
                        IsMain=false,
                        ArabicName= "مصروف إهلاك الات والمعدات   ",
                        LatinName="Car Depreciation Expense",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.depreciationExpenses,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0025.0002",
                        autoCoding ="5.2.25.2",
                        MainCode=5,
                        SubCode=2
                    },//مصروف إهلاك الات والمعدات  
					/*134*/  
                    new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.OfficeEquipmentAndPintersDepreciationExpense,
                        IsMain=false,
                        ArabicName= "مصروف إهلاك أجهزة مكتبية وطابعات  ",
                        LatinName="Office Equipment And Pinters Depreciation Expense",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.depreciationExpenses,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0025.0003",
                        autoCoding ="5.2.25.3",
                        MainCode=5,
                        SubCode=3
                    },//مصروف إهلاك أجهزة مكتبية وطابعات				
				/*135*/ 
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.OtherAuthorities,
                        IsMain=true,
                        ArabicName= "جهات صرف اخري ",
                        LatinName="Other Authorities",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0026",
                        autoCoding ="5.2.26",
                        MainCode=5,
                        SubCode=26
                    },//جهات صرف اخري	
            				/*112*/ 
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.pettyCash,
                        IsMain=false,
                        ArabicName= "رسوم نثرية",
                        LatinName="petty Cash",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.ExpensesAndOperationalCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0027",
                        autoCoding ="5.2.27",
                        MainCode=5,
                        SubCode=27
                    },//رسوم نثرية
                      ///*136*/ 
                   new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.WaterBill,
                        IsMain=false,
                        ArabicName= "فواتير المياه ",
                        LatinName="Water Bill",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.OtherAuthorities,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0026.0001",
                        autoCoding ="5.2.26.1",
                        MainCode=5,
                        SubCode=1
                    },//فواتير المياه	
                      ///*137*/ 
                   new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.ElectricBill,
                        IsMain=false,
                        ArabicName= "فواتير الكهرباء ",
                        LatinName="Electric Bill",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.OtherAuthorities,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0002.0026.0002",
                        autoCoding ="5.2.26.2",
                        MainCode=5,
                        SubCode=2
                    },//فواتير الكهرباء	
			/*138*/  
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.NonoperatingExpenses,
                        IsMain=true,
                        ArabicName= "مصاريف غير التشغيلية",
                        LatinName="Non-operating Expenses",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.expensesAndCosts,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0003",
                        autoCoding ="5.3",
                        MainCode=5,
                        SubCode=3
                    },//مصاريف غير التشغيلية	
				/*139*/ 
            new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.Taxes,
                        IsMain=false,
                        ArabicName= "الضرائب ",
                        LatinName="Taxes",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.NonoperatingExpenses,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0003.0001",
                        autoCoding ="5.3.1",
                        MainCode=5,
                        SubCode=1
                    },//الضرائب		
				/*140*/  new GLFinancialAccount
                    {
                        Id = (int)FinanicalAccountDefultIds.HypotheticalBenefits,
                        IsMain=false,
                        ArabicName= "فوائد فروض ",
                        LatinName="Hypothetical Benefits",
                        Status=(int)Status.Active,
                        ParentId=(int)FinanicalAccountDefultIds.NonoperatingExpenses,
                        CurrencyId=1,
                        BranchId=1,
                        FA_Nature=(int)FA_Nature.Debit,
                        FinalAccount=2,
                        AccountCode="05.0003.0002",
                        autoCoding ="5.3.2",
                        MainCode=5,
                        SubCode=2
                    },//فوائد فروض	
            });
            return list;
        }
    }
}
