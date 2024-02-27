using App.Domain.Entities;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain
{
    public class RecieptsRequest: MainRequestData
    {
        public List<CostCenterReciepts> costCenterReciepts { get; set; }
        public List<MultiCollectionReceiptsInvocesRequestDTO> invoices { get; set; }

    }
    public class MultiCollectionReceiptsInvocesRequestDTO
    {
        public int invoiceId { get; set; }
        public double amount { get; set; }
    }
    public class MainRequestDataForAdd
    {
        public List<CostCenterReciepts> costCenterReciepts { get; set; }
        public int? SafeID { get; set; }
        public int? BankId { get; set; }
        public string PaperNumber { get; set; }
        public DateTime RecieptDate { get; set; }
        public string Notes { get; set; } = ""; // ملاحطات طرق السداد في السندات
        public string InvoiceNotes { get; set; } = ""; // ملاحظات الفتورة الأساسية
        public double Amount { get; set; } // المبلغ
        public int Authority { get; set; } // الجهه To
        public int PaymentMethodId { get; set; } // طريقة السداد
        public string ChequeNumber { get; set; }
        public string ChequeBankName { get; set; }
        public DateTime ChequeDate { get; set; }
        public int RecieptTypeId { get; set; } // نوع السند ( صرف خزينة 19 - قيض خزينة 18 - صرف بنك 21 - قيض بنك 20 - تحويل من خزينة 45 - تحويل من بنك 46 
        // public int Signal { get; set; }// صرف -1  لو قبض 1
        //public int BranchId { get; set; }
        public int UserId { get; set; }
        // public DateTime CreationDate { get; set; }
        public int? SalesManId { get; set; }
        public bool IsIncludeVat { get; set; }
        public int BenefitId { get; set; }
        public IFormFile[] AttachedFile { get; set; }

    }

    public class MainRequestData
    {
        public int? SafeID { get; set; }
        public int? BankId { get; set; }
        [JsonIgnore]
        public int? CollectionMainCode { get; set; }
        public string PaperNumber { get; set; }
        public DateTime RecieptDate { get; set; }
       
        public string Notes { get; set; }
        public string InvoiceNotes { get; set; } = ""; // ملاحظات الفتورة الأساسية

        public double Amount { get; set; } // المبلغ
        public int Authority { get; set; } // الجهه --- من
        public int PaymentMethodId { get; set; } // طريقة السداد
        public string ChequeNumber { get; set; }
        public string ChequeBankName { get; set; } // اسم بنك الشيك
        public DateTime ChequeDate { get; set; }
        public int RecieptTypeId { get; set; } // نوع السند ( صرف خزينة 19 - قيض خزينة 18 - صرف بنك 21 - قيض بنك 20 - تحويل من خزينة 45 - تحويل من بنك 46                                    // public int Signal { get; set; }// صرف -1  لو قبض 1
        public int BranchId { get; set; }
        public int subTypeId { get; set; } = 0;
        public int UserId { get; set; }
        public double Creditor { get; set; }
        public double Debtor { get; set; }
        public int? SalesManId { get; set; }
        public bool IsIncludeVat { get; set; }
        public int BenefitId { get; set; }
        public bool fromInvoice { get; set; } = false;
        public IFormFile[] AttachedFile { get; set; }

        // From BackEnd
        [JsonIgnore]
        public int FA_Id { get; set; } = 0;
        // From BackEnd
        [JsonIgnore]
        public bool Deferre { get; set; } = false; //سند وهمى للفواتير الاجله
        public int? isPartialPaid { get; set; }
        public bool ReceiptOnly { get; set; } = false; 

        public bool IsAccredit { get; set; } = true;
        public int? ParentId { get; set; } // كود الفاتورة او خصم مكتسب او مسموح به اللي معمول من خلاله السند
        public int? ParentTypeId { get; set; }// نوع الفاتورة او الخصم .. from enum RecieptsParentType
        public string ParentType { get; set; }//1-p-6
        public int Code { get; set; }// from invoice
        public double Serialize { get; set; }//for invoice
    }
    public class CostCenterReciepts
    {
        public int CostCenterId { get; set; }
        public string CostCenteCode { get; set; }
        public string CostCenteName { get; set; }
        public double Percentage { get; set; }
        public double Number { get; set; }
    }
    public class UpdateCostCenterReciepts
    {
        public int Id { get; set; }//id if new costcenter id = 0
        public int CostCenterId { get; set; }
        public string CostCenteCode { get; set; }
        public string CostCenteName { get; set; }
        public double Percentage { get; set; }
        public double Number { get; set; }
    }
    public class GetRecieptsData: ReceiptSearch
    { 
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int ReceiptsType { get; set; }
    }

    public class ReceiptSearch
    {
        public string Code { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int FinancialAccountId { get; set; }
        public int BenefitId { get; set; }
        public int SafeOrBankId { get; set; }
        public int PaymentWays { get; set; }
        public int AuthotityId { get; set; }           
    }


//for attachment 
    public class financialData 
    { 
        public string FinancialName { get; set; }    
        public int financialId { get; set; }
        public string financialCode { get; set; }  
        public GlReciepts ReceiptsData { get; set; }
    }



    public class UpdateRecieptsRequest: MainRequestData
    { 
        public int Id { get; set; }
        public List<int> FileId { get; set; } // ملفات قديمة لن يتم حذفها
        public List<UpdateCostCenterReciepts> costCenterReciepts { get; set; }
    }

    public class ValidationData

    {
        public int? safeBankId { get; set; }
        public int? financialAccountId { get; set; }
        public string  ErrorMessageAr { get; set; }
        public string  ErrorMessageEn { get; set; }
    }

    public class ReceiptsOfflinePos : MainRequestData
    {
        public string RecieptType { get; set; }//1-sc-5
        public string RectypeWithPayment { get; set; }
        public DateTime CreationDate { get; set; }
        public int Signal { get; set; }
        public int? PersonId { get; set; }//benefit id  for persons only 

    }

    
}
