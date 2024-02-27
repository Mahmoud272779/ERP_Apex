
using App.Domain.Entities.Process.Barcode;
using App.Domain.Entities.Process.Store;
using App.Domain.Entities.Setup;
using App.Infrastructure.Persistence.Context;
using App.Domain.Entities.Process;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;

using static App.Domain.Enums.BarcodeEnums;
using static App.Domain.Enums.Enums;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations;
using App.Domain.Entities;
using App.Domain.Enums;
using Newtonsoft.Json.Schema;

namespace App.Infrastructure.Persistence.Seed
{
    public static class DBSeeder
    {
        //public static void SeedDB(ApplicationOracleDbContext context)
        //{
        //    //context.Database.EnsureCreated();
        //    //context.Database.Migrate();
        //    context.Database.EnsureCreatedAsync();
        //    context.Database.MigrateAsync();
        //}
        public static async void SeedDB(ClientSqlDbContext context)
        {
            //addDefaultColor(context);
            //addDefaultStoreplaces(context);
            //addDefaultJobs(context);
            //addDefaultSize(context);
            //addDefaultUnit(context);
            //addDefaultStore(context);
            //addDefaultCategory(context);
            //addDefaultEmployee(context);
            //addOtherAuthorities(context);
            //addDefaultSettings(context);
            //addDefaultSalesMan(context);
            //addDefaultSupplier(context);
            //addDefaultCustomer(context);
            //addDefaultPaymentMethod(context);
            //addDefaultItemCardMaster(context);
            //addDefaultCompanyData(context);
            //addDefaultBarCodeTemplateData(context);
            //addGLGeneralSettingSubLevelsDefultValues(context);
            //purchasesAndSalesGLsettingDefult(context);
        }

     

