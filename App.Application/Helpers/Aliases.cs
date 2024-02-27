using App.Domain.Models.Security.Authentication.Response;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Collections.Generic;
using System.IO;
using static App.Domain.Enums.Enums;

namespace App.Application.Helpers
{
    //Adding comments to Aliases
    public class Aliases
    {
        public class TemporaryRequiredData
        {
            //test from my device
            public static string UserName = "Admin";
            public static int BranchId = 1;
        }
        public class InvoiceTransaction
        {
            public static string PurchaseAr = " فاتورة مشتريات";
            public static string PurchaseEn = " purchase invoice";
            public static string ReturnPurchaseAr = "مرتجع مشتريات";
            public static string ReturnPurchaseEn = "Return Purchases";


            public static string SalesAr = " فاتورة مبيعات";
            public static string SalesEn = " Sales Invoice";

            public static string returnSalesAr = " فاتورة مرتجع مبيعات";
            public static string returnSalesEn = " Return Sales Invoice";

            public static string POSAr = " فاتورة نقاط بيع";
            public static string POSEn = " POS Invoice";
            
            public static string returnPOSAr = " فاتورة مرتجع نقاط بيع";
            public static string returnPOSEn = " Return POS Invoice";

            public static string addPermissionAr = " اذن اضافة";
            public static string addPermissionEn = " Add Permission";
            public static string ExtractPermissionAr = " اذن صرف";
            public static string ExtractPermissionEn = " Extract Permission";

            public static string WOVPurchaseAr = " فاتورة مشتريات بدون ضريبة";
            public static string WOVPurchaseEn = " purchase invoice without vat";
            public static string WOVReturnPurchaseAr = "مرتجع مشتريات بدون ضريبة ";
            public static string WOVReturnPurchaseEn = "Return Purchases  without vat";


        }

        public class InvoicesCode
        {
            public static string AddPermission = "AD";  // اذن اضافة
            public static string DeleteAddPermission = "AD-D";   // حذف اذن اضافة
            public static string ExtractPermission = "EX"; // اذن صرف
            public static string DeleteExtractPermission = "EX-D"; // حذف اذن صرف
            public static string Purchase = "P";  // مشتريات
            public static string ReturnPurchase = "RP"; // مرتجع مشتريات
            public static string DeletePurchase = "P-D"; // حذف مشتريات
            public static string Sales = "S"; // مبيعات
            public static string ReturnSales = "RS"; // مرتحع مبيعات
            public static string DeleteSales = "S-D"; // حذف مبيعات
            public static string POS = "POS"; // نقاط بيع
            public static string BindingPOS = "BIND";
            public static string ReturnPOS = "RPOS"; // مرتحع نقاط بيع
            public static string DeletePOS = "POS-D"; // حذف نقاط بيع
            public static string AcquiredDiscount = "DA"; // خصم مكتسب
            public static string DeleteAcquiredDiscount = "DA-D";  //  حذف خصم  مكتسب
            public static string PermittedDiscount = "DP"; //  خصم مسموح به
            public static string DeletePermittedDiscount = "DP-D"; // حذف خصم مسموح به
            public static string ItemsFund = "FN"; // أرصدة أول مده
            public static string customerFund = "CF"; //  عملا أرصدة أول مده
            public static string SupplierFund = "SF"; // أرصدة أول مده موردين
            public static string DeleteItemsFund = "FN-D"; // حذف أرصدة أول مده
            public static string SafeCash = "SC";// سند قبض خزينة
            public static string SafePayment = "SP";// سند صرف خزينه
            public static string SafeTransfer = "ST";// سند تحويل خزينه
            public static string BankCash = "BC";// سند قبض بنكي
            public static string BankPayment = "BP";// سند صرف بنكي
            public static string BankTransfer = "BT";// سند تحويل بنكي
            public static string CompinedBankPayment = "CBP";// سند مجمع صرف بنكي
            public static string CompinedBankCash = "CBC";// سند مجمع قبض بنكي
            public static string CompinedSafePayment = "CSP";// سند مجمع صرف خزينه
            public static string CompinedSafeCash = "CSC";// سند مجمع قبض خزينه
            public static string OutgoingTransfer = "TO";//Transfer Outgoing تحويل صادر
            public static string DeletedOutgoingTransfer = "TO-D";//Transfer Outgoing تحويل صادر
            public static string IncomingTransfer = "TI";//Transfer Incomming تحويل وارد
            public static string OfferPrice = "OP"; // عرض سعر
            public static string DeleteOfferPrice = "OP-D";// حذف عرض السعر 
            public static string PurchaseOrder = "PO";// امر الشراء
            public static string DeletePurchaseOrder = "PO-D";//  حذف امر الشراء
            public static string WOV_Purchase = "PV";  //  مشتريات بدون ضريبه 
            public static string ReturnWOV_Purchase = "RPV";  // مرتجع مشتريات بدون ضريبه  
            public static string DeleteWOV_Purchase = "PV-D";  // حذف مشتريات بدون ضريبه
            public static string SafeMultiCollectionReceipts = "SM";  // سند مجمع خزائن
            public static string BankMultiCollectionReceipts = "BM";  // سند مجمع بنوك

        }

