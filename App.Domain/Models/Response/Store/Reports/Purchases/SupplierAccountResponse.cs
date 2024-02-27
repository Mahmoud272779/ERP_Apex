using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response.Reports.Purchases
{
    public class SupplierAccountResponse
    {

        public double ActualDebtorTrans { get; set; }
        public double ActualCreditorTrans { get; set; }
        public double ActualDebtorBalance { get; set; }
        public double ActualCreditorBalance { get; set; }
        public double PeriodDebtorTrans { get; set; }
        public double PeriodCreditorTrans { get; set; }
        public double PeriodDebtorBalance { get; set; }
        public double PeriodCreditorBalance { get; set; }
        public List<SupplierAccountList> SupplierAccountData { get; set; }
    }
    public class SupplierAccountList
    {
        public int Id { get; set; }
        public int docTypeId { get; set; }
        public int pageId { get; set; }
        public int code { get; set; }
        public int parentId { get; set; }
        public int recTypeId { get; set; }
        public double Serialize { get; set; }
        public string Transactiondate { get; set; }
        public string DocumentId { get; set; }
        public string PaperNumber { get; set; }
        public int InvoiceTypeId { get; set; }//enum
        public int collectionSubType { get; set; }//enum
        public string InvoiceTypeAr { get; set; }//enum
        public string InvoiceTypeEn { get; set; }//enum
        public string Notes { get; set; }
        public string rowClassName { get; set; }
        public double DebtorTrans { get; set; }
        public double CreditorTrans { get; set; }
        public double DebtorBalance { get; set; }
        public double CreditorBalance { get; set; }
        public int Signal { get; set; }
        public bool isCustomer { get; set; }
        public DateTime CreationDate { get;set;}
    }
}
