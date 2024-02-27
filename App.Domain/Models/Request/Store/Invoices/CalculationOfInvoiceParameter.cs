using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request.Invoices
{
    public class CalculationOfInvoiceParameter
    {
        public CalculationOfInvoiceParameter()
        {
            itemDetails = new List<InvoiceDetailsAttributes>();
        }
        // List Of Items was entered at invoice by user 
        public List<InvoiceDetailsAttributes> itemDetails { get; set; }
        
        public int DiscountType { get; set; } // front will determine this enum "Enums -> DiscountType"

        public double TotalDiscountValue { get; set; } // اجمالى قيمة الخصم يتم توزيعه على الأصناف
        public double TotalDiscountRatio { get; set; } // اجمالى نسبة الخصم يتم توزيعه على الأصناف
        public int InvoiceTypeId { get; set; } // front will determine this enum  "Enums -> DocumentType"
        public int? InvoiceId { get; set; } // كود الفاتورة فى حالة التعديل على فاتورة قديمة
        public string ParentInvoice { get; set; } // لو الفاتورة مرتجع لفاتورة مشتريات او مبيعات ....
        public int? PersonId { get; set; }
        public bool? isCopy { get; set; } = false;
        public bool isDiscountRatio { get; set; }
        public invoiceExistSettings invoiceExistSetting { get; set; } 
    }
 
    public class InvoiceDetailsAttributes
    {
        public string itemCode { get; set; }
        public int ItemTypeId { get; set; }  // FillItemCardQuery نوع الصنف بيتم ارساله للفرونت فى ال 
        public double Quantity { get; set; } = 0.0;// كمية الصنف
        public double Price { get; set; } = 0.0; // سعر الشراء او البيع
                                                 //  public double SalePrice { get; set; } // سعر البيع
        public double DiscountValue { get; set; } = 0.0; //قيمه الخصم على مستوى الصنف 
        public double DiscountRatio { get; set; } = 0.0;// نسبه الخصم على مستوى الصنف
        public bool ApplyVat { get; set; } // الصنف يخضع للضريبة ؟
        public double VatRatio { get; set; } = 0.0;// الضريبه على مستوى الصنف 
        public bool isBalanceBarcode { get; set; }// هل الصنف باركود ميزان او لا
        public bool isDiscountRatioItem { get; set; }

    }

    public class CheckQuantityInStoreRequest
    {
        public int StoreId { get; set; }
        public int ItemId { get; set; }
        public int UnitId { get; set; }
        public double Quantity { get; set; }
        public int ItemTypeId { get; set; }
        public string ParentInvoiceType { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int invoiceTypeId { get; set; }
        public DateTime invoiceDate { get; set; }
    }

    public class checkItemPrice
    { 
    
    public int itemId { get; set; } 
    public int unitId { get; set; }
        public int? personId { get; set; } 
        public int invoiceTypeId { get; set; }
        public double price { get; set; }
        public int? salesManId { get; set; }
        public int? PriceListId { get; set; }

    }

    public class invoiceExistSettings
    {
        public bool ApplyVat { get; set; }//يخضع للضريبه ام لا
        public bool PriceWithVat { get; set; }//السعر شامل الضريبه ام لا
        public int RoundNumber { get; set; } //  رقم التقريب المسجل عند حفظ الفاتورة
        public bool ActiveDiscount { get; set; }

    }
}