        private static void addDefaultColor(ClientSqlDbContext context)
        {
            if (!context.colors.Any(e => e.Code == 1))
            {
                //code to check if the color is empty
                var color = new InvColors()
                {
                    Code = 1,
                    ArabicName = "أحمر",
                    LatinName = "Red",
                    Color = "#FF0000",
                    Status = (int)Status.Active
                };
                context.colors.Add(color);
            }
            context.SaveChanges();
        }
        private static void addDefaultSize(ClientSqlDbContext context)
        {
            if (!context.sizes.Any(e => e.Code == 1))
            {
                var size = new InvSizes()
                {
                    Code = 1,
                    ArabicName = "صغير",
                    LatinName = "Small",
                    Status = (int)Status.Active,
                    CanDelete = false
                };
                context.sizes.Add(size);
            }
            context.SaveChanges();
        }
        private static void addDefaultUnit(ClientSqlDbContext context)
        {
            if (!context.units.Any(e => e.Code == 1))
            {
                var unit = new InvStpUnits()
                {
                    Code = 1,
                    ArabicName = "حبة",
                    LatinName = "Piece",
                    Status = (int)Status.Active,
                };
                context.units.Add(unit);
            }
            context.SaveChanges();
        }
        private static void addDefaultJobs(ClientSqlDbContext context)
        {
            if (!context.jobs.Any(e => e.Code == 1))
            {
                var job = new InvJobs()
                {
                    Code = 1,
                    ArabicName = "مدير النظام",
                    LatinName = "Administrator",
                    Status = (int)Status.Active,
                };
                context.jobs.Add(job);
            }
            context.SaveChanges();
        }
        private static void addDefaultStoreplaces(ClientSqlDbContext context)
        {
            if (!context.storePlaces.Any(e => e.Code == 1))
            {
                var storePlace = new InvStorePlaces()
                {
                    Code = 1,
                    ArabicName = "رف1",
                    LatinName = "Rack 1",
                    Status = (int)Status.Active,
                };
                context.storePlaces.Add(storePlace);
            }
            context.SaveChanges();
        }
        private static void addDefaultStore(ClientSqlDbContext context)
        {
            if (!context.stores.Any(e => e.Code == 1))
            {
                var store = new InvStpStores()
                {
                    Code = 1,
                    ArabicName = "مستودع رئيسي",
                    LatinName = "Main store",
                    Status = (int)Status.Active
                };
                context.stores.Add(store);

                context.SaveChanges();

                var StoreBranch = new InvStoreBranch()
                {
                    BranchId = 1,
                    StoreId = 1
                };
                context.StoreBranches.Add(StoreBranch);
                context.SaveChanges();
            }

        }
        private static void addDefaultCategory(ClientSqlDbContext context)
        {
            if (!context.categories.Any(e => e.Code == 1))
            {
                var category = new InvCategories()
                {

                    Code = 1,
                    ArabicName = "مجموعة افتراضية",
                    LatinName = "Default category",
                    Status = (int)Status.Active,
                    Color = "#040404",
                    UsedInSales = 1
                };
                context.categories.Add(category);
            }
            context.SaveChanges();
        }
        private static void addDefaultEmployee(ClientSqlDbContext context)
        {
            if (!context.employees.Any(e => e.Code == 1))
            {
                var employee = new InvEmployees()
                {
                    Code = 1,
                    ArabicName = "موظف افتراضي",
                    LatinName = "Default employee",
                    Status = (int)Status.Active,
                    JobId = 1

                };
                context.employees.Add(employee);
                var employeeCasher = new InvEmployees()
                {
                    Code = 2,
                    ArabicName = "كاشير افتراضي",
                    LatinName = "Default Casher",
                    Status = (int)Status.Active,
                    JobId = 1

                };
                context.employees.Add(employeeCasher);
            }
            context.SaveChanges();
        }
        private static void addDefaultSettings(ClientSqlDbContext context)
        {
            if (!context.invGeneralSettings.Any(e => e.Id == 1))
            {
                var GS = new InvGeneralSettings()
                {
                    Purchases_ModifyPrices = true,//السماح بتعديل الأسعار
                    Purchases_PayTotalNet = false,//يجب أن تسدد قيمة المستند كاملة
                    Purchases_UseLastPrice = false,//استخدام آخر سعر شراء للمورد
                    Purchases_PriceIncludeVat = true,//السعر يشمل الضريبة
                    Purchases_PrintWithSave = false,//الطباعة عند الحفظ
                    Purchases_ReturnWithoutQuantity = false,//استرجاع بدون رصيد
                    Purchases_ActiveDiscount = true,//تفعيل الخصم
                    Purchase_UpdateItemsPricesAfterInvoice = false, // تحديث أسعار المنتجات بعد فاتورة المشتريات

                    //نقاط البيع
                    Pos_ModifyPrices = true,//السماح بتعديل أسعار البيع
                    //اسم غير واضح عايز يتراجع
                    Pos_ModifyPricesType = (int)ModifySalePricesType.allowModifyByIncreaseUntilPrice4,//السماح بتجاوز اسعار البيع

                    Pos_ExceedDiscountRatio = true,//السماح بتجاوز نسبة الخصم
                 //   Pos_UseLastPrice = false,//استخدام أخر سعر بيع للعميل
                    Pos_ActivePricesList = false,//تنشيط قوائم الأسعار
                    Pos_ExtractWithoutQuantity = false,//الصرف بدون رصيد
                    Pos_PriceIncludeVat = true,//السعر يشمل الضريبة
                    Pos_ActiveDiscount = true,//تفعيل الخصم
                    Pos_DeferredSale = false,//البيع بالآجل
                    Pos_IndividualCoding = true,//ترقيم مستقل
                    Pos_PreventEditingRecieptFlag = false,//منع تعديل الفاتورة
                    Pos_PreventEditingRecieptValue = 2,//عدد الدقائق اللازمة لمنع تعديل الفاتورة
                    Pos_ActiveCashierCustody = false,//تفعيل عهدة الكاشير
                    Pos_PrintPreview = false,//معاينة قبل الطباعة
                    Pos_PrintWithEnding = false,//الطباعة عند إنهاء الطلب
                    Pos_EditingOnDate = true,//التعديل على التاريخ

                    // المبيعات
                    Sales_ModifyPrices = true,//السماح بتعديل أسعار البيع
                    Sales_ModifyPricesType = (int)ModifySalePricesType.allowModifyByIncreaseUntilPrice4,//السماح بتجاوز أسعار البيع
                    Sales_ExceedDiscountRatio = true,// السماح بتجاوز نسبة الخصم
                    Sales_PayTotalNet = false,// يجب أن تسدد قيمة المستند كاملة
                  //  Sales_UseLastPrice = false,//استخدام آخر سعر بيع للعميل
                    Sales_ExtractWithoutQuantity = false,//الصرف بدون رصيد
                    Sales_PriceIncludeVat = true,//السعر يشمل الضريبة
                    Sales_PrintWithSave = false,// الطباعة عند الحفظ
                    Sales_ActiveDiscount = true,//تفعيل الخصم
                    Sales_LinkRepresentCustomer = true,//ربط المندوب بالعميل
                    Sales_ActivePricesList = false,//تنشيط قوائم الأسعار


                    //أخرى
                    Other_MergeItems = true,//دمج الأصناف
                    otherMergeItemMethod = "withSave",//دمج مع الحفظ
                                                      //دمج مع الادخال
                    Other_ItemsAutoCoding = false,//تكويد آلي للأصناف
                    Other_ZeroPricesInItems = false,//السماح بالأسعار الصفرية بكارت الصنف
                    Other_PrintSerials = true, //طباعة سيريالات الأصناف
                    Other_AutoExtractExpireDate = false,//الصرف الآلي لتاريخ الصلاحية
                    Other_ViewStorePlace = false,//إظهار مكان التخزين في المبيعات ونقاط البيع
                    Other_ConfirmeSupplierPhone = false,//التأكيد من تسجيل رقم الهاتف للموردين
                    Other_ConfirmeCustomerPhone = false,//التأكد من تسجيل رقم الهاتف للعملاء
                    Other_DemandLimitNotification = true,//التنبيه بحد الطلب
                    Other_ExpireNotificationFlag = true,//تنبيه انتهاء الصلاحية
                    Other_ExpireNotificationValue = 30,//عدد أيام التنبيه على انتهاء الصلاحية
                    Other_Decimals = 6,//العلامات العشرية
                    Other_ShowBalanceOfPerson = false, // اظهار رصيد الموردين والعملاء فى الفواتير
                    // الأرصدة
                    Funds_Items = false,//إغلاق أرصدة أول مدة أصناف
                    Funds_Supplires = false,//إغلاق أرصدة أول المدة موردين
                    Funds_Customers = false,//إغلاق أرصدة أول المدة عملاء
                    Funds_Safes = false,//إغلاق أرصدة أول المدة خزائن
                    Funds_Banks = false,//إغلاق أرصدة أول المدة بنوك
                    Funds_Customers_Date = DateTime.Now,
                    Funds_Supplires_Date = DateTime.Now,

                    //الباركود
                    barcodeType = "weight",//نوع الميران وزن
                                           // نوع الميزان تكلفة
                    Barcode_ItemCodestart = false,//عدم وجود رقم مبدئي

                    //القيمة المضافة
                    Vat_Active = true,//تفعيل الضريبة
                    Vat_DefaultValue = 15,//القيمة الافتراضية للضريبة

                    // الاعتماد
                    Accredite_StartPeriod = Convert.ToDateTime(DateTime.Now.Year + "/1/1"),//بداية الفترة المالية
                    Accredite_EndPeriod = Convert.ToDateTime(DateTime.Now.Year + "/12/31"),//نهاية الفترة المالية

                    // واجهة العملاء 
                    CustomerDisplay_Active = true,//تنشيط واجهة العملاء
                    CustomerDisplay_PortNumber = "0",//رقم البورت
                    CustomerDisplay_Code = 0,//الكود
                    CustomerDisplay_LinesNumber = 0,//عدد السطور
                    CustomerDisplay_CharNumber = 0,//عدد الحروف
                    CustomerDisplay_DefaultWord = "",//الكلمة الافتتاحية
                    CustomerDisplay_ScreenType = (int)DocumentType.POS//نوع الشاشة

                };
                context.invGeneralSettings.Add(GS);

                context.SaveChanges();
            }

        }
        private static void addDefaultSalesMan(ClientSqlDbContext context)
        {
            if (!context.salesman.Any(e => e.Code == 1))
            {
                var salesman = new InvSalesMan()
                {
                    Code = 1,
                    ArabicName = "مندوب مبيعات افتراضي",
                    LatinName = "Default salesman",
                    Phone = "",
                    Email = "",
                    ApplyToCommissions = false,
                    CommissionListId = null
                    //BranchId = 1

                };
                context.salesman.Add(salesman);
                context.SaveChanges();
                var SalesManBranch = new InvSalesMan_Branches()
                {
                    BranchId = 1,
                    SalesManId = 1
                };
                context.SalesManBranches.Add(SalesManBranch);
            }
            context.SaveChanges();
        }
        private static void addDefaultSupplier(ClientSqlDbContext context)
        {
            if (!context.person.Any(e => e.Code == 1))
            {
                var person = new InvPersons()
                {
                    Code = 1,
                    ArabicName = "مورد نقدي",
                    LatinName = "Cash Supplier",
                    Type = (int)PersonType.Normal,
                    Status = (int)Status.Active,
                    SalesManId = null,
                    ResponsibleAr = "",
                    ResponsibleEn = "",
                    Phone = "",
                    Fax = "",
                    Email = "",
                    TaxNumber = "",
                    AddressAr = "",
                    AddressEn = "",
                    CreditLimit = 0,
                    CreditPeriod = 0,
                    DiscountRatio = 0,
                    SalesPriceId = 0,
                    LessSalesPriceId = 0,
                    IsCustomer = false,
                    IsSupplier = true,
                    CanDelete = false,
                    AddToAnotherList = false,
                    CodeT = "S",
                    FinancialAccountId = 35
                };
                context.person.Add(person);
                context.SaveChanges();
                var PersonBranch = new InvPersons_Branches()
                {
                    BranchId = 1,
                    PersonId = 1
                };
                context.PersonsBranches.Add(PersonBranch);
                var FundsCustomerSupplier = new InvFundsCustomerSupplier()//By wesal
                {
                    PersonId = person.Id,
                    Credit = 0,
                    Debit = 0,

                };
                context.invFundsCustomerSuppliers.Add(FundsCustomerSupplier);
            }
            context.SaveChanges();
        }
        private static void addDefaultCustomer(ClientSqlDbContext context)
        {
            if (!context.person.Any(e => e.Id == 2))
            {
                var person = new InvPersons()
                {
                    Code = 1,
                    ArabicName = "عميل نقدي",
                    LatinName = "Cash Customer",
                    Type = (int)PersonType.Normal,
                    Status = (int)Status.Active,
                    SalesManId = 1,
                    ResponsibleAr = "",
                    ResponsibleEn = "",
                    Phone = "2",
                    Fax = "2",
                    Email = "2",
                    TaxNumber = "",
                    AddressAr = "",
                    AddressEn = "",
                    CreditLimit = 0,
                    CreditPeriod = 0,
                    DiscountRatio = 0,
                    SalesPriceId = (int)SalePricesList.SalePrice1,
                    LessSalesPriceId = (int)SalePricesList.SalePrice1,
                    IsCustomer = true,
                    IsSupplier = false,
                    CanDelete = false,
                    AddToAnotherList = false,
                    CodeT = "C",
                    FinancialAccountId = 25
                };
                context.person.AddRange(person);
                context.SaveChanges();
                var PersonBranch = new InvPersons_Branches()
                {
                    BranchId = 1,
                    PersonId = person.Id
                };
                context.PersonsBranches.Add(PersonBranch);
                context.SaveChanges();
                var FundsCustomerSupplier = new InvFundsCustomerSupplier()//By wesal
                {
                    PersonId = person.Id,
                    Credit = 0,
                    Debit = 0,
                };
                context.invFundsCustomerSuppliers.AddRange(FundsCustomerSupplier);
                context.SaveChanges();

            }

        }
        private static void addDefaultPaymentMethod(ClientSqlDbContext context)
        {
            if (!context.paymentMethod.Any(e => e.PaymentMethodId == 1))
            {
                var paymentMethod = new InvPaymentMethods()
                {
                    Code = 1,
                    ArabicName = "نقدي",
                    LatinName = "Cash",
                    SafeId = 1,
                    BankId = null,
                    Status = (int)Status.Active,
                };
                context.paymentMethod.Add(paymentMethod);
                context.SaveChanges();
                paymentMethod = new InvPaymentMethods()
                {
                    Code = 2,
                    ArabicName = "شبكة",
                    LatinName = "Net",
                    SafeId = null,
                    BankId = 1,
                    Status = (int)Status.Active,
                };
                context.paymentMethod.Add(paymentMethod);
                context.SaveChanges();
                paymentMethod = new InvPaymentMethods()
                {
                    Code = 3,
                    ArabicName = "شيك",
                    LatinName = "Chique",
                    SafeId = 1,
                    BankId = null,
                    Status = (int)Status.Active,
                };
                context.paymentMethod.Add(paymentMethod);
                context.SaveChanges();
            }
        }
        private static void addDefaultItemCardMaster(ClientSqlDbContext context)
        {
            if (!context.itemCards.Any(e => e.Id == 1))
            {
                var defItemCard = new InvStpItemCardMaster()
                {
                    ItemCode = "1",
                    LatinName = "كارت الصنف الافتراضى",
                    ArabicName = "كارت الصنف الافتراضى",
                    GroupId = 1,
                    Status = 1,
                    UsedInSales = true,
                    TypeId = 1,
                    ApplyVAT = false,
                    VAT = 0,
                    BranchId = 1,
                    ReportUnit = 1,
                    WithdrawUnit = 1,
                    DepositeUnit = 1
                };
                context.itemCards.Add(defItemCard);
                context.SaveChanges();


                var itemcardDefaultUnit = new InvStpItemCardUnit()
                {
                    Barcode = "",
                    ConversionFactor = 1,
                    ItemId = 1,
                    SalePrice1 = 0,
                    SalePrice2 = 0,
                    SalePrice3 = 0,
                    SalePrice4 = 0,
                    UnitId = 1,


                };
                context.InvStpItemCardUnits.Add(itemcardDefaultUnit);
                context.SaveChanges();
            }

        }
        private static void addDefaultCompanyData(ClientSqlDbContext context)
        {
            if (!context.companyData.Any(e => e.Id == 1))
            {
                var DefCompanyData = new InvCompanyData()
                {
                    ArabicName = "اسم الشركة الافتراضي",
                    LatinName = "Default company name"

                };
                context.companyData.Add(DefCompanyData);
            }
            context.SaveChanges();
        }
        private static void addDefaultBarCodeTemplateData(ClientSqlDbContext context)
        {
            if (!context.invBarcodeTemplates.Any(e => e.ArabicName == "باركود افتراضي"))
            {
                var DefCompanyData = new InvBarcodeTemplate()
                {
                    ArabicName = "باركود افتراضي",
                    LatinName = "Default Barcode",
                    Code = 1,
                    IsDefault = true,
                    CanDelete = false,
                };
                context.invBarcodeTemplates.Add(DefCompanyData);
                context.SaveChanges();


                if (!context.invBarcodeItems.Any(e => e.BarcodeId == DefCompanyData.BarcodeId))
                {
                    var DefCompanyDataItems = new InvBarcodeItems()
                    {
                        BarcodeId = DefCompanyData.BarcodeId,
                        BarcodeItemType = (int)BarcodeItemType.Container,
                        Width = 300,
                        Height = 300,
                        PositionX = 10,
                        PositionY = 10,
                        FontType = "",
                        FontSize = 16,
                        Bold = false,
                        Italic = false,
                        UnderLine = false,
                        AlignX = "center",
                        AlignY = "center",
                        Dock = "right",
                        Image = null,
                        ImageName = null,
                        IsLogo = false,
                        Direction = 10,
                        TextType = 0,
                        BeginSplitter = "",
                        EndSplitter = "",
                        BarcodeType = (int)BarcodeType.Code128
                    };
                    context.invBarcodeItems.Add(DefCompanyDataItems);
                    context.SaveChanges();
                }
            }
        }
        private static void addGLGeneralSettingSubLevelsDefultValues(ClientSqlDbContext context)
        {
            if (!context.gLGeneralSettings.FirstOrDefault().subCodeLevels.Any())
            {
                var generalSettings = context.gLGeneralSettings.FirstOrDefault();
                var subLevels = new List<SubCodeLevels>();
                subLevels.Add(new SubCodeLevels() { value = 2 });
                subLevels.Add(new SubCodeLevels() { value = 3 });
                subLevels.Add(new SubCodeLevels() { value = 3 });
                subLevels.Add(new SubCodeLevels() { value = 3 });
                foreach (var item in subLevels)
                {
                    generalSettings.subCodeLevels.Add(item);
                    context.SaveChanges();
                }
            }
            context.SaveChanges();
        }
        //purchasesAndSalesGLsetting
        //private static void purchasesAndSalesGLsettingDefult(ClientSqlDbContext context)
        //{
        //    if (!context.GLPurchasesAndSalesSettings.Any(e => e.Id == 1))
        //    {
        //        List<GLPurchasesAndSalesSettings> Settings = new List<GLPurchasesAndSalesSettings>();
        //        Settings.Add(new GLPurchasesAndSalesSettings()
        //        {
        //            ArabicName = purAndSalesSettingNames.PurchaseAR,
        //            LatinName = purAndSalesSettingNames.PurchaseEN,
        //            MainType = (int)MainInvoiceType.Purchases, // sales or purchase
        //            RecieptsType = (int)DocumentType.Purchase,
        //            ReceiptElemntID = 1,//the type of  reciepts if  discount or vat 
        //            FinancialAccountId = 27
        //        });
        //        //
        //        Settings.Add(new GLPurchasesAndSalesSettings()
        //        {
        //            ArabicName = purAndSalesSettingNames.PurchaseReturnAR,
        //            LatinName = purAndSalesSettingNames.PurchaseReturnEN,
        //            MainType = (int)MainInvoiceType.Purchases,
        //            RecieptsType = (int)DocumentType.ReturnPurchase,
        //            ReceiptElemntID = 1,
        //            FinancialAccountId = 27
        //        });
        //        //
        //        Settings.Add(new GLPurchasesAndSalesSettings()
        //        {
        //            ArabicName = purAndSalesSettingNames.PurchaseWithoutVatAR,
        //            LatinName = purAndSalesSettingNames.PurchaseWithoutVatEN,
        //            MainType = (int)MainInvoiceType.Purchases,
        //            RecieptsType = (int)DocumentType.wov_purchase,
        //            ReceiptElemntID = 1,
        //            FinancialAccountId = 27
        //        });
        //        //
        //        Settings.Add(new GLPurchasesAndSalesSettings()
        //        {
        //            ArabicName = purAndSalesSettingNames.PurchaseWithoutVatReturnAR,
        //            LatinName = purAndSalesSettingNames.PurchaseWithoutVatReturnEN,
        //            MainType = (int)MainInvoiceType.Purchases,
        //            RecieptsType = (int)DocumentType.wov_purchase_R,
        //            ReceiptElemntID = 1,
        //            FinancialAccountId = 27
        //        });
        //        //
        //        Settings.Add(new GLPurchasesAndSalesSettings()
        //        {
        //            ArabicName = purAndSalesSettingNames.DiscountPurAR,
        //            LatinName = purAndSalesSettingNames.DiscountPurEN,
        //            MainType = (int)MainInvoiceType.Purchases,
        //            RecieptsType = (int)DocumentType.AcquiredDiscount,
        //            ReceiptElemntID = 1,
        //            FinancialAccountId = 59
        //        });

