using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Response.Store
{
    public class getListOfSalesmanResponse
    {
        public bool ApplyToCommissions { get; set; }
        public string ArabicName { get; set; }
        public string BranchNameAr { get; set; }
        public string BranchNameEn { get; set; }
        public int[] Branches { get; set; }
        public bool CanDelete { get; set; }
        public int Code { get; set; }
        public int? CommissionListId { get; set; }
        public string CommissionListNameAr { get; set; }
        public string CommissionListNameEn { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
        public string LatinName { get; set; }
        public string Notes { get; set; }
        public string Phone { get; set; }
        public FinancialAccount FinancialAccountId { get; set; }
        public int? SalesPriceId { get; set; }  

    }
    public class FinancialAccount
    {
        public int Id { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
    }
    public class listOfSalesmanResponse
    {
        public List<getListOfSalesmanResponse> getListOfSalesmanResponses { get; set; }
        public int DataCount { get; set; }
        public int TotalCount { get; set; }
        public bool isDataExist { get; set; }
    }
}
