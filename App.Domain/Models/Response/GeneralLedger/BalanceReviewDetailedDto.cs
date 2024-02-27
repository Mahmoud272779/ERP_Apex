using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class TotalsDetailed
    {
        public TotalsDetailed()
        {
            balanceReviewDtos = new List<BalanceReviewDetailedDto>();
        }
        public double TotalBalanceBalanceBetweenPeriodCredit { get; set; }
        public double TotalBalanceBalanceBetweenPeriodDebit { get; set; }
        public double TotalBalanceBeforePeriodCredit { get; set; }
        public double TotalBalanceBeforePeriodDebit { get; set; }
        public double TotalsBalanceCredit { get; set; }
        public double TotalsBalanceDebit { get; set; }
        public List<BalanceReviewDetailedDto> balanceReviewDtos { get; set; }

    }
    public class BalanceReviewDetailedDto
    {
        public BalanceReviewDetailedDto()
        {
            balanceReviewPeriods = new List<BalanceReviewPeriods>();
            balanceReviewChildsDtos = new List<BalanceReviewChildsDetailedDto>();
        }
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string FinancialAccountCode { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public string Notes { get; set; }
        public double BalanceBalanceBetweenPeriodCredit { get; set; }
        public double BalanceBalanceBetweenPeriodDebit { get; set; }
        public double BalanceBeforePeriodCredit { get; set; }
        public double BalanceBeforePeriodDebit { get; set; }
        public double TotalBalanceCredit { get; set; }
        public double TotalBalanceDebit { get; set; }
        public List<BalanceReviewPeriods> balanceReviewPeriods { get; set; }
        public List<BalanceReviewChildsDetailedDto> balanceReviewChildsDtos { get; set; }
    }
    public class BalanceReviewChildsDetailedDto
    {
        public BalanceReviewChildsDetailedDto()
        {
            Childes = new List<BalanceReviewChildsDetailedDto>();
            balanceReviewPeriods = new List<BalanceReviewPeriods>();
        }
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string FinancialAccountCode { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public string Notes { get; set; }
        public double BalanceBetweenPeriodCredit { get; set; }
        public double BalanceBalanceBetweenPeriodDebit { get; set; }
        public double BalanceBeforePeriodCredit { get; set; }
        public double BalanceBeforePeriodDebit { get; set; }
        public double TotalBalanceCredit { get; set; }
        public double TotalBalanceDebit { get; set; }
        public List<BalanceReviewPeriods> balanceReviewPeriods { get; set; }
        public List<BalanceReviewChildsDetailedDto> Childes { get; set; }
    }
    public class BalanceReviewPeriods
    {
        public int  MonthName { get; set; }
        public double BalanceMonthPeriodCredit { get; set; }
        public double BalanceMonthPeriodDebit { get; set; }
    }
    public class BalanceReviewDetailedParameter
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public BalanceReviewDetailedSearchParameter Search { get; set; } = new BalanceReviewDetailedSearchParameter();
    }
    public class BalanceReviewDetailedSearchParameter
    {
        public int FinancialAcountId { get; set; }
        public int? TypeOfPeriod { get; set; }
        public int? Monthes { get; set; }
        public DateTime? Year { get; set; }
    }
  
}
