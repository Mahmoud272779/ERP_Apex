using App.Domain.Entities.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace App.Domain.Models.Security.Authentication.Response
{
    public class FinancialAccountDto
        {
        public int Id { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int? HasCostCenter { get; set; }
        public int Status { get; set; }
        public int FA_Nature { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName{ get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public string AccountCode { get; set; }
        public string autoCoding { get; set; } = "";
        public int FinalAccount { get; set; }
        public bool IsMain { get; set; }
        public double? total { get; set; }
        //public List<FinancialAccounChildDto> children { get; set; }
        public int? ParentId { get; set; }
        public bool hasChildren { get; set; }
    }
   
    public class FinancialAccounChildDto
    {
        public int Id { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int? HasCostCenter { get; set; }
        public int Status { get; set; }
        public int FA_Nature { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public string Code { get; set; }
        public int FinalAccount { get; set; }
        public bool IsMain { get; set; }
        public int? ParentId { get; set; }
        public double? total { get; set; }
        public bool hasChildren { get; set; }

    }
    public class FinancialAccountForOpeningBalanceDto
    {
        public int Id { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public string Code { get; set; }
        public string Notes { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
    }
    public class FA_Search
    {
        public int? Status { get; set; }
        public int? FA_Nature { get; set; }
        public int? CostCenter { get; set; }
        public string? SearchCriteria { get; set; }
    }
    public class FinancialAccountById
    {
        public FinancialAccountById()
        {
            costCenters = new List<CostCentersList>();
            financialBranchList = new List<FinancialBranchList>();
        }
        public int nodeId { get; set; }
        public string AccountCode { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public int? ParentId { get; set; }
        public string ParentName { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public int Status { get; set; }
        public int FA_Nature { get; set; }
        public int FinalAccount { get; set; }
        public string Notes { get; set; }
        public bool IsMain { get; set; }
        public bool isHaveChildren { get; set; }
        public bool isHaveOperation { get; set; }
        // public string CostCenters { get; set; }
        public List<CostCentersList> costCenters { get; set; }
        public List<FinancialBranchList> financialBranchList { get; set; }
    }
    public class CostCentersList 
    {
        public int Id { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
    }

    public class FinancialBranchList
    {
        public int Id { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
    }
    public class CostCenterDropDown
    {
        public int Id { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public string Code { get; set; }
        public int? ParentId { get; set; }
    }
    public class FinancialAccountDropDown
    {
        public int Id { get; set; }
        public int fA_Nature { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public string Code { get; set; }
        public string AutoCode { get; set; }

    }
    public class FinancialAccountDropDownReceipts
        {
            public int Id { get; set; }
            public string ArabicName { get; set; }
            public string LatinName { get; set; }
            public string AutoCode { get; set; }
            public string Code { get; set; }
        }
}
