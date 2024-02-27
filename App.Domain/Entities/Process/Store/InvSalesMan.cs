using App.Domain.Common;
using App.Domain.Entities.POS;
using App.Domain.Entities.Process.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Entities.Process
{
   public class InvSalesMan 
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool ApplyToCommissions { get; set; }
        public int? CommissionListId { get; set; }
        //  public int Branch_Id { get; set; }
        public bool CanDelete { get; set; }
     //   public int BranchId { get; set; }
        public string Notes { get; set; }
        public int? SalesPriceId { get; set; }

        public InvCommissionList CommissionList { get; set; }
        public virtual ICollection<InvPersons> persons { get; set; }
        public ICollection<InvoiceMaster> InvoiceMaster { get; set; }
        public ICollection<POSInvoiceSuspension> POSInvoiceSuspension { get; set; }
        public virtual ICollection<InvSalesMan_Branches> SalesManBranch { get; set; }
        //relation to FinancialAccount
        public ICollection<GlReciepts> reciept { get; set; }
        public int? FinancialAccountId { get; set; }
        public virtual GLFinancialAccount FinancialAccount { get; set; }
        public virtual ICollection<OfferPriceMaster> OfferPriceMaster { get; set; }

    }
}