        //        //
        //        Settings.Add(new GLPurchasesAndSalesSettings()
        //        {
        //            ArabicName = purAndSalesSettingNames.VATpurchasesAR,
        //            LatinName = purAndSalesSettingNames.VATpurchasesEN,
        //            MainType = (int)MainInvoiceType.Purchases,
        //            RecieptsType = (int)DocumentType.VATPurchase,
        //            ReceiptElemntID = 1,
        //            FinancialAccountId = 37
        //        });
        //        //
        //        Settings.Add(new GLPurchasesAndSalesSettings()
        //        {
        //            ArabicName = purAndSalesSettingNames.SalesAR,
        //            LatinName = purAndSalesSettingNames.SalesEN,
        //            MainType = (int)MainInvoiceType.Sales,
        //            RecieptsType = (int)DocumentType.Sales,
        //            ReceiptElemntID = 1,
        //            FinancialAccountId = 50
        //        });
        //        //
        //        Settings.Add(new GLPurchasesAndSalesSettings()
        //        {
        //            ArabicName = purAndSalesSettingNames.SalesReturnAR,
        //            LatinName = purAndSalesSettingNames.SalesReturnEN,
        //            MainType = (int)MainInvoiceType.Sales,
        //            RecieptsType = (int)DocumentType.ReturnSales,
        //            ReceiptElemntID = 1,
        //            FinancialAccountId = 51
        //        });

