using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace App.Domain.Models.Request.Store.Invoices
{
    public class AccreditInvoiceRequest
    {
        public DateTime date { get; set; }
        public int InvType { get; set; }
        public int userId { get; set; }
        public int safeId { get; set; }
        public bool IsCompined { get; set; } = false;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int sessionId { get; set; } = 0;
        //public IFormFile file { get; set; }
        //public byte[] fileb { get; set; }


    }
    public class UpdateJournalEntryfromAccredit
    {

        public bool IsAccredit { get; set; } = true;

        public int Id { get; set; }
        public string Name { get; set; }
        //public int CurrencyId { get; set; }
        public int BranchId { get; set; }
        public DateTime? FTDate { get; set; }
        //public IFormFile[] ImagePath { get; set; }

        public string? Notes { get; set; }
        public List<GLJournalEntryDetails> journalEntryDetails = new List<GLJournalEntryDetails>();

    }
}
