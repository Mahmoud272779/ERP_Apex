using App.Domain.Entities.Process;
using App.Domain.Models.Response.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Helpers
{
    public class Lists
    {
        public static List<int> ReceiptIds = new List<int>()
        {
            (int)DocumentType.SafeCash,
            (int)DocumentType.SafePayment,
            (int)DocumentType.BankCash,
            (int)DocumentType.BankPayment,
            (int)DocumentType.BankMultiCollectionReceipts,
            (int)DocumentType.SafeMultiCollectionReceipts,
            (int)DocumentType.BankMultiCollectionReceipts,
            (int)DocumentType.SafeMultiCollectionReceipts,
        };
        public static List<int> storesInvoicesList = new List<int>()
        {
            (int)DocumentType.AddPermission,
            (int)DocumentType.DeleteAddPermission,
            (int)DocumentType.ExtractPermission,
            (int)DocumentType.DeleteExtractPermission ,
            (int)DocumentType.itemsFund ,
            (int)DocumentType.IncomingTransfer ,
            (int)DocumentType.OutgoingTransfer ,
            (int)DocumentType.DeleteItemsFund
        };
        public static List<int> rowClassNameTransaction = new List<int>()
        {
            (int)DocumentType.ReturnPOS,
            (int)DocumentType.ReturnSales,
            (int)DocumentType.ReturnPurchase,
            (int)DocumentType.DeletePOS,
            (int)DocumentType.DeleteSales,
            (int)DocumentType.DeletePurchase,
            (int)DocumentType.DeleteAddPermission,
            (int)DocumentType.DeleteExtractPermission,
            (int)DocumentType.DeleteItemsFund,
            (int)DocumentType.DeletedOutgoingTransfer,
            (int)DocumentType.DeletedIncommingTransfer,
        };
        // اذن صرف  
        public static List<int> AddPermissionInvoicesList = new List<int>()
        {
             (int)DocumentType.AddPermission,
             (int)DocumentType.DeleteAddPermission
         };
        public static List<int> itemNotRegular = new List<int>()
        {
            (int)ItemTypes.Note,
           
            //(int)ItemTypes.Composite
        };
        //الاصناف الغير محوله 
        public static List<int> itemThatNotTransfer = new List<int>()
        {
            (int)ItemTypes.Note,
            (int)ItemTypes.Service,
            (int)ItemTypes.Composite
        };

        public static List<int> InvoiceCannotAddJournalEntery = new List<int>()
        {   (int)DocumentType.IncomingTransfer,
            (int)DocumentType.OutgoingTransfer ,
            (int)DocumentType.itemsFund
        };

        public static List<int> transferStore = new List<int>()
        {   (int)DocumentType.IncomingTransfer,
            (int)DocumentType.OutgoingTransfer,
            (int)DocumentType.DeletedOutgoingTransfer
        };

        public static List<int> ExtractPermissionInvoicesList = new List<int>()
        {
            (int)DocumentType.ExtractPermission,
            (int)DocumentType.DeleteExtractPermission
        };



        public static List<int> ItemsFundList = new List<int>
        { (int)DocumentType.itemsFund,
            (int)DocumentType.DeleteItemsFund
        };


        public static List<int> ExpensesInvoicesList = new List<int>()
        {
            (int)expensesInvoiceType.Expenses,
            (int)expensesInvoiceType.ExpensesReturn,
            
        };
        public static List<int> purchasesInvoicesList = new List<int>()
        {
            (int)DocumentType.Purchase,
            (int)DocumentType.ReturnPurchase,
            (int)DocumentType.DeletePurchase
        };
        public static List<int> purchasesWithoutVatInvoicesList = new List<int>()
        {
            (int)DocumentType.wov_purchase,
            (int)DocumentType.ReturnWov_purchase,
            (int)DocumentType.DeleteWov_purchase
        };
        public static  List<int> salesInvoicesList
        {
            get {
                return new List<int>()
                    { (int)DocumentType.Sales, (int)DocumentType.ReturnSales, (int)DocumentType.DeleteSales };
            }
        } 

        public static List<int> POSInvoicesList = new List<int>()
        {
            (int)DocumentType.POS,
            (int)DocumentType.ReturnPOS,
            (int)DocumentType.DeletePOS
        };


        public static List<int> MainInvoiceList = new List<int>
        {
            (int)DocumentType.Purchase ,
            (int)DocumentType.AddPermission ,
            (int)DocumentType.itemsFund ,
            (int)DocumentType.ExtractPermission,
            (int)DocumentType.Sales,
            (int)DocumentType.POS ,
            (int)DocumentType.SafeCash,
            (int)DocumentType.SafePayment,
            (int)DocumentType.CompinedSafeCash,
            (int)DocumentType.CompinedSafePayment,
            (int)DocumentType.OutgoingTransfer,
            (int)DocumentType.IncomingTransfer,
            (int)DocumentType.BankPayment ,
            (int)DocumentType.BankCash,
            (int)DocumentType.CompinedBankPayment ,
            (int)DocumentType.CompinedBankCash,
            (int)DocumentType.CustomerFunds,
            (int)DocumentType.SuplierFunds,
            (int)DocumentType.wov_purchase
        };

        public static List<int> returnInvoiceList = new List<int>()
        { 
            (int)DocumentType.ReturnPOS,
            (int)DocumentType.ReturnPurchase,
            (int)DocumentType.ReturnSales ,
            (int)DocumentType.ReturnWov_purchase
        };
        public static List<int> returnSalesInvoiceList = new List<int>()
        {
            (int)DocumentType.ReturnPOS,     
            (int)DocumentType.ReturnSales
        };

        public static List<int> Transfers = new List<int>()
        { 
            (int)DocumentType.IncomingTransfer,
            (int)DocumentType.OutgoingTransfer 
        };

        public static List<int> deleteInvoiceAddingToStore = new List<int>()
        {   (int)DocumentType.DeletePOS, 
            (int)DocumentType.DeleteSales,
            (int)DocumentType.DeleteExtractPermission ,
            (int)DocumentType.DeletedOutgoingTransfer
        };

        public static List<int> deleteInvoiceExtractFromStore = new List<int>()
        { 
            (int)DocumentType.DeletePurchase ,
            (int)DocumentType.DeleteItemsFund,
            (int)DocumentType.DeleteAddPermission  ,
            (int)DocumentType.DeleteWov_purchase
        };

        //list of invoiceType that affect in Cost
        public static List<int> InvoicesTypeAffectToCost = new List<int>
        {
            (int)DocumentType.Purchase,
            (int)DocumentType.AddPermission,
            (int)DocumentType.IncomingTransfer,
            (int)DocumentType.ReturnPurchase,
            (int)DocumentType.itemsFund,
            (int)DocumentType.wov_purchase,
            (int)DocumentType.ReturnWov_purchase,

        };
        //list of invoiceType that Not affect in Cost
        public static List<int> InvoicesTypeNotAffectToCost = new List<int>
        {
        (int)DocumentType.Sales,
            (int)DocumentType.ReturnSales,
            (int)DocumentType.POS,
            (int)DocumentType.ReturnPOS,
            (int)DocumentType.OutgoingTransfer,
            (int)DocumentType.ExtractPermission,

        };
        public static List<int> InvoicesTypeOfAddingToStore = new List<int>
        {
            (int)DocumentType.Purchase, 
            (int)DocumentType.AddPermission,
            (int)DocumentType.itemsFund ,
            (int)DocumentType.DeleteExtractPermission ,
            (int)DocumentType.DeletePOS,
            (int)DocumentType.IncomingTransfer ,
            (int)DocumentType.DeleteSales ,
            (int)DocumentType.ReturnPOS ,
            (int)DocumentType.ReturnSales,
            (int)DocumentType.wov_purchase
        };

        public static List<int> InvoicesTypesOfExtractFromStore = new List<int>
        { 
            (int)DocumentType.ReturnPurchase, 
            (int)DocumentType.DeletePurchase,
            (int)DocumentType.DeleteAddPermission,
            (int)DocumentType.DeleteItemsFund,
            (int)DocumentType.ExtractPermission,
            (int)DocumentType.OutgoingTransfer,
            (int)DocumentType.Sales,
            (int)DocumentType.POS ,
            (int)DocumentType.DeleteWov_purchase,
            (int)DocumentType.ReturnWov_purchase
        };

        public static List<int> InvoicesTypeForAddStore_Setting = new List<int>
        { 
            (int)DocumentType.Purchase, 
            (int)DocumentType.AddPermission,
            (int)DocumentType.itemsFund ,
            (int)DocumentType.DeletePurchase ,
            (int)DocumentType.DeleteAddPermission ,
            (int)DocumentType.DeleteItemsFund ,
            (int)DocumentType.ReturnPurchase ,
            (int)DocumentType.PurchaseOrder ,
            (int)DocumentType.wov_purchase,
            (int)DocumentType.DeleteWov_purchase,
            (int)DocumentType.ReturnWov_purchase
        };

        public static List<int> InvoicesTypeForExtractStore_Setting = new List<int>
        {
            (int)DocumentType.Sales,
            (int)DocumentType.ExtractPermission,
            (int)DocumentType.DeleteSales,  
            (int)DocumentType.DeleteExtractPermission,
            (int)DocumentType.ReturnSales,
            (int)DocumentType.OfferPrice
        };

        public static List<int> MainInvoiceForReturn = new List<int>
        {
            (int)DocumentType.Purchase,
            (int)DocumentType.Sales,
            (int)DocumentType.POS,
            (int)DocumentType.wov_purchase,
        };

        public static List<int> InvoiceCreditorList = new List<int>
        {
            (int) DocumentType.Purchase,
            (int) DocumentType.ReturnPOS,
            (int) DocumentType.ReturnSales,
            (int) DocumentType.SafePayment,
            (int) DocumentType.BankPayment
        };
        public static List<int> InvoiceDebtorList = new List<int>
        {
            (int) DocumentType.ReturnPurchase,
            (int) DocumentType.POS,
            (int) DocumentType.Sales,
            (int) DocumentType.SafeCash,
            (int) DocumentType.BankCash
        };
        // المشتريات 
        public static List<int> purchasesWithOutDeleteInvoicesList = new List<int>()
        {
            (int)DocumentType.Purchase,
            (int)DocumentType.ReturnPurchase,
            (int)DocumentType.wov_purchase,
            (int)DocumentType.ReturnWov_purchase
        };
        //المبيعات 

        public static List<int> SalesWithOutDeleteInvoicesList = new List<int>()

        {
            (int)DocumentType.POS,
            (int)DocumentType.Sales,
            (int)DocumentType.ReturnSales ,
            (int)DocumentType.ReturnPOS
        };
        public static List<int> OnlySalesWithOutDeleteInvoicesList = new List<int>()
        {
            (int)DocumentType.POS,
            (int)DocumentType.Sales
        };
        public static List<int> OnlyReturnWithOutDeleteInvoicesList = new List<int>()
        {
            (int)DocumentType.ReturnSales ,
            (int)DocumentType.ReturnPOS
        };

        // فواتير ميتعملش ع كمياتها تشيك عند الاضافه
        public static List<int> QuantityNotCheckedInvoicesList = new List<int>()
        {
            (int)DocumentType.Purchase,
            (int)DocumentType.AddPermission,
            (int)DocumentType.itemsFund,
            (int)DocumentType.wov_purchase
        };

        //accreditInvoice
        public static List<int> invoiceAccredit = new List<int>()
        {
            (int)DocumentType.POS,
            (int)DocumentType.Sales,
            (int)DocumentType.ReturnSales ,
            (int)DocumentType.ReturnPOS ,
            (int)DocumentType.Purchase,
            (int)DocumentType.ReturnPurchase
        };

        public static List<int> ItemsHaveNoEffictOnInvoice = new List<int>
        {
            (int)ItemTypes.Note,
            (int)ItemTypes.Service,
            (int)ItemTypes.Additives
        };
        public static List<int> CompositeItemOnInvoice = new List<int>
        {
            (int)DocumentType.Sales, 
            (int)DocumentType .ReturnSales,
            (int)DocumentType .POS,
            (int) DocumentType.ReturnPOS,
            (int) DocumentType.ExtractPermission,
            (int) DocumentType.OutgoingTransfer,
            (int) DocumentType.IncomingTransfer,
            (int) DocumentType.OfferPrice,
        };

        public static List<int> orderPurchaseAndOfferPrice = new List<int>()
        { (int)DocumentType.OfferPrice,(int)DocumentType.DeleteOfferPrice,
          (int)DocumentType.PurchaseOrder ,(int)DocumentType.DeletePurchaseOrder};
        public static List<ReceiptsAuthority> receiptsAuthorities = new List<ReceiptsAuthority>()
        {
            new ReceiptsAuthority(){ Id=AuthorityTypes.customers,arabicName = "العملاء" , latinName = "customers" } ,
            new ReceiptsAuthority(){ Id=AuthorityTypes.suppliers,arabicName = "الموردين" , latinName = "suppliers" } ,
            new ReceiptsAuthority(){ Id=AuthorityTypes.DirectAccounts,arabicName = "الحسابات العامه" , latinName = "Financial Account" } ,
            new ReceiptsAuthority(){ Id=AuthorityTypes.other,arabicName = "جهات اخرى" , latinName = "ohtor Authority" } ,
            new ReceiptsAuthority(){ Id=AuthorityTypes.salesman,arabicName = "مندوب المبيعات" , latinName = "Sales man" },
            new ReceiptsAuthority(){ Id=AuthorityTypes.Safe,arabicName = "خزائن" , latinName = "Safes" },
            new ReceiptsAuthority(){ Id=AuthorityTypes.Bank,arabicName = "بنوك" , latinName = "Banks" }



        };

        public static List<int> SalesTransaction = new List<int>()
        {
            (int)DocumentType.Sales,
            (int)DocumentType.POS,
            (int)DocumentType.ReturnSales,
            (int)DocumentType.ReturnPOS,
            (int)DocumentType.ExtractPermission,
            
        };
        public static List<int> invociesTransaction = new List<int>()
        {
            (int)DocumentType.Sales,
            (int)DocumentType.POS,
            (int)DocumentType.ReturnSales,
            (int)DocumentType.ReturnPOS,
            (int)DocumentType.Purchase,
            (int)DocumentType.ReturnPurchase
        };
        public static List<paymentTypes> paymentTypes = new List<paymentTypes>()
        {
            new paymentTypes()
            {
                id = 0,
                arabicName = "مسدد",
                latinName = "Paid",
            },
            new paymentTypes()
            {
                id = (int)PaymentType.Complete,
                arabicName = "مسدد",
                latinName = "Paid",
            },
            new paymentTypes()
            {
                id = (int)PaymentType.Delay,
                arabicName = "اجل",
                latinName = "Delay",
            },
            new paymentTypes()
            {
                id = (int)PaymentType.Partial,
                arabicName = "جزئي",
                latinName = "Partial",
            }
        };

        public static List<int> InvoicesHaveDiscounts = new List<int>()
        { 
            (int)DocumentType.Purchase, (int)DocumentType.ReturnPurchase,  (int)DocumentType.DeletePurchase,
            (int)DocumentType.wov_purchase,  (int)DocumentType.ReturnWov_purchase,  (int)DocumentType.DeleteWov_purchase,
            (int)DocumentType.Sales, (int)DocumentType.ReturnSales, (int)DocumentType.DeleteSales,
            (int)DocumentType.POS,  (int)DocumentType.ReturnPOS,   (int)DocumentType.DeletePOS,
            (int)DocumentType.OfferPrice,(int)DocumentType.DeleteOfferPrice,
            (int)DocumentType.PurchaseOrder ,(int)DocumentType.DeletePurchaseOrder
        };

        public static List<int> AllInvoices = new List<int>()
        {
            (int)DocumentType.POS,
            (int)DocumentType.ReturnPOS,
            (int)DocumentType.Sales,
            (int)DocumentType.ReturnSales,
            (int)DocumentType.Purchase,
            (int)DocumentType.ReturnPurchase,
        };
        public static List<int> MainPaymentMehod = new List<int> { (int)PaymentMethod.paid, (int)PaymentMethod.Cheques, (int)PaymentMethod.net };

        public static List<ReceiptTitle> listOfReceiptTitle()
        {
            var _listOfReceiptTitle = new List<ReceiptTitle>();
            _listOfReceiptTitle.Add(new ReceiptTitle
            {
                Id = (int)ReceiptTitleIds.BankCashReceipt,
                TitleAr = "سند قبض بنك",
                TitleEn = "Bank Cash Receipt"
            });
            _listOfReceiptTitle.Add(new ReceiptTitle
            {
                Id = (int)ReceiptTitleIds.BankPaymentReceipt,
                TitleAr = "سند صرف بنك",
                TitleEn = "Bank Payment Receipt"
            });  
            
            _listOfReceiptTitle.Add(new ReceiptTitle
            {
                Id = (int)ReceiptTitleIds.SafeCashReceipt,
                TitleAr = "سند قبض خزينة",
                TitleEn = "Safe Cash Receipt"
            });
            _listOfReceiptTitle.Add(new ReceiptTitle
            {
                Id = (int)ReceiptTitleIds.SafePaymentReceipt,
                TitleAr = "سند صرف خزينة",
                TitleEn = "Safe Payment Receipt"
            });

            _listOfReceiptTitle.Add(new ReceiptTitle
            {
                Id = (int)ReceiptTitleIds.SafeMultiCollectionReceipts,
                TitleAr = "سند مجمع خزينة",
                TitleEn = "Safe Multi Collection Receipts"
            }); 
            _listOfReceiptTitle.Add(new ReceiptTitle
            {
                Id = (int)ReceiptTitleIds.BankMultiCollectionReceipts,
                TitleAr = "سند مجمع بنك",
                TitleEn = "Bank Multi Collection Receipts"
            });

            _listOfReceiptTitle.Add(new ReceiptTitle
            {
                Id = (int)ReceiptTitleIds.safeTransferReceipts,
                TitleAr = "سند تحويل خزينة",
                TitleEn = "Safe Transfer Receipts"
            });
            _listOfReceiptTitle.Add(new ReceiptTitle
            {
                Id = (int)ReceiptTitleIds.bankTransferReceipts,
                TitleAr = "سند تحويل بنك",
                TitleEn = "Bank Transfer Receipts"
            });

            _listOfReceiptTitle.Add(new ReceiptTitle
            {
                Id = (int)ReceiptTitleIds.SafeCollectionReceipts,
                TitleAr = "سند سداد خزينة",
                TitleEn = "Safe Collection Receipts"
            });
            _listOfReceiptTitle.Add(new ReceiptTitle
            {
                Id = (int)ReceiptTitleIds.BankCollectionReceipts,
                TitleAr = "سند سداد بنك",
                TitleEn = "Bank Collection Receipts"
            });
            return _listOfReceiptTitle;
        }
    }
}