        //        //
        //        Settings.Add(new GLPurchasesAndSalesSettings()
        //        {
        //            ArabicName = purAndSalesSettingNames.POSAR,
        //            LatinName = purAndSalesSettingNames.POSEN,
        //            MainType = (int)MainInvoiceType.Sales,
        //            RecieptsType = (int)DocumentType.POS,
        //            ReceiptElemntID = 1,
        //            FinancialAccountId = 50
        //        });
        //        //
        //        Settings.Add(new GLPurchasesAndSalesSettings()
        //        {
        //            ArabicName = purAndSalesSettingNames.POSreturnAR,
        //            LatinName = purAndSalesSettingNames.POSreturnEN,
        //            MainType = (int)MainInvoiceType.Sales,
        //            RecieptsType = (int)DocumentType.ReturnPOS,
        //            ReceiptElemntID = 1,
        //            FinancialAccountId = 51
        //        });
        //        //
        //        Settings.Add(new GLPurchasesAndSalesSettings()
        //        {
        //            ArabicName = purAndSalesSettingNames.VATsalesAR,
        //            LatinName = purAndSalesSettingNames.VATsalesEN,
        //            MainType = (int)MainInvoiceType.Sales,
        //            RecieptsType = (int)DocumentType.VATSale,
        //            ReceiptElemntID = 1,
        //            FinancialAccountId = 37
        //        });