        public class Actions
        {
            
            public static string UnAuthorized = "UnAuthorized";
            public static string IdIsRequired = "Id is required";
            public static string BranchIdIsRequired = "Branch Id is required";
            public static string userDoseNotHaveAccessToThisBranch = "user Dose Not Have Access To This Branch";
            public static string ArabicNameExist = "Arabic name exist";
            public static string EnglishNameExist = "English name exist";
            public static string NameIsRequired = "Name is required";
            public static string SavedSuccessfully = "Saved successfully";
            public static string SaveFailed = "Save failed";
            public static string UpdatedSuccessfully = "Updated successfully";
            public static string UpdateFailed = "Update failed";
            public static string CanNotBeUpdated = "Can not be updated";
            public static string DeletedSuccessfully = "Deleted successfully";
            public static string DeleteFailed = "Delete failed";
            public static string CanNotBeDeleted = "Can not be deleted";
            public static string InvalidStatus = "Invalid status";
            public static string ExceptionOccurred = "Exception occurred";
            public static string Success = "Success";
            public static string NotFound = "Not found";
            public static string Exist = "Exist";
            public static string PhoneExist = "Phone exist";
            public static string EmailExist = "Email exist";
            public static string JobIsRequired = "Job is required";
            public static string BranchIsRequired = "Branch is required";
            public static string TypeIsRequired = "Type is required";
            public static string PhoneDigitLessThan7 = "Phone digit less than 7";
            public static string PhoneIsRequired = "Phone number is required";
            public static string ItemsIsReuired = "Items is reuired";
            public static string CanNotMakeSupplierAsCustomer = "Can not make supplier as customer";
            public static string CanNotMakeCustomerAsSupplier = "Can not make customer as supplier";
            public static string SlideToLessThanSlideFrom = "Slide to less than slide from";
            public static string ErrorInCalculations = "Error in calculations";
            public static string ErrorOccurred = "Error occurred";
            public static string ItemInactive = "Item inactive";
            public static string CanNotAddCompositeItem = "Can not add composite item";
            public static string EndOfData = "End of data";
            public static string DeletedInvoice = "Deleted invoice";
            public static string ReturnedInvoice = "Returned invoice";
            public static string userNameIsExist = "Username exist";
            public static string PasswordExist = "Password exist";
            public static string invalidEmailFormat = "invalid Email Format";
            public static string DefultDataCanNotbeEdited = "Defult Data Can Not be Edited";
            public static string DefultDataCanNotbeDeleted = "Defult Data Can Not be Deleted";
            public static string FirstUnitConversionFactor = "First Unit Conversion Factor should be 1";
            public static string JWTError = "JWT Error";
            public static string CannotEditDeletedItems = "Cannot Edit Deleted Items";
            public static string ArabicNameLengthIsMoreThanMaxmum = "Arabic Name Length Is More Than Maxmum";
            public static string LatinNameLengthIsMoreThanMaxmum = "Latin Name Length Is More Than Maxmum";
            public static string StoreAndCurrentBranchDoseNotMatch = "Store And Current Branch Dose Not Match";
            public static string AcmountCantNotBeZeroOrLess = "Acmount Cant Not Be Zero Or Less";
            public static string PersonDoseNotExist = "Person Dose Not Exist";
            public static string CantEditOtherBranchesElements = "Cant Edit Other Branches Elements";
            public static string IPExist = "IP exist";


