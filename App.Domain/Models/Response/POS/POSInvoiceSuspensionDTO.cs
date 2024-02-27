using App.Domain.Models.Request.POS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.POS
{
    public class POSInvoiceSuspensionDTO
    {
        public int InvoiceId { get; set; }
        public string InvoiceCode { get; set; }
        public string ParentInvoiceCode { get; set; }
        public int InvoiceTypeId { get; set; }

        public POSInvoiceSuspensionDTO()
        {
            InvoiceDetails = new List<InvoicSuspensioneDetailsDTO>();
        }

        public string BookIndex { get; set; } = "";
        public DateTime InvoiceDate { get; set; }
        public int StoreId { get; set; }
        public string StoreNameAr { get; set; }
        public string StoreNameEn { get; set; }
        public int StoreStatus { get; set; }

        public string Notes { get; set; }
        public int BranchId { get; set; }
        public string BranchNameAr { get; set; }
        public string BranchNameEn { get; set; }
        //المشتريات
        public int? PersonId { get; set; }//المورد
        public string PersonNameAr { get; set; }
        public string PersonNameEn { get; set; }
        public string? PersonTaxNumber { get; set; }
        public string? PersonAddressAr { get; set; }
        public int PersonStatus { get; set; }
        public int SalesManId { get; set; }
        public string SalesManNameAr { get; set; }
        public string SalesManNameEn { get; set; }

        public double TotalPrice { get; set; }

        public double TotalDiscountValue { get; set; }//قيمه الخصم
        public double TotalDiscountRatio { get; set; }//نسبة الخصم
        public double Net { get; set; }//الصافي
        public double Paid { get; set; }//المدفوع 
        public double Remain { get; set; }//المتبقي
        public double VirualPaid { get; set; }//المدوفوع من العميل 
        public double TotalAfterDiscount { get; set; } //اجمالي بعد الخصم
        public double TotalVat { get; set; }//اجمالي قيمه الضريبه 
        public bool? ApplyVat { get; set; }//يخضع للضريبه ام لا
        public bool? PriceWithVat { get; set; }//السعر شامل الضريبه ام لا
        public int DiscountType { get; set; } //نوع الخصم (اجمالي او على الصنف)
        public int PaymentType { get; set; }
        public double TotalPaymentsMethod { get; set; }// اجمالي المسدد
        public bool? ActiveDiscount { get; set; }
        public bool CanDeleted { get; set; }

        public List<InvoicSuspensioneDetailsDTO> InvoiceDetails { get; set; }

    }

    public class InvoicSuspensioneDetailsDTO
    {

        public int InvoiceId { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemNameAr { get; set; }
        public string ItemNameEn { get; set; }

        public int? UnitId { get; set; }
        public string UnitNameAr { get; set; }
        public string UnitNameEn { get; set; }


        public double Quantity { get; set; }
        public double Price { get; set; }//سعر الشراء
        public double Total { get; set; }
        public string ExpireDate { get; set; }
        public int Signal { get; set; }
        public int ItemTypeId { get; set; }

        //المشتريات
        public double DiscountValue { get; set; }//قيمه الخصم على مستوى الصنف
        public double DiscountRatio { get; set; }// نسبه الخصم على مستوى الصنف
        public double VatRatio { get; set; }// الضريبه على مستوى الصنف 
        public double VatValue { get; set; }// الضريبه على مستوى الصنف 


        public double TransQuantity { get; set; }//الكمية المحولة
        public double ReturnQuantity { get; set; }//الكمية المرتجعة
        public int StatusOfTrans { get; set; }//التحويلات
        public double SplitedDiscountValue { get; set; } // قيمة الخصم الاجمالى الموزع 
        public double SplitedDiscountRatio { get; set; } //  نسبة الخصم الاجمالى الموزع
        public double AvgPrice { get; set; } // الربحية 
        public double Cost { get; set; }// الربحية
        public double AutoDiscount { get; set; }// خصومات الاصناف
        public int PriceList { get; set; }// قائمه اسعار البيع (enum)
        public double MinimumPrice { get; set; }// اقل سعر بيع 
        public double ConversionFactor { get; set; }
        public int IndexOfItem { get; set; }
        public bool CanDelete { get; set; } = true;
        public bool ApplyVat { get; set; }

        public string SerialTexts { get; set; }

    }
}