        //        //
        //        Settings.Add(new GLPurchasesAndSalesSettings()
        //        {
        //            ArabicName = purAndSalesSettingNames.DiscountSalesAR,
        //            LatinName = purAndSalesSettingNames.DiscountSalesEN,
        //            MainType = (int)MainInvoiceType.Sales,
        //            RecieptsType = (int)DocumentType.PermittedDiscount,
        //            ReceiptElemntID = 1,
        //            FinancialAccountId= 53
        //        });


        //        //Settlements
        //        Settings.Add(new GLPurchasesAndSalesSettings()
        //        {
        //            ArabicName = purAndSalesSettingNames.SettlementsAddPermissionAr,
        //            LatinName = purAndSalesSettingNames.SettlementsAddPermissionEn,
        //            MainType = (int)MainInvoiceType.Settlements,
        //            RecieptsType = (int)DocumentType.AddPermission,
        //            ReceiptElemntID = 1,
        //            FinancialAccountId = 38
        //        });
        //        //Settlements
        //        Settings.Add(new GLPurchasesAndSalesSettings()
        //        {
        //            ArabicName = purAndSalesSettingNames.SettlementsExtractPermissionAr,
        //            LatinName = purAndSalesSettingNames.SettlementsExtractPermissionEn,
        //            MainType = (int)MainInvoiceType.Settlements,
        //            RecieptsType = (int)DocumentType.ExtractPermission,
        //            ReceiptElemntID = 1,
        //            FinancialAccountId = 27
        //        });
        //        //Settlements
        //        Settings.Add(new GLPurchasesAndSalesSettings()
        //        {
        //            ArabicName = purAndSalesSettingNames.SettlementsstockBalanceExcessiveAr,
        //            LatinName = purAndSalesSettingNames.SettlementsstockBalanceExcessiveEn,
        //            MainType = (int)MainInvoiceType.Settlements,
        //            RecieptsType = (int)DocumentType.stockBalanceExcessive,
        //            ReceiptElemntID = 1,
        //            FinancialAccountId = 58
        //        });
        //        //Settlements
        //        Settings.Add(new GLPurchasesAndSalesSettings()
        //        {
        //            ArabicName = purAndSalesSettingNames.SettlementsStockBalanceDeficitAr,
        //            LatinName = purAndSalesSettingNames.SettlementsStockBalanceDeficitEn,
        //            MainType = (int)MainInvoiceType.Settlements,
        //            RecieptsType = (int)DocumentType.stockBalanceDeficit,
        //            ReceiptElemntID = 1,
        //            FinancialAccountId = 58
        //        });