            //Funds
            public static string safeFundsIsClosed = "Safe Funds is Closed";
            public static string BankFundsIsClosed = "Bank Funds is Closed";
            public static string CustomersFundsIsClosed = "Customers Funds is Closed";
            public static string SuppliersFundsIsClosed = "Suppliers Funds is Closed";
            public static string ValuesCanNotBeZero = "Values Cannot Be Zero";
            public static string CannotAddSamePaymentMethod = "Cannot Add Same Payment Method";


            //Securty
            public static string CompanyDoseNotHaveActivePerion = "Company Dose Not Have Active Perion";
            public static string YouHaveTheMaxmumOfStores = "can not add a new store because You Have The Maxmum Of Stores";
            public static string YouHaveTheMaxmumOfEmployees = "can not add a new Employees because You Have The Maxmum Of Employees";
            public static string YouHaveTheMaxmumOfUsers = "can not add a new Users because You Have The Maxmum Of Users";
            public static string YouHaveTheMaxmumOfBranchs = "can not add a new Branchs because You Have The Maxmum Of Branchs";
            public static string YouHaveTheMaxmumOfSuppliers = "can not add a new Suppliers because You Have The Maxmum Of Suppliers";
            public static string YouHaveTheMaxmumOfCustomers = "can not add a new Customers because You Have The Maxmum Of Customers";
            public static string YouHaveTheMaxmumOfPOS = "can not add a new POS Device because You Have The Maxmum Of OIS";
            public static string YouHaveTheMaxmumOfInvoices = "can not add a new Invoices Device because You Have The Maxmum Of Invoices";


            public static string YouCantViewTheHistory = "You Cant View The History";
            public static string JournalIsNotBalanaced = "Journal Is Not Balanaced";

            //POS
            public static string DeviceIdisRequired = "Device Id is Required";
            

            //Reports
            public static string ItemUnitIsNotExist = "Item Unit Is Not Exist";
            public static string ItemIsNotExist = "Item Is Not Exist";

            //invoices
            public static string MaxItemsCountOfInvoice = "Max items of invoice is ";
            public static string deleteSerial = "DeleteFromUpdate";
            public static string invalidType = "Invalid type";

            //public static string CustomerBalanceAr = "السحب من رصيد العميل";
            //public static string CustomerBalanceEn = "Withdrawal from the customer's balance";
            //public static string SupplierBalanceAr = "السحب من رصيد المورد";
            //public static string SupplierBalanceEn = "Withdrawal from the supplier's balance";

