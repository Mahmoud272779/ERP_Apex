using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{


    public class GlRecieptsResopnseDTO
    {
        public int safeOrBankNameID { get; set; }
        public string safeOrBankNameArabic { get; set; }
        public string safeOrBankNameLatin { get; set; }
        public int Code { get; set; }
        public string PaperNumber { get; set; }
        public DateTime RecieptDate { get; set; }
        public string Notes { get; set; }
        public string InvoiceNotes { get; set; }
        public double Amount { get; set; } // المبلغ
        public int Authority { get; set; } //  الجهه
        public int PaymentMethodId { get; set; } // طريقة السداد
        public string PaymentMethodNameArabic { get; set; } // طريقة السداد
        public string PaymentMethodNameLatin { get; set; } // طريقة السداد
        public string ChequeNumber { get; set; }
        public string ChequeBankName { get; set; } // اسم بنك الشيك
        public DateTime ChequeDate { get; set; }
        public int RecieptTypeId { get; set; } //نوع السند (صرف خزينه - صرف بنك - قبض خزينه - قبض بنك) 
        public string RecieptType { get; set; }//1-sc-5
        public int Signal { get; set; }// صرف -1  لو قبض 1
        public int? ParentId { get; set; } // كود الفاتورة او خصم مكتسب او مسموح به اللي معمول من خلاله السند
        public int? ParentTypeId { get; set; }// نوع الفاتورة او الخصم
        public double Serialize { get; set; }
        public int BranchId { get; set; }
        public int UserId { get; set; }
        public bool IsAccredit { get; set; } = true;
        public DateTime CreationDate { get; set; }
        public int otherSalesManId { get; set; }
        public string benefitNameArabic { get; set; }
        public string benefitNameLatin { get; set; }
        public string authorityNameArabic { get; set; }
        public string authorityNameLatin { get; set; }
        public bool IsIncludeVat { get; set; }
        public int BenefitId { get; set; }           // benefit id  for all authority
        public object benefitCode { get; set; }        // benefit id  for all authority
        public string financialCode { get; set; }  // benefit id  for all authority
        public bool IsBlock { get; set; }
        public ReceiptTitle receiptTitle { get; set; }
        public GlRecieptsResopnseDTO()
        {
            FilesDetails = new List<FilesListDto>();
        }
        public IReadOnlyCollection<GLRecieptFiles> RecieptsFiles { get; set; }
        public List<GLRecieptCostCenter> RecieptsCostCenter { get; set; }=new List<GLRecieptCostCenter> { };
        public bool isSafe { get; set; } = false;
        public List<FilesListDto> FilesDetails { get; set; }

    }
    public class ReceiptTitle
    {
        public int Id { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
    }

    public class ReceiptsResponseDto
    {

        public int Id { get; set; }
        public string Code { get; set; }
        public int? SafeOrBankId { get; set; }
        public string RecieptType { get; set; }//1-sc-5
        public DateTime? RecieptDate { get; set; }
        public string Notes { get; set; }
        public double Serialize { get; set; }
        public int PaymentMethod { get; set; } // طريقة السداد
        public string chequeNumber { get; set; }
        public DateTime? chequeDate { get; set; }
        public int? PaymentWays { get; set; }
        public double? TotalPercentage { get; set; }
        public string BankCheque { get; set; }
        public double? TotalNumber { get; set; }
        public bool IsIncludeVat { get; set; }
        public int AuthortyNo { get; set; }

        public int BenefitId { get; set; }
        public bool IsBlock { get; set; }
        public List<ReceiptsCostCenterListDto> costs { get; set; }
        public List<ReceiptsFilesListDto> Files { get; set; }
    }
    public class ReceiptsFilesListDto
    {
        public int Id { get; set; }
        public int SupportId { get; set; }
        public string File { get; set; }
    }
    public class ReceiptsCostCenterListDto
    {
        public int Id { get; set; }
        public int CostCenterId { get; set; }
        public int SupportId { get; set; }
        public string CostCenteCode { get; set; }
        public string CostCenteName { get; set; }
        public double Percentage { get; set; }
        public double Number { get; set; }
    }

    public class MainReceiptsData
    {
        public int Code { get; set; }
        public string ReceiptsType { get; set; }
        public int Id { get; set; }
        public string financialNameAr { get; set; }
        public string financialNameEn { get; set; }
        public string benefitNameAr { get; set; }
        public string benefitNameEn { get; set; }
        public string safeOrBankNameAr { get; set; }  
        public string safeOrBankNameEn { get; set; }
        public string paymentMethodAr { get; set; }
        public string paymentMethodEn { get; set; }
        public double Amount { get; set; }  
        public bool isBlock { get; set; }
        public bool isAccredit { get; set; }
        public DateTime date { get; set; }
        public bool isEdit { get; set; } = true;
        public bool isInvoice { get; set; }
        public int invoiceId { get; set; }
    }
    public class MainTransferReceiptData : MainReceiptsData
    {
        public int TSafeOrTBankID { get; set; }
        public string TSafeOrTBankNameAr { get; set; }
        public string TSafeOrTBankNameEn { get; set; }
    }

} 

