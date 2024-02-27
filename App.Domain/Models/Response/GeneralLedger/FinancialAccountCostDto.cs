using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class FinancialAccountCostDto
    {
        public FinancialAccountCostDto()
        {
            costCenterList = new List<CostCenterList>();
        }
        public int FinancialAccountId { get; set; }
        public string FInancialAccountName { get; set; }
        public List<CostCenterList> costCenterList { get; set; }
    }
     public class CostCenterList
     {
        public int CostCenterId { get; set; }
        public string CostCenterName { get; set; }
    }
    public class FinancialAccountForCostDto
    {
        public FinancialAccountForCostDto()
        {
            financialAccountList = new List<FinancialAccountList>();
        }
        public int CostCenterId { get; set; }
        public string CostCenterName { get; set; }
        public List<FinancialAccountList> financialAccountList { get; set; }
    }
    public class FinancialAccountList
    {
        public int FinancialAccountId { get; set; }
        public string FInancialAccountCode { get; set; }
        public string FInancialAccountName { get; set; }
        public int FA_Nature { get; set; }
        public bool Checked { get; set; }

    }
    public class FinancialAccountNotFoundList
    {
        public int FinancialAccountId { get; set; }
        public string FInancialAccountCode { get; set; }
        public string FInancialAccountName { get; set; }
        public int FA_Nature { get; set; }

    }
}
