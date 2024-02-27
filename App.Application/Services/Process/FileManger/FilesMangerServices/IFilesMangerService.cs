using App.Domain.Entities;
using App.Domain.Models.Request.ReportFile;
using App.Domain.Models.Request.Store.Invoices;
using App.Domain.Models.Shared;
using App.Infrastructure.UserManagementDB;
using DocumentFormat.OpenXml.Spreadsheet;
using FastReport;
using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;

namespace App.Application.Services.Process.StoreServices.Invoices.AccrediteInvoice
{
    public interface IFilesMangerService
    {
       Task<ReportFileRequest> GetReportPrintFiles(int ScreenId, bool IsArabic, int ClosingStep = 0, int fileId = 0); //first one only
       Task<ResponseResult> GetAllPrintFiles(int ScreenId); 
       Task<ResponseResult> GetPrintFileById(int FileId); 
       Task<ReportsReponse> GetInviocePrintFile(InvoiceDTO invoiceDto, string employeeNameAr, string employeeNameEn, bool isPOS = false);

        Task<ResponseResult> AddFileToScreen(int fileId,int screenId, bool isArabic);
        Task<ResponseResult> SetFileAsDefault(int fileId, int screenId);
        Task<ReportFileRequest> GetBarcodePrintFile(int fileId);
        Task<ResponseResult> BarcodePrintFiles();
        Task<ResponseResult> SetBarcodeDefautFile(int fileId);
        Task<ResponseResult> UserPermisionsForBarcodePrint();
        Task<ResponseResult> GetPOS_RPOS_PrintFilesByDate(DateTime date);
        Task<ResponseResult> SetFilesArabicNames();





    }

