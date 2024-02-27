using App.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLRecieptsHistory 
    {
        public int Id { get; set; }
        public int ReceiptsId { get; set; }
        public int SafeIDOrBank { get; set; }
        public int Code { get; set; }
        public DateTime RecieptDate { get; set; }
        public double Amount { get; set; } // المبلغ
        public int AuthorityType { get; set; } // الجهه
        public int PaymentMethodId { get; set; } // طريقة السداد
        public string ChequeNumber { get; set; }
        public int RecieptTypeId { get; set; } //نوع السند (صرف خزينه - صرف بنك - قبض خزينه - قبض بنك) 
        public string RecieptType { get; set; }//1-sc-5
        public int Signal { get; set; }// صرف -1  لو قبض 1
        public double Serialize { get; set; }
        public int BranchId { get; set; }
        public int UserId { get; set; }
        public bool IsAccredit { get; set; } = true;
        public int  SubTypeId{ get; set; }=0;
        public int BenefitId { get; set; }
        public bool IsBlock { get; set; }
        public string ReceiptsAction { get; set; }
        public DateTime? LastDate { get; set; }
        public string BrowserName { get; set; }
        [ForeignKey("employeesId")] public int employeesId { get; set; } = 1;
        public int? CollectionMainCode { get; set; } // كود اساسى مشترك ل سند التحصيل باختلاف طرق السداد

        public InvEmployees employees { get; set; }
        public bool isTechnicalSupport { get; set; } = false;
    }
}
