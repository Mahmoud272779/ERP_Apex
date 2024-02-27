using App.Domain.Entities;
using App.Domain.Entities.Process;
using System;
using System.Collections.Generic;
using System.Threading;

namespace App.Domain.Enums
{
    public class Enums
    {
        public enum PersonAccountDataDocTypeId
        {
            invoices = -1,
            reciepts = -2,
            discount = -3,
            multiCollectionRec = -4,
            offerPrice = -5,
            AddPermission=-6,
            ExtractPermission=-7,
            itemsFund=-8,
            OutgoingTransfer=-9,
            IncomingTransfer=-10,

        }
        public enum journalEntryTypes
        {
            all = 0, //الكل
            purchases = 1, //مشتريات
            returnPurchases = 2, //مرتجع مشتريات 
            sales = 3, // مبيعات
            returnSales = 4, //مرتجع مبيعات
            pos = 5, //نقاط بيع
            returnPos = 6, // مرتجع نقاط بيع
            safeCashRec = 7, // سند قبض خزائن
            safePaymentRec = 8, // سند صرف خزائن
            bankCashRec = 9, // سند قبض بنك
            bankPaymentRec = 10, //سند صرف بنك
            ManualJournalEntry = 11, // سند يدوي
        }
        public enum tcpType
        {
            exporting = 1,
            EInvoice = 2
        }

        public enum AlartType
        {
            error = 0,
            warrning = 1,
            information = 2,
            success = 3
        }
        public enum AlartShow
        {
            note = 0,
            popup = 1
        }
        public enum POSSessionStatus
        {
            Unknown = 0,
            active = 1,
            closed = 2,
            bining = 3
        }
        public enum userPOSSessionChecker
        {
            start = 1,
            resume = 2,
            opened = 3,
            notAllowed = 4
        }
        public enum Barcodestander_BarcodeType
        {
            Unspecified,
            UpcA,
            UpcE,
            UpcSupplemental2Digit,
            UpcSupplemental5Digit,
            Ean13,
            Ean8,
            Interleaved2Of5,
            Interleaved2Of5Mod10,
            Standard2Of5,
            Standard2Of5Mod10,
            Industrial2Of5,
            Industrial2Of5Mod10,
            Code39,
            Code39Extended,
            Code39Mod43,
            Codabar,
            PostNet,
            Bookland,
            Isbn,
            Jan13,
            MsiMod10,
            Msi2Mod10,
            MsiMod11,
            MsiMod11Mod10,
            ModifiedPlessey,
            Code11,
            Usd8,
            Ucc12,
            Ucc13,
            Logmars,
            Code128,
            Code128A,
            Code128B,
            Code128C,
            Itf14,
            Code93,
            Telepen,
            Fim,
            Pharmacode
        }
        public static BarcodeStandard.Type BarcodeTypes(Barcodestander_BarcodeType barcodeType)
        {
            if (barcodeType == Barcodestander_BarcodeType.Unspecified)
                return BarcodeStandard.Type.Unspecified;
            else if (barcodeType == Barcodestander_BarcodeType.UpcA)
                return BarcodeStandard.Type.UpcA;
            else if (barcodeType == Barcodestander_BarcodeType.UpcE)
                return BarcodeStandard.Type.UpcE;
            else if (barcodeType == Barcodestander_BarcodeType.UpcSupplemental2Digit)
                return BarcodeStandard.Type.UpcSupplemental2Digit;
            else if (barcodeType == Barcodestander_BarcodeType.UpcSupplemental5Digit)
                return BarcodeStandard.Type.UpcSupplemental5Digit;
            else if (barcodeType == Barcodestander_BarcodeType.Ean13)
                return BarcodeStandard.Type.Ean13;
            else if (barcodeType == Barcodestander_BarcodeType.Ean8)
                return BarcodeStandard.Type.Ean8;
            else if (barcodeType == Barcodestander_BarcodeType.Interleaved2Of5)
                return BarcodeStandard.Type.Interleaved2Of5;
            else if (barcodeType == Barcodestander_BarcodeType.Interleaved2Of5Mod10)
                return BarcodeStandard.Type.Interleaved2Of5Mod10;
            else if (barcodeType == Barcodestander_BarcodeType.Industrial2Of5)
                return BarcodeStandard.Type.Industrial2Of5;
            else if (barcodeType == Barcodestander_BarcodeType.Industrial2Of5Mod10)
                return BarcodeStandard.Type.Industrial2Of5Mod10;
            else if (barcodeType == Barcodestander_BarcodeType.Code39)
                return BarcodeStandard.Type.Code39;
            else if (barcodeType == Barcodestander_BarcodeType.Code39Extended)
                return BarcodeStandard.Type.Code39Extended;
            else if (barcodeType == Barcodestander_BarcodeType.Code39Mod43)
                return BarcodeStandard.Type.Code39Mod43;
            else if (barcodeType == Barcodestander_BarcodeType.Codabar)
                return BarcodeStandard.Type.Codabar;
            else if (barcodeType == Barcodestander_BarcodeType.PostNet)
                return BarcodeStandard.Type.PostNet;
            else if (barcodeType == Barcodestander_BarcodeType.Bookland)
                return BarcodeStandard.Type.Bookland;
            else if (barcodeType == Barcodestander_BarcodeType.Isbn)
                return BarcodeStandard.Type.Isbn;
            else if (barcodeType == Barcodestander_BarcodeType.Jan13)
                return BarcodeStandard.Type.Jan13;
            else if (barcodeType == Barcodestander_BarcodeType.MsiMod10)
                return BarcodeStandard.Type.MsiMod10;
            else if (barcodeType == Barcodestander_BarcodeType.Msi2Mod10)
                return BarcodeStandard.Type.Msi2Mod10;
            else if (barcodeType == Barcodestander_BarcodeType.MsiMod11)
                return BarcodeStandard.Type.MsiMod11;
            else if (barcodeType == Barcodestander_BarcodeType.MsiMod11Mod10)
                return BarcodeStandard.Type.MsiMod11Mod10;
            else if (barcodeType == Barcodestander_BarcodeType.ModifiedPlessey)
                return BarcodeStandard.Type.ModifiedPlessey;
            else if (barcodeType == Barcodestander_BarcodeType.Code11)
                return BarcodeStandard.Type.Code11;
            else if (barcodeType == Barcodestander_BarcodeType.Usd8)
                return BarcodeStandard.Type.Usd8;
            else if (barcodeType == Barcodestander_BarcodeType.Ucc12)
                return BarcodeStandard.Type.Ucc12;
            else if (barcodeType == Barcodestander_BarcodeType.Ucc13)
                return BarcodeStandard.Type.Ucc13;
            else if (barcodeType == Barcodestander_BarcodeType.Logmars)
                return BarcodeStandard.Type.Logmars;
            else if (barcodeType == Barcodestander_BarcodeType.Code128)
                return BarcodeStandard.Type.Code128;
            else if (barcodeType == Barcodestander_BarcodeType.Code128A)
                return BarcodeStandard.Type.Code128A;
            else if (barcodeType == Barcodestander_BarcodeType.Code128B)
                return BarcodeStandard.Type.Code128B;
            else if (barcodeType == Barcodestander_BarcodeType.Itf14)
                return BarcodeStandard.Type.Itf14;
            else if (barcodeType == Barcodestander_BarcodeType.Code93)
                return BarcodeStandard.Type.Code93;
            else if (barcodeType == Barcodestander_BarcodeType.Telepen)
                return BarcodeStandard.Type.Telepen;
            else if (barcodeType == Barcodestander_BarcodeType.Fim)
                return BarcodeStandard.Type.Fim;
            else if (barcodeType == Barcodestander_BarcodeType.Pharmacode)
                return BarcodeStandard.Type.Pharmacode;
            return BarcodeStandard.Type.Code128;
        }

        public enum generalEnum
        {
            BalanceBarcodeDecimal = 4,
            vatDecimal = 6,
            maxItemsOfInvoice = 200,
            maxItemsOfPOS = 100,
        }
        public enum SecurityApplicationAdditionalPriceIndexs
        {
            extraUsers = 1,
            extraEmployees = 2,
            extraPOS = 3,
            extraStores = 4,
            extraBranches = 5,
            extraCustomers = 6,
            extraSuppliers = 7,
            extraInvoices = 8,
            extraItems = 9,
            extraOfflinePOS = 10
        }
        public enum Result
        {
            Failed = 0,
            Success = 1,
            Exist = 2,
            ReleatedData = 3,
            NoDefinedPeriods = 4,
            OverlappingPeriods = 5,
            ExistInOtherModule = 6,
            MissingSystemOption = 7,
            NoDataFound = 8,
            RequiredData = 9,
            CanNotBeDeleted = 10,
            ItemCodeExist = 11,
            NationalExist = 12,
            BarcodeExist = 13,
            SerialExist = 14,
            NationalItemCodeExist = 15,
            NotFound = 16,
            InActive = 17,
            NotExist = 18,
            NotTotalPaid = 19,
            NotFoundEmail = 20,
            QuantityNotavailable = 21,
            PaidOvershootNet = 22,
            CanNotBeUpdated = 23,
            CanNotAddCompositeItem = 24,
            InvoiceTotalReturned = 25,
            Deleted = 26,
            NoDataChanged = 27,
            SerialNotExist = 28,
            CanNotDeleteSerial = 29,
            UnAuthorized = 30,
            EnterExpiryDate = 31,
            EnterSerials = 32,
            itemNotUsedInSales = 33,
            MaximumLength = 34,
            InvoiceDeletedOrReturned = 35,
            MaxCountOfItems = 36,
            SerialsConflicted = 37,
            logout = 38,
            bindedTransfer = 39,
            periodEnded = 40,
            userLocked = 41,
            DeferredSale = 42,
            editingDate = 43,
            canNotChangeStore = 44,
            InProgress = 45,
            profitDone = 46,
            ExceedPersonBalance = 47,
            canNotAddItem = 50,
        }

        public enum RepositoryActionStatus
        {
            Ok,
            Created,
            Updated,
            NotFound,
            Deleted,
            NothingModified,
            Error,
            BadRequest,
            UnAuthorized,
            ExistedBefore,
            NothingAdded,
            ImportScussfully,
            ImportWithErrors,
            UpdateFileScussfully,
            UpdateFileWithErrors,
            EmptyFile,
            IsShiftOpen,
            CanNotDelete
        }
        public enum Status
        {
            Active = 1,
            Inactive = 2
        }

        public enum FA_Nature
        {
            Debit = 1,
            Credit = 2
        }
        public enum FinalAccount
        {
            Budget = 1,
            IncomeList = 2
        }
        public enum HistoryTitle
        {
            Add = 1,
            Update = 2,
            Delete = 3,
            Print = 4
        }
        public enum periodTypes
        {
            monthly = 0,
            yearly = 1
        }
        //
        public enum StepType
        {
            back = -1,
            next = 1,
            first = 0,
            last = -2
        }

        public enum UsedInSales
        {
            Used = 1,
            NotUsed = 2
        }

        public class TransactionTypeModel
        {
            public int id { get; set; }
            public string arabicName { get; set; }
            public string latinName { get; set; }
        }
        public static class TransactionTypeList
        {
            public readonly static List<TransactionTypeModel> collectionRec = new List<TransactionTypeModel>()
            {
                new TransactionTypeModel() {
                        id =0,
                        arabicName = "",
                        latinName = ""},
                new TransactionTypeModel() {
                        id = (int)SubType.PaidReceipt,
                        arabicName = " سند سداد  ",
                        latinName = " Paid Receipts "},
                new TransactionTypeModel() {
                        id = (int)SubType.CollectionReceipt,
                        arabicName = " سند سداد ",
                        latinName = " Paid Receipts "}
            };

            public static List<TransactionTypeModel> transactionTypeModels()
            {
                var list = new List<TransactionTypeModel>();
                list.AddRange(new[]
                {
                    new TransactionTypeModel
                    {
                        id = 0,
                        arabicName = "الرصيد السابق",
                        latinName = "Previous Balance"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.POS,
                        arabicName = "نقاط بيع",
                        latinName = "POS"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.ReturnPOS,
                        arabicName = "مرتجع نقاط بيع",
                        latinName = "Return POS"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.Sales,
                        arabicName = "مبيعات",
                        latinName = "Sales"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.ReturnSales,
                        arabicName = "مرتجع مبيعات",
                        latinName = "Sales Return"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.Purchase,
                        arabicName = "مشتريات",
                        latinName = "Purchases"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.ReturnPurchase,
                        arabicName = "مرتجع مشتريات",
                        latinName = "Return Purchases"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.SafeCash,
                        arabicName = "سند قبض خزينة",
                        latinName = "Safe Cash"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.BankCash,
                        arabicName = "سند قبض بنك",
                        latinName = "Bank Cash"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.BankPayment,
                        arabicName = "سند صرف بنك",
                        latinName = "Bank Payment"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.SafePayment,
                        arabicName = "سند صرف خزينة",
                        latinName = "Safe Payment"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.AddPermission,
                        arabicName = "اذن اضافة",
                        latinName = "Add Permission"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.ExtractPermission,
                        arabicName = "اذن صرف",
                        latinName = "Extract Permission"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.itemsFund,
                        arabicName = "رصيد اول المدة اصناف",
                        latinName = "Items Fund"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.AcquiredDiscount,
                        arabicName = "خصم مكتسب",
                        latinName = "Acquired Discount"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.PermittedDiscount,
                        arabicName = "خصم مسموح به",
                        latinName = "Permitted Discount"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.SafeFunds,
                        arabicName = "ارصدة اول المدة",
                        latinName = "Entry Fund"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.BankFunds,
                        arabicName = "ارصدة اول المدة",
                        latinName = "Entry Fund"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.OutgoingTransfer,
                        arabicName = "تحويل صادر ",
                        latinName = "Outgoing Transfer"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.IncomingTransfer,
                        arabicName = "تحويل وارد  ",
                        latinName = "incomming Transfer"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.PaidVAT,
                        arabicName = "القيمة المضافة المسددة  ",
                        latinName = "PaidVAT"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.SafeTransfer,
                        arabicName = "سند تحويل خزينة",
                        latinName = "Safe Transfer"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.BankTransfer,
                        arabicName = "سند تحويل بنك",
                        latinName = "Bank Transfer"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.CustomerFunds,
                        arabicName = "رصيد اول المده عملاء",
                        latinName = "Customer Funds"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.SuplierFunds,
                        arabicName = "رصيد اول المدة موردين",
                        latinName = "Suplier Funds"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.SafeMultiCollectionReceipts,
                        arabicName = "سند مجمع خزائن",
                        latinName = "Safe Multi Collection Receipts"
                    },
                    new TransactionTypeModel
                    {
                        id = (int)DocumentType.BankMultiCollectionReceipts,
                        arabicName = "سند مجمع بنوك",
                        latinName = "Bank Multi Collection Receipts"
                    }
                });

                return list;
            }
        }
        public enum expensesInvoiceType
        {
            Expenses = 8,
            ExpensesReturn = 9,

        }
        public enum ReceiptTitleIds
        {
            BankCashReceipt = 1,
            BankPaymentReceipt = 2,

