using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLRecieptCostCenter
    {
        public int Id { get; set; }
        public int CostCenterId { get; set; }
        public int SupportId { get; set; }
        public string CostCenteCode { get; set; }
        public string CostCenteName { get; set; }
        public double Percentage { get; set; }
        public double Number { get; set; }
        public virtual GLCostCenter CostCenter { get; set; }
        public virtual GlReciepts Reciept { get; set; }
    }
}
