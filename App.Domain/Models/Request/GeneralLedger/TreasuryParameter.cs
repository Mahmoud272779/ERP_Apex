using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class TreasuryParameter
    {
        public string LatinName { get; set; }
        [Required]
        public string ArabicName { get; set; }
        [Required]
        public int BranchId { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
        public int? FinancialAccountId { get; set; }
    }
    public class UpdateTreasuryParameter
    {
        public int Id { get; set; }
        public string LatinName { get; set; }
        [Required]
        public string ArabicName { get; set; }
        [Required]
        public int BranchId { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
        public int? FinancialAccountId { get; set; }
    }
    public class PageTreasuryParameter
    {

        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
    }
    public class TreasurySearch
    {
        public int Status { get; set; }
    }
    
    public class ListOfTreasury
    {
        public string Name { get; set; }
    }

}