            SafeCashReceipt = 3,
            SafePaymentReceipt = 4,

            SafeMultiCollectionReceipts = 5,
            BankMultiCollectionReceipts = 6,
            SafeCollectionReceipts = 7,
            BankCollectionReceipts = 8,
            safeTransferReceipts = 9,
            bankTransferReceipts = 10

        }
        public enum DocumentType
        {
            AddPermission = 1,  // اذن اضافة
            DeleteAddPermission = 2,   // حذف اذن اضافة
            ExtractPermission = 3, // اذن صرف
            DeleteExtractPermission = 4, // حذف اذن صرف
            Purchase = 5,  // مشتريات
            ReturnPurchase = 6, // مرتجع مشتريات
            DeletePurchase = 7, // حذف مشتريات
            Sales = 8, // مبيعات
            ReturnSales = 9, // مرتحع مبيعات
            DeleteSales = 10, // حذف مبيعات
            POS = 11, // نقاط بيع
            ReturnPOS = 12, // مرتحع نقاط بيع
            DeletePOS = 13, // حذف نقاط بيع
            AcquiredDiscount = 14, // خصم مكتسب
            PermittedDiscount = 15, // خصم مسموح به 
            DeleteAcquiredDiscount = 16,  // حذف خصم مكتسب
            DeletePermittedDiscount = 17, // حذف خصم مسموح به
            SafeCash = 18, //سند قبض خزينة
            SafePayment = 19, //سند صرف خزينة
            BankCash = 20, //سند قبض بنك
            BankPayment = 21, //سند صرف بنك
            CompinedSafeCash = 38, //سند مجمع قبض خزينة
            CompinedSafePayment = 39, //سند مجمع صرف خزينة
            CompinedBankCash = 40, //سند مجمع قبض بنك
            CompinedBankPayment = 41, //سند مجمع صرف بنك
            itemsFund = 22, // أرصدة أول مدة
            DeleteItemsFund = 23, // حذف أرصدة أول مدة
            VATSale = 24, // القيمة المضافة الخاصة بالمبيعات
            VATPurchase = 25, // القيمة المضافة الخاصة بالمشتريات
            wov_purchase = 26,// مشتريات بدون قيمة مضافة
            ReturnWov_purchase = 27,// إرتجاع مشتريات بدون قيمة مضافة
            SafeFunds = 28, //ارصده اول المدة خزائن
            BankFunds = 29, // ارصده اول المدة بنوك
            stockBalanceDeficit = 30, //تسويه جرد مخزن عحز
            stockBalanceExcessive = 31, //تسويه جرد مخزن ذيادة
            CustomerFunds = 32, // ارصدة اول المدة عملاء   26
            SuplierFunds = 33, // ارصده اول المدة موردين   27
            IncomingTransfer = 34, // تحويل وارد
            OutgoingTransfer = 35, // تحويل صادر
            DeletedOutgoingTransfer = 36, // تحويل صادر حذف
            DeletedIncommingTransfer = 37, // تحويل وارد حذف
            OfferPrice = 38, //عرض سعر
            Inventory = 39, //الجرد المستمر
            DeleteOfferPrice = 40, // حذف عرض السعر
            PurchaseOrder = 41, // أمر شراء
            DeletePurchaseOrder = 42,// حذف امر الشراء
            DeleteWov_purchase = 43, // حذف مشتريات بدون فات
            PaidVAT = 44, // القيمة المضافة المسددة
            SafeTransfer = 45, // تحويل خزينة
            BankTransfer = 46, // تحويل بنك
            SafeMultiCollectionReceipts = 47, //سند مجمع خزينة
            BankMultiCollectionReceipts = 48, // سند مجمع بنك
            DetailedTransactoinsOfItem=49, //الحركة التفصيلية للصنف
            ReviewWarehouseTransfers=50,//مراجعة تحويلات المحازن
            DetailedTransferReport=51,//تحويلات تفصيلى
            DetailsOfSerialTransactions=52,//الاستعلام عن مسلسل
            GetVatStatmentData = 53,//القيمة المضافة الاجمالى
            VatDetailedReport=54,//القيمة المضافة تفصيلى


        }
        //screen names 
        public enum ScreenNames
        {
            items = 1,                 //الاصناف
            Safes = 2,                 //الخزائن
            Banks = 3,                 //البنوك
            Suppliers = 4,                 //الموردين
            Customres = 5,                 //العملاء
            Branches = 6,                 //الفروع
            Currencies = 7,                 //العملات
            OtherAuthorities = 8,                 //جهات صرف اخري
            CalculationGuide = 9,                 //الدليل المحاسبي
            OpeningBalance = 10,                //الأرصدة الافتتاحية
            AccountingEntries = 11,                //القيود
            CostCenter = 12,                //مراكز التكلفه
            PayReceiptForSafe = 13,                //سند صرف خزينة
            CatchReceiptForBank = 14,                //سند قبض بنكي
            GeneralledgersSettings = 15,                //إعدادات الحسابات العامه
            Units = 16,                //الوحدات
            Sizes = 17,                //المقاس
            Color = 18,                //اللون
            Categories = 19,                //مجموعات الاصناف
            ItemCard = 20,                //كارت الصنف
            Employees = 21,                //الموظفين
            Vacancies = 22,                //الوظائف
            Stores = 23,                //المستودعات
            Storeplaces = 24,                //أماكن التخزين
            AddPermission = 25,                //اذن اضافة
            Barcode = 26,                //الباركود
            SupplierStatement = 27,                //كشف حساب مورد
            Purchases = 28,                //المشتريات
            PurchasesReturn = 29,                //مرتجع المشتريات
            PruchasesClosing = 30,                //اقفال المشتريات
            CommissionList = 31,                //لائحة العمولات
            Salesmen = 32,                //مندوبي المبيعات
            Sales = 33,                //مبيعات
            SalesClosing = 34,                //اقفال المبيعات
            EarnedDiscount = 35,                //خصم مكتسب
            PermittedDiscount = 36,                //خصم مسموح به
            Permission = 37,                //الصلاحيات
            Users = 38,                //المستخدمين
            Settings = 39,                //الاعدادات

            //reports from 100
            IncomeList = 100,               //قائمة الدخل
            DetailedTrialBalance = 101,               //ميزان المراجعه تفصيلي
            PublicBudget = 102,               //الميزانيه العموميه
            LedgerReport = 103,               //دفتر الاستاذ
            CostCenters = 104,               //مراكز التكلفه تقرير
            AccountStatementDetail = 105,               //كشف حساب تفصيلي





        }
        public enum SubType
        {
            Nothing = 0,
            PartialReturn = 1, // مرتجع  جزئي
            TotalReturn = 2,  // مرتجع  كلي
            AcceptedTransfer = 3,
            RejectedTransfer = 4,
            OfferPriceAccridited = 5, // تم تحويل عرض السعر للمبيعات او امر الشراء المشتريات
            OfferPriceDeleted = 6, // تم حذفه
            OfferPriceUnAccepted = 7,// لم يتم التحويل للمبيعات او امر الشراء المشتريات
            CollectionReceipt = 8, // سند تحصيل
            PaidReceipt = 9, // سند سداد
            Expenses = 10,// مصروفات
            ReturnExpenses = 11,// مرتجع مصروفات
        }
        public enum salesType
        {
            all = 0,
            sales = 1,
            returnInvoices = 2
        }
        public enum PaymentType
        {
            all = 0,
            Complete = 1,// مسدد 
            Partial = 2, // جزئي
            Delay = 3,// اجل
        }
        public enum PaymentMethod
        {
            Transfer = -2,
            PersonBalance = -1,
            all = 0,
            paid = 1,
            net = 2,
            Cheques = 3
        }
        public enum invoiceTypes
        {
            all = 0,
            sales = 1,
            salesReturns = 2,
            purchase = 3,
            purchaseReturn = 4

        }
        public enum PersonType
        {
            Normal = 1,
            Sectional = 2,//قطاع
            Wholesale = 3,
            Midmost = 4
        }
        public enum CommissionType
        {
            Fixed = 1,
            Slides = 2
        }

        public enum SalePricesList
        {
            SalePrice1 = 1,
            SalePrice2 = 2,
            SalePrice3 = 3,
            SalePrice4 = 4
        }
        public enum ItemTypes//طبيعة الأصناف
        {
            all = 0, //الكل
            Store = 1,//مخزني -
            Serial = 2,//سيريال -
            Expiary = 3,//تاريخ صلاحية -
            Service = 4,//خدمة -
            Composite = 5,//مُركب -
            Note = 6,//ملاحظة -
            Offer = 7,//عرض
            Restaurant = 8,//مطعم
            SizeAndColor = 9,//الوان ومقاسات
            Additives = 10,//اضافي
        }
        public enum PurchasesAdditionalType // التكاليف الاضافيه على الفاتوره(الاضافات الاخرى)
        {
            constant = 1, // قيمة ثابته
            RatioOfInvoiceTotal = 2,  // نسبة من اجمالى الفاتورة
            RatioOfInvoiceNet = 3  // نسبه من صافي الفاتوره

        }

        public enum DiscountType  // نوع الخصم على الأصناف او على الفاتورة
        {
            NoDiscount = 0,  // If there is no any discount in invoice but there is special discount on items
            DiscountOnItem = 1,  // Discount enterd with item on creating the invoice
            DiscountOnInvoice = 2  // Discount enterd on total of invoice on creating the invoice
        }
        public enum Organize
        {
            FinancialAccount = 1, //حسابات 
            Customers = 2, // عملاء
            Suppliers = 3 //موردين
        }

        public enum PaymentWays
        {
            Bank = 1,
            Cheque = 2,
            Cash = 3
        }

        public enum SerialsStatus
        {
            accepted = 0,
            repeatedInItems = 1,
            repeatedInOldSerials = 2,
            repeatedInCurrentSerials = 3,
            requiredPeriod = 4,
            errorInPeriod = 5,
            requiredSerial = 6,
            repeatedInThisInvoice = 7,
            bindedTransfer = 8,
            limitOfSerialsCount = 500

        }

        public enum CostCenterType
        {
            Sub = 1,
            Main = 2
        }
        public enum finalAccount
        {
            IncomingList = 2,
            Balance = 1
        }
        public enum MainInvoiceType
        {
            Purchases = 1,
            Sales = 2,
            Settlements = 3,
            funds = 4

        }
        public enum InvoiceTypeReport
        {
            TotalInovice = 1,
            Purchase,
            Sales,
            receipts

        }

        public enum RecieptsParentType
        {
            Purchase = 1,  // مشتريات
            ReturnPurchase = 2, // مرتجع مشتريات
            Sales = 3, // مبيعات
            ReturnSales = 4, // مرتحع مبيعات
            POS = 5, // نقاط بيع
            ReturnPOS = 6, // مرتحع نقاط بيع
            AcquiredDiscount = 7, // خصم مكتسب
            PermittedDiscount = 8, // حذف خصم مكتسب
            CustomersFunds = 9, // أرصدة عملاء
            SuppliersFunds = 10,//أرصدة موردين
            SafesFunds = 11,//أرصدة خزائن
            BanksFunds = 12,//أرصدة بنوك
            commissionsPayment = 13,//صرف العمولات
            ClosingInvoices = 14,//اقفال الفواتير
        }
        public class AuthorityTypes
        {
            public static int customers = 1;
            public static int suppliers = 2;
            public static int other = 3;
            public static int salesman = 4;
            public static int DirectAccounts = 5;//الحسابات العامه
            public static int Safe = 6;
            public static int Bank = 7;

        }
        public enum FinancialAccountRelationSettings
        {
            ChooseManualAccount = 1,
            CreateAccountAutomaticWithTheParentAccountId = 2,
            ChooseoneAccountTobeDefult = 3
        }
        public enum GLFinancialAccountRelation
        {
            customer = 1,
            supplier = 2,
            bank = 3,
            safe = 4,
            salesman = 5,
            employee = 6,
            OtherAuthorities = 7
        }
        public enum notificationTypes
        {
            System = 1,
            admin = 2,
            manager = 3
        }
        //public enum FinanicalAccountDefultIds
        //{
        //    /*Level 1*/
        //    /*1*/
        //    assets = -1,             //الاصول 
        //    /*2*/
        //    fixedAssets = -101,           //االاصول الثايتة
        //    /*3*/
        //    Equipment = -101001,        //اجهزة ومعدات
        //    /*4*/
        //    computers = -101001001,     //اجهزة كمبيوتر
        //    /*5*/
        //    furniture = -101002,        //اثاث ومفروشات
        //    /*6*/
        //    offices = -101002001,     //مكاتب 
        //    /*7*/
        //    cars = -101003,        //سيارات
        //    /*8*/
        //    PrivateCars = -101003001,     //سيارات خاصة 
        //    /*9*/
        //    LandsAndRealEstate = -101004,        //اراضي وعقارات
        //    /*10*/
        //    lands = -101004001,     //الاراضي
        //    /*11*/
        //    longTermOpponents = -102,           //الاصول المتداولة 
        //    /*12*/
        //    banks = -102001,        //البنوك
        //    /*13*/
        //    DefultBank = -102001001,     //بنك افتراضي
        //    /*14*/
        //    Safes = -102002,        //الخزائن
        //    /*15*/
        //    MasterSafe = -102002001,     //خزينة رئيسية
        //    /*16*/
        //    Customers = -102003,        //العملاء
        //    /*17*/
        //    cashCustomer = -102003001,     //عميل نقدي
        //    /*18*/
        //    Inventory = -102004,        // البضاعه بالمخزن
        //    /*19*/
        //    StaffReceivablesAndAdvances = -102005,        //ذمم الموظفين والسلف
        //    /*20*/
        //    OtherDebitBalances = -102006,        //أرصدة مدينة اخري
        //    /*21*/
        //    otherDebitBalances = -102006001,     //ارصدة مدينة اخري
        //    /*Level 2*/
        //    /*22*/
        //    liabilities = -2,             //الخصوم
        //    /*23*/
        //    currentLiabilities = -201,           //الخصوم  طويلة الاجل 
        //    /*24*/
        //    loans = -201001,        //قروض
        //    /*25*/
        //    LongTermOpponents = -202,           //الخصوم المتداولة 
        //    /*26*/
        //    Suppliers = -202001,        //الموردين
        //    /*27*/
        //    cashSuppliers = -202001001,     //مورد نقدي
        //    /*28*/
        //    ValueAddedTax = -202002,        //ضريبة القيمة المضافة
        //    /*29*/
        //    VATPurchases = -202002001,     //ضريبة القيمة المضافة مشتريات
        //    /*30*/
        //    VATSales = -202002002,     //ضريبة القيمة المضافة مبيعات
        //    /*31*/
        //    OtherCreditBalances = -202003,        //أرصدة دائنة اخري
        //    /*32*/
        //    otherCreditBalances = -202003001,     //أرصدة دائنة اخري
        //    /*Level 3*/
        //    /*33*/
        //    propertyRights = -3,             //حقوق الملكية
        //    /*34*/
        //    capital = -301,           //راس المال 
        //    /*35*/
        //    PartnersCapitalA = -301001,       //رأس مال الشريك أ
        //    /*36*/
        //    PartnersCapitalB = -301002,       //رأس مال الشريك ب
        //    /*37*/
        //    stageProfitsAndLosses = -302,           //أرباح وخسائر  مرحلة
        //    /*38*/
        //    retainedEarnings = -302001,        //الارباح المحتجزة
        //    /*39*/
        //    phaseLosses = -302002,        //الخسائر المرحلة
        //    /*Level 4*/
        //    /*40*/
        //    revenues = -4,             //الايرادات
        //    /*41*/
        //    businessRevenue = -401,           //إيرادات النشاط التجاري
        //    /*42*/
        //    Sales = -401001,           //المبيعات
        //    /*43*/
        //    SalesReturns = -401002,        //مردودات المبيعات
        //    /*44*/
        //    SalesAllowances = -401003,        //مسموحات المبيعات
        //    /*45*/
        //    DiscountPermitted = -401004,        //خصم مسموح بة
        //    /*46*/
        //    otherIncome = -402,           //إيرادات اخري
        //    /*47*/
        //    BankBenefits = -402001,        //فوائد بنكية
        //    /*Level 5*/
        //    /*48*/
        //    expensesAndCosts = -5,             //المصروفات و التكاليف
        //    /*49*/
        //    businessCost = -501,           //تكلفة النشاط التجاري   
        //    /*50*/
        //    FirstTermMerchandise = -501001,        //بضاعة أول المدة       -------> use in itemfund document
        //    /*51*/
        //    Purchases = -501002,        //المشتريات
        //    /*52*/
        //    ReturnsPurchases = -501003,        //مردودات المشتريات
        //    /*52*/
        //    EndTermMerchandise = -501004,        //بضاعة اخر المدة
        //    /*53*/
        //    ProcurementExpenses = -501005,        //مصاريف المشتريات
        //    /*54*/
        //    AcquiredDiscount = -501006,        //خصم مكتسب
        //    /*54.1*/
        //    SalesCostAcc = -501007,        //حساب تكلفه المبيعات 

