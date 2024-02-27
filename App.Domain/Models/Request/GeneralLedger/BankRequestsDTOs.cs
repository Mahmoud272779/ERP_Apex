using System.ComponentModel.DataAnnotations;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class BankRequestsDTOs
    {
        public class Add
        {
            public string LatinName { get; set; }
            [Required]
            public string ArabicName { get; set; }

            public string Phone { get; set; }
            public string Email { get; set; }
            public int[]? BranchesId { get; set; }
            public string AccountNumber { get; set; }
            public int? FinancialAccountId { get; set; }
            public int Status { get; set; }
            public string AddressAr { get; set; }
            public string AddressEn { get; set; }
            public string Notes { get; set; }
            public string Website { get; set; }
        }
        public class Update
        {
            public int Id { get; set; }
            public string LatinName { get; set; }
            [Required]
            public string ArabicName { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public int[]? BranchesId { get; set; }
            public string AccountNumber { get; set; }
            public int? FinancialAccountId { get; set; }
            public int Status { get; set; }
            public string AddressAr { get; set; }
            public string AddressEn { get; set; }
            public string Notes { get; set; }
            public string Website { get; set; }
        }
        public class Search
        {
            public int PageSize { get; set; }
            public int PageNumber { get; set; }
            public string SearchCriteria { get; set; }
            public int Status { get; set; }
        }

    }
    
    public class ListOfBanks
    {
        public string Name { get; set; }
    }
}