using App.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.Process
{
    public class InvoiceMasterHistory: AuditableEntity
    {

        //Is Branch Id required so as to keep the branch that modified the invoice????
        public int Id { get; set; }//table primary key
        public int InvoiceId { get; set; } //Invoice Id
        public string InvoiceType { get; set; }//Invoice type(purchases,sales,POS,Return Purchase,....)
        public int Code { get; set; }
        public double Serialize { get; set; }
        public string ParentInvoiceCode { get; set; }
        public bool IsDeleted { get; set; }
        public string BookIndex { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int StoreId { get; set; }
        public double TotalPrice { get; set; }
        public string Notes { get; set; }
        public int InvoiceTypeId { get; set; } // enum of invoiceType
        public int SubType { get; set; } // مرتجع كلى او جزئي
        public string LastAction { get; set; }
        public DateTime? LastDate { get; set; }
        public string BrowserName { get; set; }

        [ForeignKey("employeesId")]
        public int employeesId { get; set; } = 1;
        public int? CollectionMainCode { get; set; }
        public int? MultiCollectionReceiptsId { get; set; }
        public InvEmployees employees { get; set; }
    }
}
