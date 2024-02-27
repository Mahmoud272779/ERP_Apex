using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class TreasuryDto
    {
        public int Id { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public int  Code { get; set; }

    }
    public class TreasurySettingDto
    {
        public int Id { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public string Code { get; set; }
        public int? FinancialAccountId { get; set; }
        public string FinancialAccountCode { get; set; }
        public string FinancialAccountName { get; set; }
    }
    public class AllTreasuryDto
    {
        public int Id { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public string Code { get; set; }
        public int BranchId { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
        public int? FinancialAccountId { get; set; }
        public string FinancialName { get; set; }
        public string BranchName { get; set; }
    }
}