        //    /*55*/
        //    expenses = -502,           //المصروفات
        //    /*56*/
        //    AdministrativeExpenses = -502001,        //مصاريف ادارية
        //    /*58*/
        //    Electricity = -502001001,     //كهرباء 
        //    /*57*/
        //    waters = -502001002,     //مياه
        //    /*59*/
        //    SellingAndMarketingExpenses = -502002,        //مصاريف بيعية وتسوقية
        //    /*60*/
        //    advertisment = -502002001,     //دعايا واعلان 
        //    /*61*/
        //    salesMan = -502002002,     //عمولات المناديب
        //    /*62*/
        //    operatingExpenses = -502003,        //مصاريف تشغيلية
        //    /*63*/
        //    carMaintenance = -502003001,     //صيانة السيارات
        //    /*64*/
        //    depreciationExpenses = -502004,        //مصاريف الاهلاك
        //    /*65*/
        //    equipmentDepreciationExpense = -502004001,     //مصروف اهلاك الاجهزة 
        //    /*66*/
        //    carDepreciationExpense = -502004002,     //مصروف اهلاك السيارات
        //    /*67*/
        //    furnitureAndFurnishingsDepreciationExpense = -502004003,     //مصروف اهلاك الاثاث والمفروشات
        //    /*68*/
        //    OtherAuthorities = -502005,        //جهات صرف اخري
        //    /*69*/
        //    StaffSalariesAndWages = -502006,        //رواتب  واجور الموظفين
        //    /*70*/
        //    DefultEmployee = -502006001,     //موظف افتراضي
        //    /*70*/
        //    DefultCasher = -502006002,     //كاشير افتراضي
        //}
        //public static List<GLFinancialAccount> DefultGLFinancialAccountList()
        //{
        //    var list = new List<GLFinancialAccount>();
        //    list.AddRange(new[]
        //    {

        //    /*Level 1*/
        //    /*1*/   new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.assets,
        //            IsMain=true,
        //            ArabicName= "الاصول" ,
        //            LatinName="Assets",
        //            Status=(int)Status.Active,
        //            ParentId=null,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Debit,
        //            FinalAccount=1,
        //            AccountCode="01",
        //            autoCoding ="1",
        //            MainCode=1,
        //            SubCode=0
        //        }, //الاصول
        //    /*2*/   new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.fixedAssets,
        //            IsMain=true,
        //            ArabicName= "الاصول الثابتة" ,
        //            LatinName="Fixed Assets",
        //            Status=(int)Status.Active,
        //            ParentId=(int)FinanicalAccountDefultIds.assets,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Debit,
        //            FinalAccount=1,
        //            AccountCode="01.0001",
        //            autoCoding ="1.1",
        //            MainCode=1,
        //            SubCode=1
        //        }, //"الاصول الثابتة"
        //    /*3*/   new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.Equipment,
        //            IsMain=true,
        //            ArabicName= "اجهزة ومعدات" ,
        //            LatinName="Equipment",
        //            Status=(int)Status.Active,
        //            ParentId=(int)FinanicalAccountDefultIds.fixedAssets,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Debit,
        //            FinalAccount=1,
        //            AccountCode="01.0001.0001",
        //            autoCoding ="1.1.1",
        //            MainCode=1,
        //            SubCode=1
        //        }, //"اجهزة ومعدات"
        //    /*4*/   new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.computers,
        //            IsMain=false,
        //            ArabicName= "اجهزة كمبيوتر" ,
        //            LatinName="Computers",
        //            Status=(int)Status.Active,
        //            ParentId=(int)FinanicalAccountDefultIds.Equipment,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Debit,
        //            FinalAccount=1,
        //            AccountCode="01.0001.0001.0001",
        //            autoCoding ="1.1.1.1",
        //            MainCode=1,
        //            SubCode=1
        //        }, //"اجهزة كمبيوتر"
        //    /*5*/   new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.furniture,
        //            IsMain=true,
        //            ArabicName= "اثاث ومفروشات" ,
        //            LatinName="Furniture",
        //            Status=(int)Status.Active,
        //            ParentId=(int)FinanicalAccountDefultIds.fixedAssets,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Debit,
        //            FinalAccount=1,
        //            AccountCode="01.0001.0002",
        //            autoCoding ="1.1.2",
        //            MainCode=1,
        //            SubCode=2
        //        }, //"اثاث ومفروشات"
        //    /*6*/   new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.offices,
        //            IsMain=true,
        //            ArabicName= "مكاتب" ,
        //            LatinName="Offices",
        //            Status=(int)Status.Active,
        //            ParentId=(int)FinanicalAccountDefultIds.furniture,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Debit,
        //            FinalAccount=1,
        //            AccountCode="01.0001.0002.0001",
        //            autoCoding ="1.1.2.1",
        //            MainCode=1,
        //            SubCode=1
        //        }, //"مكاتب"
        //    /*7*/   new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.cars,
        //            IsMain=true,
        //            ArabicName= "سيارات" ,
        //            LatinName="Cars",
        //            Status=(int)Status.Active,
        //            ParentId=(int)FinanicalAccountDefultIds.fixedAssets,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Debit,
        //            FinalAccount=1,
        //            AccountCode="01.0001.0003",
        //            autoCoding ="1.1.3",
        //            MainCode=1,
        //            SubCode=3
        //        }, //"سيارات"
        //    /*8*/   new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.PrivateCars,
        //            IsMain=false,
        //            ArabicName= "سيارات خاصة" ,
        //            LatinName="Cars",
        //            Status=(int)Status.Active,
        //            ParentId=(int)FinanicalAccountDefultIds.cars,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Debit,
        //            FinalAccount=1,
        //            AccountCode="01.0001.0003.0001",
        //            autoCoding ="1.1.3.1",
        //            MainCode=1,
        //            SubCode=1
        //        }, //"سيارات خاصة"
        //    /*9*/   new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.LandsAndRealEstate,
        //            IsMain=true,
        //            ArabicName= "اراضي وعقارات" ,
        //            LatinName="Lands And Real Estate",
        //            Status=(int)Status.Active,
        //            ParentId=(int)FinanicalAccountDefultIds.fixedAssets,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Debit,
        //            FinalAccount=1,
        //            AccountCode="01.0001.0004",
        //            autoCoding ="1.1.4",
        //            MainCode=1,
        //            SubCode=4
        //        }, //"اراضي وعقارات"
        //    /*10*/  new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.lands,
        //            IsMain=false,
        //            ArabicName= "الاراضي" ,
        //            LatinName="Lands And Real Estate",
        //            Status=(int)Status.Active,
        //            ParentId=(int)FinanicalAccountDefultIds.LandsAndRealEstate,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Debit,
        //            FinalAccount=1,
        //            AccountCode="01.0001.0004.0001",
        //            autoCoding ="1.1.4.1",
        //            MainCode=1,
        //            SubCode=1
        //        }, //"الاراضي"
        //    /*11*/  new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.longTermOpponents,
        //            IsMain=true,
        //            ArabicName= "الاصول المتداولة " ,
        //            LatinName="long Term Opponents",
        //            Status=(int)Status.Active,
        //            ParentId=(int)FinanicalAccountDefultIds.assets,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Debit,
        //            FinalAccount=1,
        //            AccountCode="01.0002",
        //            autoCoding ="1.2",
        //            MainCode=1,
        //            SubCode=2
        //        },//الاصول المتداولة 
        //    /*12*/  new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.banks,
        //            IsMain=true,
        //            ArabicName= "البنوك" ,
        //            LatinName="Banks",
        //            Status=(int)Status.Active,
        //            ParentId=(int)FinanicalAccountDefultIds.longTermOpponents,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Debit,
        //            FinalAccount=1,
        //            AccountCode="01.0002.0001",
        //            autoCoding ="1.2.1",
        //            MainCode=1,
        //            SubCode=1
        //        }, //البنوك
        //    /*13*/  new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.DefultBank,
        //            IsMain=false,
        //            ArabicName= "بنك افتراضي" ,
        //            LatinName="virtual Bank",
        //            Status=(int)Status.Active,
        //            ParentId=(int)FinanicalAccountDefultIds.banks,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Debit,
        //            FinalAccount=1,
        //            AccountCode="01.0002.0001.0001",
        //            autoCoding ="1.2.1.1",
        //            MainCode=1,
        //            SubCode=1
        //        }, //"بنك افتراضي"
        //    /*14*/  new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.Safes,
        //            IsMain=true,
        //            ArabicName= "الخزائن" ,
        //            LatinName="Safes",
        //            Status=(int)Status.Active,
        //            ParentId=(int)FinanicalAccountDefultIds.longTermOpponents,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Debit,
        //            FinalAccount=1,
        //            AccountCode="01.0002.0002",
        //            autoCoding ="1.2.2",
        //            MainCode=1,
        //            SubCode=2
        //        }, //"الخزائن"
        //    /*15*/  new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.MasterSafe,
        //            IsMain=false,
        //            ArabicName= "خزينة رئيسية" ,
        //            LatinName="Master Safe",
        //            Status=(int)Status.Active,
        //            ParentId=(int)FinanicalAccountDefultIds.Safes,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Debit,
        //            FinalAccount=1,
        //            AccountCode="01.0002.0002.0001",
        //            autoCoding ="1.2.2.1",
        //            MainCode=1,
        //            SubCode=1
        //        }, //"خزينة رئيسية"
        //    /*16*/  new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.Customers,
        //            IsMain=true,
        //            ArabicName= "العملاء" ,
        //            LatinName="Customers",
        //            Status=(int)Status.Active,
        //            ParentId=(int)FinanicalAccountDefultIds.longTermOpponents,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Debit,
        //            FinalAccount=1,
        //            AccountCode="01.0002.0003",
        //            autoCoding ="1.2.3",
        //            MainCode=1,
        //            SubCode=3
        //        },//العملاء
        //    /*17*/  new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.cashCustomer,
        //            IsMain=false,
        //            ArabicName= "عميل نقدي" ,
        //            LatinName="Cash Customer",
        //            Status=(int)Status.Active,
        //            ParentId=(int)FinanicalAccountDefultIds.Customers,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Debit,
        //            FinalAccount=1,
        //            AccountCode="01.0002.0003.0001",
        //            autoCoding ="1.2.3.1",
        //            MainCode=1,
        //            SubCode=1
        //        }, //"عميل نقدي"
        //    /*18*/  new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.Inventory,
        //            IsMain=false,
        //            ArabicName= "البضاعه بالمخزن" ,
        //            LatinName="Inventory",
        //            Status=(int)Status.Active,
        //            ParentId=(int)FinanicalAccountDefultIds.longTermOpponents,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Debit,
        //            FinalAccount=1,
        //            AccountCode="01.0002.0004",
        //            autoCoding ="1.2.4",
        //            MainCode=1,
        //            SubCode=4
        //        }, //"البضاعه بالمخزن"
        //    /*19*/  new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.StaffReceivablesAndAdvances,
        //            IsMain=true,
        //            ArabicName= "ذمم الموظفين والسلف" ,
        //            LatinName="Staff Receivables  And Advances",
        //            Status=(int)Status.Active,
        //            ParentId=(int)FinanicalAccountDefultIds.longTermOpponents,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Debit,
        //            FinalAccount=1,
        //            AccountCode="01.0002.0005",
        //            autoCoding ="1.2.5",
        //            MainCode=1,
        //            SubCode=5
        //        }, //"ذمم الموظفين والسلف"
        //    /*20*/  new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.OtherDebitBalances,
        //            IsMain=true,
        //            ArabicName= "أرصدة مدينة اخري" ,
        //            LatinName="Other Debit Balances",
        //            Status=(int)Status.Active,
        //            ParentId=(int)FinanicalAccountDefultIds.longTermOpponents,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Debit,
        //            FinalAccount=1,
        //            AccountCode="01.0002.0006",
        //            autoCoding ="1.2.6",
        //            MainCode=1,
        //            SubCode=6
        //        }, //"أرصدة مدينة اخري"
        //    /*21*/  new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.otherDebitBalances,
        //            IsMain=false,
        //            ArabicName= "أرصدة مدينة اخري" ,
        //            LatinName="Other Debit Balances",
        //            Status=(int)Status.Active,
        //            ParentId=(int)FinanicalAccountDefultIds.OtherDebitBalances,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Debit,
        //            FinalAccount=1,
        //            AccountCode="01.0002.0006.0001",
        //            autoCoding ="1.2.6.1",
        //            MainCode=1,
        //            SubCode=1
        //        }, //"أرصدة مدينة اخري"
        //    /*Level 2*/
        //    /*22*/  new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.liabilities,
        //            IsMain=true,
        //            ArabicName= "الخصوم" ,
        //            LatinName="Liabilities",
        //            Status=(int)Status.Active,
        //            ParentId=null,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Credit,
        //            FinalAccount=1,
        //            AccountCode="02",
        //            autoCoding ="2",
        //            MainCode=2,
        //            SubCode=0
        //        }, //"الخصوم"
        //    /*23*/  new GLFinancialAccount
        //        {
        //            Id = (int)FinanicalAccountDefultIds.currentLiabilities,
        //            IsMain=true,
        //            ArabicName= "الخصوم  طويلة الاجل " ,
        //            LatinName="Current Liabilities",
        //            Status=(int)Status.Active,
        //            ParentId=(int)FinanicalAccountDefultIds.liabilities,
        //            CurrencyId=1,
        //            BranchId=1,
        //            FA_Nature=(int)FA_Nature.Credit,
        //            FinalAccount=1,
        //            AccountCode="02.0001",
        //            autoCoding ="2.1",
        //            MainCode=2,
        //            SubCode=1
        //        }, //"الخصوم  طويلة الاجل "
        //    /*24*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.loans,
        //                IsMain=false,
        //                ArabicName= "قروض" ,
        //                LatinName="Loans",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.currentLiabilities,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=1,
        //                AccountCode="02.0001.0001",
        //                autoCoding ="2.1.1",
        //                MainCode=2,
        //                SubCode=1
        //            }, //"قروض"
        //    /*25*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.LongTermOpponents,
        //                IsMain=true,
        //                ArabicName= "الخصوم المتداولة" ,
        //                LatinName="Long Term Opponents",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.liabilities,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=1,
        //                AccountCode="02.0002",
        //                autoCoding ="2.2",
        //                MainCode=2,
        //                SubCode=2
        //            }, //"الخصوم المتداولة"
        //    /*26*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.Suppliers,
        //                IsMain=true,
        //                ArabicName= "الموردين" ,
        //                LatinName="Suppliers",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.LongTermOpponents,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=1,
        //                AccountCode="02.0002.0001",
        //                autoCoding ="2.2.1",
        //                MainCode=2,
        //                SubCode=1
        //            },//"الموردين"
        //    /*27*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.cashSuppliers,
        //                IsMain=false,
        //                ArabicName= "مورد نقدي" ,
        //                LatinName="Cash Suppliers",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.Suppliers,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=1,
        //                AccountCode="02.0002.0001.0001",
        //                autoCoding ="2.2.1.1",
        //                MainCode=2,
        //                SubCode=1
        //            },//"مورد نقدي"
        //    /*28*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.ValueAddedTax,
        //                IsMain=true,
        //                ArabicName= "ضريبة القيمة المضافة" ,
        //                LatinName="Value Added Tax",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.LongTermOpponents,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=1,
        //                AccountCode="02.0002.0002",
        //                autoCoding ="2.2.2",
        //                MainCode=2,
        //                SubCode=2
        //            },//"ضريبة القيمة المضافة"
        //    /*29*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.VATPurchases,
        //                IsMain=false,
        //                ArabicName= "ضريبة القيمة المضافة مشتريات" ,
        //                LatinName="VAT Purchases",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.ValueAddedTax,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=1,
        //                AccountCode="02.0002.0002.0001",
        //                autoCoding ="2.2.2.1",
        //                MainCode=2,
        //                SubCode=1
        //            },//"ضريبة القيمة المضافة مشتريات"
        //    /*30*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.VATSales,
        //                IsMain=false,
        //                ArabicName= "ضريبة القيمة المضافة مبيعات" ,
        //                LatinName="VAT Sales",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.ValueAddedTax,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=1,
        //                AccountCode="02.0002.0002.0002",
        //                autoCoding ="2.2.2.2",
        //                MainCode=2,
        //                SubCode=2
        //            },//"ضريبة القيمة المضافة مبيعات"
        //    /*31*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.OtherCreditBalances,
        //                IsMain=true,
        //                ArabicName= "أرصدة دائنة اخري" ,
        //                LatinName="Other Credit Balances",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.LongTermOpponents,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=1,
        //                AccountCode="02.0002.0003",
        //                autoCoding ="2.2.3",
        //                MainCode=2,
        //                SubCode=3
        //            },//أرصدة دائنة اخري
        //    /*32*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.otherCreditBalances,
        //                IsMain=false,
        //                ArabicName= "أرصدة دائنة اخري" ,
        //                LatinName="Other Credit Balances",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.OtherCreditBalances,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=1,
        //                AccountCode="02.0002.0003.0001",
        //                autoCoding ="2.2.3.1",
        //                MainCode=2,
        //                SubCode=1
        //            },//أرصدة دائنة اخري
        //    /*Level 3*/
        //    /*33*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.propertyRights,
        //                IsMain=true,
        //                ArabicName= "حقوق الملكية" ,
        //                LatinName="Property Rights",
        //                Status=(int)Status.Active,
        //                ParentId=null,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=1,
        //                AccountCode="03",
        //                autoCoding ="3",
        //                MainCode=3,
        //                SubCode=0
        //            },//"حقوق الملكية"
        //    /*34*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.capital,
        //                IsMain=true,
        //                ArabicName= "راس المال" ,
        //                LatinName="Property Rights",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.propertyRights,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=1,
        //                AccountCode="03.0001",
        //                autoCoding ="3.1",
        //                MainCode=3,
        //                SubCode=1
        //            },//"راس المال "
        //    /*35*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.PartnersCapitalA,
        //                IsMain=false,
        //                ArabicName= "رأس مال الشريك أ" ,
        //                LatinName="Partners Capital A",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.capital,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=1,
        //                AccountCode="03.0001.0001",
        //                autoCoding ="3.1.1",
        //                MainCode=3,
        //                SubCode=1
        //            },//رأس مال الشريك أ
        //    /*36*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.PartnersCapitalB,
        //                IsMain=false,
        //                ArabicName= "رأس مال الشريك ب" ,
        //                LatinName="Partners Capital B ",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.capital,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=1,
        //                AccountCode="03.0001.0002",
        //                autoCoding ="3.1.2",
        //                MainCode=3,
        //                SubCode=2
        //            },//رأس مال الشريك ب
        //    /*37*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.stageProfitsAndLosses,
        //                IsMain=true,
        //                ArabicName= "أرباح وخسائر  مرحلة" ,
        //                LatinName="Stage Profits And Losses",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.propertyRights,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=1,
        //                AccountCode="03.0002",
        //                autoCoding ="3.2",
        //                MainCode=3,
        //                SubCode=2
        //            },//"أرباح وخسائر  مرحلة
        //    /*38*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.retainedEarnings,
        //                IsMain=false,
        //                ArabicName= "الارباح المحتجزة",
        //                LatinName="Retained Earnings",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.stageProfitsAndLosses,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=1,
        //                AccountCode="03.0002.0001",
        //                autoCoding ="3.2.1",
        //                MainCode=3,
        //                SubCode=1
        //            },//"الارباح المحتجزة"
        //    /*39*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.phaseLosses,
        //                IsMain=false,
        //                ArabicName= "الخسائر المرحلة",
        //                LatinName="Phase Losses",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.stageProfitsAndLosses,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=1,
        //                AccountCode="03.0002.0002",
        //                autoCoding ="3.2.2",
        //                MainCode=3,
        //                SubCode=2
        //            },//"الخسائر المرحلة"
        //    /*Level 4*/
        //    /*40*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.revenues,
        //                IsMain=true,
        //                ArabicName= "الايرادات",
        //                LatinName="Revenues",
        //                Status=(int)Status.Active,
        //                ParentId=null,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=2,
        //                AccountCode="04",
        //                autoCoding ="4",
        //                MainCode=4,
        //                SubCode=0
        //            },//"الايرادات"
        //    /*41*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.businessRevenue,
        //                IsMain=true,
        //                ArabicName= "إيرادات النشاط التجاري",
        //                LatinName="Business Revenue",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.revenues,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=2,
        //                AccountCode="04.0001",
        //                autoCoding ="4.1",
        //                MainCode=4,
        //                SubCode=1
        //            },//إيرادات النشاط التجاري
        //    /*42*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.Sales,
        //                IsMain=false,
        //                ArabicName= "المبيعات",
        //                LatinName="Sales",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.businessRevenue,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=2,
        //                AccountCode="04.0001.0001",
        //                autoCoding ="4.1.1",
        //                MainCode=4,
        //                SubCode=1
        //            },//المبيعات
        //    /*43*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.SalesReturns,
        //                IsMain=false,
        //                ArabicName= "مردودات المبيعات",
        //                LatinName="Sales Returns",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.businessRevenue,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="04.0001.0002",
        //                autoCoding ="4.1.2",
        //                MainCode=4,
        //                SubCode=2
        //            },//مردودات المبيعات
        //    /*44*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.SalesAllowances,
        //                IsMain=false,
        //                ArabicName= "مسموحات المبيعات",
        //                LatinName="Sales Returns",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.businessRevenue,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="04.0001.0003",
        //                autoCoding ="4.1.3",
        //                MainCode=4,
        //                SubCode=3
        //            },//مسموحات المبيعات
        //    /*45*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.DiscountPermitted,
        //                IsMain=false,
        //                ArabicName= "خصم مسموح بة",
        //                LatinName="Discount Permitted",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.businessRevenue,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="04.0001.0004",
        //                autoCoding ="4.1.4",
        //                MainCode=4,
        //                SubCode=4
        //            },//خصم مسموح بة

