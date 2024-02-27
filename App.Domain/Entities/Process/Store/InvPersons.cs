using App.Domain.Common;
using App.Domain.Entities.POS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{ 
    public class InvPersons
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }

        public string? LatinName { get; set; }
        public int Type { get; set; } //نوع المورد
        public int Status { get; set; }// نشط 
        public int? SalesManId { get; set; }//مندوب للعميل 
        public InvSalesMan SalesMan { get; set; }
        public string ResponsibleAr { get; set; }// اسم المسول للعملا
        public string ResponsibleEn { get; set; }

        public string? Phone { get; set; }
        public string? Fax { get; set; }
        //[RegularExpression("^[A-Za-z0-9](([_\\.\\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\\.\\-]?[a-zA-Z0-9]+)*)\\.([A-Za-z]{2,})$")]
        [EmailAddress]
        public string? Email { get; set; } // should be available email not fake "Nooote"
        public string? TaxNumber { get; set; }
        public string? AddressAr { get; set; }
        public string? AddressEn { get; set; }
        public double? CreditLimit { get; set; }
        public int? CreditPeriod { get; set; }
        public double? DiscountRatio { get; set; }
        public int? SalesPriceId { get; set; }
        public int? InvEmployeesId { get; set; } = 1;
        public int? LessSalesPriceId { get; set; }
        public bool IsCustomer { get; set; }// عميل 
        public bool IsSupplier { get; set; }// مورد 
        public bool CanDelete { get; set; }
        public bool AddToAnotherList { get; set; }
        public DateTime UTime { get; set; } 
        public string CodeT { get; set; }//S ==1
        public virtual ICollection<InvPersons_Branches> PersonBranch { get; set; }
        public virtual ICollection<InvDiscount_A_P> Discount_A_P { get; set; }
        public virtual InvFundsCustomerSupplier FundsCustomerSuppliers { get; set; }
        public virtual ICollection<InvoiceMaster> InvoiceMaster { get; set; }
        public virtual ICollection<POSInvoiceSuspension> POSInvoiceSuspension { get; set; }
        public virtual ICollection<GlReciepts> reciept { get; set; }

        //Data in case of customers
        [StringLength(70)]
        public string? BuildingNumber { get; set; }
        [StringLength(200)]
        public string? StreetName { get; set; }
        [StringLength(70)]
        public string? Neighborhood { get; set; }// الحي
        [StringLength(70)]
        public string? City { get; set; }
        [StringLength(70)]
        public string? Country { get; set; }
        [StringLength(70)]
        public string? PostalNumber { get; set; }
        //relation to FinancialAccount
        public int? FinancialAccountId { get; set; }
        public virtual GLFinancialAccount FinancialAccount { get; set; }
        public InvEmployees InvEmployees { get; set; }
        public virtual ICollection<OfferPriceMaster> OfferPriceMaster { get; set; }

    }
}
 