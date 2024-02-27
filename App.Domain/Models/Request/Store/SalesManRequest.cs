using System.Collections.Generic;
using App.Domain.Models.Security.Authentication.Response;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class SalesManRequest
    {
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool ApplyToCommissions { get; set; }
        public int? CommissionListId { get; set; }
        public int? FinancialAccountId { get; set; }
        public int[] Branches { get; set; }
        public string Notes { get; set; }
        public int? SalesPriceId { get; set; }


    }



    public class UpdateSalesManRequest
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool ApplyToCommissions { get; set; }
        public int? CommissionListId { get; set; }
        public int? FinancialAccountId { get; set; }
        public int[] Branches { get; set; }
        public string Notes { get; set; }
        public int? SalesPriceId { get; set; }

    }

    public class SalesManSearch : GeneralPageSizeParameter
    {
        public string  Name { get; set; }

        public string BranchList { get; set; } = "0";

    }
   
}