        //    /*46*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.otherIncome,
        //                IsMain=true,
        //                ArabicName= "إيرادات اخري",
        //                LatinName="Other Income",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.revenues,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=2,
        //                AccountCode="04.0002",
        //                autoCoding ="4.2",
        //                MainCode=4,
        //                SubCode=2
        //            },//إيرادات اخري
        //    /*47*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.BankBenefits,
        //                IsMain=false,
        //                ArabicName= "فوائد بنكية",
        //                LatinName="Bank Benefits",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.otherIncome,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=2,
        //                AccountCode="04.0002.0001",
        //                autoCoding ="4.2.1",
        //                MainCode=4,
        //                SubCode=1
        //            },//فوائد بنكية
        //    /*Level 5*/
        //    /*48*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.expensesAndCosts,
        //                IsMain=true,
        //                ArabicName= "المصروفات و التكاليف",
        //                LatinName="Expenses And Costs",
        //                Status=(int)Status.Active,
        //                ParentId=null,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="05",
        //                autoCoding ="5",
        //                MainCode=5,
        //                SubCode=0
        //            },//المصروفات و التكاليف
        //    /*49*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.businessCost,
        //                IsMain=true,
        //                ArabicName= "تكلفة النشاط التجاري",
        //                LatinName="Expenses And Costs",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.expensesAndCosts,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="05.0001",
        //                autoCoding ="5.1",
        //                MainCode=5,
        //                SubCode=1
        //            },//تكلفة النشاط التجاري   
        //    /*50*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.FirstTermMerchandise,
        //                IsMain=false,
        //                ArabicName= "بضاعة أول المدة",
        //                LatinName="Expenses And Costs",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.businessCost,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="05.0001.0001",
        //                autoCoding ="5.1.1",
        //                MainCode=5,
        //                SubCode=1
        //            },//بضاعة أول المدة  

