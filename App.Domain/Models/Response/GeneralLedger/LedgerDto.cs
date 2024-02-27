using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class LedgerDto
    {
        public LedgerDto()
        {
            children = new List<LedgerReportChildDto>();
            LedgerDetails = new List<LedgerDetails>();
        }
        public int Id { get; set; }
        public string CostCenterName { get; set; }
        public double TotalsCredit { get; set; }
        public double TotalsDebit { get; set; }
        public double TotalsBalanceDebit { get; set; }
        public double TotalsBalanceCredit { get; set; }
        public List<LedgerDetails> LedgerDetails { get; set; }
        public List<LedgerReportChildDto> children { get; set; }
    }
    public class LedgerReportChildDto
    {
        public LedgerReportChildDto()
        {
            children = new List<LedgerReportChildDto>();
            LedgerDetails = new List<LedgerDetails>();
        }
        public int Id { get; set; }
        public string CostCenterName { get; set; }
        public double TotalsCredit { get; set; }
        public double TotalsDebit { get; set; }
        public double TotalsBalanceDebit { get; set; }
        public double TotalsBalanceCredit { get; set; }
        public List<LedgerDetails> LedgerDetails { get; set; }
        public List<LedgerReportChildDto> children { get; set; }
    }
    public class LedgerDetails
    {
        public int JournalCode { get; set; }
        public DateTime? JournalDate { get; set; }
        public string Notes { get; set; }
        public double ProcessDebit { get; set; }
        public double ProcessCredit { get; set; }
        public double BalanceDebit { get; set; }
        public double BalanceCredit { get; set; }

    }
    public class PageParameterLedgerReport
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int AccountId { get; set; } 
        public  int DecumentId { get; set; }
    }
   
}