        //        //Funds
        //        Settings.Add(new GLPurchasesAndSalesSettings()
        //        {
        //            ArabicName = purAndSalesSettingNames.FundsSafesAr,
        //            LatinName = purAndSalesSettingNames.FundsSafesEn,
        //            MainType = (int)MainInvoiceType.funds,
        //            RecieptsType = (int)DocumentType.SafeFunds,
        //            ReceiptElemntID = 1,
        //            FinancialAccountId = 53
        //        });
        //        Settings.Add(new GLPurchasesAndSalesSettings()
        //        {
        //            ArabicName = purAndSalesSettingNames.FundsBanksAr,
        //            LatinName = purAndSalesSettingNames.FundsBanksEn,
        //            MainType = (int)MainInvoiceType.funds,
        //            RecieptsType = (int)DocumentType.BankFunds,
        //            ReceiptElemntID = 4,
        //            FinancialAccountId = 38
        //        });
        //        Settings.Add(new GLPurchasesAndSalesSettings()
        //        {
        //            ArabicName = purAndSalesSettingNames.FundsCustomersAr,
        //            LatinName = purAndSalesSettingNames.FundsCustomersEn,
        //            MainType = (int)MainInvoiceType.funds,
        //            RecieptsType = (int)DocumentType.CustomerFunds,
        //            ReceiptElemntID = 1,
        //            FinancialAccountId = 38
        //        });
        //        Settings.Add(new GLPurchasesAndSalesSettings()
        //        {
        //            ArabicName = purAndSalesSettingNames.FundsSuppliersAr,
        //            LatinName = purAndSalesSettingNames.FundsSuppliersEn,
        //            MainType = (int)MainInvoiceType.funds,
        //            RecieptsType = (int)DocumentType.SuplierFunds,
        //            ReceiptElemntID = 1,
        //            FinancialAccountId = 27
        //        });
        //        Settings.Add(new GLPurchasesAndSalesSettings()
        //        {
        //            ArabicName = purAndSalesSettingNames.FundsItemsAr,
        //            LatinName = purAndSalesSettingNames.FundsItemsEn,
        //            MainType = (int)MainInvoiceType.funds,
        //            RecieptsType = (int)DocumentType.itemsFund,
        //            ReceiptElemntID = 1,
        //            FinancialAccountId = 38
        //        });

