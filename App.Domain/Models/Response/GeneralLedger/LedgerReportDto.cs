using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class LedgerReportDto
    {
        public LedgerReportDto()
        {
            ledgerReportDetailsDtos = new List<LedgerReportDetailsDto>();
        }
        public string FinancialAccountName { get; set; }
        public string CurrencyName { get; set; }
        public List<LedgerReportDetailsDto>  ledgerReportDetailsDtos { get; set; }

    }
    public class LedgerReportDetailsDto
    {
        public LedgerReportDetailsDto()
        {
            ledgerReportDetailsJournalEntryDtos = new List<LedgerReportDetailsJournalEntryDto>();
        }
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Code { get; set; }
        public string FinancialAccountName { get; set; }
        public string CurrencyName { get; set; }
        public double OpeningBalance { get; set; }
        public int OpeningBalanceType { get; set; }
        public double TotalDebitProcess { get; set; }
        public double TotalCridetProcess { get; set; }
        public double TotalDebitBalance { get; set; }
        public double TotalCridetBalance { get; set; }
        public List<LedgerReportDetailsJournalEntryDto> ledgerReportDetailsJournalEntryDtos { get; set; }

    }
    public class LedgerReportDetailsJournalEntryDto
    {
        public DateTime? JournalEntryDate { get; set; }
        public string JournalEntryCode { get; set; }
        public string Note { get; set; }
        public double DebitProcess { get; set; }
        public double CridetProcess { get; set; }
        public double DebitBalance { get; set; }
        public double CridetBalance { get; set; }
    }
}
