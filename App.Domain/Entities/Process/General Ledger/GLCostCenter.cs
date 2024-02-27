using App.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLCostCenter : GeneralProperities
    {
        public GLCostCenter()
        {
            costCenters = new List<GLCostCenter>();
        }
        public string Code { get; set; }
        public double InitialBalance { get; set; } 
        public int CC_Nature { get; set; }
        public int Type { get; set; }
        public string Notes { get; set; }
        public int? ParentId { get; set; }
        public string BrowserName { get; set; }
        public string AutoCode { get; set; } // serial of paren code for child
        public virtual GLCostCenter Cost_Center { get; set; }
        public IReadOnlyCollection<GLCostCenter> costCenters { get; set; }
        public IReadOnlyCollection<GLRecieptCostCenter> supportCostCenters { get; set; }
        // public IReadOnlyCollection<FinancialCost> financialCosts { get; set; }
    }
}
