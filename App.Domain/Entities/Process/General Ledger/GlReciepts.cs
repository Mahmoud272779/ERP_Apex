using App.Domain.Entities.Process;
using App.Domain.Entities.Process.General_Ledger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities
{
    public  class GlReciepts
    {
        public int Id { get; set; }
        public int? SafeID { get; set; }
        public int? BankId { get; set; }
        public int? EntryFundId { get; set; }
        public int Code { get; set; }
        public string PaperNumber { get; set; }
        public DateTime RecieptDate { get; set; }
        public string Notes { get; set; }
        public string InvoiceNotes { get; set; }
        public double  Amount { get; set; } // المبلغ
        public int Authority { get; set; } //  الجهه
        public int PaymentMethodId { get; set; } // طريقة السداد
        public string ChequeNumber { get; set; } // رقم الشيك
        public string ChequeBankName { get; set; } // اسم بنك الشيك
        public DateTime ChequeDate { get; set; } // تاريخ الشيك
        public int RecieptTypeId { get; set; } //نوع السند (صرف خزينه - صرف بنك - قبض خزينه - قبض بنك) 
        public string RecieptType { get; set; }//1-sc-5
        public int Signal { get; set; }// صرف -1  لو قبض 1
        public int? ParentId { get; set; } // كود الفاتورة او خصم مكتسب او مسموح به اللي معمول من خلاله السند

        public int? ParentTypeId { get; set; }// نوع الفاتورة او الخصم
        public double Serialize { get; set; }
        public int BranchId { get; set; }
        public int UserId { get; set; }
        public bool IsAccredit { get; set; } = true;
        public bool IsCompined { get; set; } = false;
        public int? CompinedParentId { get; set; } 
        public string NoteAR { get; set; }// نوع الفاتوره او السند
        public string NoteEN { get; set; }//البيان
        public double Creditor { get; set; }
        public double Debtor { get; set; }
        public DateTime CreationDate { get; set; }
        public int otherSalesManId { get; set; }
        public int? FinancialAccountId { get; set; }
        public bool IsIncludeVat { get; set; }
        public int BenefitId { get; set; }// benefit id  for all authority
        public int? PersonId { get; set; }//benefit id  for persons only 
        public int? SalesManId { get; set; }//benefit id  for salesman only 
        public int? OtherAuthorityId { get; set; }//benefit id  for OtherAuthorityId only       الريط فقط مع جدول الجهات الاخرى
        public bool IsBlock { get; set; }
        public int SubTypeId { get; set; } // سند تحصيل= 8  او سند سداد= 9 
        public int CollectionCode { get; set; } // كود خاص ب سند التحصيل
        public int? CollectionMainCode { get; set; } // كود اساسى مشترك ل سند التحصيل باختلاف طرق السداد
        public int? MultiCollectionReceiptParentId { get; set; }
        public virtual GLFinancialAccount FinancialAccount { get; set; }
        public virtual GLBank Banks { get; set; }
        public virtual GLSafe Safes { get; set; }
        public virtual InvPaymentMethods PaymentMethods { get; set; }
        public virtual IReadOnlyCollection<GLRecieptFiles> RecieptsFiles { get; set; }
        public virtual IReadOnlyCollection<GLRecieptCostCenter> RecieptsCostCenters { get; set; }
        public virtual InvPersons person { get; set; }
        public virtual GLOtherAuthorities OtherAuthorities { get; set; }
        public virtual InvSalesMan SalesMan { get; set; }
        public  bool Deferre { get; set; }
        public  string RectypeWithPayment { get; set; }
        public  int? isPartialPaid { get; set; }

    }
}
