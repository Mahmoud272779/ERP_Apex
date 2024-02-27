using App.Domain.Models.Security.Authentication.Response;
using Microsoft.AspNetCore.Http;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class EmployeesRequestDTOs
    {
        public class Add
        {
            public string ArabicName { get; set; }
            public string LatinName { get; set; }
            public int Status { get; set; }
            public string Notes { get; set; }
            public IFormFile Image { get; set; }
            public int[] Branches { get; set; }
            public int JobId { get; set; }
            public int? FinancialAccountId { get; set; }
            public int? SalesPriceId { get; set; }
        }
        public class Update
        {
            public int Id { get; set; }
            public int Code { get; set; }
            public string ArabicName { get; set; }
            public string LatinName { get; set; }
            public int Status { get; set; }
            public string Notes { get; set; }
            public IFormFile? Image { get; set; }
            public int[] Branches { get; set; }
            public int JobId { get; set; }
            public int? FinancialAccountId { get; set; }
            public bool ChangeImage { get; set; }
            public int? SalesPriceId { get; set; }
        }
        public class Search : GeneralPageSizeParameter
        {
            public string Name { get; set; }
            public int Status { get; set; }
            public int[] JobList { get; set; }
            public int[] BranchList { get; set; }
            public int EmployeeId { get; set; }

            /// <summary>
            /// Only For Print 
            /// </summary>
           public string Ids { get; set; }
            public bool IsSearcheData { get; set; } = true;

        }
    }

    

}
