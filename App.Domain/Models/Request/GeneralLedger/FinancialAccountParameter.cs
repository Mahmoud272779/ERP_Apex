using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class FinancialAccountParameter
    { 
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public string AccountCode { get; set; }
        public int CurrencyId { get; set; }
        public int Status { get; set; }
        public int FA_Nature { get; set; }
        public int FinalAccount { get; set; }
        public int? ParentId { get; set; }
        public string Notes { get; set; }
        public bool IsMain { get; set; }
        public int[]? BranchesId { get; set; }
        public int[] CostCenterId { get; set; }
        public bool withOutSaving { get; set; } = false;
    }
    public class UpdateFinancialAccountParameter
    {
        public int Id { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public int CurrencyId { get; set; }
        public int Status { get; set; }
        public int FA_Nature { get; set; }
        public int FinalAccount { get; set; }

        #region auto coding
        //public int? ParentId { get; set; }
        #endregion

        public string Notes { get; set; }
        public bool IsMain { get; set; }
        public int[] CostCenterId { get; set; }
        public int[]? BranchesId { get; set; }

    }
    
    
    public class MoveFinancial 
    {
        public int FinancialIdWillMove { get; set; }
        public int FinancialIdMovedTo { get; set; }
        public bool IsMainMoveTo { get; set; }
    }

    public class GetAllFinancialAccount
    {
        public int? Status { get; set; }
        public int? FA_Nature { get; set; }
        public int? CostCenter { get; set; }
        public string? SearchCriteria { get; set; }
    }
}
