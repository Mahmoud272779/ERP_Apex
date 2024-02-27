using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.GeneralLedger
{
    public class CompinedRecieptsRequest
    {
        public int UserId { get; set; }
        public int BranchId { get; set; }
        public int? SafeID { get; set; }
        public int? BankId { get; set; }
        public int RecieptTypeId { get; set; }
        public DateTime? RecieptDate { get; set; }
        public string PaperNumber { get; set; }
        public string Notes { get; set; }
        
        public List<CompinedReciept> Reciepts { get; set; } = new List<CompinedReciept>();
        public IFormFile[] AttachedFile { get; set; }
    }
    public class CompinedReciept
    {
        public int? RecieptId { get; set; }
        public string Notes { get; set; }
        public double Amount { get; set; } // المبلغ
        public int Authority { get; set; } // الجهه
        public int PaymentMethodId { get; set; } // طريقة السداد
        public string ChequeNumber { get; set; }
        public string ChequeBankName { get; set; } // اسم بنك الشيك
        public DateTime ChequeDate { get; set; }
                                    
        public int BenefitId { get; set; }
       
        public List<CostCenterReciepts> costCenterReciepts { get; set; }
       
    }
    public class UpdateCompinedRecieptsRequest : CompinedRecieptsRequest
    {
        public int ParentRecieptId { get; set; }
        public List<int> FileId { get; set; } // ملفات قديمة لن يتم حذفها
        //public List<UpdateCostCenterReciepts> costCenterReciepts { get; set; }
    }
}
