using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class FinancialCostParameter
    {
        public int FinancialAccountId { get; set; }
        public int[] CostCenterId { get; set; }
    }
    public class FinancialForCostParameter
    {
        public int []FinancialAccountId { get; set; }
        public int CostCenterId { get; set; }
    }
    public class FinancialCostsParameter
    {
        public int []FinancialAccountId { get; set; }
        public int CostCenterId { get; set; }
    }
}
