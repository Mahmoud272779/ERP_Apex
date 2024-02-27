using App.Domain.Entities.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class JournalEntryDto
    {
        public JournalEntryDto()
        {
            JournalEntryDetailsDtos = new List<JournalEntryDetailsDto>();
        }
        public int Id { get; set; }
        public int? Code { get; set; }
        public string Name { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public int? BranchId { get; set; }
        public string BranchName { get; set; }
        public double CreditTotal { get; set; }
        public double DebitTotal { get; set; }
        public DateTime? FTDate { get; set; }
        public string Notes { get; set; }
        public bool IsBlock { get; set; }
        public bool IsTransfer { get; set; }
        public bool IsDraft { get; set; }
        public bool Auto { get; set; }

        public List<JournalEntryDetailsDto> JournalEntryDetailsDtos { get; set; }
    }
    public class JournalEntryDetailsDto
    {
        public int Id { get; set; }
        public int JournalEntryId { get; set; }
        public int? FinancialAccountId { get; set; }
        public string FinancialAccountCode { get; set; }
        public string FinancialAccountName { get; set; }
        public int? CostCenterId { get; set; }
        public string CostCenterName { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }

    }
    public class JournalEntriesFilesDto
    {
        public int Id { get; set; }
        public int JournalEntryId { get; set; }
        public string File { get; set; }
        public string FileName { get; set; }
    }
    public class PageParameterJournalEntries
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public JournalEntriesSearchParameter Search { get; set; } = new JournalEntriesSearchParameter();

    }
    public class JournalEntriesSearchParameter
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? Code { get; set; }
        public int? IsTransfer { get; set; }
        public string? branches { get; set; }
        public journalEntryTypes type { get; set; } = journalEntryTypes.all;
    }
    public class JournalEntryById
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public int CurrencyId { get; set; }
        public double CreditTotal { get; set; }
        public double DebitTotal { get; set; }
        public DateTime? FTDate { get; set; }
        public string Notes { get; set; }
        public bool IsBlock { get; set; }
    }
    public class GetJournalEntryByID
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public string BranchNameEn { get; set; }

        public double CreditTotal { get; set; }
        public double DebitTotal { get; set; }
        public bool Auto { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public DateTime? FTDate { get; set; }
        public bool IsBlock { get; set; }
        public int Code { get; set; }
        public string Notes { get; set; }

        //for print
        public int GroupId { get; set; }
        public string Date { get; set; }
        public List<GetJournalEntryDetailsByID> JournalEntryDetailsDtos { get; set; }
    }
    public class GetJournalEntryDetailsByID
    {
        public int Id { get; set; }
        public int JournalEntryId { get; set; }
        public int? CostCenterId { get; set; }
        public int? FinancialAccountId { get; set; }
        public string financialAccountNameAr { get; set; }
        public string financialAccountNameEn { get; set; }
        public string FinancialAccountCode { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string CostCenterName { get; set; }
        public string CostCenterNameEn { get; set; }
        public bool isStoreFund { get; set; }
        public int GroupId { get; set; }

    }
    public class getAllJournalEntryPrepering
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public double CreditTotal { get; set; }
        public double DebitTotal { get; set; }
        public int CurrencyId { get; set; }
        public string FTDate { get; set; }
        public string Notes { get; set; }
        public int BranchId { get; set; }
        public string BranchNameAr { get; set; }
        public string BranchNameEn { get; set; }
        public bool IsBlock { get; set; }
        public bool IsDraft { get; set; }
        public bool IsTransfer { get; set; }
        public bool Auto { get; set; }
        public bool canDelete { get; set; }
        public IEnumerable<journalEntryDetailsDtosResponse> journalEntryDetailsDtos { get; set; }
    }
    public class journalEntryDetailsDtosResponse
    {
        public int Id { get; set; }
        public int? FinancialAccountId { get; set; }
        public int JournalEntryId { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public int? CostCenterId { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public string financialAccountNameAr { get; set; }
        public string financialAccountNameEn { get; set; }
        public string financialAccountCode { get; set; }
        public string CostCenterName { get; set; }
        public string CostCenterNameEn { get; set; }
    }
    public class getAllJournalEntryResponse
    {
        public IEnumerable<getAllJournalEntryPrepering> data { get; set; }
        public IQueryable<GLJournalEntry> journalEntry { get; set; }
    }
}