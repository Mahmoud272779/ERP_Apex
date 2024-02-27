using System;
using System.Collections.Generic;
using App.Domain.Models.Security.Authentication.Response;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class DiscountRequest
    {
        public string PaperNumber { get; set; }
        public DateTime DocDate { get; set; }
        public bool IsCustomer { get; set; }
        public int Person { get; set; }
        public double amountMoney { get; set; }
        public string Notes { get; set; }
        public int DocType { get; set; }
        
        public int BranchId { get; set; }
    }


    public class journalDetailsDTO
    {
        public List<JournalEntryDetail> journalEntryDetail { get; set; }
        public string notes { get; set; }

    }
    public class DiscountSearch : GeneralPageSizeParameter
    {
        public int DiscountId { get; set; }
        public string  CodeOrPaperNumber { get; set; }
        public int DocType { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class UpdateDiscountRequest
    {
        public int Id { get; set; }
        public string DocNumber { get; set; }
        public string PaperNumber { get; set; }
        public DateTime DocDate { get; set; }
        public bool IsCustomer { get; set; }
        public int Person { get; set; }
        public double amountMoney { get; set; }
        public string Notes { get; set; }
        public double Creditor { get; set; }
        public double Debtor { get; set; }
        public int DocType { get; set; }
        public string Refrience { get; set; }
    }
}
