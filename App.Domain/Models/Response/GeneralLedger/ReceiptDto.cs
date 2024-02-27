using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class ReceiptsForPaymentVoucherTreasuryDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime Date { get; set; }
        public int Organize { get; set; }
        public int FinancialAccountId { get; set; }
        public string FinancialAccountName { get; set; }
        public int? TreasuryId { get; set; }
        public string TreasuryName { get; set; }
        public int PaymentWays { get; set; }
        public double PaymentNumber { get; set; }
        public bool IsIncludedTaxes { get; set; }
        public bool IsBlock { get; set; }
    }
    public class ReceiptsForPaymentVoucherBankDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime Date { get; set; }
        public int Organize { get; set; }
        public int FinancialAccountId { get; set; }
        public string FinancialAccountName { get; set; }
        public int? BankId { get; set; }
        public string BankName { get; set; }
        public int PaymentWays { get; set; }
        public bool IsBlock { get; set; }
        public bool IsIncludedTaxes { get; set; }
        public double PaymentNumber { get; set; }

    }
    public class ReceiptSearchSafeParameter
    {
        public string Code { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int Organize { get; set; }
        public int FinancialAccountId { get; set; }
        public int TreasuryId { get; set; }
        public int PaymentWays { get; set; }
    }

    public class ReceiptSafePages
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public ReceiptSearchSafeParameter Search { get; set; } = new ReceiptSearchSafeParameter();

    }
    public class ReceiptSearchBankParameter
    {
        public string Code { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int FinancialAccountId { get; set; }
        public int BankId { get; set; }
        public int PaymentWays { get; set; }
        public int Organize { get; set; }
    }

    public class ReceiptBankPages
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public ReceiptSearchBankParameter Search { get; set; } = new ReceiptSearchBankParameter();

    }
    public class SupportsDto
    {
        public SupportsDto()
        {
            costs = new List<SupportsCostListDto>();
            Files = new List<SupportFilesListDto>();
        }
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime? Date { get; set; }
        public string SerialNote { get; set; }
        public int? TreasuryId { get; set; }
        public string TreasuryName{ get; set; }
        public bool IsBank { get; set; }
        public int Organize { get; set; }

        public int? FinancialAccountId { get; set; }
        public string FinancialAccountName { get; set; }
        public string FinancialAccountCode { get; set; }
        public double? PaymentNumber { get; set; }
        public int? BankId { get; set; }
        public string chequeNumber { get; set; }
        public string BankName { get; set; }
        public DateTime? BankDate { get; set; }
        public int? PaymentWays { get; set; }
        public string Notes { get; set; }
        public double? TotalPercentage { get; set; }
        public string BankCheque { get; set; }
        public double? TotalNumber { get; set; }
        public bool IsIncludedTaxes { get; set; }
        public string BenefitName { get; set; }
        public string BenefitNumber { get; set; }
        public List<SupportsCostListDto> costs { get; set; }
        public List<SupportFilesListDto> Files { get; set; }
    }
    public class recieptDataViewDTO
    {
        public TotalFinancial Total{get; set ;}
        public SupportsDto recietsDto { get; set ; }
    }
    public class SupportsCostListDto
    {
        public int Id { get; set; }
        public int CostCenterId { get; set; }
        public int SupportId { get; set; }
        public string CostCenteCode { get; set; }
        public string CostCenteName { get; set; }
        public double Percentage { get; set; }
        public double Number { get; set; }
    }
    public class SupportFilesListDto
    {
        public int Id { get; set; }
        public int SupportId { get; set; }
        public string File { get; set; }
    }

    public class TotalFinancial 
    {
        public double Total { get; set; }
        public int  Type { get; set; }
        public string CurrancyName { get; set; }
    }
}

