using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request.Reports
{

    public class InvoiceMasterRequest
    {
        public string BookIndex { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int StoreId { get; set; }
        public int? StoreIdTo { get; set; }
        public string Notes { get; set; }
        //  public int BranchId { get; set; }
        public double TotalPrice { get; set; }
        public int? SalesManId { get; set; } = 0;// المندوب
        public int? PriceListId { get; set; } = 0;// قائمة الأسعار
        public int? PersonId { get; set; }//المورد  او العميل 
        public double TotalDiscountValue { get; set; }//قيمه الخصم
        public double TotalDiscountRatio { get; set; }//نسبة الخصم
        public double Net { get; set; }//الصافي
        public double Paid { get; set; } //المدفوع 
        public double Remain { get; set; }//المتبقي
        public double VirualPaid { get; set; }//المدوفوع من العميل 
        public double TotalAfterDiscount { get; set; } //اجمالي بعد الخصم
        public double TotalVat { get; set; }//اجمالي قيمه الضريبه 
        public int DiscountType { get; set; } // enum  نوع الخصم (اجمالي او على الصنف)
        public bool ActiveDiscount { get; set; }
        public bool ApplyVat { get; set; }//يخضع للضريبه ام لا
        public bool PriceWithVat { get; set; }//السعر شامل الضريبه ام لا
        public int transferStatus { get; set; }

        public string? ParentInvoiceCode { get; set; } // كود الفاتورة الاصليه لو بعمل مرتجع او حذف
        public IFormFile[] AttachedFile { get; set; }
        public bool? isOtherPayment { get; set; } = false; // لو دفع بطرق سداد اخرى
        public bool isDiscountRatio { get; set; }

        public List<InvoiceDetailsRequest> InvoiceDetails { get; set; } = new List<InvoiceDetailsRequest>();
        public List<OtherAdditionList> OtherAdditionList { get; set; } = new List<OtherAdditionList>();
        public List<PaymentList>? PaymentsMethods { get; set; } = new List<PaymentList>();
        public int OfferPriceId { get; set; }
        public bool? isExpenses { get; set; } = false;// مصروفات

        public bool IsPurchasesWithoutVat { get; set; } = false;
    }
    public class OtherAdditionList
    {
        public int AddtionalCostId { get; set; }
        public int AdditionsType { get; set; } 
        public double Amount { get; set; }//القيمة المدفوعة 

    }
    public class PaymentList
    {
        public int PaymentMethodId { get; set; }
        public double Value { get; set; }
        public string Cheque { get; set; }
    }

    public class InvoiceDetailsRequest
    {
        public InvoiceDetailsRequest()
        {
            ListSerials = new List<string>();
        }

        public int InvoiceId { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        //  public string ItemNameAr { get; set; }
        //   public string ItemNameEn { get; set; }
        public int? UnitId { get; set; }
        public int? SizeId { get; set; } = null;
        //     public string UnitNameAr { get; set; }
        //     public string UnitNameEn { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; } // سعر الشراء او البيع

        //  public double PurchasePrice { get; set; }//سعر الشراء
        public double Total { get; set; }  // for front total without splited discount just discount value on item
        public double TotalWithSplitedDiscount { get; set; } // for reports total with splited discount

        public DateTime? ExpireDate { get; set; }
        // public int Signal { get; set; }
        public int ItemTypeId { get; set; }

        //المشتريات
        public double DiscountValue { get; set; }//قيمه الخصم على مستوى الصنف
        public double DiscountRatio { get; set; }// نسبه الخصم على مستوى الصنف
        public bool ApplyVat { get; set; }
        public double VatRatio { get; set; }// الضريبه على مستوى الصنف 
        public double VatValue { get; set; }// الضريبه على مستوى الصنف 

        //   public double SalePrice { get; set; }//سعر البيع 
        public double TransQuantity { get; set; }//الكمية المحولة
        public int TransStatus { get; set; }//الحاله  المحولة
        //public double ReturnQuantity { get; set; }//الكمية المرتجعة
        //public int StatusOfTrans { get; set; }//التحويلات
        public double SplitedDiscountValue { get; set; } // قيمة الخصم الاجمالى الموزع 
        public double SplitedDiscountRatio { get; set; } //  نسبة الخصم الاجمالى الموزع
                                                         // public double AvgPrice { get; set; } // الربحية 
                                                         //public double Cost { get; set; }// الربحية
        public double AutoDiscount { get; set; }// خصومات الاصناف
                                                // public int PriceList { get; set; }// قائمه اسعار البيع (enum)
                                                // public double MinimumPrice { get; set; }// اقل سعر بيع 
        public double ConversionFactor { get; set; }  // معامل التحويل
                                                      //  public double SplitedDiscount { get; set; } // الخصم الاجمالى الموزع
        public int IndexOfItem { get; set; }// لترتيب الاصناف بنفس ترتيب اضافتها لو بيساوي زيرو يبقا ده من مكونات صنف مركب
        public bool isBalanceBarcode { get; set; } = false;// هل الصنف باركود ميزان او لا
        public string balanceBarcode { get; set; }
        public int? parentItemId { get; set; } // رقم الصنف المركب 
        public List<string> ListSerials { get; set; }
        public double VatValueForPrint { get; set; } // القيمة الضريبية بعد الخصم على الصنف فى طباعة الفاتورة
        public bool isDiscountRatioItem { get; set; }

       
    }
    public class ListOfSerialsInvoice
    {
        public string SerialNumber { get; set; }
    }
    public class UpdateInvoiceMasterRequest : InvoiceMasterRequest
    {
        public int InvoiceId { get; set; }
        public List<int> FileId { get; set; } // ملفات قديمة لن يتم حذفها
        public int? posSessionId { get; set; }

    }

    public class ListOfSerials
    {
        public string SerialNumber { get; set; }
    }


    public class invoiceDetailsCollection
    {
        public List<InvoiceDetailsRequest> InvoiceDetails { get; set; } = new List<InvoiceDetailsRequest>();

    }
    public class CompositeItemsRequest
    {
        public int itemId { get; set; }
        public int unitId { get; set; }
        public double quantity { get; set; }
        public int indexOfItem { get; set; }
        public int invoiceId { get; set; }
    }
}