        //        context.AddRange(Settings);
        //        context.SaveChanges();


        //    }


        //}
        //private static void addOtherAuthorities(ClientSqlDbContext context)
        //{
        //    if (!context.OtherAuthorities.Any())
        //    {
        //        var list = new List<GLOtherAuthorities>();
        //        list.Add(new GLOtherAuthorities
        //        {
        //ArabicName = "ضريبةالقيمة المضافة",
        //            LatinName = "Vat",
        //            Code = 1,
        //            BranchId = 1,
        //            CanDelete = false,
        //            FinancialAccountId = 37,
        //            Status = 1
        //        });
        //        list.Add(new GLOtherAuthorities
        //        {
        //ArabicName = "الكهرباء",
        //            LatinName = "Electricity bill",
        //            Code = 2,
        //            BranchId = 1,
        //            CanDelete = true,
        //            FinancialAccountId = 37,
        //            Status = 1
        //        });
        //        list.Add(new GLOtherAuthorities
        //        {
        //ArabicName = "مياه",
        //            LatinName = "Water bill",
        //            Code = 3,
        //            BranchId = 1,
        //            CanDelete = true,
        //            FinancialAccountId = 37,
        //            Status = 1
        //        });
        //        context.AddRange(list);
        //        context.SaveChanges();
        //    }
        //}
        private static void addFundsEntry(ClientSqlDbContext context, MigrationBuilder migrationBuilder)
        {
            if(!context.gLJournalEntries.Where(x=> x.Id == -1).Any())
            {
                migrationBuilder.InsertData(
                    table: "GLJournalEntry",
                    columns: new[] { "Id", "Code", "CurrencyId", "IsDeleted", "CreditTotal", "DebitTotal", "FTDate", "Notes", "IsBlock", "IsTransfer", "BrowserName", "Auto", "SupportId", "BranchId" },
                    values: new object[,]
                    {
                        { -1,1,1,false,0,0,DateTime.Now,"",false,false,true,1,1}
                    });
            }
        }
       
    }
}
