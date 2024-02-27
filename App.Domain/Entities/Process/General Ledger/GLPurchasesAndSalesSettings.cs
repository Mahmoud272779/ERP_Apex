using App.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLPurchasesAndSalesSettings
    {
        public int Id { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public int ReceiptElemntID { get; set; }
        public int RecieptsType { get; set; }
        public int MainType { get; set; }
        public int? FinancialAccountId { get; set; }
        public int branchId { get; set; }
        public GLBranch GLBranch { get; set; }
        public virtual GLFinancialAccount FinancialAccount { get; set; }

    }
}