        //    /*51*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.Purchases,
        //                IsMain=false,
        //                ArabicName= "المشتريات",
        //                LatinName="Purchases",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.businessCost,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="05.0001.0002",
        //                autoCoding ="5.1.2",
        //                MainCode=5,
        //                SubCode=2
        //            },//المشتريات
        //    /*52*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.ReturnsPurchases,
        //                IsMain=false,
        //                ArabicName= "مردودات المشتريات",
        //                LatinName="Purchases",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.businessCost,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=2,
        //                AccountCode="05.0001.0003",
        //                autoCoding ="5.1.3",
        //                MainCode=5,
        //                SubCode=3
        //            },//مردودات المشتريات
        //     new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.EndTermMerchandise,
        //                IsMain=false,
        //                ArabicName= "بضاعة اخر المدة",
        //                LatinName="End Term Merchandise",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.businessCost,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=2,
        //                AccountCode="05.0001.0004",
        //                autoCoding ="5.1.4",
        //                MainCode=5,
        //                SubCode=4
        //            },//مردودات المشتريات
        //    /*53*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.ProcurementExpenses,
        //                IsMain=false,
        //                ArabicName= "مصاريف المشتريات",
        //                LatinName="Procurement Expenses",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.businessCost,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Credit,
        //                FinalAccount=2,
        //                AccountCode="05.0001.0005",
        //                autoCoding ="5.1.5",
        //                MainCode=5,
        //                SubCode=5
        //            },//مصاريف المشتريات
        //    /*54*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.AcquiredDiscount,
        //                IsMain=false,
        //                ArabicName= "خصم مكتسب",
        //                LatinName="Earned Discount",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.businessCost,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="05.0001.0006",
        //                autoCoding ="5.1.6",
        //                MainCode=5,
        //                SubCode=6
        //            },//خصم مكتسب
        //      /**/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.SalesCostAcc,
        //                IsMain=false,
        //                ArabicName= "تكلفه مبيعات ",
        //                LatinName="Sales Cost",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.businessCost,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="05.0001.0007",
        //                autoCoding ="5.1.7",
        //                MainCode=5,
        //                SubCode=7
        //            },//تكلفه المبيعات 
        //    /*55*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.expenses,
        //                IsMain=true,
        //                ArabicName= "المصروفات",
        //                LatinName="Procurement Expenses",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.expensesAndCosts,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="05.0002",
        //                autoCoding ="5.2",
        //                MainCode=5,
        //                SubCode=2
        //            },//المصروفات 
        //    /*56*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.AdministrativeExpenses,
        //                IsMain=true,
        //                ArabicName= "مصاريف ادارية",
        //                LatinName="Administrative Expenses",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.expenses,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="05.0002.0001",
        //                autoCoding ="5.2.1",
        //                MainCode=5,
        //                SubCode=1
        //            },//مصاريف ادارية
        //    /*57*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.Electricity,
        //                IsMain=false,
        //                ArabicName= "كهرباء",
        //                LatinName="Administrative Expenses",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.AdministrativeExpenses,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="05.0002.0001.0001",
        //                autoCoding ="5.2.1.1",
        //                MainCode=5,
        //                SubCode=1
        //            },//كهرباء 
        //    /*58*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.waters,
        //                IsMain=false,
        //                ArabicName= "مياه",
        //                LatinName="Waters",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.AdministrativeExpenses,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="05.0002.0001.0002",
        //                autoCoding ="5.2.1.2",
        //                MainCode=5,
        //                SubCode=2
        //            },//مياه
        //    /*59*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.SellingAndMarketingExpenses,
        //                IsMain=true,
        //                ArabicName= "مصاريف بيعية وتسوقية",
        //                LatinName="Selling And Marketing Expenses",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.expenses,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="05.0002.0002",
        //                autoCoding ="5.2.2",
        //                MainCode=5,
        //                SubCode=2
        //            },//مصاريف بيعية وتسوقية
        //    /*60*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.advertisment,
        //                IsMain=false,
        //                ArabicName= "مصاريف بيعية وتسوقية",
        //                LatinName="Selling And Marketing Expenses",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.SellingAndMarketingExpenses,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="05.0002.0002.0001",
        //                autoCoding ="5.2.2.1",
        //                MainCode=5,
        //                SubCode=1
        //            },//دعايا واعلان
        //    /*61*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.salesMan,
        //                IsMain=true,
        //                ArabicName= "مناديب البيع",
        //                LatinName="SalesMan",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.SellingAndMarketingExpenses,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="05.0002.0002.0002",
        //                autoCoding ="5.2.2.2",
        //                MainCode=5,
        //                SubCode=2
        //            },//مناديب البيع
        //    /*62*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.operatingExpenses,
        //                IsMain=true,
        //                ArabicName= "مصاريف تشغيلية",
        //                LatinName="Operating Expenses",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.expenses,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="05.0002.0003",
        //                autoCoding ="5.2.3",
        //                MainCode=5,
        //                SubCode=3
        //            },//مصاريف تشغيلية
        //    /*63*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.carMaintenance,
        //                IsMain=false,
        //                ArabicName= "صيانة السيارات",
        //                LatinName="Operating Expenses",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.operatingExpenses,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="05.0002.0003.0001",
        //                autoCoding ="5.2.3.1",
        //                MainCode=5,
        //                SubCode=1
        //            },//صيانة السيارات
        //    /*64*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.depreciationExpenses,
        //                IsMain=true,
        //                ArabicName= "مصاريف الاهلاك",
        //                LatinName="Operating Expenses",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.expenses,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="05.0002.0004",
        //                autoCoding ="5.2.4",
        //                MainCode=5,
        //                SubCode=4
        //            },//مصاريف الاهلاك
        //    /*65*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.equipmentDepreciationExpense,
        //                IsMain=false,
        //                ArabicName= "مصروف اهلاك الاجهزة ",
        //                LatinName="Equipment Depreciation Expense",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.depreciationExpenses,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="05.0002.0004.0001",
        //                autoCoding ="5.2.4.1",
        //                MainCode=5,
        //                SubCode=1
        //            },//مصروف اهلاك الاجهزة 
        //    /*66*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.carDepreciationExpense,
        //                IsMain=false,
        //                ArabicName= "مصروف اهلاك السيارات",
        //                LatinName="Car Depreciation Expense",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.depreciationExpenses,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="05.0002.0004.0002",
        //                autoCoding ="5.2.4.2",
        //                MainCode=5,
        //                SubCode=2
        //            },//مصروف اهلاك السيارات
        //    /*67*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.furnitureAndFurnishingsDepreciationExpense,
        //                IsMain=false,
        //                ArabicName= "مصروف اهلاك الاثاث والمفروشات",
        //                LatinName="Furniture And Furnishings Depreciation Expense",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.depreciationExpenses,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="05.0002.0004.0003",
        //                autoCoding ="5.2.4.3",
        //                MainCode=5,
        //                SubCode=3
        //            },//مصروف اهلاك الاثاث والمفروشات
        //    /*68*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.OtherAuthorities,
        //                IsMain=true,
        //                ArabicName= "جهات صرف اخري",
        //                LatinName="Other Exchanges",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.expenses,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=2,
        //                AccountCode="05.0002.0005",
        //                autoCoding ="5.2.5",
        //                MainCode=5,
        //                SubCode=5
        //            },//جهات صرف اخري
        //    /*69*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.StaffSalariesAndWages,
        //                IsMain=true,
        //                ArabicName= "رواتب  واجور الموظفين",
        //                LatinName="Staff Salaries And Wages",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.expenses,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=1,
        //                AccountCode="05.0002.0006",
        //                autoCoding ="5.2.6",
        //                MainCode=5,
        //                SubCode=6
        //            },//رواتب  واجور الموظفين
        //    /*69*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.DefultEmployee,
        //                IsMain=false,
        //                ArabicName= "موظف افتراضي",
        //                LatinName="Defult Employee",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.StaffSalariesAndWages,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=1,
        //                AccountCode="05.0002.0006.0001",
        //                autoCoding ="5.2.6.1",
        //                MainCode=5,
        //                SubCode=1
        //            },//موظف افتراضي
        //    /*69*/  new GLFinancialAccount
        //            {
        //                Id = (int)FinanicalAccountDefultIds.DefultCasher,
        //                IsMain=false,
        //                ArabicName= "كاشر افتراضي",
        //                LatinName="Defult Casher",
        //                Status=(int)Status.Active,
        //                ParentId=(int)FinanicalAccountDefultIds.StaffSalariesAndWages,
        //                CurrencyId=1,
        //                BranchId=1,
        //                FA_Nature=(int)FA_Nature.Debit,
        //                FinalAccount=1,
        //                AccountCode="05.0002.0006.0002",
        //                autoCoding ="5.2.6.2",
        //                MainCode=5,
        //                SubCode=1
        //            },//كاشر افتراضي
        //    });
        //    return list;
        //}

    }
    public class screensAndAction
    {
        public int mainFormCode { get; set; }
        public int subFormCode { get; set; }
        public Opretion opretion { get; set; }
    }
    public enum Opretion
    {
        Add = 1,
        Edit = 2,
        Open = 3,
        Delete = 4,
        Print = 5

    }
    public class APISList
    {
        public string EndPointName { get; set; }
        public screensAndAction screensAndAction { get; set; }
    }
    public static class returnAPISList
    {
        public static List<APISList> APISList()
        {

            var list = new List<APISList>();
            list.AddRange(new[]
            {

                //ItemFund
                new APISList
                {
                    EndPointName = "AddItemsFund",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Add,
                        mainFormCode = 1,
                        subFormCode = 1,
                    }
                },
                new APISList
                {
                    EndPointName = "GetAllItemsFund",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 1,
                        subFormCode = 1,
                    }
                },
                new APISList
                {
                    EndPointName = "UpdateItemsFund",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Edit,
                        mainFormCode = 1,
                        subFormCode = 1,
                    }
                },
                new APISList
                {
                    EndPointName = "DeleteItemsFund",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Delete,
                        mainFormCode = 1,
                        subFormCode = 1,
                    }
                },
                new APISList
                {
                    EndPointName = "",//<= add end point name of print
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Print,
                        mainFormCode = 1,
                        subFormCode = 1,
                    }
                },
                //Safes Fund
                new APISList
                {
                    EndPointName = "AddFundsBankSafe",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Add,
                        mainFormCode = 1,
                        subFormCode = 2,
                    }
                },
                new APISList
                {
                    EndPointName = "GetListOfFundsBanksSafes",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 1,
                        subFormCode = 2,
                    }
                },
                new APISList
                {
                    EndPointName = "UpdateFundsBankSafe",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Edit,
                        mainFormCode = 1,
                        subFormCode = 2,
                    }
                },
                new APISList
                {
                    EndPointName = "DeleteFundsBankSafe",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Delete,
                        mainFormCode = 1,
                        subFormCode = 2,
                    }
                },
                new APISList
                {
                    EndPointName = "",//<= add end point name of print
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Print,
                        mainFormCode = 1,
                        subFormCode = 2,
                    }
                },
                //Banks Fund
                new APISList
                {
                    EndPointName = "AddFundsBankSafe",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Add,
                        mainFormCode = 1,
                        subFormCode = 3,
                    }
                },
                new APISList
                {
                    EndPointName = "GetListOfFundsBanksSafes",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 1,
                        subFormCode = 3,
                    }
                },
                new APISList
                {
                    EndPointName = "GetFundsById",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 1,
                        subFormCode = 3,
                    }
                },
                new APISList
                {
                    EndPointName = "UpdateFundsBankSafe",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Edit,
                        mainFormCode = 1,
                        subFormCode = 3,
                    }
                },
                new APISList
                {
                    EndPointName = "DeleteFundsBankSafe",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Delete,
                        mainFormCode = 1,
                        subFormCode = 3,
                    }
                },
                new APISList
                {
                    EndPointName = "",//<= add end point name of print
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Print,
                        mainFormCode = 1,
                        subFormCode = 3,
                    }
                },
                //Suppliers Fund
                new APISList
                {
                    EndPointName = "UpdateFundsSuppliers",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Add,
                        mainFormCode = 1,
                        subFormCode = 4,
                    }
                },
                new APISList
                {
                    EndPointName = "GetListOfFundsSuppliers",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 1,
                        subFormCode = 4,
                    }
                },
                new APISList
                {
                    EndPointName = "UpdateFundsSuppliers",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Edit,
                        mainFormCode = 1,
                        subFormCode = 4,
                    }
                },
                new APISList
                {
                    EndPointName = "UpdateFundsSuppliers",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Delete,
                        mainFormCode = 1,
                        subFormCode = 4,
                    }
                },
                new APISList
                {
                    EndPointName = "",//<= add end point name of print
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Print,
                        mainFormCode = 1,
                        subFormCode = 4,
                    }
                },
                //Customers Fund
                new APISList
                {
                    EndPointName = "UpdateFundsCustomers",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Add,
                        mainFormCode = 1,
                        subFormCode = 5,
                    }
                },
                new APISList
                {
                    EndPointName = "GetListOfFundsCustomers",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 1,
                        subFormCode = 5,
                    }
                },
                new APISList
                {
                    EndPointName = "GetFundsById",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 1,
                        subFormCode = 5,
                    }
                },
                new APISList
                {
                    EndPointName = "UpdateFundsCustomers",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Edit,
                        mainFormCode = 1,
                        subFormCode = 5,
                    }
                },
                new APISList
                {
                    EndPointName = "UpdateFundsCustomers",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Delete,
                        mainFormCode = 1,
                        subFormCode = 5,
                    }
                },
                new APISList
                {
                    EndPointName = "",//<= add end point name of print
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Print,
                        mainFormCode = 1,
                        subFormCode = 5,
                    }
                },




                //Branchs
                new APISList
                {
                    EndPointName = "CreateNewBranch",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Add,
                        mainFormCode = 2,
                        subFormCode = 2,
                    }
                },
                new APISList
                {
                    EndPointName = "GetAllBranches",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 2,
                        subFormCode = 2,
                    }
                },
                new APISList
                {
                    EndPointName = "GetBranchById",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 2,
                        subFormCode = 2,
                    }
                },
                new APISList
                {
                    EndPointName = "GetAllBranchHistory",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 2,
                        subFormCode = 2,
                    }
                },
                new APISList
                {
                    EndPointName = "UpdateBranch",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Edit,
                        mainFormCode = 2,
                        subFormCode = 2,
                    }
                },
                new APISList
                {
                    EndPointName = "UpdateBranchStatus",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Edit,
                        mainFormCode = 2,
                        subFormCode = 2,
                    }
                },
                new APISList
                {
                    EndPointName = "DeleteBranch",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Delete,
                        mainFormCode = 2,
                        subFormCode = 2,
                    }
                },
                new APISList
                {
                    EndPointName = "",//<= add end point name of print
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Print,
                        mainFormCode = 2,
                        subFormCode = 2,
                    }
                },

                //Curancy
                new APISList
                {
                    EndPointName = "CreateNewCurrency",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Add,
                        mainFormCode = 2,
                        subFormCode = 3,
                    }
                },
                new APISList
                {
                    EndPointName = "UpdateCurrency",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Edit,
                        mainFormCode = 2,
                        subFormCode = 3,
                    }
                },
                new APISList
                {
                    EndPointName = "UpdateCurrencyFactor",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Edit,
                        mainFormCode = 2,
                        subFormCode = 3,
                    }
                },
                new APISList
                {
                    EndPointName = "UpdateCurrencyStatus",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Edit,
                        mainFormCode = 2,
                        subFormCode = 3,
                    }
                },
                new APISList
                {
                    EndPointName = "DeleteCurrency",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Delete,
                        mainFormCode = 2,
                        subFormCode = 3,
                    }
                },
                new APISList
                {
                    EndPointName = "GetCurrencyById",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 2,
                        subFormCode = 3,
                    }
                },
                new APISList
                {
                    EndPointName = "GetAllCurrency",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 2,
                        subFormCode = 3,
                    }
                },
                new APISList
                {
                    EndPointName = "GetAllCurrencyHistory",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 2,
                        subFormCode = 3,
                    }
                }

                //Safes
                ,
                new APISList
                {
                    EndPointName = "CreateNewTreasury",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Add,
                        mainFormCode = 2,
                        subFormCode = 4,
                    }
                },
                new APISList
                {
                    EndPointName = "UpdateTreasury",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Edit,
                        mainFormCode = 2,
                        subFormCode = 4,
                    }
                },
                new APISList
                {
                    EndPointName = "UpdateTreasuryStatus",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Edit,
                        mainFormCode = 2,
                        subFormCode = 4,
                    }
                },
                new APISList
                {
                    EndPointName = "Deletetreasury",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Delete,
                        mainFormCode = 2,
                        subFormCode = 4,
                    }
                },
                new APISList
                {
                    EndPointName = "GetTreasuryById",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 2,
                        subFormCode = 4,
                    }
                },
                new APISList
                {
                    EndPointName = "GetAllTreasury",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 2,
                        subFormCode = 4,
                    }
                },
                new APISList
                {
                    EndPointName = "GetAllTreasuryHistory",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 2,
                        subFormCode = 4,
                    }
                },
                new APISList
                {
                    EndPointName = "GetAllTreasurySetting",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 2,
                        subFormCode = 4,
                    }
                }
                //Banks
                ,
                new APISList
                {
                    EndPointName = "CreateNewBanks",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Add,
                        mainFormCode = 2,
                        subFormCode = 5,
                    }
                },
                new APISList
                {
                    EndPointName = "UpdateBanks",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Edit,
                        mainFormCode = 2,
                        subFormCode = 5,
                    }
                },
                new APISList
                {
                    EndPointName = "UpdateBankStatus",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Edit,
                        mainFormCode = 2,
                        subFormCode = 5,
                    }
                },
                new APISList
                {
                    EndPointName = "DeleteBanks",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Delete,
                        mainFormCode = 2,
                        subFormCode = 5,
                    }
                },
                new APISList
                {
                    EndPointName = "GetBanksById",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 2,
                        subFormCode = 5,
                    }
                },
                new APISList
                {
                    EndPointName = "GetAllBanks",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 2,
                        subFormCode = 5,
                    }
                },
                new APISList
                {
                    EndPointName = "GetAllBankHistory",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 2,
                        subFormCode = 5,
                    }
                },
                new APISList
                {
                    EndPointName = "GetAllBankSetting",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 2,
                        subFormCode = 5,
                    }
                }
                //Other Authorities
                ,
                new APISList
                {
                    EndPointName = "AddOtherAuthorities",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Add,
                        mainFormCode = 2,
                        subFormCode = 6,
                    }
                },
                new APISList
                {
                    EndPointName = "GetAllOtherAuthorities",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 2,
                        subFormCode = 6,
                    }
                },
                new APISList
                {
                    EndPointName = "UpdateOtherAuthorities",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Edit,
                        mainFormCode = 2,
                        subFormCode = 6,
                    }
                },
                new APISList
                {
                    EndPointName = "UpdateOthorStatus",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Edit,
                        mainFormCode = 2,
                        subFormCode = 6,
                    }
                },
                new APISList
                {
                    EndPointName = "GetOtherAuthoritieById",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 2,
                        subFormCode = 6,
                    }
                },
                new APISList
                {
                    EndPointName = "GetOtherAuthorityHistory",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 2,
                        subFormCode = 6,
                    }
                },
                new APISList
                {
                    EndPointName = "DeleteOtherAuthorities",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Delete,
                        mainFormCode = 2,
                        subFormCode = 6,
                    }
                }
                //Finanical Account
                ,
                new APISList
                {
                    EndPointName = "CreateNewFinancialAccount",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Add,
                        mainFormCode = 2,
                        subFormCode = 6,
                    }
                },
                new APISList
                {
                    EndPointName = "UpdateFinancialAccount",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Edit,
                        mainFormCode = 2,
                        subFormCode = 6,
                    }
                },
                new APISList
                {
                    EndPointName = "UpdateFinancialAccountsStatus",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Edit,
                        mainFormCode = 2,
                        subFormCode = 6,
                    }
                },
                new APISList
                {
                    EndPointName = "DeleteFinancialAccount",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Delete,
                        mainFormCode = 2,
                        subFormCode = 6,
                    }
                },
                new APISList
                {
                    EndPointName = "GetFinancialAccountById",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 2,
                        subFormCode = 6,
                    }
                },
                new APISList
                {
                    EndPointName = "GetAllFinancialAccount",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 2,
                        subFormCode = 6,
                    }
                },
                new APISList
                {
                    EndPointName = "GetAllFinancialAccountHistory",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Open,
                        mainFormCode = 2,
                        subFormCode = 6,
                    }
                },
                new APISList
                {
                    EndPointName = "MoveFinancialAccount",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Edit,
                        mainFormCode = 2,
                        subFormCode = 6,
                    }
                },
                new APISList
                {
                    EndPointName = "RecodingFinancialAccount",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Edit,
                        mainFormCode = 2,
                        subFormCode = 6,
                    }
                },
                new APISList
                {
                    EndPointName = "GetAccountInformation",
                    screensAndAction = new screensAndAction
                    {
                        opretion = Opretion.Edit,
                        mainFormCode = 2,
                        subFormCode = 6,
                    }
                }
                //

            });
            return list;
        }

    }
    public class SystemActions
    {
        public static List<SystemActionProps> systemActionList()
        {
            List<SystemActionProps> systemActionProps = new List<SystemActionProps>();
            systemActionProps.AddRange(new[]
            {
                //items entry fund
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addItemsEntryFund,
                    ArabicTransactionType = "اضافة اصناف اول المدة",
                    LatinTransactionType = "Add Items Entry Fund"
                },
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editItemsEntryFund,
                    ArabicTransactionType = "تعديل اصناف اول المدة",
                    LatinTransactionType = "Edit Items Entry Fund"
                },
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteItemsEntryFund,
                    ArabicTransactionType = "حذف اصناف اول المدة",
                    LatinTransactionType = "Delete Items Entry Fund"
                },

                //Safes Entry Fund
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addSafesFund,
                    ArabicTransactionType = "اضافة ارصدة اول المدة خزائن",
                    LatinTransactionType = "Add Safes Entry Fund"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editSafesFund,
                    ArabicTransactionType = "تعديل ارصدة اول المدة خزائن",
                    LatinTransactionType = "Edit Safes Entry Fund"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteSafesFund,
                    ArabicTransactionType = "حذف ارصدة اول المدة خزائن",
                    LatinTransactionType = "Delete Safes Entry Fund"
                },
                  //Banks Entry Fund
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addBanksFund,
                    ArabicTransactionType = "اضافة ارصدة اول المدة بنوك",
                    LatinTransactionType = "Add Banks Entry Fund"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editBanksFund,
                    ArabicTransactionType = "تعديل ارصدة اول المدة بنوك",
                    LatinTransactionType = "Edit Banks Entry Fund"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteBanksFund,
                    ArabicTransactionType = "حذف ارصدة اول المدة بنوك",
                    LatinTransactionType = "Delete Banks Entry Fund"
                },

                   //Suppliers Entry Fund
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addSuppliersFund,
                    ArabicTransactionType = "اضافة ارصدة اول المدة موردين",
                    LatinTransactionType = "Add Suppliers Entry Fund"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editSuppliersFund,
                    ArabicTransactionType = "تعديل ارصدة اول المدة موردين",
                    LatinTransactionType = "Edit Suppliers Entry Fund"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteSuppliersFund,
                    ArabicTransactionType = "حذف ارصدة اول المدة موردين",
                    LatinTransactionType = "Delete Suppliers Entry Fund"
                },

                  //Customers Entry Fund
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addCustomersFund,
                    ArabicTransactionType = "اضافة ارصدة اول المدة عملاء",
                    LatinTransactionType = "Add Customers Entry Fund"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editCustomersFund,
                    ArabicTransactionType = "تعديل ارصدة اول المدة عملاء",
                    LatinTransactionType = "Edit Customers Entry Fund"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteCustomersFund,
                    ArabicTransactionType = "حذف ارصدة اول المدة عملاء",
                    LatinTransactionType = "Delete Customers Entry Fund"
                },

                   //Branchs
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addBranch,
                    ArabicTransactionType = "اضافة فرع",
                    LatinTransactionType = "Add Branch"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editBranch,
                    ArabicTransactionType = "تعديل فرع",
                    LatinTransactionType = "Edit Branch"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteBranch,
                    ArabicTransactionType = "حذف  فرع",
                    LatinTransactionType = "Delete Branch"
                },

                  //Currancy
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addCurrancy,
                    ArabicTransactionType = "اضافة العملات",
                    LatinTransactionType = "Add Currancy"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editCurrancy,
                    ArabicTransactionType = "تعديل العملات",
                    LatinTransactionType = "Edit Currancy"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteCurrancy,
                    ArabicTransactionType = "حذف  العملات",
                    LatinTransactionType = "Delete Currancy"
                },

                  //Safes
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addSafes,
                    ArabicTransactionType = "اضافة الخزائن",
                    LatinTransactionType = "Add Safes"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editSafes,
                    ArabicTransactionType = "تعديل الخزائن",
                    LatinTransactionType = "Edit Safes"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteSafes,
                    ArabicTransactionType = "حذف  الخزائن",
                    LatinTransactionType = "Delete Safes"
                },

                   //Banks
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addBanks,
                    ArabicTransactionType = "اضافة بنك",
                    LatinTransactionType = "Add Bank"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editBanks,
                    ArabicTransactionType = "تعديل بنك",
                    LatinTransactionType = "Edit Bank"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteBanks,
                    ArabicTransactionType = "حذف  بنك",
                    LatinTransactionType = "Delete Bank"
                },



                   //OtherAuthorities
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addOtherAuthorities,
                    ArabicTransactionType = "اضافة جهات صرف اخري",
                    LatinTransactionType = "Add Other Authorities"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editOtherAuthorities,
                    ArabicTransactionType = "تعديل جهات صرف اخري",
                    LatinTransactionType = "Edit Other Authorities"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteOtherAuthorities,
                    ArabicTransactionType = "حذف  جهات صرف اخري",
                    LatinTransactionType = "Delete Other Authorities"
                },



                   //CalculationGuide
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addCalculationGuide,
                    ArabicTransactionType = "اضافة حساب مالي",
                    LatinTransactionType = "Add Calculation Guide"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editCalculationGuide,
                    ArabicTransactionType = "تعديل حساب مالي",
                    LatinTransactionType = "Edit Calculation Guide"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteCalculationGuide,
                    ArabicTransactionType = "حذف حساب مالي",
                    LatinTransactionType = "Delete Calculation Guide"
                },


                    //OpeningBalance
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addOpeningBalance,
                    ArabicTransactionType = "اضافة ارصدة افتتاحية",
                    LatinTransactionType = "Add Opening Balance"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editOpeningBalance,
                    ArabicTransactionType = "تعديل  ارصدة افتتاحية",
                    LatinTransactionType = "Edit Opening Balance"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteOpeningBalance,
                    ArabicTransactionType = "حذف  ارصدة افتتاحية",
                    LatinTransactionType = "Delete Opening Balance"
                },

                    //JournalEntry
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addJournalEntry,
                    ArabicTransactionType = "اضافة قيد",
                    LatinTransactionType = "Add Journal Entry"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editJournalEntry,
                    ArabicTransactionType = "تعديل  قيد",
                    LatinTransactionType = "Edit Journal Entry"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteJournalEntry,
                    ArabicTransactionType = "حذف  قيد",
                    LatinTransactionType = "Delete Journal Entry"
                },


                    //CostCenter
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addCostCenter,
                    ArabicTransactionType = "اضافة مركذ تكفلة",
                    LatinTransactionType = "Add Cost Center"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editCostCenter,
                    ArabicTransactionType = "تعديل  مركذ تكلفة",
                    LatinTransactionType = "Edit Cost Center"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteCostCenter,
                    ArabicTransactionType = "حذف  مركذ تكلفة",
                    LatinTransactionType = "Delete Cost Center"
                },


                      //SafePaymentReceipt
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addSafePaymentReceipt,
                    ArabicTransactionType = "اضافة سند صرف خزينة",
                    LatinTransactionType = "Add Safe Payment Receipt"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editSafePaymentReceipt,
                    ArabicTransactionType = "تعديل  سند صرف خزينة",
                    LatinTransactionType = "Edit Safe Payment Receipt"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteSafePaymentReceipt,
                    ArabicTransactionType = "حذف  سند صرف خزينة",
                    LatinTransactionType = "Delete Safe Payment Receipt"
                },


                          //BankPaymentReceipt
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addBankPaymentReceipt,
                    ArabicTransactionType = "اضافة سند صرف بنك",
                    LatinTransactionType = "Add Bank Payment Receipt"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editBankPaymentReceipt,
                    ArabicTransactionType = "تعديل  سند صرف بنك",
                    LatinTransactionType = "Edit Bank Payment Receipt"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteBankPaymentReceipt,
                    ArabicTransactionType = "حذف  سند صرف بنك",
                    LatinTransactionType = "Delete Bank Payment Receipt"
                },



                //SafeCashReceipt
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addSafeCashReceipt,
                    ArabicTransactionType = "اضافة سند قبض خزينة",
                    LatinTransactionType = "Add Safe Cash Receipt"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editSafeCashReceipt,
                    ArabicTransactionType = "تعديل  سند قبض خزينة",
                    LatinTransactionType = "Edit Safe Cash Receipt"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteSafeCashReceipt,
                    ArabicTransactionType = "حذف  سند قبض خزينة",
                    LatinTransactionType = "Delete Safe Cash Receipt"
                },


                //BankCashReceipt
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addBankCashReceipt,
                    ArabicTransactionType = "اضافة سند قبض بنك",
                    LatinTransactionType = "Add Bank Cash Receipt"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editBankCashReceipt,
                    ArabicTransactionType = "تعديل  سند قبض بنك",
                    LatinTransactionType = "Edit Bank Cash Receipt"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteBankCashReceipt,
                    ArabicTransactionType = "حذف  سند قبض بنك",
                    LatinTransactionType = "Delete Bank Cash Receipt"
                },
                  //SafeTransferReceipt
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addSafeTransferReceipt,
                    ArabicTransactionType = "اضافة تحويل قبض خزينة",
                    LatinTransactionType = "Add Safe Transfer Receipt"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editSafeTransferReceipt,
                    ArabicTransactionType = "تعديل تحويل قبض خزينة",
                    LatinTransactionType = "Edit Safe Transfer Receipt"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteSafeTransferReceipt,
                    ArabicTransactionType = "حذف تحويل قبض خزينة",
                    LatinTransactionType = "Delete Safe Transfer Receipt"
                },


                //BankCashReceipt
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addBankTransferReceipt,
                    ArabicTransactionType = "اضافةتحويل قبض بنك",
                    LatinTransactionType = "Add Bank Transfer Receipt"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editBankTransferReceipt,
                    ArabicTransactionType = "تعديل تحويل قبض بنك",
                    LatinTransactionType = "Edit Bank Transfer Receipt"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteBankTransferReceipt,
                    ArabicTransactionType = "حذف تحويل قبض بنك",
                    LatinTransactionType = "Delete Bank Transfer Receipt"
                },

                    //CompinedBankCashReceipt
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addCompinedBankCashReceipt,
                    ArabicTransactionType = "اضافة سند مجمع قبض بنك",
                    LatinTransactionType = "Add Compined Bank Cash Receipt"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editCompinedBankCashReceipt,
                    ArabicTransactionType = "تعديل  سند مجمع قبض بنك",
                    LatinTransactionType = "Edit Compined Bank Cash Receipt"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteCompinedBankCashReceipt,
                    ArabicTransactionType = "حذف  سند مجمع قبض بنك",
                    LatinTransactionType = "Delete Compined Bank Cash Receipt"
                },


                  //CompinedSafeCashReceipt
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addCompinedSafeCashReceipt,
                    ArabicTransactionType = "اضافة سند مجمع قبض خزينة",
                    LatinTransactionType = "Add Compined Safe Cash Receipt"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editCompinedSafeCashReceipt,
                    ArabicTransactionType = "تعديل  سند مجمع قبض خزينة",
                    LatinTransactionType = "Edit Compined Safe Cash Receipt"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteCompinedSafePaymentReceipt,
                    ArabicTransactionType = "حذف  سند مجمع قبض خزينة",
                    LatinTransactionType = "Delete Compined Safe Cash Receipt"
                },
                  
                  //CompinedSafePaymentReceipt
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addCompinedSafePaymentReceipt,
                    ArabicTransactionType = "اضافة سند مجمع صرف خزينة",
                    LatinTransactionType = "Add Compined Safe Payment Receipt"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editCompinedSafePaymentReceipt,
                    ArabicTransactionType = "تعديل  سند مجمع صرف خزينة",
                    LatinTransactionType = "Edit Compined Safe Payment Receipt"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteCompinedSafePaymentReceipt,
                    ArabicTransactionType = "حذف  سند مجمع صرف خزينة",
                    LatinTransactionType = "Delete Compined Safe Payment Receipt"
                },

                  //CompinedBankPaymentReceipt
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addCompinedBankPaymentReceipt,
                    ArabicTransactionType = "اضافة سند مجمع صرف بنك",
                    LatinTransactionType = "Add Compined Bank Payment Receipt"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editCompinedSafePaymentReceipt,
                    ArabicTransactionType = "تعديل  سند مجمع صرف بنك",
                    LatinTransactionType = "Edit Compined Bank Payment Receipt"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteCompinedSafePaymentReceipt,
                    ArabicTransactionType = "حذف  سند مجمع صرف بنك",
                    LatinTransactionType = "Delete Compined Bank Payment Receipt"
                },

                  //GeneralLedger
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editGeneralLedgerSettings,
                    ArabicTransactionType = "تعديل اعدادات الحسابات العامة",
                    LatinTransactionType = "Edit General Ledger Settings"
                },

                   //Units
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addUnits,
                    ArabicTransactionType = "اضافة وحدة",
                    LatinTransactionType = "Add Units"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editUnits,
                    ArabicTransactionType = "تعديل وحدة",
                    LatinTransactionType = "Edit Units"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteUnits,
                    ArabicTransactionType = "حذف وحدة",
                    LatinTransactionType = "Delete Units"
                },

                    //Size
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addSize,
                    ArabicTransactionType = "اضافة الحجم",
                    LatinTransactionType = "Add Size"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editSize,
                    ArabicTransactionType = "تعديل الحجم",
                    LatinTransactionType = "Edit Size"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteSize,
                    ArabicTransactionType = "حذف الحجم",
                    LatinTransactionType = "Delete Size"
                },

                     //Color
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addColor,
                    ArabicTransactionType = "اضافة الالوان",
                    LatinTransactionType = "Add Color"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editColor,
                    ArabicTransactionType = "تعديل الالوان",
                    LatinTransactionType = "Edit Color"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteColor,
                    ArabicTransactionType = "حذف الالوان",
                    LatinTransactionType = "Delete Color"
                },


                  
                     //ItemCategories
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addItemCategories,
                    ArabicTransactionType = "اضافة مجموعه اصناف",
                    LatinTransactionType = "Add Item Categories"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editItemCategories,
                    ArabicTransactionType = "تعديل مجموعه اصناف",
                    LatinTransactionType = "Edit Item Categories"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteItemCategories,
                    ArabicTransactionType = "حذف مجموعه اصناف",
                    LatinTransactionType = "Delete Item Categories"
                },

                //ItemCard
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addItemCard,
                    ArabicTransactionType = "اضافة صنف",
                    LatinTransactionType = "Add Item Card"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editItemCard,
                    ArabicTransactionType = "تعديل صنف",
                    LatinTransactionType = "Edit Item Card"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteItemCard,
                    ArabicTransactionType = "حذف صنف",
                    LatinTransactionType = "Delete Item Card"
                },


                //Employee
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addEmployee,
                    ArabicTransactionType = "اضافة موظف",
                    LatinTransactionType = "Add Employee"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editEmployee,
                    ArabicTransactionType = "تعديل موظف",
                    LatinTransactionType = "Edit Employee"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteEmployee,
                    ArabicTransactionType = "حذف موظف",
                    LatinTransactionType = "Delete Employee"
                },


                //Jobs
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addJobs,
                    ArabicTransactionType = "اضافة وظيفة",
                    LatinTransactionType = "Add Jobs"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editJobs,
                    ArabicTransactionType = "تعديل وظيفة",
                    LatinTransactionType = "Edit Jobs"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteJobs,
                    ArabicTransactionType = "حذف وظيفة",
                    LatinTransactionType = "Delete Jobs"
                },


                //Stores
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addStores,
                    ArabicTransactionType = "اضافة مستودع",
                    LatinTransactionType = "Add Stores"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editStores,
                    ArabicTransactionType = "تعديل مستودع",
                    LatinTransactionType = "Edit Stores"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteStores,
                    ArabicTransactionType = "حذف مستودع",
                    LatinTransactionType = "Delete Stores"
                },

                   //StoresPlace
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addStoresPlace,
                    ArabicTransactionType = "اضافة مكان تخزين",
                    LatinTransactionType = "Add Stores Place"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editStoresPlace,
                    ArabicTransactionType = "تعديل مكان تخزين",
                    LatinTransactionType = "Edit Stores Place"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteStoresPlace,
                    ArabicTransactionType = "حذف مكان تخزين",
                    LatinTransactionType = "Delete Stores Place"
                },

                //AddPermisson
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addAddPermisson,
                    ArabicTransactionType = "اضافة اذن اضافة",
                    LatinTransactionType = "Add Add Permisson"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editAddPermisson,
                    ArabicTransactionType = "تعديل مكان تخزين",
                    LatinTransactionType = "Edit Add Permisson"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteAddPermisson,
                    ArabicTransactionType = "حذف مكان تخزين",
                    LatinTransactionType = "Delete Add Permisson"
                },

             //Barcode
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addBarcode,
                    ArabicTransactionType = "اضافة باركود",
                    LatinTransactionType = "Add Barcode"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editBarcode,
                    ArabicTransactionType = "تعديل باركود",
                    LatinTransactionType = "Edit Barcode"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteBarcode,
                    ArabicTransactionType = "حذف باركود",
                    LatinTransactionType = "Delete Barcode"
                },

            //Barcode
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addBarcode,
                    ArabicTransactionType = "اضافة باركود",
                    LatinTransactionType = "Add Barcode"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editBarcode,
                    ArabicTransactionType = "تعديل باركود",
                    LatinTransactionType = "Edit Barcode"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteBarcode,
                    ArabicTransactionType = "حذف باركود",
                    LatinTransactionType = "Delete Barcode"
                },

                //Supplier
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addSupplier,
                    ArabicTransactionType = "اضافة مورد",
                    LatinTransactionType = "Add Supplier"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editSupplier,
                    ArabicTransactionType = "تعديل مورد",
                    LatinTransactionType = "Edit Supplier"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteSupplier,
                    ArabicTransactionType = "حذف مورد",
                    LatinTransactionType = "Delete Supplier"
                },

                //PurchaseInvoice
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addPurchaseInvoice,
                    ArabicTransactionType = "اضافة فاتورة مشتريات",
                    LatinTransactionType = "Add Purchase Invoice"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editPurchaseInvoice,
                    ArabicTransactionType = "تعديل فاتورة مشتريات",
                    LatinTransactionType = "Edit Purchase Invoice"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deletePurchaseInvoice,
                    ArabicTransactionType = "حذف فاتورة مشتريات",
                    LatinTransactionType = "Delete Purchase Invoice"
                },


                //ReturnPurchaseInvoice
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addReturnPurchaseInvoice,
                    ArabicTransactionType = "اضافة فاتورة مشتريات",
                    LatinTransactionType = "Add Return Purchase Invoice"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editReturnPurchaseInvoice,
                    ArabicTransactionType = "تعديل فاتورة مشتريات",
                    LatinTransactionType = "Edit Return Purchase Invoice"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteReturnPurchaseInvoice,
                    ArabicTransactionType = "حذف فاتورة مشتريات",
                    LatinTransactionType = "Delete Return Purchase Invoice"
                },



                   //PurchaseInvoicesAccredite
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addPurchaseInvoicesAccredite,
                    ArabicTransactionType = "اعتماد فواتير المشتريات",
                    LatinTransactionType = "Add Purchase Invoices Accredite"
                },


                 //Customer
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addCustomer,
                    ArabicTransactionType = "اضافة عميل",
                    LatinTransactionType = "Add Customer"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editCustomer,
                    ArabicTransactionType = "تعديل عميل",
                    LatinTransactionType = "Edit Customer"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteCustomer,
                    ArabicTransactionType = "حذف عميل",
                    LatinTransactionType = "Delete Customer"
                },



                //CommissionList
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addCommissionList,
                    ArabicTransactionType = "اضافة عميل",
                    LatinTransactionType = "Add Customer"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editCommissionList,
                    ArabicTransactionType = "تعديل عميل",
                    LatinTransactionType = "Edit Customer"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteCommissionList,
                    ArabicTransactionType = "حذف عميل",
                    LatinTransactionType = "Delete Customer"
                },


                //Salesmen
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addSalesmen,
                    ArabicTransactionType = "اضافة مندوب مبيعات",
                    LatinTransactionType = "Add Salesmen"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editSalesmen,
                    ArabicTransactionType = "تعديل مندوب مبيعات",
                    LatinTransactionType = "Edit Salesmen"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteSalesmen,
                    ArabicTransactionType = "حذف مندوب مبيعات",
                    LatinTransactionType = "Delete Salesmen"
                },


                //Sales Invoice
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addSalesInvoice,
                    ArabicTransactionType = "اضافة فاتورة مبيعات",
                    LatinTransactionType = "Add Sales Invoice"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editSalesInvoice,
                    ArabicTransactionType = "تعديل فاتورة مبيعات",
                    LatinTransactionType = "Edit Sales Invoice"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteSalesInvoice,
                    ArabicTransactionType = "حذف فاتورة مبيعات",
                    LatinTransactionType = "Delete Sales Invoice"
                },


                  //ReturnSalesInvoice
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addReturnSalesInvoice,
                    ArabicTransactionType = "اضافة مرتجع فاتورة مبيعات",
                    LatinTransactionType = "Add Return Sales Invoice"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editSalesInvoice,
                    ArabicTransactionType = "تعديل مرتجع فاتورة مبيعات",
                    LatinTransactionType = "Edit Return Sales Invoice"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteSalesInvoice,
                    ArabicTransactionType = "حذف مرتجع فاتورة مبيعات",
                    LatinTransactionType = "Delete Return Sales Invoice"
                },

                //SalesAccredite
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addSalesAccredite,
                    ArabicTransactionType = "اعتماد فواتير المبيعات",
                    LatinTransactionType = "Sales Accredite"
                },

                //AcquiredDiscount
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addAcquiredDiscount,
                    ArabicTransactionType = "اضافة خصم مكتسب",
                    LatinTransactionType = "Add Acquired Discount"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editAcquiredDiscount,
                    ArabicTransactionType = "تعديل خصم مكتسب",
                    LatinTransactionType = "Edit Acquired Discount"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteAcquiredDiscount,
                    ArabicTransactionType = "حذف خصم مكتسب",
                    LatinTransactionType = "Delete Acquired Discount"
                },


                //PermittedDiscount
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addPermittedDiscount,
                    ArabicTransactionType = "اضافة خصم مسموح به",
                    LatinTransactionType = "Add Permitted Discount"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editPermittedDiscount,
                    ArabicTransactionType = "تعديل خصم مسموح به",
                    LatinTransactionType = "Edit Permitted Discount"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deletePermittedDiscount,
                    ArabicTransactionType = "حذف خصم مسموح به",
                    LatinTransactionType = "Delete Permitted Discount"
                },

            //Users
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addUsers,
                    ArabicTransactionType = "اضافة مستخدم",
                    LatinTransactionType = "Add Users"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editUsers,
                    ArabicTransactionType = "تعديل مستخدم",
                    LatinTransactionType = "Edit Users"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteUsers,
                    ArabicTransactionType = "حذف مستخدم",
                    LatinTransactionType = "Delete Users"
                },


                 //Permission
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addPermission,
                    ArabicTransactionType = "اضافة صلاحية",
                    LatinTransactionType = "Add Permission"
                },
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editPermission,
                    ArabicTransactionType = "تعديل صلاحية",
                    LatinTransactionType = "Edit Permission"
                },
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deletePermission,
                    ArabicTransactionType = "حذف صلاحية",
                    LatinTransactionType = "Delete Permission"
                },


                //GeneralSetting
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editGeneralSetting,
                    ArabicTransactionType = "تعديل الاعدادات العامة",
                    LatinTransactionType = "Edit General Setting"
                },
                //POS
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addPOSInvoice,
                    ArabicTransactionType = "اضافة فاتورة نقاط بيع",
                    LatinTransactionType = "Add POS Invoice"
                },
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editPOSInvoice,
                    ArabicTransactionType = "تعديل فاتورة نقاط بيع",
                    LatinTransactionType = "Edit POS Invoice"
                },
                //POSAccredite
                  new SystemActionProps
                {
                    Id = (int)SystemActionEnum.AccreditPOSInvoice,
                    ArabicTransactionType = "اعتماد فاتورة نقاط بيع",
                    LatinTransactionType = "POS Invoice Accredite"
                },
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deletePOSInvoice,
                    ArabicTransactionType = "حذف فاتورة نقاط بيع",
                    LatinTransactionType = "Delete POS Invoice"
                },
                //ReturnPOS
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addReturnPOSInvoice,
                    ArabicTransactionType = "اضافة فاتورة مرتجع نقاط بيع",
                    LatinTransactionType = "Add POS Invoice"
                },
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editReturnPOSInvoice,
                    ArabicTransactionType = "تعديل فاتورة مرتجع نقاط بيع",
                    LatinTransactionType = "Edit POS Invoice"
                },
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteReturnPOSInvoice,
                    ArabicTransactionType = "حذف فاتورة مرتجع نقاط بيع",
                    LatinTransactionType = "Delete POS Invoice"
                },

                //Login
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.login,
                    ArabicTransactionType = "تسجيل دخول",
                    LatinTransactionType = "Login"
                },
                //STORES
                 new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addAddPermisson,
                    ArabicTransactionType = "اضافةاذن اضافة",
                    LatinTransactionType = "Add addition permission"
                },
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editAddPermisson,
                    ArabicTransactionType = "تعديل اذن اضافة",
                    LatinTransactionType = "Edit addition permission"
                },
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteAddPermisson,
                    ArabicTransactionType = "حذف اذن اضافة",
                    LatinTransactionType = "Delete addition permission"
                },
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addExtractPermission,
                    ArabicTransactionType = " اضافةاذن صرف",
                    LatinTransactionType = "Add extract permission"
                },
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editExtractPermission,
                    ArabicTransactionType = " تعديل اذن صرف",
                    LatinTransactionType = "Edit extract permission"
                },
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteExtractPermission,
                    ArabicTransactionType = " حذف اذن صرف",
                    LatinTransactionType = "Delete extract permission"
                },
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.DeletedOutgoingTransfer,
                    ArabicTransactionType = " حذف مستند التحويل  الصادر",
                    LatinTransactionType = "Delete outgoing Transfer  "
                } ,
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.OutGoingTransferInvoice,
                    ArabicTransactionType = " اضافة مستند التحويل  الصادر",
                    LatinTransactionType = "Add outgoing Transfer  "
                } ,
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editOutgoingTransfer,
                    ArabicTransactionType = " تعديل مستند التحويل  الصادر",
                    LatinTransactionType = "Edit outgoing Transfer  "
                }  ,
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.IncomingTranferInvoice,
                    ArabicTransactionType = " اضافة مستند التحويل  الوارد",
                    LatinTransactionType = "Add incoming Transfer  "
                }
                 ,
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addOfferPrice,
                    ArabicTransactionType = " اضافة عرض السعر",
                    LatinTransactionType = "Add Offer Price  "
                } ,
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editOfferPrice,
                    ArabicTransactionType = " تعديل عرض السعر",
                    LatinTransactionType = "Edit Offer Price  "
                }  ,
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deleteOfferPrice,
                    ArabicTransactionType = " حذف عرض سعر",
                    LatinTransactionType = "Delete Offer Price  "
                },
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addPurchaseOrder,
                    ArabicTransactionType = " اضافة أمر شراء",
                    LatinTransactionType = "Add Purchase Order  "
                },
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editPurchaseOrder,
                    ArabicTransactionType = " تعديل أمر شراء",
                    LatinTransactionType = "Edit Purchase Order  "
                },
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deletePurchaseOrder,
                    ArabicTransactionType = " حذف أمر شراء",
                    LatinTransactionType = "Delete Purchase Order  "
                },
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.addPrinter,
                    ArabicTransactionType = " إضافة طابعةء",
                    LatinTransactionType = "Add Printer  "
                },
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.editPrinter,
                    ArabicTransactionType = " تعديل طابعة",
                    LatinTransactionType = "Edite Printer  "
                },
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.deletePrinter,
                    ArabicTransactionType = " حذف طابعة",
                    LatinTransactionType = "Delete Printer  "
                }



                ,
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.add_safeMultiCollectionReceipts,
                    ArabicTransactionType = "اضافة سند مجمع خزينة",
                    LatinTransactionType = "Add Safe Multi Collection Receipts"
                },
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.edit_safeMultiCollectionReceipts,
                    ArabicTransactionType = "تعديل سند مجمع خزينة",
                    LatinTransactionType = "Edit Safe Multi Collection Receipts"
                },
                new SystemActionProps
                {
                    Id = (int)SystemActionEnum.delete_safeMultiCollectionReceipts,
                    ArabicTransactionType = "حذف سند مجمع خزينة",
                    LatinTransactionType = "Delete Safe Multi Collection Receipts"
                }

            });
            return systemActionProps;

        }

    }
    public class SystemActionProps
    {
        public int Id { get; set; }
        public string ArabicTransactionType { get; set; }
        public string LatinTransactionType { get; set; }
    }

    public enum SystemActionEnum
    {
        //addSalesInvoice = 1,
        //editSalesInvoice = 2,
        //deleteSalesInvoice = 3,

        //addSalesPurchase = 4,
        //editSalesPurchase = 5,
        //deleteSalesPurchase = 6,

        addItemsEntryFund = 7,
        editItemsEntryFund = 8,
        deleteItemsEntryFund = 9,

        addSafesFund = 10,
        editSafesFund = 11,
        deleteSafesFund = 12,

        addBanksFund = 13,
        editBanksFund = 14,
        deleteBanksFund = 15,

        addSuppliersFund = 16,
        editSuppliersFund = 17,
        deleteSuppliersFund = 18,

        addCustomersFund = 19,
        editCustomersFund = 20,
        deleteCustomersFund = 21,

        addBranch = 22,
        editBranch = 23,
        deleteBranch = 24,

        addCurrancy = 25,
        editCurrancy = 26,
        deleteCurrancy = 27,

        addSafes = 28,
        editSafes = 29,
        deleteSafes = 30,

        addBanks = 31,
        editBanks = 32,
        deleteBanks = 33,

        addOtherAuthorities = 34,
        editOtherAuthorities = 35,
        deleteOtherAuthorities = 36,

        addCalculationGuide = 37,
        editCalculationGuide = 38,
        deleteCalculationGuide = 39,

        addOpeningBalance = 40,
        editOpeningBalance = 41,
        deleteOpeningBalance = 42,

        addJournalEntry = 43,
        editJournalEntry = 44,
        deleteJournalEntry = 45,

        addCostCenter = 46,
        editCostCenter = 47,
        deleteCostCenter = 48,

        addSafePaymentReceipt = 49,
        editSafePaymentReceipt = 50,
        deleteSafePaymentReceipt = 51,



        addSafeCashReceipt = 52,
        editSafeCashReceipt = 53,
        deleteSafeCashReceipt = 54,

        addBankCashReceipt = 55,
        editBankCashReceipt = 56,
        deleteBankCashReceipt = 57,


        editGeneralLedgerSettings = 58,

        addUnits = 59,
        editUnits = 60,
        deleteUnits = 61,

        addSize = 62,
        editSize = 63,
        deleteSize = 64,

        addColor = 62,
        editColor = 63,
        deleteColor = 64,

        addItemCategories = 65,
        editItemCategories = 66,
        deleteItemCategories = 67,

        addItemCard = 68,
        editItemCard = 69,
        deleteItemCard = 70,


        addEmployee = 71,
        editEmployee = 72,
        deleteEmployee = 73,


        addJobs = 74,
        editJobs = 75,
        deleteJobs = 76,

        addStores = 77,
        editStores = 78,
        deleteStores = 79,

        addStoresPlace = 80,
        editStoresPlace = 81,
        deleteStoresPlace = 82,


        addAddPermisson = 83,
        editAddPermisson = 84,
        deleteAddPermisson = 85,

        addBarcode = 86,
        editBarcode = 87,
        deleteBarcode = 88,

        addSupplier = 89,
        editSupplier = 90,
        deleteSupplier = 91,

        addPurchaseInvoice = 92,
        editPurchaseInvoice = 93,
        deletePurchaseInvoice = 94,


        addReturnPurchaseInvoice = 95,
        editReturnPurchaseInvoice = 96,
        deleteReturnPurchaseInvoice = 97,


        addPurchaseInvoicesAccredite = 98,

        addCustomer = 99,
        editCustomer = 100,
        deleteCustomer = 101,


        addCommissionList = 102,
        editCommissionList = 103,
        deleteCommissionList = 104,

        addSalesmen = 105,
        editSalesmen = 106,
        deleteSalesmen = 107,

        addSalesInvoice = 108,
        editSalesInvoice = 109,
        deleteSalesInvoice = 110,

        addReturnSalesInvoice = 111,
        editReturnSalesInvoice = 112,
        deleteReturnSalesInvoice = 113,

        addSalesAccredite = 114,
        editSalesAccredite = 115,
        deleteSalesAccredite = 116,

        addAcquiredDiscount = 117,
        editAcquiredDiscount = 118,
        deleteAcquiredDiscount = 119,

        addPermittedDiscount = 120,
        editPermittedDiscount = 121,
        deletePermittedDiscount = 122,

        addUsers = 123,
        editUsers = 124,
        deleteUsers = 125,

        addPermission = 126,
        editPermission = 127,
        deletePermission = 128,

        editGeneralSetting = 129,

        addPOSInvoice = 130,
        editPOSInvoice = 131,
        deletePOSInvoice = 132,

        login = 133,

        addExtractPermission = 134,
        editExtractPermission = 135,
        deleteExtractPermission = 136,

        addReturnPOSInvoice = 137,
        editReturnPOSInvoice = 138,
        deleteReturnPOSInvoice = 139,

        OutGoingTransferInvoice = 140,
        IncomingTranferInvoice = 141,
        DeletedOutgoingTransfer = 142,

        addCompinedSafePaymentReceipt = 143,
        editCompinedSafePaymentReceipt = 144,
        deleteCompinedSafePaymentReceipt = 145,

        addCompinedBankPaymentReceipt = 146,
        editCompinedBankPaymentReceipt = 147,
        deleteCompinedBankPaymentReceipt = 148,

        addCompinedSafeCashReceipt = 149,
        editCompinedSafeCashReceipt = 150,
        deleteCompinedSafeCashReceipt = 151,

        addCompinedBankCashReceipt = 152,
        editCompinedBankCashReceipt = 153,
        deleteCompinedBankCashReceipt = 154,

        editOutgoingTransfer = 155,
        AccreditPOSInvoice = 156,

        addOfferPrice = 157,
        editOfferPrice = 158,
        deleteOfferPrice = 159,
        addPurchaseOrder = 160,
        editPurchaseOrder = 161,
        deletePurchaseOrder = 162,

        addAdditions = 163,
        editAdditions = 164,
        deleteAdditions = 165,

        addWOVPurchase = 166,
        editWOVPurchase = 167,
        deleteWOVPurchase = 168,

        addReturnWOVPurchase = 169,

        addPrinter = 170,
        deletePrinter = 171,
        editPrinter = 172,

        addSafeTransferReceipt = 173,
        editSafeTransferReceipt = 174,
        deleteSafeTransferReceipt = 175,

        addBankTransferReceipt = 176,
        editBankTransferReceipt = 177,
        deleteBankTransferReceipt = 178,

        addBankPaymentReceipt = 179,
        editBankPaymentReceipt = 180,
        deleteBankPaymentReceipt = 181,

        add_safeMultiCollectionReceipts = 182,
        edit_safeMultiCollectionReceipts = 183,
        delete_safeMultiCollectionReceipts = 184,

        add_bankMultiCollectionReceipts = 185,
        edit_bankMultiCollectionReceipts = 186,
        delete_bankMultiCollectionReceipts = 187,

    }

    public enum InventoryValuationType
    {
        PurchasePrice = 1,
        SalesPrice1 = 2,
        SalesPrice2 = 3,
        SalesPrice3 = 4,
        SalesPrice4 = 5,
        CostPrice = 6,
        AverageSalesPrice = 7,
        AveragePurchaesPrice = 8

    }
    public enum SecureSocketOptionsEnum
    {
        auto = 1,
        none = 2,
        StartTls = 3,
        StartTlsWhenAvailable = 4,
        SslOnConnect = 5

    }
    public enum exportType
    {
        Print = 1,
        ExportToPdf = 2,
        ExportToExcle = 3,
        ExportToImage = 4,
        ExportToWord = 5,
        ExportToSVG = 6
    }
    public static class allowedAPIs
    {
        public static List<allowedAPIsModel> allowedAPIS()
        {
            var list = new List<allowedAPIsModel>()
            {
                new allowedAPIsModel()
                {
                    APIName = "Login"
                },
                new allowedAPIsModel()
                {
                    APIName = "error"
                },
                new allowedAPIsModel()
                {
                    APIName = "DownloadFile"
                },
                new allowedAPIsModel()
                {
                    APIName = "createDB"
                },
                new allowedAPIsModel()
                {
                    APIName = "AddReports"
                },
                new allowedAPIsModel()
                {
                    APIName = "ReplaceFiles"
                },
                new allowedAPIsModel()
                {
                    APIName = "DownLoadPDF"
                },
                new allowedAPIsModel()
                {
                    APIName = "negotiate"
                },
                new allowedAPIsModel()
                {
                    APIName = "progressbar"
                },
                new allowedAPIsModel()
                {
                    APIName = "NotificationHub"
                },
                new allowedAPIsModel()
                {
                    APIName = "ForgetPassword"
                },
                new allowedAPIsModel()
                {
                    APIName = "getcompanysubscriptioninformation"
                },
                new allowedAPIsModel()
                {
                    APIName = "swagger"
                },
                new allowedAPIsModel()
                {
                    APIName = "getMigrations"
                },new allowedAPIsModel()
                {
                    APIName = "GetCompanyConsumption"
                },new allowedAPIsModel()
                {
                    APIName = "BroadCast"
                },new allowedAPIsModel()
                {
                    APIName = "deployingSystem"
                },new allowedAPIsModel()
                {
                    APIName = "forceLogout"
                },
                new allowedAPIsModel()
                {
                    APIName = "getTotalOnlineUsers"
                },
                new allowedAPIsModel()
                {
                    APIName = "getTotalOnlineUsersInfo"
                },
            };
            return list;
        }
    }
    public class allowedAPIsModel
    {
        public string APIName { get; set; }
    }

    public enum dataTypeOfRound
    {
        quantity = 1,
        price = 2,
        other = 3,
    }
    public enum DebitoAndCredito
    {
        debit = 1,
        creditor = 2
    }
    public class barcodeLength
    {
        public static int ScalBarcodeLength = 13;
        public static int ItemcodeLength = 6;
    }
    public class ScalbarcodeType
    {
        public static string Weight = "weight";
        public static string Cost = "cost";

    }
    public enum TransferStatusEnum
    {
        all = 0,
        Rejected = 1,
        Accepted = 2,
        PartialAccepted = 3,
        Binded = 4

    }
    public enum BalanceReviewReportScreens
    {
        DetailedTrialBalance = 1,
        TotalAccountBalance = 2,
        BalanceReviewFunds = 3
    }

    public enum TableType
    {
        Units = 1,
        Categories = 2,
        Persons = 3,
        Stores = 4,
        Items = 5,
        Users = 6,
        Employees = 7,
        Branches = 8,
        PaymentMethod = 9,
        PermissionList = 10,
        PermissionListUsers = 11,
        Printers = 12
    }
    public enum GLGeneralSetting_RelationTypes
    {
        withoutAccount = 1,
        generateUnderOneAccount = 2,
        OneAccountForAll = 3
    }
    public enum CategoryType
    {
        Inventory = 1,
        kitchen = 2
    }

    public enum PriceListsType
    {
        PriceLists = 1,
        PersonPrice = 2,
        employeePrice = 3,
        BranchPrice = 4,
        LastPriceForCustomer = 5,
        SalesManPrice = 6
    }
    public enum ModifySalePricesType
    {
        allowModifyByIncreaseOnly = 1, //PriceListsType التعديل بالزياده بس . ممنوع اقل عن السعر المحدد حسب قوائم الاسعار 
        allowModifyByIncreaseUntilPrice4 = 2,  // التعديل لحد سعر بيع 4 ممنوع اقل عن سعر 4  
        modifyWithoutConstrains = 3, // اعدل بالزيادة والنقصان

    }
    public enum PersonTypes
    {
        customer = 1,
        supplier = 2
    }
    public enum ERPTool_CreateJournalEntry
    {
        entryFund=1,
        puchases = 2,
        sales = 3,
        pos = 4,
        recsAndSettlements = 5 
    }
}
