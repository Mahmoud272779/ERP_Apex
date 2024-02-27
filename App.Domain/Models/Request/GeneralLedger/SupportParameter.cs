using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class SupportParameter
    {
        public SupportParameter()
        {
            costCenterSupports = new List<CostCenterSupport>();
        }
        public bool IsCashReceipts { get; set; } //سند قبض
        public bool IsPaymentVoucher { get; set; }//سند صرف
        public DateTime Date { get; set; }
        public bool IsBank { get; set; }
        public int? BankId { get; set; }
        public bool IsTreasury { get; set; } //خزينه
        public int? TreasuryId { get; set; }
        public string serialNote { get; set; }
        public int Organize { get; set; }
        public int FinancialAccountId { get; set; }
        public string BenefitNumber { get; set; }
        public string BenefitName { get; set; }
        public double PaymentNumber { get; set; }
        public int PaymentWays { get; set; }
        public string chequeNumber { get; set; }
        public string BankName { get; set; }
        public DateTime? BankDate { get; set; }
        public string Notes { get; set; }
        public bool IsIncludedTaxes { get; set; }
        public IFormFile[] ImagePath { get; set; }
        public string MainInvoiceCode { get; set; }
        public double Creditor { get; set; }//دائن
        public double Debtor { get; set; }//مدين
        public int InvoiceTypeId { get; set; }// نوع الفاتورة
        public int Signal { get; set; }
        public double Serialize { get; set; }
        public string Code { get; set; }
        public int BranchId { get; set; }
        //public IFormFile ImagePath { get; set; }
        public List<CostCenterSupport> costCenterSupports { get; set; }
    }
    public class CostCenterSupport
    {
        public int CostCenterId { get; set; }
        public string CostCenteCode { get; set; }
        public string CostCenteName { get; set; }
        public double Percentage { get; set; }
        public double Number { get; set; }
    }
    public class UpdateSupportParameter 
    {
        public int Id { get; set; }
        public bool IsCashReceipts { get; set; } //سند قبض
        public bool IsPaymentVoucher { get; set; }//سند صرف
        public DateTime Date { get; set; }
        public bool IsBank { get; set; }
        public int BankId { get; set; }
        public bool IsTreasury { get; set; } //خزينه
        public int? TreasuryId { get; set; }
        public string serialNote { get; set; }
        public int Organize { get; set; }
        public int FinancialAccountId { get; set; }
        public string BenefitNumber { get; set; }
        public string BenefitName { get; set; }
        public double PaymentNumber { get; set; }

        public int PaymentWays { get; set; }
        public string chequeNumber { get; set; }
        public string BankCheque { get; set; }
        public DateTime BankDate { get; set; }
        public string Notes { get; set; }
        public bool IsIncludedTaxes { get; set; }
        public IFormFile[] ImagePath { get; set; }
        public string[] ImageLinks { get; set; }
        //public IFormFile ImagePath { get; set; }
        public List<CostCenterSupport> costCenterSupports { get; set; }
    }
}
