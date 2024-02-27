using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Domain.Models.Security.Authentication.Request.Reports;
using Microsoft.AspNetCore.Http;

namespace App.Domain.Models.Request.POS
{
    public class InvoiceSuspensionRequest
    {
        public string BookIndex { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public int StoreId { get; set; }
        public int? StoreIdTo { get; set; }
        public string Notes { get; set; }
        //  public int BranchId { get; set; }
        public double TotalPrice { get; set; }
        public int? SalesManId { get; set; } = null;// المندوب
        public int? PriceListId { get; set; } = null;// قائمة الأسعار
        public int? PersonId { get; set; }//المورد  او العميل 
        public double TotalDiscountValue { get; set; }//قيمه الخصم
        public double TotalDiscountRatio { get; set; }//نسبة الخصم
        public double Net { get; set; }
        public double Paid { get; set; } 
        public double Remain { get; set; }
        public double VirualPaid { get; set; }
        public double TotalAfterDiscount { get; set; } 
        public double TotalVat { get; set; }
        public int DiscountType { get; set; } 
        public bool ActiveDiscount { get; set; }
        public bool ApplyVat { get; set; }
        public bool PriceWithVat { get; set; }
        public int transferStatus { get; set; }

        public string? ParentInvoiceCode { get; set; } 
        public IFormFile[] AttachedFile { get; set; }
        
        public List<InvoiceDetailsRequest> InvoiceDetails { get; set; } = new List<InvoiceDetailsRequest>();
        public List<OtherAdditionList> OtherAdditionList { get; set; } = new List<OtherAdditionList>();
        public List<PaymentList>? PaymentsMethods { get; set; } = new List<PaymentList>();
    }
    public class InvoicSuspensioneDetailsRequest
    {
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
        public double Total { get; set; }
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
        //public double TransQuantity { get; set; }//الكمية المحولة
        //public double ReturnQuantity { get; set; }//الكمية المرتجعة
        //public int StatusOfTrans { get; set; }//التحويلات
        public double SplitedDiscountValue { get; set; } // قيمة الخصم الاجمالى الموزع 
        public double SplitedDiscountRatio { get; set; } //  نسبة الخصم الاجمالى الموزع
                                                         // public double AvgPrice { get; set; } // الربحية 
                                                         //public double Cost { get; set; }// الربحية
        public double AutoDiscount { get; set; }
        public double ConversionFactor { get; set; }  
        public int IndexOfItem { get; set; }
        public string? SerialTexts { get; set; }
    }
}
