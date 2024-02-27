using App.Domain.Entities.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class BankResponsesDTOs
    {
        public class GetAll
        {
            public int Id { get; set; }
            public string LatinName { get; set; }
            public string ArabicName { get; set; }
            public int Code { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public int Status { get; set; }
            public string AddressAr { get; set; }
            public string AddressEn { get; set; }
            public string Website { get; set; }
            public List<int> BranchesId { get; set; }
            public string AccountNumber { get; set; }
            public string Notes { get; set; }
            public int? FinancialAccountId { get; set; }
            public string FinancialAccountCode { get; set; }
            public string FinancialName { get; set; }
            public string BranchNameAr { get; set; }
            public string BranchNameEn { get; set; }
            public bool CanDelete { get; set; }
        }
        public class BankSettingDto
        {
            public int Id { get; set; }
            public string ArabicName { get; set; }
            public string LatinName { get; set; }
            public string Code { get; set; }
            public int? FinancialAccountId { get; set; }
            public string FinancialAccountCode { get; set; }
            public string FinancialAccountName { get; set; }
        }
    }
        
    
    
    
}
