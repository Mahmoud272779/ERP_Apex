using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class FinancialSettingParameter
    {
        public bool IsAssumption { get; set; }
        public int FinancialAccountId { get; set; }
        public bool UseFinancialAccount { get; set; }
        public bool AddUnderFinancialAccount { get; set; }
        public bool IsBanks { get; set; }
        public bool IsTreasuries { get; set; }
        public bool IsCustomers { get; set; }
        public bool IsSuppliers { get; set; }
        public bool IsSalesRepresentatives { get; set; }
    }
    public class UpdateFinancialSettingParameter
    {
        public int Id { get; set; }
        public bool IsAssumption { get; set; }
        public int FinancialAccountId { get; set; } 
        public bool UseFinancialAccount { get; set; }
        public bool AddUnderFinancialAccount { get; set; }
        public bool IsBanks { get; set; }
        public bool IsTreasuries { get; set; }
        public bool IsCustomers { get; set; }
        public bool IsSuppliers { get; set; }
        public bool IsSalesRepresentatives { get; set; }
    }
}