            public class FundList
            {
                public int id { get; set; }
                public string descAr { get; set; }
                public string descEn { get; set; }
            }
            public static List<FundList> fundLists()
            {
                var list = new List<FundList>();
                list.AddRange(new[]
                {
                    new FundList
                    {
                        id = -1,
                        descAr = "ارصدة اول المدة",
                        descEn ="Entry Fund",
                    },
                    new FundList
                    {
                        id = -2,
                        descAr = "ارصدة اول المدة عملاء",
                        descEn ="Entry Fund Customers",
                    },
                    new FundList
                    {
                        id = -3,
                        descAr = "ارصدة اول المدة موردين",
                        descEn ="Entry Fund Suppliers",
                    },
                    new FundList
                    {
                        id = -4,
                        descAr = "ارصدة اول المدة خزائن",
                        descEn ="Entry Fund Safes",
                    },
                    new FundList
                    {
                        id = -5,
                        descAr = "ارصدة اول المدة بنوك",
                        descEn ="Entry Fund Banks",
                    },
                    new FundList
                    {
                        id = -6,
                        descAr = "ارصدة اول المدة اصناف",
                        descEn ="Entry Fund Items",
                    },
                });
                return list;
            }

        }

        public class ItemCardMessages
        {
            public static string UnitsAreRequired = "Units are required";
        }
        public class GeneralSettingsMessages
        {
            public static string AutomaticCodingIsDisabled = "Items Automatic coding is disabled";
        }

        public class FilesDirectories
        {
            public static string JournalEntriesDirectory = "JournalEntries";
            public static string Reciepts = "Reciepts";
            public static string Invoices = "Invoices";
            public static string Purchases = Path.Combine(new[] { Invoices, "Purchases" });
            public static string ReturnPurchases = Path.Combine(new[] { Invoices, "ReturnPurchases" });
            public static string Sales = Path.Combine(new[] { Invoices, "Sales" });
            public static string ReturnSales = Path.Combine(new[] { Invoices, "ReturnSales" });
            public static string AdditionPermission = Path.Combine(new[] { Invoices, "AdditionPermission" });
            public static string ItemsFunds = Path.Combine(new[] { Invoices, "ItemsFunds" });
            public static string SafeCash = Path.Combine(new[] { Reciepts, "SafeCash" });
            public static string SafePayment = Path.Combine(new[] { Reciepts, "SafePayment" });
            public static string SafeTransfer = Path.Combine(new[] { Reciepts, "SafeTransfer" });
            public static string BankCash = Path.Combine(new[] { Reciepts, "BankCash" });
            public static string BankPayment = Path.Combine(new[] { Reciepts, "BankPayment" });
            public static string BankTransfer = Path.Combine(new[] { Reciepts, "BankTransfer" });
            public static string WOV_Purchases = Path.Combine(new[] { Invoices, "PurchasesWithoutVat" });
            public static string ReturnWOV_Purchases = Path.Combine(new[] { Invoices, "ReturnPurchasesWithoutVat" });
            public static string SafeMultiCollectionReceipts = Path.Combine(new[] { Reciepts, "SafeMultiCollectionReceipts" });
            public static string BankMultiCollectionReceipts = Path.Combine(new[] { Reciepts, "BankMultiCollectionReceipts" });

        }
        public class HistoryActions
        {
            public static string Add = "A";
            public static string Update = "U";
            public static string Delete = "D";
            public static string Accredit = "ACC";
            public static string Print = "Print";
            public static string ExportToPdf = "ExportToPdf";
            public static string ExportToImage = "ExportToImage";
            public static string ExportToWord = "ExportToWord";
            public static string ExportToExcle = "ExportToExcle";
            public static string ExportToSVG = "ExportToSVG";
            public static string TechnicalSupportAr = "دعم فنى";
            public static string TechnicalSupportEn = "Technical support";
        }
        public class HistoryActionsNames
        {

