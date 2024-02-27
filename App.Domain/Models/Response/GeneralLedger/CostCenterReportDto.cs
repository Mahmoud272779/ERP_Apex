using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class CostCenterReportDto
    {
        public CostCenterReportDto()
        {
            costCenterReportChildDtos = new List<CostCenterReportChildDto>();
            costCenterDetails = new List<CostCenterDetails>();
        }
        public int Id { get; set; }
        public string CostCenterName { get; set; }
        public double TotalsCredit { get; set; }
        public double TotalsDebit { get; set; }
        public double TotalsBalanceDebit { get; set; }
        public double TotalsBalanceCredit { get; set; }
        public List<CostCenterDetails> costCenterDetails { get; set; }
        public List<CostCenterReportChildDto> costCenterReportChildDtos { get; set; }
    }
    public class CostCenterReportChildDto
    {
        public CostCenterReportChildDto()
        {
            childes = new List<CostCenterReportChildDto>();
            costCenterDetails = new List<CostCenterDetails>();
        }
        public int Id { get; set; }
        public string CostCenterName { get; set; }
        public double TotalsCredit { get; set; }
        public double TotalsDebit { get; set; }
        public double TotalsBalanceDebit { get; set; }
        public double TotalsBalanceCredit { get; set; }
        public List<CostCenterDetails> costCenterDetails { get; set; }
        public List<CostCenterReportChildDto> childes { get; set; }
    }
    public class CostCenterDetails
    {
        public int JournalCode { get; set; }
        public DateTime? JournalDate { get; set; }
        public string Notes { get; set; }
        public double ProcessDebit { get; set; }
        public double ProcessCredit { get; set; }
        public double BalanceDebit { get; set; }
        public double BalanceCredit { get; set; }

    }
    public class PageParameterCostCenterReport
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public CostCenterReportSearchParameter Search { get; set; } = new CostCenterReportSearchParameter();

    }
    public class CostCenterReportSearchParameter
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int CostCenterId { get; set; }
    }
    public class CostCenterReportReportData
    {
        public string FinantialAccNameAr { get; set; }
        public string FinantialAccNameEn { get; set; }
        public DateTime? Date { get; set; }
        public string Note { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public int JournalEntryId { get; set; }
        public int JournalEntryCode { get; set; }
    }
    public class FinalCostCenterReportReportData
    {

        public string FinantialAccNameAr { get; set; }
        public string FinantialAccNameEn { get; set; }
        public DateTime? Date { get; set; }
        public string Note { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public double periodCredit { get; set; }
        public double periodDebit { get; set; }
        public double balanceCredit { get; set; }
        public double balanceDebit { get; set; }
        public int JournalEntryId { get; set; }
        public int JournalEntryCode { get; set; }

        //for print
        public string JournalDate { get; set; }
        public string periodsDebit { get; set; }
        public string balancesDebit { get; set; }
       
    }
    public class FinalCostCenterReportReportAllData
    {
        public List<FinalCostCenterReportReportData> FinalCostCenterReportReportDataList;
        public double TotalperiodCredit { get; set; }
        public double TotalperiodDebit { get; set; }

        public double TotalbalanceCredit { get; set; }
        public double TotalbalanceDebit { get; set; }
        public double  PreviousBalance { get; set; }


        //for print
        //public string TotalBalancesDebit { get; set; }
        //public string TotalBeriodsDebit { get; set; }
        public string PrevBalance { get; set; }
        public int Count { get; set; } = 1;

        
    }
}
