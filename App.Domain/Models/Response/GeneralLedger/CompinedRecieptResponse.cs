using App.Domain.Entities.Process;
using App.Domain.Models.Request.GeneralLedger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.GeneralLedger
{
    public class CompinedRecieptResponse
    {
        public int RecieptId { get; set; }
        public int Code { get; set; }
        public string ReceiptsType { get; set; }
        public string safeOrBankNameAr { get; set; }
        public string safeOrBankNameEn { get; set; }
        public double Amount { get; set; }
        public DateTime date { get; set; }

    }
    public class CompinedRecieptDetailsResponse
    {
        public int UserId { get; set; }
        public int BranchId { get; set; }
        public int Code { get; set; }
        public int safeOrBankID { get; set; }
        public string safeOrBankNameArabic { get; set; }
        public string safeOrBankNameLatin { get; set; }
        public int RecieptTypeId { get; set; }
        public string RecieptType { get; set; }
        public DateTime RecieptDate { get; set; }
        public string PaperNumber { get; set; }
        public double Amount { get; set; }
        public List<CompinedRecieptItem> Reciepts { get; set; } = new List<CompinedRecieptItem>();
        public IReadOnlyCollection<GLRecieptFiles> RecieptsFiles { get; set; }

    }

    public class CompinedRecieptItem
    {
        public int RecieptId { get; set; }
        public string Notes { get; set; }
        public double Amount { get; set; } // المبلغ
        public int Authority { get; set; } //  الجهه
        public int PaymentMethodId { get; set; } // طريقة السداد
        public string PaymentMethodNameArabic { get; set; } // طريقة السداد
        public string PaymentMethodNameLatin { get; set; }
        public string ChequeNumber { get; set; }
        public string ChequeBankName { get; set; } // اسم بنك الشيك
        public DateTime ChequeDate { get; set; }
        public string benefitNameArabic { get; set; }
        public string benefitNameLatin { get; set; }
        public string authorityNameArabic { get; set; }
        public string authorityNameLatin { get; set; }
        public int BenefitId { get; set; }
        public bool IsIncludeVat { get; set; }
    }
}