            public string ArabicName;
            public string LatinName;
            public HistoryActionsNames(string _ArabicName, string _LatinName)
            {
                ArabicName = _ArabicName;
                LatinName = _LatinName;
            }

        }
        public class HistoryActionsAliasNames
        {
            public static Dictionary<string, HistoryActionsNames> HistoryName = new Dictionary<string, HistoryActionsNames>(){
    {HistoryActions.Add, new HistoryActionsNames("إضافه","Add") },
    {HistoryActions.Delete,  new HistoryActionsNames("حذف","Delete")},
    {HistoryActions.Update,  new HistoryActionsNames("تعديل","Update")},
    {HistoryActions.Accredit,  new HistoryActionsNames("اعتماد","Accredit")},
    {HistoryActions.Print,  new HistoryActionsNames("طباعة","Print")},
    {HistoryActions.ExportToPdf,  new HistoryActionsNames("تصدير","Export")},
    {HistoryActions.ExportToWord,  new HistoryActionsNames("تصدير","Export")},
    {HistoryActions.ExportToImage,  new HistoryActionsNames("تصدير","Export")},
    {HistoryActions.ExportToExcle,  new HistoryActionsNames("تصدير","Export")},
    {HistoryActions.ExportToSVG,  new HistoryActionsNames("تصدير","Export")},




};
        }
        public class NotesOfReciepts
        {
            public static string PurchaseAr = "اعتماد فاتورة مشتريات";
            public static string PurchaseEn = "Accredit of purchase invoice";
            public static string ReturnPurchaseAr = "اعتماد فاتورة مرتجع مشتريات";
            public static string ReturnPurchaseEn = "Accredit of return purchase invoice";
            public static string SalesAr = "اعتماد فاتورة مبيعات";
            public static string SalesEn = "Accredit of sales invoice";
            public static string ReturnSalesAr = "اعتماد فاتورة مرتجع مبيعات";
            public static string ReturnSalesEn = "Accredit of return sales invoice";
            public static string PosAr = "اعتماد فاتورة نقاط بيع";
            public static string PosEn = "Accredit of POS invoice";
            public static string ReturnPosAr = "اعتماد فاتورة مرتجع نقاط بيع";
            public static string ReturnPosEn = "Accredit of return POS invoice";
            public static string AcquiredDiscountAr = "خصم مكتسب";
            public static string AcquiredDiscountEN = "Acquired discount";
            public static string PermittedDiscountAR = "خصم مسموح به";
            public static string PermittedDiscountEn = "Permitted discount";
            public static string CustomerFundsAr = "أرصدة عملاء";
            public static string CustomerFundsEn = "Customers funds";
            public static string SupplierFundsAr = "أرصدة موردين";
            public static string SupplierFundsEn = "Suppliers funds";
            public static string SafeFundsAr = "أرصدة خزائن";
            public static string SafeFundsEn = "Safes funds";
            public static string BankFundsAr = "أرصدة بنوك";
            public static string BankFundsEn = "Banks funds";
            public static string commissionsPaymentAr = "صرف عمولات";
            public static string commissionsPaymentEn = "commissions payment";
            public static string SafeCashAr = "سند قبض خزينة";
            public static string SafeCashEn = "Safe Cash Receipts ";
            public static string SafePaymentAr = "سند صرف خزينة";
            public static string SafePaymentEn = "Safe Payment Receipts";
            public static string SafeTransferAr = "سند تحويل خزينة";
            public static string SafeTransferEn = "Safe Transfer Receipts";
            public static string BankCashAr = "سند قبض بنك";
            public static string BankCashEn = "Bank Cash Receipts";
            public static string BankPaymentAr = "سند صرف بنك";
            public static string BankPaymentEn = "Bank Payment Receipts";
            public static string BankTransferAr = "سند تحويل بنك";
            public static string BankTransferEn = "Bank Transfer Receipts";
            public static string CompinedSafeCashAr = "سند مجمع قبض خزينة";
            public static string CompinedSafeCashEn = "Compined Safe Cash Receipts ";
            public static string CompinedSafePaymentAr = "سند مجمع صرف خزينة";
            public static string CompinedSafePaymentEn = "Compined Safe Payment Receipts";
            public static string CompinedBankCashAr = "سند مجمع قبض بنك";
            public static string CompinedBankCashEn = "Compined Bank Cash Receipts";
            public static string CompinedBankPaymentAr = "سند مجمع صرف بنك";
            public static string CompinedBankPaymentEn = "Compined Bank Payment Receipts";
            public static string CollectionReceiptsAr = "سند تحصيل ";
            public static string CollectionReceiptsEn = "Collection Receipts";
            public static string PaidReciptsAr = "سند سداد";
            public static string PaidReciptsEn = "Paid Receipts";
            public static string SafeMultiCollectionReceiptsAr = "سند مجمع خزينة";
            public static string SafeMultiCollectionReceiptsEn = "Safe Multi Collection Receipt";

