using App.Domain.Entities.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Security.Authentication.Response.Store
{
   public class SalesManDto
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool ApplyToCommissions { get; set; }
        public int? CommissionListId { get; set; }
        public string CommissionListNameAr { get; set; }
        public string CommissionListNameEn { get; set; }

        public bool CanDelete { get; set; }
        public int[] Branches { get; set; }
        public string BranchNameAr { get; set; }
        public string BranchNameEn { get; set; }
        public string Notes { get; set; }
        public int? FinancialAccountId { get; set; }
        public int? SalesPriceId { get; set; }  


    }
}