    // Aly
    //alaaa
    public static class GetListOfFile
    {
        public static List<FilesModel> ReportFilesList()
        {
            var list = new List<FilesModel>();
            list.AddRange(new[]
            {
                //new FilesModel
                //{
                //    screenId = (int)SubFormsIds.Sales,
                //    screen = "sales",
                //    reportName = "SimplifiedTaxSalesInvoice",
                //    reportNameAr="فاتورة مبيعات ضريبية مبسطة",
                //    isReport=false

                //},
                 new FilesModel
                {
                    screenId = (int)SubFormsIds.Sales,
                    screen = "sales",
                    reportName = "SalesInvioce",
                    reportNameAr="مبيعات",
                    isReport=false
                },
                new FilesModel
                {
                    screenId = (int)SubFormsIds.SalesReturn_Sales,
                    screen = "returnSales",
                    reportName = "ReturnSalesInvioce",
                    reportNameAr="مرتجع مبيعات",
                    isReport=false

                },
                new FilesModel
                {
                    screenId = (int)SubFormsIds.Purchases,
                    screen = "purchases",
                    reportName = "PurchasesInvioce",
                    reportNameAr="مشتريات",
                    isReport=false

                },
                new FilesModel
                {
                    screenId = (int)SubFormsIds.PurchasesReturn_Purchases,
                    screen = "returnPurchases",
                    reportName = "ReturnPurshaseInvioce",
                    reportNameAr="مرتجع مشتريات",
                    isReport=false

                },
                new FilesModel
                {
                    screenId = (int)SubFormsIds.POS,
                    screen = "POS",
                    reportName = "POSInvoice",
                    reportNameAr="نقاط بيع",
                    isReport=false

                },
                new FilesModel
                {
                    screenId = (int)SubFormsIds.returnPOS,
                    screen = "ReturnPOS",
                    reportName = "ReturnPOSInvoice",
                    reportNameAr="مرتجع نقاط بيع",
                    isReport=false

                },
                 new FilesModel
                {
                    screenId = (int)SubFormsIds.AddPermission_Repository,
                    screen = "AddPermision",
                    reportName = "Add-Permision",
                    reportNameAr="اذن اضافة",
                    isReport=false

                },
                  new FilesModel
                {
                    screenId = (int)SubFormsIds.pay_permission,
                    screen = "PayPermission",
                    reportName = "Extract-Permision",
                    reportNameAr="اذن صرف",
                    isReport=false


                },
                   new FilesModel
                {
                    screenId = (int)SubFormsIds.items_Fund,
                    screen = "ItemsFund",
                    reportName = "ItemsFund",
                    reportNameAr="ارصدة اول مدة اصناف",
                    isReport=false

                },
                   new FilesModel
                {
                    screenId = (int)SubFormsIds.SupplierStatement_Purchases,
                    screen = "SupplierStatementPurchases",
                    reportName = "SupplierAccounts",
                    reportNameAr="كشف حساب مورد",
                    isReport=true

                },
                   new FilesModel
                {
                    screenId = (int)SubFormsIds.GetVatStatmentData_Purchases,
                    screen = "VatStatmentDataPurchases",
                    reportName = "VatStatmentDataPurchases",
                    reportNameAr = "كشف حساب القيمة المضافة الاجمالى",
                    isReport=true

                },
                    new FilesModel
                {
                    screenId = (int)SubFormsIds.DetailedMovementOfAnItem_Repository,
                    screen = "DetailedMovementOfAnItem",
                    reportName = "DetailedMovementOfAnItem",
                    reportNameAr = "الحركة التفصلية لصنف",
                    isReport=true

                },
                    
                    new FilesModel
                {
                    screenId = (int)SubFormsIds.PayReceiptForSafe,
                    screen = "PayReceiptForSafe",
                    reportName = "PayReceiptForSafe",
                    reportNameAr = "سند صرف خزينة",
                    isReport=true

                },
                     new FilesModel
                {
                    screenId = (int)SubFormsIds.CashReceiptForSafe,
                    screen = "CashReceiptForSafe",
                    reportName = "CashReceiptForSafe",
                    reportNameAr = "سند قبض خزينة",
                    isReport=true

                },
                      new FilesModel
                {
                    screenId = (int)SubFormsIds.PayReceiptForBank,
                    screen = "PayReceiptForBank",
                    reportName = "PayReceiptForBank",
                    reportNameAr = "سند صرف بنك",
                    isReport=true

                },
                       new FilesModel
                {
                    screenId = (int)SubFormsIds.CashReceiptForBank,
                    screen = "CashReceiptForBank",
                    reportName = "CashReceiptForBank",
                    reportNameAr = "سند قبض بنك",
                    isReport=true

                },
                       new FilesModel
                {
                    screenId = (int)SubFormsIds.VatDetailedStatement,
                    screen = "VatDetailedReport",
                    reportName = "VatDetailedReport",
                    reportNameAr = "كشف حساب القيمة المضافة التفصيلى",
                    isReport=true

                },
                       new FilesModel
                {
                    screenId = (int)SubFormsIds.CustomerStatement_Sales,
                    screen = "CustomerAccounts",
                    reportName = "CustomerAccounts",
                    reportNameAr = "كشف حساب عميل",
                    isReport=true

                },
                       new FilesModel
                {
                    screenId = (int)SubFormsIds.salesOfCasher,
                    screen = "salesOfCasherAccredit",
                    reportName = "salesOfCasherAccredit",
                    reportNameAr = "مبيعات الكاشير المتعمدة",
                    isReport=true

                },
                       new FilesModel
                {
                    screenId = (int)SubFormsIds.salesOfCasherNotAccredit,
                    screen = "salesOfCasherNotAccredit",
                    reportName = "salesOfCasherNotAccredit",
                    reportNameAr = "مبيعات الكاشير الغير المتعمدة",
                    isReport=true

                },
                       new FilesModel
                {
                    screenId = (int)SubFormsIds.SafeAccountStatement,
                    screen = "SafeAccountStatement",
                    reportName = "SafeAccountStatement",
                    reportNameAr = "كشف حساب خزينة",
                    isReport=true

                },
                       new FilesModel
                {
                    screenId = (int)SubFormsIds.BankAccountStatement,
                    screen = "BankAccountStatement",
                    reportName = "BankAccountStatement",
                    reportNameAr = "كشف حساب بنك",
                    isReport=true

                },
                       new FilesModel
                {
                    screenId = (int)SubFormsIds.TotalItemBalancesInStore_Repository,
                    screen = "ItemsBalanceInStores",
                    reportName = "ItemsBalanceInStores",
                    reportNameAr = "ارصدة اصناف فى المخازن",
                    isReport=true

                },
                //       new FilesModel
                //{
                //    screenId = (int)SubFormsIds.TotalItemBalancesInStore_Repository,
                //    screen = "ItemsBalanceInStores",
                //    reportName = "ItemsBalanceInStores"
                // },
                       new FilesModel
                {
                    screenId = (int)SubFormsIds.ItemSales,
                    screen = "ItemSales",
                    reportName = "ItemSales",
                    reportNameAr = "مبيعات أصناف",
                    isReport=true

                 },
                       new FilesModel
                {
                    screenId = (int)SubFormsIds.ItemsPurchases_Purchases,
                    screen = "itemPurchases",
                    reportName = "ItemsPurchases",
                    reportNameAr = "مشتريات أصناف",
                    isReport=true

                },
                       new FilesModel
                {
                    screenId = (int)SubFormsIds.SalesTransaction,
                    screen = "SalesTransaction",
                    reportName = "SalesTransaction",
                    reportNameAr = "حركة المبيعات",
                    isReport=true

                },
                       new FilesModel
                {
                    screenId = (int)SubFormsIds.PaymentsAndDisbursements,
                    screen = "PaymentsAndDisbursements",
                    reportName = "PaymentsAndDisbursements",
                    reportNameAr = "جهات قبض وصرف",
                    isReport=true

                },
                       new FilesModel
                {
                    screenId = (int)SubFormsIds.AccountStatementDetail_GL,
                    screen = "AccountStatementDetails",
                    reportName = "AccountStatementDetails",
                    reportNameAr = "كشف حساب تفصيلى",
                    isReport=true

                },
                       new FilesModel
                {
                    screenId = (int)SubFormsIds.ItemNotSold,
                    screen = "ItemsNotSold",
                    reportName = "ItemsNotSold",
                    reportNameAr = "أصناف لم تباع",
                    isReport=true

                },
                       new FilesModel
                {
                    screenId = (int)SubFormsIds.itemsSoldMost,
                    screen = "ItemsSoldMost",
                    reportName = "ItemsSoldMost",
                    reportNameAr = "أصناف اكثر مبيعا",
                    isReport=true

                },
                       new FilesModel
                       {
                    screenId = (int)SubFormsIds.EarnedDiscount_Settelments,
                    screen = "EarnedDiscount",
                    reportName = "EarnedDiscount",
                    reportNameAr = "خصم مكتسب",
                    isReport=true

                },
                       new FilesModel
                {
                    screenId = (int)SubFormsIds.PermittedDiscount_Settelments,
                    screen = "PermittedDiscount",
                    reportName = "PermittedDiscount",
                    reportNameAr = "خصم مسموح به",
                    isReport=true

                },
                       new FilesModel
                {
                    screenId = (int)SubFormsIds.itemsPrices,
                    screen = "ItemsPrices",
                    reportName = "ItemsPrices",
                    reportNameAr = "اسعار أصناف",
                    isReport=true

                },
                       new FilesModel
                {
                    screenId = (int)SubFormsIds.LedgerReport_GL,
                    screen = "AccountStatementMR",
                    reportName = "AccountStatementMR",
                    reportNameAr = "دفتر الاستاذ",
                    isReport=true

                },
                //       new FilesModel
                //{
                //    screenId = (int)SubFormsIds.AccountStatementDetail_GL,
                //    screen = "AccountStatementDetails",
                //    reportName = "AccountStatementDetails"
                //},
                        new FilesModel
                {
                    screenId = (int)SubFormsIds.DetailedTrialBalance_GL,
                    screen = "DetailedBalanceReview",
                    reportName = "DetailedBalanceReview",
                    reportNameAr = "ميزان المراجعة التفصيلى",
                    isReport=true

                },
                        new FilesModel
                {
                    screenId = (int)SubFormsIds.IncomeList_GL,
                    screen = "IncomeList",
                    reportName = "IncomeList",
                    reportNameAr = "قائمة الدخل",
                    isReport=true

                },
                       
                        new FilesModel
                {
                    screenId = (int)SubFormsIds.AccountingEntries_GL,
                    screen = "AccountingEntries",
                    reportName = "AccountingEntries",
                    reportNameAr = "القيود",
                    isReport=false

                },

                        new FilesModel
                {
                    screenId = (int)SubFormsIds.CostCenterReport_GL,
                    screen = "CostCenter",
                    reportName = "CostCenter",
                    reportNameAr = "مركز التكلفة",
                    isReport=true

                },    
                        new FilesModel
                {
                    screenId = (int)SubFormsIds.ItemCard_MainUnits,
                    screen = "ItemCard",
                    reportName = "ItemCard",
                    reportNameAr = "كارت الصنف",
                    isReport=true

                },
                        new FilesModel
                {
                    screenId = (int)SubFormsIds.Employees_MainUnits,
                    screen = "Employees",
                    reportName = "Employees",
                    reportNameAr = "الموظفين",
                    isReport=true

                },
                        new FilesModel
                {
                    screenId = (int)SubFormsIds.Customres_Fund,
                    screen = "Customers-Fund",
                    reportName = "CustomersFund",
                    reportNameAr = "ارصدة اول مدة عملاء",
                    isReport=true

                },
                        new FilesModel
                {
                    screenId = (int)SubFormsIds.Suppliers_Fund,
                    screen = "Suppliers-Fund",
                    reportName = "SuppliersFund",
                    reportNameAr = "ارصدة اول مدة موردين",
                    isReport=true

                },
                        new FilesModel
                {
                    screenId = (int)SubFormsIds.Suppliers_Purchases,
                    screen = "Suppliers-Purchases",
                    reportName = "SuppliersPurchases",
                    reportNameAr = "الموردين",
                    isReport=true

                },
                        new FilesModel
                {
                    screenId = (int)SubFormsIds.Customers_Sales,
                    screen = "Customers-Sales",
                    reportName = "CustomersSales",
                    reportNameAr = "العملاء",
                    isReport=true

                },
                        new FilesModel
                {
                    screenId = (int)SubFormsIds.OutgoingTransfer,
                    screen = "OutgoingTransfer",
                    reportName = "OutgoingTransfer",
                    reportNameAr = "تحويل صادر",
                    isReport=false

                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.IncomingTransfer,
                    screen = "IncomingTransfer",
                    reportName = "IncomingTransfer",
                    reportNameAr = "تحويل وارد",
                    isReport=false

                }
                //        new FilesModel

                //{
                //    screenId = (int)SubFormsIds.supplierItemsPurchased_Purchases,
                //    screen = "ItemPurchasesForSupplier",
                //    reportName = "ItemPurchasesForSupplier",
                //    reportNameAr = "مشتريات صنف من مورد",
                //    isReport=true

                //        }
                //},
                //        new FilesModel

                //{
                //    screenId = (int)SubFormsIds.SupplierItemsPurchases_Purchases,
                //    screen = "ItemsPurchasesForSupplier",
                //    reportName = "ItemsPurchasesForSupplier"
                //}
                //,
                ,
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.purchasesTransaction_Purchases,
                    screen = "PurchasesTransaction",
                    reportName = "PurchasesTransaction",
                    reportNameAr = "حركة المشتريات",
                    isReport=true

                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.GetSuppliersAccountData_Purchases,
                    screen = "SuppliersBalances",
                    reportName = "SuppliersBalances",
                    reportNameAr = "ارصدة موردين",
                    isReport=true

                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.CustomersBalances,
                    screen = "CustomersBalances",
                    reportName = "CustomersBalances",
                    reportNameAr = "ارصدة عملاء",
                    isReport=true

                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.SafeExpenses,
                    screen = "SafeExpenses",
                    reportName = "SafeExpenses",
                    reportNameAr = "مصروفات خزنة",
                    isReport=true

                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.BankExpenses,
                    screen = "BankExpenses",
                    reportName = "BankExpenses",
                    reportNameAr = "مصروفات بنك",
                    isReport=true

                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.SafeReceipts,
                    screen = "SafeReceipts",
                    reportName = "SafeReceipts",
                    reportNameAr = "مقبوضات خزنة",
                    isReport=true

                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.BankReceipts,
                    screen = "BankReceipts",
                    reportName = "BankReceipts",
                    reportNameAr = "مقبوضات بنك",
                    isReport=true

                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.itemSalesForCustomer,
                    screen = "ItemSalesForCustomer",
                    reportName = "ItemSalesForCustomer",
                    reportNameAr = "مبيعات صنف لعميل",
                    isReport=true

                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.itemsSalesForCustomer,
                    screen = "ItemsSalesForCustomer",
                    reportName = "ItemsSalesForCustomer",
                    reportNameAr = "مبيعات اصناف لعميل",
                    isReport=true

                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.itemSalesForCustomers,
                    screen = "ItemSalesForCustomers",
                    reportName = "ItemSalesForCustomers",
                    reportNameAr = "مبيعات صنف لعملاء",
                    isReport=true

                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.SalesAndReturnSalesTransaction,
                    screen = "SalesAndSalesReturnTransaction",
                    reportName = "SalesAndSalesReturnTransaction",
                    reportNameAr = "حركة المبيعات والمرتجعات",
                    isReport=true

                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.TotalSalesOfBranch,
                    screen = "TotalSalesOfBranch",
                    reportName = "TotalSalesOfBranch",
                    reportNameAr = "إجمالى مبيعات الفرع",
                    isReport=true

                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.totalBranchTransaction,
                    screen = "TotalBranchTransaction",
                    reportName = "TotalBranchTransaction",
                    reportNameAr = "إجمالى حركات الفرع",
                    isReport=true

                },
                        
                        //new FilesModel

                //{
                //    screenId = (int)SubFormsIds.getItemBalanceInStores_Repository,
                //    screen = "ItemsBalanceInStores",
                //    reportName = "ItemsBalanceInStores",
                //    reportNameAr = "رصيد صنف فى المخازن",
                //    isReport=true

                //},

                        new FilesModel

                {
                    screenId = (int)SubFormsIds.getTotalTransactionsOfItems_Repository,
                    screen = "TotalTransactionsOfItem",
                    reportName = "TotalTransactionsOfItem",
                    reportNameAr = "إجمالى حركة صنف فى المخازن",
                    isReport=true

                },

                        new FilesModel

                {
                    screenId = (int)SubFormsIds.itemsTransaction,
                    screen = "TotalTransactionsOfItems",
                    reportName = "TotalTransactionsOfItems",
                    reportNameAr = "إجمالى حركات الاصناف",
                    isReport=true
                },

                        new FilesModel

                {
                    screenId = (int)SubFormsIds.InventoryValuation_Repository,
                    screen = "InventoryValuation",
                    reportName = "InventoryValuation",
                    reportNameAr = "تقييم المخزون",
                    isReport=true
                },

                        new FilesModel

                {
                    screenId = (int)SubFormsIds.DetailsOfSerialTransactions,
                    screen = "DetailsOfSerialTransactions",
                    reportName = "DetailsOfSerialTransactions",
                    reportNameAr = "الاستعلام عن سيريال",
                    isReport=true

                },

                        new FilesModel

                {
                    screenId = (int)SubFormsIds.ReviewWarehouseTransfersReport,
                    screen = "ReviewWarehouseTransfers",
                    reportName = "ReviewWarehouseTransfers",
                    reportNameAr = "مراجعة تحويلات المخازن",
                    isReport=true
                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.DetailedTransferReport,
                    screen = "DetailedTransfer",
                    reportName = "DetailedTransfer",
                    reportNameAr = "تحويلات تفصيلى",
                    isReport=true
                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.GetDetailsOfExpiredItems,
                    screen = "ExpiredItems",
                    reportName = "ExpiredItems",
                    reportNameAr = "إنتهاء صلاحية",
                    isReport=true
                }
                        ,
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.Salesmen_Sales,
                    screen = "SalesMan",
                    reportName = "SalesMan",
                    reportNameAr = "مندوبى المبيعات",
                    isReport=true
                } ,
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.PruchasesClosing_Purchases,
                    screen = "ClosingInvoivesPeroidPurchases",
                    reportName = "ClosingInvoivesPeroidPurchases",
                    reportNameAr = "الفواتير عن فتره -إقفال مشتريات",
                    isReport=true
                } ,
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.PruchasesClosing_Purchases,
                    screen = "ClosingReturnPeroidPurchasesan",
                    reportName = "ClosingReturnPeroidPurchases",
                    reportNameAr = "المرتجعات عن فتره - إقفال مشتريات",
                    isReport=true
                } ,
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.PruchasesClosing_Purchases,
                    screen = "ClosingPaymentsAndReceiptsPurchases",
                    reportName = "ClosingPaymentsAndReceiptsPurchases",
                    reportNameAr = "بيان الفواتير والمرتجعات  - إقفال مشتريات",
                    isReport=true
                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.SalesClosing_Sales,
                    screen = "ClosingInvoivesPeroidSales",
                    reportName = "ClosingInvoivesPeroidSales",
                    reportNameAr = "الفواتير عن فتره - إقفال مبيعات",
                    isReport=true
                } ,
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.SalesClosing_Sales,
                    screen = "SalesMClosingReturnPeroidSales",
                    reportName = "ClosingReturnPeroidSales",
                     reportNameAr = "المرتجعات عن فتره - إقفال مبيعات",
                    isReport=true
                } ,
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.SalesClosing_Sales,
                    screen = "ClosingPaymentsAndReceiptsSales",
                    reportName = "ClosingPaymentsAndReceiptsSales",
                     reportNameAr = "بيان الفواتير والمرتجعات  - إقفال مبيعات",
                    isReport=true
                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.POSClosing_Sales,
                    screen = "ClosingInvoivesPeroidPOS",
                    reportName = "ClosingInvoivesPeroidPOS",
                     reportNameAr = "الفواتير عن فتره - إقفال نقاط البيع",
                    isReport=true
                } ,
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.POSClosing_Sales,
                    screen = "SalesMClosingReturnPeroidPOS",
                    reportName = "ClosingReturnPeroidPOS",
                     reportNameAr = "المرتجعات عن فتره - إقفال نقاط البيع",
                    isReport=true

                } ,
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.POSClosing_Sales,
                    screen = "ClosingPaymentsAndReceiptsPOS",
                    reportName = "ClosingPaymentsAndReceiptsPOS",
                     reportNameAr = "بيان الفواتير والمرتجعات  - إقفال نقاط البيع",
                    isReport=true

                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.offerPrice_Sales,
                    screen = "PriceOfferSales",
                    reportName = "PriceOffer",
                     reportNameAr = "عرض السعر",
                    isReport=false

                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.purchasesTransactionOfBranch,
                    screen = "PurchasesTransactionOfBranch",
                    reportName = "PurchasesTransactionOfBrach",
                     reportNameAr = "حكرة مشتريات الفرع",
                    isReport=true
                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.DemandLimit,
                    screen = "DemandLimit",
                    reportName = "DemandLimit",
                     reportNameAr = "حد الطلب",
                    isReport=true
                }  ,
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.SupplierItemsPurchases_Purchases,
                    screen = "ItemsPurchasesFromSupplier",
                    reportName = "ItemsPurchasesFromSupplier",
                     reportNameAr = "مشتريات أصناف من مورد",
                    isReport=true
                }
                        ,
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.PublicBudget_GL,
                    screen = "PublicBudget",
                    reportName = "PublicBudget",
                     reportNameAr = "الميزانيه العمومية",
                    isReport=true
                }  ,
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.SalesBranchProfit,
                    screen = "SalesProfitOfBranch",
                    reportName = "SalesProfitOfBranch",
                     reportNameAr = "ربحية مبيعات الفرع",
                    isReport=true
                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.TotalAccountBalance,
                    screen = "TotalAccountBalance",
                    reportName = "TotalBalanceOfAccount",
                     reportNameAr = "اجمالى رصيد الحساب",
                    isReport=true
                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.BalanceReviewFunds,
                    screen = "BalanceReviewFunds",
                    reportName = "BalancesOfReviewBalance",
                     reportNameAr = "ميزان المراجعة ارصدة",
                    isReport=true
                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.costCenterForAccount,
                    screen = "costCenterForAccount",
                    reportName = "CostCenterForAccount",
                     reportNameAr = "مراكز التكلفة لحساب",
                    isReport=true
                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.SalesOfSalesMan,
                    screen = "SalesOfSalesMan",
                    reportName = "SalesOfSalesManReport",
                     reportNameAr = "مبيعات مندوب المبيعات",
                    isReport=true
                }
                        ,
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.SalesProfitOfItems,
                    screen = "SalesProfitOfItems",
                    reportName = "ItemsProfitReport",
                     reportNameAr = "ربحية مبيعات الاصناف" ,
                     
                    isReport=true

                }
                        ,
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.GetTotalVatData,
                    screen = "TotalVat",
                    reportName = "TotalVat",
                     reportNameAr = "إجمالى الضريبة" ,

                    isReport=true

                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.DebtAgingForCustomers,
                    screen = "DebtAgingForCustomers",
                    reportName = "DebtAgingForCustomers",
                     reportNameAr = "أعمار الديون لفواتير العملاء" ,

                    isReport=true

                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.DebtAgingForSupplier,
                    screen = "DebtAgingForSuppliers",
                    reportName = "DebtAgingForSuppliers",
                     reportNameAr = "أعمار الديون لفواتير الموردين" ,

                    isReport=true

                }
                        ,
                //        new FilesModel

                //{
                //    screenId = (int)SubFormsIds.POS,
                //    screen = "POSInvoiceAndroid",
                //    reportName = "POSInvoiceAndroid",
                //    reportNameAr="نقاط البيع موبايل"
                //} ,
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.userActions,
                    screen = "User Actions",
                    reportName = "UsersTransactions",
                    reportNameAr="حركات المستخدمين",
                    updateNumber=1
                },
                        new FilesModel

                {
                    screenId = (int)SubFormsIds.offerPriceReport,
                    screen = "Offer Price Report",
                    reportName = "PricesOffersReport",
                    reportNameAr="تقرير عروض الاسعار",
                    updateNumber=1
                },

                         new FilesModel

                {
                    screenId = (int)SubFormsIds.DetailedInvoices,
                    screen = "Detailed Invoices Report",
                    reportName = "DetailedInvoices",
                    reportNameAr="تقرير حركة السداد للفواتير",
                    updateNumber=4
                },

                         new FilesModel

                {
                    screenId = (int)SubFormsIds.transferFromSafes,
                    screen = "TransferReceiptForSafe",
                    reportName = "TransferReceiptForSafe",
                    reportNameAr="سند تحويل خزينة",
                    updateNumber=8
                },

                         new FilesModel

                {
                    screenId = (int)SubFormsIds.transferFromBanks,
                    screen = "TransferReceiptForBank",
                    reportName = "TransferReceiptForBank",
                    reportNameAr="سند تحويل بنك",
                    updateNumber=8
                },


                         new FilesModel

                {
                    screenId = (int)SubFormsIds.SafeMultiCollectionReceipt,
                    screen = "MultiCollectionReceiptForSafe",
                    reportName = "MultiCollectionReceiptForSafe",
                    reportNameAr="سند خزينة مجمع",
                    updateNumber=13
                }
                         ,


                         new FilesModel

                {
                    screenId = (int)SubFormsIds.BankMultiCollectionReceipt,
                    screen = "MultiCollectionReceiptForBank",
                    reportName = "MultiCollectionReceiptForBank",
                    reportNameAr="سند بنك مجمع",
                    updateNumber=13
                }
                         //khaled
    
            });
            return list;
        }
    }

    public static class GetBarcodeFiles
    {
        public static List<BarcodeFiles> GetBarcodePrintFiles()
        {
            var list = new List<BarcodeFiles>();
            list.AddRange(new[]
            {
                new BarcodeFiles
                {
                    ArabicName="تصميم باركود 40-20",
                    LatineName="barcode 40-20",
                    IsDefault=true,
                },
                new BarcodeFiles
                {
                    ArabicName="تصميم باركود 50-25",
                    LatineName="barcode 50-25",
                    IsDefault=false,
                },
                new BarcodeFiles
                {
                    ArabicName="تصميم باركود 60-40",
                    LatineName="barcode 60-40",
                    IsDefault=false,
                }
            });

            return list;
        }
    }
    public class FilesModel
    {
        public int screenId{ get; set; }
        public string screen { get; set; }
        public string reportName { get; set; }
        public string reportNameAr { get; set; }
        public bool isReport { get; set; }
        public int updateNumber { get; set; }

    }
    public class BarcodeFiles
    {
        public string ArabicName { get; set; }
        public string LatineName { get; set; }
        public bool IsDefault { get; set; }
    }
}