            public static string BankMultiCollectionReceiptsAr = "سند مجمع بنوك";
            public static string BankMultiCollectionReceiptsEn = "Bank Multi Collection Receipt";

        }


        public class listOfInvoicesNames
        {
            public static List<InvoicesNames> listOfNames()
            {
                var list = new List<InvoicesNames>();
                list.AddRange(new[]
                {
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.Purchase,
                        NameAr = "فاتورة مشتريات",
                        NameEn = "Purchase",
                        MainType = 1
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.ReturnPurchase,
                        NameAr = "مرتجع فاتورة مشتريات",
                        NameEn = "Purchase return",
                        MainType =1
                    },
                     new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.DeletePurchase,
                        NameAr = "حذف فاتورة مشتريات",
                        NameEn = "Deletes Purchase",
                        MainType =1
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.wov_purchase,
                        NameAr = "فاتورة مشتريات بدون قيمة مضافة",
                        NameEn = "Purchase Without Vat",
                        MainType = 1
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.ReturnWov_purchase,
                        NameAr = "مرتجع فاتورة مشتريات بدون قيمة مضافة",
                        NameEn = "Purchase without vat return",
                        MainType =1
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId =(int)DocumentType.AcquiredDiscount,
                        NameAr = "خصم مكتسب",
                        NameEn = "Discount",
                        MainType = 1
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.VATPurchase,
                        NameAr = "ضريبة القيمة المضافة للمشتريات",
                        NameEn = "VAT",
                        MainType = 1
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.Sales,
                        NameAr = "فاتورة بيع",
                        NameEn = "Sales",
                        MainType =2
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.ReturnSales,
                        NameAr = "مرتجع فاتورة بيع",
                        NameEn = "Sales return",
                        MainType = 2
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.DeleteSales,
                        NameAr = "حذف فاتورة بيع",
                        NameEn = "Delete Sales",
                        MainType =1
                    }, 
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.POS,
                        NameAr = "فاتورة نقاط بيع",
                        NameEn = "POS",
                        MainType = 2
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.ReturnPOS,
                        NameAr = "مرتجع نقاط بيع",
                        NameEn = "POS return",
                        MainType =2
                    },
                      new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.DeletePOS,
                        NameAr = "حذف فاتورة نقاط بيع",
                        NameEn = "Delete POS",
                        MainType =1
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.VATSale,
                        NameAr = "ضريبة القيمة المضافة للمبيعات",
                        NameEn = "VAT",
                        MainType =2
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.PermittedDiscount,
                        NameAr = "خصم مسموح به",
                        NameEn = "Discount",
                        MainType = 2
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.Inventory,
                        NameAr = "تكلفه المبيعات ",
                        NameEn = "SalesCost",
                        MainType = 2
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.AddPermission,
                        NameAr = "اذن اضافة المخزني",
                        NameEn = "Add Permission",
                        MainType =3
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.ExtractPermission,
                        NameAr = "اذن الصرف المخزني",
                        NameEn = "Extract Permission",
                        MainType = 3
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.stockBalanceExcessive,
                        NameAr = "تسوية الجرد الوارد(ذيادة)",
                        NameEn = "Stock Balance Excessive",
                        MainType = 3
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.stockBalanceDeficit,
                        NameAr = "تسوية الجرد الوارد(عجز)",
                        NameEn = "Stock Balance Deficit",
                        MainType = 3
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.SafeFunds,
                        NameAr = "ارصده اول المده خزائن",
                        NameEn = "Entry Fund Safes",
                        MainType = 4
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.BankFunds,
                        NameAr = "ارصده اول المده بنوك",
                        NameEn = "Entry Fund Banks",
                        MainType = 4
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.CustomerFunds,
                        NameAr = "ارصده اول المده عملاء",
                        NameEn = "Entry Fund Customers",
                        MainType = 4
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.SuplierFunds,
                        NameAr = "ارصده اول المده موردين",
                        NameEn = "Entry Fund Suppliers",
                        MainType = 4
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.itemsFund,
                        NameAr = "ارصده اول المده اصناف",
                        NameEn = "Entry Fund Items",
                        MainType = 4
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.DeleteItemsFund,
                        NameAr = "ارصده اول المده اصناف - محذوف",
                        NameEn = "Entry Fund Items - Deleted",
                        MainType = 4
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.OutgoingTransfer,
                        NameAr = "تحويل صادر",
                        NameEn = "Outgoing Transfer",
                        MainType = 4
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.IncomingTransfer,
                        NameAr = "تحويل وارد",
                        NameEn = "Incoming Transfer",
                        MainType = 4
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.DeletedOutgoingTransfer,
                        NameAr = "تحويل صادر - محذوف",
                        NameEn = "Incoming Transfer - Deleted",
                        MainType = 4
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.DeletedIncommingTransfer,
                        NameAr = "تحويل وارد - محذوف",
                        NameEn = "Incoming Transfer - Deleted",
                        MainType = 4
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.DeleteAddPermission,
                        NameAr = "اذن اضافة - محذوف",
                        NameEn = "Add Permission - Deleted",
                        MainType = 4
                    },
                    new InvoicesNames
                    {
                        invoiceTypeId = (int)DocumentType.DeleteExtractPermission,
                        NameAr = "اذن صرف - محذوف",
                        NameEn = "Extract Permission - Deleted",
                        MainType = 4
                    }
                });

                return list;
            }

           
            //public static List<InvoicesNames> listOfAccreditInvoiceNames()
            //{
            //    var list = new List<InvoicesNames>()
            //    {
            //        new InvoicesNames
            //        {
            //            invoiceTypeId = (int)DocumentType.Purchase,
            //            NameAr = "فاتورة مشتريات",
            //            NameEn = "Purchase",
            //            MainType = 1
            //        },
            //         new InvoicesNames
            //        {
            //            invoiceTypeId = (int)DocumentType.Purchase,
            //            NameAr = "فاتورة مشتريات",
            //            NameEn = "Purchase",
            //            MainType = 1
            //        },
            //          new InvoicesNames
            //        {
            //            invoiceTypeId = (int)DocumentType.Purchase,
            //            NameAr = "فاتورة مشتريات",
            //            NameEn = "Purchase",
            //            MainType = 1
            //        },
            //    };
            //    }


            public class InvoicesNames
            {
                public int invoiceTypeId { get; set; }
                public int MainType { get; set; }
                public string NameAr { get; set; }
                public string NameEn { get; set; }
            }
            #region Barcode
            //Dock
            public static string TopDock = "Top";
            public static string BottomDock = "Bottom";
            public static string RightDock = "Right";
            public static string LeftDock = "Left";
            public static string CenterDock = "Center";
            //Align
            public static string RightAlign = "Right";
            public static string LeftAlign = "Left";
            public static string CenterAlign = "Center";

            #endregion

        }
        public class TransferStatus
        {
            public static int Rejected = 1;
            public static int Accepted = 2;
            public static int PartialAccepted = 3;
            public static int Binded = 4;
            public static int Deleted = 5;

        }
        public class ProgressStatus
        {
            public static int InProgress = 2;
            public static int ProgressFinshed = 1;
            public static int ProgressFalid = 0;
            public static int subProgress = 3;
        }
    }
}