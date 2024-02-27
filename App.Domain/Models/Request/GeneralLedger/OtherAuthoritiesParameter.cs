using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class OtherAuthoritiesParameter
    {
        public string LatinName { get; set; }
        [Required]
        public string ArabicName { get; set; }
        [Required]
        public int BranchId { get; set; } = 1;
        public int Status { get; set; }
        public string Notes { get; set; }
        public int? FinancialAccountId { get; set; }
        public int userId { get; set; } = 1;
    }
    public class UpdateOtherAuthoritiesParameter
    {
        public int Id { get; set; }
        public string LatinName { get; set; }
        [Required]
        public string ArabicName { get; set; }
        [Required]
        public int BranchId { get; set; } = 1;
        public bool CanDelete { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
        public int? FinancialAccountId { get; set; }
        public int userId { get; set; } = 1;
    }
    public class PageOtherAuthoritiesParameter:GeneralPageSizeParameter
    {      
        public string Name { get; set; }
        public int? Status { get; set; } = 0;
        public string SearchCriteria { get; set; }

    }
    public class OtherAuthoritiesSearch : GeneralPageSizeParameter
    {
        public int Status { get; set; }
    }
    
    public class ListOfOtherAuthorities
    {
        public string Name { get; set; }
    }
    public class DropDownParameter
    {
        public int? code { get; set; }
        public string? SearchCriteria { get; set; }
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
    }
}
