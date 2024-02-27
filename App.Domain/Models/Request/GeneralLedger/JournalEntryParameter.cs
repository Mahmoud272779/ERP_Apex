using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class JournalEntryParameter
    {
        public JournalEntryParameter()
        {
            JournalEntryDetails = new List<JournalEntryDetail>();
;        }
        [JsonIgnore]
        public bool isAuto { get; set; } = false;
        [JsonIgnore]
        public bool AddWithOutElements { get; set; } = false;
        public string Name { get; set; }
        //public int CurrencyId { get; set; }
        public int BranchId { get; set; }
        public int ReceiptsId { get; set; }
        public int CompinedReceiptCode { get; set; } = 0;
        public int InvoiceId { get; set; }
        public DateTime? FTDate { get; set; }
        public string Notes { get; set; }
        public bool IsAccredit { get; set; } = true;
        public bool IsCompined { get; set; } = false;
        public IFormFile[] AttachedFiles { get; set; }
        public int? DocType { get; set; }
        public List<JournalEntryDetail> JournalEntryDetails { get; set; } 
    }
    public class JournalEntryDetail
    {
        public int id { get; set; }
        public int? FinancialAccountId { get; set; }
        public string FinancialCode { get; set; }
        public string FinancialName { get; set; }
        public int? CostCenterId { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public bool isCostSales { get; set; } = false;
        public int? JournalEntryId { get; set; }
        public int? ReceiptsMainCode { get; set; }

    }
    public class UpdateJournalEntryParameter
    {
        public UpdateJournalEntryParameter()
        {
            journalEntryDetails = new List<JournalEntryDetail>();
        }
        public bool IsAccredit { get; set; } = true;
        [JsonIgnore]
        public bool fromSystem { get; set; } = false;
        public int Id { get; set; }
        public string Name { get; set; }
        //public int CurrencyId { get; set; }
        public int BranchId { get; set; }
        public DateTime? FTDate { get; set; }
        //public IFormFile[] ImagePath { get; set; }

        public List<int> FileIds { get; set; } // ملفات قديمة لن يتم حذفها
        public IFormFile[] AttachedFiles { get; set; }

        public string? Notes { get; set; }
        public List<JournalEntryDetail> journalEntryDetails { get; set; }
    }
    public class UpdateJournalEntryDetail
    {
        public int? FinancialAccountId { get; set; }
        public string FinancialCode { get; set; }
        public string FinancialName { get; set; }
        public int? CostCenterId { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public string DescriptionAr { get; set; } = "";
        public string DescriptionEn { get; set; }
    }
    public class JournalEntryFilesDto
    {
        public int JournalEntryId { get; set; }
        public IFormFile[] ImagePath { get; set; }
    }
    public class BlockJournalEntry
    {
        public int[] Ids{ get; set; }
    }
    public class UpdateTransfer
    {
        public int[] Id { get; set; }
        public int IsTransfer { get; set; }
    }
    public class FileTrans
    {
        public int Id { get; set; }
    }

    public class EntryFunds
    {
        public double Credit { get; set; }
        public double Debit { get; set; }

        //public int JournalEntryId { get; set; }
        //public string FinancialName { get; set; }
        //public string FinancialCode { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public int? FinancialAccountId { get; set; }
        public int? CostCenterId { get; set; }
        [JsonIgnore]
        public int JournalEntryId { get; set; } = -1;
        [JsonIgnore]
        public bool isStoreFund { get; set; } = false;
        [JsonIgnore]
        public int? StoreFundId { get; set; }
        [JsonIgnore]
        public int? DocType { get; set; }


    }
    public class addEntryFunds
    {
        public List<EntryFunds> EntryFunds { get; set; }
        public DateTime date { get; set; }
        public string? note { get; set; }
        [JsonIgnore]
        public bool isFund { get; set; } = false;
    }
}
