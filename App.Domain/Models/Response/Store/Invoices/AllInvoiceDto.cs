using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Setup.ItemCard.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response.PurchasesDtos
{
    public class AllInvoiceDto
    {
        public int InvoiceId { get; set; }
        public string InvoiceType { get; set; }
        public string BookIndex { get; set; }
        public DateTime InvoiceDate { get; set; }
        public double TotalPrice { get; set; }
        public double paid { get; set; }
        public double Discount { get; set; }
        public int InvoiceTypeId { get; set; }
        public int InvoiceSubTypesId { get; set; } // enum of invoice sub Types for main types in InvoiceTypeId
        public int PaymentType { get; set; }
        public bool CanDelete { get; set; } = true;
        public bool CanEdit { get; set; } = true;
        public bool IsDeleted { get; set; }
        public int StoreId { get; set; }
        public string StoreNameAr { get; set; }
        public string StoreNameEn { get; set; }
        public int? PersonId { get; set; }
        public string PersonNameAr { get; set; }
        public string PersonNameEn { get; set; }
        public int PersonStatus { get; set; }
        public string PersonEmail { get; set; }
        public string ParentInvoiceCode { get; set; }
        public bool IsAccredited { get; set; }
        public bool IsReturn { get; set; } // تم عمل مرتجع للفاتورة
        public bool? isExpenses { get; set; } // مصروفات

    }
    public class InvoiceInfoDTO
    {
        public int InvoiceId { get; set; }
        public string InvoiceType { get; set; }
        public int InvoiceTypeId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public double TotalPrice { get; set; }
        public double Discount { get; set; }
        public int InvoiceSubTypesId { get; set; }
        public int PaymentType { get; set; }
        public string PersonNameAr { get; set; }
        public string PersonNameEn { get; set; }
        public double paid { get; set; }
    }

    public class SuspensionInvoiceInfoDTO : InvoiceInfoDTO
    {
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public int itemsCount { get; set; }
    }
    public class InvoiceDto
    {
       
        public int InvoiceId { get; set; }
        public string InvoiceCode { get; set; }
        public string ParentInvoiceCode { get; set; }
        public int InvoiceTypeId { get; set; }
        public int InvoiceSubTypesId { get; set; }

        public InvoiceDto()
        {
            InvoiceDetails = new List<InvoiceDetailsDto>();
            OtherAdditionList = new List<OtherAdditionListDto>();
            PaymentsMethods = new List<PaymentListDto>();
            FilesDetails = new List<FilesListDto>();
        }
        public string QRCode { get; set; }
        public string BookIndex { get; set; } = "";
        public string InvoiceDate { get; set; }
        public int StoreId { get; set; }
        public int? StoreIdTo { get; set; }
        public string StoreNameAr { get; set; }
        public string StoreNameEn { get; set; }
        //Store To Transfer

        public string StoreToNameAr { get; set; }
        public string StoreToNameEn { get; set; }
        public int StoreStatus { get; set; }

        public string Notes { get; set; }
        public int BranchId { get; set; }
        public string BranchNameAr { get; set; }
        public string BranchNameEn { get; set; }
        public string BranchPhoneNumber { get; set; }
        public string BranchAddressAr { get; set; }
        public string BranchAddressEn { get; set; }
        public string CommercialRegisterNumber { get; set; }    



        //المشتريات
        public int? PersonId { get; set; }//المورد
        public string PersonNameAr { get; set; }
        public string PersonNameEn { get; set; }
        public string? PersonTaxNumber { get; set; }
        public string? PersonAddressAr { get; set; }
        public string? PersonAddressEn { get; set; }
        public string PersonPhone { get; set; }
        public string PersonFax { get; set; }
        public string PersonEmail { get; set; }
        public double? PersonCreditLimit { get; set; }
        public double balance { get; set; }
        public bool  isCreditor { get; set; }
        public int PersonStatus { get; set; }

        public int PersonCode { get; set; }

        public int? PersonCreditPeriod { get; set; } // فترة الائتمان
        public double? PersonDiscountRatio { get; set; } // نسبة الخصم

        public string? PersonBuildingNumber { get; set; }   //  رقم المبنى     
        public string? PersonStreetName { get; set; } //اسم الشارع
        
        public string? PersonNeighborhood { get; set; }// الحي
        
        public string? PersonCity { get; set; } //المدينة
        
        public string? PersonCountry { get; set; } //الدولة
        
        public string? PersonPostalNumber { get; set; } // الرقم البريدى

        //for print

        public string PersonResponsibleAr { get; set; }// اسم المسول للعملا
        public string PersonResponsibleEn { get; set; }
       

        //end 
        public int SalesManId { get; set; }
        public string SalesManNameAr { get; set; }
        public string SalesManNameEn { get; set; }
        public string SalesManPhone { get; set; }
        public string SalesManEmail { get; set; }

        public int employeeId { get; set; }
        public string EmployeeNameAr { get; set; }
        public string EmployeeNameEn { get; set; }

        public string BrowserName { get; set; }
        public int Code { get; set; }
        public string InvoiceType { get; set; }
        public double Serialize { get; set; }

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
        public bool IsDeleted { get; set; }
        public bool IsAccredited { get; set; }
        public int RoundNumber { get; set; }
        public string balanceBarcode { get; set; }
        public bool IsCollectionReceipt { get; set; } // تمنع التعديل على الفاتورة لان تم اضافة سند تحصيل عليها
        public bool isDiscountRatio { get; set; }

        public bool isReturn { get; set; }
        public int? PriceListId { get; set; } = 0;// قائمة الأسعار
        public bool? isExpenses { get; set; } // مصروفات

        public List<InvoiceDetailsDto> InvoiceDetails { get; set; }
        public List<OtherAdditionListDto> OtherAdditionList { get; set; }
        public List<PaymentListDto> PaymentsMethods { get; set; }
        public List<FilesListDto> FilesDetails { get; set; }

    }
    public class InvoiceDetailsDto
    {
        public InvoiceDetailsDto()
        {
            InvoiceSerialDtos = new List<InvoiceSerialDto>();
            itemUnits = new List<ItemUnitsDto>();
        }
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemNameAr { get; set; }
        public string ItemNameEn { get; set; }

        public int? UnitId { get; set; }
        public string UnitNameAr { get; set; }
        public string UnitNameEn { get; set; }

        public string SerialNumbers { get; set; }
        public double Quantity { get; set; }
        public double OldQuantity { get; set; }
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
        public bool isBalanceBarcode { get; set; } = false;// هل الصنف باركود ميزان او لا
        public string balanceBarcode { get; set; }
        public double VatValueForPrint { get; set; } // القيمة الضريبية بعد الخصم على الصنف فى طباعة الفاتورة
        public bool isDiscountRatioItem { get; set; } // القيمة الضريبية بعد الخصم على الصنف فى طباعة الفاتورة

        public List<InvoiceSerialDto> InvoiceSerialDtos { get; set; }
        public List<InvoiceSerialDto> storeSerialDtos { get; set; }
        public string transferSerialDtos { get; set; }
        public List<ItemUnitsDto> itemUnits { get; set; } // for ios
    }
    public class InvoiceSerialDto
    {
  //      public int InvoiceId { get; set; }
        public int ItemId { get; set; }
        public string SerialNumber { get; set; }
        public bool CanDelete { get; set; } = true;
    }
    public class OtherAdditionListDto
    {
        public int AddtionalCostId { get; set; }
        public string AddtionalCostNameAr { get; set; }
        public string AddtionalCostNameEn { get; set; }

        public int Code { get; set; }
        public int AdditionalType { get; set; }
        public double Amount { get; set; }//القيمة المدفوعة 
        public double Total { get; set; }

    }
    public class PaymentListDto
    {
        public int PaymentMethodId { get; set; }
        public string PaymentNameAr { get; set; }
        public string PaymentNameEn { get; set; }
        public double Value { get; set; }
        public string Cheque { get; set; }
    }
    public class FilesListDto
    {
        public int FileId { get; set; }
        public int InvoiceId { get; set; }
        public string FileName { get; set; }
        public string FileLink { get; set; }
        public string FileExtensions { get; set; }
       // public double  FileSize { get; set; }
    }
}
